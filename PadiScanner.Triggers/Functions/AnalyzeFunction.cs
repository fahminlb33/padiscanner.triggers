using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PadiScanner.Triggers.Data;
using PadiScanner.Triggers.Infra;

namespace PadiScanner.Triggers.Functions
{
    public class AnalyzeFunction
    {
        private readonly PadiDataContext _dbContext;
        private readonly IImageAnalysisService _imageAnalysisService;

        public AnalyzeFunction(PadiDataContext dbContext, IImageAnalysisService imageAnalysisService)
        {
            _dbContext = dbContext;
            _imageAnalysisService = imageAnalysisService;
        }

        [FunctionName("AnalyzeFunction")]
        public async Task Run([QueueTrigger("padi-process-queue", Connection = "AZURE_STORAGE_CONNECTION_STRING")] string jobId, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {jobId}");

            // get the job
            var job = await _dbContext.Predictions.FindAsync(Ulid.Parse(jobId));
            if (job == null)
            {
                log.LogInformation("Job not found: {0}, skipping...", jobId);
                return;
            }

            try
            {
                // update status to processing
                job.Status = PredictionStatus.Processing;
                job.ProcessedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                log.LogInformation("Starting job: {0}", job.Id);

                // run prediction
                var result = await _imageAnalysisService.Analyze(new PredictionRequest
                {
                    UserId = job.UploaderId.ToString(),
                    PredictionId = job.Id.ToString(),
                    OriginalFilename = Path.GetFileName(job.OriginalImageUrl.AbsolutePath)
                });

                // update status to success
                job.Status = PredictionStatus.Success;
                job.Result = result.PredictedClass;
                job.Probabilities = result.ClassProbabilities;
                job.ProcessedAt = DateTime.Now;
                job.HeatmapImageUrl = result.Heatmap;
                job.OverlayedImageUrl = result.Superimposed;
                job.ClippedImageUrl = result.Masked;

                await _dbContext.SaveChangesAsync();
                log.LogInformation("Job finished: {0}", job.Id);
            }
            catch (OperationCanceledException)
            {
                // update status to failed
                job.Status = PredictionStatus.Failed;
                job.ProcessedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                log.LogWarning("Job cancelled: {0}", job.Id);
            }
            catch (PadiException pe)
            {
                // update status to failed
                job.Status = PredictionStatus.Failed;
                job.ProcessedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                log.LogError(pe, "Job failed from service: {0}", job.Id);
            }
            catch (Exception ex)
            {
                // update status to failed
                job.Status = PredictionStatus.Failed;
                job.ProcessedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                log.LogError(ex, "Job failed: {0}", job.Id);
            }
        }
    }
}

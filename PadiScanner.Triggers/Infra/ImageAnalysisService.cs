using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PadiScanner.Triggers.Infra;

public interface IImageAnalysisService
{
    Task<PredictionResult> Analyze(PredictionRequest request);
}

public class ImageAnalysisService : IImageAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImageAnalysisService> _logger;

    public ImageAnalysisService(HttpClient httpClient, ILogger<ImageAnalysisService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PredictionResult> Analyze(PredictionRequest model)
    {
        try
        {
            var request = await _httpClient.PostAsJsonAsync("/analysis/image", model);
            request.EnsureSuccessStatusCode();

            return await request.Content.ReadFromJsonAsync<PredictionResult>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot run prediction");
            throw new PadiException("Tidak dapat memproses entri", ex);
        }
    }
}

public class PredictionRequest
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("prediction_id")]
    public string PredictionId { get; set; }

    [JsonPropertyName("original_filename")]
    public string OriginalFilename { get; set; }
}

public class PredictionResult
{
    [JsonPropertyName("predicted_class")]
    public string PredictedClass { get; set; }

    [JsonPropertyName("class_probabilities")]
    public Dictionary<string, double> ClassProbabilities { get; set; }

    [JsonPropertyName("heatmap")]
    public Uri Heatmap { get; set; }

    [JsonPropertyName("superimposed")]
    public Uri Superimposed { get; set; }

    [JsonPropertyName("masked")]
    public Uri Masked { get; set; }
}

public class ClassProbabilities
{
    [JsonPropertyName("BACTERIALBLIGHT")]
    public long Bacterialblight { get; set; }

    [JsonPropertyName("BLAST")]
    public long Blast { get; set; }

    [JsonPropertyName("BROWNSPOT")]
    public long Brownspot { get; set; }

    [JsonPropertyName("HEALTHY")]
    public long Healthy { get; set; }

    [JsonPropertyName("TUNGRO")]
    public long Tungro { get; set; }
}

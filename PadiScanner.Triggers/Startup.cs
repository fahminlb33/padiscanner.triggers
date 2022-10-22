using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PadiScanner.Triggers.Infra;
using System;
using System.Net.Http.Headers;
using System.Text;

[assembly: FunctionsStartup(typeof(PadiScanner.Triggers.Startup))]

namespace PadiScanner.Triggers;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContext<PadiDataContext>(c => c.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING")));
        builder.Services.AddHttpClient<IImageAnalysisService, ImageAnalysisService>(x =>
        {
            var credential = Environment.GetEnvironmentVariable("ANALYSIS_API_CREDENTIAL");
            var baseUrl = Environment.GetEnvironmentVariable("ANALYSIS_API_BASE_URL");
            var base64Cred = Convert.ToBase64String(Encoding.ASCII.GetBytes(credential));

            x.BaseAddress = new Uri(baseUrl);
            x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Cred);
            x.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PadiScanner", "1.0"));
        });
    }
}
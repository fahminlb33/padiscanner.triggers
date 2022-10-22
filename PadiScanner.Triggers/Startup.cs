using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PadiScanner.Triggers.Infra;
using System;

[assembly: FunctionsStartup(typeof(PadiScanner.Triggers.Startup))]

namespace PadiScanner.Triggers;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContext<PadiDataContext>(c => c.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING")));
        builder.Services.AddHttpClient<IImageAnalysisService, ImageAnalysisService>();
    }
}
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Infrastructure.Data;
using Infrastructure.IRepositery;
using Infrastructure.Repositery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var config = builder.Configuration;
var env = builder.Environment;

// Setup Serilog
var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day);

// Optional: AWS Logging
var awsAccessKey = config["AWS:AccessKey"];
var awsSecretKey = config["AWS:SecretKey"];
var awsRegion = config["AWS:Region"];
var awsLogGroup = config["AWS:LogGroup"];

if (!string.IsNullOrEmpty(awsAccessKey) && !string.IsNullOrEmpty(awsSecretKey))
{
    var awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
    var cloudWatchClient = new AmazonCloudWatchLogsClient(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(awsRegion));

    loggerConfig.WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
    {
        LogGroupName = awsLogGroup,
        TextFormatter = new Serilog.Formatting.Compact.CompactJsonFormatter()
    }, cloudWatchClient);
}
// Optional: Application Insights
if (!string.IsNullOrEmpty(config["ApplicationInsights:InstrumentationKey"]))
{
    builder.Services.AddApplicationInsightsTelemetry(config["ApplicationInsights:InstrumentationKey"]);
}
Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IProductsRepositery, ProductsRepository>();
builder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

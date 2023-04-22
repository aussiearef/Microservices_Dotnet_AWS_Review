using System.Text.Json;
using Amazon.SimpleNotificationService;
using BookingReview.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c=>
    c.AddDefaultPolicy(p=>
        p.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

app.MapPost("/approve", async (string? bookingId, int status) =>
{
    var topicArn = Environment.GetEnvironmentVariable("topicArn") ?? "arn:aws:sns:ap-southeast-2:867964100065:booking-approval-topic";
    using var client = new AmazonSimpleNotificationServiceClient();
    await client.PublishAsync(topicArn, JsonSerializer.Serialize(new ApprovalEvent()
    {
        Status = status,
        BookingId = bookingId
    }));
});

// Health Check 
app.MapGet("/", () => true);

app.Run();


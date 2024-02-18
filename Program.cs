using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Auth0.AspNetCore.Authentication;
using Twilio.TwiML;
using Twilio.AspNet.Core;
using Twilio.AspNet.Common;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

/*builder.Services.AddDbContext<TrackerContext>(options =>
    options.UseSqlite("Data Source=Tracker.db"));*/
    
builder.Services.AddDbContext<TrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrackerContext")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioConfig"));
builder.Services.Configure<Auth0Config>(builder.Configuration.GetSection("Auth0Config"));
builder.Services.Configure<PredictionConfig>(builder.Configuration.GetSection("PredictionConfig"));
builder.Services.Configure<GraphMailConfig>(builder.Configuration.GetSection("GraphMailConfig"));

builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<ICheckinService, CheckinService>();
builder.Services.AddScoped<ICheckpointService, CheckpointService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMonitorService, MonitorService>();
builder.Services.AddScoped<ILeaderService, LeaderService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IRaceService, RaceService>();
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<IWatcherService, WatcherService>();
builder.Services.AddScoped<IAlertMessageService, AlertMessageService>();
builder.Services.AddScoped<ITwilioService, TwilioService>();
builder.Services.AddScoped<IAuth0Service, Auth0Service>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddSingleton<SlackService>();
builder.Services.AddScoped<IGraphMailService, GraphMailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string Auth0Domain = builder.Configuration["Auth0Config:Domain"];
string Auth0ClientId = builder.Configuration["Auth0Config:AppClientId"];
string Auth0ClientSecret = builder.Configuration["Auth0Config:AppClientSecret"];
string Auth0Audience = builder.Configuration["Auth0Config:Audience"];

builder.Services.AddAuth0WebAppAuthentication(options => {
    options.Domain = Auth0Domain;
    options.ClientId = Auth0ClientId;
    options.ClientSecret = Auth0ClientSecret;
})
.WithAccessToken(options =>
  {
    options.Audience = Auth0Audience;
});

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("admin", p => p.RequireRole("Administrator"));
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Tracker API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var CorsAllowAny = "CorsAllowAny";
builder.Services.AddCors(options => {
    options.AddPolicy(name: CorsAllowAny, 
        policy => {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddRazorPages();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMigrationsEndPoint();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsAllowAny);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/seed", async (ISeedService seedService) =>
{
    await seedService.Initialize();
});

app.MapGet("/api/simulate", async (Guid raceId, int hourLimit, int numberOfParticipants, bool addCheckins, ISeedService seedService) =>
{
    await seedService.SimulateRace(raceId, numberOfParticipants, addCheckins, hourLimit);
});

app.MapGet("/api/races", async (IRaceService raceService) =>
{
    return await raceService.GetRacesAsync();
});

app.MapGet("/api/races/{raceId}", async (Guid raceId, IRaceService raceService) =>
{
    return await raceService.GetRaceAsync(raceId);
});

app.MapGet("/api/segments", async (ISegmentService segmentService) =>
{
    return await segmentService.GetSegmentsAsync();
});

app.MapGet("/api/segments/{raceId}", async (Guid raceId, ISegmentService segmentService) =>
{
    return await segmentService.GetSegmentsAsync(raceId);
});

app.MapGet("/api/checkpoints", async (ICheckpointService checkpointService) =>
{
    return await checkpointService.GetCheckpointsAsync();
});

app.MapGet("/api/checkpoints/{checkpointId}", async (Guid checkpointId, ICheckpointService checkpointService) =>
{
    return await checkpointService.GetCheckpointAsync(checkpointId);
});

app.MapGet("/api/participants", async (IParticipantService participantService) =>
{
    return await participantService.GetParticipantsAsync();
});

app.MapGet("/api/participants/{participantId}", async (Guid participantId, IParticipantService participantService) =>
{
    return await participantService.GetParticipantAsync(participantId);
});

app.MapGet("/api/participants/{participantId}/checkins", async (Guid participantId, ICheckinService checkinService) =>
{
    return await checkinService.GetCheckinsForParticipantAsync(participantId);
});

app.MapGet("/api/leaders", async (ILeaderService leaderService) => 
{
    return await leaderService.GetLeadersAsync();
});

app.MapGet("/api/leaders/{raceId}", async (Guid raceId, ILeaderService leaderService) => 
{
    return await leaderService.GetLeadersByRaceIdAsync(raceId);
});

app.MapGet("/api/checkins", async (ICheckinService checkinService) =>
{
    return await checkinService.GetCheckinsAsync();
});

app.MapGet("/api/checkins/{raceId}", async (Guid raceId, ICheckinService checkinService) =>
{
    return await checkinService.GetCheckinsAsync(raceId);
});

app.MapGet("/api/checkins/recent/{count}", async (int count, ICheckinService checkinService) =>
{
    return await checkinService.GetCheckinsAsync(count);
});

app.MapGet("/api/participants/sync", async (IRaceService raceService) => 
{
    await raceService.SyncParticipantsWithUltraSignup();
});

app.MapPost("/api/messages", async (HttpContext httpContext, IMessageService messageService) =>
{
    var message = await messageService.AddMessageAsync(httpContext);
    var responseBody = await messageService.HandleMessageAsync(message);
    
    var response = new MessagingResponse();
    response.Message(responseBody);
    return new TwiMLResult(response);
});
/*
app.MapGet("/api/mail", async (IParticipantService participantService) => 
{
    await participantService.SendParticipantProfileEmails();
}).RequireAuthorization("admin");

app.MapGet("/api/mail/{participantId}", async (Guid participantId, IParticipantService participantService) => 
{
    await participantService.SendParticipantProfileEmail(participantId);
}).RequireAuthorization("admin");
*/
app.Run();

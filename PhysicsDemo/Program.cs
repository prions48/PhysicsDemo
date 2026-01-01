using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using PhysicsDemo.Components;
using PhysicsDemo.Data.Email;
using PhysicsDemo.Data.Files;
using PhysicsDemo.Data.Users;
using Microsoft.EntityFrameworkCore;
using PhysicsDemo.Data.GameData;
using PhysicsDemo.Data.WebHooks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAuth0WebAppAuthentication(options => {
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ClientId"];
    });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddCircuitOptions(e => e.DetailedErrors = true);

builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailSettings,EmailSettings>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<IFileSettings,FileSettings>();
builder.Services.AddScoped<FileService>();

builder.Services.AddDbContext<UserContext>(o => o.UseSqlServer(builder.Configuration["ConnectionStrings:SQL"]));
builder.Services.AddScoped<UserService>();

builder.Services.AddDbContext<PhysicsContext>(o => o.UseSqlServer(builder.Configuration["ConnectionStrings:SQL"]));
builder.Services.AddScoped<PhysicsService>();

builder.Services.AddSingleton<WebHookService>();
builder.Services.AddSingleton<CircuitHandler, CircuitTracker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
  var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
          .WithRedirectUri("/")
          .Build();

  await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapPost("/webhook", async (HttpRequest request, WebHookService webhookService) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    // Validate signature if required
    string? signatureHeader = null;
    try
    {
        signatureHeader = request.Headers["X-Webhook-Signature"].FirstOrDefault();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Webook received with no webhook signature: {ex.Message}");
        return Results.BadRequest();
    }
    if (signatureHeader == null)
    {
        Console.WriteLine($"Webhook received but no signature found: {body}");
        return Results.Problem();
    }
    if (!webhookService.ValidateSignature(body, signatureHeader))
    {
        Console.WriteLine($"Webhook received but failed validation: {body}");
        return Results.Unauthorized();
    }
    // Process the webhook data
    webhookService.RaiseEvent(body);
    Console.WriteLine($"Webhook received: {body}");

    return Results.Ok();
});


app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

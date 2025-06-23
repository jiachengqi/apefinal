using apenew;
using apenew.Components;
using apenew.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using MudBlazor.Services;
using Radzen;
using Radzen.Blazor;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Sources.Clear();
    config.AddYamlFile("appsettings.yaml", optional: false, reloadOnChange: true);

    var env = hostingContext.HostingEnvironment;
    config.AddYamlFile($"appsettings.{env.EnvironmentName}.yaml", optional: true, reloadOnChange: true);
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMudServices();
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<AssessmentService>();
builder.Services.AddScoped<RadzenDialog>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

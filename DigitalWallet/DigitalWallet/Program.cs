using System.Globalization;

using DigitalWallet.Converters;
using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using DigitalWallet.Services;
using DigitalWallet.Services.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

using Stripe;

using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var keyVaultUri = Environment.GetEnvironmentVariable("VaultUri");
    if (keyVaultUri is not null)
    {
        var keyVaultEndpoint = new Uri(keyVaultUri);
        builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
    }
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddIdentity<Client, IdentityRole<Guid>>(o => o.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddScoped<WalletManager>();
builder.Services.AddScoped<TransactionManager>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSenderOptions>(builder.Configuration);

builder.Services.AddRazorPages()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter()));

StripeConfiguration.ApiKey = builder.Configuration["StripeKey"];

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var defaultCulture = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = [defaultCulture],
    SupportedUICultures = [defaultCulture]
};

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

using DigitalWallet.Data;
using DigitalWallet.Data.Models;

using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Helpers;

public static class DataSeeder
{
    public static async Task Seed(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var adminEmail = configuration["AdminAccountOptions:Email"];
        var adminPassword = configuration["AdminAccountOptions:Password"];

        ArgumentException.ThrowIfNullOrEmpty(adminEmail, nameof(adminEmail));
        ArgumentException.ThrowIfNullOrEmpty(adminPassword, nameof(adminPassword));

        var userManager = serviceProvider.GetRequiredService<UserManager<Client>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        var roleExist = await roleManager.RoleExistsAsync(StaticData.AdminRoleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new Role { Name = StaticData.AdminRoleName });
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new Client();

            await userManager.SetUserNameAsync(admin, adminEmail);
            await userManager.SetEmailAsync(admin, adminEmail);
            admin.EmailConfirmed = true;

            await userManager.CreateAsync(admin, adminPassword);
        }

        var isInRole = await userManager.IsInRoleAsync(admin, StaticData.AdminRoleName);
        if (!isInRole)
        {
            await userManager.AddToRoleAsync(admin, StaticData.AdminRoleName);
        }
    }
}

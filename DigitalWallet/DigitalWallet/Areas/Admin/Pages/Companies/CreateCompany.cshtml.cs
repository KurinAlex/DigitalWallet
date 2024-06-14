using DigitalWallet.Data.Models;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Areas.Admin.Pages.Companies;

[Authorize(Roles = StaticData.AdminRoleName)]
public class CreateCompanyModel(CompanyManager companyManager) : PageModel
{
    [BindProperty]
    public Company Company { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        var company = await companyManager.FindByNameAsync(Company.Name);
        if (company is not null)
        {
            ModelState.AddModelError("Company.Name", "Company with this name already exists.");
            return Page();
        }

        await companyManager.CreateAsync(Company);
        return RedirectToPage("Companies");
    }
}

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Areas.Admin.Pages.Companies;

[Authorize(Roles = StaticData.AdminRoleName)]
public class EditCompanyModel(CompanyManager companyManager) : PageModel
{
    [BindProperty]
    public Company Company { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var company = await companyManager.FindByIdAsync(id);
        if (company is null)
        {
            return ActionResultHelper.GetEntityNotFoundResult("Company");
        }

        Company = company;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var company = await companyManager.FindByNameAsync(Company.Name);
        if (company is not null && company.Id != Company.Id)
        {
            ModelState.AddModelError("Company.Name", "Company with this name already exists.");
            return Page();
        }

        await companyManager.UpdateAsync(Company);
        return RedirectToPage("Companies");
    }
}

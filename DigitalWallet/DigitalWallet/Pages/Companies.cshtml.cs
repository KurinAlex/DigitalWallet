using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

[Authorize(Roles = StaticData.AdminRoleName)]
public class CompaniesModel(CompanyManager companyManager) : PageModel
{
    public async Task<IActionResult> OnGetAllCompaniesAsync()
    {
        var companies = await companyManager.GetAllAsync();
        return new JsonResult(companies);
    }
}

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class SupportDataModel : PageModel
{
    [EmailAddress] public string Email { get; set; } = "support@digital-wallet.com";

    [Phone]
    public string Phone { get; set; } = "+380-44-000-00-00";

    public string Address { get; set; } = "123 Shevchenko Street, 12345 Kyiv, Ukraine";
}

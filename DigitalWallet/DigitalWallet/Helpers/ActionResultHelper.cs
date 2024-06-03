using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Helpers;

public static class ActionResultHelper
{
    public static IActionResult GetClientNotFoundResult()
    {
        return GetEntityNotFoundResult("Current client");
    }

    public static IActionResult GetEntityNotFoundResult(string entityName)
    {
        return new NotFoundObjectResult($"{entityName} not found.");
    }

    public static IActionResult GetClientDoesNotHaveWalletResult()
    {
        return new BadRequestObjectResult("You can't do this operation without a wallet.");
    }
}

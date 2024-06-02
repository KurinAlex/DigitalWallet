using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Models;

public class Client : IdentityUser<Guid>, IEntity
{
}

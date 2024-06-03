using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Data.Models;

public class Client : IdentityUser<Guid>, IEntity;

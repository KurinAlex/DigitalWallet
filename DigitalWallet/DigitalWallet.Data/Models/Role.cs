using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Data.Models;

public class Role : IdentityRole<Guid>, IEntity;

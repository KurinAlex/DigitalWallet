using DigitalWallet.Data;
using DigitalWallet.Data.Models;

namespace DigitalWallet.Services.Managers;

public class CompanyManager(ApplicationDbContext dbContext) : Manager<Company>(dbContext);

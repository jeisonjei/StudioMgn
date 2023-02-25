
namespace StudioMgn.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbInitializer:IDisposable{
    private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
    private readonly UserManager<IdentityUser> userManager;
    private readonly ILogger<DbInitializer> logger;

    public DbInitializer(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager, ILogger<DbInitializer> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.userManager = userManager;
        this.logger = logger;
    }
    public async Task CreateAdminUserAsync(){
        string adminEmail="admin@primer.ru";
        var user=await userManager.FindByEmailAsync(adminEmail);
        if (user==null)
        {
            user = new IdentityUser(adminEmail);
            user.Email=adminEmail;
            user.EmailConfirmed=true;
            var result =await userManager.CreateAsync(user, "123qwe!@#");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError(error.Description);
                }
                throw new Exception("=== Ошибка");
            }
            logger.LogInformation($"=== Пользователь {adminEmail} успешно создан");
        }
    }

    public void Dispose()
    {
    }
}

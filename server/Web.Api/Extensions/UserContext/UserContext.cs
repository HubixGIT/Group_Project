using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Web.Api.Database;
using Web.Api.Entities;

namespace Web.Api.Extensions.UserContext;

public class UserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDBContext _dbContext;
    public UserContext(IHttpContextAccessor httpContextAccessor, ApplicationDBContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }
    
    public async Task<User?> GetLoggedUser(CancellationToken cancellationToken)
    {
        var userId = Guid.Empty;
        var subjectClaim = _httpContextAccessor.HttpContext?.User;
       

        return await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
    }
}
namespace Web.Api.Extensions.CurrentUserService;

public interface ICurrentUserService
{
    public Guid UserId { get; }

    public string UserEmail { get; }
    
    public bool IsAuthenticated { get; }
}
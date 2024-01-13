namespace Web.Api.Entities;

public class UserProject
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
    public UserProjectRankEnum Rank { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
    
}
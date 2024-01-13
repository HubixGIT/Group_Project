namespace Web.Api.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
    public virtual List<UserProject> UserProjects { get; set; }

}
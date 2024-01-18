namespace Web.Api.Dtos.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserProjectDto> Participants { get; set; }
}
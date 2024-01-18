using Web.Api.Entities;

namespace Web.Api.Dtos.Project;

public class UserProjectDto
{
    public int Id { get; set; }
    public UserProjectRankEnum Rank { get; set; }
    public UserDto User { get; set; }
}
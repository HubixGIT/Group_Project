namespace Web.Api.Entities;

public class User
{
   public Guid Id { get; set; }
   public string Email { get; set; }
   public string Password { get; set; }
   public string FullName { get; set; }
   public DateTime CreatedOnUtc { get; set; }
}
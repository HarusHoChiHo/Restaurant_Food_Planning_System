namespace Application.Dtos.User;

public class UserQueryDto : UserFullDto
{
    public IList<string>? Role { get; set; }
}
namespace Application.Dtos.User
{
    public class UserResultDto
    {
        public int           Id       { get; set; }
        public string        UserName { get; set; }
        public string        Password { get; set; }
        public IList<string> Role     { get; set; }
    }
}
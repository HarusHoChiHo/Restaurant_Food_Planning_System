namespace Application.ResponseDto;

public class UserResDto<T> : DbOperationResult<T>
{
    public string Token { get; set; }
}
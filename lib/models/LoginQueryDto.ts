interface ILoginQueryDto{
    UserName: string,
    Password: string
}


class LoginQueryDto implements ILoginQueryDto {
    Password: string;
    UserName: string;
    
    constructor(username: string,  password: string) {
        this.UserName = username;
        this.Password = password;
    }
}


export  default LoginQueryDto;
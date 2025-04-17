interface IUserInfo {
    userName: string;
    role: string[];
}

class UserInfo implements IUserInfo {
    userName: string;
    role: string[];

    constructor(userName: string, role: string[]) {
        this.userName = userName;
        this.role = role;
    }
}

export default UserInfo;
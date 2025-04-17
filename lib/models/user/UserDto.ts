interface IUserDto {
    userName: string,
    password: string,
    role: string[]
    id: number
}

export const userHeaders = [{
    key  : "id",
    label: "ID"
}, {
    key  : "username",
    label: "User name"
}, {
    key  : "password",
    label: "Password"
}, {
    key  : "role",
    label: "Role"
}, {
    key  : "delete",
    label: "Delete"
}, {
    key  : "edit",
    label: "Edit"
}];

class UserDto implements IUserDto {
    password: string;
    userName: string;
    role: string[];
    id: number;

    constructor(username: string, password: string, id: number, role: string[]) {
        this.userName = username;
        this.password = password;
        this.role = role;
        this.id   = id;
    }
}

export default UserDto;
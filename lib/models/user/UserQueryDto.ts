import {id} from "postcss-selector-parser";

interface IUserQueryDto {
    userName: string | null;
    password: string | null;
    role: string[] | [];
    id: number | null;
}

class UserQueryDto implements IUserQueryDto {
    userName: string | null;
    password: string | null;
    role: string[] | [];
    id: number | null;


    constructor(userName: string | null, password: string | null, role: string[] | [], id: number | null) {
        this.userName = userName;
        this.password = password;
        this.role = role;
        this.id = id;
    }
}

export default UserQueryDto;
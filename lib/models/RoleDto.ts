import {create} from "node:domain";

interface IRoleDto {
    name: string;
    description: string;
    id: number;
    createdDate: string;
}

class RoleDto implements IRoleDto {
    name: string;
    description: string;
    id: number;
    createdDate: string;

    constructor(name: string, description: string, id: number, createdDate: string) {
        this.name = name;
        this.description = description;
        this.id = id;
        this.createdDate = createdDate;
    }
}

export default RoleDto;
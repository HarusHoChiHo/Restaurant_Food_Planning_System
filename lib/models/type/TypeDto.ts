interface ITypeDto {
    id: number;
    name: string;
}

export const typeHeaders = [{
    key  : "id",
    label: "ID"
}, {
    key  : "name",
    label: "Name"
}, {
    key  : "delete",
    label: "Delete"
}, {
    key  : "edit",
    label: "Edit"
}];

export const typeHeadersStaff = [{
    key  : "id",
    label: "ID"
}, {
    key  : "name",
    label: "Name"
}];

class TypeDto implements ITypeDto {
    id: number;
    name: string;
    
    constructor(id: number, name: string) {
        this.id = id;
        this.name = name;
    }
}

export default TypeDto;
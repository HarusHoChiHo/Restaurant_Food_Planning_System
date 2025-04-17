interface IMenuItemDto {
    id: number;
    name: string;
}

export const menuItemHeaders = [{
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

export const menuItemHeadersStaff = [{
    key  : "id",
    label: "ID"
}, {
    key  : "name",
    label: "Name"
}];

class MenuItemDto implements IMenuItemDto {
    id: number;
    name: string;
    
    constructor(menuItem_Id: number, name: string) {
        this.id = menuItem_Id;
        this.name = name;
    }
}

export default MenuItemDto;
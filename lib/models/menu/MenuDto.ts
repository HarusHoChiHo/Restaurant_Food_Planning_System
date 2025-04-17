import MenuItemDto from "./MenuItemDto";

interface IMenuDto {
    id: number;
    date: Date;
    menuItem: MenuItemDto;
}

export const menuHeaders = [{
    key  : "id",
    label: "ID"
}, {
    key  : "date",
    label: "Date"
}, {
    key  : "item",
    label: "Item"
}, {
    key  : "delete",
    label: "Delete"
}, {
    key  : "edit",
    label: "Edit"
}];

export const menuHeadersStaff = [{
    key  : "id",
    label: "ID"
}, {
    key  : "date",
    label: "Date"
}, {
    key  : "item",
    label: "Item"
}];

class MenuDto implements IMenuDto {
    date: Date;
    id: number;
    menuItem: MenuItemDto;
    
    
    constructor(date: Date, id: number, menuItem: MenuItemDto) {
        this.date        = new Date(date);
        this.id          = id;
        this.menuItem = menuItem;
    }
}

export default MenuDto;
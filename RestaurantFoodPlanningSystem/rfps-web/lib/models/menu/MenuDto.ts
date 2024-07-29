import MenuItemDto from "./MenuItemDto";

interface IMenuDto {
    id: number;
    date: Date;
    menuItem: MenuItemDto;
}

export const menuHeaders = ["ID", "Date", "Item"];

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
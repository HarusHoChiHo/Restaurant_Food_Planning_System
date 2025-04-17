import {id} from "postcss-selector-parser";

interface IMenuQueryDto {
    id: number | null;
    date: string | null;
    menuItem_Id: number | null;
}

class MenuQueryDto implements IMenuQueryDto {
    id: number | null;
    date: string | null;
    menuItem_Id: number | null;


    constructor(id: number | null, date: string | null, menuItem_Id: number | null) {
        this.id          = id;
        this.date        = date;
        this.menuItem_Id = menuItem_Id;
    }
}

export default MenuQueryDto;
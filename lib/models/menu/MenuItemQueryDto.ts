interface IMenuItemQueryDto {
    id: number | null;
    name: string | null;
}

class MenuItemQueryDto implements IMenuItemQueryDto {
    id: number | null;
    name: string | null;


    constructor(id: number | null, name: string | null) {
        this.id   = id;
        this.name = name;
    }
}
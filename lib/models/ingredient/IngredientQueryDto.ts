interface IIngredientQueryDto {
    id: number | null;
    name: string | null;
    quantity: number | null;
    unit_Id: number | null;
    type_Id: number | null
}

class IngredientQueryDto implements IIngredientQueryDto {
    id: number | null;
    name: string | null;
    quantity: number | null;
    type_Id: number | null;
    unit_Id: number | null;

    constructor(id: number | null, name: string | null, quantity: number | null, type_Id: number | null, unit_Id: number | null) {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.type_Id = type_Id;
        this.unit_Id = unit_Id;
    }
}

export default IngredientQueryDto;
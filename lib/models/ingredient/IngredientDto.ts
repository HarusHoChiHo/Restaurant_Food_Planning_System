import UnitDto from "../unit/UnitDto";
import TypeDto from "../type/TypeDto";

interface IIngredientDto {
    id: number;
    name: string;
    quantity: number;
    unit: UnitDto;
    type: TypeDto
}

export const ingredientHeaders = [{
    key  : "id",
    label: "ID"
}, {
    key  : "name",
    label: "Name"
}, {
    key  : "quantity",
    label: "Quantity"
}, {
    key  : "unit",
    label: "Unit"
}, {
    key  : "type",
    label: "Type"
}, {
    key  : "delete",
    label: "Delete"
}, {
    key  : "edit",
    label: "Edit"
}];

export const ingredientHeadersStaff = [{
    key  : "id",
    label: "ID"
}, {
    key  : "name",
    label: "Name"
}, {
    key  : "quantity",
    label: "Quantity"
}, {
    key  : "unit",
    label: "Unit"
}, {
    key  : "type",
    label: "Type"
}];

class IngredientDto implements IIngredientDto {
    id: number;
    name: string;
    quantity: number;
    unit: UnitDto;
    type: TypeDto;
    
    constructor(id: number, name: string, quantity: number, unit: UnitDto, type: TypeDto) {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.unit = unit;
        this.type = type;
    }
}

export default IngredientDto;
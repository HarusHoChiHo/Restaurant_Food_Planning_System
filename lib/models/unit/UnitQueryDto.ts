import {id} from "postcss-selector-parser";

interface IUnitQueryDto {
    id: number | null;
    name: string | null;
}

class UnitQueryDto implements IUnitQueryDto {
    id: number | null;
    name: string | null;

    constructor(id: number | null, name: string | null) {
        this.id = id;
        this.name = name;
    }
}

export default UnitQueryDto;
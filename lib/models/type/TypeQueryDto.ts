interface ITypeQueryDto {
    id: number | null;
    name: string | null;
}

class TypeQueryDto implements ITypeQueryDto {
    id: number | null;
    name: string | null;

    constructor(id: number | null, name: string | null) {
        this.id = id;
        this.name = name;
    }
}

export default TypeQueryDto;
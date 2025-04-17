
interface IOrderQueryDto {
    id: number | null;
    isCanceled: boolean | null;
}

interface IOrderQueryPerPageDto {
    pageNumber: number | null;
    limit: number | null;
}

export class OrderQueryDto implements IOrderQueryDto {
    id: number | null;
    isCanceled: boolean | null;

    constructor(id: number | null, isCanceled: boolean | null) {
        this.id = id;
        this.isCanceled = isCanceled;
    }
}

export class OrderQueryPerPageDto implements IOrderQueryDto, IOrderQueryPerPageDto {
    pageNumber: number | null;
    limit: number | null;
    id: number | null;
    isCanceled: boolean | null;
    
    constructor(pageNumber: number | null, limit: number | null, id: number | null, isCanceled: boolean | null) {
        this.pageNumber = pageNumber;
        this.limit = limit;
        this.id = id;
        this.isCanceled = isCanceled;
    }
}
import OrderItemDto from "./OrderItemDto";

interface IOrderPlacementDto {
    id: number;
    isCanceled: boolean;
    orderItems: OrderItemDto[];
}

class OrderPlacementDto implements IOrderPlacementDto {
    id: number;
    isCanceled: boolean;
    orderItems: OrderItemDto[];

    constructor(id: number, isCanceled: boolean, orderItems: OrderItemDto[]) {
        this.id = id;
        this.isCanceled = isCanceled;
        this.orderItems = orderItems;
    }
}

export default OrderPlacementDto;
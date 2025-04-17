import OrderItemQueryDto from "./OrderItemQueryDto";
import OrderQueryDto from "./OrderQueryDto";

interface IOrderPlacementQueryDto {
    order: OrderQueryDto;
    orderItems: OrderItemQueryDto[];
}

class OrderPlacementQueryDto implements IOrderPlacementQueryDto {
    order: OrderQueryDto;
    orderItems: OrderItemQueryDto[];

    constructor(order: OrderQueryDto, orderItems: OrderItemQueryDto[]) {
        this.order = order;
        this.orderItems = orderItems;
    }
}

export default OrderPlacementQueryDto;
import OrderItemDto from "./OrderItemDto";

interface IOrderDto {
    id: number;
    isCanceled: boolean;
    orderItems: OrderItemDto[];
}

export const orderHeaders = [{
    key  : "id",
    label: "ID"
}, {
    key  : "cancelled",
    label: "Cancelled?"
}, {
    key  : "items",
    label: "Items"
}, {
    key  : "delete",
    label: "Delete"
}, {
    key  : "edit",
    label: "Edit"
}];

export const orderHeadersStaff = [{
    key  : "id",
    label: "ID"
}, {
    key  : "cancelled",
    label: "Cancelled?"
}, {
    key  : "items",
    label: "Items"
}];

class OrderDto implements IOrderDto {
    id: number;
    isCanceled: boolean;
    orderItems: OrderItemDto[];

    constructor(id: number, isCanceled: boolean, orderItems: OrderItemDto[]) {
        this.id = id;
        this.isCanceled = isCanceled;
        this.orderItems = orderItems;
    }
}

export default OrderDto;
import MenuItemDto from "../menu/MenuItemDto";

interface IOrderItemQueryDto {
    id: number | null;
    orderId: number | null;
    menuItemId: number;
}

class OrderItemQueryDto implements IOrderItemQueryDto {
    id: number | null;
    orderId: number | null;
    menuItemId: number;

    constructor(id: number | null, orderId: number | null, menuItemId: number) {
        this.id = id;
        this.orderId = orderId;
        this.menuItemId = menuItemId;
    }
}

export default OrderItemQueryDto;
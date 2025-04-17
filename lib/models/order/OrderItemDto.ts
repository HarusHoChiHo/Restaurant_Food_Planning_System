import MenuItemDto from "../menu/MenuItemDto";

interface IOrderItemDto {
    id: number;
    orderId: number;
    menuItem: MenuItemDto;
}

class OrderItemDto implements IOrderItemDto {
    id: number;
    orderId: number;
    menuItem: MenuItemDto;
    
    constructor(id: number, orderId: number, menuItemId: MenuItemDto) {
        this.id = id;
        this.orderId = orderId;
        this.menuItem = menuItemId;
    }
}

export default OrderItemDto;
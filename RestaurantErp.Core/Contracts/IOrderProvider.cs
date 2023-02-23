namespace RestaurantErp.Core
{
    public interface IOrderProvider
    {
        Guid CreateOrder();

        void AddItem(OrderItemRequest request);

        void CancelItem(OrderItemRequest request);

        Bill Checkout(Guid orderId);
    }
}
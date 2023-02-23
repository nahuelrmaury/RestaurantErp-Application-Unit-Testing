using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Order;

namespace RestaurantErp.Core.Contracts
{
    public interface IOrderProvider
    {
        Guid CreateOrder();

        void AddItem(OrderItemRequest request);

        void CancelItem(OrderItemRequest request);

        BillExternal Checkout(Guid orderId);
    }
}
using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Providers;

namespace RestaurantErp.Core.Helpers
{
    public class BillHelper
    {
        private readonly IProductProvider _productProvider;

        public BillHelper(IProductProvider productProvider)
        {
            _productProvider = productProvider;
        }

        public BillExternal GetExternalBill(Bill internalBill)
        {
            var items = internalBill.Items.Select(i => new BillItemExternal
            {
                Amount = i.Amount,
                AmountDiscounted = i.AmountDiscounted,
                Discount = i.Discount,
                PersonId = i.PersonId,
                ProductName = _productProvider.GetProductInfo(i.ProductId).Name
            })
            .ToArray();

            var bill = new BillExternal
            {
                Amount = internalBill.Amount,
                Discount = internalBill.Discount,
                AmountDiscounted = internalBill.AmountDiscounted,
                OrderId = internalBill.OrderId,
                Items = items,
                Service = internalBill.Service,
                Total = internalBill.Total
            };

            return bill;
        }
    }
}

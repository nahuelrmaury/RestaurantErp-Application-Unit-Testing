namespace RestaurantErp.Core
{
    public interface IDiscountProvider
    {
        BillDiscountInfo Calculate(Order order); 
    }
}

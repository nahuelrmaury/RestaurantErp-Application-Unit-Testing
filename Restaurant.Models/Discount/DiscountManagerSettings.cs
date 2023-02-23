namespace RestaurantErp.Core.Models.Discount
{
    public class DiscountCalculatorSettings
    {
        // Due to local laws,
        // because for example in Ukraine product price cannot be less then 0.01 UAH.
        // If price is 0,00 then it will be not selling but gifting and it cannot be provided by usual bill.
        public decimal MinimalProductPrice { get; set; }
    }
}

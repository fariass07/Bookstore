
namespace Bookstore.Models.ViewModels
{
    public class SellerDetailsViewModel
    {
        public Seller Seller { get; set; }
        public ICollection<Sales> RecentSales => FoundRecentSales();
        public ICollection<Sales> BiggestSales => FoundBiggestSales();

        private ICollection<Sales> FoundRecentSales()
        {
            return Seller.Sales.OrderByDescending(x => x.Date).Take(5).ToList();
        }

        private ICollection<Sales> FoundBiggestSales()
        {
            return Seller.Sales.OrderByDescending(x => x.Amount).Take(5).ToList();
        }
    }
}

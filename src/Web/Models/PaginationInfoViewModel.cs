namespace Web.Models
{
    public class PaginationInfoViewModel
    {
        public int PageId { get; set; } // anlık ksayfa
        public int TotalItems { get; set; }
        public int ItemsOnPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)Constants.ITEMS_PER_PAGE); // toplam sayfa sayısı
        public bool HasPrevious => PageId > 1; // öncesi var mı
        public bool HasNext => PageId < TotalPages; // sonrası var mı
        public int RangeStart => (PageId - 1) * Constants.ITEMS_PER_PAGE + 1; // sayfanın ilk ürünün sırası
        public int RangeEnd => RangeStart + ItemsOnPage - 1; // sayfanın son ürün sırası
    }
}

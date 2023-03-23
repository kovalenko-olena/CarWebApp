using CarWebApp.ViewModels;

namespace CarWebApp.Infrastructure
{
    public class WayBillListViewModel
    {
        public IEnumerable<WayBillViewModel>? WayBillViews { get; set; }
        public PagingInfo? PagingInfo { get; set; }
    }
}

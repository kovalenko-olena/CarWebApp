using CarWebApp.Models;
using CarWebApp.ViewModels;

namespace CarWebApp.Repository
{
    public interface IWayBillRepository
    {
        Task<IEnumerable<WayBill>> GetAllItems();
        Task<WayBill> GetItem(int Id);
        Task<bool> FindItem(int Id);
		Task<WayBill> Add(WayBill item);
		Task Delete(int id);
        Task<WayBill> Update(WayBill itemChanges);
        Task<int> GetLastId();
		Task<int> GetLastCd();
        Task<decimal> GetLastFuel(VehicleSpr vehicle);
        Task<int> GetLastSpd(VehicleSpr vehicle);
    }
}

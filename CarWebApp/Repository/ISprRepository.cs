using CarWebApp.Models;
using CarWebApp.ViewModels;

namespace CarWebApp.Repository
{
    public interface ISprRepository<T>
    {
        Task<IEnumerable<T>> GetAllItems();
        Task<T> GetItem(int Id);
        Task<bool> FindItem(int Id);
		Task<T> Add(T item);
		Task Delete(int id);
        Task<T> Update(T itemChanges);
        Task<int> GetLastId();
		Task<int> GetLastCd();
    }
}

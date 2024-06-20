using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Interfaces
{
    public interface IResourceRepository
    {
        Task<IEnumerable<Resource>> GetResourcesAsync();
        Task<Resource> GetResourceByIdAsync(int id);
        Task<bool> ResourceExistsAsync(int id);
        Task AddResourceAsync(Resource resource);
        Task UpdateResourceAsync(Resource resource);
        Task DeleteResourceAsync(Resource resource);
        Task<bool> SaveAsync();
    }
}

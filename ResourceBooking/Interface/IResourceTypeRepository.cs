using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Interfaces
{
    public interface IResourceTypeRepository
    {
        Task<IEnumerable<ResourceType>> GetResourceTypesAsync();
        Task<ResourceType> GetResourceTypeByIdAsync(int id);
        Task AddResourceTypeAsync(ResourceType resourceType);
        Task UpdateResourceTypeAsync(ResourceType resourceType);
        Task DeleteResourceTypeAsync(ResourceType resourceType);
        Task<bool> SaveAsync();
    }
}

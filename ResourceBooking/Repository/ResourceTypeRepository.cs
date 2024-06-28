using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceBooking.Repositories
{
    public class ResourceTypeRepository : IResourceTypeRepository
    {
        private readonly DataContext _context;

        public ResourceTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResourceType>> GetResourceTypesAsync()
        {
            return await _context.ResourceTypes.ToListAsync();
        }

        public async Task<ResourceType> GetResourceTypeByIdAsync(int resourceTypeId)
        {
            return await _context.ResourceTypes.FindAsync(resourceTypeId);
        }

        public async Task<ResourceType> CreateResourceTypeAsync(ResourceType resourceType)
        {
            _context.ResourceTypes.Add(resourceType);
            await _context.SaveChangesAsync();
            return resourceType;
        }

        public async Task<ResourceType> UpdateResourceTypeAsync(ResourceType resourceType)
        {
            _context.ResourceTypes.Update(resourceType);
            await _context.SaveChangesAsync();
            return resourceType;
        }

        public async Task<bool> DeleteResourceTypeAsync(int resourceTypeId)
        {
            var resourceType = await _context.ResourceTypes.FindAsync(resourceTypeId);
            if (resourceType == null)
            {
                return false;
            }

            _context.ResourceTypes.Remove(resourceType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

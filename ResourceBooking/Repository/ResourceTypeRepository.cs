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

        public async Task<ResourceType> GetResourceTypeByIdAsync(int id)
        {
            return await _context.ResourceTypes.FindAsync(id);
        }

        public async Task AddResourceTypeAsync(ResourceType resourceType)
        {
            await _context.ResourceTypes.AddAsync(resourceType);
        }

        public async Task UpdateResourceTypeAsync(ResourceType resourceType)
        {
            _context.Entry(resourceType).State = EntityState.Modified;
        }

        public async Task DeleteResourceTypeAsync(ResourceType resourceType)
        {
            _context.ResourceTypes.Remove(resourceType);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

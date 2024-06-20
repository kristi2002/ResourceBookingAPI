using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceBooking.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly DataContext _context;

        public ResourceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetResourcesAsync()
        {
            return await _context.Resources.Include(r => r.ResourceType).ToListAsync();
        }

        public async Task<Resource> GetResourceByIdAsync(int id)
        {
            return await _context.Resources.Include(r => r.ResourceType).SingleOrDefaultAsync(r => r.ResourceId == id);
        }

        public async Task<bool> ResourceExistsAsync(int id)
        {
            return await _context.Resources.AnyAsync(r => r.ResourceId == id);
        }

        public async Task AddResourceAsync(Resource resource)
        {
            await _context.Resources.AddAsync(resource);
        }

        public async Task UpdateResourceAsync(Resource resource)
        {
            _context.Entry(resource).State = EntityState.Modified;
        }

        public async Task DeleteResourceAsync(Resource resource)
        {
            _context.Resources.Remove(resource);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

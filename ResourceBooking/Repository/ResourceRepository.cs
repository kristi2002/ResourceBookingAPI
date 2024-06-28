using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Resource> GetResourceByIdAsync(int resourceId)
        {
            return await _context.Resources.Include(r => r.ResourceType).FirstOrDefaultAsync(r => r.ResourceId == resourceId);
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<Resource> UpdateResourceAsync(Resource resource)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<bool> DeleteResourceAsync(int resourceId)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null)
            {
                return false;
            }

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync(DateTime startDate, DateTime endDate, int? resourceId = null)
        {
            var query = _context.Resources.Include(r => r.Bookings).AsQueryable();

            if (resourceId.HasValue)
            {
                query = query.Where(r => r.ResourceId == resourceId);
            }

            return await query.Where(r => !r.Bookings.Any(b => b.StartDate < endDate && b.EndDate > startDate)).ToListAsync();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceTypeRepository _resourceTypeRepository;

        public ResourceController(IResourceRepository resourceRepository, IResourceTypeRepository resourceTypeRepository)
        {
            _resourceRepository = resourceRepository;
            _resourceTypeRepository = resourceTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceRepository.GetResourcesAsync();
            var resourcesToReturn = resources.Select(r => new ResourceDto
            {
                ResourceId = r.ResourceId,
                Nome = r.Name,
                ResourceTypeName = r.ResourceType.TypeName
            });
            return Ok(resourcesToReturn);
        }

        [HttpGet("{id}", Name = "GetResource")]
        public async Task<IActionResult> GetResource(int id)
        {
            var resource = await _resourceRepository.GetResourceByIdAsync(id);
            if (resource == null)
                return NotFound();

            var resourceToReturn = new ResourceDto
            {
                ResourceId = resource.ResourceId,
                Nome = resource.Name,
                ResourceTypeName = resource.ResourceType.TypeName
            };

            return Ok(resourceToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResource(ResourceForCreationDto resourceForCreation)
        {
            var resource = new Resource
            {
                Name = resourceForCreation.Nome,
                ResourceTypeId = resourceForCreation.ResourceTypeId
            };

            await _resourceRepository.AddResourceAsync(resource);
            if (await _resourceRepository.SaveAsync())
                return CreatedAtRoute("GetResource", new { id = resource.ResourceId }, resource);

            return BadRequest("Error creating resource");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(int id, ResourceForCreationDto resourceForUpdate)
        {
            var resourceFromRepo = await _resourceRepository.GetResourceByIdAsync(id);
            if (resourceFromRepo == null)
                return NotFound();

            resourceFromRepo.Name = resourceForUpdate.Nome;
            resourceFromRepo.ResourceTypeId = resourceForUpdate.ResourceTypeId;

            await _resourceRepository.UpdateResourceAsync(resourceFromRepo);
            if (await _resourceRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error updating resource");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            var resourceFromRepo = await _resourceRepository.GetResourceByIdAsync(id);
            if (resourceFromRepo == null)
                return NotFound();

            await _resourceRepository.DeleteResourceAsync(resourceFromRepo);
            if (await _resourceRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error deleting resource");
        }
    }
}

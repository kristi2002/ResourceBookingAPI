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
    public class ResourceTypeController : ControllerBase
    {
        private readonly IResourceTypeRepository _resourceTypeRepository;

        public ResourceTypeController(IResourceTypeRepository resourceTypeRepository)
        {
            _resourceTypeRepository = resourceTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetResourceTypes()
        {
            var resourceTypes = await _resourceTypeRepository.GetResourceTypesAsync();
            var resourceTypesToReturn = resourceTypes.Select(rt => new ResourceTypeDto
            {
                ResourceTypeId = rt.ResourceTypeId,
                TypeName = rt.TypeName
            });
            return Ok(resourceTypesToReturn);
        }

        [HttpGet("{id}", Name = "GetResourceType")]
        public async Task<IActionResult> GetResourceType(int id)
        {
            var resourceType = await _resourceTypeRepository.GetResourceTypeByIdAsync(id);
            if (resourceType == null)
                return NotFound();

            var resourceTypeToReturn = new ResourceTypeDto
            {
                ResourceTypeId = resourceType.ResourceTypeId,
                TypeName = resourceType.TypeName
            };

            return Ok(resourceTypeToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResourceType(ResourceTypeForCreationDto resourceTypeForCreation)
        {
            var resourceType = new ResourceType
            {
                TypeName = resourceTypeForCreation.TypeName
            };

            await _resourceTypeRepository.AddResourceTypeAsync(resourceType);
            if (await _resourceTypeRepository.SaveAsync())
                return CreatedAtRoute("GetResourceType", new { id = resourceType.ResourceTypeId }, resourceType);

            return BadRequest("Error creating resource type");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResourceType(int id, ResourceTypeForCreationDto resourceTypeForUpdate)
        {
            var resourceTypeFromRepo = await _resourceTypeRepository.GetResourceTypeByIdAsync(id);
            if (resourceTypeFromRepo == null)
                return NotFound();

            resourceTypeFromRepo.TypeName = resourceTypeForUpdate.TypeName;

            await _resourceTypeRepository.UpdateResourceTypeAsync(resourceTypeFromRepo);
            if (await _resourceTypeRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error updating resource type");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceType(int id)
        {
            var resourceTypeFromRepo = await _resourceTypeRepository.GetResourceTypeByIdAsync(id);
            if (resourceTypeFromRepo == null)
                return NotFound();

            await _resourceTypeRepository.DeleteResourceTypeAsync(resourceTypeFromRepo);
            if (await _resourceTypeRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error deleting resource type");
        }
    }
}

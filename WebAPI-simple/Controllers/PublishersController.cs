using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublishersController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-publishers")]
        public async Task<IActionResult> GetAllPublishers()
        {
            var publishers = await _publisherRepository.GetAllPublishersAsync();
            return Ok(publishers);
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public async Task<IActionResult> GetPublisherById([FromRoute] int id)
        {
            var publisher = await _publisherRepository.GetPublisherByIdAsync(id);
            if (publisher == null)
                return NotFound($"Publisher with Id = {id} not found");

            return Ok(publisher);
        }

        [HttpPost("add-publisher")]
        public async Task<IActionResult> AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdPublisher = await _publisherRepository.AddPublisherAsync(addPublisherRequestDTO);
                return Ok(createdPublisher);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-publisher-by-id/{id}")]
        public async Task<IActionResult> UpdatePublisherById([FromRoute] int id, [FromBody] PublisherNoIdDTO publisherNoIdDTO)
        {
            var updatedPublisher = await _publisherRepository.UpdatePublisherByIdAsync(id, publisherNoIdDTO);
            if (updatedPublisher == null)
                return NotFound($"Publisher with Id = {id} not found");

            return Ok(updatedPublisher);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public async Task<IActionResult> DeletePublisherById([FromRoute] int id)
        {
            try
            {
                var deletedPublisher = await _publisherRepository.DeletePublisherByIdAsync(id);
                if (deletedPublisher == null)
                    return NotFound(new { message = $"Publisher with Id = {id} not found" });

                return Ok(new { message = "Publisher deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

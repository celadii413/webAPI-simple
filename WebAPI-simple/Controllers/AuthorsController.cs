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
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        // GET: api/authors/get-all-authors
        [HttpGet("get-all-authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return Ok(authors);
        }

        // GET: api/authors/get-author-by-id/1
        [HttpGet("get-author-by-id/{id}")]
        public async Task<IActionResult> GetAuthorById([FromRoute] int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(author);
        }

        // POST: api/authors/add-author
        [HttpPost("add-author")]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdAuthor = await _authorRepository.AddAuthorAsync(addAuthorRequestDTO);
            return Ok(createdAuthor);
        }

        // PUT: api/authors/update-author-by-id/1
        [HttpPut("update-author-by-id/{id}")]
        public async Task<IActionResult> UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updatedAuthor = await _authorRepository.UpdateAuthorByIdAsync(id, authorNoIdDTO);
            if (updatedAuthor == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(updatedAuthor);
        }

        // DELETE: api/authors/delete-author-by-id/1
        [HttpDelete("delete-author-by-id/{id}")]
        public async Task<IActionResult> DeleteAuthorById([FromRoute] int id)
        {
            var deletedAuthor = await _authorRepository.DeleteAuthorByIdAsync(id);
            if (deletedAuthor == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(deletedAuthor);
        }
    }
}

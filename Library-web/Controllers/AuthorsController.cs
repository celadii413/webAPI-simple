using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Library_web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AuthorsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // GET: /Authors
        public async Task<IActionResult> Index()
        {
            List<AuthorDTO> response = new List<AuthorDTO>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponse = await client.GetAsync("https://localhost:7005/api/authors/get-all-authors");
                httpResponse.EnsureSuccessStatusCode();
                response.AddRange(await httpResponse.Content.ReadFromJsonAsync<IEnumerable<AuthorDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Authors/addAuthor
        [HttpGet]
        public IActionResult addAuthor()
        {
            return View();
        }

        // POST: /Authors/addAuthor
        [HttpPost]
        public async Task<IActionResult> addAuthor(addAuthorDTO addAuthorDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var json = JsonSerializer.Serialize(addAuthorDTO, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://localhost:7005/api/authors/add-author")
                {
                    Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> editAuthor(int id)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7005/api/authors/get-author-by-id/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Index");

            var author = await response.Content.ReadFromJsonAsync<AuthorDTO>();
            if (author == null) return RedirectToAction("Index");

            ViewBag.AuthorId = id;

            return View(new editAuthorDTO { Id = id, FullName = author.FullName });
        }

        [HttpPost]
        public async Task<IActionResult> editAuthor([FromRoute] int id, editAuthorDTO authorDTO)
        {
            var client = httpClientFactory.CreateClient();
            try
            {
                var json = JsonSerializer.Serialize(authorDTO, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7005/api/authors/update-author-by-id/{id}")
                {
                    Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch
            {
                // Load lại dữ liệu khi update lỗi
                var response = await client.GetAsync($"https://localhost:7005/api/authors/get-author-by-id/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var author = await response.Content.ReadFromJsonAsync<AuthorDTO>();
                    return View(new editAuthorDTO { FullName = author.FullName });
                }
                return RedirectToAction("Index");
            }
        }


        // GET: /Authors/delAuthor/5
        [HttpGet]
        public async Task<IActionResult> delAuthor(int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"https://localhost:7005/api/authors/delete-author-by-id/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return RedirectToAction("Index");
        }

        // GET: /Authors/ListAuthor/5
        [HttpGet]
        public async Task<IActionResult> ListAuthor(int id)
        {
            AuthorDTO author = null;
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponse = await client.GetAsync($"https://localhost:7005/api/authors/get-author-by-id/{id}");
                httpResponse.EnsureSuccessStatusCode();

                author = await httpResponse.Content.ReadFromJsonAsync<AuthorDTO>();

                if (author != null && author.Id == 0)
                    author.Id = id;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(author);
        }

    }
}

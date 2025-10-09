using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Library_web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public BooksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index([FromQuery] string filterOn = null, string filterQuery = null, string sortBy = null, bool isAscending = true)
        {
            List<BookDTO> response = new List<BookDTO>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync($"https://localhost:7005/api/Books/get-all-books?filterOn={filterOn}&filterQuery={filterQuery}&sortBy={sortBy}&isAscending={isAscending}");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>());
            }
            catch (Exception ex)
            {
                //log exception
            }
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> addBook()
        {
            var client = httpClientFactory.CreateClient();

            var listAuthor = await client.GetFromJsonAsync<IEnumerable<AuthorDTO>>(
                "https://localhost:7005/api/Authors/get-all-authors");

            var listPublisher = await client.GetFromJsonAsync<IEnumerable<PublisherDTO>>(
                "https://localhost:7005/api/Publishers/get-all-publishers");

            ViewBag.ListAuthor = listAuthor;
            ViewBag.ListPublisher = listPublisher;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addBook(addBookDTO addBookDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var jsonBody = JsonSerializer.Serialize(addBookDTO);

                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7005/api/Books/add-book"),
                    Content = new StringContent(jsonBody, Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var httpResponseMess = await client.SendAsync(httpRequestMess);
                var responseText = await httpResponseMess.Content.ReadAsStringAsync();
                Console.WriteLine(JsonSerializer.Serialize(addBookDTO));

                Console.WriteLine("Response: " + responseText);

                httpResponseMess.EnsureSuccessStatusCode();

                var response = await httpResponseMess.Content.ReadFromJsonAsync<addBookDTO>();
                if (response != null)
                {
                    return RedirectToAction("Index", "Books");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Load lại danh sách
            var reloadClient = httpClientFactory.CreateClient();
            ViewBag.ListAuthor = await reloadClient.GetFromJsonAsync<IEnumerable<AuthorDTO>>(
                "https://localhost:7005/api/Authors/get-all-authors");
            ViewBag.ListPublisher = await reloadClient.GetFromJsonAsync<IEnumerable<PublisherDTO>>(
                "https://localhost:7005/api/Publishers/get-all-publishers");

            return View();
        }


        public async Task<IActionResult> ListBook(int id)
        {
            BookDTO response = new BookDTO();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync($"https://localhost:7005/api/Books/get-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var stringResponseBody = await httpResponseMess.Content.ReadAsStringAsync();
                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> editBook(int id)
        {
            var client = httpClientFactory.CreateClient();

            var bookResponse = await client.GetAsync($"https://localhost:7005/api/Books/get-book-by-id/{id}");
            if (!bookResponse.IsSuccessStatusCode) return RedirectToAction("Index");

            var book = await bookResponse.Content.ReadFromJsonAsync<BookDTO>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (book == null)
            {
                var rawJson = await bookResponse.Content.ReadAsStringAsync();
                Console.WriteLine("API trả về (null BookDTO): " + rawJson);
                return RedirectToAction("Index");
            }

            ViewBag.ListAuthor = await client.GetFromJsonAsync<IEnumerable<AuthorDTO>>("https://localhost:7005/api/Authors/get-all-authors");
            ViewBag.ListPublisher = await client.GetFromJsonAsync<IEnumerable<PublisherDTO>>("https://localhost:7005/api/Publishers/get-all-publishers");
            ViewBag.Book = book;

            var model = new editBookDTO
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherID = book.PublisherId,
                AuthorIds = book.AuthorNames != null ? new List<int>() : new List<int>() 
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> editBook([FromRoute] int id, editBookDTO bookDTO)
        {
            var client = httpClientFactory.CreateClient();
            try
            {
                var json = JsonSerializer.Serialize(bookDTO, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7005/api/Books/update-book-by-id/{id}")
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
                ViewBag.ListAuthor = await client.GetFromJsonAsync<IEnumerable<AuthorDTO>>("https://localhost:7005/api/Authors/get-all-authors");
                ViewBag.ListPublisher = await client.GetFromJsonAsync<IEnumerable<PublisherDTO>>("https://localhost:7005/api/Publishers/get-all-publishers");
                return View(bookDTO);
            }
        }

        [HttpGet]
        public async Task<IActionResult> delBook([FromRoute] int id)
        {
            try
            {
                //Lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.DeleteAsync($"https://localhost:7005/api/Books/delete-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message; 
            }
            return View("Index");
        }
    }
}

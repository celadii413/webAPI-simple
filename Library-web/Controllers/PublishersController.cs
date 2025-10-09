using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Library_web.Controllers
{
    public class PublishersController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public PublishersController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // GET: /Publishers
        public async Task<IActionResult> Index()
        {
            List<PublisherDTO> response = new List<PublisherDTO>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponse = await client.GetAsync("https://localhost:7005/api/Publishers/get-all-publishers");
                httpResponse.EnsureSuccessStatusCode();
                response.AddRange(await httpResponse.Content.ReadFromJsonAsync<IEnumerable<PublisherDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Publishers/addPublisher
        [HttpGet]
        public IActionResult addPublisher()
        {
            return View();
        }

        // POST: /Publishers/addPublisher
        [HttpPost]
        public async Task<IActionResult> addPublisher(addPublisherDTO addPublisherDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(addPublisherDTO, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    "https://localhost:7005/api/Publishers/add-publisher")
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

        // GET: /Publishers/editPublisher/{id}
        [HttpGet]
        public async Task<IActionResult> editPublisher(int id)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7005/api/Publishers/get-publisher-by-id/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var publisher = await response.Content.ReadFromJsonAsync<PublisherDTO>();
            if (publisher == null)
                return RedirectToAction("Index");

            ViewBag.Publisher = publisher;
            ViewBag.PublisherId = id;

            var model = new editPublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name
            };

            return View(model);
        }

        // POST: /Publishers/editPublisher/{id}
        [HttpPost]
        public async Task<IActionResult> editPublisher([FromRoute] int id, editPublisherDTO publisherDTO)
        {
            var client = httpClientFactory.CreateClient();
            try
            {
                var json = JsonSerializer.Serialize(publisherDTO, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var request = new HttpRequestMessage(HttpMethod.Put,
                    $"https://localhost:7005/api/Publishers/update-publisher-by-id/{id}")
                {
                    Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch
            {
                var response = await client.GetAsync($"https://localhost:7005/api/Publishers/get-publisher-by-id/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var publisher = await response.Content.ReadFromJsonAsync<PublisherDTO>();
                    ViewBag.Publisher = publisher;
                    ViewBag.PublisherId = id;

                    return View(new editPublisherDTO
                    {
                        Id = publisher?.Id ?? id,
                        Name = publisher?.Name
                    });
                }

                return RedirectToAction("Index");
            }
        }

        // GET: /Publishers/delPublisher/{id}
        [HttpGet]
        public async Task<IActionResult> delPublisher(int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"https://localhost:7005/api/Publishers/delete-publisher-by-id/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return RedirectToAction("Index");
        }

        // GET: /Publishers/ListPublisher/{id}
        [HttpGet]
        public async Task<IActionResult> ListPublisher(int id)
        {
            PublisherDTO publisher = null;
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponse = await client.GetAsync($"https://localhost:7005/api/Publishers/get-publisher-by-id/{id}");
                httpResponse.EnsureSuccessStatusCode();

                publisher = await httpResponse.Content.ReadFromJsonAsync<PublisherDTO>();

                if (publisher != null && publisher.Id == 0)
                    publisher.Id = id;

                ViewBag.Publisher = publisher;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(publisher);
        }
    }
}

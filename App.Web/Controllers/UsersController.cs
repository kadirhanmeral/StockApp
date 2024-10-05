using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using App.Application;
using App.Web.DTO;
using App.Web.DTO.Login;
using App.Web.DTO.Register;
using App.Web.DTO.Token;
using App.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace App.Web.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }
                var apiUrl = "https://localhost:7099/api/User/Register";
            
                using var httpClient = new HttpClient();

                var newUser = new RegisterUserResponse
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Password = user.Password,
                };
                
                var json = JsonConvert.SerializeObject(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
            
                using var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var loginUser = new LoginUserRequest
                    {
                        Email = user.Email,
                        Password = user.Password
                    };

                    var loginApiUrl = "https://localhost:7099/api/User/Login";
                    var loginJson = JsonConvert.SerializeObject(loginUser);
                    var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
                    
                    using var loginResponse = await httpClient.PostAsync(loginApiUrl, loginContent);
                    
                    if (loginResponse.IsSuccessStatusCode)
                    {
                        var responseBody = await loginResponse.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(responseBody);
                        var token = jsonObject["data"]?["token"]?["result"]?["data"]?.ToString();

                        // Token'ı Session'a kaydet
                        HttpContext.Session.SetString("authToken", token);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Login hatası durumu
                        ModelState.AddModelError("", "Giriş işlemi başarısız.");
                        return View(user);
                    }
                }
                else
                {
                    // Kayıt hatası durumunda yanıtı işleyin
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorModelState = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseContent);

                    // ModelState'e hata mesajlarını ekle
                    foreach (var error in errorModelState)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError(error.Key, message);
                        }
                    }

                    return View(user); // Hata mesajları ile birlikte formu geri döndür
                }
            }
            catch (Exception e)
            {
                return View("ErrorView"); // Hata durumunda gösterilecek view
            }
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserRequest user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                var apiUrl = "https://localhost:7099/api/User/Login";
                using var httpClient = new HttpClient();
            
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                
                    var jsonObject = JObject.Parse(responseBody);
                
                    var token = jsonObject["data"]?["token"]?["result"]?["data"]?.ToString();

                    HttpContext.Session.SetString("authToken", token);

                    return RedirectToAction("Index", "Home");
                }
            
                return View(user);
            }
            catch (Exception e)
            {
                return View(user); // Hata durumunda gösterilecek view
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                var apiUrl = "https://localhost:7099/api/User/Logout";
                var token = new TokenRequest
                (
                    TokenStr : HttpContext.Session.GetString("authToken")
                );
                
            
                using var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
                var json = JsonConvert.SerializeObject(token);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
            
                var response = await httpClient.PostAsync(apiUrl, content);
            
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("authToken");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Yanıtın içeriğini logla
                    Console.WriteLine($"Logout failed: {responseBody}");
                    return StatusCode((int)response.StatusCode, $"Logout failed: {responseBody}");
                }
            }
            catch (Exception e)
            {
                return View("ErrorView"); // Hata durumunda gösterilecek view
            }
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var apiUrl = "https://localhost:7099/api/User/GetProfile";
                using var httpClient = new HttpClient();
                var token = HttpContext.Session.GetString("authToken");
                if (token != null)
                {
                    // Authorization başlığı ekle
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                using var response = await httpClient.GetAsync(apiUrl);
                
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // JSON'da case-insensitive özelliği
                    };
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ServiceResult<GetProfileView>>(apiResponse, options);
                    
                    if (result != null)
                    {
                        var isAuthenticated = HttpContext.Session.GetString("authToken") != null;
                        ViewBag.IsAuthenticated = isAuthenticated;
                        return View(result.Data);
                    }

                    return NotFound("Profile not found.");
                }

                return StatusCode((int)response.StatusCode, "API'den veri alınamadı.");


            }
            catch (Exception e)
            {
                return View("ErrorView"); // Hata durumunda gösterilecek view
            }
        }
    }
}

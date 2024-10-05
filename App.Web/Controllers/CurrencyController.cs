using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using App.Application;
using App.Application.Features.Buy;
using App.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace App.Web.Controllers;

public class CurrencyController : Controller
{
    public async Task<IActionResult> Details(string name)
    {
        try
        {
            var apiUrl = $"https://localhost:7099/api/currency/GetCurrencyDetails?name={name}";

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // JSON'da case-insensitive özelliği
                };

                var currency = JsonSerializer.Deserialize<ServiceResult<CurrencyDetailsView>>(apiResponse,options);

                if (currency is { IsSuccess: true })
                {
                    var isAuthenticated = HttpContext.Session.GetString("authToken") != null;
                    ViewBag.IsAuthenticated = isAuthenticated;
                    return View(currency.Data);
                }
                else
                {
                    return NotFound("Stock details not found.");
                }
            }
            else
            {
                return StatusCode((int)response.StatusCode, "API'den veri alınamadı.");
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Server Error: {e.Message}");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> BuyCurrency( [FromBody] BuyStockRequest stock)
    {
        var apiUrl = "https://localhost:7099/api/Currency/BuyCurrency";
        
        using var httpClient = new HttpClient();
        var token = HttpContext.Session.GetString("authToken");
        if (token != null)
        {
            // Authorization başlığı ekle
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var buyStock = new BuyStockView
        {
            Price = stock.Price,
            Quantity = stock.Quantity,
            Symbol = stock.Symbol
        };
        var json = JsonConvert.SerializeObject(buyStock);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync(apiUrl, content);
        
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Yanıtın içeriğini logla
            Console.WriteLine($"Logout failed: {responseBody}");
            return StatusCode((int)response.StatusCode, $"Logout failed: {responseBody}");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> SellCurrency( [FromBody] BuyStockRequest stock)
    {
        var apiUrl = "https://localhost:7099/api/Currency/SellCurrency";
        
        using var httpClient = new HttpClient();
        var token = HttpContext.Session.GetString("authToken");
        if (token != null)
        {
            // Authorization başlığı ekle
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var buyStock = new BuyStockView
        {
            Price = stock.Price,
            Quantity = stock.Quantity,
            Symbol = stock.Symbol
        };
        var json = JsonConvert.SerializeObject(buyStock);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync(apiUrl, content);
        
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            Console.WriteLine($"Logout failed: {responseBody}");
            return StatusCode((int)response.StatusCode, $"Logout failed: {responseBody}");
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using App.Application;
using App.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers;

public class HomeController : Controller
{
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    try
    {
        // API URL'leri
        var apiUrl = $"https://localhost:7099/api/stock/GetStocksDataPaged?page={page}&pageSize={pageSize}";
        var currencyApiUrl = $"https://localhost:7099/api/currency/GetCurrencyList";

        List<StockDetailsView>? stockList = null;
        CurrencyListView? currencyList = null; 
        var totalPages = 0;

        using (var httpClient = new HttpClient())
        {
            // İlk API isteği
            using (var response = await httpClient.GetAsync(apiUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = JsonSerializer.Deserialize<ServiceResult<List<StockDetailsView>>>(apiResponse, options);

                    if (result != null && result.IsSuccess)
                    {
                        stockList = result.Data;
                        totalPages = 10; // Toplam sayfaları burada alabilirsiniz
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, string.Join(", ", result?.ErrorMessages ?? new List<string>()));
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "API'den veri alınamadı.");
                }
            }

            // İkinci API isteği
            using (var currencyResponse = await httpClient.GetAsync(currencyApiUrl))
            {
                if (currencyResponse.IsSuccessStatusCode)
                {
                    var currencyApiResponse = await currencyResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var currencyResult = JsonSerializer.Deserialize<ServiceResult<CurrencyListView>>(currencyApiResponse, options);

                    if (currencyResult != null && currencyResult.IsSuccess)
                    {
                        currencyList = currencyResult.Data;
                    }
                    else
                    {
                        return StatusCode((int)currencyResponse.StatusCode, string.Join(", ", currencyResult?.ErrorMessages ?? new List<string>()));
                    }
                }
                else
                {
                    return StatusCode((int)currencyResponse.StatusCode, "Para birimi API'sinden veri alınamadı.");
                }
            }
        }
        // Mevcut currencyNames sözlüğü


        var viewModel = new PaginatedStockViewModel
        {
            Stocks = stockList!,
            TotalPages = totalPages,
            CurrentPage = page,
            Currencies = currencyList
        };


        var isAuthenticated = HttpContext.Session.GetString("authToken") != null;
        var authToken = HttpContext.Session.GetString("authToken");
        string? userName = null;

        if (!string.IsNullOrEmpty(authToken))
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authToken);
            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == "nameid");
            userName = nameClaim?.Value;
        }

        ViewBag.IsAuthenticated = isAuthenticated;
        if (userName != null) ViewBag.UserName = userName;

        return View(viewModel);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Sunucu hatası: {ex.Message}");
    }
}


public async Task<IActionResult> GetStocksData(int page = 1, int pageSize = 10)
{
    try
    {
        var apiUrl = $"https://localhost:7099/api/stock/GetStocksDataPaged?page={page}&pageSize={pageSize}";

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // JSON'da case-insensitive özelliği
            };
            var result = JsonSerializer.Deserialize<ServiceResult<List<StockDetailsView>>>(apiResponse, options);
                    
            var result1 = new
            {
                items = result.Data, // Bu adın JavaScript'teki ile uyumlu olduğundan emin olun
                totalPages = 10, // Bu adın JavaScript'teki ile uyumlu olduğundan emin olun
                currentPage = page
                            
            };

            return Json(result1);
        }
        else
        {
            return StatusCode((int)response.StatusCode, "API'den veri alınamadı.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Sunucu hatası: {ex.Message}");
    }
}
    


}

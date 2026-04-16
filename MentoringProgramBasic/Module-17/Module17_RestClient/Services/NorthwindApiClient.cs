using System.Net.Http.Json;
using System.Text.Json;
using Module17_RestClient.Models;

namespace Module17_RestClient.Services;

public class NorthwindApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public NorthwindApiClient(string baseUrl = "https://localhost:5001")
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    #region Categories

    public async Task<List<Category>?> GetAllCategoriesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Category>>("/api/categories", _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting categories: {ex.Message}");
            return null;
        }
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Category>($"/api/categories/{id}", _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Category with ID {id} not found");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting category: {ex.Message}");
            return null;
        }
    }

    public async Task<Category?> CreateCategoryAsync(Category category)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/categories", category, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Category>(_jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating category: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, Category category)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/categories/{id}", category, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating category: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting category: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Products

    public async Task<PagedResult<Product>?> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
    {
        try
        {
            var url = $"/api/products?pageNumber={pageNumber}&pageSize={pageSize}";
            if (categoryId.HasValue)
            {
                url += $"&categoryId={categoryId.Value}";
            }

            return await _httpClient.GetFromJsonAsync<PagedResult<Product>>(url, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting products: {ex.Message}");
            return null;
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Product>($"/api/products/{id}", _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Product with ID {id} not found");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting product: {ex.Message}");
            return null;
        }
    }

    public async Task<Product?> CreateProductAsync(Product product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/products", product, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating product: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateProductAsync(int id, Product product)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/products/{id}", product, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/products/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting product: {ex.Message}");
            return false;
        }
    }

    #endregion

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

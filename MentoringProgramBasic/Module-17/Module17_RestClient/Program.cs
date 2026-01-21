using Module17_RestClient.Models;
using Module17_RestClient.Services;

Console.WriteLine("=== Module 17 REST API Client Demo ===\n");
Console.WriteLine("Starting API tests...\n");

using var client = new NorthwindApiClient("https://localhost:5001");

// Wait for user confirmation that API is running
Console.WriteLine("⚠️  Make sure the API is running (dotnet run in Module17_WebApplication)");
Console.WriteLine("Press any key to start tests...");
Console.ReadKey();
Console.WriteLine("\n");

try
{
    await RunCategoriesTests(client);
    Console.WriteLine("\n" + new string('=', 80) + "\n");
    await RunProductsTests(client);
    Console.WriteLine("\n" + new string('=', 80) + "\n");
    await RunPaginationTests(client);
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
}

Console.WriteLine("\n\n=== Tests Completed ===");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static async Task RunCategoriesTests(NorthwindApiClient client)
{
    Console.WriteLine("📋 CATEGORIES TESTS");
    Console.WriteLine(new string('-', 80));

    // 1. Get all categories
    Console.WriteLine("\n1️⃣  GET All Categories:");
    var categories = await client.GetAllCategoriesAsync();
    if (categories != null)
    {
        Console.WriteLine($"   ✅ Found {categories.Count} categories");
        foreach (var cat in categories.Take(3))
        {
            Console.WriteLine($"      - {cat.CategoryName}: {cat.Description}");
        }
    }

    // 2. Get specific category
    Console.WriteLine("\n2️⃣  GET Category by ID (1):");
    var category = await client.GetCategoryByIdAsync(1);
    if (category != null)
    {
        Console.WriteLine($"   ✅ {category.CategoryName}: {category.Description}");
    }

    // 3. Create new category
    Console.WriteLine("\n3️⃣  POST Create New Category:");
    var newCategory = new Category
    {
        CategoryName = "Test Category",
        Description = "Created by REST Client"
    };
    var created = await client.CreateCategoryAsync(newCategory);
    if (created != null)
    {
        Console.WriteLine($"   ✅ Created category with ID: {created.CategoryID}");

        // 4. Update the category
        Console.WriteLine("\n4️⃣  PUT Update Category:");
        created.Description = "Updated by REST Client";
        var updated = await client.UpdateCategoryAsync(created.CategoryID, created);
        if (updated)
        {
            Console.WriteLine($"   ✅ Category {created.CategoryID} updated successfully");
        }

        // 5. Delete the category
        Console.WriteLine("\n5️⃣  DELETE Category:");
        var deleted = await client.DeleteCategoryAsync(created.CategoryID);
        if (deleted)
        {
            Console.WriteLine($"   ✅ Category {created.CategoryID} deleted successfully");
        }
    }

    // 6. Try to get non-existent category
    Console.WriteLine("\n6️⃣  GET Non-existent Category (999):");
    var notFound = await client.GetCategoryByIdAsync(999);
    if (notFound == null)
    {
        Console.WriteLine("   ✅ Correctly returned null for non-existent category");
    }
}

static async Task RunProductsTests(NorthwindApiClient client)
{
    Console.WriteLine("📦 PRODUCTS TESTS");
    Console.WriteLine(new string('-', 80));

    // 1. Get products with default pagination
    Console.WriteLine("\n1️⃣  GET Products (default pagination):");
    var products = await client.GetProductsAsync();
    if (products != null)
    {
        Console.WriteLine($"   ✅ Page {products.PageNumber}/{products.TotalPages}");
        Console.WriteLine($"   ✅ Found {products.TotalItems} total products, showing {products.Items.Count}");
        foreach (var prod in products.Items.Take(3))
        {
            Console.WriteLine($"      - {prod.ProductName}: ${prod.UnitPrice}");
        }
    }

    // 2. Get specific product
    Console.WriteLine("\n2️⃣  GET Product by ID (1):");
    var product = await client.GetProductByIdAsync(1);
    if (product != null)
    {
        Console.WriteLine($"   ✅ {product.ProductName}");
        Console.WriteLine($"      Price: ${product.UnitPrice}");
        Console.WriteLine($"      Category: {product.Category?.CategoryName}");
    }

    // 3. Create new product
    Console.WriteLine("\n3️⃣  POST Create New Product:");
    var newProduct = new Product
    {
        ProductName = "Test Product",
        CategoryID = 1,
        UnitPrice = 99.99m,
        UnitsInStock = 10,
        Discontinued = false
    };
    var createdProduct = await client.CreateProductAsync(newProduct);
    if (createdProduct != null)
    {
        Console.WriteLine($"   ✅ Created product with ID: {createdProduct.ProductID}");

        // 4. Update the product
        Console.WriteLine("\n4️⃣  PUT Update Product:");
        createdProduct.UnitPrice = 149.99m;
        var updated = await client.UpdateProductAsync(createdProduct.ProductID, createdProduct);
        if (updated)
        {
            Console.WriteLine($"   ✅ Product {createdProduct.ProductID} updated successfully");
        }

        // 5. Delete the product
        Console.WriteLine("\n5️⃣  DELETE Product:");
        var deleted = await client.DeleteProductAsync(createdProduct.ProductID);
        if (deleted)
        {
            Console.WriteLine($"   ✅ Product {createdProduct.ProductID} deleted successfully");
        }
    }
}

static async Task RunPaginationTests(NorthwindApiClient client)
{
    Console.WriteLine("📄 PAGINATION & FILTERING TESTS");
    Console.WriteLine(new string('-', 80));

    // 1. First page
    Console.WriteLine("\n1️⃣  GET Products - Page 1, Size 5:");
    var page1 = await client.GetProductsAsync(pageNumber: 1, pageSize: 5);
    if (page1 != null)
    {
        Console.WriteLine($"   ✅ Page {page1.PageNumber}/{page1.TotalPages} ({page1.Items.Count} items)");
        Console.WriteLine($"   ✅ Has Next Page: {page1.HasNextPage}");
        foreach (var p in page1.Items)
        {
            Console.WriteLine($"      - {p.ProductName}");
        }
    }

    // 2. Second page
    Console.WriteLine("\n2️⃣  GET Products - Page 2, Size 5:");
    var page2 = await client.GetProductsAsync(pageNumber: 2, pageSize: 5);
    if (page2 != null)
    {
        Console.WriteLine($"   ✅ Page {page2.PageNumber}/{page2.TotalPages} ({page2.Items.Count} items)");
        Console.WriteLine($"   ✅ Has Previous Page: {page2.HasPreviousPage}");
        foreach (var p in page2.Items)
        {
            Console.WriteLine($"      - {p.ProductName}");
        }
    }

    // 3. Filter by category
    Console.WriteLine("\n3️⃣  GET Products - Filter by Category ID 1 (Beverages):");
    var filtered = await client.GetProductsAsync(pageNumber: 1, pageSize: 10, categoryId: 1);
    if (filtered != null)
    {
        Console.WriteLine($"   ✅ Found {filtered.TotalItems} beverages");
        foreach (var p in filtered.Items)
        {
            Console.WriteLine($"      - {p.ProductName} (Category: {p.Category?.CategoryName})");
        }
    }

    // 4. Filter by category with pagination
    Console.WriteLine("\n4️⃣  GET Products - Category 2, Page 1, Size 3:");
    var filteredPaged = await client.GetProductsAsync(pageNumber: 1, pageSize: 3, categoryId: 2);
    if (filteredPaged != null)
    {
        Console.WriteLine($"   ✅ Page {filteredPaged.PageNumber}/{filteredPaged.TotalPages}");
        Console.WriteLine($"   ✅ Total items in category: {filteredPaged.TotalItems}");
        foreach (var p in filteredPaged.Items)
        {
            Console.WriteLine($"      - {p.ProductName}");
        }
    }

    // 5. Large page size
    Console.WriteLine("\n5️⃣  GET Products - Page 1, Size 50:");
    var largePage = await client.GetProductsAsync(pageNumber: 1, pageSize: 50);
    if (largePage != null)
    {
        Console.WriteLine($"   ✅ Retrieved {largePage.Items.Count} items (Total: {largePage.TotalItems})");
        Console.WriteLine($"   ✅ Total Pages: {largePage.TotalPages}");
    }
}


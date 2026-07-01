using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Common;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                // Seeding
                // form the path : "E:\education\Route\Backend\09_Web API\project\E_Commerce\E_Commerce.API\bin\Debug\net8.0\DataSeed\products.json"
                var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeed"); //E:\education\Route\Backend\09_Web API\project\E_Commerce\E_Commerce.API\bin\Debug\net8.0\DataSeed
                // order matters here!
                await SeedIfEmptyAsync<ProductBrand, int>(seedRoot, "brands.json", ct);
                await SeedIfEmptyAsync<ProductType, int>(seedRoot, "types.json", ct);
                await SeedIfEmptyAsync<Product, int>(seedRoot, "products.json", ct);

                int result = await dbContext.SaveChangesAsync(ct);
                if (result > 0)
                    logger.LogInformation($"{result} Rows Added");
                else
                    logger.LogInformation("Database Already Seeded");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to Seed Data: {ex.Message}");
            }
        }

        private async Task SeedIfEmptyAsync<T, TKey>(string rootPath, string fileName, CancellationToken ct = default) where T : BaseEntity<TKey>
        {
            if (await dbContext.Set<T>().AnyAsync())
            {
                logger.LogInformation("Data Already Seeded");
                return;
            }
            var filePath = Path.Combine(rootPath, fileName);
            if (!File.Exists(filePath))
            {
                logger.LogInformation($"File {fileName} Is Not Exist");
                return;
            }
            using var fileStream = File.OpenRead(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            var items = await JsonSerializer.DeserializeAsync<List<T>>(fileStream, options, ct);
            if(items?.Any() ?? false)
                dbContext.Set<T>().AddRange(items);
        }
    }
}

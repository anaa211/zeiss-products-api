using Microsoft.EntityFrameworkCore;
using Products.Application.Interfaces.Helpers;
using Products.Infrastructure;

namespace Products.Application.Helpers;

public class RandomIdGenerator: IRandomIdGenerator
{
    private readonly DatabaseContext _databaseContext;
    private static readonly Random _random = new Random();

    public RandomIdGenerator(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<int> GenerateUniqueIdAsync()
    {
        int newId;
        bool exists;

        do
        {
            newId = _random.Next(100000, 999999); // 6-digit number
            exists = await _databaseContext.Products.AnyAsync(p => p.Id == newId);
        }
        while (exists);

        return newId;
    }
}


namespace Products.Application.Interfaces.Helpers;

public interface IRandomIdGenerator
{
    Task<int> GenerateUniqueIdAsync();
}

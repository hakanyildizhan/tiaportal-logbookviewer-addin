using System.Threading.Tasks;

namespace Sirius.LogbookViewer.Product
{
    /// <summary>
    /// Interface for product-specific parser implementations.
    /// </summary>
    public interface IParser
    {
        Task<LogbookData> Parse(string filePath);
    }
}

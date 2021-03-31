using System.Globalization;

namespace Sirius.LogbookViewer.Product
{
    public interface IResourceManager
    {
        string GetString(string key, CultureInfo culture);
        string GetStringInCulture(string text, CultureInfo culture);
        string GetStringInCulture(ResourceType resourceType, string text, CultureInfo culture);
    }
}

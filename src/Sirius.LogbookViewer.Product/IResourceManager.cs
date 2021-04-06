using System.Globalization;

namespace Sirius.LogbookViewer.Product
{
    public interface IResourceManager
    {
        /// <summary>
        /// Gets a resource string via its resource key in current culture.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        string GetStringViaKey(string resourceKey);

        /// <summary>
        /// Gets a resource string via its resource key in given culture.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetStringViaKey(string resourceKey, CultureInfo culture);

        /// <summary>
        /// Gets a resource string that is equivalent to the given string in given culture.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetString(string text, CultureInfo culture);

        /// <summary>
        /// Gets a resource string of requested type that is equivalent to the given string in given culture.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="text"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetString(ResourceType resourceType, string text, CultureInfo culture);

        /// <summary>
        /// Gets a resource string of requested type that is equivalent to the given string in current culture.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        string GetString(ResourceType resourceType, string text);

        /// <summary>
        /// Gets a resource string that is equivalent to the given string in current culture.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string GetString(string text);
    }
}

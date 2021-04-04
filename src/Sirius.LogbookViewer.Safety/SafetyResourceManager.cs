using Sirius.LogbookViewer.Product;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Sirius.LogbookViewer.Safety
{
    [Export(typeof(IResourceManager))]
    class SafetyResourceManager : IResourceManager
    {
        private readonly ResourceManager _resourceManager;
        private static List<string> AVAILABLE_CULTURES = new List<string>() { "en-US", "fr-FR", "de-DE" };

        public SafetyResourceManager()
        {
            _resourceManager = new ResourceManager("Sirius.LogbookViewer.Safety.Resources.Text.Resource", System.Reflection.Assembly.GetAssembly(typeof(SafetyResourceManager)));
        }

        public string GetString(string resourceKey)
        {
            return _resourceManager.GetString(resourceKey, CultureInfo.CurrentCulture);
        }

        public string GetString(string resourceKey, CultureInfo culture)
        {
            return _resourceManager.GetString(resourceKey, culture);
        }

        public string GetStringInCulture(ResourceType resourceType, string text, CultureInfo culture)
        {
            // find Key
            string resourceKey = string.Empty;

            foreach (var availableCulture in AVAILABLE_CULTURES)
            {
                resourceKey = FindResource(availableCulture, text, resourceType);
                if (!string.IsNullOrEmpty(resourceKey)) break;
            }

            if (string.IsNullOrEmpty(resourceKey))
            {
                return text; // not found
            }

            return GetString(resourceKey, culture);
        }

        public string GetStringInCulture(string text, CultureInfo culture)
        {
            return GetStringInCulture(ResourceType.Any, text, culture);
        }

        public string GetStringInCurrentCulture(ResourceType resourceType, string text)
        {
            return GetStringInCulture(resourceType, text, CultureInfo.CurrentCulture);
        }

        public string GetStringInCurrentCulture(string text)
        {
            return GetStringInCurrentCulture(ResourceType.Any, text);
        }

        private string FindResource(string culture, string text, ResourceType resourceType = ResourceType.Any)
        {
            DictionaryEntry entry = new DictionaryEntry();

            if (resourceType != ResourceType.Any)
            {
                entry = _resourceManager.GetResourceSet(new CultureInfo(culture), true, true)
              .OfType<DictionaryEntry>()
              .Where(e => e.Key.ToString().StartsWith(resourceType.ToString()))
              .FirstOrDefault(e => e.Value.ToString() == text);
            }
            else
            {
                entry = _resourceManager.GetResourceSet(new CultureInfo(culture), true, true)
              .OfType<DictionaryEntry>()
              .FirstOrDefault(e => e.Value.ToString() == text);
            }

            return entry.Key != null ? entry.Key.ToString() : string.Empty;
        }
    }
}

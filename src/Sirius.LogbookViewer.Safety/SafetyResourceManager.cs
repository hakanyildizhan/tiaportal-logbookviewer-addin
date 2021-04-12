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
        private readonly static List<string> AVAILABLE_CULTURES = new List<string>() { "en", "fr-FR", "de-DE" };

        public SafetyResourceManager()
        {
            _resourceManager = new ResourceManager("Sirius.LogbookViewer.Safety.Resources.Text.Resource", System.Reflection.Assembly.GetAssembly(typeof(SafetyResourceManager)));
        }

        public string GetStringViaKey(string resourceKey)
        {
            return _resourceManager.GetString(resourceKey, CultureInfo.CurrentCulture);
        }

        public string GetStringViaKey(string resourceKey, CultureInfo culture)
        {
            return _resourceManager.GetString(resourceKey, culture);
        }

        public string GetString(ResourceType resourceType, string text, CultureInfo culture)
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

            return GetStringViaKey(resourceKey, culture);
        }

        public string GetString(string text, CultureInfo culture)
        {
            return GetString(ResourceType.Any, text, culture);
        }

        public string GetString(ResourceType resourceType, string text)
        {
            if (AVAILABLE_CULTURES.Any(c => CultureInfo.CurrentCulture.Name.StartsWith(c)))
            {
                return GetString(resourceType, text, CultureInfo.CurrentCulture);
            }

            return GetString(resourceType, text, new CultureInfo("en"));
        }

        public string GetMessage(int objectNumber, int elementNumber)
        {
            string key = $"Message.ON{objectNumber}{(elementNumber != 0 ? $".EN{elementNumber}" : string.Empty)}";
            return _resourceManager.GetString(key, CultureInfo.CurrentCulture);
        }

        public string GetString(string text)
        {
            return GetString(ResourceType.Any, text);
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

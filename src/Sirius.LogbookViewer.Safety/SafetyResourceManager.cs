using Sirius.LogbookViewer.Product;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Resources;

namespace Sirius.LogbookViewer.Safety
{
    [Export(typeof(IResourceManager))]
    class SafetyResourceManager : IResourceManager
    {
        private readonly ResourceManager _resourceManager;
        private ResourceSet _resourcesEn;
        private ResourceSet _resourcesDe;
        private ResourceSet _resourcesFr;

        public ResourceSet ResourcesEn => _resourcesEn != null ? _resourcesEn : (_resourcesEn = _resourceManager?.GetResourceSet(CultureInfo.GetCultureInfo("en"), true, false));

        public ResourceSet ResourcesDe => _resourcesDe != null ? _resourcesDe : (_resourcesDe = _resourceManager?.GetResourceSet(CultureInfo.GetCultureInfo("de"), true, false));

        public ResourceSet ResourcesFr => _resourcesFr != null ? _resourcesFr : (_resourcesFr = _resourceManager?.GetResourceSet(CultureInfo.GetCultureInfo("fr"), true, false));

        public SafetyResourceManager()
        {
            _resourceManager = new ResourceManager("Sirius.LogbookViewer.Safety.Resources.Text.Resource", System.Reflection.Assembly.GetAssembly(typeof(SafetyResourceManager)));
        }

        public string GetString(string key, CultureInfo culture)
        {
            return _resourceManager.GetString(key, culture);
        }

        public string GetStringInCulture(string text, CultureInfo culture)
        {
            string result = FindResource(ResourcesEn, text, culture);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            result = FindResource(ResourcesDe, text, culture);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            result = FindResource(ResourcesFr, text, culture);

            return !string.IsNullOrEmpty(result) ? result : text;
        }

        public string GetStringInCulture(ResourceType resourceType, string text, CultureInfo culture)
        {
            string result = FindResource(ResourcesEn, text, culture, resourceType);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            result = FindResource(ResourcesDe, text, culture, resourceType);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            result = FindResource(ResourcesFr, text, culture, resourceType);

            return !string.IsNullOrEmpty(result) ? result : text;
        }

        private string FindResource(ResourceSet resources, string textToFind, CultureInfo culture, ResourceType resourceType = ResourceType.Any)
        {
            if (resources == null)
            {
                return textToFind;
            }

            foreach (DictionaryEntry entry in resources)
            {
                if (resourceType == ResourceType.Message && !entry.Value.ToString().StartsWith("Message"))
                {
                    continue;
                }
                else if (resourceType == ResourceType.UI && !entry.Value.ToString().StartsWith("UI"))
                {
                    continue;
                }

                if (entry.Value.ToString().Equals(textToFind, StringComparison.InvariantCultureIgnoreCase))
                {
                    return GetString(entry.Key.ToString(), culture);
                }
            }

            return string.Empty;
        }
    }
}

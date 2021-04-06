using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Sirius.LogbookViewer.UI.Service
{
    public class UIResourceManager
    {
        private readonly ResourceManager _resourceManager;
        private readonly static List<string> AVAILABLE_CULTURES = new List<string>() { "en", "de-DE", "fr-FR" };

        public UIResourceManager()
        {
            _resourceManager = new ResourceManager("Sirius.LogbookViewer.UI.Resource.Text.Resource", System.Reflection.Assembly.GetAssembly(typeof(UIResourceManager)));
        }

        public string GetString(string key)
        {
            return AVAILABLE_CULTURES.Any(c => CultureInfo.CurrentCulture.Name.StartsWith(c)) ? _resourceManager.GetString(key) : _resourceManager.GetString(key, new CultureInfo("en"));
        }
    }
}

using System;
using Siemens.Engineering.AddIn;
using System.Collections.Generic;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering;

namespace Sirius.LogbookViewer
{
    public class DevicesAndNetworksProvider : DevicesAndNetworksAddInProvider
    {
        private readonly TiaPortal _tiaPortal;

        public DevicesAndNetworksProvider(TiaPortal tiaPortal)
        {
            _tiaPortal = tiaPortal;
        }

        protected override IEnumerable<ContextMenuAddIn> GetContextMenuAddIns()
        {
            yield return new LogbookViewerAddIn(_tiaPortal); 
        }
    }
}

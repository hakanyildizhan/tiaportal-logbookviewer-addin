using Siemens.Engineering.AddIn;
using System.Collections.Generic;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering;

namespace Sirius.LogbookViewer.AddIn
{
    public class ProjectTreeProvider : ProjectTreeAddInProvider
    {
        private readonly TiaPortal _tiaPortal;

        public ProjectTreeProvider(TiaPortal tiaPortal)
        {
            _tiaPortal = tiaPortal;
        }

        protected override IEnumerable<ContextMenuAddIn> GetContextMenuAddIns()
        {
            yield return new LogbookViewerAddIn(_tiaPortal);
        }
    }
}

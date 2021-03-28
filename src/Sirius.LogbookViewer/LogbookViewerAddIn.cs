using System;
using System.Diagnostics;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering;
using Sirius.LogbookViewer.Service;

namespace Sirius.LogbookViewer
{
    public class LogbookViewerAddIn : ContextMenuAddIn
    {
        public LogbookViewerAddIn(TiaPortal tiaPortal) : base("Safety AddIns")
        {
            ServiceContainer.Instance.Register<TiaPortal>(tiaPortal);
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Offline logbook viewer...", OnClick);
        }

        private void OnClick(MenuSelectionProvider menuSelectionProvider)
        {
            try
            {
                var app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message + "\r\n" + ex.InnerException);
            }
            
        }
    }
}

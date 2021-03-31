using Siemens.Engineering;
using Sirius.LogbookViewer.UI.Service;
using System.Threading.Tasks;

namespace Sirius.LogbookViewer.Service
{
    /// <summary>
    /// Load indicator that will show a TIA-style loading dialog.
    /// </summary>
    public class TIALoadingDialog : IWaitIndicator
    {
        private readonly TiaPortal _tiaPortal;

        /// <summary>
        /// Exclusive access object has to be static.
        /// </summary>
        private static ExclusiveAccess _exclusiveAccess;

        public TIALoadingDialog(TiaPortal tiaPortal)
        {
            _tiaPortal = tiaPortal;
        }
        
        public void Show()
        {
            _exclusiveAccess = _tiaPortal.ExclusiveAccess();
        }

        public void Show(string text)
        {
            _exclusiveAccess = _tiaPortal.ExclusiveAccess(text);
        }

        public void ShowMessage(string text)
        {
            if (_exclusiveAccess != null)
            {
                _exclusiveAccess.Text = text;
            }
        }

        public async Task ShowAsync(string message)
        {
            await Task.Run(() => Show(message));
        }

        public async Task ShowAsync(string windowTitle, string header, string message)
        {
            await Task.Run(() => Show(message));
        }

        public void Prompt(string message)
        {
            ShowMessage(message);
        }

        public void Close()
        {
            _exclusiveAccess?.Dispose();
        }

        public void Prompt(string message, Prompt promptType)
        {
            Prompt(message);
        }

        public void Prompt(string header, string message, Prompt promptType)
        {
            Prompt(message);
        }
    }
}

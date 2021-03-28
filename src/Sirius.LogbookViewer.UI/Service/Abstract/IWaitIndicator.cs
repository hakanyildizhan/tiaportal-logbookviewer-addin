using System.Threading.Tasks;

namespace Sirius.LogbookViewer.UI.Service
{
    /// <summary>
    /// Interface for loading indicator dialog implementations.
    /// </summary>
    public interface IWaitIndicator
    {
        void Show();
        void Show(string message);
        Task ShowAsync(string message);
        Task ShowAsync(string windowTitle, string header, string message);
        void ShowMessage(string message);
        void Prompt(string message);
        void Prompt(string message, Prompt promptType);
        void Prompt(string header, string message, Prompt promptType);
        void Close();
    }
}

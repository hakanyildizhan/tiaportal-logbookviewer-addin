namespace Sirius.LogbookViewer.UI.Service
{
    /// <summary>
    /// Interface for FilePicker implementations to be defined in executing assemblies.
    /// </summary>
    public interface IFilePicker
    {
        string SelectFile();
    }
}

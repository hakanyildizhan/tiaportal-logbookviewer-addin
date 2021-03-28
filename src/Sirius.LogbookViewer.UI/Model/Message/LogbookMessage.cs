namespace Sirius.LogbookViewer.UI.Model
{
    public class LogbookMessage : BaseViewModel
    {
        public int Index { get; set; }
        public string Source { get; set; }
        public string OperatingHours { get; set; }
        public string Message { get; set; }
        public int ObjectNumber { get; internal set; }
    }
}

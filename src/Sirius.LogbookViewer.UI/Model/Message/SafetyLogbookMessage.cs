namespace Sirius.LogbookViewer.UI.Model
{
    public class SafetyLogbookMessage : LogbookMessage
    {
        public int Type { get; set; }
        public string ElementNumber { get; set; }
        public string ElementFunction { get; set; }
    }
}

using System.Collections.ObjectModel;

namespace Sirius.LogbookViewer.UI.Model
{
    /// <summary>
    /// A sample list of Safety specific logbook messages for Safety Logbook grid.
    /// </summary>
    public class MessageListDesignModelSafety : BaseViewModel
    {
        private ObservableCollection<SafetyLogbookMessage> _messages;

        public static MessageListDesignModelSafety Instance => new MessageListDesignModelSafety();
        
        public ObservableCollection<SafetyLogbookMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public MessageListDesignModelSafety()
        {
            Messages = new ObservableCollection<SafetyLogbookMessage>()
            {
                new SafetyLogbookMessage() { Type = 1, Index = 1, Source = "Device", OperatingHours = "836:36:53", ElementNumber = "-", ElementFunction = "-", ObjectNumber = 7004, Message = "+Operating mode change rejected"},
                new SafetyLogbookMessage() { Type = 3, Index = 2, Source = "Device", OperatingHours = "837:36:53", ElementNumber = "-", ElementFunction = "-", ObjectNumber = 15674, Message = "+Configuration not released"},
                new SafetyLogbookMessage() { Type = 2, Index = 3, Source = "Elements", OperatingHours = "837:38:53", ElementNumber = "-", ElementFunction = "-", ObjectNumber = 4007, Message = "+User program stopped"},
                new SafetyLogbookMessage() { Type = 4, Index = 4, Source = "Product", OperatingHours = "838:26:02", ElementNumber = "-", ElementFunction = "-", ObjectNumber = 7004, Message = "+Password protection for device access is inactive"},
            };
        }
    }
}

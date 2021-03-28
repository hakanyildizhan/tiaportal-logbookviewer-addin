using System;
using System.Windows.Input;

namespace Sirius.LogbookViewer.UI.Model
{
    public class LoadingWindowViewModel : BaseViewModel
    {
        private bool _canAnimate;
        private bool _prompt;
        private string _message;
        private Uri _loadingAnimationSource;
        private ICommand _acknowledgeCommand;

        /// <summary>
        /// Text header to show on the dialog.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        /// <summary>
        /// Text to show on the dialog.
        /// </summary>
        public string MessageTitle { get; set; }
        
        public string WindowTitle { get; set; }

        /// <summary>
        /// Whether the loading image can be animated.
        /// </summary>
        public bool CanAnimate 
        { 
            get { return _canAnimate; }
            set
            {
                if (_canAnimate != value)
                {
                    _canAnimate = value;
                    OnPropertyChanged(nameof(CanAnimate));
                }
            }
        }

        public Uri LoadingAnimationSource
        {
            get { return _loadingAnimationSource; }
            set
            {
                if (_loadingAnimationSource != value)
                {
                    _loadingAnimationSource = value;
                    OnPropertyChanged(nameof(LoadingAnimationSource));
                }
            }
        }

        /// <summary>
        /// Whether to prompt the user, possibly with a message.
        /// </summary>
        public bool Prompt
        {
            get { return _prompt; }
            set
            {
                if (_prompt != value)
                {
                    _prompt = value;
                    OnPropertyChanged(nameof(Prompt));
                }
            }
        }
        
        public ICommand AcknowledgeCommand
        {
            get { return _acknowledgeCommand; }
            set
            {
                if (_acknowledgeCommand != value)
                {
                    _acknowledgeCommand = value;
                    OnPropertyChanged(nameof(AcknowledgeCommand));
                }
            }
        }

        public LoadingWindowViewModel()
        {
            // set window defaults
            WindowTitle = "Loading";
            MessageTitle = "Loading in progress";
            Prompt = false;

            AcknowledgeCommand = null; // set by the service
        }
    }
}

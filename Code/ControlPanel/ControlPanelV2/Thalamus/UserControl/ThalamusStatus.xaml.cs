using System;
using System.ComponentModel;
using System.Windows;
using EuxUtils;
using Thalamus;

namespace ControlPanel.Thalamus.UserControl
{

    public class ThalamusStatusViewModel : ViewModelBase
    {
        private bool _isConnected = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyPropertyChanged("IsConnected");
            }
        }
    }
    /// <summary>
    /// Interaction logic for ThalamusStatus.xaml
    /// </summary>
    public partial class ThalamusStatus
    {
        private readonly ThalamusStatusViewModel _data;
        public ThalamusStatus()
        {
            InitializeComponent();
            _data = (ThalamusStatusViewModel) DataContext;
        }

        public ThalamusClient WatchedClient
        {
            get { return (ThalamusClient)GetValue(WatchedClientProperty); }
            set
            {
                if (WatchedClient != null)
                {
                    WatchedClient.ClientConnected -= WatchedClientOnClientConnected;
                    WatchedClient.ClientDisconnected -= WatchedClientOnClientDisconnected;
                }
                SetValueDp(WatchedClientProperty, value);
                if (WatchedClient != null)
                {
                    WatchedClient.ClientConnected += WatchedClientOnClientConnected;
                    WatchedClient.ClientDisconnected += WatchedClientOnClientDisconnected;
                    _data.IsConnected = WatchedClient.IsConnected;
                }
            }
        }

        private void WatchedClientOnClientDisconnected()
        {
            _data.IsConnected = true;
        }

        private void WatchedClientOnClientConnected()
        {
            _data.IsConnected = true;
        }

        public static readonly DependencyProperty WatchedClientProperty =
            DependencyProperty.Register("WatchedClient", typeof(IThalamusClient), typeof(ThalamusStatus), null);

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value,
            [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}

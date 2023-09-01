using System.Windows;
using GeolocationApp.ViewModel;
using Unity;

namespace GeolocationApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [Dependency]
        public GeolocationViewModel IpViewModel
        {
            set => DataContext = value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GeolocationViewModel viewModel) viewModel.OnWindowLoaded();
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
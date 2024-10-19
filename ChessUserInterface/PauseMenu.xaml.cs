using System.Windows;
using System.Windows.Controls;

namespace ChessUserInterface
{
    public partial class PauseMenu : UserControl
    {
        public event Action<Options> OptionSelected;
        public PauseMenu()
        {
            InitializeComponent();
        }

        private void Continue_click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Options.Continue);
        }

        private void Exit_click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Options.Exit);
        }
    }
}

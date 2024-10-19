using System.Windows;
using System.Windows.Controls;

namespace ChessUserInterface
{
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            LoadLeaders();
        }

        private void LoadLeaders()
        {
            List<User> leaders = DatabaseHelper.GetLeaders();
            LeadersList.ItemsSource = leaders;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            string fenString;
            int selectedDepth = 5;
            int playerColor = 1;

            if (FenInput.Text == "")
            {
                fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            }
            else
            {
                fenString = FenInput.Text;
            }

            if (DifficultyLevel.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedDepth = int.Parse((string)selectedItem.Tag);
            }

            if (ColorSelection.SelectedItem is ComboBoxItem selectedColor)
            {
                playerColor = int.Parse((string)selectedColor.Tag);
            }

            if (FenValidation.IsValidFEN(fenString))
            {
                MainWindow gameWindow = new MainWindow(fenString, selectedDepth, playerColor);
                gameWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверная FEN строка!");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public class User
        {
            public string Name { get; set; }
            public int Wins { get; set; }
        }
    }
}

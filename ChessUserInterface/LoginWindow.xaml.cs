using System.Windows;
using System.Windows.Controls;

namespace ChessUserInterface
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (DatabaseHelper.ValidateUser(username, password))
            {
                int wins = DatabaseHelper.GetWins(username);
                int losses = DatabaseHelper.GetLosses(username);
                int draws = DatabaseHelper.GetDraws(username);
                MessageBox.Show($"Welcome {username}! Wins: {wins}, Losses: {losses}, Draws: {draws}");
                MainMenu mainMenu = new MainMenu();
                mainMenu.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(this);
            registerWindow.Show();
            this.Hide();
        }

    }
}

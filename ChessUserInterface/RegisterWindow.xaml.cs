using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace ChessUserInterface
{
    public partial class RegisterWindow : Window
    {
        private Window loginWindow;
        public RegisterWindow(Window loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;

            try
            {
                DatabaseHelper.AddUser(username, password, email);
                MessageBox.Show("Успешная регистрация!");
                this.Close();
                loginWindow.Show();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Регистрация провалена: " + ex.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            loginWindow.Show();
        }

    }
}

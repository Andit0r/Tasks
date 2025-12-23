using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using System.Windows;

namespace WpfApp
{
    public partial class LoginWindow : Window
    {
        private readonly ShoesMarketContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ShoesMarketContext();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user is not null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
                MessageBox.Show("Неверный логин или пароль");
        }
    }
}
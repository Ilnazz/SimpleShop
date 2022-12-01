using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SessionProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        #region Таймер авторизации

        public int AuthorizationTimerSeconds
        {
            get { return (int)GetValue(AuthorizationTimerSecondsProperty); }
            set { SetValue(AuthorizationTimerSecondsProperty, value); }
        }
        public static readonly DependencyProperty AuthorizationTimerSecondsProperty =
            DependencyProperty.Register("AuthorizationTimerSeconds", typeof(int), typeof(AuthorizationPage), new PropertyMetadata(0));

        private DispatcherTimer _authorizationTimer = new DispatcherTimer();
        private void AuthorizationTimer_Tick(object sender, EventArgs e)
        {
            if (AuthorizationTimerSeconds > 0)
                AuthorizationTimerSeconds--;
            else
            {
                _authorizationTimer.Stop();
                Properties.Settings.Default.LastAuthorizationAttemptTime = TimeSpan.Zero;
                ButtonAuthorize.IsEnabled = true;
                AuthorizationTimerBox.Visibility = Visibility.Collapsed;
                _tryNumber = 0;
            }
        }

        #endregion
        public AuthorizationPage()
        {
            InitializeComponent();

            if (Properties.Settings.Default.UserLogin != null)
                TBLogin.Text = Properties.Settings.Default.UserLogin;
            if (Properties.Settings.Default.UserPassword != null)
                PBPassword.Password = Properties.Settings.Default.UserPassword;
            Properties.Settings.Default.Save();

            _authorizationTimer.Tick += new EventHandler(AuthorizationTimer_Tick);
            _authorizationTimer.Interval = new TimeSpan(0, 0, 1); // интервал между тиками в секунду

            if (Properties.Settings.Default.LastAuthorizationAttemptTime != TimeSpan.Zero)
            {
                var timesOffset = DateTime.Now.TimeOfDay - Properties.Settings.Default.LastAuthorizationAttemptTime;

                if (timesOffset.Seconds < 60) // минута ещё не прошла
                {
                    // продолжить отсчёт
                    AuthorizationTimerSeconds = timesOffset.Seconds;
                    _authorizationTimer.Start();
                    
                    ButtonAuthorize.IsEnabled = false;
                    AuthorizationTimerBox.Visibility = Visibility.Visible;
                }
                else
                {
                    Properties.Settings.Default.LastAuthorizationAttemptTime = TimeSpan.Zero;
                    Properties.Settings.Default.Save();
                    _tryNumber = 0;
                }
            }
        }

        private int _tryNumber = 0;

        private void ButtonAuthorize_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = TBLogin.Text.Trim();
            PBPassword.Password = PBPassword.Password.Trim();

            if (TBLogin.Text == ""
                || PBPassword.Password == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            _tryNumber++;

            if (_tryNumber > 3) // это 4-ая попытка входа
            {
                _tryNumber = 0;
                Properties.Settings.Default.LastAuthorizationAttemptTime = DateTime.Now.TimeOfDay;
                Properties.Settings.Default.Save();
                AuthorizationTimerSeconds = 60;
                _authorizationTimer.Start();
                ButtonAuthorize.IsEnabled = false;
                AuthorizationTimerBox.Visibility = Visibility.Visible;
                return;
            }

            var user = App.DB.Users.FirstOrDefault(u => u.Login == TBLogin.Text && u.Password == PBPassword.Password);
            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данным не зарегистрирован");
                return;
            }

            if (CBRememberUser.IsChecked == true)
            {
                Properties.Settings.Default.UserLogin = TBLogin.Text;
                Properties.Settings.Default.UserPassword = PBPassword.Password;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.UserLogin = null;
                Properties.Settings.Default.UserPassword = null;
                Properties.Settings.Default.Save();
            }

            // do actions by redirection
            MessageBox.Show("Пользователь успешно авторизован");
        }
    }
}

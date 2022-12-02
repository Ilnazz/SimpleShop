using SessionProject.Pages;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SessionProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        #region Таймер обратного отсчёта

        public int AuthorizationTimerSeconds
        {
            get { return (int)GetValue(AuthorizationTimerSecondsProperty); }
            set { SetValue(AuthorizationTimerSecondsProperty, value); }
        }
        public static readonly DependencyProperty AuthorizationTimerSecondsProperty =
            DependencyProperty.Register("AuthorizationTimerSeconds", typeof(int), typeof(AuthorizationWindow), new PropertyMetadata(0));

        private readonly DispatcherTimer _authorizationTimer = new DispatcherTimer();
        private void AuthorizationTimer_Tick(object sender, EventArgs e)
        {
            if (AuthorizationTimerSeconds > 0)
                AuthorizationTimerSeconds--;
            else
            {
                ResetAuthorizationTimer();
                _authorizationAttemptNumber = 0;
            }
        }

        private void StartAuthorizationTimer(int seconds)
        {
            AuthorizationTimerSeconds = seconds;
            _authorizationTimer.Start();

            ButtonAuthorize.IsEnabled = false;
            AuthorizationTimerBox.Visibility = Visibility.Visible;
        }

        private void ResetAuthorizationTimer()
        {
            _authorizationTimer.Stop();

            Properties.Settings.Default.LastAuthorizationAttemptDateTime = DateTime.MinValue;
            Properties.Settings.Default.Save();

            ButtonAuthorize.IsEnabled = true;
            AuthorizationTimerBox.Visibility = Visibility.Collapsed;
        }

        private void InitializeAuthorizationTimer()
        {
            _authorizationTimer.Tick += new EventHandler(AuthorizationTimer_Tick);
            _authorizationTimer.Interval = new TimeSpan(0, 0, 1);

            if (Properties.Settings.Default.LastAuthorizationAttemptDateTime != DateTime.MinValue)
            {
                var dateTimesOffset = DateTime.Now - Properties.Settings.Default.LastAuthorizationAttemptDateTime;

                if (dateTimesOffset.TotalSeconds < 60)
                    StartAuthorizationTimer(60 - (int)dateTimesOffset.TotalSeconds);
                else
                {
                    Properties.Settings.Default.LastAuthorizationAttemptDateTime = DateTime.MinValue;
                    Properties.Settings.Default.Save();
                }
            }
        }

        #endregion

        private int _authorizationAttemptNumber = 0;

        public AuthorizationWindow()
        {
            InitializeComponent();

            FillRememberedUserLoginAndPassword();

            InitializeAuthorizationTimer();
        }

        private void ButtonAuthorize_Click(object sender, RoutedEventArgs e)
        {
            NormalizeInputData();

            if (AreAllFieldsFilled() == false)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            _authorizationAttemptNumber++;

            if (_authorizationAttemptNumber > 3) // это 4-ая попытка входа
            {
                _authorizationAttemptNumber = 0;

                Properties.Settings.Default.LastAuthorizationAttemptDateTime = DateTime.Now;
                Properties.Settings.Default.Save();

                StartAuthorizationTimer(60);

                return;
            }

            var user = App.DB.Users.FirstOrDefault(u => u.Login == TBLogin.Text && u.Password == PBPassword.Password);
            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данным не зарегистрирован");
                return;
            }

            if (CBRememberUser.IsChecked == true)
                RememberUserLoginAndPassword();
            else
                ResetRememberedUserLoginAndPassword();

            MessageBox.Show("Пользователь авторизован");

            App.CurrentUser = user;

            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Activate();

            Close();
        }

        private void NormalizeInputData()
        {
            TBLogin.Text = TBLogin.Text.Trim();
            PBPassword.Password = PBPassword.Password.Trim();
        }

        private bool AreAllFieldsFilled()
            => TBLogin.Text != ""
                && PBPassword.Password != "";

        private void FillRememberedUserLoginAndPassword()
        {
            if (Properties.Settings.Default.UserLogin != null)
                TBLogin.Text = Properties.Settings.Default.UserLogin;
            if (Properties.Settings.Default.UserPassword != null)
                PBPassword.Password = Properties.Settings.Default.UserPassword;
        }

        private void RememberUserLoginAndPassword()
        {
            Properties.Settings.Default.UserLogin = TBLogin.Text;
            Properties.Settings.Default.UserPassword = PBPassword.Password;
            Properties.Settings.Default.Save();
        }

        private void ResetRememberedUserLoginAndPassword()
        {
            Properties.Settings.Default.UserLogin = null;
            Properties.Settings.Default.UserPassword = null;
            Properties.Settings.Default.Save();
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }
    }
}

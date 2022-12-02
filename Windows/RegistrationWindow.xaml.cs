using SessionProject.Components;
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

namespace SessionProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void ButtonRegiser_Click(object sender, RoutedEventArgs e)
        {
            NormalizeInputData();

            if (AreAllFieldsFilled() == false)
            {
                MessageBox.Show("Заполните все данные");
                return;
            }

            if (RBGenderMale.IsChecked == null)
            {
                MessageBox.Show("Выберите пол");
                return;
            }

            if (IsLoginValid(TBLogin.Text) == false)
            {
                MessageBox.Show("Логин должен содержать как минимум: " +
                    "6 символов, " +
                    "1 прописную букву, " +
                    "1 цифру, " +
                    "1 символ из набора - !@#$%^");
                return;
            }

            var isThisPhoneNumberExists = App.DB.Users.Any(user => user.PhoneNumber == TBPhoneNumber.Text);
            if (isThisPhoneNumberExists == true)
            {
                MessageBox.Show("Пользователь с таким номером телефона уже зарегистрирован");
                return;
            }

            var isThisEmailExists = App.DB.Users.Any(user => user.Email == TBEmail.Text);
            if (isThisEmailExists == true)
            {
                MessageBox.Show("Пользователь с таким адресом электронной почты уже зарегистрирован");
                return;
            }

            var isThisLoginExists = App.DB.Users.Any(user => user.Login == TBLogin.Text);
            if (isThisLoginExists == true)
            {
                MessageBox.Show("Пользователь с таким логином уже зарегистрирован");
                return;
            }

            var newUser = CreateNewUser();
            App.DB.Users.Add(newUser);
            App.DB.SaveChanges();

            MessageBox.Show("Пользователь зарегистрирован");
        }

        private void NormalizeInputData()
        {
            TBLastName.Text = TBLastName.Text.Trim();
            TBFirstName.Text = TBFirstName.Text.Trim();
            TBPatronymic.Text = TBPatronymic.Text.Trim();
            TBPhoneNumber.Text = TBPhoneNumber.Text.Trim();
            TBEmail.Text = TBEmail.Text.Trim();
            TBLogin.Text = TBLogin.Text.Trim();
            PBPassword.Password = PBPassword.Password.Trim();
        }

        private bool AreAllFieldsFilled()
            => TBLastName.Text != ""
                && TBFirstName.Text != ""
                && TBPatronymic.Text != ""
                && TBPhoneNumber.Text != ""
                && TBEmail.Text != ""
                && TBLogin.Text != ""
                && PBPassword.Password != "";

        private bool IsLoginValid(string login)
        {
            var specialSymbols = "!@#$%^";
            return login.Length < 6 // минимум 6 символов
                || login.Any(letter => char.IsUpper(letter)) == false // минимум одна прописная буква
                || login.Any(letter => char.IsDigit(letter)) == false // минимум одна цифра
                || login.Any(letter => specialSymbols.Contains(letter)) == false; // минимум один символ из набора !@#$%^
        }

        private User CreateNewUser()
        {
            var newUser = new User();
            newUser.Role = App.DB.Roles.First(role => role.Title == "Customer");
            newUser.LastName = TBLastName.Text;
            newUser.FirstName = TBFirstName.Text;
            newUser.Patronymic = TBPatronymic.Text;

            if (RBGenderMale.IsChecked == true)
                newUser.Gender = App.DB.Genders.First(gender => gender.Title == "Male");
            else
                newUser.Gender = App.DB.Genders.First(gender => gender.Title == "Female");

            newUser.PhoneNumber = TBPhoneNumber.Text;
            newUser.Email = TBEmail.Text;
            newUser.Login = TBLogin.Text;
            newUser.Password = PBPassword.Password;

            return newUser;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using SessionProject.Components;

namespace SessionProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        /// <summary>
        /// Фотография продукта
        /// </summary>
        public byte[] Photo
        {
            get { return (byte[])GetValue(PhotoProperty); }
            set { SetValue(PhotoProperty, value); }
        }
        public static readonly DependencyProperty PhotoProperty =
            DependencyProperty.Register("Photo", typeof(byte[]), typeof(AddEditProductWindow), new PropertyMetadata(null));

        /// <summary>
        /// Страны поставщики данного продукта
        /// </summary>
        public ObservableCollection<SupplierCountry> SupplierCountries
        {
            get { return (ObservableCollection<SupplierCountry>)GetValue(SupplierCountriesProperty); }
            set { SetValue(SupplierCountriesProperty, value); }
        }
        public static readonly DependencyProperty SupplierCountriesProperty =
            DependencyProperty.Register("SupplierCountries", typeof(ObservableCollection<SupplierCountry>), typeof(AddEditProductWindow), new PropertyMetadata(null));

        /// <summary>
        /// Редактируемый продукт
        /// </summary>
        private Product _editingProduct;

        /// <summary>
        /// Создаёт окно редактирования продукта
        /// </summary>
        /// <param name="product">Продукт для редактирования или null (создаётся новый)</param>
        public AddEditProductWindow(Product product = null)
        {
            InitializeComponent();

            InitializeEditingProduct(product);

            InitializeMeasureUnitRadioButtons();

            FillFiledsWithProductData();

            ToggleAddReplacePhotoBtnContent();

            ToggleSupplierCountryListVisibility();
        }

        /// <summary>
        /// Устанавливает переданный продукт в кач-ве редактируемого продукта;
        /// Создаёт новый, если передан null
        /// </summary>
        /// <param name="product">Продукт, который будет редактироваться или null</param>
        private void InitializeEditingProduct(Product product)
        {
            if (product != null)
                _editingProduct = product;
            else
            {
                _editingProduct = new Product();
                _editingProduct.AdditionDateTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Создаёт кнопки-переключатели для возможности выбора любой имеющейся единицы измерения
        /// </summary>
        private void InitializeMeasureUnitRadioButtons()
        {
            App.DB.MeasureUnits.ToList().ForEach(mu =>
            {
                MeasureUnitRadioButtons.Children.Add(
                    new RadioButton
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 5, 0),
                        Content = mu.Title,
                        Tag = mu.ID
                    }
                );
            });

            MeasureUnitRadioButtons.Children.Cast<RadioButton>().Last().Margin = new Thickness();
        }

        /// <summary>
        /// Заполняет поля формы данными из продукта
        /// </summary>
        private void FillFiledsWithProductData()
        {
            // Фотография
            if (_editingProduct.Photo != null)
                Photo = _editingProduct.Photo;

            // Текстовые поля
            TBIdentificator.Text = $"Идентификатор: {_editingProduct.ID}";
            TBAdditionDateTime.Text = $"{_editingProduct.AdditionDateTime:dd.MM.yyyy HH:mm}";
            TBTitle.Text = _editingProduct.Title;
            TBCost.Text = $"{_editingProduct.Cost:f}";
            TBDescription.Text = _editingProduct.Description;

            // Страны поставщики
            SupplierCountries = new ObservableCollection<SupplierCountry>(_editingProduct.SupplierCountries);
            
            // Единица измерения
            if (_editingProduct.MeasureUnit != null)
            {
                MeasureUnitRadioButtons.Children.Cast<RadioButton>().ToList().ForEach(murb => {
                    if ((int)murb.Tag == _editingProduct.MeasureUnitID)
                    {
                        murb.IsChecked = true;
                        return;
                    }
                });
            }
        }

        /// <summary>
        /// Предлагает выбрать фото для продукта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddReplacePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "All image files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg",
                CheckPathExists = true
            };
            
            if (openFileDialog.ShowDialog().GetValueOrDefault() == false)
                return;
            
            var photoLengthInBytes = new FileInfo(openFileDialog.FileName).Length;
            var photoLengthInKiloBytes = photoLengthInBytes / 1024;

            if (photoLengthInKiloBytes > 150)
            {
                MessageBox.Show("Размер фотографии не должен превышать 150 килобайт");
                return;
            }

            Photo = File.ReadAllBytes(openFileDialog.FileName);

            ToggleAddReplacePhotoBtnContent();
        }

        /// <summary>
        /// Меняет текст кнопки добавления фотографии в зависимости от наличия фотографии
        /// </summary>
        private void ToggleAddReplacePhotoBtnContent()
        {
            if (Photo != null)
                BtnAddReplacePhoto.Content = "Заменить фотографию";
            else
                BtnAddReplacePhoto.Content = "Добавить фотографию";
        }

        /// <summary>
        /// Проверяет наличие и корректность введённых данных и записывает данные в продукт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            NormalizeInputData();

            if (ValidateAllData() == false)
                return;

            _editingProduct.Photo = Photo;
            _editingProduct.Title = TBTitle.Text;
            _editingProduct.Cost = decimal.Parse(TBCost.Text);
            _editingProduct.Description = TBDescription.Text;

            MeasureUnitRadioButtons.Children.Cast<RadioButton>().ToList().ForEach(murb =>
            {
                if (murb.IsChecked == true)
                {
                    _editingProduct.MeasureUnitID = int.Parse(murb.Tag.ToString());
                }
            });

            _editingProduct.SupplierCountries.Clear();
            SupplierCountries.ToList().ForEach(sc => _editingProduct.SupplierCountries.Add(sc));

            if (_editingProduct.ID == 0)
                App.DB.Products.Local.Add(_editingProduct);

            App.DB.SaveChanges();

            Close();
        }

        /// <summary>
        /// Проверяет наличие и корректность заполнения всех данных
        /// </summary>
        /// <returns></returns>
        private bool ValidateAllData()
            => ValidatePhoto() == true
                && ValidateTitle() == true
                && ValidateCost() == true
                && ValidateDescription() == true
                && ValidateMeasureUnit() == true
                && ValidateSupplierCountries() == true;

        /// <summary>
        /// Удаляет ненужные пробелы у текстовых полей ввода
        /// </summary>
        private void NormalizeInputData()
        {
            TBTitle.Text = TBTitle.Text.Trim();
            TBDescription.Text = TBDescription.Text.Trim();
            TBCost.Text = TBCost.Text.Trim();
        }

        /// <summary>
        /// Проверяет, выбрана ли фотография
        /// </summary>
        /// <returns></returns>
        private bool ValidatePhoto()
        {
            if (Photo == null)
            {
                MessageBox.Show("Необходимо выбрать фотографию продукта");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет, правильно ли задано название
        /// </summary>
        /// <returns></returns>
        private bool ValidateTitle()
        {
            if (TBTitle.Text == "")
            {
                MessageBox.Show("Название продукта не может быть пустым");
                return false;
            }
            else if (Regex.IsMatch(TBTitle.Text, @"^[\/\da-яА-Яa-zA-Z\- ]+$") == false)
            {
                MessageBox.Show("Название продукта должно содержать только буквы, пробелы и дефисы");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет, правильно ли задана цена
        /// </summary>
        /// <returns></returns>
        private bool ValidateCost()
        {
            if (TBCost.Text == "")
            {
                MessageBox.Show("Необхоимо назначить цену");
                return false;
            }
            else if (Regex.IsMatch(TBCost.Text, @"^\d+([,\.]\d+)?$") == false)
            {
                MessageBox.Show("Стоимость продукта должна быть задан десятичным числом");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Првоеряет, правильно ли задано описание
        /// </summary>
        /// <returns></returns>
        private bool ValidateDescription()
        {
            if (TBDescription.Text == "")
            {
                MessageBox.Show("Описание продукта не может быть пустым");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет, выбрана ли единица измерения
        /// </summary>
        /// <returns></returns>
        private bool ValidateMeasureUnit()
        {
            var isMeasureUnitSelected =
                MeasureUnitRadioButtons.Children
                    .Cast<RadioButton>()
                    .Any(rb => rb.IsChecked == true);
            
            if (isMeasureUnitSelected == false)
            {
                MessageBox.Show("Необходимо выбрать единицу измерения");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет, выбрана ли хотя бы одна страна поставщик
        /// </summary>
        /// <returns></returns>
        private bool ValidateSupplierCountries()
        {
            if (SupplierCountries.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать как минимум одну страну поставщика");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Позволяет выбрать страну поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddSupplierCountry_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierCountries.Count == App.DB.SupplierCountries.Local.Count)
            {
                MessageBox.Show("Все имеющиеся страны поставщики уже добавлены.");
                return;
            }

            // Те страны поставщики, которые не привязаны к продукту
            var availableCountries = App.DB.SupplierCountries.Local
                .Where(sc => SupplierCountries.Contains(sc) == false);

            var selectSCWindow = new SelectSupplierCountryWindow(availableCountries);
            selectSCWindow.ShowDialog();

            SupplierCountries.Add(selectSCWindow.SelectedSupplierCountry);

            ToggleSupplierCountryListVisibility();
        }

        /// <summary>
        /// Удаляет выбранную страну поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoveSupplierCountry_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierCountries.Count == 0)
            {
                MessageBox.Show("У продукта нет стран поставщиков.");
                return;
            }
            
            if (SupplierCountryList.SelectedItem is SupplierCountry selectedSP == false)
            {
                MessageBox.Show("Выберите страну поставщика для удаления");
                return;
            }

            SupplierCountries.Remove(selectedSP);

            ToggleSupplierCountryListVisibility();
        }

        /// <summary>
        /// Скрывает/показыват список стран поставщиков продукта и сообщение на форме, если их нет
        /// </summary>
        private void ToggleSupplierCountryListVisibility()
        {
            if (SupplierCountries.Count == 0)
            {
                SupplierCountryList.Visibility = Visibility.Collapsed;
                TBThereAreNoSuppliers.Visibility = Visibility.Visible;
            }
            else
            {
                SupplierCountryList.Visibility = Visibility.Visible;
                TBThereAreNoSuppliers.Visibility = Visibility.Collapsed;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

using SessionProject.Components;
using SessionProject.Windows;

namespace SessionProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductListPage.xaml
    /// </summary>
    public partial class ProductListPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        public enum Filtering
        {
            All, Gramm, Kilogramm, Milliliter, Liter, Piece, Package
        }
        private readonly Dictionary<Filtering, string> _filteringTitles = new Dictionary<Filtering, string>()
        {
            { Filtering.All, "Все" },
            { Filtering.Gramm, "Грамм" },
            { Filtering.Kilogramm, "Килограмм" },
            { Filtering.Milliliter, "Миллилитр" },
            { Filtering.Liter, "Литр" },
            { Filtering.Piece, "Штука" },
            { Filtering.Package, "Упаковка" }
        };
        public IEnumerable<string> FilteringTitles { get => _filteringTitles.Values; }
        public Filtering CurrentFiltering { get => _filteringTitles.Keys.ElementAt(CBFiltering?.SelectedIndex ?? 0); }



        public enum Sorting
        {
            None, TitleAscending, AdditionDateTimeAscending
        }
        private readonly Dictionary<Sorting, string> _sortingTitles = new Dictionary<Sorting, string>()
        {
            { Sorting.None, "Нет" },
            { Sorting.TitleAscending, "По наименованию (в алфавитном порядке)" },
            { Sorting.AdditionDateTimeAscending, "По дате добавления (от новых к старым)" }
        };
        public IEnumerable<string> SortingTitles { get => _sortingTitles.Values; }
        public Sorting CurrentSorting { get => _sortingTitles.Keys.ElementAt(CBSorting?.SelectedIndex ?? 0); }



        public enum ItemsNumberPerPage
        {
            One, Five, Ten, Fifty, TwoHundred, All
        }
        private readonly Dictionary<ItemsNumberPerPage, string> _itemsNumberPerPageTitles = new Dictionary<ItemsNumberPerPage, string>()
        {
            { ItemsNumberPerPage.One, "1" },
            { ItemsNumberPerPage.Five, "5" },
            { ItemsNumberPerPage.Ten, "10" },
            { ItemsNumberPerPage.Fifty, "50" },
            { ItemsNumberPerPage.TwoHundred, "200" },
            { ItemsNumberPerPage.All, "Все" }
        };
        public IEnumerable<string> ItemsNumberPerPageTitles { get => _itemsNumberPerPageTitles.Values; }
        public ItemsNumberPerPage CurrentItemsNumberPerPage { get => _itemsNumberPerPageTitles.Keys.ElementAt(CBProductsNumberPerPage?.SelectedIndex ?? 0); }


        private int _totalPagesNumber;
        public int TotalPagesNumber
        {
            get => _totalPagesNumber;
            set
            {
                _totalPagesNumber = value;
                OnPropertyChanged("DisplayedProductsNumber");
                OnPropertyChanged("TotalProductsNumber");
            }
        }
        private int _currentPageNumber = 1;
        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                _currentPageNumber = value;
                OnPropertyChanged("DisplayedProductsNumber");
                OnPropertyChanged("TotalProductsNumber");
            }
        }
        private int _currentProductsNumberPerPage;
        public int CurrentProductsNumberPerPage
        {
            get => _currentProductsNumberPerPage;
            set
            {
                _currentProductsNumberPerPage = value;
                OnPropertyChanged("DisplayedProductsNumber");
                OnPropertyChanged("TotalProductsNumber");
            }
        }

        public int DisplayedProductsNumber { get => _currentPageNumber * _currentProductsNumberPerPage; }
        public int TotalProductsNumber { get => _totalPagesNumber * _currentProductsNumberPerPage; }



        public ObservableCollection<Product> DisplayedProducts
        {
            get { return (ObservableCollection<Product>)GetValue(DisplayedProductsProperty); }
            set { SetValue(DisplayedProductsProperty, value); }
        }
        public static readonly DependencyProperty DisplayedProductsProperty =
            DependencyProperty.Register("DisplayedProducts", typeof(ObservableCollection<Product>), typeof(ProductListPage), new PropertyMetadata(null));



        private void Refresh()
        {
            var filteredProducts = Filter(App.DB.Products.Local);
            var sortedProducts = Sort(filteredProducts);
            var pagedProducts = Page(sortedProducts);

            DisplayedProducts = new ObservableCollection<Product>(pagedProducts);
        }

        private IEnumerable<Product> Filter(IEnumerable<Product> products)
        {
            return products.Where(p =>
            {
                if (CBAddedInCurrentMonth?.IsChecked == true
                    && p.AdditionDateTime.Month != DateTime.Now.Month)
                    return false;

                if (CurrentFiltering == Filtering.Gramm && p.MeasureUnit.Title != "Грамм"
                    || CurrentFiltering == Filtering.Kilogramm && p.MeasureUnit.Title != "Килограмм"
                    || CurrentFiltering == Filtering.Milliliter && p.MeasureUnit.Title != "Миллилитр"
                    || CurrentFiltering == Filtering.Liter && p.MeasureUnit.Title != "Литр"
                    || CurrentFiltering == Filtering.Piece && p.MeasureUnit.Title != "Штука"
                    || CurrentFiltering == Filtering.Package && p.MeasureUnit.Title != "Упаковка")
                    return false;

                if (SearchBox.IsEmpty() == false)
                {
                    var searchText = SearchBox.SearchText.ToLower().Trim();
                    if (p.Title.ToLower().Contains(searchText) == false
                        && p.Description.ToLower().Contains(searchText) == false)
                        return false;
                }

                return true;
            });
        }

        private IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            if (CurrentSorting == Sorting.TitleAscending)
                return products.OrderBy(p => p.Title);
            else if (CurrentSorting == Sorting.AdditionDateTimeAscending)
                return products.OrderBy(p => p.AdditionDateTime);
            return products;
        }

        private IEnumerable<Product> Page(IEnumerable<Product> products)
        {
            if (CurrentItemsNumberPerPage == ItemsNumberPerPage.All)
            {
                TotalPagesNumber = 1;
                CurrentPageNumber = 1;
                CurrentProductsNumberPerPage = products.Count();
                return products;
            }

            CurrentProductsNumberPerPage = int.Parse(_itemsNumberPerPageTitles[CurrentItemsNumberPerPage]);
            if (products.Count() < CurrentProductsNumberPerPage)
            {
                TotalPagesNumber = 1;
                CurrentPageNumber = 1;
                CurrentProductsNumberPerPage = products.Count();
                return products;
            }

            TotalPagesNumber = (int)Math.Ceiling((double)(products.Count() / CurrentProductsNumberPerPage));

            return products.Skip((CurrentPageNumber - 1) * CurrentProductsNumberPerPage)
                           .Take(CurrentProductsNumberPerPage);
        }

        public ProductListPage()
        {
            InitializeComponent();

            if (App.CurrentUser.RoleID == 4)
            {
                Buttons.Visibility = Visibility.Collapsed;
            }

            SearchBox.TextChanged += Refresh;
            CBSorting.SelectionChanged += (s, e) => Refresh();
            CBFiltering.SelectionChanged += (s, e) => Refresh();
            CBAddedInCurrentMonth.Click += (s, e) => Refresh();
            CBProductsNumberPerPage.SelectionChanged += (s, e) => Refresh();

            BtnGoToPreviousPage.Click += (s, e) =>
            {
                if (CurrentPageNumber > 1)
                    CurrentPageNumber--;
                Refresh();
            };

            BtnGoToNextPage.Click += (s, e) =>
            {
                if (CurrentPageNumber < TotalPagesNumber)
                    CurrentPageNumber++;
                Refresh();
            };
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addEditProductWindow = new AddEditProductWindow();
            addEditProductWindow.ShowDialog();
            Refresh();
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductList.SelectedItem is Product selectedProduct == false)
            {
                MessageBox.Show("Выберите продукт для редактирования");
                return;
            }
            var addEditProductWindow = new AddEditProductWindow(selectedProduct);
            addEditProductWindow.ShowDialog();
            Refresh();
        }
    }
}

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

namespace SessionProject.Components
{
    /// <summary>
    /// Логика взаимодействия для SearchBoxComponent.xaml
    /// </summary>
    public partial class SearchBoxComponent : UserControl
    {
        public string SearchPlaceHolder
        {
            get { return (string)GetValue(SearchPlaceHolderProperty); }
            set { SetValue(SearchPlaceHolderProperty, value); }
        }
        public static readonly DependencyProperty SearchPlaceHolderProperty =
            DependencyProperty.Register("SearchPlaceHolder", typeof(string), typeof(SearchBoxComponent), new PropertyMetadata(""));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBoxComponent), new PropertyMetadata(""));

        public event Action TextChanged
        {
            add { TBSearch.TextChanged += (s, e) => value?.Invoke(); }
            remove { TBSearch.TextChanged -= (s, e) => value?.Invoke(); }
        }

        public SearchBoxComponent()
        {
            InitializeComponent();
        }

        public void Clear() => SearchText = SearchPlaceHolder;
        public bool IsEmpty() => SearchText == SearchPlaceHolder;

        private void OnLoaded(object sender, RoutedEventArgs e)
            => SearchText = SearchPlaceHolder;

        private void TBSearch_GotFocus(object sender, RoutedEventArgs e)
            => SearchText = SearchText == SearchPlaceHolder ? "" : SearchText;

        private void TBSearch_LostFocus(object sender, RoutedEventArgs e)
            => SearchText = string.IsNullOrWhiteSpace(SearchText) ? SearchPlaceHolder : SearchText;
    }
}

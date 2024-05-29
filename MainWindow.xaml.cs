using MainProject27.ViewModel;
using System.Windows;
using FindYourMobilePhone.View;
using System.Windows.Controls;

namespace MainProject27
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            Author author = new Author();
            author.ShowDialog();
        }
    }
}
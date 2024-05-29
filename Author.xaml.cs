using System.Windows;

namespace FindYourMobilePhone.View
{
    /// <summary>
    /// Interaction logic for Author.xaml
    /// </summary>
    public partial class Author : Window
    {
        public Author()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

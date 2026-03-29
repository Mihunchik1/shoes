using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZOO_magazin.Pages;

namespace ZOO_magazin.Pages
{
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();

            ComboSortBy.SelectedIndex = 0;
            ComboSort.SelectedIndex = 0;

            string role_id = "";
            if (App.CurrentUser != null)
            {
                role_id = App.CurrentUser.Role_id.ToString();
            }

            string userRole = "";

            if (role_id == "1")
            {
                userRole = "Администратор";
            }
            else if (role_id == "2")
            {
                userRole = "Менеджер";
            }
            else if (role_id == "3")
            {
                userRole = "Клиент";
            }
            else
            {
                userRole = "Гость";
            }

            if (App.CurrentUser != null)
            {
                UserInfoLabel.Text = $"Пользователь: {userRole} {App.CurrentUser.Name}";
            }
            else
            {
                UserInfoLabel.Text = "Гость";
                BtnAdd.Visibility = Visibility.Collapsed;
            }

            UpdateProductList();
        }

        private void UpdateProductList()
        {
            try
            {
                var products = App.Context.Product.ToList();

                if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
                {
                    products = products.Where(p =>
                        p.Name.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
                        (p.Articul != null && p.Articul.ToLower().Contains(TBoxSearch.Text.ToLower())) ||
                        (p.Discription != null && p.Discription.ToLower().Contains(TBoxSearch.Text.ToLower()))
                    ).ToList();
                }

                if (ComboSort.SelectedIndex > 0)
                {
                    switch (ComboSort.SelectedIndex)
                    {
                        case 1: // Менее 10 шт
                            products = products.Where(p => p.CountSklad < 10).ToList();
                            break;
                        case 2: // 10-50 шт
                            products = products.Where(p => p.CountSklad >= 10 && p.CountSklad <= 50).ToList();
                            break;
                        case 3: // Более 50 шт
                            products = products.Where(p => p.CountSklad > 50).ToList();
                            break;
                    }
                }

                // Сортировка по цене
                if (ComboSortBy.SelectedIndex == 1) // По возрастанию
                {
                    products = products.OrderBy(p => p.Price).ToList();
                }
                else if (ComboSortBy.SelectedIndex == 2) // По убыванию
                {
                    products = products.OrderByDescending(p => p.Price).ToList();
                }
                else // По умолчанию
                {
                    products = products.OrderBy(p => p.Name).ToList();
                }

                // Добавляем картинки
                string[] images = { "1.jpg", "3.jpg", "4.jpg", "7.jpg" };
                int imageIndex = 0;

                foreach (var product in products)
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri($"pack://application:,,,/Resources/{images[imageIndex % images.Length]}", UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        product.Tag = bitmap;
                    }
                    catch
                    {
                        product.Tag = null;
                    }
                    imageIndex++;
                }

                LviewProduct.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Для добавления товаров необходимо авторизоваться.");
                return;
            }
            NavigationService.Navigate(new AddEditProductPage());
        }

        private void ComboSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProductList();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Для редактирования товаров необходимо авторизоваться.");
                return;
            }

            var currentProduct = (sender as Button).DataContext as Entities.Product;
            NavigationService.Navigate(new AddEditProductPage(currentProduct));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Для удаления товаров необходимо авторизоваться.");
                return;
            }

            var currentProduct = (sender as Button)?.DataContext as Entities.Product;

            if (MessageBox.Show($"Вы уверены, что хотите удалить товар '{currentProduct.Name}'?",
                "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    App.Context.Product.Remove(currentProduct);
                    App.Context.SaveChanges();
                    UpdateProductList();
                    MessageBox.Show("Товар успешно удален.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }
    }
}
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ZOO_magazin.Pages
{
    public partial class AddEditProductPage : Page
    {
        private Entities.Product _currentProduct = null;

        public AddEditProductPage()
        {
            InitializeComponent();
            TitleText.Text = "Добавить товар";
            ComboUnit.SelectedIndex = 0;
        }

        public AddEditProductPage(Entities.Product product)
        {
            InitializeComponent();
            _currentProduct = product;
            TitleText.Text = "Редактировать товар";

            // Заполнение полей
            TBart.Text = _currentProduct.Articul;
            TextBoxName.Text = _currentProduct.Name;
            TextBoxDesc.Text = _currentProduct.Discription;
            TextBoxPrice.Text = _currentProduct.Price.ToString("N2");
            TextBoxCount.Text = _currentProduct.CountSklad.ToString();
            TextBoxDiscount.Text = (_currentProduct.Discount ?? 0).ToString();

            // Выбор единицы измерения
            if (!string.IsNullOrEmpty(_currentProduct.Unit))
            {
                for (int i = 0; i < ComboUnit.Items.Count; i++)
                {
                    if (ComboUnit.Items[i] is ComboBoxItem item &&
                        item.Content.ToString() == _currentProduct.Unit)
                    {
                        ComboUnit.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                ComboUnit.SelectedIndex = 0;
            }
        }

        private string CheckErrors()
        {
            var errorbuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TextBoxName.Text))
                errorbuilder.AppendLine("Введите название товара!");

            if (string.IsNullOrWhiteSpace(TBart.Text))
                errorbuilder.AppendLine("Введите артикул!");

            if (!decimal.TryParse(TextBoxPrice.Text, out decimal price) || price <= 0)
                errorbuilder.AppendLine("Введите корректную цену (положительное число)!");

            if (!int.TryParse(TextBoxCount.Text, out int count) || count < 0)
                errorbuilder.AppendLine("Введите корректное количество (неотрицательное число)!");

            if (!double.TryParse(TextBoxDiscount.Text, out double discount) || discount < 0 || discount > 100)
                errorbuilder.AppendLine("Скидка должна быть от 0 до 100%!");

            if (errorbuilder.Length > 0)
                errorbuilder.Insert(0, "Устраните следующие ошибки:\n");

            return errorbuilder.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = CheckErrors();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var unit = (ComboUnit.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "шт.";
                var discount = double.Parse(TextBoxDiscount.Text);

                if (_currentProduct == null) // Добавление
                {
                    var product = new Entities.Product
                    {
                        Name = TextBoxName.Text,
                        Articul = TBart.Text,
                        Discription = TextBoxDesc.Text,
                        Price = decimal.Parse(TextBoxPrice.Text),
                        CountSklad = int.Parse(TextBoxCount.Text),
                        Unit = unit,
                        Discount = discount,
                        Category_id = 1,
                        Suplier_id = 1,
                        Manufacturer_id = 1
                    };

                    App.Context.Product.Add(product);
                    App.Context.SaveChanges();
                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Редактирование
                {
                    _currentProduct.Name = TextBoxName.Text;
                    _currentProduct.Articul = TBart.Text;
                    _currentProduct.Discription = TextBoxDesc.Text;
                    _currentProduct.Price = decimal.Parse(TextBoxPrice.Text);
                    _currentProduct.CountSklad = int.Parse(TextBoxCount.Text);
                    _currentProduct.Unit = unit;
                    _currentProduct.Discount = discount;

                    App.Context.SaveChanges();
                    MessageBox.Show("Товар успешно обновлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
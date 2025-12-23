using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{

    public partial class MainWindow : Window
    {
        private readonly ShoesMarketContext _context;
        private User _currentUser;
        private List<Shoe> _shoes = new();
        private List<Brand> _brands = new();

        public MainWindow(User user)
        {
            InitializeComponent();
            _context = new ShoesMarketContext();
            _currentUser = user;
            LoadShoes();
            LoadBrands();

            // Выдача прав на изменение данных только админам и менеджерам
            if (_currentUser.Role.Name == "admin" || _currentUser.Role.Name == "manager")
            {
                AddButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        // Загрузка брендов
        private void LoadBrands()
        {
            _brands = _context.Brands.ToList();
            var allBrands = new List<Brand> { new Brand { BrandId = -1, Name = "Все бренды" } };
            allBrands.AddRange(_brands);
            BrandComboBox.ItemsSource = allBrands;
            BrandComboBox.DisplayMemberPath = "Name";
            BrandComboBox.SelectedValuePath = "BrandId";
            BrandComboBox.SelectedIndex = 0;
        }

        // Загрузка обуви
        private void LoadShoes()
        {
            try
            {
                _context.ChangeTracker.Clear();
                var shoes = _context.Shoes
                    .Include(s => s.Brand)
                    .Include(s => s.Category)
                    .Include(s => s.Vendor)
                    .ToList();
                _shoes = [.. shoes];
                ShoesDataGrid.ItemsSource = null;
                ShoesDataGrid.ItemsSource = _shoes;
                ShoesDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в загрузке обуви: {ex.Message}");
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filtered = GetFilteredShoes();
            _shoes = [.. filtered];
            ShoesDataGrid.ItemsSource = null;
            ShoesDataGrid.ItemsSource = _shoes;
            ShoesDataGrid.Items.Refresh();
        }

        // Фильтрация обуви
        private List<Shoe> GetFilteredShoes()
        {
            var shoes = _context.Shoes
                .Include(s => s.Brand)
                .Include(s => s.Category)
                .Include(s => s.Vendor)
                .AsQueryable();
            
            // По описанию
            var searchText = SearchTextBox?.Text ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(searchText))
                shoes = shoes.Where(s => s.Description != null && s.Description.Contains(searchText));

            // По бренду
            if (BrandComboBox.SelectedItem is Brand selectedBrand && selectedBrand.BrandId != -1)
                shoes = shoes.Where(s => s.BrandId == selectedBrand.BrandId);

            // По максимальной цене
            if (decimal.TryParse(MaxPriceTextBox.Text, out var maxPrice))
                shoes = shoes.Where(s => s.Price - (s.Price * s.Discount / 100) <= maxPrice);

            // По наличию скидки
            if (DiscountCheckBox.IsChecked == true)
                shoes = shoes.Where(s => s.Discount > 0);
            
            // По наличию самого товара
            if (InStockCheckBox.IsChecked == true)
                shoes = shoes.Where(s => s.Quantity > 0);

            // Сортировка
            if (SortComboBox.SelectedItem is ComboBoxItem selectedSort)
            {
                var sortValue = selectedSort.Tag?.ToString();
                switch (sortValue)
                {
                    case "name":
                        shoes = shoes.OrderBy(s => s.Category.Name);
                        break;
                    case "vendor":
                        shoes = shoes.OrderBy(s => s.Vendor.Name);
                        break;
                    case "price":
                        shoes = shoes.OrderBy(s => s.Price - (s.Price * s.Discount / 100));
                        break;
                    case "price_desc":
                        shoes = shoes.OrderByDescending(s => s.Price - (s.Price * s.Discount / 100));
                        break;
                    default:
                        break;
                }
            }

            return shoes.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditShoeWindow(null);
            editWindow.ShowDialog();
            LoadShoes();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShoesDataGrid.SelectedItem is Shoe selectedShoe)
            {
                var editWindow = new EditShoeWindow(selectedShoe.ShoeId);
                editWindow.ShowDialog();
                LoadShoes();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShoesDataGrid.SelectedItem is Shoe selectedShoe)
            {
                if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Shoes.Remove(selectedShoe);
                    _context.SaveChanges();
                    LoadShoes();
                }
            }
        }
    }
}
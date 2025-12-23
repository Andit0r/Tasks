using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public partial class EditShoeWindow : Window
    {
        private readonly ShoesMarketContext _context;
        private Shoe? _shoe;

        public EditShoeWindow(int? shoeId)
        {
            InitializeComponent();
            _context = new ShoesMarketContext();
            _shoe = null;

            if (shoeId is not null)
            {
                _shoe = _context.Shoes
                    .Include(s => s.Brand)
                    .Include(s => s.Category)
                    .Include(s => s.Vendor)
                    .FirstOrDefault(s => s.ShoeId == shoeId);
            }

            LoadCombos();
            LoadInfo();
        }

        // Загрузка информации об обуви
        private void LoadInfo()
        {
            if (_shoe is not null)
            {
                CategoryComboBox.SelectedValue = _shoe.CategoryId;
                DescriptionTextBox.Text = _shoe.Description;
                ArticleTextBox.Text = _shoe.Article;
                BrandComboBox.SelectedValue = _shoe.BrandId;
                VendorComboBox.SelectedValue = _shoe.VendorId;
                PriceTextBox.Text = _shoe.Price.ToString();
                DiscountTextBox.Text = _shoe.Discount.ToString();
                SizeTextBox.Text = _shoe.Size?.ToString() ?? "";
                QuantityTextBox.Text = _shoe.Quantity.ToString();
                PhotoTextBox.Text = _shoe.Photo;
                GenderComboBox.SelectedItem = GenderComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(i =>
                    string.Equals(i.Content.ToString(), _shoe.Gender, StringComparison.OrdinalIgnoreCase));
            }
        }

        // Загрузка ComboBox'ов
        private void LoadCombos()
        {
            CategoryComboBox.ItemsSource = _context.Categories.ToList();
            BrandComboBox.ItemsSource = _context.Brands.ToList();
            VendorComboBox.ItemsSource = _context.Vendors.ToList();
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            ChoosePhoto();
        }

        private void ChoosePhoto()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var fileName = Path.GetFileName(openFileDialog.FileName);
                    var destPath = Path.Combine(Environment.CurrentDirectory, "Images", fileName);
                    var dir = Path.GetDirectoryName(destPath);
                    if (dir != null)
                        Directory.CreateDirectory(dir);
                    File.Copy(openFileDialog.FileName, destPath, true);
                    PhotoTextBox.Text = $"{fileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveShoe())
            {
                Close();
            }
        }


        // Сохранение обуви в бд
        private bool SaveShoe()
        {
            // Валидация данных
            if (CategoryComboBox.SelectedValue == null || BrandComboBox.SelectedValue == null || VendorComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля");
                return false;
            }

            if (!int.TryParse(PriceTextBox.Text, out var price) ||
                !byte.TryParse(DiscountTextBox.Text, out var discount) ||
                !byte.TryParse(QuantityTextBox.Text, out var quantity))
            {
                MessageBox.Show("Неверный формат данных");
                return false;
            }

            byte? size = null;
            if (!string.IsNullOrWhiteSpace(SizeTextBox.Text))
            {
                if (!byte.TryParse(SizeTextBox.Text, out var s))
                {
                    MessageBox.Show("Неверный формат размера");
                    return false;
                }
                size = s;
            }

            if (_shoe == null)
            {
                _shoe = new Shoe();
                _context.Shoes.Add(_shoe);
            }

            var gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (string.IsNullOrWhiteSpace(gender))
            {
                MessageBox.Show("Выберите пол");
                return false;
            }

            // Запись данных в объект
            _shoe.CategoryId = (int)CategoryComboBox.SelectedValue;
            _shoe.Description = DescriptionTextBox.Text;
            _shoe.Article = ArticleTextBox.Text;
            _shoe.BrandId = (int)BrandComboBox.SelectedValue;
            _shoe.VendorId = (int)VendorComboBox.SelectedValue;
            _shoe.Price = price;
            _shoe.Discount = discount;
            _shoe.Size = size;
            _shoe.Quantity = quantity;
            _shoe.Photo = PhotoTextBox.Text;
            _shoe.Gender = gender.Trim();

            _context.SaveChanges();
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace lab10
{
    public partial class MainWindow : Window
    {
        private SortableBindingList<Car> MyCars { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeMyCars();
        }

        private void InitializeMyCars()
        {
            MyCars = new SortableBindingList<Car>(GetCars());
            dataGrid.ItemsSource = MyCars;
        }

        private List<Car> GetCars()
        {
            return new List<Car>()
            {
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };
        }

        private void PropertiesComboBox_DropDownOpened(object sender, EventArgs e)
        {
            propertiesComboBox.Items.Clear();
            var properties = typeof(Car).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(int));
            foreach (var property in properties)
            {
                propertiesComboBox.Items.Add(property.Name);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (propertiesComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a property to search.");
                return;
            }

            string propertyName = propertiesComboBox.SelectedItem.ToString();
            string searchText = searchTextBox.Text;

            var prop = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyName, true);
            if (prop == null)
            {
                MessageBox.Show("Property not found.");
                return;
            }

            object searchValue;
            if (prop.PropertyType == typeof(int) && int.TryParse(searchText, out int intResult))
            {
                searchValue = intResult;
            }
            else
            {
                searchValue = searchText;
            }

            int index = MyCars.Find(prop, searchValue);
            if (index >= 0)
            {
                dataGrid.SelectedIndex = index;
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
            else
            {
                MessageBox.Show("Item not found.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                MyCars.Remove((Car)dataGrid.SelectedItem);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newCar = new Car("New Model", new Engine(0, 0, "New Engine"), 2022);
            MyCars.Add(newCar);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = string.Empty;
            MyCars = new SortableBindingList<Car>(GetCars());
            dataGrid.ItemsSource = MyCars;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newCar = new Car("New Model", new Engine(0, 0, "New Engine"), 2022);
            MyCars.Add(newCar);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                MyCars.Remove((Car)dataGrid.SelectedItem);
            }
        }
    }
}

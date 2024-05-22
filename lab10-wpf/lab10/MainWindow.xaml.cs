using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

            RunDelegates();

            ShowQueryExpressionResult();
            ShowMethodBasedResult();
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

        private static void ShowQueryExpressionResult()
        {
            List<Car> myCars = new List<Car>()
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

            var result = from c in myCars
                         where c.Model == "A6"
                         let engineType = c.Engine.EngineType == "TDI" ? "diesel" : "petrol"
                         let hppl = (double)c.Engine.HorsePower / c.Engine.Displacement
                         group hppl by engineType
                into g
                         orderby g.Average() descending
                         select new
                         {
                             engineType = g.Key,
                             avgHPPL = g.Average()
                         };

            var odp = result.Aggregate("query_expression \n", (current, e) => current + (e.engineType + ": " + e.avgHPPL + " \n"));
            MessageBox.Show(odp);
        }

        private static void ShowMethodBasedResult()
        {
            List<Car> myCars = new List<Car>()
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

            var result = myCars
                .Where(c => c.Model == "A6")
                .Select(c => new
                {
                    engineType = c.Engine.EngineType == "TDI" ? "diesel" : "petrol",
                    hppl = (double)c.Engine.HorsePower / c.Engine.Displacement
                })
                .GroupBy(c => c.engineType)
                .Select(g => new
                {
                    engineType = g.Key,
                    avgHPPL = g.Average(c => c.hppl)
                })
                .OrderByDescending(c => c.avgHPPL);

            var odp = result.Aggregate("method-based query \n", (current, e) => current + (e.engineType + ": " + e.avgHPPL + " \n"));
            MessageBox.Show(odp);
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

            var engineProperties = typeof(Engine).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(int));

            foreach (var property in engineProperties)
            {
                propertiesComboBox.Items.Add($"Engine.{property.Name}");
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

            var propertyNames = propertyName.Split('.');
            PropertyDescriptor prop = null;

            if (propertyNames.Length == 1)
            {
                prop = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyName, true);
            }
            else if (propertyNames.Length == 2)
            {
                var parentProp = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyNames[0], true);
                if (parentProp != null)
                {
                    prop = TypeDescriptor.GetProperties(parentProp.PropertyType).Find(propertyNames[1], true);
                }
            }

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

            int index = -1;
            if (propertyNames.Length == 1)
            {
                index = MyCars.Find(prop, searchValue);
            }
            else if (propertyNames.Length == 2)
            {
                for (int i = 0; i < MyCars.Count; i++)
                {
                    var car = MyCars[i];
                    var parentValue = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyNames[0], true).GetValue(car);
                    if (parentValue != null && searchValue.Equals(prop.GetValue(parentValue)))
                    {
                        index = i;
                        break;
                    }
                }
            }

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

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = string.Empty;
            MyCars = new SortableBindingList<Car>(GetCars());
            dataGrid.ItemsSource = MyCars;
        }


        private static void RunDelegates()
        {
            Func<Car, Car, int> arg1 = Func;
            Predicate<Car> arg2 = Predicate;
            Action<Car> arg3 = Action;

            List<Car> myCars = new List<Car>()
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

            myCars.Sort(new Comparison<Car>(arg1));
            myCars.FindAll(arg2).ForEach(arg3);
        }

        private static int Func(Car car, Car b)
        {
            if (car.Engine.HorsePower > b.Engine.HorsePower)
            {
                return 1;
            }

            if (car.Engine.HorsePower < b.Engine.HorsePower)
            {
                return -1;
            }

            return 0;
        }

        private static bool Predicate(Car a)
        {
            return a.Engine.EngineType == "TDI";
        }

        private static void Action(Car a)
        {
            MessageBox.Show("2. Model: " + a.Model + " Engine: " + a.Engine.EngineType + " Year: " + a.Year);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lab10
{
    public class Car : IComparable<Car>
    {
        public string Model { get; set; }
        public Engine Engine { get; set; }
        public int Year { get; set; }

        public Car() { }

        public Car(string model, Engine engine, int year)
        {
            Model = model;
            Engine = engine;
            Year = year;
        }

        public int CompareTo(Car? other)
        {
            if (other == null) return 1; // Traktuje null jako "mniejszy" niż jakikolwiek aktualny obiekt
            return this.Model.CompareTo(other.Model);
        }
    }
}

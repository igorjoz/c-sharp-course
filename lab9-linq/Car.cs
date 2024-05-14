using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab9_linq
{
    public class Car
    {
        public string Model { get; set; }
        public Engine Engine { get; set; }
        public int Year { get; set; }

        // Konstruktor bezparametrowy
        public Car() { }

        // Konstruktor parametryczny
        public Car(string model, Engine engine, int year)
        {
            Model = model;
            Engine = engine;
            Year = year;
        }
    }
}

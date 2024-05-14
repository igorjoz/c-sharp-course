using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab9_linq
{
    public class Engine
    {
        public double Displacement { get; set; }
        public int HorsePower { get; set; }
        public string EngineType { get; set; }

        // Konstruktor bezparametrowy
        public Engine() { }

        // Konstruktor parametryczny
        public Engine(double displacement, int horsePower, string engineType)
        {
            Displacement = displacement;
            HorsePower = horsePower;
            EngineType = engineType;
        }
    }
}

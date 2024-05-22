using System;

namespace lab10
{
    public class Engine : IComparable<Engine>
    {
        public double Displacement { get; set; }
        public int HorsePower { get; set; }
        public string EngineType { get; set; }

        public Engine() { }

        public Engine(double displacement, int horsePower, string engineType)
        {
            Displacement = displacement;
            HorsePower = horsePower;
            EngineType = engineType;
        }

        public int CompareTo(Engine other)
        {
            if (other == null) return 1;
            return this.HorsePower.CompareTo(other.HorsePower);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace lab9_linq
{
    class Program
    {
        static void Main(string[] args)
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

            var query1 = from car in myCars
                         where car.Model == "A6"
                         select new
                         {
                             engineType = car.Engine.EngineType == "TDI" ? "diesel" : "petrol",
                             hppl = car.Engine.HorsePower / car.Engine.Displacement
                         };

            var query2 = query1.GroupBy(x => x.engineType)
                               .Select(g => new
                               {
                                   EngineType = g.Key,
                                   AverageHppl = g.Average(x => x.hppl)
                               });

            foreach (var group in query2)
            {
                Console.WriteLine($"Engine Type: {group.EngineType}, Average HPPL: {group.AverageHppl}");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (TextWriter writer = new StreamWriter("cars.xml"))
            {
                serializer.Serialize(writer, myCars);
            }

            // Załaduj dokument XML
            XDocument xdoc = XDocument.Load("cars.xml");

            // Użyj LINQ do XML do obliczenia średniej mocy silników innych niż TDI
            var nonTDICars = xdoc.Descendants("car")
                                 .Where(c => (string)c.Element("engine").Attribute("type") != "TDI")
                                 .Select(c => (int)c.Element("engine").Attribute("horsePower"));

            var averageHorsePower = nonTDICars.Any() ? nonTDICars.Average() : 0;

            // Wypisz wynik
            Console.WriteLine($"Average HorsePower for non-TDI engines: {averageHorsePower}");

            // Modele samochodów bez powtórzeń
            IEnumerable<XElement> uniqueModels = xdoc.XPathSelectElements("/cars/car/model[not(.=following::model)]");

            // Wypisz unikalne modele
            foreach (var model in uniqueModels)
            {
                Console.WriteLine($"Unique Model: {model.Value}");
            }

            // Przenieś wartość z 'year' do atrybutu i usuń element 'year'
            foreach (XElement car in xdoc.Descendants("car"))
            {
                car.SetAttributeValue("year", car.Element("year")?.Value);
                car.Element("year")?.Remove();
            }

            xdoc.Save("modifiedCars.xml");

            var table = new XElement("table",
                new XElement("thead",
                    new XElement("tr",
                        new XElement("th", "Model"),
                        new XElement("th", "Engine Type"),
                        new XElement("th", "Horse Power"),
                        new XElement("th", "Year"))),
                new XElement("tbody",
                    myCars.Select(car =>
                        new XElement("tr",
                            new XElement("td", car.Model),
                            new XElement("td", car.Engine.EngineType),
                            new XElement("td", car.Engine.HorsePower),
                            new XElement("td", car.Year)))));

            var doc = new XDocument(new XElement("html",
                new XElement("body", table)));
            doc.Save("cars.xhtml");
        }
    }
}

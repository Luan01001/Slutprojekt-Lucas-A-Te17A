using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BreakoutLucasA
{
    public class PoängManager
    {
        private static string _fileName = "poäng.xml"; 

        public List<Poäng> Highscore { get; private set; }

        public List<Poäng> Poängs { get; private set; }

        public PoängManager()
            : this(new List<Poäng>())
        {

        }
        
        public PoängManager(List<Poäng> poängs)
        {
            Poängs = poängs;

            UpdateHighscore();
        }

        public void Add(Poäng poäng)
        {
            Poängs.Add(poäng);

            Poängs = Poängs.OrderByDescending(c => c.Värde).ToList(); //sorterar listan så att de högsta poängen är först
        }

        public static PoängManager Load()
        {
            if (!File.Exists(_fileName)) // om det inte finns en fil att ladda så skapas en ny "PoängManager" 
                return new PoängManager();

            //annars laddas filen
            using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
            {
                var serializer = new XmlSerializer(typeof(List<Poäng>));

                var poängs = (List<Poäng>)serializer.Deserialize(reader);

                return new PoängManager(poängs);
            }
        }

        public void UpdateHighscore()
        {
            Highscore = Poängs.Take(10).ToList(); //Gör att highscorelistan innehåller de 10 bästa resultaten
        }

        public static void Save(PoängManager poängManager)
        {
            using (var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                var serializer = new XmlSerializer(typeof(List<Poäng>));

                serializer.Serialize(writer, poängManager.Poängs);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutLucasA
{
    /// <summary>
    /// liten klass som enbart definierar en string och en int
    /// </summary>
    /// Spelare och Värde används i Breakout.cs
    public class Poäng
    {
        public string Spelare { get; set; } // Spelare är det som sätts in i highscore listan

        public int Värde { get; set; } // värde är poängen

    }
}

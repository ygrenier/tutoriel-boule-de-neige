using System;
using System.Collections.Generic;
using System.Text;

namespace BouleDeNeige
{
    public class Lancer
    {
        public DateTimeOffset Date { get; set; }
        public String Cible { get; set; }
        public String Lanceur { get; set; }
        public bool Succes { get; set; }
    }
}

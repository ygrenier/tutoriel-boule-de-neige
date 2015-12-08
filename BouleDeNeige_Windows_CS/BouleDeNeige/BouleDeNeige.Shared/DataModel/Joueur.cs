using System;
using System.Collections.Generic;
using System.Text;

namespace BouleDeNeige
{
    public class Joueur
    {
        public String Id { get; set; }
        public String Nom { get; set; }
        public int BoulesRestantes { get; set; }
        public int BoulesRecues { get; set; }
        public int BoulesLancees { get; set; }
        public int Points { get; set; }
    }
}

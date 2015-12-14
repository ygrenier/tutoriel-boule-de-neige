using System;
using System.Collections.Generic;
using System.Text;

namespace BouleDeNeige
{
    public class LancerHistorique
    {
        public DateTimeOffset Date { get; set; }
        public String LanceurId { get; set; }
        public String LanceurNom { get; set; }
        public String CibleId { get; set; }
        public String CibleNom { get; set; }
        public bool Success { get; set; }
    }
}

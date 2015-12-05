using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouleDeNeigeService.DataObjects
{
    public class Joueur : EntityData
    {
        [Required, StringLength(128)]
        public String Nom { get; set; }
        public int BoulesRestantes { get; set; }
        public int BoulesRecues { get; set; }
        public int BoulesLancees { get; set; }
    }
}

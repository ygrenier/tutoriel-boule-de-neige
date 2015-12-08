using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouleDeNeigeService.DataObjects
{
    public class Lancer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTimeOffset Date { get; set; }
        public String LanceurId { get; set; }
        [ForeignKey("LanceurId")]
        public virtual Joueur Lanceur { get; set; }
        public String CibleId { get; set; }
        [ForeignKey("CibleId")]
        public virtual Joueur Cible { get; set; }
        public bool Success { get; set; }
    }
}

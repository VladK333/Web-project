using System;
using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public class SmestajnaJedinica
    {
        public long Id { get; set; }

        [Required]
        public int DozvoljenBrojGostiju { get; set; }

        public bool DozvoljenBoravakKucnimLjubimcima { get; set; }

        [Required]
        public decimal Cena { get; set; }

        public SmestajnaJedinica() { }

        public SmestajnaJedinica(long id, int dozvoljenBrojGostiju, bool dozvoljenBoravakKucnimLjubimcima, decimal cena)
        {
            Id = id;
            DozvoljenBrojGostiju = dozvoljenBrojGostiju;
            DozvoljenBoravakKucnimLjubimcima = dozvoljenBoravakKucnimLjubimcima;
            Cena = cena;
        }
    }
}

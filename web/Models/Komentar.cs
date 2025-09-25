using System;
using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public class Komentar
    {
        public long Id { get; set; }

        [Required]
        public long TuristaId { get; set; } // ID turista koji ostavlja komentar

        [Required]
        public long SmestajId { get; set; } // ID smeštaja na koji se komentar odnosi

        [Required]
        public string Tekst { get; set; } 

        [Required]
        [Range(1, 5)]
        public int Ocena { get; set; }

        public Komentar() { }

        public Komentar(long id, long turistaId, long smestajId, string tekst, int ocena)
        {
            Id = id;
            TuristaId = turistaId;
            SmestajId = smestajId;
            Tekst = tekst;
            Ocena = ocena;
        }
    }
}

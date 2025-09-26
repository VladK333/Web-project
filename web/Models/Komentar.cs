using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public enum StatusKomentara { KREIRAN, ODOBREN, ODBIJEN };

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

        [Required]
        public long RezervacijaId { get; set; }

        public StatusKomentara Status { get; set; }

        public Komentar() { }

        public Komentar(long id, long turistaId, long smestajId, string tekst, int ocena, long rezervacijaId, StatusKomentara status)
        {
            Id = id;
            TuristaId = turistaId;
            SmestajId = smestajId;
            Tekst = tekst;
            Ocena = ocena;
            RezervacijaId = rezervacijaId;
            Status = status;
        }
    }
}

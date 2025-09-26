using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public enum StatusRezervacijeAranzmana { AKTIVNA, OTKAZANA }

    public class RezervacijaAranzmana
    {
        public long Id { get; set; }

        [Required]
        public long TuristaId { get; set; } // ID korisnika koji vrši rezervaciju

        [Required]
        public StatusRezervacijeAranzmana Status { get; set; } = StatusRezervacijeAranzmana.AKTIVNA;

        [Required]
        public long AranzmanId { get; set; } // ID izabranog aranžmana

        [Required]
        public long SmestajnaJedinicaId { get; set; } // ID izabrane smeštajne jedinice

        public RezervacijaAranzmana() { }

        public RezervacijaAranzmana(long id, long turistaId, StatusRezervacijeAranzmana status, long aranzmanId, long smestajnaJedinicaId)
        {
            Id = id;
            TuristaId = turistaId;
            Status = status;
            AranzmanId = aranzmanId;
            SmestajnaJedinicaId = smestajnaJedinicaId;
        }
    }
}

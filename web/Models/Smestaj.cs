using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public enum TipSmestaja
    {
        Hotel,
        Motel,
        Vila
    }

    public class Smestaj
    {
        public long Id { get; set; }

        [Required]
        public TipSmestaja Tip { get; set; }

        [Required]
        public string Naziv { get; set; }

        public int BrojZvezdica { get; set; }

        public bool ImaBazen { get; set; }

        public bool ImaSpaCentar { get; set; }

        public bool PrilagodjenoZaInvaliditet { get; set; }

        public bool ImaWifi { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public List<long> SmestajneJedinice { get; set; } = new List<long>();

        public Smestaj() { }

        public Smestaj(long id, TipSmestaja tip, string naziv, int brojZvezdica, bool imaBazen, bool imaSpaCentar, bool prilagodjenoZaInvaliditet, bool imaWifi, bool isDeleted, List<long> smestajneJedinice)
        {
            Id = id;
            Tip = tip;
            Naziv = naziv;
            BrojZvezdica = brojZvezdica;
            ImaBazen = imaBazen;
            ImaSpaCentar = imaSpaCentar;
            PrilagodjenoZaInvaliditet = prilagodjenoZaInvaliditet;
            ImaWifi = imaWifi;
            IsDeleted = isDeleted;
            SmestajneJedinice = smestajneJedinice;
        }
    }
}

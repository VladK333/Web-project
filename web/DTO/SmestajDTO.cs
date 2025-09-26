using projekat.Models;
using System.Collections.Generic;

namespace projekat.DTO
{
    public class SmestajDTO
    {
        public TipSmestaja Tip { get; set; }
        public string Naziv { get; set; }
        public int BrojZvezdica { get; set; }
        public bool ImaBazen { get; set; }
        public bool ImaSpaCentar { get; set; }
        public bool PrilagodjenoZaInvaliditet { get; set; }
        public bool ImaWifi { get; set; }
        public List<SmestajnaJedinicaDTO> SmestajneJedinice { get; set; } = new List<SmestajnaJedinicaDTO>();

        public SmestajDTO() { }
    }
}
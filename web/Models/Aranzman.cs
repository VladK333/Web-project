using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public enum TipAranzmana
    {
        NocenjeSDoruckom,
        Polupansion,
        PunPansion,
        AllInclusive,
        NajamApartmana
    }

    public enum TipPrevoza
    {
        Autobus,
        Avion,
        AutobusAvion,
        Individualan,
        Ostalo
    }

    public class Aranzman
    {
        public long Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        [Required]
        public TipAranzmana TipAranzmana { get; set; }

        [Required]
        public TipPrevoza TipPrevoza { get; set; }

        [Required]
        public string Lokacija { get; set; }

        [Required]
        [JsonConverter(typeof(DateParser), "dd/MM/yyyy")]
        public DateTime DatumPocetka { get; set; }

        [Required]
        [JsonConverter(typeof(DateParser), "dd/MM/yyyy")]
        public DateTime DatumZavrsetka { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Maksimalan broj putnika mora biti veći od 0.")]
        public int MaksimalanBrojPutnika { get; set; }

        [Required]
        public string Opis { get; set; }

        [Required]
        public string ProgramPutovanja { get; set; }

        public string PosterAranzmana { get; set; }

        [Required]
        public long SmestajId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Aranzman() { }

        public Aranzman(long id, string naziv, TipAranzmana tipAranzmana, TipPrevoza tipPrevoza, string lokacija, DateTime datumPocetka, DateTime datumZavrsetka, int maksimalanBrojPutnika, string opis, string programPutovanja, string posterAranzmana, long smestajId, bool isDeleted)
        {
            Id = id;
            Naziv = naziv;
            TipAranzmana = tipAranzmana;
            TipPrevoza = tipPrevoza;
            Lokacija = lokacija;
            DatumPocetka = datumPocetka;
            DatumZavrsetka = datumZavrsetka;
            MaksimalanBrojPutnika = maksimalanBrojPutnika;
            Opis = opis;
            ProgramPutovanja = programPutovanja;
            PosterAranzmana = posterAranzmana;
            SmestajId = smestajId;
            IsDeleted = isDeleted;
        }
    }
}

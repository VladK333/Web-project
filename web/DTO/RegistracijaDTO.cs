using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace projekat.DTO
{
    public class RegistracijaDTO
    {
        [Required]
        public string KorisnickoIme { get; set; }

        [Required]
        public string Lozinka { get; set; }

        [Required]
        public string Ime { get; set; }

        [Required]
        public string Prezime { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DatumRodjenja { get; set; }

        [Required]
        public string Pol { get; set; }

        public RegistracijaDTO() { }
    }
}
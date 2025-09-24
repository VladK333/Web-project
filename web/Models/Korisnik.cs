using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projekat.Models
{
    public enum TipKorisnika { ADMIN, TURISTA, MENADZER };

    public class Korisnik
    {
        public long Id { get; set; }

        [Required]
        public string KorisnickoIme { get; set; }

        [Required]
        public string Lozinka { get; set; }

        [Required]
        public string Ime { get; set; }

        [Required]
        public string Prezime { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [JsonConverter(typeof(DateParser), "dd/MM/yyyy")]
        public DateTime DatumRodjenja { get; set; }

        [Required]
        public string Pol { get; set; }

        [Required]
        public TipKorisnika TipKorisnika { get; set; } = TipKorisnika.TURISTA;

        public List<long> Rezervacije { get; set; } = new List<long>();

        public List<long> KreiraniAranzmani { get; set; } = new List<long>();

        public Korisnik() { }

        public Korisnik(long id, string korisnickoIme, string lozinka, string ime, string prezime, string email, DateTime datumRodjenja, string pol, TipKorisnika tipKorisnika, List<long> rezervacije, List<long> kreiraniAranzmani)
        {
            Id = id;
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Email = email;
            DatumRodjenja = datumRodjenja;
            Pol = pol;
            TipKorisnika = tipKorisnika;
            Rezervacije = rezervacije;
            KreiraniAranzmani = kreiraniAranzmani;
        }
    }
}
using projekat.DTO;
using projekat.Models;
using projekat.Podaci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/smestaji")]
    public class SmestajiController : ApiController
    {
        [Route("svi")]
        [HttpGet]
        public IHttpActionResult SviSmestaji()
        {
            try
            {
                // Samo menadžeri i admin mogu da pristupaju smeštajima
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik &&
                   (korisnik.TipKorisnika == TipKorisnika.ADMIN || korisnik.TipKorisnika == TipKorisnika.MENADZER))
                {
                    var smestaji = BazaPodataka.Smestaji.Where(s => !s.IsDeleted).ToList();
                    return Ok(new { data = smestaji });
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("get/{id:long}")]
        [HttpGet]
        public IHttpActionResult DobaviSmestaj(long id)
        {
            try
            {
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik &&
                   (korisnik.TipKorisnika == TipKorisnika.ADMIN || korisnik.TipKorisnika == TipKorisnika.MENADZER))
                {
                    Smestaj smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
                    if (smestaj == null)
                        return NotFound();

                    // Mapiranje u DTO
                    var dto = new
                    {
                        Id = smestaj.Id,
                        Tip = smestaj.Tip,
                        Naziv = smestaj.Naziv,
                        BrojZvezdica = smestaj.BrojZvezdica,
                        ImaBazen = smestaj.ImaBazen,
                        ImaSpaCentar = smestaj.ImaSpaCentar,
                        PrilagodjenoZaInvaliditet = smestaj.PrilagodjenoZaInvaliditet,
                        ImaWifi = smestaj.ImaWifi,
                        SmestajneJedinice = smestaj.SmestajneJedinice
                            .Select(jId => BazaPodataka.SmestajneJedinice.FirstOrDefault(j => j.Id == jId && !j.IsDeleted))
                            .Where(j => j != null)
                            .Select(j => new SmestajnaJedinicaDTO
                            {
                                Id = j.Id,
                                BrojKreveta = j.DozvoljenBrojGostiju,
                                Cena = j.Cena,
                                DozvoljenBoravakKucnimLjubimcima = j.DozvoljenBoravakKucnimLjubimcima
                            }).ToList()
                    };

                    return Ok(new { data = dto });
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }


        [Route("dodaj")]
        [HttpPost]
        public IHttpActionResult DodajSmestaj([FromBody] SmestajDTO podaci)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Nisu uneti validni podaci!");

                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    (korisnik.TipKorisnika != TipKorisnika.ADMIN && korisnik.TipKorisnika != TipKorisnika.MENADZER))
                    return Unauthorized();

                if (BazaPodataka.Smestaji.Any(s => s.Naziv == podaci.Naziv && !s.IsDeleted))
                    return BadRequest("Smeštaj sa istim nazivom već postoji!");

                long noviId = BazaPodataka.Smestaji.Count > 0 ? BazaPodataka.Smestaji.Max(s => s.Id) + 1 : 1;

                Smestaj noviSmestaj = new Smestaj
                {
                    Id = noviId,
                    Tip = podaci.Tip,
                    Naziv = podaci.Naziv,
                    BrojZvezdica = podaci.BrojZvezdica,
                    ImaBazen = podaci.ImaBazen,
                    ImaSpaCentar = podaci.ImaSpaCentar,
                    PrilagodjenoZaInvaliditet = podaci.PrilagodjenoZaInvaliditet,
                    ImaWifi = podaci.ImaWifi,
                    IsDeleted = false,
                    SmestajneJedinice = new List<long>()
                };

                // Dodaj svaku jedinicu
                foreach (var jedinicaDto in podaci.SmestajneJedinice)
                {
                    long novaJedinicaId = BazaPodataka.SmestajneJedinice.Count > 0 ? BazaPodataka.SmestajneJedinice.Max(j => j.Id) + 1 : 1;
                    var jedinica = new SmestajnaJedinica
                    {
                        Id = novaJedinicaId,
                        DozvoljenBrojGostiju = jedinicaDto.BrojKreveta,
                        Cena = jedinicaDto.Cena,
                        DozvoljenBoravakKucnimLjubimcima = jedinicaDto.DozvoljenBoravakKucnimLjubimcima,
                        IsDeleted = false
                    };

                    BazaPodataka.SmestajneJedinice.Add(jedinica);
                    noviSmestaj.SmestajneJedinice.Add(jedinica.Id);
                }

                BazaPodataka.Smestaji.Add(noviSmestaj);
                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = noviSmestaj });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("azuriraj/{id:long}")]
        [HttpPut]
        public IHttpActionResult AzurirajSmestaj(long id, [FromBody] SmestajDTO podaci)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Nisu uneti validni podaci!");

                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    (korisnik.TipKorisnika != TipKorisnika.ADMIN && korisnik.TipKorisnika != TipKorisnika.MENADZER))
                    return Unauthorized();

                var smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
                if (smestaj == null)
                    return NotFound();

                if (BazaPodataka.Smestaji.Any(s => s.Id != id && s.Naziv == podaci.Naziv && !s.IsDeleted))
                    return BadRequest("Smeštaj sa istim nazivom već postoji!");

                // Ažuriraj osnovne podatke
                smestaj.Tip = podaci.Tip;
                smestaj.Naziv = podaci.Naziv;
                smestaj.BrojZvezdica = podaci.BrojZvezdica;
                smestaj.ImaBazen = podaci.ImaBazen;
                smestaj.ImaSpaCentar = podaci.ImaSpaCentar;
                smestaj.PrilagodjenoZaInvaliditet = podaci.PrilagodjenoZaInvaliditet;
                smestaj.ImaWifi = podaci.ImaWifi;

                // === Ažuriranje jedinica ===
                var postojeceJedinice = BazaPodataka.SmestajneJedinice.Where(j => smestaj.SmestajneJedinice.Contains(j.Id) && !j.IsDeleted).ToList();

                // Prođi kroz DTO liste i mapiraj
                foreach (var jedinicaDto in podaci.SmestajneJedinice)
                {
                    // Ako već postoji jedinica (na osnovu indeksa/pozicije)
                    var postojeca = postojeceJedinice.FirstOrDefault();
                    if (postojeca != null)
                    {
                        var rezervacijeZaSmestaj = BazaPodataka.Rezervacije.Where(r => r.SmestajnaJedinicaId == postojeca.Id && r.Status == StatusRezervacijeAranzmana.AKTIVNA);

                        // aranzmani koji predostoje
                        var predstojeciAranzmani = BazaPodataka.Aranzmani.Where(a => a.DatumPocetka > DateTime.Now);

                        foreach (var trenAranz in predstojeciAranzmani)
                        {
                            var rezervacijeZaAranzman = rezervacijeZaSmestaj.Where(r => r.AranzmanId == trenAranz.Id);

                            if (rezervacijeZaAranzman.Any(rr => rr.Status == StatusRezervacijeAranzmana.AKTIVNA))
                                return BadRequest("Postoje rezervacije za aranzman!");

                        }

                        postojeca.DozvoljenBrojGostiju = jedinicaDto.BrojKreveta;
                        postojeca.Cena = jedinicaDto.Cena;
                        postojeca.DozvoljenBoravakKucnimLjubimcima = jedinicaDto.DozvoljenBoravakKucnimLjubimcima;
                        postojeceJedinice.Remove(postojeca);
                    }
                    else
                    {
                        // Nova jedinica
                        long novaJedinicaId = BazaPodataka.SmestajneJedinice.Count > 0 ? BazaPodataka.SmestajneJedinice.Max(j => j.Id) + 1 : 1;
                        var nova = new SmestajnaJedinica
                        {
                            Id = novaJedinicaId,
                            DozvoljenBrojGostiju = jedinicaDto.BrojKreveta,
                            Cena = jedinicaDto.Cena,
                            DozvoljenBoravakKucnimLjubimcima = jedinicaDto.DozvoljenBoravakKucnimLjubimcima,
                            IsDeleted = false
                        };
                        BazaPodataka.SmestajneJedinice.Add(nova);
                        smestaj.SmestajneJedinice.Add(nova.Id);
                    }
                }

                // Preostale jedinice koje nisu u DTO listi → kandidat za brisanje
                foreach (var jedinicaZaBrisanje in postojeceJedinice)
                {
                    bool imaRezervacija = BazaPodataka.Rezervacije.Any(r =>
                        r.SmestajnaJedinicaId == jedinicaZaBrisanje.Id);

                    if (!imaRezervacija)
                        jedinicaZaBrisanje.IsDeleted = true;
                }

                BazaPodataka.SacuvajPodatke();
                return Ok(new { data = smestaj });
            }
            catch
            {
                return InternalServerError();
            }
        }


        [Route("obrisi/{id:long}")]
        [HttpDelete]
        public IHttpActionResult ObrisiSmestaj(long id)
        {
            try
            {
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik &&
                   (korisnik.TipKorisnika == TipKorisnika.ADMIN || korisnik.TipKorisnika == TipKorisnika.MENADZER))
                {
                    Smestaj smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
                    if (smestaj == null)
                        return NotFound();

                    // Proveri da li postoji aranžman u budućnosti sa ovim smeštajem
                    bool imaAranzmaneUBuducnosti = BazaPodataka.Aranzmani.Any(a =>
                        a.SmestajId == id &&
                        !a.IsDeleted &&
                        a.DatumPocetka > DateTime.Now);

                    if (imaAranzmaneUBuducnosti)
                        return BadRequest("Brisanje smeštaja nije dozvoljeno jer postoje aranžmani u budućnosti!");

                    // Logičko brisanje
                    smestaj.IsDeleted = true;

                    // Takođe obriši sve smeštajne jedinice ovog smeštaja
                    foreach (long jedinicaId in smestaj.SmestajneJedinice)
                    {
                        var jedinica = BazaPodataka.SmestajneJedinice.FirstOrDefault(j => j.Id == jedinicaId);
                        if (jedinica != null)
                            jedinica.IsDeleted = true;
                    }

                    BazaPodataka.SacuvajPodatke();
                    return Ok(new { message = "Smeštaj je uspešno obrisan!" });
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("slobodni")]
        public IHttpActionResult SlobodniSmestaji()
        {
            // Uzimamo sve smeštaje
            var svi = BazaPodataka.Smestaji.Where(s => !s.IsDeleted).ToList();

            // Uzimamo sve aktivne aranžmane
            var zauzetiSmestaji = BazaPodataka.Aranzmani
                .Where(a => !a.IsDeleted)
                .Select(a => a.SmestajId)
                .ToHashSet();

            // Filtriramo one koji nisu zauzeti
            var slobodni = svi
                .Where(s => !zauzetiSmestaji.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Naziv,
                    s.Tip
                });

            return Ok(new { data = slobodni });
        }
    }
}

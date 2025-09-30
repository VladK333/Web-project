using projekat.Models;
using projekat.Podaci;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/rezervacije")]
    public class RezervacijeController : ApiController
    {
        // POST: api/rezervacije/dodaj
        [Route("dodaj")]
        [HttpPost]
        public IHttpActionResult DodajRezervaciju([FromBody] RezervacijaAranzmana rezervacija)
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.TURISTA)
                    return BadRequest("Morate biti prijavljeni kao turista da biste izvršili rezervaciju.");

                var aranzman = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == rezervacija.AranzmanId);
                if (aranzman == null)
                    return NotFound();

                var smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == aranzman.SmestajId);
                if (smestaj == null)
                    return BadRequest("Smeštaj nije pronađen.");

                // Dohvati jedinicu po ID-ju iz smestaj.SmestajneJedinice
                var jedinica = BazaPodataka.SmestajneJedinice
                    .FirstOrDefault(j => smestaj.SmestajneJedinice.Contains(j.Id) && j.Id == rezervacija.SmestajnaJedinicaId);

                if (jedinica == null)
                    return BadRequest("Izabrana smeštajna jedinica nije pronađena.");

                // Dodavanje rezervacije
                rezervacija.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                rezervacija.TuristaId = korisnik.Id;
                rezervacija.Status = StatusRezervacijeAranzmana.AKTIVNA;

                BazaPodataka.Rezervacije.Add(rezervacija);

                // Ako postoji slobodna mesta, smanji broj
                if (jedinica.DozvoljenBrojGostiju > 0)
                    jedinica.DozvoljenBrojGostiju--;

                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = rezervacija });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // POST: api/rezervacije/otkazi
        [Route("otkazi")]
        [HttpPost]
        public IHttpActionResult OtkaziRezervaciju([FromBody] long rezervacijaId)
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik))
                    return BadRequest("Morate biti prijavljeni.");

                var rezervacija = BazaPodataka.Rezervacije.FirstOrDefault(r => r.Id == rezervacijaId);
                if (rezervacija == null)
                    return NotFound();

                if (rezervacija.TuristaId != korisnik.Id)
                    return BadRequest("Ne možete otkazati tuđu rezervaciju.");

                var aranzman = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == rezervacija.AranzmanId);
                if (aranzman == null)
                    return NotFound();

                if (aranzman.DatumZavrsetka < DateTime.Now)
                    return BadRequest("Ne možete otkazati rezervaciju za prošli aranžman.");

                rezervacija.Status = StatusRezervacijeAranzmana.OTKAZANA;

                var smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == aranzman.SmestajId);
                if (smestaj != null)
                {
                    var jedinica = BazaPodataka.SmestajneJedinice
                        .FirstOrDefault(j => smestaj.SmestajneJedinice.Contains(j.Id) && j.Id == rezervacija.SmestajnaJedinicaId);
                    if (jedinica != null)
                        jedinica.DozvoljenBrojGostiju++; // povrati mesto
                }

                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = rezervacija });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // GET: api/rezervacije/mojerezervacije
        [Route("mojerezervacije")]
        [HttpGet]
        public IHttpActionResult MojeRezervacije()
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik))
                    return BadRequest("Morate biti prijavljeni.");

                var rezervacije = BazaPodataka.Rezervacije
                    .Where(r => r.TuristaId == korisnik.Id)
                    .Select(r =>
                    {
                        var aranzman = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == r.AranzmanId);
                        var smestaj = (aranzman != null)
                            ? BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == aranzman.SmestajId)
                            : null;
                        var jedinica = (smestaj != null)
                            ? BazaPodataka.SmestajneJedinice.FirstOrDefault(j => smestaj.SmestajneJedinice.Contains(j.Id) && j.Id == r.SmestajnaJedinicaId)
                            : null;

                        return new
                        {
                            Rezervacija = r,
                            Aranzman = aranzman,
                            Smestaj = smestaj,
                            Jedinica = jedinica
                        };
                    })
                    .ToList();

                return Ok(new { data = rezervacije });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // GET: api/rezervacije/aranzman/{id}
        [Route("aranzman/{aranzmanId}")]
        [HttpGet]
        public IHttpActionResult RezervacijePoAranzmanu(long aranzmanId)
        {
            try
            {
                var rezervacije = BazaPodataka.Rezervacije
                    .Where(r => r.AranzmanId == aranzmanId)
                    .Select(r =>
                    {
                        var turista = BazaPodataka.Korisnici.FirstOrDefault(k => k.Id == r.TuristaId);
                        return new
                        {
                            ImeKlijenta = turista?.Ime + " " + turista?.Prezime,
                            Email = turista?.Email,
                            MaksimalanBrojPutnika = 1,
                            DatumRezervacije = DateTimeOffset.FromUnixTimeSeconds(r.Id).DateTime.ToString("dd/MM/yyyy"),
                            Napomene = r.Status.ToString()
                        };
                    })
                    .ToList();

                return Ok(new { data = rezervacije });
            }
            catch
            {
                return InternalServerError();
            }
        }
    }
}

using projekat.DTO;
using projekat.Models;
using projekat.Podaci;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/identity")]
    public class IdentityController : ApiController
    {
        [Route("sesija")]
        [HttpGet]
        public IHttpActionResult ProveraSesije()
        {
            try
            {
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik)
                    return Ok(new { data = korisnik });
                else
                    return BadRequest("Niste prijavljeni!");
            }
            catch
            {
                return NotFound();
            }
        }

        [Route("prijava")]
        [HttpPost]
        public IHttpActionResult Prijava([FromBody] PrijavaDTO podaci)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return BadRequest("Nisu uneti validni podaci!");

                Korisnik pronadjen = BazaPodataka.Korisnici.FirstOrDefault(x => x.KorisnickoIme == podaci.KorisnickoIme && x.Lozinka == podaci.Lozinka);
                if (pronadjen != null)
                {
                    HttpContext.Current.Session["korisnik"] = pronadjen;
                    return Ok(new { data = pronadjen });
                }
                else
                {
                    return BadRequest("Nevalidni kredencijali!");
                }
            }
            catch
            {
                return NotFound();
            }
        }

        [Route("registracija")]
        [HttpPost]
        public IHttpActionResult Registracija([FromBody] RegistracijaDTO podaci)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return BadRequest("Nisu uneti validni podaci!");

                Korisnik pronadjen = BazaPodataka.Korisnici.FirstOrDefault(x => x.KorisnickoIme == podaci.KorisnickoIme);
                if (pronadjen == null)
                {
                    Korisnik korisnik = new Korisnik()
                    {
                        Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        KorisnickoIme = podaci.KorisnickoIme,
                        Lozinka = podaci.Lozinka,
                        Ime = podaci.Ime,
                        Prezime = podaci.Prezime,
                        Email = podaci.Email,
                        DatumRodjenja = podaci.DatumRodjenja,
                        Pol = podaci.Pol,
                        TipKorisnika = TipKorisnika.TURISTA
                    };

                    BazaPodataka.Korisnici.Add(korisnik);
                    BazaPodataka.SacuvajPodatke();

                    HttpContext.Current.Session["korisnik"] = korisnik;
                    return Ok(new { data = korisnik });
                }
                else
                {
                    return BadRequest("Korisnicko ime je zauzeto!");
                }
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("dodajmenadzera")]
        [HttpPost]
        public IHttpActionResult DodajMenadzera([FromBody] RegistracijaDTO podaci)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return BadRequest("Nisu uneti validni podaci!");

                Korisnik pronadjen = BazaPodataka.Korisnici.FirstOrDefault(x => x.KorisnickoIme == podaci.KorisnickoIme);
                if (pronadjen == null)
                {
                    Korisnik korisnik = new Korisnik()
                    {
                        Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        KorisnickoIme = podaci.KorisnickoIme,
                        Lozinka = podaci.Lozinka,
                        Ime = podaci.Ime,
                        Prezime = podaci.Prezime,
                        Email = podaci.Email,
                        DatumRodjenja = podaci.DatumRodjenja,
                        Pol = podaci.Pol,
                        TipKorisnika = TipKorisnika.MENADZER
                    };

                    BazaPodataka.Korisnici.Add(korisnik);
                    BazaPodataka.SacuvajPodatke();

                    return Ok(new { data = korisnik });
                }
                else
                {
                    return BadRequest("Korisnicko ime je zauzeto!");
                }
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("odjava")]
        [HttpGet]
        public IHttpActionResult Odjava()
        {
            try
            {
                HttpContext.Current.Session["korisnik"] = null;
                HttpContext.Current.Session.Clear();
                return Ok();
            }
            catch
            {
                return InternalServerError();
            }
        }
    }
}

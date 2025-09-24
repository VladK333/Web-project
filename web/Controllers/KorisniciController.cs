using projekat.DTO;
using projekat.Models;
using projekat.Podaci;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/korisnici")]
    public class KorisniciController : ApiController
    {
        [Route("svi")]
        [HttpGet]
        public IHttpActionResult SviKorisnici()
        {
            try
            {
                // Samo admin moze da pristupa listi svih korisnika
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik && korisnik.TipKorisnika == TipKorisnika.ADMIN)
                {
                    return Ok(new { data = BazaPodataka.Korisnici });
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("get")]
        [HttpGet]
        public IHttpActionResult DobaviKorisnika()
        {
            try
            {
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik)
                {
                    Korisnik trazeni = BazaPodataka.Korisnici.FirstOrDefault(x => x.Id == korisnik.Id);
                    if (trazeni == null)
                        return NotFound();
                    else
                        return Ok(new { data = trazeni });
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("update")]
        [HttpPost]
        public IHttpActionResult AzurirajKorisnika([FromBody] RegistracijaDTO podaci)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return BadRequest("Nisu uneti validni podaci!");

                // Samo korisnik moze da pristupa svojim podacima i da ih menja tj azurira
                if (HttpContext.Current.Session["korisnik"] is Korisnik korisnik)
                {
                    Korisnik trazeni = BazaPodataka.Korisnici.FirstOrDefault(x => x.Id == korisnik.Id);
                    if (trazeni == null)
                        return NotFound();
                    else
                    {
                        // Ako menja korisnicko ime, onda ono mora biti jedinstveno
                        if (podaci.KorisnickoIme != korisnik.KorisnickoIme)
                        {
                            if (BazaPodataka.Korisnici.FirstOrDefault(x => x.Id != korisnik.Id && x.KorisnickoIme == podaci.KorisnickoIme) != null)
                                return BadRequest("Korisnicko ime je zauzeto!");
                        }

                        // Azuriranje podataka korisnika i podataka o korisniku u sesiji
                        trazeni.KorisnickoIme = podaci.KorisnickoIme;
                        trazeni.Lozinka = podaci.Lozinka;
                        trazeni.Ime = podaci.Ime;
                        trazeni.Prezime = podaci.Prezime;
                        trazeni.Email = podaci.Email;
                        trazeni.DatumRodjenja = podaci.DatumRodjenja;
                        trazeni.Pol = podaci.Pol;

                        BazaPodataka.SacuvajPodatke();
                        HttpContext.Current.Session["korisnik"] = trazeni;

                        return Ok(new { data = trazeni });
                    }
                }
                else
                    return Unauthorized();
            }
            catch
            {
                return InternalServerError();
            }
        }
    }
}

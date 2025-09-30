using projekat.Models;
using projekat.Podaci;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/aranzmani")]
    public class AranzmaniController : ApiController
    {
        [HttpGet]
        [Route("menadzer")]
        public IHttpActionResult MojiAranzmani()
        {
            if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) || korisnik.TipKorisnika != TipKorisnika.MENADZER)
                return Unauthorized();

            var svi = BazaPodataka.Aranzmani.Where(a => !a.IsDeleted).ToList();

            var dto = svi.Select(a => new
            {
                a.Id,
                a.Naziv,
                TipAranzmana = a.TipAranzmana.ToString(),
                TipPrevoza = a.TipPrevoza.ToString(),
                DatumPocetka = a.DatumPocetka.ToString("yyyy-MM-dd"),
                DatumZavrsetka = a.DatumZavrsetka.ToString("yyyy-MM-dd"),
                MaksPutnika = a.MaksimalanBrojPutnika,
                MozeUrediti = a.KorisnikId == korisnik.Id
            });

            return Ok(new { data = dto });
        }

        [HttpPost]
        [Route("dodaj")]
        public IHttpActionResult Dodaj(Aranzman dto)
        {
            if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) || korisnik.TipKorisnika != TipKorisnika.MENADZER)
                return Unauthorized();

            // Provera da li smestaj već zauzet
            if (BazaPodataka.Aranzmani.Any(a => a.SmestajId == dto.SmestajId && !a.IsDeleted))
                return BadRequest("Smestaj je već dodeljen nekom aranžmanu.");

            dto.Id = DateTimeOffset.Now.ToUnixTimeSeconds();
            dto.KorisnikId = korisnik.Id;

            BazaPodataka.Aranzmani.Add(dto);
            BazaPodataka.SacuvajPodatke();

            return Ok(new { success = true });
        }

        [HttpPatch]
        [Route("update")]
        public IHttpActionResult Izmeni(Aranzman dto)
        {
            if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) || korisnik.TipKorisnika != TipKorisnika.MENADZER)
                return Unauthorized();

            var ar = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == dto.Id && !a.IsDeleted);
            if (ar == null)
                return NotFound();

            if (ar.KorisnikId != korisnik.Id)
                return Unauthorized();

            ar.Naziv = dto.Naziv;
            ar.TipAranzmana = dto.TipAranzmana;
            ar.TipPrevoza = dto.TipPrevoza;
            ar.DatumPocetka = dto.DatumPocetka;
            ar.DatumZavrsetka = dto.DatumZavrsetka;
            ar.MaksimalanBrojPutnika = dto.MaksimalanBrojPutnika;
            ar.Opis = dto.Opis;
            ar.ProgramPutovanja = dto.ProgramPutovanja;
            ar.PosterAranzmana = dto.PosterAranzmana;
            ar.SmestajId = dto.SmestajId;
            ar.Lokacija = dto.Lokacija;

            BazaPodataka.SacuvajPodatke();

            return Ok(new { success = true });
        }

        [HttpPatch]
        [Route("delete/{id:long}")]
        public IHttpActionResult Obrisi(long id)
        {
            if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) || korisnik.TipKorisnika != TipKorisnika.MENADZER)
                return Unauthorized();

            var ar = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (ar == null)
                return NotFound();

            if (ar.KorisnikId != korisnik.Id)
                return Unauthorized();

            if (BazaPodataka.Rezervacije.Any(r => r.AranzmanId == id))
                return BadRequest("Ne moze!");

            ar.IsDeleted = true;
            BazaPodataka.SacuvajPodatke();

            return Ok(new { success = true });
        }

        [HttpGet]
        [Route("get/{id:long}")]
        public IHttpActionResult Detalji(long id)
        {
            var ar = BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (ar == null)
                return NotFound();

            // Dohvati smeštaj
            var smestaj = BazaPodataka.Smestaji
                .Where(s => s.Id == ar.SmestajId && !s.IsDeleted)
                .Select(s => new
                {
                    s.Id,
                    s.Naziv,
                    s.Tip,
                    s.BrojZvezdica,
                    s.ImaBazen,
                    s.ImaSpaCentar,
                    s.ImaWifi,
                    s.PrilagodjenoZaInvaliditet,
                    SmestajneJedinice = BazaPodataka.SmestajneJedinice
                        .Where(j => s.SmestajneJedinice.Contains(j.Id) && !j.IsDeleted)
                        .Select(j => new
                        {
                            j.Id,
                            j.DozvoljenBrojGostiju,
                            j.Cena,
                            j.DozvoljenBoravakKucnimLjubimcima
                        })
                        .ToList()
                })
                .FirstOrDefault();

            var dto = new
            {
                ar.Id,
                ar.Naziv,
                TipAranzmana = ar.TipAranzmana.ToString(),
                TipPrevoza = ar.TipPrevoza.ToString(),
                ar.Lokacija,
                DatumPocetka = ar.DatumPocetka.ToString("yyyy-MM-dd"),
                DatumZavrsetka = ar.DatumZavrsetka.ToString("yyyy-MM-dd"),
                ar.MaksimalanBrojPutnika,
                ar.Opis,
                ar.ProgramPutovanja,
                ar.PosterAranzmana,
                ar.KorisnikId,
                Smestaj = smestaj
            };

            return Ok(new { data = dto });
        }

        [HttpGet]
        [Route("predstojeci")]
        public IHttpActionResult Predstojeci()
        {
            var danas = DateTime.Now.Date;
            var lista = BazaPodataka.Aranzmani
                .Where(a => !a.IsDeleted && a.DatumPocetka >= danas)
                .Select(a => new
                {
                    a.Id,
                    a.Naziv,
                    TipAranzmana = a.TipAranzmana.ToString(),
                    TipPrevoza = a.TipPrevoza.ToString(),
                    DatumPocetka = a.DatumPocetka.ToString("yyyy-MM-dd"),
                    DatumZavrsetka = a.DatumZavrsetka.ToString("yyyy-MM-dd"),
                    a.Lokacija,
                    MaksPutnika = a.MaksimalanBrojPutnika,
                    a.PosterAranzmana,
                    a.SmestajId
                })
                .ToList();

            return Ok(new { data = lista });
        }

        [HttpGet]
        [Route("prethodni")]
        public IHttpActionResult Prethodni()
        {
            var danas = DateTime.Now.Date;
            var lista = BazaPodataka.Aranzmani
                .Where(a => !a.IsDeleted && a.DatumPocetka < danas)
                .Select(a => new
                {
                    a.Id,
                    a.Naziv,
                    TipAranzmana = a.TipAranzmana.ToString(),
                    TipPrevoza = a.TipPrevoza.ToString(),
                    DatumPocetka = a.DatumPocetka.ToString("yyyy-MM-dd"),
                    DatumZavrsetka = a.DatumZavrsetka.ToString("yyyy-MM-dd"),
                    a.Lokacija,
                    MaksPutnika = a.MaksimalanBrojPutnika,
                    a.PosterAranzmana,
                    a.SmestajId
                })
                .ToList();

            return Ok(new { data = lista });
        }

    }
}

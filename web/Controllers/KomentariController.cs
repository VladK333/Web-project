using projekat.Models;
using projekat.Podaci;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace projekat.Controllers
{
    [RoutePrefix("api/komentari")]
    public class KomentariController : ApiController
    {
        // POST: api/komentari/dodaj
        [Route("dodaj")]
        [HttpPost]
        public IHttpActionResult DodajKomentar([FromBody] Komentar komentar)
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.TURISTA)
                    return BadRequest("Morate biti prijavljeni kao turista da biste ostavili komentar.");

                var aranzmaniTurista = BazaPodataka.Rezervacije
                    .Where(r => r.TuristaId == korisnik.Id && r.Status == StatusRezervacijeAranzmana.AKTIVNA)
                    .Select(r => BazaPodataka.Aranzmani.FirstOrDefault(a => a.Id == r.AranzmanId))
                    .Where(a => a != null && a.DatumZavrsetka < DateTime.Now)
                    .Select(a => a.SmestajId)
                    .ToList();

                if (!aranzmaniTurista.Contains(komentar.SmestajId))
                    return BadRequest("Možete komentarisati samo smeštaj u okviru završenih aranžmana na kojima ste boravili.");

                komentar.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                komentar.TuristaId = korisnik.Id;
                komentar.Status = StatusKomentara.KREIRAN;

                BazaPodataka.Komentari.Add(komentar);
                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = komentar });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // GET: api/komentari/moji
        [Route("moji")]
        [HttpGet]
        public IHttpActionResult MojiKomentari()
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.TURISTA)
                    return BadRequest("Morate biti prijavljeni kao turista.");

                var komentari = BazaPodataka.Komentari
                    .Where(k => k.TuristaId == korisnik.Id)
                    .Select(k =>
                    {
                        var smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == k.SmestajId);
                        return new
                        {
                            Komentar = k,
                            Smestaj = smestaj?.Naziv ?? ""
                        };
                    })
                    .ToList();

                return Ok(new { data = komentari });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // GET: api/komentari/menadzer
        [Route("menadzer")]
        [HttpGet]
        public IHttpActionResult KomentariMenadzera()
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.MENADZER)
                    return BadRequest("Samo menadžer može pristupiti ovom resursu.");

                // Dohvati sve aranžmane koje je menadžer kreirao
                var aranzmaniMenadzera = BazaPodataka.Aranzmani
                    .Where(a => a.KorisnikId == korisnik.Id)
                    .Select(a => a.SmestajId)
                    .ToList();

                // Dohvati komentare koji pripadaju tim aranžmanima
                var komentari = BazaPodataka.Komentari
                            .Where(k => aranzmaniMenadzera.Contains(k.SmestajId))
                            .Select(k =>
                            {
                                var turista = BazaPodataka.Korisnici.FirstOrDefault(t => t.Id == k.TuristaId);
                                var smestaj = BazaPodataka.Smestaji.FirstOrDefault(s => s.Id == k.SmestajId);
                                return new
                                {
                                    KomentarId = k.Id,
                                    RezervacijaId = k.RezervacijaId,
                                    Turista = turista != null ? turista.Ime + " " + turista.Prezime : "",
                                    Smestaj = smestaj?.Naziv ?? "",
                                    Tekst = k.Tekst,
                                    Ocena = k.Ocena,
                                    Status = k.Status.ToString() // OBAVEZNO! pretvori enum u string
                                };
                            })
                            .ToList();

                return Ok(new { data = komentari });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // PATCH: api/komentari/odobri/{id}
        [Route("odobri/{id}")]
        [HttpPatch]
        public IHttpActionResult OdobriKomentar(long id)
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.MENADZER)
                    return BadRequest("Samo menadžer može odobriti komentar.");

                var komentar = BazaPodataka.Komentari.FirstOrDefault(k => k.Id == id);
                if (komentar == null) return NotFound();

                // menadžer može odobriti samo komentare na svojim aranžmanima
                var aranzman = BazaPodataka.Aranzmani.FirstOrDefault(a => a.SmestajId == komentar.SmestajId && a.KorisnikId == korisnik.Id);
                if (aranzman == null)
                    return BadRequest("Nemate ovlašćenje da odobrite ovaj komentar.");

                komentar.Status = StatusKomentara.ODOBREN;
                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = komentar });
            }
            catch
            {
                return InternalServerError();
            }
        }

        // GET: api/komentari/smestaj/{smestajId}
        [Route("smestaj/{smestajId}")]
        [HttpGet]
        public IHttpActionResult KomentariPoSmestaju(long smestajId)
        {
            try
            {
                // Dohvati sve komentare za dati smeštaj
                var komentari = BazaPodataka.Komentari
                    .Where(k => k.SmestajId == smestajId && k.Status == StatusKomentara.ODOBREN) // prikazujemo samo odobrene komentare
                    .Select(k =>
                    {
                        var turista = BazaPodataka.Korisnici.FirstOrDefault(t => t.Id == k.TuristaId);
                        return new
                        {
                            KomentarId = k.Id,
                            RezervacijaId = k.RezervacijaId,
                            Turista = turista != null ? turista.Ime + " " + turista.Prezime : "",
                            Tekst = k.Tekst,
                            Ocena = k.Ocena,
                            Status = k.Status.ToString()
                        };
                    })
                    .ToList();

                return Ok(new { data = komentari });
            }
            catch
            {
                return InternalServerError();
            }
        }


        // PATCH: api/komentari/odbij/{id}
        [Route("odbij/{id}")]
        [HttpPatch]
        public IHttpActionResult OdbijKomentar(long id)
        {
            try
            {
                if (!(HttpContext.Current.Session["korisnik"] is Korisnik korisnik) ||
                    korisnik.TipKorisnika != TipKorisnika.MENADZER)
                    return BadRequest("Samo menadžer može odbiti komentar.");

                var komentar = BazaPodataka.Komentari.FirstOrDefault(k => k.Id == id);
                if (komentar == null) return NotFound();

                var aranzman = BazaPodataka.Aranzmani.FirstOrDefault(a => a.SmestajId == komentar.SmestajId && a.KorisnikId == korisnik.Id);
                if (aranzman == null)
                    return BadRequest("Nemate ovlašćenje da odbijete ovaj komentar.");

                komentar.Status = StatusKomentara.ODBIJEN;
                BazaPodataka.SacuvajPodatke();

                return Ok(new { data = komentar });
            }
            catch
            {
                return InternalServerError();
            }
        }

    }
}

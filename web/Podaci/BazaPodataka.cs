using Newtonsoft.Json;
using projekat.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace projekat.Podaci
{
    public class BazaPodataka
    {
        public static List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        public static List<SmestajnaJedinica> SmestajneJedinice { get; set; } = new List<SmestajnaJedinica>();
        public static List<Aranzman> Aranzmani { get; set; } = new List<Aranzman>();
        public static List<Smestaj> Smestaji { get; set; } = new List<Smestaj>();
        public static List<RezervacijaAranzmana> Rezervacije { get; set; } = new List<RezervacijaAranzmana>();
        public static List<Komentar> Komentari { get; set; } = new List<Komentar>();

        public BazaPodataka() { }

        private static void Serialize<T>(string json, List<T> lista)
        {
            try
            {
                string filePath = Path.Combine(HostingEnvironment.MapPath(@"~/Podaci/json"), json);
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    sw.Write(JsonConvert.SerializeObject(lista, Formatting.Indented));
                }
            }
            catch { }
        }

        private static List<T> Deserialize<T>(string json)
        {
            try
            {
                string putanja = Path.Combine(HostingEnvironment.MapPath(@"~/Podaci/json/"), json);
                if (File.Exists(putanja))
                {
                    using (StreamReader sr = new StreamReader(putanja))
                    {
                        return JsonConvert.DeserializeObject<List<T>>(sr.ReadToEnd());
                    }
                }
                return new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
}
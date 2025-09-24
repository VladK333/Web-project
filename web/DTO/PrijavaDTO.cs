using System.ComponentModel.DataAnnotations;

namespace projekat.DTO
{
    public class PrijavaDTO
    {
        [Required]
        public string KorisnickoIme { get; set; }

        [Required]
        public string Lozinka { get; set; }
    }
}
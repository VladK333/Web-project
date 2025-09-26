namespace projekat.DTO
{
    public class SmestajnaJedinicaDTO
    {
        public long Id { get; set; }
        public int BrojKreveta { get; set; }
        public decimal Cena { get; set; }
        public bool DozvoljenBoravakKucnimLjubimcima { get; set; }

        public SmestajnaJedinicaDTO() { }
    }
}
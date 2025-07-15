namespace EtkinlikProjem.Models
{
    public class AddParticipantViewModel
    {
        public int EtkinlikID { get; set; }
        public List<Kullanici> AvailableUsers { get; set; }
    }

    public class Kullanici
    {
        public int KullaniciID { get; set; }
        public string KullaniciAdi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
    }
}

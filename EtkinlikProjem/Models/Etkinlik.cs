namespace EtkinlikProjem.Models
{
    public class Etkinlik
    {
        public int EtkinlikID { get; set; }
        public string EtkinlikAdi { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public TimeSpan Saat { get; set; }
        public int EtkinlikSuresi { get; set; }
        public string Konum { get; set; }
        public string Kategori { get; set; }
        public int KullaniciID { get; set; }

        public bool Onay { get; set; } // Yeni eklenen özellik
    }

}

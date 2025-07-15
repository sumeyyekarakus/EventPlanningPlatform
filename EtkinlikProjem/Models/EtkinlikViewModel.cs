namespace EtkinlikProjem.Models
{
    public class EtkinlikViewModel
    {
        
            public int EtkinlikID { get; set; }
            public string EtkinlikAdi { get; set; } = string.Empty;
            public string Aciklama { get; set; } = string.Empty;
            public DateTime Tarih { get; set; }
            public TimeSpan Saat { get; set; }
            public int EtkinlikSuresi { get; set; }
            public string Konum { get; set; } = string.Empty;
            public string Kategori { get; set; } = string.Empty;
            public bool Onay { get; set; }
        
    }
}

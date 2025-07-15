using EtkinlikProjem.Interfaces;
using EtkinlikProjem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace EtkinlikProjem.Controllers
{
    public class KullaniciPanelController : Controller
    {
        //private readonly IConfiguration _configuration;
        //private readonly IEmailService _emailService;

        //public KullaniciPanelController(IConfiguration configuration, IEmailService emailService)
        //{
        //    _configuration = configuration;
        //    _emailService = emailService;
        //}
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MainView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string kullaniciAdi, string sifre)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
            string query = "SELECT KullaniciID, Ad, Soyad, Rol, Sifre FROM Kullanici WHERE KullaniciAdi = @KullaniciAdi;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string hashedPassword = reader.GetString(4); // Veritabanındaki hashlenmiş şifre
                            if (BCrypt.Net.BCrypt.Verify(sifre, hashedPassword))
                            {
                                // Giriş başarılı
                                HttpContext.Session.SetString("KullaniciID", reader["KullaniciID"].ToString());
                                return RedirectToAction("Main");
                            }
                        }
                    }
                }
            }

            // Giriş başarısız
            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View("Index");
        }


        // Route attribute'unu kaldırın veya düzenleyin
        [HttpGet]
        public IActionResult KayitOl()
        {
            // Kayıt ol sayfasını gösterecek boş bir metot
            return View();
        }

        [HttpPost]
        public IActionResult KayitOl(string kullaniciAdi, string sifre, string ad, string soyad, DateTime dogumTarihi,
                             string cinsiyet, string email, string telefonNumarasi, string ilgiAlani,
                             string konum, IFormFile profilFotografi)
        {
            // Mevcut kayıt ol metodu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(sifre);

            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
            string query = "INSERT INTO Kullanici (KullaniciAdi, Sifre, Ad, Soyad, DogumTarihi, Cinsiyet, Email, TelefonNumarasi, IlgiAlani, Konum, ProfilFotoğrafı, Rol) " +
                           "VALUES (@KullaniciAdi, @Sifre, @Ad, @Soyad, @DogumTarihi, @Cinsiyet, @Email, @TelefonNumarasi, @IlgiAlani, @Konum, @ProfilFotoğrafı, 'Kullanıcı')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                    command.Parameters.AddWithValue("@Sifre", hashedPassword);
                    command.Parameters.AddWithValue("@Ad", ad);
                    command.Parameters.AddWithValue("@Soyad", soyad);
                    command.Parameters.AddWithValue("@DogumTarihi", dogumTarihi);
                    command.Parameters.AddWithValue("@Cinsiyet", cinsiyet);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@TelefonNumarasi", telefonNumarasi);
                    command.Parameters.AddWithValue("@IlgiAlani", ilgiAlani);
                    command.Parameters.AddWithValue("@Konum", konum);
                    command.Parameters.AddWithValue("@ProfilFotoğrafı", profilFotografi?.FileName ?? "");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Kayıt başarılı olduktan sonra nereye yönlendirmek istiyorsanız
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SifreSifirla()
        {
            // Kayıt ol sayfasını gösterecek boş bir metot
            return View();
        }

        //[HttpPost]
        //public IActionResult SifreSifirla(string email)
        //{
        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        //    string query = "SELECT Email FROM Kullanici WHERE Email = @Email";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Email", email);

        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();

        //            if (reader.HasRows)
        //            {
        //                // Şifre sıfırlama işlemi (örneğin e-posta gönderme veya yeni şifre atama)
        //                ViewBag.Mesaj = "Şifre sıfırlama talimatları e-postanıza gönderildi.";
        //            }
        //            else
        //            {
        //                ViewBag.Hata = "Bu e-posta adresi kayıtlı değil.";
        //            }
        //        }
        //    }

        //    return View("Index");
        //}
        //public class PasswordResetToken
        //{
        //    public int KullaniciID { get; set; }
        //    public string Token { get; set; }
        //    public DateTime ExpirationTime { get; set; }
        //}

        //[HttpPost]
        //public async Task<IActionResult> SifreSifirla(string email)
        //{
        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";


        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string checkQuery = "SELECT KullaniciID, Ad FROM Kullanici WHERE Email = @Email";
        //        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
        //        {
        //            checkCommand.Parameters.AddWithValue("@Email", email);

        //            connection.Open();
        //            SqlDataReader reader = checkCommand.ExecuteReader();

        //            if (reader.Read())
        //            {
        //                int kullaniciID = reader.GetInt32(0);
        //                string kullaniciAdi = reader.GetString(1);

        //                // Generate a unique reset token
        //                string resetToken = GenerateResetToken();
        //                DateTime expirationTime = DateTime.UtcNow.AddHours(1);

        //                // Store token in database
        //                StoreResetToken(kullaniciID, resetToken, expirationTime);

        //                // Send reset email
        //                string resetLink = Url.Action("SifreYenile", "KullaniciPanel",
        //                    new { token = resetToken }, Request.Scheme);

        //                await _emailService.SendPasswordResetEmailAsync(email, resetLink);

        //                TempData["Mesaj"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Bu e-posta adresi kayıtlı değil.");
        //                return View();
        //            }
        //        }
        //    }
        //}

        //[HttpGet]
        //public IActionResult SifreYenile(string token)
        //{
        //    var resetToken = ValidateResetToken(token);
        //    if (resetToken == null)
        //    {
        //        TempData["Hata"] = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
        //        return RedirectToAction("Index");
        //    }

        //    return View(new PasswordResetViewModel { Token = token });
        //}

        //[HttpPost]
        //public IActionResult SifreYenile(PasswordResetViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var resetToken = ValidateResetToken(model.Token);
        //    if (resetToken == null)
        //    {
        //        TempData["Hata"] = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
        //        return RedirectToAction("Index");
        //    }

        //    // Hash new password
        //    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.YeniSifre);

        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string updateQuery = @"
        //        UPDATE Kullanici 
        //        SET Sifre = @Sifre 
        //        WHERE KullaniciID = @KullaniciID";

        //        using (SqlCommand command = new SqlCommand(updateQuery, connection))
        //        {
        //            command.Parameters.AddWithValue("@Sifre", hashedPassword);
        //            command.Parameters.AddWithValue("@KullaniciID", resetToken.KullaniciID);

        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }

        //        // Delete used token
        //        DeleteResetToken(resetToken.KullaniciID);
        //    }

        //    TempData["Basari"] = "Şifreniz başarıyla değiştirildi. Giriş yapabilirsiniz.";
        //    return RedirectToAction("Index");
        //}

        //private string GenerateResetToken()
        //{
        //    return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        //        .Replace("/", "-")
        //        .Replace("+", "_")
        //        .TrimEnd('=');
        //}

        //private void StoreResetToken(int kullaniciID, string token, DateTime expirationTime)
        //{
        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //        DELETE FROM PasswordResetTokens WHERE KullaniciID = @KullaniciID;
        //        INSERT INTO PasswordResetTokens (KullaniciID, Token, ExpirationTime) 
        //        VALUES (@KullaniciID, @Token, @ExpirationTime)";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
        //            command.Parameters.AddWithValue("@Token", token);
        //            command.Parameters.AddWithValue("@ExpirationTime", expirationTime);

        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //private PasswordResetToken ValidateResetToken(string token)
        //{
        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //        SELECT KullaniciID, Token, ExpirationTime 
        //        FROM PasswordResetTokens 
        //        WHERE Token = @Token AND ExpirationTime > @Now";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Token", token);
        //            command.Parameters.AddWithValue("@Now", DateTime.UtcNow);

        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();

        //            if (reader.Read())
        //            {
        //                return new PasswordResetToken
        //                {
        //                    KullaniciID = reader.GetInt32(0),
        //                    Token = reader.GetString(1),
        //                    ExpirationTime = reader.GetDateTime(2)
        //                };
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private void DeleteResetToken(int kullaniciID)
        //{
        //    string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "DELETE FROM PasswordResetTokens WHERE KullaniciID = @KullaniciID";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@KullaniciID", kullaniciID);

        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}




        // Etkinlik İşlemleri

        public IActionResult Main()
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
            List<Etkinlik> etkinlikler = new List<Etkinlik>();

            string kullaniciID = HttpContext.Session.GetString("KullaniciID");
            string ilgiAlanlari = ""; // Kullanıcının ilgi alanları

            // Kullanıcının ilgi alanlarını çekiyoruz
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string ilgiAlaniQuery = "SELECT IlgiAlani FROM Kullanici WHERE KullaniciID = @KullaniciID";
                using (SqlCommand command = new SqlCommand(ilgiAlaniQuery, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", Convert.ToInt32(kullaniciID));
                    ilgiAlanlari = command.ExecuteScalar()?.ToString();
                }
            }

            // Etkinlikleri çekiyoruz
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string etkinlikQuery = @"
            WITH KullaniciIlgiAlanlari AS (
                SELECT value AS IlgiAlani
                FROM STRING_SPLIT(@IlgiAlani, ',')
            )
            SELECT e.EtkinlikID, e.EtkinlikAdi, e.Tarih, e.Kategori, e.KullaniciID
            FROM Etkinlik e
            WHERE Onay = 1
            ORDER BY 
                CASE 
                    WHEN e.Kategori IN (SELECT IlgiAlani FROM KullaniciIlgiAlanlari) THEN 3
                    WHEN e.Kategori IN (
                        SELECT DISTINCT et.Kategori 
                        FROM Katilimci k
                        INNER JOIN Etkinlik et ON k.EtkinlikID = et.EtkinlikID
                        WHERE k.KullaniciID = @KullaniciID
                    ) THEN 2
                    ELSE 1
                END DESC, 
                e.Tarih ASC";
                using (SqlCommand command = new SqlCommand(etkinlikQuery, connection))
                {
                    command.Parameters.AddWithValue("@IlgiAlani", ilgiAlanlari);
                    command.Parameters.AddWithValue("@KullaniciID", Convert.ToInt32(kullaniciID));
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        etkinlikler.Add(new Etkinlik
                        {
                            EtkinlikID = reader["EtkinlikID"] != DBNull.Value ? Convert.ToInt32(reader["EtkinlikID"]) : 0,
                            EtkinlikAdi = reader["EtkinlikAdi"] != DBNull.Value ? reader["EtkinlikAdi"].ToString() : string.Empty,
                            Tarih = reader["Tarih"] != DBNull.Value ? Convert.ToDateTime(reader["Tarih"]) : DateTime.MinValue,
                            KullaniciID = reader["KullaniciID"] != DBNull.Value ? Convert.ToInt32(reader["KullaniciID"]) : 0,
                            Kategori = reader["Kategori"] != DBNull.Value ? reader["Kategori"].ToString() : string.Empty
                        });
                    }

                }
            }
            // Kullanıcının toplam puanını al
            string toplamPuanQuery = "SELECT SUM(Puan) FROM Puan WHERE KullaniciID = @KullaniciID";
            int toplamPuan = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(toplamPuanQuery, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                    connection.Open();

                    // ExecuteScalar() kullanarak toplam puanı al
                    var result = command.ExecuteScalar();

                    // Eğer result DBNull ise, toplamPuan'ı 0 olarak ayarlayın
                    if (result != DBNull.Value)
                    {
                        toplamPuan = Convert.ToInt32(result);
                    }
                    else
                    {
                        toplamPuan = 0; // veya başka bir uygun varsayılan değer
                    }
                }
            }
            ViewBag.ToplamPuan = toplamPuan;


            return View(etkinlikler);
        }



        //[HttpGet]
        //public IActionResult EtkinlikDetay()
        //{
        //    return View();
        //}


        [HttpGet]
        public IActionResult EtkinlikDetay(int etkinlikID)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";

            Etkinlik etkinlik = null;
            string query = @"
        SELECT * FROM Etkinlik 
        WHERE EtkinlikID = @EtkinlikID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        etkinlik = new Etkinlik
                        {
                            EtkinlikID = Convert.ToInt32(reader["EtkinlikID"]),
                            EtkinlikAdi = reader["EtkinlikAdi"].ToString(),
                            Aciklama = reader["Aciklama"].ToString(),
                            Tarih = Convert.ToDateTime(reader["Tarih"]),
                            Saat = (TimeSpan)reader["Saat"],
                            EtkinlikSuresi = Convert.ToInt32(reader["EtkinlikSuresi"]),
                            Konum = reader["Konum"].ToString(),
                            Kategori = reader["Kategori"].ToString()
                        };
                    }
                }

            }
            if (TempData["AlternatifEtkinlikler"] != null)
            {
                try
                {
                    // TempData'dan gelen veriyi string olarak al
                    var alternatifEtkinliklerJson = TempData["AlternatifEtkinlikler"].ToString();

                    // JSON ayarları
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Deserialize işlemi
                    var alternatifEtkinlikler = JsonSerializer.Deserialize<List<Etkinlik>>(alternatifEtkinliklerJson, options);

                    ViewBag.AlternatifEtkinlikler = alternatifEtkinlikler;
                }
                catch (Exception ex)
                {
                    // Hata durumunda log atabilir veya kullanıcıya mesaj gösterebilirsiniz
                    ViewBag.HataMessaji = "Etkinlik bilgileri yüklenirken bir hata oluştu: " + ex.Message;
                }
            }
            if (etkinlik == null)
            {
                return NotFound();
            }

            return View(etkinlik);
        }



        public class EtkinlikCakismaDto
        {
            public int EtkinlikID { get; set; }
            public string EtkinlikAdi { get; set; }
            public string Tarih { get; set; }
            public string Saat { get; set; }
            public int EtkinlikSuresi { get; set; }
        }

        [HttpPost]
        public IActionResult Katil(int etkinlikID)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";

            try
            {
                int kullaniciID = Convert.ToInt32(HttpContext.Session.GetString("KullaniciID"));

                // Kullanıcının bu etkinliğe daha önce katılıp katılmadığını kontrol et
                string katilimKontrolQuery = @"
    SELECT COUNT(*) 
    FROM Katilimci 
    WHERE KullaniciID = @KullaniciID AND EtkinlikID = @EtkinlikID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(katilimKontrolQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);

                        connection.Open();
                        int katilimSayisi = Convert.ToInt32(command.ExecuteScalar());
                        
                        if (katilimSayisi > 0)
                        {
                            TempData["KatılımMesajı"] = "Bu etkinliğe daha önce katıldınız.";
                            return RedirectToAction("EtkinlikDetay", new { etkinlikID });
                        }
                    }
                }

                // Yeni etkinlik bilgilerini al
                string etkinlikQuery = "SELECT Tarih, Saat, EtkinlikSuresi FROM Etkinlik WHERE EtkinlikID = @EtkinlikID";
                DateTime etkinlikTarihi;
                TimeSpan yeniEtkinlikBaslangic;
                int yeniEtkinlikSuresi;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(etkinlikQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                etkinlikTarihi = Convert.ToDateTime(reader["Tarih"]);
                                yeniEtkinlikBaslangic = TimeSpan.Parse(reader["Saat"].ToString());
                                yeniEtkinlikSuresi = Convert.ToInt32(reader["EtkinlikSuresi"]);
                            }
                            else
                            {
                                TempData["KatılımMesajı"] = "Etkinlik bulunamadı.";
                                return RedirectToAction("EtkinlikDetay", new { etkinlikID });
                            }
                        }
                    }
                }

                // Çakışma kontrolü yap
                string cakisQuery = @"
    SELECT e.EtkinlikID, e.EtkinlikAdi, e.Tarih, e.Saat, e.EtkinlikSuresi
    FROM Etkinlik e
    INNER JOIN Katilimci k ON e.EtkinlikID = k.EtkinlikID
    WHERE k.KullaniciID = @KullaniciID
    AND e.Tarih = @EtkinlikTarihi
    AND (
        (@YeniEtkinlikBaslangic BETWEEN e.Saat AND DATEADD(MINUTE, e.EtkinlikSuresi, e.Saat))
        OR (DATEADD(MINUTE, @YeniEtkinlikSuresi, @YeniEtkinlikBaslangic) BETWEEN e.Saat AND DATEADD(MINUTE, e.EtkinlikSuresi, e.Saat))
        OR (@YeniEtkinlikBaslangic <= e.Saat AND DATEADD(MINUTE, @YeniEtkinlikSuresi, @YeniEtkinlikBaslangic) >= DATEADD(MINUTE, e.EtkinlikSuresi, e.Saat))
    )";

                List<Etkinlik> cakisAnkEtkinlikler = new List<Etkinlik>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(cakisQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                        command.Parameters.AddWithValue("@EtkinlikTarihi", etkinlikTarihi);
                        command.Parameters.AddWithValue("@YeniEtkinlikBaslangic", yeniEtkinlikBaslangic);
                        command.Parameters.AddWithValue("@YeniEtkinlikSuresi", yeniEtkinlikSuresi);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cakisAnkEtkinlikler.Add(new Etkinlik
                                {
                                    EtkinlikID = reader.GetInt32(0),
                                    EtkinlikAdi = reader.GetString(1),
                                    Tarih = reader.GetDateTime(2),
                                    Saat = reader.GetTimeSpan(3),
                                    EtkinlikSuresi = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }

                if (cakisAnkEtkinlikler.Count > 0)
                {
                    TempData["KatılımMesajı"] = "Bu etkinlik başka bir etkinlikle çakışıyor!";
                    TempData["AlternatifEtkinlikler"] = JsonSerializer.Serialize(cakisAnkEtkinlikler);
                    return RedirectToAction("EtkinlikDetay", new { etkinlikID });
                }

                // Katılımı kaydet
                string insertQuery = @"
    INSERT INTO Katilimci (KullaniciID, EtkinlikID) 
    VALUES (@KullaniciID, @EtkinlikID)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // Kullanıcının etkinliğe katılımı sonrası puan ekleme
                // Kullanıcının bu etkinliğe ilk defa katılıp katılmadığını kontrol et
                // Kullanıcının toplam katılım sayısını kontrol et
                string ilkKatilimKontrolQuery = @"
SELECT COUNT(*) 
FROM Katilimci 
WHERE KullaniciID = @KullaniciID";

                int toplamKatilimSayisi;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(ilkKatilimKontrolQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                        connection.Open();
                        toplamKatilimSayisi = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                // İlk etkinliğe katılım mı, yoksa sonraki mi kontrol et
                int eklenecekPuan = (toplamKatilimSayisi == 1) ? 20 : 10;

                // Puan ekleme işlemi
                string puanEkleQuery = @"
INSERT INTO Puan (KullaniciID, Puan, KazanilanTarih) 
VALUES (@KullaniciID, @Puan, GETDATE())";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand puanCommand = new SqlCommand(puanEkleQuery, connection))
                    {
                        puanCommand.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                        puanCommand.Parameters.AddWithValue("@Puan", eklenecekPuan);
                        connection.Open();
                        puanCommand.ExecuteNonQuery();
                    }
                }

                TempData["KatılımMesajı"] = "Etkinliğe katılımınız sağlandı!";
                return RedirectToAction("EtkinlikDetay", new { etkinlikID });
            }
            catch (Exception ex)
            {
                TempData["KatılımMesajı"] = "Bir hata oluştu: " + ex.Message;
                return RedirectToAction("EtkinlikDetay", new { etkinlikID });
            }
        }





        [HttpPost]
        public IActionResult Ekle(string etkinlikAdi, string aciklama, DateTime tarih, TimeSpan saat, int etkinlikSuresi, string konum, string kategori)
        {
            // Kullanıcı ID'sini session'dan al
            string kullaniciID = HttpContext.Session.GetString("KullaniciID");

            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
            string query = "INSERT INTO Etkinlik (EtkinlikAdi, Aciklama, Tarih, Saat, EtkinlikSuresi, Konum, Kategori, KullaniciID) " +
                           "VALUES (@EtkinlikAdi, @Aciklama, @Tarih, @Saat, @EtkinlikSuresi, @Konum, @Kategori, @KullaniciID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikAdi", etkinlikAdi);
                    command.Parameters.AddWithValue("@Aciklama", aciklama);
                    command.Parameters.AddWithValue("@Tarih", tarih);
                    command.Parameters.AddWithValue("@Saat", saat);
                    command.Parameters.AddWithValue("@EtkinlikSuresi", etkinlikSuresi);
                    command.Parameters.AddWithValue("@Konum", konum);
                    command.Parameters.AddWithValue("@Kategori", kategori);
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Kullanıcı etkinlik oluşturduğunda 15 puan ekleyin
            string etkinlikOlusturPuanQuery = @"
    INSERT INTO Puan (KullaniciID, Puan, KazanilanTarih) 
    VALUES (@KullaniciID, 15, GETDATE())"; // 15 puan etkinlik oluşturma için

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(etkinlikOlusturPuanQuery, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }


            return RedirectToAction("Main");
        }

        //[HttpGet]
        //public IActionResult GuncelleView()
        //{
        //    return View();
        //}


        [HttpGet]
        public IActionResult Guncelle(int etkinlikID)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";

            Etkinlik etkinlik = null;
            string query = @"
        SELECT * FROM Etkinlik 
        WHERE EtkinlikID = @EtkinlikID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        etkinlik = new Etkinlik
                        {
                            EtkinlikID = Convert.ToInt32(reader["EtkinlikID"]),
                            EtkinlikAdi = reader["EtkinlikAdi"].ToString(),
                            Aciklama = reader["Aciklama"].ToString(),
                            Tarih = Convert.ToDateTime(reader["Tarih"]),
                            Saat = (TimeSpan)reader["Saat"],
                            EtkinlikSuresi = Convert.ToInt32(reader["EtkinlikSuresi"]),
                            Konum = reader["Konum"].ToString(),
                            Kategori = reader["Kategori"].ToString(),
                            KullaniciID = reader["KullaniciID"] != DBNull.Value ? Convert.ToInt32(reader["KullaniciID"]) : 0 // Null kontrolü ekledik

                        };
                    }
                }
            }

            if (etkinlik == null)
            {
                return NotFound();
            }

            // Kullanıcının yetkisini kontrol et
            string kullaniciID = HttpContext.Session.GetString("KullaniciID");
            if (etkinlik.KullaniciID != Convert.ToInt32(kullaniciID)) // Etkinliği ekleyen kişiyle aynı değilse
            {
                TempData["Hata"] = "Buna yetkiniz yok."; // Yetki hatası mesajı
                return RedirectToAction("Main"); // Main sayfasına yönlendir
            }

            return View(etkinlik); // Yetkiliyse güncelleme sayfasını göster
        }


        [HttpPost]
        public IActionResult Guncelle(int etkinlikID, string etkinlikAdi, string aciklama, DateTime tarih, TimeSpan saat, int etkinlikSuresi, string konum, string kategori)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
            string kullaniciID = HttpContext.Session.GetString("KullaniciID");

            string query = @"
        UPDATE Etkinlik 
        SET EtkinlikAdi = @EtkinlikAdi, 
            Aciklama = @Aciklama, 
            Tarih = @Tarih, 
            Saat = @Saat, 
            EtkinlikSuresi = @EtkinlikSuresi, 
            Konum = @Konum, 
            Kategori = @Kategori
        WHERE EtkinlikID = @EtkinlikID AND KullaniciID = @KullaniciID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    command.Parameters.AddWithValue("@EtkinlikAdi", etkinlikAdi);
                    command.Parameters.AddWithValue("@Aciklama", aciklama);
                    command.Parameters.AddWithValue("@Tarih", tarih);
                    command.Parameters.AddWithValue("@Saat", saat);
                    command.Parameters.AddWithValue("@EtkinlikSuresi", etkinlikSuresi);
                    command.Parameters.AddWithValue("@Konum", konum);
                    command.Parameters.AddWithValue("@Kategori", kategori);
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                    connection.Open();
                    int etkilenenSatirSayisi = command.ExecuteNonQuery();

                    if (etkilenenSatirSayisi > 0)
                    {
                        TempData["Basari"] = "Etkinlik başarıyla güncellendi.";
                    }
                    else
                    {
                        TempData["Hata"] = "Güncelleme işlemi sırasında bir hata oluştu.";
                    }
                }
            }

            return RedirectToAction("Main");
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Sil(int etkinlikID)
        {
            string connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";

            string kullaniciID = HttpContext.Session.GetString("KullaniciID");

            // Etkinliği ekleyen kişinin ID'sini almak
            string etkinlikSahibiID = null;
            string etkinlikQuery = @"
        SELECT KullaniciID FROM Etkinlik 
        WHERE EtkinlikID = @EtkinlikID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(etkinlikQuery, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        etkinlikSahibiID = reader["KullaniciID"].ToString();
                    }
                }
            }

            // Etkinliği ekleyen kişi ile şu anki kullanıcıyı karşılaştır
            if (etkinlikSahibiID != kullaniciID)
            {
                TempData["Hata"] = "Buna yetkiniz yok."; // Yetki hatası mesajı
                return RedirectToAction("Main"); // Main sayfasına yönlendir
            }

            // Katilimci tablosundaki ilişkili kayıtları sil
            string katilimciSilQuery = @"
        DELETE FROM Katilimci 
        WHERE EtkinlikID = @EtkinlikID";

            // Etkinlik kaydını sil
            string etkinlikSilQuery = @"
        DELETE FROM Etkinlik 
        WHERE EtkinlikID = @EtkinlikID AND KullaniciID = @KullaniciID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(katilimciSilQuery, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(etkinlikSilQuery, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikID);
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                    int etkilenenSatirSayisi = command.ExecuteNonQuery();

                    if (etkilenenSatirSayisi > 0)
                    {
                        TempData["Basari"] = "Etkinlik başarıyla silindi.";
                    }
                    else
                    {
                        TempData["Hata"] = "Silme işlemi sırasında bir hata oluştu.";
                    }
                }
            }

            return RedirectToAction("Main");
        }

    }}




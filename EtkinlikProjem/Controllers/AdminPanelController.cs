using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using EtkinlikProjem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting.Server;
namespace EtkinlikProjem.Controllers
{
    // AdminController ismi ve URL'ye uygun şekilde yönlendirme yapılmalı
    public class AdminPanelController : Controller
    {
        private readonly string _connectionString = "Server=your_server;Database=your_db_name;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly HttpClient _httpClient;

        private readonly ILogger<AdminPanelController> _logger;

        public AdminPanelController(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AdminPanelController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourEventApp");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Main()
        {
            bool x = true;
            IActionResult actionResult = Main1(x);
            return View();
        }

        public IActionResult Main1(bool x)
        {
            bool x_ = x;
            int totalUsers = 0;
            int totalEvents = 0;
            int approvedEvents = 0;
            int totalParticipants = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string userQuery = "SELECT COUNT(*) FROM Kullanici";
                using (SqlCommand command = new SqlCommand(userQuery, connection))
                {
                    totalUsers = (int)command.ExecuteScalar();
                }

                string eventQuery = "SELECT COUNT(*) FROM Etkinlik";
                using (SqlCommand command = new SqlCommand(eventQuery, connection))
                {
                    totalEvents = (int)command.ExecuteScalar();
                }

                string approvedEventQuery = "SELECT COUNT(*) FROM Etkinlik WHERE Onay = 1";
                using (SqlCommand command = new SqlCommand(approvedEventQuery, connection))
                {
                    approvedEvents = (int)command.ExecuteScalar();
                }

                string participantQuery = "SELECT COUNT(*) FROM Katilimci";
                using (SqlCommand command = new SqlCommand(participantQuery, connection))
                {
                    totalParticipants = (int)command.ExecuteScalar();
                }
            }

            ViewBag.Stats = new
            {
                TotalUsers = totalUsers,
                TotalEvents = totalEvents,
                ApprovedEvents = approvedEvents,
                TotalParticipants = totalParticipants
            };

            return View(); // Herhangi bir model belirtilmezse, default olarak Main view'ını arayacak
        }
        // Ana Panel (Dashboard)
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
                            string rol = reader.GetString(3); // Rol bilgisi (4. sütun)

                            // Rol kontrolü
                            if (rol != "Admin")
                            {
                                ViewBag.Hata = "Yalnızca admin kullanıcılar giriş yapabilir!";
                                return View("Index");
                            }

                            string hashedPassword = reader.GetString(4); // Hashlenmiş şifre

                            // Şifre doğrulama
                            if (BCrypt.Net.BCrypt.Verify(sifre, hashedPassword))
                            {
                                // Giriş başarılı
                                HttpContext.Session.SetString("KullaniciID", reader["KullaniciID"].ToString());
                                HttpContext.Session.SetString("Rol", rol); // Rol bilgisini saklamak isteyebilirsiniz
                                return RedirectToAction("Main");
                            }
                            else
                            {
                                // Şifre hatalı
                                ViewBag.Hata = "Şifre hatalı!";
                                return View("Index");
                            }
                        }
                    }
                    else
                    {
                        // Kullanıcı bulunamadı
                        ViewBag.Hata = "Kullanıcı adı bulunamadı!";
                        return View("Index");
                    }
                }
            }

            // Hiçbir koşul sağlanmazsa login ekranına dön
            ViewBag.Hata = "Bir hata oluştu. Lütfen tekrar deneyin.";
            return View("Index");
        }

        // Kullanıcı Yönetimi
        public IActionResult KullaniciYonetim()
        {
            bool x = true;
            IActionResult actionResult = KullaniciYonetim1(x);
            return View();
        }
        public IActionResult KullaniciYonetim1(bool x)
        {
            var users = new List<dynamic>();

            string query = "SELECT KullaniciID, KullaniciAdi, Ad, Soyad, Email, Rol FROM Kullanici";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new
                        {
                            KullaniciID = reader["KullaniciID"],
                            KullaniciAdi = reader["KullaniciAdi"],
                            Ad = reader["Ad"],
                            Soyad = reader["Soyad"],
                            Email = reader["Email"],
                            Rol = reader["Rol"]
                        });
                    }
                }
            }

            return View(users); // Kullanıcı listesi view'ine yönlendir
        }
        public IActionResult KullaniciEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddUser(string kullaniciAdi, string sifre, string ad, string soyad, DateTime dogumTarihi,
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
            return RedirectToAction("Main");
        }

        public IActionResult DeleteUser(int id)
        {
            string query = "DELETE FROM Kullanici WHERE KullaniciID = @KullaniciID";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", id);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Silme başarılı
                        return RedirectToAction("KullaniciYonetim"); // Kullanıcı listesine geri dön
                    }
                }
            }
            // Silme başarısız
            return RedirectToAction("KullaniciYonetim");
        }
        public IActionResult EditUser()
        {
            return View();
        }
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            // Tek bir kullanıcının bilgilerini çekmek için
            dynamic user = null;
            string query = "SELECT KullaniciID, KullaniciAdi, Ad, Soyad, Email, Rol FROM Kullanici WHERE KullaniciID = @KullaniciID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", id);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new
                        {
                            KullaniciID = reader["KullaniciID"],
                            KullaniciAdi = reader["KullaniciAdi"],
                            Ad = reader["Ad"],
                            Soyad = reader["Soyad"],
                            Email = reader["Email"],
                            Rol = reader["Rol"]
                        };
                    }
                }
            }

            return View(user);
        }

        // Kullanıcı bilgilerini güncellemek için POST metodu
        [HttpPost]
        public IActionResult EditUser(int KullaniciID, string KullaniciAdi, string Ad, string Soyad, string Email, string Rol)
        {
            string query = @"UPDATE Kullanici 
                     SET KullaniciAdi = @KullaniciAdi, 
                         Ad = @Ad, 
                         Soyad = @Soyad, 
                         Email = @Email, 
                         Rol = @Rol 
                     WHERE KullaniciID = @KullaniciID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", KullaniciID);
                    command.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);
                    command.Parameters.AddWithValue("@Ad", Ad);
                    command.Parameters.AddWithValue("@Soyad", Soyad);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Rol", Rol);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Başarılı güncelleme
                        return RedirectToAction("KullaniciYonetim");
                    }
                }
            }

            // Güncelleme başarısız
            return View();
        }
        public IActionResult EtkinlikYonetim()
        {
            EventManagement();
            return View();
        }
        // Etkinlik Yönetimi
        [HttpGet]
        public IActionResult EventManagement()
        {
            var events = new List<EtkinlikViewModel>();
            try
            {
                string query = @"SELECT 
            EtkinlikID, 
            ISNULL(EtkinlikAdi, '') AS EtkinlikAdi, 
            ISNULL(Aciklama, '') AS Aciklama, 
            Tarih, -- DateTime olarak geliyor
            Saat, -- Time olarak geliyor
            EtkinlikSuresi, -- int olarak geliyor
            ISNULL(Konum, '') AS Konum, 
            ISNULL(Kategori, '') AS Kategori, 
            Onay 
        FROM [Etkinlik].[dbo].[Etkinlik]
        ORDER BY EtkinlikID DESC";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    events.Add(new EtkinlikViewModel
                                    {
                                        EtkinlikID = reader.GetInt32(reader.GetOrdinal("EtkinlikID")),
                                        EtkinlikAdi = reader.GetString(reader.GetOrdinal("EtkinlikAdi")),
                                        Aciklama = reader.GetString(reader.GetOrdinal("Aciklama")),
                                        Tarih = reader.GetDateTime(reader.GetOrdinal("Tarih")), // Tarih DateTime
                                        Saat = reader.GetTimeSpan(reader.GetOrdinal("Saat")), // Saat TimeSpan
                                        EtkinlikSuresi = reader.GetInt32(reader.GetOrdinal("EtkinlikSuresi")), // Süre int
                                        Konum = reader.GetString(reader.GetOrdinal("Konum")),
                                        Kategori = reader.GetString(reader.GetOrdinal("Kategori")),
                                        Onay = reader.GetBoolean(reader.GetOrdinal("Onay"))
                                    });
                                }
                                catch (Exception ex)
                                {
                                    // Tek bir satır hatası varsa logla ama devam et
                                    System.Diagnostics.Debug.WriteLine($"Satır Okuma Hatası: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                // Hata ayıklama için konsola yazdır
                System.Diagnostics.Debug.WriteLine($"Toplam Etkinlik Sayısı: {events.Count}");

                return View(events);
            }
            catch (Exception ex)
            {
                // Detaylı hata bilgisi
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Hata Yeri: {ex.StackTrace}");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult ApproveEvent(int etkinlikId)
        {
            string query = "UPDATE [Etkinlik].[dbo].[Etkinlik] SET Onay = 1 WHERE EtkinlikID = @EtkinlikID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("EtkinlikYonetim");
                    }
                }
            }

            return RedirectToAction("EtkinlikYonetim");
        }

        [HttpPost]
        public IActionResult CreateEvent(EtkinlikViewModel model)
        {
            try
            {
                // Saat'i string olarak alıp TimeSpan'a dönüştür
                TimeSpan parsedSaat = TimeSpan.ParseExact(model.Saat.ToString(), @"hh\:mm\:ss", null);

                string query = @"INSERT INTO [Etkinlik].[dbo].[Etkinlik] 
                         (EtkinlikAdi, Aciklama, Tarih, Saat, Kategori, EtkinlikSuresi, Konum, Onay) 
                         VALUES (@EtkinlikAdi, @Aciklama, @Tarih, @Saat, @Kategori, @EtkinlikSuresi, @Konum, 1)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikAdi", model.EtkinlikAdi);
                        command.Parameters.AddWithValue("@Aciklama", model.Aciklama);
                        command.Parameters.AddWithValue("@Tarih", model.Tarih);
                        command.Parameters.AddWithValue("@Saat", parsedSaat);
                        command.Parameters.AddWithValue("@Kategori", model.Kategori);
                        command.Parameters.AddWithValue("@EtkinlikSuresi", model.EtkinlikSuresi);
                        command.Parameters.AddWithValue("@Konum", model.Konum);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // İşlem başarılıysa EtkinlikYonetim sayfasına yönlendir
                return RedirectToAction("EtkinlikYonetim");
            }
            catch (Exception ex)
            {
                // Hata durumu: Hata mesajını logla ve Error sayfasına yönlendir
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult EditEvent(int etkinlikId)
        {
            string query = @"SELECT EtkinlikID, EtkinlikAdi, Aciklama, Tarih, Saat, Kategori, EtkinlikSuresi, Konum 
                     FROM [Etkinlik].[dbo].[Etkinlik] 
                     WHERE EtkinlikID = @EtkinlikID";

            EtkinlikViewModel etkinlik = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            etkinlik = new EtkinlikViewModel
                            {
                                EtkinlikID = reader.GetInt32(reader.GetOrdinal("EtkinlikID")),
                                EtkinlikAdi = reader.GetString(reader.GetOrdinal("EtkinlikAdi")),
                                Aciklama = reader.GetString(reader.GetOrdinal("Aciklama")),
                                Tarih = reader.GetDateTime(reader.GetOrdinal("Tarih")),
                                Saat = reader.GetTimeSpan(reader.GetOrdinal("Saat")),
                                Kategori = reader.GetString(reader.GetOrdinal("Kategori")),
                                EtkinlikSuresi = reader.GetInt32(reader.GetOrdinal("EtkinlikSuresi")),
                                Konum = reader.GetString(reader.GetOrdinal("Konum"))
                            };
                        }
                    }
                }
            }

            if (etkinlik == null)
            {
                return NotFound(); // ID geçerli değilse 404 döner
            }

            return View(etkinlik); // View'e model gönderilir
        }

        [HttpPost]
        public IActionResult EditEvent(EtkinlikViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string query = @"UPDATE [Etkinlik].[dbo].[Etkinlik] 
                             SET EtkinlikAdi = @EtkinlikAdi, Aciklama = @Aciklama, 
                                 Tarih = @Tarih, Saat = @Saat, Kategori = @Kategori, 
                                 EtkinlikSuresi = @EtkinlikSuresi, Konum = @Konum 
                             WHERE EtkinlikID = @EtkinlikID";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EtkinlikAdi", model.EtkinlikAdi);
                            command.Parameters.AddWithValue("@Aciklama", model.Aciklama);
                            command.Parameters.AddWithValue("@Tarih", model.Tarih);
                            command.Parameters.AddWithValue("@Saat", TimeSpan.Parse(model.Saat.ToString()));
                            command.Parameters.AddWithValue("@Kategori", model.Kategori);
                            command.Parameters.AddWithValue("@EtkinlikSuresi", model.EtkinlikSuresi);
                            command.Parameters.AddWithValue("@Konum", model.Konum);
                            command.Parameters.AddWithValue("@EtkinlikID", model.EtkinlikID);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("EtkinlikYonetim");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                    return View("Error");
                }
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult DeleteEvent(int etkinlikId)
        {
            try
            {
                string query = @"DELETE FROM [Etkinlik].[dbo].[Etkinlik] WHERE EtkinlikID = @EtkinlikID";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("EtkinlikYonetim");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                return View("Error");
            }
        }
        public IActionResult EventDetails(int id)
        {
            var katilimciListesi = GetKatilimciListesi(id);
            ViewBag.EtkinlikID = id; // Add this line to pass the event ID
            return View(katilimciListesi);
        }

        public List<KatilimciViewModel> GetKatilimciListesi(int etkinlikId)
        {
            var katilimciListesi = new List<KatilimciViewModel>();
            try
            {
                string query = @"
        SELECT 
            k.KullaniciID, 
            ku.KullaniciAdi, 
            ku.Ad, 
            ku.Soyad,
            e.EtkinlikID,
            e.EtkinlikAdi
        FROM [Etkinlik].[dbo].[Katilimci] k
        INNER JOIN [Etkinlik].[dbo].[Kullanici] ku ON k.KullaniciID = ku.KullaniciID
        INNER JOIN [Etkinlik].[dbo].[Etkinlik] e ON k.EtkinlikID = e.EtkinlikID
        WHERE k.EtkinlikID = @EtkinlikID
        ORDER BY ku.Ad, ku.Soyad";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                katilimciListesi.Add(new KatilimciViewModel
                                {
                                    KullaniciID = reader.GetInt32(reader.GetOrdinal("KullaniciID")),
                                    KullaniciAdi = reader.GetString(reader.GetOrdinal("KullaniciAdi")),
                                    Ad = reader.GetString(reader.GetOrdinal("Ad")),
                                    Soyad = reader.GetString(reader.GetOrdinal("Soyad")),
                                    EtkinlikID = reader.GetInt32(reader.GetOrdinal("EtkinlikID")),
                                    EtkinlikAdi = reader.GetString(reader.GetOrdinal("EtkinlikAdi"))
                                });
                            }
                        }
                    }
                }
                return katilimciListesi;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                return new List<KatilimciViewModel>();
            }
        }
        [HttpPost]
        public IActionResult EtkinliktenCikar(int kullaniciId, int etkinlikId)
        {
            try
            {
                string deleteQuery = @"
        DELETE FROM [Etkinlik].[dbo].[Katilimci] 
        WHERE KullaniciID = @KullaniciID AND EtkinlikID = @EtkinlikID";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciId);
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);

                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows > 0)
                        {
                            // Silme başarılı
                            TempData["SuccessMessage"] = "Kullanıcı etkinlikten başarıyla çıkarıldı.";
                        }
                        else
                        {
                            // Silme başarısız
                            TempData["ErrorMessage"] = "Kullanıcı etkinlikten çıkarılamadı.";
                        }
                    }
                }

                // Başarılı bir işlemden sonra EventDetails kısmına yönlendir
                return RedirectToAction("EventDetails", "AdminPanel", new { id = etkinlikId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
                TempData["ErrorMessage"] = "Bir hata oluştu.";
                return RedirectToAction("EventDetails", "AdminPanel", new { id = etkinlikId });
            }
        }
        public IActionResult AddParticipant(int etkinlikId)
        {
            // Get all users not already registered for this event
            var availableUsers = GetAvailableUsers(etkinlikId);

            // Create a view model to pass both the event ID and available users
            var viewModel = new AddParticipantViewModel
            {
                EtkinlikID = etkinlikId,
                AvailableUsers = availableUsers
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddParticipant(int etkinlikId, int kullaniciId)
        {
            try
            {
                // SQL query to insert new participant
                string query = @"
        INSERT INTO [Etkinlik].[dbo].[Katilimci] (EtkinlikID, KullaniciID)
        VALUES (@EtkinlikID, @KullaniciID)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Katılımcı başarıyla eklendi.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Katılımcı eklenemedi.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
            }

            return RedirectToAction("EventDetails", new { id = etkinlikId });
        }

        private List<Kullanici> GetAvailableUsers(int etkinlikId)
        {
            var availableUsers = new List<Kullanici>();
            try
            {
                string query = @"
        SELECT ku.KullaniciID, ku.KullaniciAdi, ku.Ad, ku.Soyad
        FROM [Etkinlik].[dbo].[Kullanici] ku
        WHERE ku.KullaniciID NOT IN (
            SELECT KullaniciID 
            FROM [Etkinlik].[dbo].[Katilimci] 
            WHERE EtkinlikID = @EtkinlikID
        )
        ORDER BY ku.Ad, ku.Soyad";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EtkinlikID", etkinlikId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                availableUsers.Add(new Kullanici
                                {
                                    KullaniciID = reader.GetInt32(reader.GetOrdinal("KullaniciID")),
                                    KullaniciAdi = reader.GetString(reader.GetOrdinal("KullaniciAdi")),
                                    Ad = reader.GetString(reader.GetOrdinal("Ad")),
                                    Soyad = reader.GetString(reader.GetOrdinal("Soyad"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hata Detayı: {ex.Message}");
            }
            return availableUsers;
        }
        public IActionResult Harita()
        {
            bool x = true;
            Harita1(x);

            return View();
        }

        public async Task<IActionResult> Harita1(bool x)
        {
            var etkinlikler = new List<object>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        "SELECT EtkinlikID, EtkinlikAdi, Tarih, Saat, Konum FROM Etkinlikler WHERE Onay = 1",
                        connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string adres = reader["Konum"].ToString();
                                string koordinat = await AdresKoordinatlariniAl(adres);

                                etkinlikler.Add(new
                                {
                                    EtkinlikID = reader["EtkinlikID"],
                                    EtkinlikAdi = reader["EtkinlikAdi"],
                                    Tarih = ((DateTime)reader["Tarih"]).ToString("yyyy-MM-dd"),
                                    Saat = reader["Saat"].ToString(),
                                    Konum = koordinat ?? adres,
                                    Adres = adres
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Harita yükleme hatası: {ex.Message}");
                return View("Error"); // Hata sayfasına yönlendir
            }

            ViewBag.Etkinlikler = JsonConvert.SerializeObject(etkinlikler);
            return View();
        }

        // Önceki AdresKoordinatlariniAl metodu
        private async Task<string> AdresKoordinatlariniAl(string adres)
        {
            if (string.IsNullOrWhiteSpace(adres))
                return null;

            try
            {
                string encodedAdres = Uri.EscapeDataString(adres);
                string url = $"https://nominatim.openstreetmap.org/search?format=json&q={encodedAdres}";

                var response = await _httpClient.GetStringAsync(url);
                var jsonResult = JArray.Parse(response);

                if (jsonResult.Count > 0)
                {
                    var location = jsonResult[0];
                    return $"{location["lat"]},{location["lon"]}";
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the specific HTTP request error
                _logger.LogError($"HTTP Request Error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Log JSON parsing errors
                _logger.LogError($"JSON Parsing Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log any other unexpected errors
                _logger.LogError($"Unexpected Error: {ex.Message}");
            }

            return null;
        }
        // Sistem İstatistikleri (Dashboard)
        public IActionResult Dashboard()
        {
            int totalUsers = 0;
            int totalEvents = 0;
            int approvedEvents = 0;
            int totalParticipants = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string userQuery = "SELECT COUNT(*) FROM Kullanici";
                using (SqlCommand command = new SqlCommand(userQuery, connection))
                {
                    totalUsers = (int)command.ExecuteScalar();
                }

                string eventQuery = "SELECT COUNT(*) FROM Etkinlik";
                using (SqlCommand command = new SqlCommand(eventQuery, connection))
                {
                    totalEvents = (int)command.ExecuteScalar();
                }

                string approvedEventQuery = "SELECT COUNT(*) FROM Etkinlik WHERE Onay = 1";
                using (SqlCommand command = new SqlCommand(approvedEventQuery, connection))
                {
                    approvedEvents = (int)command.ExecuteScalar();
                }

                string participantQuery = "SELECT COUNT(*) FROM Katilimci";
                using (SqlCommand command = new SqlCommand(participantQuery, connection))
                {
                    totalParticipants = (int)command.ExecuteScalar();
                }
            }

            var stats = new
            {
                TotalUsers = totalUsers,
                TotalEvents = totalEvents,
                ApprovedEvents = approvedEvents,
                TotalParticipants = totalParticipants
            };

            return View(stats);
        }
    }
}
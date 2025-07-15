using Microsoft.AspNetCore.Mvc;


namespace EtkinlikApp.Controllers
{
    public class HomeController : Controller
    {
        // Ana sayfa
        public ActionResult Index()
        {
            return View();
        }
        


        // Kullanýcý Giriþi
        public ActionResult KullaniciGiris()
        {
            return RedirectToAction("Index", "KullaniciPanel"); // Kullanýcý Paneline yönlendirme
        }

        // Admin Giriþi
        public ActionResult AdminGiris()
        {
            return RedirectToAction("Index", "AdminPanel"); // Admin Paneline yönlendirme
        }
    }
}

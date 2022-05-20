using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using YesilayKlubu.Models;

namespace YesilayKlubu.Controllers
{
    public class HesapController : Controller
    {
        //Oturum işlerini yapmamızı sağlar
        private SignInManager<AppUser> signInManager;
        //Kullanıcı işlemlerini yapmamızı sağlar
        private UserManager<AppUser> userManager;
        public HesapController(SignInManager<AppUser> _s, UserManager<AppUser> _u)
        {
            signInManager = _s;
            userManager = _u;
        }
        public IActionResult Kayit()
        {
            // TODO: Your code here
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Kayit(Kayit m)
        {
            if (ModelState.IsValid)
            {
                var kullanici = new AppUser
                {
                    UserName = m.Email,
                    Ad = m.Ad,
                    Soyad = m.Soyad,
                    Email = m.Email,
                    OkulNumarasi = m.OkulNumarasi
                };
                var sonuc = await userManager.CreateAsync(kullanici, m.Sifre);
                if (sonuc.Succeeded)
                {
                    ViewBag.OK = "Üye kaydınız başarıyla gerçekleşti, Lütfen giriş yapınız";
                    string icerik = $"Sayın {m.Ad} {m.Soyad}  <br> Sitemize kayıt olduğunuz için teşekkür ederiz ";
                    icerik += $" Giriş yapmak için {this.Request.Scheme}://{this.Request.Host}/Hesap/";
                    Islemler.MailGonder("Hoşgeldin", icerik, m.Email);
                }
                else
                {
                    //kayıt hata ile sonuçlanırsa ön tarafa 
                    //göstermesi için hata ekle
                    ModelState.AddModelError("", string.Join("<br>",
                    sonuc
                    .Errors
                    .Select(x => x.Description)
                    .ToList()));
                }
            }
            return View(m);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(giris m)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(m.Email, m.Sifre, true, lockoutOnFailure: true);
                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Hesap kilitli");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Giriş başarısız");
                    }

                }
            }
            return View();
        }

        public IActionResult Cikis()
        {
            signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
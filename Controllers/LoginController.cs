using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DonatingFundsClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DonatingFundsClient.Controllers
{
    public class LoginController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DonarController));

        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {

            _log4net.Info("User Login");
            User Item = new User();
            using (var httpClient = new HttpClient())
            {
              
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                //375
                //https ://localhost:44375/api/auth/User
                var response = await httpClient.PostAsync("https://851321staticauthservice.azurewebsites.net/api/auth/User", content);

                string apiResponse = await response.Content.ReadAsStringAsync();
                Item = JsonConvert.DeserializeObject<User>(apiResponse);
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response1 = await httpClient.PostAsync("https://851321staticauthservice.azurewebsites.net/api/auth/Login", content1))
                {
                    if (!response1.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }

                    string apiResponse1 = await response1.Content.ReadAsStringAsync();



                    string stringJWT = response1.Content.ReadAsStringAsync().Result;


                    Jwt jwt = JsonConvert.DeserializeObject<Jwt>(stringJWT);

                    HttpContext.Session.SetString("token", jwt.Token);
                    HttpContext.Session.SetString("user", JsonConvert.SerializeObject(Item));
                    HttpContext.Session.SetInt32("Userid", Item.Userid);
                    HttpContext.Session.SetString("Username", Item.Username);
                    ViewBag.Message = "User logged in successfully!";

                    return RedirectToAction("Index", "Organization");


                }
            }
        }
        public ActionResult Logout()
        {
            _log4net.Info("User Log Out");
            HttpContext.Session.Remove("token");
            // HttpContext.Session.SetString("user", null);

            return View("Login");
        }


    }
}

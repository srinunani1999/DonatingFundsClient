using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DonatingFundsClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DonatingFundsClient.Controllers
{
    public class DonarController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DonarController));

        // GET: DonarController
        public async Task<ActionResult> Index(int id)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                _log4net.Error("Session Expired while acccessing details");

                return RedirectToAction("Login", "Login");

            }
            List<Donar> donars = new List<Donar>();
            using (var client = new HttpClient())
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                //378/api/donars https ://localhost:44395/api/donar
                using (var response = await client.GetAsync("https://851321donarservice.azurewebsites.net/api/donars"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    donars = JsonConvert.DeserializeObject<List<Donar>>(apiResponse);
                }

            }
            List<Donar> donarsList = new List<Donar>();
            foreach (var item in donars)
            {
                if (item.organization_Id == id)
                {
                    // sum += item.Amount;
                    donarsList.Add(item);
                }
            }
            return View(donarsList);
        }

        // GET: DonarController/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                 _log4net.Error("Session Expired while acccessing details");

                return RedirectToAction("Login", "Login");

            }
            _log4net.Info("donar details of the organization with " + id);
            List<Donar> donars = new List<Donar>();
            using (var client = new HttpClient())
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                //395
                using (var response = await client.GetAsync("https://localhost:44395/api/donar"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    donars = JsonConvert.DeserializeObject<List<Donar>>(apiResponse);
                }

            }
            List<Donar> donarsList = new List<Donar>();
            foreach (var item in donars)
            {
                if (item.organization_Id == id)
                {
                    // sum += item.Amount;
                    donarsList.Add(item);
                }
            }
            return View(donarsList);
        }

      
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id, Donar donar)
        {


            if (HttpContext.Session.GetString("token") == null)
            {
                 _log4net.Error("Session Expired while creating donar");

                return RedirectToAction("Login", "Login");

            }
            _log4net.Info("http post  request initiated by donar " + donar.DonorName);

            //Code for Donation
            donar.organization_Id = id;
            using (var httpclinet = new HttpClient())
            {
                //395
                StringContent content = new StringContent(JsonConvert.SerializeObject(donar), Encoding.UTF8, "application/json");
                using (var response = await httpclinet.PostAsync("https://851321donarservice.azurewebsites.net/api/donars", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                   // donar = JsonConvert.DeserializeObject<Donar>(apiResponse);

                }
            }

            //Updating Total Funds Donated To Organization

            Organization org = new Organization();
            using (var client = new HttpClient())
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                //356 https ://localhost:44353/api/organization/
                using (var response = await client.GetAsync("https://851321orgservice.azurewebsites.net/api/organization/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    org = JsonConvert.DeserializeObject<Organization>(apiResponse);
                }
                double donations = Convert.ToInt32(org.TotalDonations);
                org.TotalDonations = (donar.Amount + donations).ToString();
                StringContent content = new StringContent(JsonConvert.SerializeObject(org), Encoding.UTF8, "application/json");
                //https ://localhost:44353/api/organization
                using (var response = await client.PutAsync("https://851321orgservice.azurewebsites.net/api/organization", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();


                }

            }



            return RedirectToAction("Index", "Organization");
        }


    }
}

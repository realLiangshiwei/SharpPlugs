using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpPlug.Geetest;
using SharpPlugGetest.Mvc.Models;

namespace SharpPlug.Getest.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeetestManager _geetestManager;

        public HomeController(GeetestManager geetestManager)
        {
            _geetestManager = geetestManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<GeeTestRegisterResult> GeetestRegister()
        {
            return await _geetestManager.Register(clientType: "web", ipAddress: "127.0.0.1");
        }

        public async Task<bool> GeetestValidate(GeetestValidateInput input)
        {
            return await _geetestManager.Validate(input);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

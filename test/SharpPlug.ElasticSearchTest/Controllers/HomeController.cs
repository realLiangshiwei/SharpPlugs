using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SharpPlug.ElasticSearch;
using SharpPlug.ElasticSearchTest.Models;
using X.PagedList;

namespace SharpPlug.ElasticSearchTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISharpElasticsearch _elasticSearch;

        public HomeController(ISharpElasticsearch elasticSearch)
        {
            _elasticSearch = elasticSearch;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index(int? page)
        {
            await _elasticSearch.CreateIndexAsync<BlogEsModel>("blog");

            var p = page ?? 1;
            var list = await _elasticSearch.SearchAsync("blog", new SearchDescriptor<BlogEsModel>(), (p - 1) * 10, 10);

            return View(new StaticPagedList<BlogEsModel>(list.Documents, p, 10, Convert.ToInt32(list.Total)));
        }

        public IActionResult AddBlogView()
        {
            return View("AddBlog");
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> AddBlog(BlogEsModel model)
        {
            await _elasticSearch.AddOrUpdateAsync("blog", model);
            return RedirectToAction("index", 1);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ReBuild()
        {
            await _elasticSearch.ReBuild<BlogEsModel>("blog");
            return RedirectToAction("index", 1);
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

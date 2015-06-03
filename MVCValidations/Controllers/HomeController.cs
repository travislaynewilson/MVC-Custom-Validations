using System.Web.Mvc;
using MVCValidations.ViewModels;

namespace MVCValidations.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Your contact page.";

            var viewModel = new FormViewModel()
            {
                NameRule = 1,
                TaxIDType = 1
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(FormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return View(viewModel);
            }
            return View(viewModel);
        }
    }
}
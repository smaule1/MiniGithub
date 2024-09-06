using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniGithub.Data;
using MiniGithub.Models;

namespace MiniGithub.Controllers
{
    public class RepositorioController : Controller
    {
        //La mayoria de estos metodos no estan implementados
        //todo este codigo es generado


        public readonly RepositorioDBContext repositorioDB;

        public RepositorioController(RepositorioDBContext repositorioDB)
        {
            this.repositorioDB = repositorioDB;
        }

        // GET: RepositorioController
        public ActionResult Index()
        {
            return View(repositorioDB.getRepositorios());
        }

        // GET: RepositorioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RepositorioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RepositorioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Repositorio repositorio)
        {
            repositorioDB.Create(repositorio);
            return RedirectToAction("Index");
        }

        // GET: RepositorioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RepositorioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RepositorioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RepositorioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

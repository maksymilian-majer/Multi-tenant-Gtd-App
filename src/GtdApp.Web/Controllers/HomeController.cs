namespace GtdApp.Web.Controllers
{
    using System;

    using GtdApp.Entities;

    using System.Linq;
    using System.Web.Mvc;

    using GtdApp.Web.Infrastructure;

    public class HomeController : GtdAppControllerBase
    {
        public HomeController(GtdAppDataContext context)
            : base(context)
        {
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the Multi-Tenant GTD Application";

            var model = this.DataContext.Todos.Include("Context").ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.Contexts =
                this.DataContext.Contexts.ToList().Select(
                    x => new SelectListItem { Selected = false, Text = x.Name, Value = x.ContextId.ToString() }).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Todo model)
        {
            if (ModelState.IsValid)
            {
                model.TodoId = Guid.NewGuid();
                this.DataContext.Todos.AddObject(model);
                return RedirectToAction("Index");
            }

            ViewBag.Contexts =
                this.DataContext.Contexts.ToList().Select(
                    x => new SelectListItem { Selected = x.ContextId == model.ContextId, Text = x.Name, Value = x.ContextId.ToString() })
                    .ToList();

            return View();
        }

        public ActionResult Edit(Guid id)
        {
            var model = this.DataContext.Todos.Single(x => x.TodoId == id);

            ViewBag.Contexts =
                this.DataContext.Contexts.ToList().Select(
                    x => new SelectListItem { Selected = x.ContextId == model.ContextId, Text = x.Name, Value = x.ContextId.ToString() })
                    .ToList();

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, Todo model)
        {
            if (ModelState.IsValid)
            {
                this.DataContext.Todos.Single(x => x.TodoId == id);
                this.DataContext.Todos.ApplyCurrentValues(model);
                return RedirectToAction("Index");
            }

            ViewBag.Contexts =
                this.DataContext.Contexts.ToList().Select(
                    x => new SelectListItem { Selected = x.ContextId == model.ContextId, Text = x.Name, Value = x.ContextId.ToString() })
                    .ToList();

            return this.View(model);
        }

        public ActionResult Details(Guid id)
        {
            var model = this.DataContext.Todos.Include("Context").Single(x => x.TodoId == id);
            return this.View(model);
        }

        public ActionResult Delete(Guid id)
        {
            var model = this.DataContext.Todos.Include("Context").Single(x => x.TodoId == id);

            return this.View(model);
        }
        
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection formCollection)
        {
            var entity = this.DataContext.Todos.Single(x => x.TodoId == id);
            this.DataContext.Todos.DeleteObject(entity);

            return RedirectToAction("Index");
        }
    }
}

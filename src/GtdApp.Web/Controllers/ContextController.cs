namespace GtdApp.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using GtdApp.Entities;
    using GtdApp.Web.Infrastructure;

    public class ContextController : GtdAppControllerBase
    {

        public ContextController(GtdAppDataContext dataContext)
            : base(dataContext)
        {
        }

        public ActionResult Index()
        {
            var model = this.DataContext.Contexts.ToList();
            
            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var model = this.DataContext.Contexts.Single(x => x.ContextId == id);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Context model)
        {
            if (ModelState.IsValid)
            {
                model.ContextId = Guid.NewGuid();
                this.DataContext.AddToContexts(model);
                return RedirectToAction("Index");
            }
            return View();
        }
        
        public ActionResult Edit(Guid id)
        {
            var model = this.DataContext.Contexts.Single(x => x.ContextId == id);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, Context model)
        {
            if (ModelState.IsValid)
            {
                this.DataContext.Contexts.Single(x => x.ContextId == id);
                this.DataContext.Contexts.ApplyCurrentValues(model);

                return RedirectToAction("Index");
            }

            return this.View(model);
        }

        public ActionResult Delete(Guid id)
        {
            var model = this.DataContext.Contexts.Single(x => x.ContextId == id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(Guid id, Context model)
        {
            var entity = this.DataContext.Contexts.Include("Todos").Single(x => x.ContextId == id);

            if (entity.Todos.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "Could not delete Context because there are To-Do items assigned to it.");
                return this.View(entity);
            }

            this.DataContext.Contexts.DeleteObject(entity);
            return RedirectToAction("Index");
        }
    }
}

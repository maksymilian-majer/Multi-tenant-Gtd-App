namespace GtdApp.Web.Infrastructure
{
    using System.Web.Mvc;

    using GtdApp.Entities;

    public class GtdAppControllerBase : Controller
    {
        private readonly GtdAppDataContext _dataContext;

        public GtdAppControllerBase(GtdAppDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            // save all the work done by the unit of work
            this.DataContext.SaveChanges();
        }

        protected GtdAppDataContext DataContext
        {
            get
            {
                return this._dataContext;
            }
        }
    }
}
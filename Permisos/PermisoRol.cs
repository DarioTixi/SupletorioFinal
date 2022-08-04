using FacturaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacturaWeb.Permisos
{
    public class PermisoRol: ActionFilterAttribute

    {
        Rol id;
        public PermisoRol(Rol id)
        {
            this.id = id;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var existeSession = HttpContext.Current.Session["Usuario"];

            if(existeSession !=null)
            {
                Usuario user = (Usuario)existeSession;

                if (user.idRol != this.id)
                {
                   filterContext.Result = new RedirectResult("~/Inicio/NoAuthorized");
                }

            }

            base.OnActionExecuting(filterContext);
        }

    }
}
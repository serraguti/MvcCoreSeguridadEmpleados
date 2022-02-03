using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute
        , IAuthorizationFilter
    {
        //ESTE METODO INTERCEPTA LAS PETICIONES QUE TENGAMOS
        //CUANDO DECORAMOS UN CONTROLLER O IACTIONRESULT
        //CON NUESTRO ATRIBUTO [AuthorizeEmpleados]
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //CUANDO VALIDAMOS A UN USUARIO (EN LOGIN)
            //DEBEMOS ALMACENARLO DENTRO DE LA APLICACION
            //DICHO USUARIO SE ENCUENTRA DENTRO DE 
            // context.HttpContext.User
            var user = context.HttpContext.User;
            //SI EL USUARIO NO SE HA VALIDADO, QUE HACEMOS?
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRedirectRoute("Manage", "Login");
            }
            else
            {
                //DEBEMOS PREGUNTAR POR SU OFICIO PARA SABER
                //SI TIENE PRIVILEGIOS SUFICIENTES
                if (user.IsInRole("PRESIDENTE") == false
                    && user.IsInRole("DIRECTOR") == false
                    && user.IsInRole("ANALISTA") == false)
                {
                    //DEVOLVEMOS UNA RESPUESTA
                    context.Result = this.GetRedirectRoute("Manage", "ErrorAcceso");
                }
            }
        }

        private RedirectToRouteResult GetRedirectRoute(string controller, string action)
        {
            RouteValueDictionary ruta =
                    new RouteValueDictionary(new
                    {
                        controller = controller,
                        action = action
                    });
            //CREAMOS UNA REDIRECCION
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}

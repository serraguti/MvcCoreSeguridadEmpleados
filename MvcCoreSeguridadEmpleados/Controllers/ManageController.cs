using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcCoreSeguridadEmpleados.Models;
using MvcCoreSeguridadEmpleados.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCoreSeguridadEmpleados.Controllers
{
    public class ManageController : Controller
    {
        private RepositoryEmpleados repo;

        public ManageController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>
            Login(string username, string password)
        {
            //BUSCAMOS AL EMPLEADO
            Empleado empleado = this.repo.ExisteEmpleado(username
                , int.Parse(password));
            //SI NOS DEVUELVE UN OBJETO EMPLEADO, LAS CREDENCIALES SON CORRECTAS
            if (empleado != null)
            {
                //USUARIO EXISTE
                //CUALQUIER USUARIO CLAIMS ESTA COMPUESTO POR UNA 
                //IDENTIDAD (NAME Y UN ROL)
                //Y UN PRINCIPAL
                ClaimsIdentity identity =
                    new ClaimsIdentity
                    (
                        CookieAuthenticationDefaults.AuthenticationScheme
                        , ClaimTypes.Name, ClaimTypes.Role);
                //VAMOS A ALMACENAR UN PAR DE DATOS
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier
                    , empleado.IdEmpleado.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, empleado.Apellido));
                //VAMOS A ALMACENAR TAMBIEN EL OFICIO DEL EMPLEADO PARA
                //LUEGO FILTRAR POR ROLES
                identity.AddClaim(new Claim(ClaimTypes.Role, empleado.Oficio));
                //CREAMOS EL USUARIO
                ClaimsPrincipal user = new ClaimsPrincipal(identity);
                //INTRODUCIR AL USUARIO DENTRO DEL SISTEMA
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , user, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                    });
                //DEJAMOS ENTRAR AL USUARIO EN EL PERFIL
                return RedirectToAction("PerfilEmpleado", "Empleados");
            }
            else
            {
                //MOSTRAMOS EL TIPICO MENSAJE
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}

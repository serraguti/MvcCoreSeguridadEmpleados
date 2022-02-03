using Microsoft.AspNetCore.Mvc;
using MvcCoreSeguridadEmpleados.Filters;
using MvcCoreSeguridadEmpleados.Models;
using MvcCoreSeguridadEmpleados.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCoreSeguridadEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        [AuthorizeEmpleados]
        public IActionResult PerfilEmpleado()
        {
            //SI HA ENTRADO AQUI, TENEMOS EMPLEADO
            //EXTRAEMOS EL DATO DE NameIdentifier DEL CLAIM
            string dato =
                User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int id = int.Parse(dato);
            Empleado empleado = this.repo.FindEmpleado(id);
            return View(empleado);
        }
    }
}

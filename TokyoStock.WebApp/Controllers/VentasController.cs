﻿using Microsoft.AspNetCore.Mvc;

namespace TokyoStock.WebApp.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            return View("Listar");
        }

        public IActionResult Listar()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

    }
}

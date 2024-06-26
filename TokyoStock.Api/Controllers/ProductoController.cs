﻿using Microsoft.AspNetCore.Mvc;
using TokyoStock.Core.Business;
using TokyoStock.Core.Data;

namespace TokyoStock.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoBusiness _pb;

        public ProductoController(ProductoBusiness pb)
        {
            _pb = pb;
        }

        [HttpGet("{id}/stock")]
        public IActionResult GetStock(int id)
        {
            var stock = _pb.CalculateStock(id);
            var producto = _pb.GetProducto(id);

            if (producto == null)
            {
                return Ok("Producto no encontrado");
            }

            var resultado = new
            {
                Name = producto.Nombre,
                Stock = stock
            };

            return Ok(resultado);
        }
    }
}

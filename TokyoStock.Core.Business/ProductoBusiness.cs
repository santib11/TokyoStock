﻿using TokyoStock.Core.Entities;
using TokyoStock.Core.Data;
using TokyoStock.Core.Entities.Filters;

namespace TokyoStock.Core.Business
{
    public class ProductoBusiness
    {
        private readonly ProductoRepository _pr;
        private readonly CompraBusiness _cb;
        private readonly VentaBusiness _vb;

        public ProductoBusiness(ProductoRepository pr)
        {
            _pr = pr;
        }

        public ProductoBusiness(ProductoRepository pr, CompraBusiness cb, VentaBusiness vb)
        {
            _pr = pr;
            _cb = cb;
            _vb = vb;
        }

        public List<Producto> GetProductos()
        {
            return _pr.GetProductos();
        }

        public (List<Producto> list, int total) GetProductosByFilter(Filter f)
        {
            return _pr.GetProductosByFilter(f);
        }

        public Producto GetProducto(int id)
        {
            return _pr.GetProducto(id);
        }

        public Producto GetProductoByName(string name)
        {
            return _pr.GetProductoByName(name);
        }

        public void AddProducto(Producto p)
        {
            _pr.AddProducto(p);
        }

        public void DeleteProducto(int id)
        {
            _pr.DeleteProducto(id);
        }

        public void UpdateProducto(Producto p)
        {
            _pr.UpdateProducto(p);
        }

        public int CalculateStock(int id)
        {
            var ventasDelProducto = _vb.GetVentas().Where(v => v.ProductoId == id).Sum(v => v.Cantidad);
            var comprasDelProducto = _cb.GetCompras().Where(c => c.ProductoId == id).Sum(c => c.Cantidad);
            
            return comprasDelProducto - ventasDelProducto;
        }
    }
}

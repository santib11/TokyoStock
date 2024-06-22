﻿
using TokyoStock.Core.Entities;
using TokyoStock.Core.Entities.Filters;

namespace TokyoStock.Core.Data
{
    public class CompraRepository
    {

        public CompraRepository()
        {
        }

        public List<Compra> GetCompras()
        {
            var list = new List<Compra>();

            using (var db = new TokyoStockContext())
            {
                list = db.Compras.ToList();
            }

            return list;
        }

        public List<Compra> GetComprasByFilter(Filter filter)
        {
            var list = new List<Compra>();

            using (var db = new TokyoStockContext())
            {
                list = db.Compras
                    .Skip((filter.PageIndex - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();
            }

            return list;
        }

        public void AddCompra(Compra c)
        {
            using (var db = new TokyoStockContext())
            {
                db.Compras.Add(c);
                db.SaveChanges();
            }
        }
    }
}

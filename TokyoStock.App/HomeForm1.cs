using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Windows.Forms;
using TokyoStock.Core.Business;
using TokyoStock.Core.Data;
using TokyoStock.Core.Entities;
using TokyoStock.Core.Entities.Filters;

namespace TokyoStock.App
{
    public partial class HomeForm1 : Form
    {
        private static ProductoRepository productoRepository = new ProductoRepository();
        private static ProductoBusiness productoBusiness = new ProductoBusiness(productoRepository);
        
        private static CategoriaRepository categoriaRepository = new CategoriaRepository();
        private static CategoriaBusiness categoriaBusiness = new CategoriaBusiness(categoriaRepository);

        private static CompraRepository compraRepository = new CompraRepository();
        private static CompraBusiness compraBusiness = new CompraBusiness(compraRepository);

        private static VentaRepository ventaRepository = new VentaRepository();
        private static VentaBusiness ventaBusiness = new VentaBusiness(ventaRepository);

        public static bool _isLogged = false;

        private int currentPageIndex = 1;
        private int pageSize = 10;
        private int totalRecords = 0;

        public HomeForm1()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {

            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ProductoId", "ID");
            dataGridView1.Columns.Add("Nombre", "Nombre");
            dataGridView1.Columns.Add("CategoriaNombre", "Categoria");
            dataGridView1.Columns.Add("Cantidad", "Stock");
            DataGridViewCheckBoxColumn habilitadoColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Habilitado",
                HeaderText = "Habilitado",
                DataPropertyName = "Habilitado",
                TrueValue = true,
                FalseValue = false
            };
            dataGridView1.Columns.Add(habilitadoColumn);

            var filter = new Filter
            {
                PageIndex = currentPageIndex,
                PageSize = pageSize
            };

            var result = productoBusiness.GetProductosByFilter(filter);
            totalRecords = result.total;

            var ventas = ventaBusiness.GetVentas();
            var compras = compraBusiness.GetCompras();

            var ds = result.list.Select(p => new
            {
                p.ProductoId,
                p.Nombre,
                CategoriaNombre = p.Categoria.Nombre,
                p.Habilitado,
                cantidad = compras.Where(c => c.ProductoId == p.ProductoId)
                                .Sum(c => c.Cantidad) - ventas.Where(v => v.ProductoId == p.ProductoId)
                                .Sum(v => v.Cantidad)
            }).ToList(); 


            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds;

            label3.Text = $"Pagina {currentPageIndex} de {Math.Ceiling((double)totalRecords / pageSize)}";
            btAnterior.Enabled = currentPageIndex > 1;
            btSiguiente.Enabled = currentPageIndex < (int)Math.Ceiling((double)totalRecords / pageSize);

            dataGridView1.Columns["ProductoId"].DataPropertyName = "ProductoId";
            dataGridView1.Columns["Nombre"].DataPropertyName = "Nombre";
            dataGridView1.Columns["CategoriaNombre"].DataPropertyName = "CategoriaNombre";
            dataGridView1.Columns["Cantidad"].DataPropertyName = "cantidad";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var categorias = categoriaBusiness.getCategorias();
            comboBox1.DataSource = categorias;
            comboBox1.DisplayMember = "Nombre";
        }
        private void btSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btIniciar_Click(object sender, EventArgs e)
        {
            var initForm = new InitForm();
            initForm.ShowDialog();
            if (_isLogged)
            {
                btAdministracion.Visible = true;
                btLista.Visible = true;
                btVentas.Visible = true;
                btBuscar.Visible = true;
                tbNombre.Visible = true;
                comboBox1.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                dataGridView1.Visible = true;
                btAnterior.Visible = true;
                btSiguiente.Visible = true;
            }
        }

        private void btAdministracion_Click(object sender, EventArgs e)
        {
            var adminForm = new AdministracionForm();
            adminForm.ShowDialog();
            InitControls();
        }

        private void btLista_Click(object sender, EventArgs e)
        {
            var listaform = new ListaForm();
            listaform.ShowDialog();
        }

        private void btVentas_Click(object sender, EventArgs e)
        {
            var cvform = new CompraVentaForm();
            cvform.ShowDialog();
        }


        private void btBuscar_Click(object sender, EventArgs e)
        {
            string nombre = tbNombre.Text.ToLower();
            string categoria = comboBox1.Text;

            var filter = new Filter
            {
                PageIndex = currentPageIndex,
                PageSize = pageSize
            };

            var ventas = ventaBusiness.GetVentas();
            var compras = compraBusiness.GetCompras();

            var result = productoBusiness.GetProductosByFilter(filter);

            var ds = result.list.Select(p => new
            {
                p.ProductoId,
                p.Nombre,
                CategoriaNombre = p.Categoria.Nombre,
                p.Habilitado,
                cantidad = compras.Where(c => c.ProductoId == p.ProductoId)
                               .Sum(c => c.Cantidad) - ventas.Where(v => v.ProductoId == p.ProductoId)
                               .Sum(v => v.Cantidad)
            }).ToList();

            if (!nombre.IsNullOrEmpty())
            {
                ds = ds.Where(p => p.Nombre.ToLower().StartsWith(nombre) && p.CategoriaNombre.Contains(categoria)).ToList();
                totalRecords = ds.Count();
            }
            else if (nombre.IsNullOrEmpty())
            {
                ds = ds.Where(p => p.CategoriaNombre.Contains(categoria)).ToList();
                totalRecords = ds.Count();
            }
            dataGridView1.DataSource = ds;
            label3.Text = $"Pagina {currentPageIndex} de {Math.Ceiling((double)(totalRecords > 0 ? totalRecords : 1) / pageSize)}";
            btAnterior.Enabled = currentPageIndex > 1;
            btSiguiente.Enabled = currentPageIndex < (int)Math.Ceiling((double)(totalRecords > 0 ? totalRecords : 1) / pageSize);

            btCancelar.Visible = true;
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            tbNombre.Text = "";
            InitControls();
            btCancelar.Visible = false;
        }

        private void btSiguiente_Click(object sender, EventArgs e)
        {
            if ((currentPageIndex * pageSize) < totalRecords)
            {
                currentPageIndex++;
                InitControls();
            }
        }

        private void btAnterior_Click(object sender, EventArgs e)
        {
            if (currentPageIndex > 1)
            {
                currentPageIndex--;
                InitControls();
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var producto = dataGridView1.Rows[e.RowIndex].DataBoundItem;
            foreach (DataGridViewCell cell in dataGridView1.Rows[e.RowIndex].Cells)
            {
                if (!(bool)producto.GetType().GetProperty("Habilitado").GetValue(producto))
                {
                    cell.Style.BackColor = System.Drawing.Color.LightCoral;

                }
                else if((bool)producto.GetType().GetProperty("Habilitado").GetValue(producto) && cell.OwningColumn.Name == "Habilitado")
                {
                    cell.Style.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Modelito.Entidades;
using Modelito.Servicios;
using Modelito.AccesoDatos;

namespace Modelito
{
    public partial class frmNuevaFactura : Form
    {
        private Factura nuevaFactura;
        private GestorFactura gestor;
        public frmNuevaFactura()
        {
            InitializeComponent();
            nuevaFactura = new Factura();
            gestor = new GestorFactura(new DaoFactory()); 
        }

        //EVENTOS
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelar?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Dispose();

            }
            else
            {
                return;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtCliente.Text == "")
            {
                MessageBox.Show("Debe especificar un cliente.", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCliente.Focus();
                return;
            }
            if (cboForma.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar una forma de pago", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar al menos un detalle.", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboProducto.Focus();
                return;
            }

            GuardarPresupuesto();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboProducto.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar un producto", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (txtDescuento.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe ingresar un descuento", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["Producto"].Value.ToString().Equals(cboProducto.Text))
                {
                    MessageBox.Show("Este producto ya se encuentra presupuestado", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            DataRowView item = (DataRowView)cboProducto.SelectedItem;

            int prod = Convert.ToInt32(item.Row.ItemArray[0]);
            string nom = item.Row.ItemArray[1].ToString();
            double pre = Convert.ToDouble(item.Row.ItemArray[2]);
            int cant = Convert.ToInt32(nudCantidad.Text);
            double desc = pre * cant;

            Producto p = new Producto(prod, nom, pre);
            DetalleFactura detalle = new DetalleFactura(p, cant);

            nuevaFactura.AgregarDetalle(detalle);
            dgvDetalles.Rows.Add(new object[] { prod, nom, pre, cant, desc });

            CalcularTotales();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarProductos();
            txtCliente.Text = "Consumidor Final";
            lblNroFactura.Text = Convert.ToString(gestor.ProximaFactura());
            txtDescuento.Text = Convert.ToString(0);
        }

        //METODOS
        private void CargarProductos()
        {
            DataTable tabla = gestor.ObtenerProductos();

            cboProducto.DataSource = tabla;
            cboProducto.DisplayMember = tabla.Columns[1].ColumnName;
            cboProducto.ValueMember = tabla.Columns[0].ColumnName;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            //cboProducto.ValueMember = "id_producto";
            //cboProducto.DisplayMember = "n_producto";
        }

        private void CalcularTotales()
        {
            lblEsSubtotal.Text = nuevaFactura.CalcularTotal().ToString();
            double desc = nuevaFactura.CalcularTotal() * Convert.ToDouble(txtDescuento.Text) / 100;
            lblEsTotal.Text = (nuevaFactura.CalcularTotal() - desc).ToString();
            lblEsDescuento.Text = Convert.ToString(desc);
        }

        private void GuardarPresupuesto()
        {
            nuevaFactura.FacturaNro = Convert.ToInt32(lblNroFactura.Text);
            nuevaFactura.Fecha = Convert.ToDateTime(dtpFecha.Value);
            nuevaFactura.Forma = cboForma.SelectedIndex;
            nuevaFactura.Cliente = txtCliente.Text;
            nuevaFactura.Descuento = double.Parse(txtDescuento.Text);
            nuevaFactura.Total = Convert.ToDouble(lblEsTotal.Text);

            if (gestor.ConfirmarFactura(nuevaFactura))
            {
                MessageBox.Show("Presupuesto registrado con exito.", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Dispose();
            }
            else
            {
                MessageBox.Show("ERROR. No se pudo registrar el presupuesto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 5)
            {
                nuevaFactura.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                CalcularTotales();
            }
        }
    }
}

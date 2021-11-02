using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelito.Entidades;

namespace Modelito.AccesoDatos
{
    class FacturaDao : IFacturaDao
    {
        public bool Crear(Factura oFactura)
        {

				//aca confirmo la operacion sql
				bool resultado = true;

				SqlConnection cnn = new SqlConnection();
				SqlTransaction trans = null;

				try
				{
					cnn.ConnectionString = @"Data Source=DESKTOP-VODM8NI\SQLEXPRESS;Initial Catalog=db_facturas;Integrated Security=True";
					cnn.Open();
					trans = cnn.BeginTransaction();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = cnn;
					cmd.Transaction = trans;
					cmd.CommandText = "SP_INSERTAR_FACTURA";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cliente", oFactura.Cliente);
					cmd.Parameters.AddWithValue("@forma", oFactura.Forma);
					cmd.Parameters.AddWithValue("@total", oFactura.Total);
					cmd.Parameters.AddWithValue("@nro", oFactura.FacturaNro);
					//SqlParameter parameter = new SqlParameter("@factura_nro", SqlDbType.Int);
					//parameter.Direction = ParameterDirection.Output;
					//cmd.Parameters.Add(parameter);
					cmd.ExecuteNonQuery();

					//this.FacturaNro = Convert.ToInt32(parameter.Value);
					int detalleNro = 1;

					foreach (DetalleFactura item in oFactura.Detalles)
					{
						SqlCommand cmdDet = new SqlCommand();
						cmdDet.Connection = cnn;
						cmdDet.Transaction = trans;
						cmdDet.CommandText = "SP_INSERTAR_DETALLES";
						cmdDet.CommandType = CommandType.StoredProcedure;
						cmdDet.Parameters.AddWithValue("@nro", oFactura.FacturaNro);
						cmdDet.Parameters.AddWithValue("@detalle", detalleNro);
						cmdDet.Parameters.AddWithValue("@id_producto", item.Producto.IdProducto);
						cmdDet.Parameters.AddWithValue("@cantidad", item.Cantidad);
						cmdDet.ExecuteNonQuery();
						detalleNro++;
					}

					trans.Commit();
					cnn.Close();
				}
				catch (Exception)
				{
					//en caso que quiera saber que error ocurre
					//MessageBox.Show("error: " + E.Message);
					trans.Rollback();
					resultado = false;
				}
				finally
				{
					if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
				}

				return resultado;
			
		}

        public DataTable CargarCombo()
        {
            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = @"Data Source=DESKTOP-VODM8NI\SQLEXPRESS;Initial Catalog=db_facturas;Integrated Security=True";
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_CONSULTAR_PRODUCTOS";
            DataTable tabla = new DataTable();
            tabla.Load(cmd.ExecuteReader());
            cnn.Close();

            return tabla;
        }

        public int ObtenerProximoNumero()
        {
            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = @"Data Source=DESKTOP-VODM8NI\SQLEXPRESS;Initial Catalog=db_facturas;Integrated Security=True";
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_PROXIMO_ID";
            SqlParameter param = new SqlParameter("@next", SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
            cnn.Close();
            return (int)param.Value + 1;
        }
    }
}

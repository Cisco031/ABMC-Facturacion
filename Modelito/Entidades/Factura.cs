using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Modelito.Entidades
{
    class Factura
    {
		public int FacturaNro { get; set; }
        public DateTime Fecha { get; set; }
		public string Cliente { get; set; }
		public double Total { get; set; }
		public double Descuento { get; set; }
		public DateTime FechaBaja { get; set; }
		public List<DetalleFactura> Detalles { get; set; }
		public int Forma { get; set; }

		public Factura()
		{
			Detalles = new List<DetalleFactura>();
		}
		public void AgregarDetalle(DetalleFactura detalle)
		{
			Detalles.Add(detalle);
		}
		public double CalcularTotal()
		{
			double total = 0;
			foreach (DetalleFactura item in Detalles)
			{
				total += item.CalcularSubtotal();
			}

			return total;
		}

		public void QuitarDetalle(int indice)
		{
			Detalles.RemoveAt(indice);
		}

		

		//public int ProximaFactura()
		//{
		//	en fatucra dao
		//}
	}
}

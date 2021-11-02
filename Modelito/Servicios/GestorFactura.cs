using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelito.AccesoDatos;
using Modelito.Entidades;

namespace Modelito.Servicios
{
    class GestorFactura
    {
        private IFacturaDao dao;

        public GestorFactura(AbstractDaoFactory factory)
        {
            dao = factory.CrearFacturaDao();
        }

        public int ProximaFactura()
        {
           return dao.ObtenerProximoNumero();
        }
        public DataTable ObtenerProductos()
        {
            return dao.CargarCombo();
        }
        public bool ConfirmarFactura(Factura oFactura)
        {
            return dao.Crear(oFactura);
        }

    }
}

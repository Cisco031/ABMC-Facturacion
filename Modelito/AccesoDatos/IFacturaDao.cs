using System.Data;
using Modelito.Entidades;

namespace Modelito.AccesoDatos
{
    interface IFacturaDao
    {
        bool Crear(Factura oFactura);
        int ObtenerProximoNumero();
        DataTable CargarCombo();
    }
}
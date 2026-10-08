using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class Accion_BLL
    {
        private Accion_DAL accionDal = new Accion_DAL();
    
        ///PRE: Recibe accion (Accion_BE) con los datos de auditoría del comando.
        ///POST: No retorna valor. Delega la persistencia del registro en la capa DAL.
        public void RegistrarAccion(Accion_BE accion)
        {
            accionDal.GuardarRegistro();
        }
    }
}

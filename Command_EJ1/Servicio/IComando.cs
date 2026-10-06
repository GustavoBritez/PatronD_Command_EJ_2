using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio
{
    public interface IComando
    {
        string nombre { get; }
        void ejecutar();
        void deshacer();
    }
}

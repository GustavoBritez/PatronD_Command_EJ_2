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

        ///PRE: Ninguno.
        ///POST: No retorna valor. Ejecuta la operación encapsulada sobre el receptor.
        void ejecutar();

        ///PRE: Ninguno.
        ///POST: No retorna valor. Revierte la operación restaurando el estado previo del receptor.
        void deshacer();
    }
}

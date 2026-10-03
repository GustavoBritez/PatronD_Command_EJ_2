using System.Collections.Generic;

namespace Servicio
{
    public class Historial
    {
        private readonly Stack<IComando> _pila = new Stack<IComando>();

        public void Apilar(IComando comando)
        {
            _pila.Push(comando);
        }

        public IComando? Deshacer()
        {
            if (_pila.Count > 0)
            {
                var cmd = _pila.Pop();
                cmd.deshacer();
                return cmd;
            }
            return null;
        }

        public IEnumerable<IComando> ObtenerTodos()
        {
            return _pila.ToArray();
        }

        public int Cantidad => _pila.Count;
    }
}

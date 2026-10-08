using System.Collections.Generic;

namespace Command_EJ2
{
    public class Historial
    {
        private readonly Stack<IComando> _pila = new Stack<IComando>();

        ///PRE: Recibe comando (IComando) recién ejecutado.
        ///POST: No retorna valor. Apila el comando en la estructura LIFO.
        public void Apilar(IComando comando)
        {
            _pila.Push(comando);
        }

        ///PRE: Ninguno.
        ///POST: Retorna el comando IComando deshecho tras invocar su método deshacer(), o null si la pila está vacía.
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

        ///PRE: Ninguno.
        ///POST: Retorna un IEnumerable<IComando> con la secuencia de comandos apilados en el historial.
        public IEnumerable<IComando> ObtenerTodos()
        {
            return _pila.ToArray();
        }

        public int Cantidad => _pila.Count;
    }
}

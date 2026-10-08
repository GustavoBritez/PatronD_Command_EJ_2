using System;
using System.Collections.Generic;

namespace Command_EJ3
{
    public class ConsolaBatiscafo
    {
        private readonly Dictionary<int, BotonConsola> _botones = new Dictionary<int, BotonConsola>();
        private readonly Historial _historial;
        private readonly List<BitacoraMision> _bitacora = new List<BitacoraMision>();

        public Historial Historial => _historial;

        ///PRE: Recibe historial (Historial) para la gestión LIFO de maniobras del batiscafo.
        ///POST: Inicializa una nueva instancia de ConsolaBatiscafo.
        public ConsolaBatiscafo(Historial historial)
        {
            _historial = historial;
        }

        ///PRE: Recibe numero (int) de ranura, descripcion (string) y comando (IComando).
        ///POST: No retorna valor. Configura o reasigna el interruptor de la consola.
        public void ConfigurarBoton(int numero, string descripcion, IComando comando)
        {
            if (!_botones.ContainsKey(numero))
            {
                _botones[numero] = new BotonConsola(numero, descripcion);
            }
            _botones[numero].Descripcion = descripcion;
            _botones[numero].AsignarComando(comando);
        }

        ///PRE: Recibe numero (int) del botón solicitado.
        ///POST: Retorna el BotonConsola si existe, o null en caso contrario.
        public BotonConsola? ObtenerBoton(int numero)
        {
            return _botones.TryGetValue(numero, out var boton) ? boton : null;
        }

        ///PRE: Recibe numero (int) del botón accionado por el piloto.
        ///POST: Retorna true si se ejecutó el comando asociado, false en caso contrario.
        public bool PresionarBoton(int numero)
        {
            if (_botones.TryGetValue(numero, out var boton) && boton.Comando != null)
            {
                boton.Comando.ejecutar();
                _historial.Apilar(boton.Comando);
                _bitacora.Add(new BitacoraMision(Guid.NewGuid(), boton.Comando.nombre, DateTime.Now, false));
                return true;
            }
            return false;
        }

        ///PRE: Ninguno.
        ///POST: Retorna el IComando deshecho si había maniobras en el historial, o null.
        public IComando? PresionarDeshacer()
        {
            var cmd = _historial.Deshacer();
            if (cmd != null)
            {
                _bitacora.Add(new BitacoraMision(Guid.NewGuid(), cmd.nombre, DateTime.Now, true));
            }
            return cmd;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una colección enumerable (IEnumerable<BotonConsola>) con todos los botones configurados.
        public IEnumerable<BotonConsola> ObtenerTodosLosBotones()
        {
            return _botones.Values;
        }

        ///PRE: Ninguno.
        ///POST: Retorna la lista de bitácora con los eventos registrados en la misión.
        public IReadOnlyList<BitacoraMision> ObtenerBitacora()
        {
            return _bitacora.AsReadOnly();
        }
    }
}

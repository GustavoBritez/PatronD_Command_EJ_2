using System;
using System.Collections.Generic;
using BE;
using BLL;

namespace Servicio
{
    public class ConsolaOperador
    {
        private readonly Dictionary<int, BotonConsola> _botones = new Dictionary<int, BotonConsola>();
        private readonly Historial _historial;
        private readonly Accion_BLL? _accionBll;

        public Historial Historial => _historial;

        ///PRE: Recibe historial (Historial) y opcionalmente accionBll (Accion_BLL?).
        ///POST: Inicializa una nueva instancia de ConsolaOperador.
        public ConsolaOperador(Historial historial, Accion_BLL? accionBll = null)
        {
            _historial = historial;
            _accionBll = accionBll;
        }

        ///PRE: Recibe numero (int) de slot, descripcion (string) y comando (IComando).
        ///POST: No retorna valor. Configura o actualiza el botón en la posición indicada.
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
        ///POST: Retorna la instancia de BotonConsola correspondiente, o null si no existe.
        public BotonConsola? ObtenerBoton(int numero)
        {
            return _botones.TryGetValue(numero, out var boton) ? boton : null;
        }

        ///PRE: Recibe numero (int) del botón a presionar.
        ///POST: Retorna true si el botón tiene comando y se ejecutó con éxito, false en caso contrario.
        public bool PresionarBoton(int numero)
        {
            if (_botones.TryGetValue(numero, out var boton) && boton.Comando != null)
            {
                boton.Comando.ejecutar();
                _historial.Apilar(boton.Comando);
                _accionBll?.RegistrarAccion(new Accion_BE(Guid.NewGuid(), boton.Comando.nombre, DateTime.Now, false));
                return true;
            }
            return false;
        }

        ///PRE: Ninguno.
        ///POST: Retorna el IComando deshecho si existía en el historial, o null si la pila estaba vacía.
        public IComando? PresionarBotonPanico()
        {
            var comandoDeshecho = _historial.Deshacer();
            if (comandoDeshecho != null)
            {
                _accionBll?.RegistrarAccion(new Accion_BE(Guid.NewGuid(), comandoDeshecho.nombre, DateTime.Now, true));
            }
            return comandoDeshecho;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una colección enumerable (IEnumerable<BotonConsola>) con los botones configurados.
        public IEnumerable<BotonConsola> ObtenerTodosLosBotones()
        {
            return _botones.Values;
        }
    }
}

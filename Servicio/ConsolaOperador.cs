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

        public ConsolaOperador(Historial historial, Accion_BLL? accionBll = null)
        {
            _historial = historial;
            _accionBll = accionBll;
        }

        public void ConfigurarBoton(int numero, string descripcion, IComando comando)
        {
            if (!_botones.ContainsKey(numero))
            {
                _botones[numero] = new BotonConsola(numero, descripcion);
            }
            _botones[numero].Descripcion = descripcion;
            _botones[numero].AsignarComando(comando);
        }

        public BotonConsola? ObtenerBoton(int numero)
        {
            return _botones.TryGetValue(numero, out var boton) ? boton : null;
        }

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

        public IComando? PresionarBotonPanico()
        {
            var comandoDeshecho = _historial.Deshacer();
            if (comandoDeshecho != null)
            {
                _accionBll?.RegistrarAccion(new Accion_BE(Guid.NewGuid(), comandoDeshecho.nombre, DateTime.Now, true));
            }
            return comandoDeshecho;
        }

        public IEnumerable<BotonConsola> ObtenerTodosLosBotones()
        {
            return _botones.Values;
        }
    }
}

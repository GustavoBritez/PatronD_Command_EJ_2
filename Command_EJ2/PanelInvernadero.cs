using System;
using System.Collections.Generic;

namespace Command_EJ2
{
    public class PanelInvernadero
    {
        private readonly Dictionary<int, BotonPanel> _botones = new Dictionary<int, BotonPanel>();
        private readonly Historial _historial;
        private readonly List<AccionCultivo> _auditoria = new List<AccionCultivo>();

        public Historial Historial => _historial;

        ///PRE: Recibe historial (Historial) para la gestión LIFO de operaciones.
        ///POST: Inicializa una nueva instancia de PanelInvernadero.
        public PanelInvernadero(Historial historial)
        {
            _historial = historial;
        }

        ///PRE: Recibe numero (int) de ranura, descripcion (string) y comando (IComando).
        ///POST: No retorna valor. Asigna o reconfigura el botón con el comando recibido.
        public void ConfigurarBoton(int numero, string descripcion, IComando comando)
        {
            if (!_botones.ContainsKey(numero))
            {
                _botones[numero] = new BotonPanel(numero, descripcion);
            }
            _botones[numero].Descripcion = descripcion;
            _botones[numero].AsignarComando(comando);
        }

        ///PRE: Recibe numero (int) de ranura del botón requerido.
        ///POST: Retorna el BotonPanel si existe, o null en caso contrario.
        public BotonPanel? ObtenerBoton(int numero)
        {
            return _botones.TryGetValue(numero, out var boton) ? boton : null;
        }

        ///PRE: Recibe numero (int) del botón a accionar por el operador.
        ///POST: Retorna true si se ejecutó el comando asociado, false en caso contrario.
        public bool PresionarBoton(int numero)
        {
            if (_botones.TryGetValue(numero, out var boton) && boton.Comando != null)
            {
                boton.Comando.ejecutar();
                _historial.Apilar(boton.Comando);
                _auditoria.Add(new AccionCultivo(Guid.NewGuid(), boton.Comando.nombre, DateTime.Now, false));
                return true;
            }
            return false;
        }

        ///PRE: Ninguno.
        ///POST: Retorna el IComando deshecho si había operaciones pendientes en el historial, o null.
        public IComando? PresionarDeshacer()
        {
            var cmd = _historial.Deshacer();
            if (cmd != null)
            {
                _auditoria.Add(new AccionCultivo(Guid.NewGuid(), cmd.nombre, DateTime.Now, true));
            }
            return cmd;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una colección enumerable (IEnumerable<BotonPanel>) con todos los botones configurados.
        public IEnumerable<BotonPanel> ObtenerTodosLosBotones()
        {
            return _botones.Values;
        }

        ///PRE: Ninguno.
        ///POST: Retorna la lista de auditoría con las acciones registradas.
        public IReadOnlyList<AccionCultivo> ObtenerAuditoria()
        {
            return _auditoria.AsReadOnly();
        }
    }
}

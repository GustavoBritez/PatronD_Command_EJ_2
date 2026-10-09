using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using BLL;
using Servicio;

namespace Command
{
    public partial class Form1 : Form
    {
       

        private readonly Transmisor_BLL _transmisor = new Transmisor_BLL();
        private readonly ProcesadorVoz_BLL _procesador = new ProcesadorVoz_BLL();
        private readonly Accion_BLL _accionBll = new Accion_BLL();

        private readonly Historial _historial = new Historial();
        private readonly ConsolaOperador _consola;

        
        ///PRE: Ninguno.
        ///POST: Inicializa los componentes, consola, interfaz y activa la configuración inicial.
        public Form1()
        {
            InitializeComponent();
            _consola = new ConsolaOperador(_historial, _accionBll);

            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            ConstruirInterfaz();
            ActivarModoManana();
            RefrescarTelemetria();

            KeyDown += Consola_KeyDown;
        }


        ///PRE: Ninguno.
        ///POST: No retorna valor. Asigna comandos del perfil de la mañana a los botones del operador y actualiza la vista.
        private void ActivarModoManana()
        {
            _consola.ConfigurarBoton(1, "Mutear Micrófonos", new ComandoModoMuteS(_transmisor));
            _consola.ConfigurarBoton(2, "Distorsión Voz (Anónimo)", new ComandoPitchS(_procesador));
            _consola.ConfigurarBoton(3, "Potencia Reducida (50 W)", new ComandoPotenciaS(_transmisor, 50));
            _consola.ConfigurarBoton(4, "Corte de Emergencia", new ComandoEmergenciaS(_transmisor));

            btnModoManana.BackColor = AccentBlue;
            btnModoManana.FlatAppearance.BorderColor = AccentBlueBorder;
            btnModoManana.ForeColor = TextPrimary;

            btnModoNoche.BackColor = BtnDefaultBg;
            btnModoNoche.FlatAppearance.BorderColor = BorderControl;
            btnModoNoche.ForeColor = TextSecondary;

            lblStatusBar.Text = "Configuración activa: Turno Mañana (Entrevistas • Potencia 50 W • Filtro de Distorsión).";
            RefrescarTelemetria();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Asigna comandos del perfil de la noche a los botones del operador y actualiza la vista.
        private void ActivarModoNoche()
        {
            _consola.ConfigurarBoton(1, "Mutear Transmisión", new ComandoModoMuteS(_transmisor));
            _consola.ConfigurarBoton(2, "Filtro DJ / Electrónica", new ComandoPitchS(_procesador));
            _consola.ConfigurarBoton(3, "Potencia Completa (100 W)", new ComandoPotenciaS(_transmisor, 100));
            _consola.ConfigurarBoton(4, "Corte de Emergencia", new ComandoEmergenciaS(_transmisor));

            btnModoNoche.BackColor = AccentBlue;
            btnModoNoche.FlatAppearance.BorderColor = AccentBlueBorder;
            btnModoNoche.ForeColor = TextPrimary;

            btnModoManana.BackColor = BtnDefaultBg;
            btnModoManana.FlatAppearance.BorderColor = BorderControl;
            btnModoManana.ForeColor = TextSecondary;

            lblStatusBar.Text = "Configuración activa: Turno Noche (Música Electrónica • Potencia 100 W • Filtro DJ).";
            RefrescarTelemetria();
        }

        ///PRE: Recibe numero (int) correspondiente al slot de botón presionado.
        ///POST: No retorna valor. Ejecuta el comando asociado, actualiza la telemetría y la barra de estado.
        private void PresionarBoton(int numero)
        {
            var boton = _consola.ObtenerBoton(numero);
            if (boton?.Comando != null)
            {
                string nombre = boton.Comando.nombre;
                _consola.PresionarBoton(numero);
                lblStatusBar.Text = $"Ejecutado: Botón [{numero}] activó '{nombre}' a las {DateTime.Now:HH:mm:ss}";
                RefrescarTelemetria();
            }
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Deshace la última acción registrada en el historial y actualiza la telemetría.
        private void PresionarPanico()
        {
            var deshecho = _consola.PresionarBotonPanico();
            if (deshecho != null)
            {
                lblStatusBar.Text = $"Deshecho: Se revirtió el comando '{deshecho.nombre}' a las {DateTime.Now:HH:mm:ss}";
            }
            else
            {
                lblStatusBar.Text = "Aviso: No hay comandos en el historial para deshacer.";
            }
            RefrescarTelemetria();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Actualiza todos los elementos de la interfaz reflejando el estado de los receptores e historial.
        private void RefrescarTelemetria()
        {
            lblTagSlot1.Text = $"Comando asignado: {_consola.ObtenerBoton(1)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot2.Text = $"Comando asignado: {_consola.ObtenerBoton(2)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot3.Text = $"Comando asignado: {_consola.ObtenerBoton(3)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot4.Text = $"Comando asignado: {_consola.ObtenerBoton(4)?.Comando?.nombre ?? "Ninguno"}";

            bool muteado = _transmisor.EstaMuteado();
            if (muteado)
            {
                lblDisplayTxStatus.Text = "■ CANAL SILENCIADO";
                lblDisplayTxStatus.BackColor = MutedBg;
                lblDisplayTxStatus.ForeColor = MutedText;
            }
            else
            {
                lblDisplayTxStatus.Text = "● TRANSMISIÓN EN EL AIRE";
                lblDisplayTxStatus.BackColor = OnAirBg;
                lblDisplayTxStatus.ForeColor = OnAirText;
            }

            int potencia = _transmisor.ObtenerPotenciaActual();
            string modoPotencia = potencia == 0 ? "Apagada" : (potencia <= 50 ? "Modo Reducido" : "Nivel Máximo");
            lblDisplayTxPower.Text = $"Potencia de Antena: {potencia} W ({modoPotencia})";
            pnlPowerMeter?.Invalidate();

            bool pitch = _procesador.EstaPitchActivo();
            if (pitch)
            {
                lblDisplayDspPitch.Text = "● Efecto de Voz Activado";
                lblDisplayDspPitch.BackColor = PitchActiveBg;
                lblDisplayDspPitch.ForeColor = PitchActiveText;
            }
            else
            {
                lblDisplayDspPitch.Text = "○ Voz Natural (Bypass)";
                lblDisplayDspPitch.BackColor = BgInnerPanel;
                lblDisplayDspPitch.ForeColor = TextSecondary;
            }

            lblDisplayDspGain.Text = $"Ganancia de Entrada: {_procesador.ObtenerGananciaActual()} dB (Nivel Nominal)";

            lstPilaComandos.Items.Clear();
            var lista = _historial.ObtenerTodos().ToList();
            lblProfundidadPila.Text = $"Comandos en pila: {lista.Count}";

            if (lista.Count == 0)
            {
                lstPilaComandos.Items.Add("(No hay comandos en el historial)");
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    string prefijo = i == 0 ? "▶ [Tope] " : $"   [{lista.Count - i}] ";
                    lstPilaComandos.Items.Add($"{prefijo}{lista[i].nombre}");
                }
            }

            lblDisplayTxStatus.Invalidate();
            lblDisplayDspPitch.Invalidate();
        }

        ///PRE: Recibe sender (object?) y e (KeyEventArgs) con los datos del evento de teclado.
        ///POST: No retorna valor. Captura combinaciones de teclas de acceso rápido y acciona los comandos.
        private void Consola_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                PresionarPanico();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
            {
                PresionarBoton(1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
            {
                PresionarBoton(2);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
            {
                PresionarBoton(3);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4)
            {
                PresionarBoton(4);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.P || e.KeyCode == Keys.Space)
            {
                PresionarBoton(1);
                e.Handled = true;
            }
        }

        #region graficos

        private static readonly Color BgForm = Color.FromArgb(17, 20, 24);
        private static readonly Color BgCard = Color.FromArgb(24, 28, 35);
        private static readonly Color BgCardHeader = Color.FromArgb(30, 35, 45);
        private static readonly Color BgInnerPanel = Color.FromArgb(19, 22, 28);
        private static readonly Color BorderCard = Color.FromArgb(43, 50, 64);
        private static readonly Color BorderControl = Color.FromArgb(51, 60, 77);

        private static readonly Color TextPrimary = Color.FromArgb(248, 250, 252);
        private static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);
        private static readonly Color TextMuted = Color.FromArgb(100, 116, 139);

        private static readonly Color BtnDefaultBg = Color.FromArgb(32, 38, 49);
        private static readonly Color BtnDefaultHover = Color.FromArgb(43, 51, 66);
        private static readonly Color BtnDefaultPress = Color.FromArgb(26, 31, 40);

        private static readonly Color AccentBlue = Color.FromArgb(37, 99, 235);
        private static readonly Color AccentBlueHover = Color.FromArgb(29, 78, 216);
        private static readonly Color AccentBlueBorder = Color.FromArgb(59, 130, 246);

        private static readonly Color OnAirBg = Color.FromArgb(6, 78, 59);
        private static readonly Color OnAirBorder = Color.FromArgb(16, 185, 129);
        private static readonly Color OnAirText = Color.FromArgb(167, 243, 208);

        private static readonly Color MutedBg = Color.FromArgb(69, 10, 10);
        private static readonly Color MutedBorder = Color.FromArgb(239, 68, 68);
        private static readonly Color MutedText = Color.FromArgb(254, 202, 202);

        private static readonly Color PitchActiveBg = Color.FromArgb(46, 16, 101);
        private static readonly Color PitchActiveBorder = Color.FromArgb(139, 92, 246);
        private static readonly Color PitchActiveText = Color.FromArgb(233, 213, 255);

        private static readonly Color UndoBg = Color.FromArgb(50, 24, 28);
        private static readonly Color UndoBorder = Color.FromArgb(185, 28, 28);
        private static readonly Color UndoText = Color.FromArgb(254, 202, 202);

        private Button btnSlot1 = null!;
        private Button btnSlot2 = null!;
        private Button btnSlot3 = null!;
        private Button btnSlot4 = null!;
        private Button btnPedalSuelo = null!;
        private Button btnDeshacer = null!;

        private Label lblTagSlot1 = null!;
        private Label lblTagSlot2 = null!;
        private Label lblTagSlot3 = null!;
        private Label lblTagSlot4 = null!;

        private Button btnModoManana = null!;
        private Button btnModoNoche = null!;

        private Label lblDisplayTxStatus = null!;
        private Label lblDisplayTxPower = null!;
        private Panel pnlPowerMeter = null!;
        private Label lblDisplayDspPitch = null!;
        private Label lblDisplayDspGain = null!;

        private ListBox lstPilaComandos = null!;
        private Label lblProfundidadPila = null!;
        private Label lblStatusBar = null!;
        private void ConstruirInterfaz()
        {
            Controls.Clear();
            BackColor = BgForm;
            ForeColor = TextPrimary;

            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = BgCard
            };
            pnlHeader.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using var penBorder = new Pen(BorderCard, 1);
                g.DrawLine(penBorder, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
            pnlHeader.Resize += (s, e) => pnlHeader.Invalidate();

            var lblTitle = new Label
            {
                Text = "Consola de Transmisión de Audio",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(24, 12)
            };

            var lblSubtitle = new Label
            {
                Text = "Implementación del Patrón Command  •  Invocador, IComando, Receptores BLL e Historial LIFO",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(25, 38)
            };

            var pnlHeaderRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 260,
                BackColor = Color.Transparent
            };

            var lblFreq = new Label
            {
                Text = "Frecuencia: 100.1 MHz FM",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(40, 16)
            };

            var lblOnlineStatus = new Label
            {
                Text = "● Sistema en línea",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = OnAirBorder,
                AutoSize = true,
                Location = new Point(40, 36)
            };

            pnlHeaderRight.Controls.AddRange(new Control[] { lblFreq, lblOnlineStatus });
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, pnlHeaderRight });
            Controls.Add(pnlHeader);

            lblStatusBar = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = BgCardHeader,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Text = "Listo  •  Presione los botones [1..4], [P] para pedal de mute, o [Ctrl+Z] para deshacer."
            };
            lblStatusBar.Paint += (s, e) =>
            {
                using var p = new Pen(BorderCard, 1);
                e.Graphics.DrawLine(p, 0, 0, lblStatusBar.Width, 0);
            };
            lblStatusBar.Resize += (s, e) => lblStatusBar.Invalidate();
            Controls.Add(lblStatusBar);

            var tblMainGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 14, 16, 14)
            };
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37F));
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tblMainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var pnlCol1Card = CrearCardPanel("PANEL DE OPERADOR (INVOCADORES)");
            pnlCol1Card.Dock = DockStyle.Fill;

            var pnlCol1Content = new Panel
            {
                Location = new Point(16, 44),
                Size = new Size(pnlCol1Card.ClientSize.Width - 32, pnlCol1Card.ClientSize.Height - 52),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            int yPos = 4;
            int btnHeight = 42;
            int slotGap = 64;

            btnSlot1 = CrearBotonAccion("[1]  Silenciar Canal", yPos, btnHeight);
            btnSlot1.Click += (s, e) => PresionarBoton(1);
            lblTagSlot1 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot1, lblTagSlot1 });

            yPos += slotGap;
            btnSlot2 = CrearBotonAccion("[2]  Modulador de Voz", yPos, btnHeight);
            btnSlot2.Click += (s, e) => PresionarBoton(2);
            lblTagSlot2 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot2, lblTagSlot2 });

            yPos += slotGap;
            btnSlot3 = CrearBotonAccion("[3]  Potencia de Transmisión", yPos, btnHeight);
            btnSlot3.Click += (s, e) => PresionarBoton(3);
            lblTagSlot3 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot3, lblTagSlot3 });

            yPos += slotGap;
            btnSlot4 = CrearBotonAccion("[4]  Corte de Emergencia", yPos, btnHeight);
            btnSlot4.Click += (s, e) => PresionarBoton(4);
            lblTagSlot4 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot4, lblTagSlot4 });

            yPos += slotGap + 6;
            var sepLine = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(pnlCol1Content.ClientSize.Width, 1),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BorderCard
            };
            pnlCol1Content.Controls.Add(sepLine);

            yPos += 14;
            btnPedalSuelo = CrearBotonAccion("Pedal de Mute (Atajo Tecla P / Espacio)", yPos, 38);
            btnPedalSuelo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnPedalSuelo.Click += (s, e) => PresionarBoton(1);
            pnlCol1Content.Controls.Add(btnPedalSuelo);

            yPos += 48;
            btnDeshacer = new Button
            {
                Text = "Deshacer Último Comando (Ctrl + Z)",
                Location = new Point(0, yPos),
                Size = new Size(pnlCol1Content.ClientSize.Width, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = UndoBg,
                ForeColor = UndoText,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnDeshacer.FlatAppearance.BorderColor = UndoBorder;
            btnDeshacer.FlatAppearance.BorderSize = 1;
            btnDeshacer.FlatAppearance.MouseOverBackColor = ControlPaint.Light(UndoBg, 0.25f);
            btnDeshacer.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(UndoBg, 0.20f);
            btnDeshacer.Click += (s, e) => PresionarPanico();
            pnlCol1Content.Controls.Add(btnDeshacer);

            pnlCol1Card.Controls.Add(pnlCol1Content);
            tblMainGrid.Controls.Add(pnlCol1Card, 0, 0);

            var tblCenterStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            tblCenterStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            tblCenterStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var pnlProfilesCard = CrearCardPanel("PERFILES DE TRANSMISIÓN");
            pnlProfilesCard.Dock = DockStyle.Fill;

            var lblProfileInfo = new Label
            {
                Text = "Reconfiguración dinámica de comandos para los botones del operador:",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                Location = new Point(16, 42),
                AutoSize = true
            };

            var tblProfileButtons = new TableLayoutPanel
            {
                Location = new Point(16, 64),
                Size = new Size(pnlProfilesCard.ClientSize.Width - 32, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tblProfileButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblProfileButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            btnModoManana = new Button
            {
                Text = "Turno Mañana (Entrevistas)",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 4, 0),
                BackColor = AccentBlue,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoManana.FlatAppearance.BorderColor = AccentBlueBorder;
            btnModoManana.FlatAppearance.BorderSize = 1;
            btnModoManana.Click += (s, e) => ActivarModoManana();

            btnModoNoche = new Button
            {
                Text = "Turno Noche (Musical / DJ)",
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 0, 0),
                BackColor = BtnDefaultBg,
                ForeColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoNoche.FlatAppearance.BorderColor = BorderControl;
            btnModoNoche.FlatAppearance.BorderSize = 1;
            btnModoNoche.Click += (s, e) => ActivarModoNoche();

            tblProfileButtons.Controls.Add(btnModoManana, 0, 0);
            tblProfileButtons.Controls.Add(btnModoNoche, 1, 0);

            pnlProfilesCard.Controls.AddRange(new Control[] { lblProfileInfo, tblProfileButtons });
            tblCenterStack.Controls.Add(pnlProfilesCard, 0, 0);

            var pnlReceiversCard = CrearCardPanel("ESTADO DE RECEPTORES DE AUDIO (BLL)");
            pnlReceiversCard.Dock = DockStyle.Fill;
            pnlReceiversCard.Margin = new Padding(0, 8, 0, 0);

            var pnlReceiversContent = new Panel
            {
                Location = new Point(16, 42),
                Size = new Size(pnlReceiversCard.ClientSize.Width - 32, pnlReceiversCard.ClientSize.Height - 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            var lblTxSection = new Label
            {
                Text = "Transmisor Principal (Transmisor_BLL)",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 4),
                AutoSize = true
            };

            lblDisplayTxStatus = new Label
            {
                Text = "● TRANSMISIÓN EN EL AIRE",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = OnAirText,
                BackColor = OnAirBg,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 28),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lblDisplayTxStatus.Paint += (s, e) =>
            {
                using var p = new Pen(OnAirBorder, 1);
                e.Graphics.DrawRectangle(p, 0, 0, lblDisplayTxStatus.Width - 1, lblDisplayTxStatus.Height - 1);
            };

            lblDisplayTxPower = new Label
            {
                Text = "Potencia de Antena: 100 W (Nivel Máximo)",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                Location = new Point(0, 70),
                AutoSize = true
            };

            pnlPowerMeter = new Panel
            {
                Location = new Point(0, 92),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgInnerPanel
            };
            pnlPowerMeter.Paint += (s, e) =>
            {
                var g = e.Graphics;
                int pot = _transmisor.ObtenerPotenciaActual();
                float ratio = Math.Clamp(pot / 100.0f, 0.0f, 1.0f);
                int fillWidth = (int)((pnlPowerMeter.Width - 2) * ratio);

                if (fillWidth > 0)
                {
                    Color fillCol = pot == 0 ? MutedBorder : (pot <= 50 ? AccentBlueBorder : OnAirBorder);
                    using var b = new SolidBrush(fillCol);
                    g.FillRectangle(b, 1, 1, fillWidth, pnlPowerMeter.Height - 2);
                }

                using var penBorder = new Pen(BorderCard, 1);
                g.DrawRectangle(penBorder, 0, 0, pnlPowerMeter.Width - 1, pnlPowerMeter.Height - 1);

                int midX = pnlPowerMeter.Width / 2;
                using var penMid = new Pen(Color.FromArgb(70, 78, 95), 1);
                g.DrawLine(penMid, midX, 1, midX, pnlPowerMeter.Height - 2);
            };
            pnlPowerMeter.Resize += (s, e) => pnlPowerMeter.Invalidate();

            var lblDspSection = new Label
            {
                Text = "Procesador de Voz (ProcesadorVoz_BLL)",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 126),
                AutoSize = true
            };

            lblDisplayDspPitch = new Label
            {
                Text = "○ Voz Natural (Bypass)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = TextSecondary,
                BackColor = BgInnerPanel,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 150),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lblDisplayDspPitch.Paint += (s, e) =>
            {
                bool pitch = _procesador.EstaPitchActivo();
                Color border = pitch ? PitchActiveBorder : BorderCard;
                using var p = new Pen(border, 1);
                e.Graphics.DrawRectangle(p, 0, 0, lblDisplayDspPitch.Width - 1, lblDisplayDspPitch.Height - 1);
            };

            lblDisplayDspGain = new Label
            {
                Text = "Ganancia de Entrada: 0 dB (Nivel Nominal)",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                Location = new Point(0, 192),
                AutoSize = true
            };

            var pnlArchNote = new Panel
            {
                Location = new Point(0, 222),
                Size = new Size(pnlReceiversContent.ClientSize.Width, Math.Max(120, pnlReceiversContent.ClientSize.Height - 230)),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgInnerPanel
            };
            pnlArchNote.Paint += (s, e) =>
            {
                using var p = new Pen(BorderCard, 1);
                e.Graphics.DrawRectangle(p, 0, 0, pnlArchNote.Width - 1, pnlArchNote.Height - 1);
            };
            pnlArchNote.Resize += (s, e) => pnlArchNote.Invalidate();

            var lblArchTitle = new Label
            {
                Text = "Arquitectura del Patrón Command:",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(12, 10),
                AutoSize = true
            };

            var lblArchText = new Label
            {
                Text = "1. Invocador (Botón): Solo invoca comando.ejecutar() sin conocer detalles.\n" +
                       "2. IComando: Interfaz abstracta que encapsula la acción y su reversión.\n" +
                       "3. Receptores (BLL): Realizan la lógica interna de audio y transmisión.\n" +
                       "4. Historial (Pila LIFO): Almacena los comandos para deshacer() seguro.",
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(12, 32),
                Size = new Size(pnlArchNote.ClientSize.Width - 24, pnlArchNote.ClientSize.Height - 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            pnlArchNote.Controls.AddRange(new Control[] { lblArchTitle, lblArchText });

            pnlReceiversContent.Controls.AddRange(new Control[]
            {
                lblTxSection, lblDisplayTxStatus, lblDisplayTxPower, pnlPowerMeter,
                lblDspSection, lblDisplayDspPitch, lblDisplayDspGain, pnlArchNote
            });

            pnlReceiversCard.Controls.Add(pnlReceiversContent);
            tblCenterStack.Controls.Add(pnlReceiversCard, 0, 1);
            tblMainGrid.Controls.Add(tblCenterStack, 1, 0);

            var pnlStackCard = CrearCardPanel("HISTORIAL DE COMANDOS (PILA LIFO)");
            pnlStackCard.Dock = DockStyle.Fill;

            lblProfundidadPila = new Label
            {
                Text = "Comandos en pila: 0",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = TextSecondary,
                Location = new Point(16, 42),
                AutoSize = true
            };

            lstPilaComandos = new ListBox
            {
                Location = new Point(16, 68),
                Size = new Size(pnlStackCard.ClientSize.Width - 32, pnlStackCard.ClientSize.Height - 116),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgInnerPanel,
                ForeColor = TextPrimary,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                ItemHeight = 28,
                DrawMode = DrawMode.OwnerDrawFixed
            };
            lstPilaComandos.DrawItem += LstPilaComandos_DrawItem;

            var lblStackNote = new Label
            {
                Text = "Al pulsar Deshacer se extrae el comando del tope y se restaura el receptor.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = TextMuted,
                Location = new Point(16, pnlStackCard.ClientSize.Height - 34),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                AutoSize = true
            };

            pnlStackCard.Controls.AddRange(new Control[] { lblProfundidadPila, lstPilaComandos, lblStackNote });
            tblMainGrid.Controls.Add(pnlStackCard, 2, 0);

            Controls.Add(tblMainGrid);
        }

        ///PRE: Recibe titulo (string) del encabezado de la tarjeta.
        ///POST: Retorna un nuevo Panel estilizado como tarjeta con borde y cabecera.
        private Panel CrearCardPanel(string titulo)
        {
            var pnl = new Panel
            {
                BackColor = BgCard
            };

            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using var penBorder = new Pen(BorderCard, 1);
                g.DrawRectangle(penBorder, 0, 0, pnl.Width - 1, pnl.Height - 1);

                using var brushHeader = new SolidBrush(BgCardHeader);
                g.FillRectangle(brushHeader, 1, 1, pnl.Width - 2, 32);
                g.DrawLine(penBorder, 0, 32, pnl.Width, 32);

                using var brushText = new SolidBrush(TextSecondary);
                using var fontHdr = new Font("Segoe UI", 8F, FontStyle.Bold);
                g.DrawString(titulo, fontHdr, brushText, 14, 8);
            };
            pnl.Resize += (s, e) => pnl.Invalidate();

            return pnl;
        }

        ///PRE: Recibe texto (string) del botón, top (int) de posición vertical y height (int) de altura.
        ///POST: Retorna una nueva instancia de Button estilizada de forma plana.
        private Button CrearBotonAccion(string texto, int top, int height)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(0, top),
                Size = new Size(320, height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BtnDefaultBg,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderColor = BorderControl;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = BtnDefaultHover;
            btn.FlatAppearance.MouseDownBackColor = BtnDefaultPress;
            return btn;
        }

        ///PRE: Recibe top (int) con la coordenada vertical del control.
        ///POST: Retorna una nueva instancia de Label para indicar el comando asignado al botón.
        private Label CrearLabelDetalle(int top)
        {
            return new Label
            {
                Text = "Comando asignado: Ninguno",
                Location = new Point(2, top),
                Size = new Size(318, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular)
            };
        }

        ///PRE: Recibe sender (object?) y e (DrawItemEventArgs) con el contexto de pintado del ítem.
        ///POST: No retorna valor. Dibuja el elemento del ListBox con formato personalizado.
        private void LstPilaComandos_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstPilaComandos.Items.Count) return;

            var g = e.Graphics;
            string texto = lstPilaComandos.Items[e.Index].ToString() ?? "";
            bool isTop = e.Index == 0 && _historial.Cantidad > 0;

            Color itemBg = isTop ? Color.FromArgb(28, 36, 52) : ((e.State & DrawItemState.Selected) != 0 ? Color.FromArgb(35, 42, 56) : BgInnerPanel);
            using (var brushBg = new SolidBrush(itemBg))
            {
                g.FillRectangle(brushBg, e.Bounds);
            }

            if (isTop)
            {
                using var bAccent = new SolidBrush(AccentBlueBorder);
                g.FillRectangle(bAccent, e.Bounds.Left, e.Bounds.Top, 4, e.Bounds.Height);
            }

            Color textColor = isTop ? TextPrimary : ((_historial.Cantidad == 0) ? TextMuted : TextSecondary);
            using (var brushText = new SolidBrush(textColor))
            {
                using var font = new Font("Segoe UI", 9F, isTop ? FontStyle.Bold : FontStyle.Regular);
                var textRect = new Rectangle(e.Bounds.Left + (isTop ? 10 : 8), e.Bounds.Top + 4, e.Bounds.Width - 14, e.Bounds.Height - 8);
                g.DrawString(texto, font, brushText, textRect);
            }

            using var penLine = new Pen(BorderCard, 1);
            g.DrawLine(penLine, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
        }
        #endregion
    }
}

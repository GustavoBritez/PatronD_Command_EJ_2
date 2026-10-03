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
        // =========================================================================
        // PALETA FUTURISTA: NEGRO PROFUNDO, BLANCO NÍTIDO Y PLATINO METÁLICO
        // =========================================================================
        private static readonly Color BgChassis         = Color.FromArgb(10, 11, 14);     // Negro titanio ultra profundo
        private static readonly Color BgRackCard        = Color.FromArgb(17, 19, 24);     // Grafito de cabina espacial
        private static readonly Color BgPlateHeader     = Color.FromArgb(24, 27, 34);     // Placa de titanio maquinado
        private static readonly Color BorderPlatinumDim = Color.FromArgb(70, 77, 90);     // Platino mate (borde pasivo)
        private static readonly Color BorderPlatinumHi  = Color.FromArgb(185, 193, 206);  // Platino cepillado brillante
        private static readonly Color RivetColor        = Color.FromArgb(120, 128, 142);  // Remaches metálicos de rack

        private static readonly Color TextWhiteCrisp    = Color.FromArgb(255, 255, 255);  // Blanco puro de alta legibilidad
        private static readonly Color TextPlatinum      = Color.FromArgb(195, 201, 212);  // Platino claro para lecturas
        private static readonly Color TextMutedCarbon   = Color.FromArgb(108, 115, 128);  // Gris carbón para anotaciones

        // BOTONES DE COLORES OPACOS (MATTE HARDWARE FINISH)
        private static readonly Color BtnMuteMatte      = Color.FromArgb(38, 48, 62);     // Azul pizarra mate opaco
        private static readonly Color BtnPitchMatte     = Color.FromArgb(48, 42, 58);     // Amatista humo mate opaco
        private static readonly Color BtnPowerMatte     = Color.FromArgb(36, 49, 44);     // Verde petróleo ceniza mate
        private static readonly Color BtnCutMatte       = Color.FromArgb(56, 45, 38);     // Bronce óxido mate opaco
        private static readonly Color BtnPedalMatte     = Color.FromArgb(28, 31, 38);     // Hierro forjado industrial
        private static readonly Color BtnPanicMatte     = Color.FromArgb(78, 26, 29);     // Granate / Óxido mate de emergencia

        // CAPA DE NEGOCIO Y HARDWARE (RECEPTORES)
        private readonly Transmisor_BLL _transmisor = new Transmisor_BLL();
        private readonly ProcesadorVoz_BLL _procesador = new ProcesadorVoz_BLL();
        private readonly Accion_BLL _accionBll = new Accion_BLL();

        // CAPA DE SERVICIO Y COMANDOS (INVOCADOR + PILA)
        private readonly Historial _historial = new Historial();
        private readonly ConsolaOperador _consola;

        // CONTROLES DE LA INTERFAZ
        private Button btnSlot1 = null!;
        private Button btnSlot2 = null!;
        private Button btnSlot3 = null!;
        private Button btnSlot4 = null!;
        private Button btnPedalSuelo = null!;
        private Button btnPanicPurge = null!;

        private Label lblTagSlot1 = null!;
        private Label lblTagSlot2 = null!;
        private Label lblTagSlot3 = null!;
        private Label lblTagSlot4 = null!;

        private Button btnModoManana = null!;
        private Button btnModoNoche = null!;

        private Label lblDisplayTxStatus = null!;
        private Label lblDisplayTxPower = null!;
        private Panel pnlSegmentedPowerMeter = null!;
        private Label lblDisplayDspPitch = null!;
        private Label lblDisplayDspGain = null!;

        private ListBox lstPilaComandos = null!;
        private Label lblProfundidadPila = null!;
        private Label lblTelemetryBar = null!;

        public Form1()
        {
            InitializeComponent();
            _consola = new ConsolaOperador(_historial, _accionBll);

            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            ConstruirConsolaTitaniumResponsive();
            ActivarModoManana();
            RefrescarTelemetria();

            KeyDown += Consola_KeyDown;
        }

        private void ConstruirConsolaTitaniumResponsive()
        {
            Controls.Clear();
            BackColor = BgChassis;
            ForeColor = TextWhiteCrisp;

            // =========================================================================
            // 1. HEADER RESPONSIVE: BANNER SUPERIOR DE TRANSMISIÓN
            // =========================================================================
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = BgPlateHeader
            };
            pnlHeader.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Marco inferior de platino brillante
                using var penBorder = new Pen(BorderPlatinumHi, 1);
                g.DrawLine(penBorder, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);

                // Acento metálico lateral
                using var brushBar = new SolidBrush(BorderPlatinumHi);
                g.FillRectangle(brushBar, 22, 16, 6, 52);

                // Remaches en las 4 esquinas responsivas
                DibujarRemache(g, 12, 12);
                DibujarRemache(g, pnlHeader.Width - 18, 12);
                DibujarRemache(g, 12, pnlHeader.Height - 18);
                DibujarRemache(g, pnlHeader.Width - 18, pnlHeader.Height - 18);
            };
            pnlHeader.Resize += (s, e) => pnlHeader.Invalidate();

            var lblBrand = new Label
            {
                Text = "RADIO FX 100 // TITANIUM BROADCAST MATRIX",
                Font = new Font("Consolas", 15F, FontStyle.Bold),
                ForeColor = TextWhiteCrisp,
                AutoSize = true,
                Location = new Point(40, 16)
            };

            var lblSubtitle = new Label
            {
                Text = "PATRÓN COMMAND: INVOCADOR FÍSICO ➔ ICOMANDO (CONTRATO) ➔ RECEPTORES DE AUDIO",
                Font = new Font("Consolas", 8.5F, FontStyle.Regular),
                ForeColor = TextPlatinum,
                AutoSize = true,
                Location = new Point(42, 48)
            };

            var lblTelemetryTag = new Label
            {
                Text = "STATUS: [ONLINE]\nFREQ:   100.1 MHz\nSYNC:   LOCKED",
                Font = new Font("Consolas", 8F, FontStyle.Bold),
                ForeColor = BorderPlatinumHi,
                TextAlign = ContentAlignment.TopRight,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(pnlHeader.Width - 180, 18)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblBrand, lblSubtitle, lblTelemetryTag });
            Controls.Add(pnlHeader);

            // =========================================================================
            // 2. STATUS HUD BAR (INFERIOR - RESPONSIVE)
            // =========================================================================
            lblTelemetryBar = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 38,
                BackColor = BgPlateHeader,
                ForeColor = TextPlatinum,
                Font = new Font("Consolas", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(28, 0, 0, 0),
                Text = "SYSTEM_READY >> Matriz adaptativa en espera. Presione [1..4], [P] Pedal, o [Ctrl+Z] Pánico."
            };
            lblTelemetryBar.Paint += (s, e) =>
            {
                using var p = new Pen(BorderPlatinumDim, 1);
                e.Graphics.DrawLine(p, 0, 0, lblTelemetryBar.Width, 0);
            };
            lblTelemetryBar.Resize += (s, e) => lblTelemetryBar.Invalidate();
            Controls.Add(lblTelemetryBar);

            // =========================================================================
            // 3. GRILLA PRINCIPAL RESPONSIVA (TABLE LAYOUT PANEL: 3 COLUMNAS ADAPTABLES)
            // =========================================================================
            var tblMainGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 12, 16, 12)
            };
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F)); // Col 1: Hardware Invokers
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F)); // Col 2: Audio Rack & Matrix
            tblMainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Col 3: Command Stack History
            tblMainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // -------------------------------------------------------------------------
            // COLUMNA 1: HARDWARE INVOKERS (MESA FÍSICA RESPONSIVA)
            // -------------------------------------------------------------------------
            var pnlInvokers = CrearRackChassis("MODULE 01 // OPERATOR HARDWARE INVOKERS");
            pnlInvokers.Dock = DockStyle.Fill;

            btnSlot1 = CrearBotonHardware("BTN [01] ➔ CANAL MUTE", BtnMuteMatte, 36);
            btnSlot1.Click += (s, e) => PresionarBoton(1);
            lblTagSlot1 = CrearLabelBinding(82);
            pnlInvokers.Controls.AddRange(new Control[] { btnSlot1, lblTagSlot1 });

            btnSlot2 = CrearBotonHardware("BTN [02] ➔ MODULADOR DSP PITCH", BtnPitchMatte, 122);
            btnSlot2.Click += (s, e) => PresionarBoton(2);
            lblTagSlot2 = CrearLabelBinding(168);
            pnlInvokers.Controls.AddRange(new Control[] { btnSlot2, lblTagSlot2 });

            btnSlot3 = CrearBotonHardware("BTN [03] ➔ REGULADOR POTENCIA RF", BtnPowerMatte, 208);
            btnSlot3.Click += (s, e) => PresionarBoton(3);
            lblTagSlot3 = CrearLabelBinding(254);
            pnlInvokers.Controls.AddRange(new Control[] { btnSlot3, lblTagSlot3 });

            btnSlot4 = CrearBotonHardware("BTN [04] ➔ CORTE DE EMERGENCIA", BtnCutMatte, 294);
            btnSlot4.Click += (s, e) => PresionarBoton(4);
            lblTagSlot4 = CrearLabelBinding(340);
            pnlInvokers.Controls.AddRange(new Control[] { btnSlot4, lblTagSlot4 });

            btnPedalSuelo = CrearBotonHardware("🦶 PEDAL DE SUELO [ATAJO RÁPIDO MUTE] (P)", BtnPedalMatte, 390);
            btnPedalSuelo.Height = 44;
            btnPedalSuelo.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            btnPedalSuelo.Click += (s, e) => PresionarBoton(1);
            pnlInvokers.Controls.Add(btnPedalSuelo);

            btnPanicPurge = CrearBotonHardware("🚨 BOTÓN DE PÁNICO // DESHACER (Ctrl+Z)", BtnPanicMatte, 456);
            btnPanicPurge.Height = 68;
            btnPanicPurge.Font = new Font("Consolas", 10F, FontStyle.Bold);
            btnPanicPurge.Click += (s, e) => PresionarPanico();
            pnlInvokers.Controls.Add(btnPanicPurge);

            tblMainGrid.Controls.Add(pnlInvokers, 0, 0);

            // -------------------------------------------------------------------------
            // COLUMNA 2: RECONFIGURACIÓN EN CALIENTE + RACK DE AUDIO (BLL)
            // -------------------------------------------------------------------------
            var tblCenterStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            tblCenterStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 126F));
            tblCenterStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Submódulo: Matriz de Reconfiguración Dinámica
            var pnlMatrixSwap = CrearRackChassis("MODULE 02 // HOT-SWAP MATRIX");
            pnlMatrixSwap.Dock = DockStyle.Fill;

            var lblSwapInfo = new Label
            {
                Text = "Reasigna la matriz de comandos dinámicamente:",
                Font = new Font("Consolas", 8F),
                ForeColor = TextMutedCarbon,
                Location = new Point(18, 28),
                AutoSize = true
            };

            var tblSwapButtons = new TableLayoutPanel
            {
                Location = new Point(18, 50),
                Size = new Size(pnlMatrixSwap.ClientSize.Width - 36, 54),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tblSwapButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblSwapButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            btnModoManana = new Button
            {
                Text = "🌅 TURNO MAÑANA\n[Noticias / Entrevistas]",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 5, 0),
                BackColor = Color.FromArgb(32, 36, 44),
                ForeColor = TextWhiteCrisp,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Consolas", 8.5F, FontStyle.Bold)
            };
            btnModoManana.FlatAppearance.BorderColor = BorderPlatinumHi;
            btnModoManana.FlatAppearance.BorderSize = 1;
            btnModoManana.Click += (s, e) => ActivarModoManana();

            btnModoNoche = new Button
            {
                Text = "🎧 TURNO NOCHE\n[Electrónica / DJ Set]",
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 0, 0),
                BackColor = Color.FromArgb(24, 27, 33),
                ForeColor = TextPlatinum,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Consolas", 8.5F, FontStyle.Bold)
            };
            btnModoNoche.FlatAppearance.BorderColor = BorderPlatinumDim;
            btnModoNoche.FlatAppearance.BorderSize = 1;
            btnModoNoche.Click += (s, e) => ActivarModoNoche();

            tblSwapButtons.Controls.Add(btnModoManana, 0, 0);
            tblSwapButtons.Controls.Add(btnModoNoche, 1, 0);

            pnlMatrixSwap.Controls.AddRange(new Control[] { lblSwapInfo, tblSwapButtons });
            tblCenterStack.Controls.Add(pnlMatrixSwap, 0, 0);

            // Submódulo: Rack de Audio BLL
            var pnlAudioRack = CrearRackChassis("MODULE 03 // AUDIO RECEIVERS RACK (BLL)");
            pnlAudioRack.Dock = DockStyle.Fill;
            pnlAudioRack.Margin = new Padding(0, 8, 0, 0);

            var lblTxHeader = new Label
            {
                Text = "▶ TRANSMISOR PRINCIPAL [Transmisor_BLL]:",
                Font = new Font("Consolas", 8.5F, FontStyle.Bold),
                ForeColor = TextPlatinum,
                Location = new Point(18, 30),
                AutoSize = true
            };

            lblDisplayTxStatus = new Label
            {
                Text = "● ON-AIR // TRANSMISIÓN ACTIVA",
                Font = new Font("Consolas", 10.5F, FontStyle.Bold),
                ForeColor = TextWhiteCrisp,
                BackColor = Color.FromArgb(24, 38, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(18, 52),
                Size = new Size(pnlAudioRack.ClientSize.Width - 36, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lblDisplayTxStatus.Paint += (s, e) =>
            {
                using var p = new Pen(BorderPlatinumDim, 1);
                e.Graphics.DrawRectangle(p, 0, 0, lblDisplayTxStatus.Width - 1, lblDisplayTxStatus.Height - 1);
            };

            lblDisplayTxPower = new Label
            {
                Text = "POTENCIA RF: 100 W [MAX BROADCAST]",
                Font = new Font("Consolas", 8.5F, FontStyle.Bold),
                ForeColor = TextPlatinum,
                Location = new Point(18, 98),
                AutoSize = true
            };

            // Vúmetro de Potencia Segmentado Adaptable
            pnlSegmentedPowerMeter = new Panel
            {
                Location = new Point(18, 120),
                Size = new Size(pnlAudioRack.ClientSize.Width - 36, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(12, 14, 18)
            };
            pnlSegmentedPowerMeter.Paint += (s, e) =>
            {
                var g = e.Graphics;
                int pot = _transmisor.ObtenerPotenciaActual();
                int totalSegments = 10;
                int activeSegments = (int)Math.Round((pot / 100.0) * totalSegments);
                int gap = 3;
                int segWidth = (pnlSegmentedPowerMeter.Width - (totalSegments * gap)) / totalSegments;

                for (int i = 0; i < totalSegments; i++)
                {
                    int x = i * (segWidth + gap);
                    bool active = i < activeSegments;
                    Color fillCol = active ? (i < 6 ? Color.FromArgb(180, 190, 205) : (i < 9 ? Color.FromArgb(220, 225, 235) : Color.White))
                                           : Color.FromArgb(26, 30, 38);

                    using var b = new SolidBrush(fillCol);
                    g.FillRectangle(b, x, 2, Math.Max(2, segWidth), pnlSegmentedPowerMeter.Height - 4);
                }
                using var pen = new Pen(BorderPlatinumDim, 1);
                g.DrawRectangle(pen, 0, 0, pnlSegmentedPowerMeter.Width - 1, pnlSegmentedPowerMeter.Height - 1);
            };
            pnlSegmentedPowerMeter.Resize += (s, e) => pnlSegmentedPowerMeter.Invalidate();

            var lblDspHeader = new Label
            {
                Text = "▶ PROCESADOR DIGITAL DE VOZ [ProcesadorVoz_BLL]:",
                Font = new Font("Consolas", 8.5F, FontStyle.Bold),
                ForeColor = TextPlatinum,
                Location = new Point(18, 154),
                AutoSize = true
            };

            lblDisplayDspPitch = new Label
            {
                Text = "○ DSP PITCH: BYPASS [VOZ NATURAL]",
                Font = new Font("Consolas", 10F, FontStyle.Bold),
                ForeColor = TextPlatinum,
                BackColor = Color.FromArgb(24, 27, 34),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(18, 176),
                Size = new Size(pnlAudioRack.ClientSize.Width - 36, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lblDisplayDspPitch.Paint += (s, e) =>
            {
                using var p = new Pen(BorderPlatinumDim, 1);
                e.Graphics.DrawRectangle(p, 0, 0, lblDisplayDspPitch.Width - 1, lblDisplayDspPitch.Height - 1);
            };

            lblDisplayDspGain = new Label
            {
                Text = "GANANCIA ENTRADA: 0 dB [NIVEL NOMINAL]",
                Font = new Font("Consolas", 8.5F),
                ForeColor = TextPlatinum,
                Location = new Point(18, 222),
                AutoSize = true
            };

            // Placa Técnica Informativa (Se estira verticalmente si se maximiza)
            var pnlTechNote = new Panel
            {
                Location = new Point(18, 252),
                Size = new Size(pnlAudioRack.ClientSize.Width - 36, Math.Max(120, pnlAudioRack.ClientSize.Height - 270)),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgPlateHeader
            };
            pnlTechNote.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderPlatinumDim, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, pnlTechNote.Width - 1, pnlTechNote.Height - 1);
            };
            pnlTechNote.Resize += (s, e) => pnlTechNote.Invalidate();

            var lblTechNoteTitle = new Label
            {
                Text = "ARQUITECTURA DE PATRÓN EN PIZARRÓN:",
                Font = new Font("Consolas", 8F, FontStyle.Bold),
                ForeColor = BorderPlatinumHi,
                Location = new Point(12, 10),
                AutoSize = true
            };
            var lblTechNoteBody = new Label
            {
                Text = "1. Invocador (Botón) ➔ solo dispara comando.ejecutar().\n2. IComando ➔ puente que delega al Transmisor/DSP.\n3. Historial (Stack) ➔ almacena el objeto ejecutado.\n4. Botón de Pánico ➔ desapila y llama a deshacer().\n\nCero métodos técnicos nuevos en la consola.",
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = TextPlatinum,
                Location = new Point(12, 32),
                Size = new Size(pnlTechNote.ClientSize.Width - 24, pnlTechNote.ClientSize.Height - 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlTechNote.Controls.AddRange(new Control[] { lblTechNoteTitle, lblTechNoteBody });

            pnlAudioRack.Controls.AddRange(new Control[]
            {
                lblTxHeader, lblDisplayTxStatus, lblDisplayTxPower, pnlSegmentedPowerMeter,
                lblDspHeader, lblDisplayDspPitch, lblDisplayDspGain, pnlTechNote
            });
            tblCenterStack.Controls.Add(pnlAudioRack, 0, 1);

            tblMainGrid.Controls.Add(tblCenterStack, 1, 0);

            // -------------------------------------------------------------------------
            // COLUMNA 3: REGISTRO LIFO DE COMANDOS (STACK<ICOMANDO>) - EXPANDIBLE
            // -------------------------------------------------------------------------
            var pnlStackCol = CrearRackChassis("MODULE 04 // LIFO COMMAND STACK (UNDO)");
            pnlStackCol.Dock = DockStyle.Fill;

            lblProfundidadPila = new Label
            {
                Text = "[PROFUNDIDAD DE PILA: 0 COMANDOS]",
                Font = new Font("Consolas", 8.5F, FontStyle.Bold),
                ForeColor = BorderPlatinumHi,
                Location = new Point(18, 30),
                AutoSize = true
            };

            // La lista de historial se estira tanto a lo ancho como a lo alto
            lstPilaComandos = new ListBox
            {
                Location = new Point(18, 56),
                Size = new Size(pnlStackCol.ClientSize.Width - 36, pnlStackCol.ClientSize.Height - 96),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgPlateHeader,
                ForeColor = TextWhiteCrisp,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.None,
                ItemHeight = 24
            };

            var lblStackHelp = new Label
            {
                Text = "Al pulsar PÁNICO se hace Pop() del tope y se restaura el receptor.",
                Font = new Font("Consolas", 7.5F, FontStyle.Italic),
                ForeColor = TextMutedCarbon,
                Location = new Point(18, pnlStackCol.ClientSize.Height - 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                AutoSize = true
            };

            pnlStackCol.Controls.AddRange(new Control[] { lblProfundidadPila, lstPilaComandos, lblStackHelp });
            tblMainGrid.Controls.Add(pnlStackCol, 2, 0);

            Controls.Add(tblMainGrid);
        }

        private Panel CrearRackChassis(string titulo)
        {
            var pnl = new Panel
            {
                BackColor = BgRackCard
            };

            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Marco exterior de platino mate
                using var penBorder = new Pen(BorderPlatinumDim, 1);
                g.DrawRectangle(penBorder, 0, 0, pnl.Width - 1, pnl.Height - 1);

                // Barra superior de la tarjeta (Chassis Plate)
                using var brushPlate = new SolidBrush(BgPlateHeader);
                g.FillRectangle(brushPlate, 1, 1, pnl.Width - 2, 26);
                g.DrawLine(penBorder, 0, 26, pnl.Width, 26);

                // Título grabado en la placa
                using var brushText = new SolidBrush(BorderPlatinumHi);
                using var fontHdr = new Font("Consolas", 8F, FontStyle.Bold);
                g.DrawString(titulo, fontHdr, brushText, 12, 6);

                // Remaches en las 4 esquinas responsivas
                DibujarRemache(g, 6, 6);
                DibujarRemache(g, pnl.Width - 12, 6);
                DibujarRemache(g, 6, pnl.Height - 12);
                DibujarRemache(g, pnl.Width - 12, pnl.Height - 12);
            };
            pnl.Resize += (s, e) => pnl.Invalidate();

            return pnl;
        }

        private static void DibujarRemache(Graphics g, int x, int y)
        {
            using var b = new SolidBrush(RivetColor);
            g.FillEllipse(b, x, y, 6, 6);
            using var p = new Pen(Color.FromArgb(50, 54, 64), 1);
            g.DrawEllipse(p, x, y, 6, 6);
        }

        private Button CrearBotonHardware(string texto, Color colorMatte, int top)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(18, top),
                Size = new Size(314, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = colorMatte,
                ForeColor = TextWhiteCrisp,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Consolas", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderColor = BorderPlatinumDim;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(colorMatte, 0.20f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(colorMatte, 0.20f);
            return btn;
        }

        private Label CrearLabelBinding(int top)
        {
            return new Label
            {
                Text = "BINDING: NONE",
                Location = new Point(20, top),
                Size = new Size(310, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ForeColor = TextMutedCarbon,
                Font = new Font("Consolas", 7.5F, FontStyle.Regular)
            };
        }

        // =========================================================================
        // RECONFIGURACIÓN DINÁMICA DE PROGRAMAS (COMMAND PATTERN EN ACCIÓN)
        // =========================================================================
        private void ActivarModoManana()
        {
            _consola.ConfigurarBoton(1, "Mutear Micrófonos", new ComandoModoMuteS(_transmisor));
            _consola.ConfigurarBoton(2, "Distorsión Voz (Anónimo)", new ComandoPitchS(_procesador));
            _consola.ConfigurarBoton(3, "Potencia Eco (50W)", new ComandoPotenciaS(_transmisor, 50));
            _consola.ConfigurarBoton(4, "Corte de Emergencia", new ComandoEmergenciaS(_transmisor));

            btnModoManana.BackColor = Color.FromArgb(36, 42, 54);
            btnModoManana.FlatAppearance.BorderColor = BorderPlatinumHi;
            btnModoManana.ForeColor = TextWhiteCrisp;

            btnModoNoche.BackColor = Color.FromArgb(20, 22, 28);
            btnModoNoche.FlatAppearance.BorderColor = BorderPlatinumDim;
            btnModoNoche.ForeColor = TextMutedCarbon;

            lblTelemetryBar.Text = "HOT-SWAP >> Matriz reconfigurada para TURNO MAÑANA [Entrevistas / Filtro Pitch / Potencia 50W].";
            RefrescarTelemetria();
        }

        private void ActivarModoNoche()
        {
            _consola.ConfigurarBoton(1, "Mutear Transmisión", new ComandoModoMuteS(_transmisor));
            _consola.ConfigurarBoton(2, "Filtro DJ Electrónica", new ComandoPitchS(_procesador));
            _consola.ConfigurarBoton(3, "Potencia Full (100W)", new ComandoPotenciaS(_transmisor, 100));
            _consola.ConfigurarBoton(4, "Corte de Emergencia", new ComandoEmergenciaS(_transmisor));

            btnModoNoche.BackColor = Color.FromArgb(36, 42, 54);
            btnModoNoche.FlatAppearance.BorderColor = BorderPlatinumHi;
            btnModoNoche.ForeColor = TextWhiteCrisp;

            btnModoManana.BackColor = Color.FromArgb(20, 22, 28);
            btnModoManana.FlatAppearance.BorderColor = BorderPlatinumDim;
            btnModoManana.ForeColor = TextMutedCarbon;

            lblTelemetryBar.Text = "HOT-SWAP >> Matriz reconfigurada para TURNO NOCHE [Música Electrónica / DJ Pitch / Potencia 100W].";
            RefrescarTelemetria();
        }

        private void PresionarBoton(int numero)
        {
            var boton = _consola.ObtenerBoton(numero);
            if (boton?.Comando != null)
            {
                string nombre = boton.Comando.nombre;
                _consola.PresionarBoton(numero);
                lblTelemetryBar.Text = $"EXECUTE >> Botón [{numero}] activó '{nombre}' a las {DateTime.Now:HH:mm:ss}";
                RefrescarTelemetria();
            }
        }

        private void PresionarPanico()
        {
            var deshecho = _consola.PresionarBotonPanico();
            if (deshecho != null)
            {
                lblTelemetryBar.Text = $"ROLLBACK >> Botón de Pánico restauró '{deshecho.nombre}' a las {DateTime.Now:HH:mm:ss}";
            }
            else
            {
                lblTelemetryBar.Text = "WARNING >> Pila LIFO vacía. No existen comandos en historial para deshacer.";
            }
            RefrescarTelemetria();
        }

        private void RefrescarTelemetria()
        {
            // 1. Asignaciones de los botones físicos
            lblTagSlot1.Text = $"BINDING: {_consola.ObtenerBoton(1)?.Comando?.nombre?.ToUpper() ?? "NONE"}";
            lblTagSlot2.Text = $"BINDING: {_consola.ObtenerBoton(2)?.Comando?.nombre?.ToUpper() ?? "NONE"}";
            lblTagSlot3.Text = $"BINDING: {_consola.ObtenerBoton(3)?.Comando?.nombre?.ToUpper() ?? "NONE"}";
            lblTagSlot4.Text = $"BINDING: {_consola.ObtenerBoton(4)?.Comando?.nombre?.ToUpper() ?? "NONE"}";

            // 2. Estado del Transmisor
            bool muteado = _transmisor.EstaMuteado();
            if (muteado)
            {
                lblDisplayTxStatus.Text = "■ SILENCIADO // MUTED";
                lblDisplayTxStatus.BackColor = Color.FromArgb(64, 26, 26);
                lblDisplayTxStatus.ForeColor = Color.FromArgb(250, 190, 190);
            }
            else
            {
                lblDisplayTxStatus.Text = "● ON-AIR // TRANSMISIÓN ACTIVA";
                lblDisplayTxStatus.BackColor = Color.FromArgb(24, 38, 30);
                lblDisplayTxStatus.ForeColor = Color.FromArgb(190, 250, 200);
            }

            int potencia = _transmisor.ObtenerPotenciaActual();
            lblDisplayTxPower.Text = $"POTENCIA RF: {potencia} W [{(potencia == 0 ? "OFF" : (potencia <= 50 ? "ECO-MODE" : "MAX-POWER"))}]";
            pnlSegmentedPowerMeter?.Invalidate();

            // 3. Estado del Procesador de Voz
            bool pitch = _procesador.EstaPitchActivo();
            if (pitch)
            {
                lblDisplayDspPitch.Text = "● DSP PITCH: ENGAGED [VOZ MODIFICADA]";
                lblDisplayDspPitch.BackColor = Color.FromArgb(48, 40, 60);
                lblDisplayDspPitch.ForeColor = Color.FromArgb(230, 210, 255);
            }
            else
            {
                lblDisplayDspPitch.Text = "○ DSP PITCH: BYPASS [VOZ NATURAL]";
                lblDisplayDspPitch.BackColor = BgPlateHeader;
                lblDisplayDspPitch.ForeColor = TextPlatinum;
            }

            lblDisplayDspGain.Text = $"GANANCIA ENTRADA: {_procesador.ObtenerGananciaActual()} dB [NIVEL NOMINAL]";

            // 4. Pila de Historial
            lstPilaComandos.Items.Clear();
            var lista = _historial.ObtenerTodos().ToList();
            lblProfundidadPila.Text = $"[PROFUNDIDAD DE PILA: {lista.Count} COMANDOS]";

            if (lista.Count == 0)
            {
                lstPilaComandos.Items.Add("  (Pila vacía // Esperando eventos)");
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    string prefijo = i == 0 ? "▶ [TOP] " : "  [ - ] ";
                    lstPilaComandos.Items.Add($"{prefijo}{lista[i].nombre}");
                }
            }

            lblDisplayTxStatus.Invalidate();
            lblDisplayDspPitch.Invalidate();
        }

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
                PresionarBoton(1); // Pedal de suelo
                e.Handled = true;
            }
        }
    }
}

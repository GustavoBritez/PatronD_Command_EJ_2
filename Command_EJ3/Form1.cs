using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Command_EJ3
{
    public partial class Form1 : Form
    {
        private static readonly Color BgForm            = Color.FromArgb(10, 15, 26);
        private static readonly Color BgCard            = Color.FromArgb(16, 25, 42);
        private static readonly Color BgCardHeader      = Color.FromArgb(22, 34, 56);
        private static readonly Color BgInnerPanel      = Color.FromArgb(12, 19, 32);
        private static readonly Color BorderCard        = Color.FromArgb(32, 52, 82);
        private static readonly Color BorderControl     = Color.FromArgb(42, 68, 106);

        private static readonly Color TextPrimary       = Color.FromArgb(240, 249, 255);
        private static readonly Color TextSecondary     = Color.FromArgb(165, 243, 252);
        private static readonly Color TextMuted         = Color.FromArgb(103, 132, 160);

        private static readonly Color BtnDefaultBg      = Color.FromArgb(20, 32, 54);
        private static readonly Color BtnDefaultHover   = Color.FromArgb(28, 45, 76);
        private static readonly Color BtnDefaultPress   = Color.FromArgb(16, 26, 44);

        private static readonly Color AccentCyan        = Color.FromArgb(6, 182, 212);
        private static readonly Color AccentCyanHover   = Color.FromArgb(8, 145, 178);
        private static readonly Color AccentCyanBorder  = Color.FromArgb(34, 211, 238);

        private static readonly Color ActiveStateBg     = Color.FromArgb(12, 74, 96);
        private static readonly Color ActiveStateBorder = Color.FromArgb(6, 182, 212);
        private static readonly Color ActiveStateText   = Color.FromArgb(207, 250, 254);

        private static readonly Color WarningStateBg    = Color.FromArgb(69, 26, 10);
        private static readonly Color WarningStateBorder= Color.FromArgb(245, 158, 11);
        private static readonly Color WarningStateText  = Color.FromArgb(254, 243, 199);

        private static readonly Color UndoBg            = Color.FromArgb(58, 20, 24);
        private static readonly Color UndoBorder        = Color.FromArgb(239, 68, 68);
        private static readonly Color UndoText          = Color.FromArgb(254, 202, 202);

        private readonly SistemaBalasto _balasto = new SistemaBalasto();
        private readonly BrazoMuestreo _brazo = new BrazoMuestreo();
        private readonly FocosAbisales _focos = new FocosAbisales();

        private readonly Historial _historial = new Historial();
        private readonly ConsolaBatiscafo _consola;

        private Button btnSlot1 = null!;
        private Button btnSlot2 = null!;
        private Button btnSlot3 = null!;
        private Button btnSlot4 = null!;
        private Button btnAtajoBrazo = null!;
        private Button btnDeshacer = null!;

        private Label lblTagSlot1 = null!;
        private Label lblTagSlot2 = null!;
        private Label lblTagSlot3 = null!;
        private Label lblTagSlot4 = null!;

        private Button btnModoNavegacion = null!;
        private Button btnModoMuestreo = null!;

        private Label lblDisplayProfundidad = null!;
        private Label lblDisplayPresion = null!;
        private Label lblDisplayBrazo = null!;
        private Label lblDisplayFocos = null!;
        private Panel pnlDepthMeter = null!;

        private ListBox lstPilaComandos = null!;
        private Label lblProfundidadPila = null!;
        private Label lblStatusBar = null!;

        ///PRE: Ninguno.
        ///POST: Inicializa los componentes, consola, interfaz y activa la misión inicial.
        public Form1()
        {
            InitializeComponent();
            _consola = new ConsolaBatiscafo(_historial);

            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            ConstruirInterfaz();
            ActivarPerfilNavegacion();
            RefrescarTelemetria();

            KeyDown += Consola_KeyDown;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Ensambla la jerarquía completa de controles de la cabina.
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
                using var penBorder = new Pen(BorderCard, 1);
                e.Graphics.DrawLine(penBorder, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
            pnlHeader.Resize += (s, e) => pnlHeader.Invalidate();

            var lblTitle = new Label
            {
                Text = "Consola de Mando de Batiscafo de Exploración Abisal",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(24, 12)
            };

            var lblSubtitle = new Label
            {
                Text = "Patrón Command • Invocador (Consola), IComando, Receptores (Balasto, Brazo, Focos) y Reversión LIFO",
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

            var lblMision = new Label
            {
                Text = "Inmersión: Fosa de las Marianas",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 16)
            };

            var lblCascoStatus = new Label
            {
                Text = "● Casco de Titanio Presurizado",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = AccentCyan,
                AutoSize = true,
                Location = new Point(20, 36)
            };

            pnlHeaderRight.Controls.AddRange(new Control[] { lblMision, lblCascoStatus });
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
                Text = "Listo • Use botones [1..4], tecla [B] para brazo robótico, o [Ctrl+Z] para revertir maniobra."
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

            var pnlCol1Card = CrearCardPanel("MANDOS DE PILOTAJE (INVOCADOR)");
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

            btnSlot1 = CrearBotonAccion("[1] Maniobrar Balasto", yPos, btnHeight);
            btnSlot1.Click += (s, e) => PresionarBoton(1);
            lblTagSlot1 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot1, lblTagSlot1 });

            yPos += slotGap;
            btnSlot2 = CrearBotonAccion("[2] Control de Proyectores", yPos, btnHeight);
            btnSlot2.Click += (s, e) => PresionarBoton(2);
            lblTagSlot2 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot2, lblTagSlot2 });

            yPos += slotGap;
            btnSlot3 = CrearBotonAccion("[3] Accionar Brazo Robótico", yPos, btnHeight);
            btnSlot3.Click += (s, e) => PresionarBoton(3);
            lblTagSlot3 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot3, lblTagSlot3 });

            yPos += slotGap;
            btnSlot4 = CrearBotonAccion("[4] Purga de Emergencia", yPos, btnHeight);
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
            btnAtajoBrazo = CrearBotonAccion("Accionamiento Rápido de Pinza (Tecla B)", yPos, 38);
            btnAtajoBrazo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnAtajoBrazo.Click += (s, e) => PresionarBoton(3);
            pnlCol1Content.Controls.Add(btnAtajoBrazo);

            yPos += 48;
            btnDeshacer = new Button
            {
                Text = "Revertir Maniobra Previa (Ctrl + Z)",
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
            btnDeshacer.Click += (s, e) => PresionarDeshacer();
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

            var pnlProfilesCard = CrearCardPanel("RÉGIMEN DE MISIÓN (PERFILES)");
            pnlProfilesCard.Dock = DockStyle.Fill;

            var lblProfileInfo = new Label
            {
                Text = "Configuración dinámica de mandos según fase de exploración:",
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

            btnModoNavegacion = new Button
            {
                Text = "Régimen Navegación / Sonar",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 4, 0),
                BackColor = AccentCyan,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoNavegacion.FlatAppearance.BorderColor = AccentCyanBorder;
            btnModoNavegacion.FlatAppearance.BorderSize = 1;
            btnModoNavegacion.Click += (s, e) => ActivarPerfilNavegacion();

            btnModoMuestreo = new Button
            {
                Text = "Régimen Muestreo en Fosa",
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 0, 0),
                BackColor = BtnDefaultBg,
                ForeColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoMuestreo.FlatAppearance.BorderColor = BorderControl;
            btnModoMuestreo.FlatAppearance.BorderSize = 1;
            btnModoMuestreo.Click += (s, e) => ActivarPerfilMuestreo();

            tblProfileButtons.Controls.Add(btnModoNavegacion, 0, 0);
            tblProfileButtons.Controls.Add(btnModoMuestreo, 1, 0);

            pnlProfilesCard.Controls.AddRange(new Control[] { lblProfileInfo, tblProfileButtons });
            tblCenterStack.Controls.Add(pnlProfilesCard, 0, 0);

            var pnlReceiversCard = CrearCardPanel("TELEMETRÍA DE SISTEMAS DEL BATISCAFO");
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

            var lblBalastoSection = new Label
            {
                Text = "Sistema de Balasto e Inmersión",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 4),
                AutoSize = true
            };

            lblDisplayProfundidad = new Label
            {
                Text = "Cota de Profundidad: 1500 m",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = ActiveStateText,
                BackColor = ActiveStateBg,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 26),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            pnlDepthMeter = new Panel
            {
                Location = new Point(0, 64),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 14),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgInnerPanel
            };
            pnlDepthMeter.Paint += (s, e) =>
            {
                var g = e.Graphics;
                int depth = _balasto.ObtenerProfundidad();
                float ratio = Math.Clamp(depth / 5000.0f, 0.0f, 1.0f);
                int fillWidth = (int)((pnlDepthMeter.Width - 2) * ratio);

                if (fillWidth > 0)
                {
                    Color col = _balasto.EstaEnFlotabilidadPositiva() ? WarningStateBorder : AccentCyan;
                    using var b = new SolidBrush(col);
                    g.FillRectangle(b, 1, 1, fillWidth, pnlDepthMeter.Height - 2);
                }
                using var p = new Pen(BorderCard, 1);
                g.DrawRectangle(p, 0, 0, pnlDepthMeter.Width - 1, pnlDepthMeter.Height - 1);
            };

            lblDisplayPresion = new Label
            {
                Text = "Presión Hidrostática Calculada: 150 atm",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                Location = new Point(0, 84),
                AutoSize = true
            };

            var lblLuzSection = new Label
            {
                Text = "Focos Abisales de Alta Presión",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 114),
                AutoSize = true
            };

            lblDisplayFocos = new Label
            {
                Text = "Régimen: Luz Estroboscópica Guía (Bajo Consumo)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSecondary,
                BackColor = BgInnerPanel,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 136),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblBrazoSection = new Label
            {
                Text = "Brazo Robótico de Muestreo",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 178),
                AutoSize = true
            };

            lblDisplayBrazo = new Label
            {
                Text = "Posición: Retraído y Bloqueado en Casco",
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSecondary,
                BackColor = BgInnerPanel,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 200),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            pnlReceiversContent.Controls.AddRange(new Control[]
            {
                lblBalastoSection, lblDisplayProfundidad, pnlDepthMeter, lblDisplayPresion,
                lblLuzSection, lblDisplayFocos,
                lblBrazoSection, lblDisplayBrazo
            });

            pnlReceiversCard.Controls.Add(pnlReceiversContent);
            tblCenterStack.Controls.Add(pnlReceiversCard, 0, 1);
            tblMainGrid.Controls.Add(tblCenterStack, 1, 0);

            var pnlStackCard = CrearCardPanel("BITÁCORA DE MANIOBRAS (LIFO)");
            pnlStackCard.Dock = DockStyle.Fill;

            lblProfundidadPila = new Label
            {
                Text = "Maniobras en pila: 0",
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
                Text = "Al pulsar Revertir se deshace la maniobra del tope y se restaura el batiscafo.",
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
        ///POST: Retorna un Panel configurado con cabecera y borde.
        private Panel CrearCardPanel(string titulo)
        {
            var pnl = new Panel { BackColor = BgCard };
            pnl.Paint += (s, e) =>
            {
                using var penBorder = new Pen(BorderCard, 1);
                e.Graphics.DrawRectangle(penBorder, 0, 0, pnl.Width - 1, pnl.Height - 1);

                using var brushHdr = new SolidBrush(BgCardHeader);
                e.Graphics.FillRectangle(brushHdr, 1, 1, pnl.Width - 2, 32);
                e.Graphics.DrawLine(penBorder, 0, 32, pnl.Width, 32);

                using var brushTxt = new SolidBrush(TextSecondary);
                using var fontHdr = new Font("Segoe UI", 8F, FontStyle.Bold);
                e.Graphics.DrawString(titulo, fontHdr, brushTxt, 14, 8);
            };
            pnl.Resize += (s, e) => pnl.Invalidate();
            return pnl;
        }

        ///PRE: Recibe texto (string) del botón, top (int) de posición y height (int) de altura.
        ///POST: Retorna un Button preconfigurado con estilo náutico plano.
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
        ///POST: Retorna una nueva instancia de Label para indicar el comando asignado.
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

        ///PRE: Recibe sender (object?) y e (DrawItemEventArgs) con los datos del evento de dibujado.
        ///POST: No retorna valor. Renderiza el elemento del ListBox con formato de pila LIFO.
        private void LstPilaComandos_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstPilaComandos.Items.Count) return;

            var g = e.Graphics;
            string texto = lstPilaComandos.Items[e.Index].ToString() ?? "";
            bool isTop = e.Index == 0 && _historial.Cantidad > 0;

            Color itemBg = isTop ? Color.FromArgb(18, 48, 70) : ((e.State & DrawItemState.Selected) != 0 ? Color.FromArgb(22, 38, 60) : BgInnerPanel);
            using (var brushBg = new SolidBrush(itemBg))
            {
                g.FillRectangle(brushBg, e.Bounds);
            }

            if (isTop)
            {
                using var bAccent = new SolidBrush(AccentCyanBorder);
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

        ///PRE: Ninguno.
        ///POST: No retorna valor. Reasigna los comandos del régimen de navegación y sonar al panel.
        private void ActivarPerfilNavegacion()
        {
            _consola.ConfigurarBoton(1, "Descenso a Media Profundidad (1500 m)", new ComandoInmersion(_balasto, 1500));
            _consola.ConfigurarBoton(2, "Focos de Prospección (5.000 lm)", new ComandoFocosAbisales(_focos, "Prospección Normal (5.000 lm • Haz Estrecho)"));
            _consola.ConfigurarBoton(3, "Bloquear Brazo (Modo Crucero)", new ComandoBrazoMuestreo(_brazo, "Retraído y Bloqueado en Casco"));
            _consola.ConfigurarBoton(4, "Purga de Emergencia", new ComandoPurgaEmergencia(_balasto, _brazo));

            btnModoNavegacion.BackColor = AccentCyan;
            btnModoNavegacion.FlatAppearance.BorderColor = AccentCyanBorder;
            btnModoNavegacion.ForeColor = TextPrimary;

            btnModoMuestreo.BackColor = BtnDefaultBg;
            btnModoMuestreo.FlatAppearance.BorderColor = BorderControl;
            btnModoMuestreo.ForeColor = TextSecondary;

            lblStatusBar.Text = "Régimen activo: Navegación / Sonar (Inmersión 1500 m • Focos 5.000 lm • Brazo en descanso).";
            RefrescarTelemetria();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Reasigna los comandos del régimen de muestreo geológico en lecho marino.
        private void ActivarPerfilMuestreo()
        {
            _consola.ConfigurarBoton(1, "Inmersión a Fosa Abisal (3800 m)", new ComandoInmersion(_balasto, 3800));
            _consola.ConfigurarBoton(2, "Reflectores Alta Potencia (20.000 lm)", new ComandoFocosAbisales(_focos, "Iluminación Fondo Marino (20.000 lm • Gran Angular)"));
            _consola.ConfigurarBoton(3, "Extender Pinza de Sedimentos", new ComandoBrazoMuestreo(_brazo, "Extendido con Garra Abierta (Listo para Extracción)"));
            _consola.ConfigurarBoton(4, "Purga de Emergencia", new ComandoPurgaEmergencia(_balasto, _brazo));

            btnModoMuestreo.BackColor = AccentCyan;
            btnModoMuestreo.FlatAppearance.BorderColor = AccentCyanBorder;
            btnModoMuestreo.ForeColor = TextPrimary;

            btnModoNavegacion.BackColor = BtnDefaultBg;
            btnModoNavegacion.FlatAppearance.BorderColor = BorderControl;
            btnModoNavegacion.ForeColor = TextSecondary;

            lblStatusBar.Text = "Régimen activo: Muestreo en Fosa (Inmersión 3800 m • Reflectores 20.000 lm • Pinza extendida).";
            RefrescarTelemetria();
        }

        ///PRE: Recibe numero (int) del botón accionado.
        ///POST: No retorna valor. Dispara el comando en la consola, actualiza telemetría y barra de estado.
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
        ///POST: No retorna valor. Deshace la última maniobra del historial restaurando los subsistemas afectados.
        private void PresionarDeshacer()
        {
            var deshecho = _consola.PresionarDeshacer();
            if (deshecho != null)
            {
                lblStatusBar.Text = $"Reversión: Se restauró el estado previo de '{deshecho.nombre}' a las {DateTime.Now:HH:mm:ss}";
            }
            else
            {
                lblStatusBar.Text = "Aviso: No hay maniobras en el historial para revertir.";
            }
            RefrescarTelemetria();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Sincroniza los indicadores de telemetría con el estado del batiscafo.
        private void RefrescarTelemetria()
        {
            lblTagSlot1.Text = $"Comando asignado: {_consola.ObtenerBoton(1)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot2.Text = $"Comando asignado: {_consola.ObtenerBoton(2)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot3.Text = $"Comando asignado: {_consola.ObtenerBoton(3)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot4.Text = $"Comando asignado: {_consola.ObtenerBoton(4)?.Comando?.nombre ?? "Ninguno"}";

            int depth = _balasto.ObtenerProfundidad();
            bool flotabilidad = _balasto.EstaEnFlotabilidadPositiva();

            if (flotabilidad)
            {
                lblDisplayProfundidad.Text = "▲ ASCENSO DE EMERGENCIA A SUPERFICIE (0 m)";
                lblDisplayProfundidad.BackColor = WarningStateBg;
                lblDisplayProfundidad.ForeColor = WarningStateText;
                lblDisplayPresion.Text = "Presión Hidrostática: 1 atm (Superficie / Flotabilidad Positiva)";
            }
            else
            {
                lblDisplayProfundidad.Text = $"Cota de Profundidad: {depth} m";
                lblDisplayProfundidad.BackColor = ActiveStateBg;
                lblDisplayProfundidad.ForeColor = ActiveStateText;
                lblDisplayPresion.Text = $"Presión Hidrostática Calculada: {(depth / 10) + 1} atm";
            }
            pnlDepthMeter?.Invalidate();

            lblDisplayFocos.Text = $"Régimen: {_focos.ObtenerModoLuz()}";
            lblDisplayBrazo.Text = $"Posición: {_brazo.ObtenerPosicion()}";

            lstPilaComandos.Items.Clear();
            var lista = _historial.ObtenerTodos().ToList();
            lblProfundidadPila.Text = $"Maniobras en pila: {lista.Count}";

            if (lista.Count == 0)
            {
                lstPilaComandos.Items.Add("(No hay maniobras en bitácora)");
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    string prefijo = i == 0 ? "▶ [Tope] " : $"   [{lista.Count - i}] ";
                    lstPilaComandos.Items.Add($"{prefijo}{lista[i].nombre}");
                }
            }
        }

        ///PRE: Recibe sender (object?) y e (KeyEventArgs) con los datos del evento de teclado.
        ///POST: No retorna valor. Captura teclas rápidas para maniobras náuticas o reversión.
        private void Consola_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                PresionarDeshacer();
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
            else if (e.KeyCode == Keys.B)
            {
                PresionarBoton(3);
                e.Handled = true;
            }
        }
    }
}

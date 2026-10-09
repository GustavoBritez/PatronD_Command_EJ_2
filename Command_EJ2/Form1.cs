using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Command_EJ2
{
    public partial class Form1 : Form
    {
        #region
        private static readonly Color BgForm            = Color.FromArgb(17, 22, 20);
        private static readonly Color BgCard            = Color.FromArgb(23, 30, 27);
        private static readonly Color BgCardHeader      = Color.FromArgb(29, 38, 34);
        private static readonly Color BgInnerPanel      = Color.FromArgb(19, 25, 22);
        private static readonly Color BorderCard        = Color.FromArgb(39, 52, 46);
        private static readonly Color BorderControl     = Color.FromArgb(49, 66, 58);

        private static readonly Color TextPrimary       = Color.FromArgb(240, 253, 244);
        private static readonly Color TextSecondary     = Color.FromArgb(167, 243, 208);
        private static readonly Color TextMuted         = Color.FromArgb(110, 140, 126);

        private static readonly Color BtnDefaultBg      = Color.FromArgb(30, 41, 36);
        private static readonly Color BtnDefaultHover   = Color.FromArgb(40, 56, 49);
        private static readonly Color BtnDefaultPress   = Color.FromArgb(24, 33, 29);

        private static readonly Color AccentGreen       = Color.FromArgb(16, 185, 129);
        private static readonly Color AccentGreenHover  = Color.FromArgb(5, 150, 105);
        private static readonly Color AccentGreenBorder = Color.FromArgb(52, 211, 153);

        private static readonly Color ActiveStateBg     = Color.FromArgb(6, 78, 59);
        private static readonly Color ActiveStateBorder = Color.FromArgb(16, 185, 129);
        private static readonly Color ActiveStateText   = Color.FromArgb(167, 243, 208);

        private static readonly Color WarningStateBg    = Color.FromArgb(69, 26, 10);
        private static readonly Color WarningStateBorder= Color.FromArgb(245, 158, 11);
        private static readonly Color WarningStateText  = Color.FromArgb(254, 243, 199);

        private static readonly Color UndoBg            = Color.FromArgb(58, 20, 24);
        private static readonly Color UndoBorder        = Color.FromArgb(239, 68, 68);
        private static readonly Color UndoText          = Color.FromArgb(254, 202, 202);

        private readonly Climatizador _climatizador = new Climatizador();
        private readonly SistemaRiego _riego = new SistemaRiego();
        private readonly IluminacionCultivo _iluminacion = new IluminacionCultivo();

        private readonly Historial _historial = new Historial();
        private readonly PanelInvernadero _panel;

        private Button btnSlot1 = null!;
        private Button btnSlot2 = null!;
        private Button btnSlot3 = null!;
        private Button btnSlot4 = null!;
        private Button btnPulsoRiego = null!;
        private Button btnDeshacer = null!;

        private Label lblTagSlot1 = null!;
        private Label lblTagSlot2 = null!;
        private Label lblTagSlot3 = null!;
        private Label lblTagSlot4 = null!;

        private Button btnModoVegetativo = null!;
        private Button btnModoFloracion = null!;

        private Label lblDisplayClima = null!;
        private Label lblDisplayCompuertas = null!;
        private Label lblDisplayRiego = null!;
        private Label lblDisplayLuz = null!;
        private Panel pnlTempMeter = null!;

        private ListBox lstPilaComandos = null!;
        private Label lblProfundidadPila = null!;
        private Label lblStatusBar = null!;
        private void RefrescarTelemetria()
        {
            lblTagSlot1.Text = $"Comando asignado: {_panel.ObtenerBoton(1)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot2.Text = $"Comando asignado: {_panel.ObtenerBoton(2)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot3.Text = $"Comando asignado: {_panel.ObtenerBoton(3)?.Comando?.nombre ?? "Ninguno"}";
            lblTagSlot4.Text = $"Comando asignado: {_panel.ObtenerBoton(4)?.Comando?.nombre ?? "Ninguno"}";

            int temp = _climatizador.ObtenerTemperatura();
            lblDisplayClima.Text = $"Temperatura del Domo: {temp} °C";
            pnlTempMeter?.Invalidate();

            bool compuertas = _climatizador.EstanCompuertasAbiertas();
            lblDisplayCompuertas.Text = compuertas ? "Compuertas de Ventilación: ABIERTAS (VENTILACIÓN FORZADA)" : "Compuertas de Ventilación: CERRADAS (ATMÓSFERA CONTROLADA)";
            lblDisplayCompuertas.ForeColor = compuertas ? WarningStateBorder : TextSecondary;

            bool riegoActivo = _riego.EstaRiegoActivo();
            if (riegoActivo)
            {
                lblDisplayRiego.Text = $"● Bomba Activa • Dosificando {_riego.ObtenerDosificacion()} ml de Nutrientes";
                lblDisplayRiego.BackColor = ActiveStateBg;
                lblDisplayRiego.ForeColor = ActiveStateText;
            }
            else
            {
                lblDisplayRiego.Text = "○ Bomba de Riego en Espera (0 ml)";
                lblDisplayRiego.BackColor = BgInnerPanel;
                lblDisplayRiego.ForeColor = TextSecondary;
            }

            lblDisplayLuz.Text = $"Régimen Lumínico: {_iluminacion.ObtenerEspectro()}";

            lstPilaComandos.Items.Clear();
            var lista = _historial.ObtenerTodos().ToList();
            lblProfundidadPila.Text = $"Comandos en pila: {lista.Count}";

            if (lista.Count == 0)
            {
                lstPilaComandos.Items.Add("(No hay operaciones registradas)");
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
                Text = "Controlador de Invernadero Hidropónico Automatizado",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(24, 12)
            };

            var lblSubtitle = new Label
            {
                Text = "Patrón Command • Invocador (Panel), IComando, Receptores (Clima, Riego, LED) y Reversión LIFO",
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

            var lblCultivo = new Label
            {
                Text = "Módulo de Cultivo: Hortalizas NFT",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 16)
            };

            var lblSensorStatus = new Label
            {
                Text = "● Telemetría en tiempo real",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = AccentGreen,
                AutoSize = true,
                Location = new Point(20, 36)
            };

            pnlHeaderRight.Controls.AddRange(new Control[] { lblCultivo, lblSensorStatus });
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
                Text = "Listo • Use botones [1..4], tecla [R] para riego rápido, o [Ctrl+Z] para deshacer."
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

            var pnlCol1Card = CrearCardPanel("ACTUADORES DEL PANEL (INVOCADOR)");
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

            btnSlot1 = CrearBotonAccion("[1] Dosificar Riego", yPos, btnHeight);
            btnSlot1.Click += (s, e) => PresionarBoton(1);
            lblTagSlot1 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot1, lblTagSlot1 });

            yPos += slotGap;
            btnSlot2 = CrearBotonAccion("[2] Ajustar Climatización", yPos, btnHeight);
            btnSlot2.Click += (s, e) => PresionarBoton(2);
            lblTagSlot2 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot2, lblTagSlot2 });

            yPos += slotGap;
            btnSlot3 = CrearBotonAccion("[3] Conmutar Espectro LED", yPos, btnHeight);
            btnSlot3.Click += (s, e) => PresionarBoton(3);
            lblTagSlot3 = CrearLabelDetalle(yPos + btnHeight + 4);
            pnlCol1Content.Controls.AddRange(new Control[] { btnSlot3, lblTagSlot3 });

            yPos += slotGap;
            btnSlot4 = CrearBotonAccion("[4] Ventilación de Emergencia", yPos, btnHeight);
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
            btnPulsoRiego = CrearBotonAccion("Inyección Rápida Nutrientes (Tecla R)", yPos, 38);
            btnPulsoRiego.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnPulsoRiego.Click += (s, e) => PresionarBoton(1);
            pnlCol1Content.Controls.Add(btnPulsoRiego);

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

            var pnlProfilesCard = CrearCardPanel("REGÍMENES DE CULTIVO (PERFILES)");
            pnlProfilesCard.Dock = DockStyle.Fill;

            var lblProfileInfo = new Label
            {
                Text = "Reconfiguración de comandos según etapa del cultivo:",
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

            btnModoVegetativo = new Button
            {
                Text = "Fase Vegetativa (Crecimiento)",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 4, 0),
                BackColor = AccentGreen,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoVegetativo.FlatAppearance.BorderColor = AccentGreenBorder;
            btnModoVegetativo.FlatAppearance.BorderSize = 1;
            btnModoVegetativo.Click += (s, e) => ActivarPerfilVegetativo();

            btnModoFloracion = new Button
            {
                Text = "Fase Floración (Fructificación)",
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 0, 0),
                BackColor = BtnDefaultBg,
                ForeColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnModoFloracion.FlatAppearance.BorderColor = BorderControl;
            btnModoFloracion.FlatAppearance.BorderSize = 1;
            btnModoFloracion.Click += (s, e) => ActivarPerfilFloracion();

            tblProfileButtons.Controls.Add(btnModoVegetativo, 0, 0);
            tblProfileButtons.Controls.Add(btnModoFloracion, 1, 0);

            pnlProfilesCard.Controls.AddRange(new Control[] { lblProfileInfo, tblProfileButtons });
            tblCenterStack.Controls.Add(pnlProfilesCard, 0, 0);

            var pnlReceiversCard = CrearCardPanel("TELEMETRÍA DE RECEPTORES AMBIENTALES");
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

            var lblClimaSection = new Label
            {
                Text = "Climatizador Ambiental",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 4),
                AutoSize = true
            };

            lblDisplayClima = new Label
            {
                Text = "Temperatura: 22 °C (Óptima)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = ActiveStateText,
                BackColor = ActiveStateBg,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 26),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            pnlTempMeter = new Panel
            {
                Location = new Point(0, 64),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 14),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = BgInnerPanel
            };
            pnlTempMeter.Paint += (s, e) =>
            {
                var g = e.Graphics;
                int temp = _climatizador.ObtenerTemperatura();
                float ratio = Math.Clamp((temp - 10) / 30.0f, 0.0f, 1.0f);
                int fillWidth = (int)((pnlTempMeter.Width - 2) * ratio);

                if (fillWidth > 0)
                {
                    using var b = new SolidBrush(AccentGreen);
                    g.FillRectangle(b, 1, 1, fillWidth, pnlTempMeter.Height - 2);
                }
                using var p = new Pen(BorderCard, 1);
                g.DrawRectangle(p, 0, 0, pnlTempMeter.Width - 1, pnlTempMeter.Height - 1);
            };

            lblDisplayCompuertas = new Label
            {
                Text = "Compuertas de Ventilación: CERRADAS",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                Location = new Point(0, 84),
                AutoSize = true
            };

            var lblRiegoSection = new Label
            {
                Text = "Sistema de Riego Hidropónico",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 114),
                AutoSize = true
            };

            lblDisplayRiego = new Label
            {
                Text = "○ Bomba de Solución Inactiva (0 ml)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSecondary,
                BackColor = BgInnerPanel,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 136),
                Size = new Size(pnlReceiversContent.ClientSize.Width, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblLuzSection = new Label
            {
                Text = "Iluminación Fotosintética LED",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, 178),
                AutoSize = true
            };

            lblDisplayLuz = new Label
            {
                Text = "Régimen Lumínico: Luz Natural (Standby)",
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
                lblClimaSection, lblDisplayClima, pnlTempMeter, lblDisplayCompuertas,
                lblRiegoSection, lblDisplayRiego,
                lblLuzSection, lblDisplayLuz
            });

            pnlReceiversCard.Controls.Add(pnlReceiversContent);
            tblCenterStack.Controls.Add(pnlReceiversCard, 0, 1);
            tblMainGrid.Controls.Add(tblCenterStack, 1, 0);

            var pnlStackCard = CrearCardPanel("HISTORIAL DE OPERACIONES (LIFO)");
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
                Text = "Al pulsar Deshacer se revierte el comando del tope y se restaura el invernadero.",
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
        ///POST: Retorna un nuevo Button con estilo plano configurado.
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
        ///POST: No retorna valor. Dibuja el elemento del ListBox con formato de pila LIFO.
        private void LstPilaComandos_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstPilaComandos.Items.Count) return;

            var g = e.Graphics;
            string texto = lstPilaComandos.Items[e.Index].ToString() ?? "";
            bool isTop = e.Index == 0 && _historial.Cantidad > 0;

            Color itemBg = isTop ? Color.FromArgb(20, 46, 35) : ((e.State & DrawItemState.Selected) != 0 ? Color.FromArgb(28, 40, 34) : BgInnerPanel);
            using (var brushBg = new SolidBrush(itemBg))
            {
                g.FillRectangle(brushBg, e.Bounds);
            }

            if (isTop)
            {
                using var bAccent = new SolidBrush(AccentGreenBorder);
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
        public Form1()
        {
            InitializeComponent();
            _panel = new PanelInvernadero(_historial);

            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            ConstruirInterfaz();
            ActivarPerfilVegetativo();
            RefrescarTelemetria();

            KeyDown += Panel_KeyDown;
        }


        private void ActivarPerfilVegetativo()
        {
            _panel.ConfigurarBoton(1, "Riego de Crecimiento (150 ml)", new ComandoRiegoNutrientes(_riego, 150));
            _panel.ConfigurarBoton(2, "Clima Templado (22 °C)", new ComandoTemperatura(_climatizador, 22));
            _panel.ConfigurarBoton(3, "Espectro Azul (Fase Vegetativa)", new ComandoEspectroLuz(_iluminacion, "Azul (450 nm • Crecimiento Vegetativo)"));
            _panel.ConfigurarBoton(4, "Ventilación de Emergencia", new ComandoVentilacionEmergencia(_climatizador, _riego));

            btnModoVegetativo.BackColor = AccentGreen;
            btnModoVegetativo.FlatAppearance.BorderColor = AccentGreenBorder;
            btnModoVegetativo.ForeColor = TextPrimary;

            btnModoFloracion.BackColor = BtnDefaultBg;
            btnModoFloracion.FlatAppearance.BorderColor = BorderControl;
            btnModoFloracion.ForeColor = TextSecondary;

            lblStatusBar.Text = "Régimen activo: Fase Vegetativa (Dosis 150 ml • Clima 22 °C • Espectro Azul 450 nm).";
            RefrescarTelemetria();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Reasigna los comandos de la fase de floración a los botones del panel.
        private void ActivarPerfilFloracion()
        {
            _panel.ConfigurarBoton(1, "Riego Intensivo (300 ml)", new ComandoRiegoNutrientes(_riego, 300));
            _panel.ConfigurarBoton(2, "Clima Cálido (26 °C)", new ComandoTemperatura(_climatizador, 26));
            _panel.ConfigurarBoton(3, "Espectro Rojo (Fase Floración)", new ComandoEspectroLuz(_iluminacion, "Rojo Profundo (660 nm • Inducción Floral)"));
            _panel.ConfigurarBoton(4, "Ventilación de Emergencia", new ComandoVentilacionEmergencia(_climatizador, _riego));

            btnModoFloracion.BackColor = AccentGreen;
            btnModoFloracion.FlatAppearance.BorderColor = AccentGreenBorder;
            btnModoFloracion.ForeColor = TextPrimary;

            btnModoVegetativo.BackColor = BtnDefaultBg;
            btnModoVegetativo.FlatAppearance.BorderColor = BorderControl;
            btnModoVegetativo.ForeColor = TextSecondary;

            lblStatusBar.Text = "Régimen activo: Fase Floración (Dosis 300 ml • Clima 26 °C • Espectro Rojo 660 nm).";
            RefrescarTelemetria();
        }

        ///PRE: Recibe numero (int) del botón accionado.
        ///POST: No retorna valor. Ejecuta el comando en el panel, actualiza la barra de estado y refresca la vista.
        private void PresionarBoton(int numero)
        {
            var boton = _panel.ObtenerBoton(numero);
            if (boton?.Comando != null)
            {
                string nombre = boton.Comando.nombre;
                _panel.PresionarBoton(numero);
                lblStatusBar.Text = $"Acción: Botón [{numero}] ejecutó '{nombre}' a las {DateTime.Now:HH:mm:ss}";
                RefrescarTelemetria();
            }
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Deshace la última acción del historial y restaura los receptores afectados.
        private void PresionarDeshacer()
        {
            var deshecho = _panel.PresionarDeshacer();
            if (deshecho != null)
            {
                lblStatusBar.Text = $"Reversión: Se restauró el estado previo de '{deshecho.nombre}' a las {DateTime.Now:HH:mm:ss}";
            }
            else
            {
                lblStatusBar.Text = "Aviso: No hay operaciones en el historial para deshacer.";
            }
            RefrescarTelemetria();
        }

        ///PRE: Recibe sender (object?) y e (KeyEventArgs) con los datos de pulsación de teclado.
        ///POST: No retorna valor. Captura teclas rápidas para accionar comandos o deshacer.
        private void Panel_KeyDown(object? sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.R)
            {
                PresionarBoton(1);
                e.Handled = true;
            }
        }
    }
}

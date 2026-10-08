namespace Command
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        ///PRE: Recibe disposing (bool) indicando si los recursos administrados deben liberarse.
        ///POST: No retorna valor. Libera los recursos del formulario.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Inicializa las propiedades del formulario.
        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(17, 20, 24);
            ClientSize = new Size(1180, 780);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.FromArgb(248, 250, 252);
            FormBorderStyle = FormBorderStyle.Sizable;
            KeyPreview = true;
            MaximizeBox = true;
            MinimumSize = new Size(1020, 680);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Consola de Transmisión de Audio - Patrón Command";
            ResumeLayout(false);
        }
    }
}

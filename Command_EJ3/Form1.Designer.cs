namespace Command_EJ3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        ///PRE: Recibe disposing (bool) indicando si los recursos administrados deben liberarse.
        ///POST: No retorna valor. Libera los componentes y recursos del formulario.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Inicializa las propiedades básicas del formulario.
        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 15, 24);
            ClientSize = new Size(1180, 780);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.FromArgb(240, 249, 255);
            FormBorderStyle = FormBorderStyle.Sizable;
            KeyPreview = true;
            MaximizeBox = true;
            MinimumSize = new Size(1020, 680);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Consola de Mando de Batiscafo Abisal - Patrón Command";
            ResumeLayout(false);
        }
    }
}

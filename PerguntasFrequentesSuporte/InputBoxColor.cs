using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class InputBoxColor : Form
    {
        public Color CorSelecionada;

        public InputBoxColor()
        {
            InitializeComponent();
            InitializeComponents();
            Text = "Seletor de Cores";
        }
        public static Color Show(string Mensagem, Color CorAntiga)
        {
            foreach (Form form in Application.OpenForms) // Esconde o menu
            {
                if (form is Menu)
                    form.Hide();
            }

            InputBoxColor formsColor = new InputBoxColor();
            formsColor.Text = Mensagem;
            formsColor.CorSelecionada = CorAntiga;
            formsColor.ShowDialog();
            foreach (Form form in Application.OpenForms)  // Mostra novamente o menu
            {
                if (form is Menu)
                    form.Show();
            }

            return formsColor.CorSelecionada;
        }
        // Controles
        private Bitmap colorWheel;
        private Point selectedPoint = Point.Empty;
        private bool isDragging = false; // Controla o arrasto
        private List<string> systemColors;

        private void InitializeComponents()
        {
            // Obter cores do sistema
            systemColors = typeof(Color)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Select(c => c.Name)
                .ToList();

            // Criar a roda de cores ao iniciar
            colorWheel = GenerateColorWheel(pictureBox.Width, pictureBox.Height);

            // Definir a região da PictureBox como um círculo
            SetCircularRegion();

            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(systemColors.ToArray());
            textBoxColor.AutoCompleteCustomSource = autoComplete;
        }
        private void SetCircularRegion()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, pictureBox.Width, pictureBox.Height);
            pictureBox.Region = new Region(path);
        }
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(colorWheel, 0, 0, pictureBox.Width, pictureBox.Height);

            if (selectedPoint != Point.Empty)
            {
                using (SolidBrush brush = new SolidBrush(Color.Black))
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    int circleSize = 10;
                    e.Graphics.FillEllipse(brush, selectedPoint.X - circleSize / 2, selectedPoint.Y - circleSize / 2, circleSize, circleSize);
                    e.Graphics.DrawEllipse(pen, selectedPoint.X - circleSize / 2, selectedPoint.Y - circleSize / 2, circleSize, circleSize);
                }
            }
        }
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsInsideCircle(e.X, e.Y))
            {
                isDragging = true;
                UpdateColorSelection(e.X, e.Y);
            }
        }
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && IsInsideCircle(e.X, e.Y))
                UpdateColorSelection(e.X, e.Y);
        }
        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
        private bool IsInsideCircle(int x, int y)
        {
            int centerX = pictureBox.Width / 2;
            int centerY = pictureBox.Height / 2;
            int radius = Math.Min(pictureBox.Width, pictureBox.Height) / 2;

            double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
            return distance <= radius;
        }
        private Bitmap GenerateColorWheel(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                Point center = new Point(width / 2, height / 2);
                int radius = Math.Min(width, height) / 2;

                for (int y = -radius; y < radius; y++)
                {
                    for (int x = -radius; x < radius; x++)
                    {
                        double distance = Math.Sqrt(x * x + y * y);
                        if (distance <= radius)
                        {
                            double angle = Math.Atan2(y, x) * 180 / Math.PI;
                            if (angle < 0) angle += 360;
                            Color color = HsvToRgb(angle, distance / radius, 1);
                            bmp.SetPixel(center.X + x, center.Y + y, color);
                        }
                    }
                }
            }
            return bmp;
        }

        private void UpdateColorSelection(int x, int y)
        {
            if (x >= 0 && x < colorWheel.Width && y >= 0 && y < colorWheel.Height)
            {
                selectedPoint = new Point(x, y);
                Color selectedColor = colorWheel.GetPixel(x, y);

                trackBarR.Value = selectedColor.R;
                trackBarG.Value = selectedColor.G;
                trackBarB.Value = selectedColor.B;

                UpdateLabels();
                UpdatePreview();
                pictureBox.Invalidate();
            }
        }
        private void TrackBar_ValueChanged(object sender, EventArgs e)  // Atualiza as labels conforme os TrackBars são ajustados
        {
            UpdateLabels();
            UpdatePreview();
        }
        private void UpdateLabels() // Atualiza os valores de RGB e Alpha
        {
            labelR.Text = $"R: {trackBarR.Value}";
            labelG.Text = $"G: {trackBarG.Value}";
            labelB.Text = $"B: {trackBarB.Value}";
        }
        private void UpdatePreview()
        {
            panelPreview.BackColor = Color.FromArgb(trackBarR.Value, trackBarG.Value, trackBarB.Value);
        } // Atualiza a pré-visualização da cor
        private Color HsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;
            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return Color.FromArgb((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
        } // Converte HSV para RGB
		private void BtnConfirm_Click(object sender, EventArgs e)
		{
            //SelectedColor = Color.FromArgb(trackBarR.Value, trackBarG.Value, trackBarB.Value);
            CorSelecionada = panelPreview.BackColor;
			Close();
		}
		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
        private void textBoxColor_Leave(object sender, EventArgs e)
        {
            if (systemColors.Contains(textBoxColor.Text))
            {
                Color selectedColor = Color.FromName(textBoxColor.Text);

                // Atualizar os TrackBars com os valores da cor selecionada
                trackBarR.Value = selectedColor.R;
                trackBarG.Value = selectedColor.G;
                trackBarB.Value = selectedColor.B;

                // Atualizar os rótulos das cores
                labelR.Text = $"R: {trackBarR.Value}";
                labelG.Text = $"G: {trackBarG.Value}";
                labelB.Text = $"B: {trackBarB.Value}";

                // Atualizar a pré-visualização da cor
                panelPreview.BackColor = selectedColor;
                UpdatePreview();
            }
        }
        private Size lastSize = Size.Empty;
        private Timer resizeTimer;
        // Subprograma auxiliar para ativar DoubleBuffered em controles

        private void pictureBox_SizeChanged_1(object sender, EventArgs e)
        {
            if (resizeTimer == null)
            {
                resizeTimer = new Timer();
                resizeTimer.Interval = 150; // Espera 100ms antes de redimensionar
                resizeTimer.Tick += (s, ev) =>
                {
                    resizeTimer.Stop();
                   // pictureBox.Dock = DockStyle.Fill;
                    if (pictureBox.Size != lastSize)
                    {
                        lastSize = pictureBox.Size;
                        colorWheel = GenerateColorWheel(pictureBox.Width, pictureBox.Height);
                        SetCircularRegion();
                       // pictureBox.Dock = DockStyle.None;
                        pictureBox.Invalidate();
                    }
                };
            }
            resizeTimer.Stop();
            resizeTimer.Start();
        }
    }
}

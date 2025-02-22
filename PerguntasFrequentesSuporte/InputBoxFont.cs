using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Text;

namespace PerguntasFrequentesSuporte
{
    public partial class InputBoxFont : Form
    {
        private Font FonteInicial;
        private Font FonteAtual;

        public InputBoxFont(Font fonteInicial)
        {
            InitializeComponent();
            FonteInicial = fonteInicial;
            FonteAtual = fonteInicial;
        }

        private void InputBoxFont_Load(object sender, EventArgs e)
        {
            ListarFontes();
            PreencherEstilosFonte(ComboBoxEstilo);
            ComboBoxFontes.SelectedItem = FonteAtual;

            NumTamanhoFonte.Minimum = 6;
            NumTamanhoFonte.Maximum = 100;
            NumTamanhoFonte.Value = (decimal)FonteAtual.Size;
            ComboBoxEstilo.SelectedItem = FonteAtual.Style.ToString();
            AtualizarFonte();
            //Scale(10f);
        }

        private void ListarFontes()
        {
            InstalledFontCollection fontes = new InstalledFontCollection();
            foreach (FontFamily font in fontes.Families)
            {
                ComboBoxFontes.Items.Add(font.Name);
            }
        }

        private void NumTamanhoFonte_ValueChanged(object sender, EventArgs e)
        {
            FonteAtual = new Font(FonteAtual.FontFamily, (float)NumTamanhoFonte.Value, FonteAtual.Style);
            AtualizarFonte();
        }

        private void ComboBoxFontes_SelectedIndexChanged(object sender, EventArgs e)
        {
            FonteAtual = new Font(ComboBoxFontes.SelectedItem.ToString(), FonteAtual.Size, FonteAtual.Style);
            AtualizarFonte();
        }

        private void ComboBoxEstilo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse(ComboBoxEstilo.SelectedItem.ToString(), out FontStyle estilo))
            {
                FonteAtual = new Font(FonteAtual.FontFamily, FonteAtual.Size, estilo);
                AtualizarFonte();
            }
        }
        public void ComboBoxFontes_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                string fontName = ComboBoxFontes.Items[e.Index].ToString();
                try
                {
                    e.Graphics.DrawString(fontName, new Font(fontName, 11), Brushes.Black, e.Bounds);
                }
                catch
                {
                    e.Graphics.DrawString(fontName, ComboBoxFontes.Font, Brushes.Black, e.Bounds);
                }
                e.DrawFocusRectangle();
            }
        }
        private void AtualizarFonte()
        {
            TxtBoxExemplo.Font.Dispose();
            TxtBoxExemplo.Font = FonteAtual;
        }

        public static Font Show(string mensagem, Font fonteAtual)
        {
            InputBoxFont form = new InputBoxFont(fonteAtual);
            form.Text = mensagem;

            foreach (Form FormMenu in Application.OpenForms)  // Mostra novamente o menu
            {
                if (FormMenu is Menu)
                    FormMenu.Hide();
            }
            form.ShowDialog();
            foreach (Form FormMenu in Application.OpenForms)  // Mostra novamente o menu
            {
                if (FormMenu is Menu)
                    FormMenu.Show();
            }
            return form.FonteInicial;
        }
        private void PreencherEstilosFonte(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (FontStyle estilo in Enum.GetValues(typeof(FontStyle)))
            {
                comboBox.Items.Add(estilo.ToString());
            }
            comboBox.SelectedIndex = 0; // Define um valor padrão
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FonteInicial = FonteAtual;
            FonteInicial = new Font(FonteAtual.Name, FonteAtual.Size, FonteAtual.Style);
            Close();
            Dispose();
        }
    }
}

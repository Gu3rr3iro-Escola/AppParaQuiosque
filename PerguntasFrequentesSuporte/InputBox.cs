using System;
using System.Drawing;
using System.Windows.Forms;



namespace PerguntasFrequentesSuporte
{
    public partial class InputBox : Form
    {
        // Declara uma instância da classe para as cores
        public InputBox()
        {
            InitializeComponent();
        }
        /*public static Color? InputCor()
        {
            using (InputBoxColor form = new InputBoxColor())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    return form;
                return null;
            }
        }
    */

        // E no Subprograma de ProcessarDado, quando o tipo for Color:
        public void ProcessarDado(object dadoAtual)
        {
            if (dadoAtual == null)
            {
                MessageBox.Show("Dado inválido!");
                return;
            }

            AbasInput.Enabled = false;

            switch (dadoAtual)
            {
                case bool b:
                    AbasInput.SelectedTab = Bool;
                    //ProcessarBooleano(b);
                    break;
                case string s:
                    AbasInput.SelectedTab = tabPage2;
                    //ProcessarString(s);
                    break;
                case DateTime dt:
                    AbasInput.SelectedTab = InputData;
                    //ProcessarData(dt);
                    break;
                default:
                    MessageBox.Show("Tipo de dado não suportado.");
                    break;
            }
        }
        private void AbasInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AbasInput.SelectedTab != null && AbasInput.SelectedTab.Tag != null)
            {
                // Supomos que a propriedade Tag da TabPage tenha o formato "largura;altura"
                string tag = AbasInput.SelectedTab.Tag.ToString();
                string[] partes = tag.Split(';');
                if (partes.Length == 2 && int.TryParse(partes[0], out int largura) && int.TryParse(partes[1], out int altura))
                {
                    // Ajusta o tamanho da aba
                    AbasInput.SelectedTab.Size = new Size(largura, altura);

                    // Se quiseres ajustar também o tamanho do form:
                    int margemLargura = this.Width - AbasInput.Width;
                    int margemAltura = this.Height - AbasInput.Height;
                    this.ClientSize = new Size(largura + margemLargura, altura + margemAltura);
                }
            }
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            AbasInput.Appearance = TabAppearance.FlatButtons;
            AbasInput.ItemSize = new Size(0, 1);
            AbasInput.SizeMode = TabSizeMode.Fixed;
        }
        // Resto do código do InputBox...
    }
}

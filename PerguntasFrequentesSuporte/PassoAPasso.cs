using System;
using System.Drawing;
using System.Windows.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class PassoAPasso : Form
    {
        public PassoAPasso()
        {
            InitializeComponent();
        }
        public Image[][] ListaTotal;// Array 0=Btn / 1=Passo
        public int CategoriaAtual = new int();
        public int PassoAtual = new int(); 

        private void btnProximo_Click(object sender, EventArgs e)
        {
            if (PassoAtual + 1 < ListaTotal[CategoriaAtual].Length)
                PassoAtual++;
            AtualizarImagem();
        }
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (CategoriaAtual > 0)
                PassoAtual--;
            AtualizarImagem();
        }
        private void Reiniciar(int? dispositivo)
        {
            if (dispositivo.HasValue) // Verifica se dispositivo contém um valor
            {
                CategoriaAtual = dispositivo.Value; // Usa .Value para obter o int
                PassoAtual = 0;
                AtualizarImagem();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void ExplicacaoWifi_Load(object sender, EventArgs e) {}
        public void AtualizarImagem() // Atualiza a imagem exibida
        {
            bool temMultiplasImagens = ListaTotal[CategoriaAtual].Length > 1;
            btnAnterior.Enabled = !(temMultiplasImagens && PassoAtual > 0);
            btnProximo.Enabled = !(temMultiplasImagens && PassoAtual < ListaTotal[CategoriaAtual].Length - 1);

            picBox.Image = ListaTotal[CategoriaAtual][PassoAtual];
            txtPasso.Text = $"{PassoAtual+1}/{ListaTotal[CategoriaAtual].Length}";
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Name.Length > 0)
                Reiniciar(ClasseAuxiliar.ExtrairNumeroFinal(btn.Name));
        }
    }
}

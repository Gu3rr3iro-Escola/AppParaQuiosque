using System;
using System.IO;
using System.Windows.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class InputBoxImagem : Form
    {
        private string caminhoOriginal;
        private string caminhoImagemAtual;
        private string novaImagem;
        private string pastaDestino = Path.Combine(Ficheiros.Caminho, "Imagens");
        public InputBoxImagem(string caminhoAtual)
        {
            InitializeComponent();
            caminhoImagemAtual = caminhoAtual;
            caminhoOriginal = caminhoAtual;
            textBox1.Text = caminhoAtual ?? "";
        }
        // Subprograma estático Show que cria a instância do formulário, esconde/mostra o menu e retorna o resultado
        public static string Show(string Mensagem, string CaminhoAntigo)
        {
            // Esconde o menu
            foreach (Form form in Application.OpenForms)
            {
                if (form is Menu)
                    form.Hide();
            }

            // Cria o formulário e define o título (mensagem) e o caminho atual
            InputBoxImagem formImagem = new InputBoxImagem(CaminhoAntigo);
            formImagem.Text = Mensagem;
            formImagem.ShowDialog();

            // Mostra novamente o menu
            foreach (Form form in Application.OpenForms)
            {
                if (form is Menu)
                    form.Show();
            }

            return formImagem.novaImagem; // Pode ser null se o utilizador cancelar ou retirar a imagem
        }
        private void BtnEscolherCaminho_Click(object sender, EventArgs e)
        {
            OpenFile.Filter = "Imagens (*.jpg;*.jpeg;*.png;*.ico)|*.jpg;*.jpeg;*.png;*.ico|Todos os arquivos (*.*)|*.*";
            OpenFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            OpenFile.Title = "Escolher Imagem";

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = OpenFile.FileName;
                VerificarCaminho();
            }
        }
        private void VerificarCaminho()
        {
            string caminho = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(caminho))
                return;

            if (!File.Exists(caminho))
            {
                MessageBox.Show("O caminho da imagem não é válido ou a imagem não existe.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = caminhoImagemAtual ?? "";
                return;
            }
        }
        // Evento do botão Confirmar
        private void Confirmar_Click(object sender, EventArgs e)
        {
            string caminho = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(caminho))
            {
                // Se o usuário limpar o caminho, pergunta se deseja apagar a imagem antiga
                if (!string.IsNullOrEmpty(caminhoImagemAtual) && PerguntarApagarImagem())
                {
                    string destinoAntigo = Path.Combine(pastaDestino, Path.GetFileName(caminhoImagemAtual));
                    if (File.Exists(destinoAntigo))
                        File.Delete(destinoAntigo);
                }
                caminho = null;
            }
            else
            {
                if (!File.Exists(caminho))
                {
                    MessageBox.Show("O caminho informado não é válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string nomeImagem = Path.GetFileName(caminho);
                string caminhoDestino = Path.Combine(pastaDestino, nomeImagem);

                // Se a imagem já existe no destino, pergunta se deseja substituir
                if (File.Exists(caminhoDestino))
                {
                    DialogResult result = MessageBox.Show(
                        $"A imagem \"{nomeImagem}\" já existe. Deseja substituí-la?",
                        "Imagem Existente", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        return;
                    else if (result == DialogResult.Yes)
                        File.Copy(caminho, caminhoDestino, true);
                    // Se No, mantém o arquivo existente (caminhoDestino)
                }
                else
                {
                    File.Copy(caminho, caminhoDestino);
                }

                // Se já houver uma imagem antiga diferente, pergunta se deseja apagá-la
                if (!string.IsNullOrEmpty(caminhoImagemAtual) && caminhoImagemAtual != nomeImagem && PerguntarApagarImagem())
                {
                    string destinoAntigo = Path.Combine(pastaDestino, Path.GetFileName(caminhoImagemAtual));
                    if (File.Exists(destinoAntigo))
                        File.Delete(destinoAntigo);
                }

                caminho = nomeImagem;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        private void RemoverImagem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(caminhoImagemAtual) && PerguntarApagarImagem())
            {
                File.Delete(Path.Combine(pastaDestino, caminhoImagemAtual));
            }

            novaImagem = null;
            DialogResult = DialogResult.OK;
            Close();
        }
        private bool PerguntarApagarImagem()
        {
            return MessageBox.Show(
                "Deseja apagar a imagem guardada?",
                "Apagar Imagem", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }
        private void textBox1_DragLeave(object sender, EventArgs e)
        {
            VerificarCaminho();
        }
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                VerificarCaminho();
                Confirmar_Click(null, null);
            }
        }
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            VerificarCaminho();
        }
    }
}

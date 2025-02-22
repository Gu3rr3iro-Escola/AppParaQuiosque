using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PerguntasFrequentesSuporte
{
    public partial class Menu : Form
    {
        public AppConfig ConfiguracoesGlobais;
        private Configuracoes configForm;
        private List<PassoAPasso> PassoForms = new List<PassoAPasso>();
        public Menu()
        {
            InitializeComponent();
            //Environment.SetEnvironmentVariable("IdPerfilAtual", 0.ToString(), EnvironmentVariableTarget.User);
            ConfiguracoesGlobais = AcederConfig.ConfigAtual.AppConfig;  // Inicialização garantida
            Tag = "BotoesVisiveis";
            //MudarEstadoBtnMostrar_Esconder();
        }
        public void BtnMudarEsconder_MostrarMenu_Click(object sender, EventArgs e)
        {
            MudarEstadoBtnMostrar_Esconder();
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int? NumBotoa = ClasseAuxiliar.ExtrairNumeroFinal(btn.Name);
                if(NumBotoa.HasValue && NumBotoa.Value <=  ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesBotoesMenu.Count-1)
                    AcaoDoBotao(ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesBotoesMenu[NumBotoa.Value].Tipo, ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesBotoesMenu[NumBotoa.Value].Diretorio_Link);                
            }
        }
        private void AcaoDoBotao(string Tipo, string Diretorio_Link)
        {
            if (Tipo.ToUpper() == "FORMS")
                MudarEstadoBtnMostrar_Esconder(true);
            else
                MudarEstadoBtnMostrar_Esconder();

            switch (Tipo.ToUpper())
            {
                case "PDF": AbrirPDF(Diretorio_Link); break;
                case "LINK": AbrirSite(Diretorio_Link); break;
                case "FORMS": AbrirForms(Diretorio_Link); break;
            }
        }
        private void AbrirPDF(string caminhoPDF)
        {
            if (!File.Exists(caminhoPDF))
            {
                MessageBox.Show($"O documento PDF {caminhoPDF} não existe.", "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminhoPDF,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao abrir o documento PDF: {ex.Message}");
            }
        }
        private void AbrirSite(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                CreateNoWindow = true
            });
        }
        private void OcultarBotoesMenu()
        {
            foreach (Control control in Controls)
            {
                if (control is Button)
                {
                    if (control != BtnMudarEsconder_MostrarMenu && control != BtnSair)  // Mantém apenas os botões de alternância e saída visíveis
                        control.Visible = false;
                }
            }
        }
        public void MostrarBotoesMenu()
        {
            foreach (Control control in Controls)
            {
                if (control is Button || control.Enabled)
                {
                    int? Indice = ClasseAuxiliar.ExtrairNumeroFinal(control.Name);
                    if(Indice.HasValue)
                        control.Visible = Indice < ConfiguracoesGlobais.ConfiguracaoAplicacao.QuantidadeBotoes;
                }
            }
        }
        private void AbrirForms(string Diretorio_Link)
        {
            if (Diretorio_Link.ToUpper() == "CONFIG")
                MostrarFormConfiguracoes();
            else if (Diretorio_Link.ToUpper() == "FORMULARIO")
                MostrarFormConfiguracoes();  //AbrirFormsNoMenu(); /// por Fazer formulario
            else
            {
                // Divide a string com base nos caracteres "[" e "]"
                string[] partes = Diretorio_Link.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                int indice;
                if (partes.Length > 1 && partes[0].ToUpper() == "PASSOAPASSO")
                    if (int.TryParse(partes[1], out indice))
                        MostrarPassoAPasso(indice);  // O índice será a parte dentro de [ ]
            }
        }
        public void MostrarFormConfiguracoes()
        {
            configForm.Show();
            configForm.BringToFront();
        }
        public void MostrarPassoAPasso(int indice)
        {
            if (indice >= 0 && indice < PassoForms.Count)
            {
                PassoForms[indice].Show();
                PassoForms[indice].BringToFront();
            }
        }
        private void Menu_Load(object sender, EventArgs e)
        {

            Hide(); 
            // Carregar e iniciar o formulário de configurações
            configForm = new Configuracoes();
            configForm.TopLevel = false;
            Controls.Add(configForm);
            configForm.Hide();

            for (int i = 0; i < ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso.Count; i++) // Carregar e iniciar cada formulário de PassoAPasso
            {
                PassoAPasso passoForm = new PassoAPasso { Tag = i };
                passoForm.TopLevel = false;
                PassoForms.Add(passoForm);
                Controls.Add(passoForm);
                passoForm.Hide();
            }
            Configuracao_AtualizacaoForms.AtualizarTudo();
            Show();
        }
        private void MudarEstadoBtnMostrar_Esconder(bool AbrirForms = false)
        {
            if((string)Tag == "BotoesEscondidos")
            {
                MostrarBotoesMenu();
                Tag = "BotoesVisiveis";
                BtnMudarEsconder_MostrarMenu.Text = ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.TextoBotaoMostrarEsconder[(string)Tag];
            }
            else if ((string)Tag == "BotoesVisiveis")
            {
                OcultarBotoesMenu();
                if (AbrirForms)
                    Tag = "FormsAberto";
                else
                    Tag = "BotoesEscondidos";
                BtnMudarEsconder_MostrarMenu.Text = ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.TextoBotaoMostrarEsconder[(string)Tag];
            }
            else if ((string)Tag == "FormsAberto")
            {
                MostrarBotoesMenu();
                Tag = "BotoesVisiveis";
                BtnMudarEsconder_MostrarMenu.Text = ConfiguracoesGlobais.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.TextoBotaoMostrarEsconder[(string)Tag];
            }
        }

        private NotifyIcon trayIcon;
        private ContextMenuStrip contextMenu;

        public void TrayMenu()
        {
            contextMenu = new ContextMenuStrip();

            // Criar item de menu principal
            ToolStripMenuItem menuBotoes = new ToolStripMenuItem("Botões Abertos");

            // Loop por todas as janelas abertas
            foreach (Form form in Application.OpenForms)
            {
                if (form is Menu) // Verifica se é o formulário Menu
                {
                    foreach (Control control in form.Controls)
                    {
                        if (control is Button btn) // Apenas botões
                        {
                            ToolStripMenuItem subItem = new ToolStripMenuItem(btn.Text);
                            subItem.Click += (s, ev) => btn_Click(btn, EventArgs.Empty); // Associa o evento
                            menuBotoes.DropDownItems.Add(subItem);
                        }
                    }
                }
            }
            // Adicionar ao menu apenas se houver botões
            if (menuBotoes.DropDownItems.Count > 0)
            {
                contextMenu.Items.Add(menuBotoes);
                contextMenu.Items.Add(new ToolStripSeparator());
            }

            contextMenu.Items.Add(new ToolStripMenuItem("Mostrar_Esconder", null, Mostrar_Esconder));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Configurações", null, AbrirConfiguracoes));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Reiniciar", null, ReiniciarApp));
            contextMenu.Items.Add(new ToolStripMenuItem("Sair", null, Sair));

            // Criar o ícone da bandeja
            trayIcon = new NotifyIcon()
            {
                Icon = Icon, // Podes usar um ícone personalizado
                ContextMenuStrip = contextMenu,
                Text = "Menu Botões",
                Visible = true
            };
            trayIcon.DoubleClick += Mostrar_Esconder; // Duplo clique no ícone abre a app
            trayIcon.MouseClick += TrayIcon_MouseClick;

            // Personalizar a aparência do menu
            contextMenu.Renderer = new CustomMenuRenderer();
            contextMenu.ForeColor = Color.White; // Cor do texto
            contextMenu.BackColor = Color.Black;
        }
        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Apenas para clique esquerdo
            {
                contextMenu.Show(Cursor.Position);
            }
        }
        private void Mostrar_Esconder(object sender, EventArgs e)
        {
            if (Visible)
                Hide();
            else
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name == "Menu")
                    {
                        form.Show();
                        form.BringToFront();
                        form.Activate();
                        break;
                    }
                    else
                        form.Hide();
                }
            }
        }
        //private void Esconder(object sender, EventArgs e) { Hide(); }
        private void AbrirConfiguracoes(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "Menu")
                {
                    form.Show();
                    form.BringToFront();
                    form.Activate();
                    break;
                }
                else
                    form.Hide();
            }
            MudarEstadoBtnMostrar_Esconder(true);
            AcaoDoBotao("Forms", "Config");
        }
        private void ReiniciarApp(object sender, EventArgs e) { Application.Restart(); }
        private void Sair(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
    public class CustomMenuRenderer : ToolStripProfessionalRenderer
    {
        public static Color CorDeFundo = Color.Black;
        public static Color CorDeFundoAlternativa = CorTexto;
        public static Color CorTexto = Color.White;
        public static Color CorTextoAlternativa = CorDeFundo;

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected) // Quando o item é selecionado
            {
                using (SolidBrush brush = new SolidBrush(CorDeFundoAlternativa))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
                e.Item.ForeColor = CorTextoAlternativa; // Texto preto ao passar o mouse
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(CorDeFundo)) // Cor normal do fundo
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
                e.Item.ForeColor = CorTexto; // Texto branco normal
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(CorDeFundo)) // Cor de fundo do menu
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(CorDeFundo)) // Cor igual ao fundo do menu
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using (Pen pen = new Pen(CorTexto)) // Cor do separador
            {
                int y = e.Item.ContentRectangle.Top + e.Item.ContentRectangle.Height / 2;
                e.Graphics.DrawLine(pen, e.Item.ContentRectangle.Left, y, e.Item.ContentRectangle.Right, y);
            }
        }
    }
}
//passoForm.FormBorderStyle = FormBorderStyle.Fixed3D;

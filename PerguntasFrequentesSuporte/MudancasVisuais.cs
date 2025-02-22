using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PerguntasFrequentesSuporte
{
    static class MudancasVisuais
    {
        public static void ArredondarBorda(this Control controle, int intensidadeCurvas, int tamanhoBorda, Color corBorda)
        {
            controle.Paint += delegate (object sender, PaintEventArgs e)
            {
                GraphicsPath caminho = null;
                if (intensidadeCurvas == 8)
                    caminho = ObterCaminhoBordaCircular(controle, e); // Borda circular
                else if (intensidadeCurvas > 0)
                    caminho = ObterCaminhoBordaCurva(controle, CalcularRaioDaCurva(controle, intensidadeCurvas));

                if (caminho == null)
                {
                    caminho = new GraphicsPath();
                    caminho.AddRectangle(new Rectangle(0, 0, controle.Width, controle.Height));
                }
                controle.Region = new Region(caminho);

                if (tamanhoBorda != 0)
                    PintarBorda(controle, e, caminho, tamanhoBorda, corBorda);

                caminho.Dispose();
            };
        }
        private static int CalcularRaioDaCurva(Control controle, int intensidade)
        {
            int raioMaximo = Math.Min(controle.Width, controle.Height) / 2;
            switch (intensidade)
            {
                case 1: return Math.Min(raioMaximo / 6, raioMaximo);
                case 2: return Math.Min(raioMaximo / 5, raioMaximo);
                case 3: return Math.Min(raioMaximo / 4, raioMaximo);
                case 4: return Math.Min(raioMaximo / 3, raioMaximo);
                case 5: return Math.Min(raioMaximo / 2, raioMaximo);
                case 6: return raioMaximo;
                case 7: return Math.Max(raioMaximo * 2, raioMaximo);
                default: return Math.Max(raioMaximo / 2, raioMaximo);
            }
        }
        private static GraphicsPath ObterCaminhoBordaCurva(Control controle, int raio)
        {
            GraphicsPath caminho = new GraphicsPath();
            caminho.AddArc(new Rectangle(0, 0, raio, raio), 180, 90);
            caminho.AddArc(new Rectangle(controle.Width - raio, 0, raio, raio), -90, 90);
            caminho.AddArc(new Rectangle(controle.Width - raio, controle.Height - raio, raio, raio), 0, 90);
            caminho.AddArc(new Rectangle(0, controle.Height - raio, raio, raio), 90, 90);
            caminho.CloseFigure();
            return caminho;
        }
        private static GraphicsPath ObterCaminhoBordaCircular(Control controle, PaintEventArgs e)
        {
            GraphicsPath caminho = new GraphicsPath();
            int tamanho = Math.Min(controle.Width, controle.Height);
            int x = (controle.Width - tamanho) / 2;
            int y = (controle.Height - tamanho) / 2;
            int ajusteBorda = (int)(3 / 2.0);
            caminho.AddEllipse(x + ajusteBorda, y + ajusteBorda, tamanho - 3, tamanho - 3);
            return caminho;
        }
        public static void PintarBorda(Control controle, PaintEventArgs e, GraphicsPath caminho, int larguraBorda, Color cor)
        {
            if (cor == Color.Empty)
                cor = controle.ForeColor;
            using (Pen pen = new Pen(cor, larguraBorda))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, caminho);
            }
        }
    }
    static class Configuracao_AtualizacaoForms
    {
        static AppConfig config = AcederConfig.ConfigAtual.AppConfig;
        public static void AplicarConfiguracoesAoForm(Form form)
        {
            if (form == null)
                return;
            TemaVisual TemaAtual = config.VisualAplicacao.Tema[config.IndiceTemaAtual];


            form.Text = config.VisualAplicacao.Titulo;  // Aplica o título da janela
            form.Opacity = config.VisualAplicacao.OpacidadeDoPrograma; // Define a opacidade
            form.ShowInTaskbar = config.VisualAplicacao.AparecerNaBarraDeTarefas; // Define se aparece na barra de tarefas

            if (!config.VisualAplicacao.JanelasMoveis && !config.VisualAplicacao.JanelasRedimensionaveis)
                form.FormBorderStyle = FormBorderStyle.None;  // Se a janela não pode ser movida nem redimensionada, remove a borda completamente.
            else if (!config.VisualAplicacao.JanelasRedimensionaveis)
                form.FormBorderStyle = FormBorderStyle.FixedSingle;  // Se só não pode ser redimensionada, mantém a barra de título, mas bloqueia o redimensionamento.
            else
                form.FormBorderStyle = FormBorderStyle.Sizable; // Se pode ser redimensionada, mantém o estilo padrão de janelas redimensionáveis.

            if (!config.VisualAplicacao.JanelasMoveis)  // Define se a janela pode ser movida
            {
                form.ControlBox = false;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
            }
            if (config.VisualAplicacao.UsaIcone) // Aplica ícone se configurado
            {
                if (File.Exists(config.VisualAplicacao.CaminhoIcone))
                    form.Icon = new Icon(config.VisualAplicacao.CaminhoIcone);
            }

            foreach (Control control in form.Controls) // Aplica configurações aos botões do form
            {
                if (control is Button)
                {
                    Button botao = (Button)control;
                    string tipoBotao = botao.Tag as string;
                    VisualBotoesPorJanela aparenciaBotao = TemaAtual.ObterVisualDoBotao(form, tipoBotao);
                    if (tipoBotao != null)
                    {
                        if (TemaAtual.VisualBotoes.ContainsKey(form.Name))
                        {
                            VisualBotoesPorJanela[] botoes = TemaAtual.VisualBotoes[form.Name];

                            for (int i = 0; i < botoes.Length; i++)
                            {
                                if (botoes[i].TipoDoControl == tipoBotao)
                                    aparenciaBotao = botoes[i];
                            }
                        }
                    }

                    // Aplica as configurações ao botão
                    botao.BackColor = aparenciaBotao.CorFundo;
                    botao.ForeColor = aparenciaBotao.CorTexto;
                    botao.Font = aparenciaBotao.Fonte;
                    botao.FlatStyle = FlatStyle.Flat;
                    botao.UseVisualStyleBackColor = false;
                    botao.TabStop = false;
                    botao.Margin = new Padding(aparenciaBotao.Margem, aparenciaBotao.Margem, aparenciaBotao.Margem, aparenciaBotao.Margem);

                    // Se houver método de arredondar borda, aplica-o (supondo que é uma extensão de Control)
                    botao.ArredondarBorda(aparenciaBotao.IntensidadeArredondarBorda,
                                          aparenciaBotao.TamanhoContrasteBorda,
                                          aparenciaBotao.CorContrasteBorda);
                }
            }


            if (form.Name == "Menu")  // Define a cor de fundo do form
            {
                form.Size = config.VisualAplicacao.TamanhoJanelas.ObterTamanho("Menu");
                form.BackColor = TemaAtual.VisualDoMenu.CorFundoMenu;
                form.ControlBox = false;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.FormBorderStyle = FormBorderStyle.None;
                if (TemaAtual.VisualDoMenu.MenuTransparente)
                    form.BackColor = form.TransparencyKey;

                if (TemaAtual.VisualDoMenu.UsarImagemDeFundoMenu)
                {
                    if (File.Exists(TemaAtual.VisualDoMenu.CaminhoImagemDeFundoMenu))
                    {
                        form.BackgroundImage = Image.FromFile(TemaAtual.VisualDoMenu.CaminhoImagemDeFundoMenu);
                        form.BackgroundImageLayout = ImageLayout.Stretch; // Ajusta a imagem ao tamanho do formulário
                    }
                }

                if (form is Menu menu)
                    AplicarAjustesVisuaisMenu(menu);
            }
            else
            {
                form.Size = config.VisualAplicacao.TamanhoJanelas.ObterTamanho(form.Name);
                form.BackColor = TemaAtual.CorFundo;

                form.ArredondarBorda(0,
                 TemaAtual.TamanhoDoContraste,
                 TemaAtual.CorDoContraste);

            }

            if (form is PassoAPasso passoAPasso)
                AplicarConfiguracoesPassoAPasso(passoAPasso);
        }
        public static void AplicarConfiguracoesPassoAPasso(PassoAPasso passoAPasso)
        {
            if (passoAPasso == null || config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso == null)
            {
                MessageBox.Show("Erro: Configuração do Passo a Passo não está disponível.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = 0;
            if (passoAPasso.Tag != null && int.TryParse(passoAPasso.Tag.ToString(), out int resultado))
                id = resultado;

            if (id < 0 || id >= config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso.Count)
            {
                MessageBox.Show($"Erro: O índice {id} não existe em Config_PassoAPasso.Dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passoAPasso.Close();
                return;
            }

            string caminhoImagens = Path.Combine(Ficheiros.Caminho, "ImagensPassoAPasso", id.ToString());

            if (!Directory.Exists(caminhoImagens))
            {
                MessageBox.Show($"A pasta {caminhoImagens} não existe. Por favor, carregue as imagens necessárias para essa pasta.",
                    "Pasta não encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passoAPasso.Close();
                return;
            }

            // Obtém a configuração específica
            ConfigPassoAPasso ConfigPassoAPassoAtual = config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso[id];
            passoAPasso.Text = ConfigPassoAPassoAtual.Titulo;

            Image[][] ListaImagens = new Image[ConfigPassoAPassoAtual.Imagens.Length][];

            foreach (Button btn in ClasseAuxiliar.ObterBotoesOrdenados(passoAPasso)) // Obtém os botões ordenados por número
            {
                int? numero = ClasseAuxiliar.ExtrairNumeroFinal(btn.Name); // Obtém o número final do nome do botão

                if (numero.HasValue && ConfigPassoAPassoAtual.Imagens.Length > numero.Value)
                {
                    btn.Text = ConfigPassoAPassoAtual.Imagens[numero.Value].NomeCompleto;

                    ListaImagens[numero.Value] = Ficheiros.SalvarImagens(
                        Directory.GetFiles(caminhoImagens,
                            ConfigPassoAPassoAtual.Imagens[numero.Value].NomeFicheiro + "." +
                            ConfigPassoAPassoAtual.Imagens[numero.Value].FormatoFicheiro));
                }
            }

            if (passoAPasso.ListaTotal == null || passoAPasso.ListaTotal.Length != ListaImagens.Length) // Se ListaTotal for nula ou tiver tamanho diferente, recarrega
            {
                passoAPasso.ListaTotal = ListaImagens;
            }
            else
            {
                for (int i = 0; i < ListaImagens.Length; i++)  // Verifica se alguma sublista tem tamanho diferente
                {
                    if (passoAPasso.ListaTotal[i]?.Length != ListaImagens[i]?.Length)
                    {
                        passoAPasso.ListaTotal = ListaImagens;
                        break;
                    }
                }
            }

            AtualizarInterfacePassoAPasso(passoAPasso);  // Atualiza a interface
        }
        private static void AtualizarInterfacePassoAPasso(PassoAPasso passoAPasso)
        {
            int quantidadeImagens = passoAPasso.ListaTotal.Length, colunasAtivas = 0;
            bool precisaReconstruir = false, temImagens = false;

            foreach (Button btn in ClasseAuxiliar.ObterBotoesOrdenados(passoAPasso))
            {
                int? numero = ClasseAuxiliar.ExtrairNumeroFinal(btn.Name);
                if (numero.HasValue)
                {
                    if (quantidadeImagens > numero.Value)
                    {
                        if (passoAPasso.ListaTotal[numero.Value] != null)
                        {
                            if (passoAPasso.ListaTotal[numero.Value].Length > 0)
                            {
                                temImagens = true;
                            }
                        }
                    }

                    if (btn.Visible != temImagens) // Só marca reconstrução se houver mudança na visibilidade
                        precisaReconstruir = true;

                    btn.Visible = temImagens;

                    if (temImagens)
                        btn.Text = config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso[Convert.ToInt32(passoAPasso.Tag)].Imagens[numero.Value].NomeCompleto;
                }
            }

            if (precisaReconstruir)  // Reconfigura o layout do painel de botões
            {
                passoAPasso.PanelBtns.Controls.Clear();

                foreach (Button btn in ClasseAuxiliar.ObterBotoesOrdenados(passoAPasso))
                {
                    if (btn.Visible)
                        passoAPasso.PanelBtns.Controls.Add(btn, colunasAtivas++, 0);
                }

                passoAPasso.PanelBtns.Controls.Add(passoAPasso.txtPasso, colunasAtivas++, 0);
                passoAPasso.PanelBtns.ColumnCount = colunasAtivas;

                passoAPasso.PanelBtns.ColumnStyles.Clear();

                for (int i = 0; i < colunasAtivas - 1; i++)
                {
                    passoAPasso.PanelBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / colunasAtivas));
                }
                passoAPasso.PanelBtns.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            passoAPasso.AtualizarImagem();
        }
        public static void AplicarAjustesVisuaisMenu(Menu menu)
        {
            if (menu == null)
                return;

            AplicarAjustesVisuaisBtnSair_EsconderMostrar(menu);

            ConfigGeral ConfigAtualMenu = config.ConfiguracaoAplicacao;

            // Obter os botões ordenados com base no número final
            Button[] arrayBotoes = ClasseAuxiliar.ObterBotoesOrdenados(menu);

            int linhaAtual = 0, colunaAtual = 0;
            int espacamento = arrayBotoes[0].Margin.Left*2;
            int posX = espacamento, posY = espacamento;

            if (ConfigAtualMenu.QuantidadeBotoes == 0 || ConfigAtualMenu.BotoesPorLinha == 0)
            {
                foreach (Button btn in arrayBotoes)
                {
                    btn.Visible = false;
                    btn.Enabled = false;
                }
                return;
            }

            int numeroLinhas = ConfigAtualMenu.QuantidadeBotoes / ConfigAtualMenu.BotoesPorLinha;

            if (ConfigAtualMenu.QuantidadeBotoes % ConfigAtualMenu.BotoesPorLinha != 0)
                numeroLinhas++;

            int larguraDisponivel = (menu.ClientSize.Width * ConfigAtualMenu.PercentagemEcraParaBotoesLargura) / 100;
            int alturaDisponivel = (menu.ClientSize.Height * ConfigAtualMenu.PercentagemEcraParaBotoesAltura) / 100;

            int larguraBotao = (larguraDisponivel - (ConfigAtualMenu.BotoesPorLinha + 1) * (espacamento)) / ConfigAtualMenu.BotoesPorLinha;
            int alturaBotao = (alturaDisponivel - (numeroLinhas + 1) * (espacamento)) / numeroLinhas;

            for (int i = 0; i < ConfigAtualMenu.QuantidadeBotoes; i++)
            {
                arrayBotoes[i].SendToBack();

                arrayBotoes[i].Size = new Size(larguraBotao, alturaBotao);
                arrayBotoes[i].Location = new Point(posX, posY);
                arrayBotoes[i].Visible = true;
                arrayBotoes[i].Text = ConfigAtualMenu.ConfiguracoesBotoesMenu[i].Nome;

                if (ConfigAtualMenu.TextoDosBotoesDeLado)
                    arrayBotoes[i].TextImageRelation = TextImageRelation.ImageAboveText;

                colunaAtual++;
                if (colunaAtual >= ConfigAtualMenu.BotoesPorLinha)
                {
                    colunaAtual = 0;
                    linhaAtual++;

                    posY += alturaBotao + espacamento;   // Atualiza a posição Y quando passar para a próxima linha
                    posX = espacamento; // Reseta X para o início da linha
                }
                else
                    posX += larguraBotao + espacamento; // Atualiza a posição X para o próximo botão na mesma linha
            }

            // Esconde os botões não usados
            for (int i = ConfigAtualMenu.QuantidadeBotoes; i < arrayBotoes.Length; i++)
            {
                arrayBotoes[i].Visible = false;
                arrayBotoes[i].Enabled = false;
            }

            menu.AutoScroll = false;
        }
        public static void AplicarAjustesVisuaisBtnSair_EsconderMostrar(Menu menu)
        {
            if (menu == null)
                return;

            if (config.VisualAplicacao.ProgramaNoTrayMenu)
                menu.TrayMenu();

            int posX, posY, posXSair, margem;

            menu.BtnSair.Size = config.VisualAplicacao.TamanhoJanelas.ObterTamanho("Botoes_Sair_EsconderMostrar");
            margem = menu.BtnMudarEsconder_MostrarMenu.Margin.Left;// Usa a margem definida no próprio controle.


            if (config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.PosicaoEsquerda)
                posX = Screen.PrimaryScreen.WorkingArea.Right - menu.BtnMudarEsconder_MostrarMenu.Width - margem;
            else
                posX = Screen.PrimaryScreen.WorkingArea.Left + margem;
            if (config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.PosicaoSuperior)
                posY = Screen.PrimaryScreen.WorkingArea.Top + margem;
            else
                posY = Screen.PrimaryScreen.WorkingArea.Bottom - menu.BtnMudarEsconder_MostrarMenu.Height - margem;

            menu.BtnMudarEsconder_MostrarMenu.Location = new Point(posX, posY);


            // Configura o botão de sair.
            menu.BtnSair.Size = menu.BtnMudarEsconder_MostrarMenu.Size;

            if (config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.PosicaoEsquerda)
                posXSair = Screen.PrimaryScreen.WorkingArea.Left + menu.BtnSair.Margin.Left;
            else
                posXSair = Screen.PrimaryScreen.WorkingArea.Right - menu.BtnSair.Width - menu.BtnSair.Margin.Right;
            menu.BtnSair.Location = new Point(posXSair, menu.BtnMudarEsconder_MostrarMenu.Location.Y);

            menu.BtnMudarEsconder_MostrarMenu.UseVisualStyleBackColor = false;
            menu.BtnSair.UseVisualStyleBackColor = false;
        }
        public static void AtualizarTudo()
        {
            foreach (Form form in Application.OpenForms)   // Atualiza as janelas que estão abertas no momento
            {
                AplicarConfiguracoesAoForm(form);
            }
        }
    }
}

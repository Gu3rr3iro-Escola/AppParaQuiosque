using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Linq;

// Talvez várias abas de botões
// Por colocar segurança para fechar Programa e abrir Config 
namespace PerguntasFrequentesSuporte
{
    //---------------------------
    // Tamanhos iniciais para janelas
    //---------------------------
    public class TamanhoInicialJanelas
    {
        [Description("Dicionário que armazena os tamanhos das janelas")]
        public Dictionary<string, Size> Tamanhos { get; set; } = new Dictionary<string, Size>
        {
            { "Default", new Size(400, 300) },
            { "Botoes_Sair_EsconderMostrar", new Size(240, 120) },
            { "PassoAPasso", new Size(390, 540) },
            { "Configuracoes", new Size(800, 550) },
            { "InputBoxColor", new Size(400, 560) },
            { "InputBoxFont", new Size(310, 355) },
            { "InputBoxImagem", new Size(370, 180) },
            { "Menu", Screen.PrimaryScreen.Bounds.Size } // Usa o tamanho total do ecrã
        };
        public TamanhoInicialJanelas()  // Construtor: valida com os tamanhos mínimos definidos
        {
            Validate();
        }
        public void Validate()
        {
            Dictionary<string, Size> MinSizes = new Dictionary<string, Size> // Tamanhos mínimos por janela
            {
                { "Default", new Size(200, 150) },
                { "Configuracoes", new Size(600, 400) },
                { "InputBoxColor", new Size(300, 400) },
                { "InputBoxFont", new Size(250, 300) },
                { "InputBoxImagem", new Size(250, 150) },
                { "Menu", new Size(800, 600) },
                { "PassoAPasso", new Size(350, 500) }
            };

            Size maxSize = new Size(1920, 1080);

            foreach (string key in Tamanhos.Keys.ToList())
            {
                Size minSize, tamanhoAtual = Tamanhos[key];

                if (MinSizes.ContainsKey(key))
                    minSize = MinSizes[key];
                else
                    minSize = MinSizes["Default"];

                int larguraCorrigida = Math.Max(minSize.Width, Math.Min(tamanhoAtual.Width, maxSize.Width));
                int alturaCorrigida = Math.Max(minSize.Height, Math.Min(tamanhoAtual.Height, maxSize.Height));

                Tamanhos[key] = new Size(larguraCorrigida, alturaCorrigida);
            }
        }
        public Size ObterTamanho(string nomeJanela)
        {
            if (nomeJanela == "Menu")
                return Screen.PrimaryScreen.Bounds.Size;

            if (Tamanhos.ContainsKey(nomeJanela))
                return Tamanhos[nomeJanela];

            return Tamanhos["Default"]; // Retorna o tamanho padrão
        }
    }
    //---------------------------
    // Aparência dos Controlo (por exemplo, botões)
    //---------------------------
    public class VisualBotoesPorJanela
    {
        [Description("Depende da Janela Forms")]
        public string TipoDoControl { get; set; } = "Default";
        //[JsonIgnore]
        [Description("Fonte associada ao Forms.  (O atributo pode não ser usado em certos casos)")]
        public Font Fonte { get; set; } = SystemFonts.DefaultFont;

        // [Description("Array de cada valor da fonte, usado apenas para escrever nos JSON")]
        //  public string[] FonteArray { get; set; } = new string[3];

        [Description("Margem que o Control vai ter de espaçamento. (Pode não funcionar em certos casos)")]
        public int Margem { get; set; } = 0;
        [Description("Intensidade das curvas. (Pode não funcionar em certos casos)" +
            "\n\n" +
            "Opções:" +
            "\n\t0 = Sem arredondamenton " +
            "\n\t1 = Muito ligeiro" +
            "\n\t2 = Ligeiramente curvo" +
            "\n\t3 = Curvatura moderada" +
            "\n\t4 = Curvas aceitáveis" +
            "\n\t5 = Curvas visíveis" +
            "\n\t6 = Muito arredondado" +
            "\n\t7 = Circular se for quadrado" +
            "\n\t8 = Forçar círculo (pode cortar o texto)")]
        public int IntensidadeArredondarBorda { get; set; } = 0;
        [Description("Tamanho que o contraste vai ter á volta do controlo. (O atributo pode não ser usado em certos casos)")]
        public int TamanhoContrasteBorda { get; set; } = 2;
        [Description("Cor que o contraste vai ter á volta do controlo, se for igual á cor de fundo não se notará,  normalmente é igual á cor do texto. (O atributo pode não ser usado em certos casos)")]
        public Color CorContrasteBorda { get; set; } = Color.Black;
        [Description("Cor que o Texto do controlo, vai ter, não deve ser igual á cor de fundo.  (O atributo pode não ser usado em certos casos)")]
        public Color CorTexto { get; set; } = Color.Black;
        [Description("Cor que o fundo do controlo vai ter, não deve ser igual á cor do texto.  (O atributo pode não ser usado em certos casos)")]
        public Color CorFundo { get; set; } = Color.White;
        public void Validate()
        {
            if (Fonte == null)  // Verificação da Fonte
                Fonte = SystemFonts.DefaultFont;

            if (Fonte.Size < 6 || Fonte.Size > 100)
                Fonte = new Font(Fonte.FontFamily, SystemFonts.DefaultFont.Size, Fonte.Style);

            Margem = Math.Max(0, Math.Min(Margem, 80));
            TamanhoContrasteBorda = Math.Max(0, Math.Min(TamanhoContrasteBorda, 200));
            IntensidadeArredondarBorda = Math.Max(0, Math.Min(IntensidadeArredondarBorda, 8));

            if (CorFundo.IsEmpty)   // Permitir cores vazias
                CorFundo = Color.White;
            if (CorTexto.IsEmpty)
                CorTexto = Color.Black;
            if (CorContrasteBorda.IsEmpty)
                CorContrasteBorda = Color.Black;

            if (CorFundo == CorTexto)  // Não permite que a CorFundo seja igual à CorTexto
            {
                if (CorFundo == Color.White)
                    CorTexto = Color.Black;
                else
                    CorTexto = Color.White;
            }

            if (string.IsNullOrWhiteSpace(TipoDoControl))
                TipoDoControl = "Default";
        }
    }
    //---------------------------
    // Aparência geral (ex.: temas)
    //---------------------------
    public class TemaVisual
    {
        [Description("Nome do Tema.")]
        public string NomeTema { get; set; } = "Default";
        [Description("Indica a cor de fundo dos forms")]
        public Color CorFundo { get; set; } = Color.White;
        [Description("Indica a cor que o contraste/borda dos forms")]
        public Color CorDoContraste { get; set; } = Color.Black;
        [Description("Indica o tamanho do contraste/borda para os forms, coloque uma cor diferente do fundo, caso queria que não aparesa é só colocar tamanho 0")]
        public int TamanhoDoContraste { get; set; } = 3;
        [Description("Define a aparencia do menu, se é transparente, se tem imagem de fundo, etc")]
        public TemaMenu VisualDoMenu = new TemaMenu();
        [Description("Cada form pode ter vários tipos de botões, e vários botões podem compartilhar a mesma aparência.")]
        public Dictionary<string, VisualBotoesPorJanela[]> VisualBotoes { get; set; } = new Dictionary<string, VisualBotoesPorJanela[]>();

        public TemaVisual()
        {
            VisualDoMenu = new TemaMenu();
            VisualDoMenu.Validate();

            VisualBotoes = new Dictionary<string, VisualBotoesPorJanela[]>
            {
                { "Default", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } },
                { "Configuracoes", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } },
                { "InputBoxColor", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } },
                { "InputBoxFont", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } },
                { "InputBoxImagem", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } },
                {"Menu", new VisualBotoesPorJanela[]{
                new VisualBotoesPorJanela
                {
                    TipoDoControl = "BotaoSair_Mostrar",
                    Fonte = new Font("Arial", 30, FontStyle.Bold),
                    Margem = 5,
                    IntensidadeArredondarBorda = 3,
                    TamanhoContrasteBorda = 4,
                    CorContrasteBorda = Color.Black,
                    CorTexto = Color.Black,
                    CorFundo = Color.White
                },
                new VisualBotoesPorJanela
                {
                    TipoDoControl = "BotaoAcao",
                    Fonte = new Font("Arial", 40, FontStyle.Bold),
                    Margem = 10,
                    IntensidadeArredondarBorda = 6,
                    TamanhoContrasteBorda = 4,
                    CorContrasteBorda = Color.Black,
                    CorTexto = Color.Black,
                    CorFundo = Color.White
                }}},
                { "PassoAPasso", new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() } }
            };
        }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(NomeTema))
                NomeTema = "Default";

            if (CorFundo.IsEmpty)
                CorFundo = Color.White;

            if (CorDoContraste.IsEmpty)
                CorDoContraste = Color.Black;

            TamanhoDoContraste = Math.Max(0, Math.Min(TamanhoDoContraste, 6));

            if (CorFundo == CorDoContraste)  // Não permite que a CorFundo seja igual do CorDoContraste
            {
                if (CorFundo == Color.White)
                    CorDoContraste = Color.Black;
                else
                    CorDoContraste = Color.White;
            }

            // Garante que todas as chaves obrigatórias estão presentes
            string[] chavesObrigatorias = {
                "Default", "Configuracoes", "InputBoxColor",
                "InputBoxFont", "InputBoxImagem", "Menu", "PassoAPasso"
            };

            foreach (string chave in chavesObrigatorias)
            {
                if (!VisualBotoes.ContainsKey(chave))
                    VisualBotoes[chave] = new VisualBotoesPorJanela[] { new VisualBotoesPorJanela() };
            }

            foreach (VisualBotoesPorJanela[] botaoArray in VisualBotoes.Values) // Validar os botões de cada janela
            {
                foreach (VisualBotoesPorJanela botao in botaoArray)
                {
                    if (botao != null)
                        botao.Validate();
                }
            }
        }
        public VisualBotoesPorJanela ObterVisualDoBotao(Form form, string tipoBotao)
        {
            if (form == null || tipoBotao == null || tipoBotao == "")
                return new VisualBotoesPorJanela(); // Retorna padrão se os parâmetros forem inválidos

            if (VisualBotoes.ContainsKey(form.Name))
            {
                VisualBotoesPorJanela[] botoes = VisualBotoes[form.Name];

                for (int i = 0; i < botoes.Length; i++)
                {
                    if (botoes[i].TipoDoControl == tipoBotao)
                        return botoes[i]; // Retorna o primeiro botão que corresponde ao tipo
                }
            }

            return new VisualBotoesPorJanela(); // Retorna configuração padrão se não encontrar
        }
    }
    //---------------------------
    // Aparência do menu (ex.: Cor de Fundo, imagem de fundo)
    //---------------------------
    public class TemaMenu
    {
        [Description("Indica se menu usa uma imagem como fundo, o caminho da imagem deve ser válido.")]
        public bool UsarImagemDeFundoMenu { get; set; } = false;
        [Description("Indica a imagem que aparece de fundo do menu, se o caminho for válido e UsarImagemDeFundo estiver ativado.")]
        public string CaminhoImagemDeFundoMenu { get; set; } = "";
        [Description("Indica se o fundo do menu é transparente, se a imagem de fundo estiver ativada isto é ignorado.")]
        public bool MenuTransparente { get; set; } = true; // por acontecer erros se a cor de fundo estiver desligada e se ativar mas não se mudar a cor, é preciso corrigir nesse caso
        [Description("Indica se a cor do fundo do menu, se for transparente isto é ignorado. ")]
        public Color CorFundoMenu { get; set; } = Color.White;

        public void Validate()
        {
            if (CorFundoMenu.IsEmpty)   // Permitir cores vazias
                CorFundoMenu = Color.White;
        }
    }
    //-------------------------------------------//
    //  Definições gerais do Pograma //
    //-------------------------------------------//
    public class AparenciaGeral
    {
        [Description("Nome do programa que aparece na barra de tarefas e em outros locais.")]
        public string Titulo { get; set; } = "Menu de Botões";
        [Description("Bloqueia ou permite que as janelas sejam redimensionaveis pelo utilizador.")]
        public bool JanelasRedimensionaveis { get; set; } = true;
        [Description("Bloqueia ou permite que as janelas sejam movidas pelo utilizador, quando está desativado ele também bloqueia o redimensionamento "+
        "\nSe as janelas não poderem ser redimensionaveis nem movives elas não terão uma Barra de titulo.")]
        public bool JanelasMoveis { get; set; } = true;
        [Description("Define se o programa aparece na barra de tarefas.")]
        public bool AparecerNaBarraDeTarefas { get; set; } = false;
        [Description("Define se o programa usa algum icone diferente do icone padrão.")]
        public bool UsaIcone { get; set; } = false;
        [Description("Caminho do icone usado pela aplicação se o UsaIcone estiver ativado.")]
        public string CaminhoIcone { get; set; } = "";
        [Description("Opacidade do programa, 1 para 100% visivel e 0.1 para muito pouco visivel.")]
        public double OpacidadeDoPrograma { get; set; } = 0.99;
        [Description("Define o programa deve criar um icone no menu de icones da barra de tarefa ou não.\n " +
        "Tray menu = icones de aplicações como aplicações em segundo plano, acesso fácil às funções do sistema, etc. (geralmente no canto direito inferior ao lado do relógio).")]
        public bool ProgramaNoTrayMenu { get; set; } = true;
        [Description("Tamanho inicial das janelas, quando o utlizador redimensiona a janela o valor não é atualizado pois ele é só o tamanho inicial.")]
        public TamanhoInicialJanelas TamanhoJanelas { get; set; } = new TamanhoInicialJanelas();
        [Description("Lista de temas. (Exemplo de temas.: Claro, Escuro, Azul)")]
        public List<TemaVisual> Tema { get; set; } = new List<TemaVisual>();
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                Titulo = "Menu de Botões";

            OpacidadeDoPrograma = Math.Max(0.1, Math.Min(OpacidadeDoPrograma, 1.0));

            if (Tema != null)
            {
                foreach (TemaVisual tema in Tema)
                    tema.Validate();
            }
            TamanhoJanelas?.Validate(); // Valida os tamanhos ao validar as configurações gerais

        }
    }
    //---------------------------
    // Classe para representar uma Imagem
    //---------------------------
    public class FicheiroImagem
    {
        [Description("Nome do botão")]
        public string NomeCompleto { get; set; } = "Default";
        [Description("Nome Do ficheiro")]
        public string NomeFicheiro { get; set; } = "Default";
        [Description("Tipo do ficheiro")]
        public string FormatoFicheiro { get; set; } = "png";

        public FicheiroImagem() { }

        public void Validate()  // Validação básica para Imagem
        {
            if (string.IsNullOrWhiteSpace(NomeCompleto))
                NomeCompleto = "Nome Completo Default";

            if (string.IsNullOrWhiteSpace(NomeFicheiro))
                NomeFicheiro = "NomeFicheiroDefault";
            else if (NomeFicheiro.Contains(".")) // Verifica se há um ponto na string
            {
                string[] partes = NomeFicheiro.Split('.'); // Separa a parte antes e depois do ponto
                FormatoFicheiro = partes.Last().ToUpper(); // Atribui o formato do ficheiro (sempre em maiúsculas)
                NomeFicheiro = string.Join("", partes.Take(partes.Length - 1));  // Remove o ponto e tudo o que vem depois
                MessageBox.Show("O nome da imagem for armazenado com o tipo, o tipo será corrigido mas é possivel que aconteçam erros");
            }

            string[] formatosPermitidos = { "ICO", "JPG", "JPEG", "GIF", "PNG" };

            if (!formatosPermitidos.Contains(FormatoFicheiro.ToUpper()))  // Se o formato não for permitido, ajusta para o padrão
            {
                FormatoFicheiro = "PNG";
                MessageBox.Show("Formato de imagem não permitido, por favor altera para JPEG, GIF, PNG ou ICO");
            }
            else
                FormatoFicheiro = "PNG";   // Se não houver ponto, usa o formato padrão
        }
    }
    //---------------------------
    // Dados do Passo a Passo com array fixo de 4 imagens (índices 0 a 3)
    //---------------------------
    public class ConfigPassoAPasso
    {
        [Description("Nome da janela do passo a passo")]
        public string Titulo { get; set; } = "Passo a Passo";

        [Description("Imagens deste Passo a Passo")]
        public FicheiroImagem[] Imagens { get; set; } = new FicheiroImagem[3]; // Inicialização direta

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                Titulo = "Passo a Passo";

            if (Imagens == null || Imagens.Length != 3) // Garante sempre 3 posições
                Imagens = new FicheiroImagem[3];
        }
    }
    //---------------------------
    // Dados para o botão "Sair/Mostrar"
    //---------------------------
    public class ConfigSair_Mostrar
    {
        [Description("Define se o botão que de mostrar/esconde o Menu está visivel ou não.")]
        public bool BotaoEsconder_MostrarEsconder { get; set; } = true;
        [Description("Define se o botão de sair está visivel ou não.")]
        public bool BtnSairVisivel { get; set; } = true;
        [Description("Define se os botões de Sair e o botão de mostrar/esconder o menu estão em cima ou em baixo (em baixo quando desativado).")]
        public bool PosicaoSuperior { get; set; } = false;
        [Description("Define se o botão de mostrar/esconder o menu está a esquerda, o botão de sair fica do lado oposto quando está ativado.")]
        public bool PosicaoEsquerda { get; set; } = true;
        [Description("Textos para o botão que mostra/esconde o menu.\n BotoesVisiveis : Mostrar, BotoesEscondidos : Esconder, FormsAberto : Feche a janela.\n Não se deve mudar o primeiro elemento só a mensagem")]
        public Dictionary<string, string> TextoBotaoMostrarEsconder { get; set; } = new Dictionary<string, string>()
        {
            { "BotoesVisiveis", "Mostrar" },
            { "BotoesEscondidos", "Esconder" },
            { "FormsAberto", "Feche a janela." }
        };
        public void Validate()
        {
            if (TextoBotaoMostrarEsconder == null)
                TextoBotaoMostrarEsconder = new Dictionary<string, string>();

            // Chaves corretas conforme a descrição
            List<string> chavesCorretas = new List<string> { "BotoesVisiveis", "BotoesEscondidos", "FormsAberto" };

            // Textos padrão para cada chave
            Dictionary<string, string> textosPadrao = new Dictionary<string, string>()
            {
                { "BotoesVisiveis", "Mostrar" },
                { "BotoesEscondidos", "Esconder" },
                { "FormsAberto", "Feche a janela." }
            };

            // Verificar e corrigir chaves incorretas
            List<string> chavesAtuais = TextoBotaoMostrarEsconder.Keys.ToList();
            foreach (string chave in chavesAtuais)
            {
                if (!chavesCorretas.Contains(chave)) // Se a chave não for uma das corretas, apaga-a
                    TextoBotaoMostrarEsconder.Remove(chave);
            }

            foreach (string chaveCorreta in chavesCorretas) // Garantir que todas as chaves corretas existem
            {
                if (!TextoBotaoMostrarEsconder.ContainsKey(chaveCorreta))  // Se faltar alguma chave, adiciona com o texto padrão
                    TextoBotaoMostrarEsconder[chaveCorreta] = textosPadrao[chaveCorreta];
            }
        }
    }
    //---------------------------
    // Função do botão no menu
    //---------------------------
    public class FuncaoBotaoMenu
    {
        [Description("Nome do Botão.")]
        public string Nome { get; set; } = "DefaultBotão";
        [Description("Tipo do botão. " +  //Ainda por acresentar
            "\nOpções:" +
            "\n\t PDF" +
            "\n\t LINK" +
            "\n\t FORMS" +
            "\n\tAinda por acresentar")]
        public string Tipo { get; set; } = "Link";
        [Description("Diretorio ou Link indica o arquivo/link que o botão vai abrir." +
            "\n\tPara Pdf é o nome do ficheiro pdf sem .pdf" +
            "\n\tPara Link é o link do site que se quer abrir" +
            "\n\tPara forms pode ser CONFIG,FORMULARIO,PASSOAPASSO[ ]" +
            "\n\tDentro do [] coloca-se o id do PassoA passo, se só houver 1 passo a passo o id é 0")]
        public string Diretorio_Link { get; set; } = "";

        public void Validate()
        {
            List<string> tiposPermitidos = new List<string> { "FORMS", "LINK", "PDF" };   // Lista de tipos permitidos

            if (string.IsNullOrWhiteSpace(Nome))  // Garante que o nome não está vazio
                Nome = "Botao Default";

            if (string.IsNullOrWhiteSpace(Tipo) || !tiposPermitidos.Contains(Tipo, StringComparer.OrdinalIgnoreCase)) // Valida o tipo
                Tipo = "LINK"; // Define como "Link" se o valor for inválido

            if (string.IsNullOrWhiteSpace(Diretorio_Link)) // Valida o diretório ou link
            {
                if (Tipo.Equals("LINK", StringComparison.OrdinalIgnoreCase))
                    Diretorio_Link = "https://example.com";
                else if (Tipo.Equals("PDF", StringComparison.OrdinalIgnoreCase))
                    Diretorio_Link = "DocumentoDefault";
                else if (Tipo.Equals("FORMS", StringComparison.OrdinalIgnoreCase))
                    Diretorio_Link = "CONFIG";
            }
        }
    }
    //---------------------------
    // Dados do Menu
    //---------------------------
    public class ConfigGeral
    {
        [Description("Define se o texto do botão fica na vertical, util se os botões ficarem demasiado altos")]
        public bool TextoDosBotoesDeLado { get; set; } = false;
        [Description("Define qual a percentagem da altura do ecrã é que os botões do menu vão usar. " +
            "\nO tamanho dos botões é definida automaticamente com todo o espaço disponivel na largura " +
            "\n(Dica: Definir uma margem alta para que os botões não fiquem muito grandes)")]
        public int PercentagemEcraParaBotoesAltura { get; set; } = 80;
        [Description("Define qual a percentagem da largura do ecrã é que os botões do menu vão usar. " +
    "\nO tamanho dos botões é definida automaticamente com todo o espaço disponivel na largura " +
    "\n(Dica: Definir uma margem alta para que os botões não fiquem muito grandes)")]
        public int PercentagemEcraParaBotoesLargura { get; set; } = 100;
        [Description("Define a quantidade de botões que cada linha vai ter, não pode ter mais que 2 linhas no total")]
        public int BotoesPorLinha { get; set; } = 0
            ;
        [Description("Quantidade de botões que aparecem no menu, podem haver mais botões configurados do que os botões visiveis mas não o contrário")]
        public int QuantidadeBotoes { get; set; } = 8;
        [Description("Configuração de cada botão. Os botões aparecem por ordem que estão na lista")]
        public List<FuncaoBotaoMenu> ConfiguracoesBotoesMenu { get; set; } = new List<FuncaoBotaoMenu>();
        [Description("Configurações do botão de sair e do que mostra/esconde o menu")]
        public ConfigSair_Mostrar ConfiguracoesSair_Mostrar { get; set; } = new ConfigSair_Mostrar();
        [Description("Configurações dos Passo a Passo, pode haver vários passo a passo diferentes no mesmo perfil")]
        public List<ConfigPassoAPasso> ConfiguracoesPassoAPasso { get; set; } = new List<ConfigPassoAPasso>();

        public ConfigGeral()
        {
            ConfiguracoesBotoesMenu = new List<FuncaoBotaoMenu>();
            ConfiguracoesPassoAPasso = new List<ConfigPassoAPasso>();

            // Adiciona um botão padrão
            FuncaoBotaoMenu novoBotao = new FuncaoBotaoMenu();
            novoBotao.Validate();
            ConfiguracoesBotoesMenu.Add(novoBotao);

            // Adiciona um passo a passo padrão
            ConfigPassoAPasso novoPasso = new ConfigPassoAPasso();
            novoPasso.Validate();
            ConfiguracoesPassoAPasso.Add(novoPasso);

            // Configuração do botão de sair/mostrar
            ConfiguracoesSair_Mostrar = new ConfigSair_Mostrar();
            ConfiguracoesSair_Mostrar.Validate();
        }
        public void Validate()
        {
            ConfiguracoesSair_Mostrar?.Validate();

            PercentagemEcraParaBotoesAltura = Math.Max(10, Math.Min(PercentagemEcraParaBotoesAltura, 100));
            PercentagemEcraParaBotoesLargura = Math.Max(10, Math.Min(PercentagemEcraParaBotoesLargura, 100));
            BotoesPorLinha = Math.Max(0, Math.Min(BotoesPorLinha, 8));
            QuantidadeBotoes = Math.Max(0, Math.Min(QuantidadeBotoes, 8));

            if (ConfiguracoesBotoesMenu == null || ConfiguracoesBotoesMenu.Count == 0)
            {
                FuncaoBotaoMenu novoBotao = new FuncaoBotaoMenu();
                novoBotao.Validate();
                ConfiguracoesBotoesMenu.Add(novoBotao);
            }

            if (ConfiguracoesPassoAPasso != null)
            {
                foreach (ConfigPassoAPasso passo in ConfiguracoesPassoAPasso)
                    passo.Validate();
            }

            if (ConfiguracoesBotoesMenu != null)
            {
                foreach (FuncaoBotaoMenu botao in ConfiguracoesBotoesMenu)
                    botao.Validate();
            }
        }
    }
    //---------------------------------//
    // Configurações do Perfil //
    //---------------------------------//
    public class AppConfig
    {
        public string Perfil { get; set; } = "Padrao";
        [JsonIgnore]
        public int Id_Perfil { get; set; } = 0;
        public int IndiceTemaAtual { get; set; } = 0;
        public ConfigGeral ConfiguracaoAplicacao { get; set; } = new ConfigGeral();
        public AparenciaGeral VisualAplicacao { get; set; } = new AparenciaGeral();

        public AppConfig()  // Construtor default: utiliza os valores padrão definidos acima
        {
            if (ConfiguracaoAplicacao.ConfiguracoesBotoesMenu == null || ConfiguracaoAplicacao.ConfiguracoesBotoesMenu.Count == 0)
                ConfiguracaoAplicacao.ConfiguracoesBotoesMenu = new List<FuncaoBotaoMenu> { new FuncaoBotaoMenu() };

            if (ConfiguracaoAplicacao.ConfiguracoesPassoAPasso == null || ConfiguracaoAplicacao.ConfiguracoesPassoAPasso.Count == 0)
                ConfiguracaoAplicacao.ConfiguracoesPassoAPasso = new List<ConfigPassoAPasso> { new ConfigPassoAPasso() };

            if (VisualAplicacao.Tema == null || VisualAplicacao.Tema.Count == 0)
                VisualAplicacao.Tema = new List<TemaVisual> { new TemaVisual() };

            Validate(); // Garante que o objeto está válido ao ser criado
        }

        // Construtor que recebe uma configuração carregada (por exemplo, do JSON)
        // e faz merge com os valores padrão, validando em seguida.
        public AppConfig(AppConfig carregada) : this()
        {
            // Merge simples: se o valor carregado for diferente de null ou zero, copia-o
            if (!string.IsNullOrWhiteSpace(carregada.Perfil))
                Perfil = carregada.Perfil;
            if (carregada.Id_Perfil != 0)
                Id_Perfil = carregada.Id_Perfil;
            if (carregada.IndiceTemaAtual != 0)
                IndiceTemaAtual = carregada.IndiceTemaAtual;
            if (carregada.ConfiguracaoAplicacao != null)
                ConfiguracaoAplicacao = carregada.ConfiguracaoAplicacao;
            if (carregada.VisualAplicacao != null)
                VisualAplicacao = carregada.VisualAplicacao;

            Validate();  // Validação: garante que todos os valores estão corretos
        }
        public void Validate()// Método de validação que pode ser expandido com regras específicas para cada propriedade
        {
            if (string.IsNullOrWhiteSpace(Perfil))
                Perfil = "Padrao";

            ConfiguracaoAplicacao.Validate();  // Chama a validação, se existirem
            VisualAplicacao.Validate();  // Chama a validação, se existirem
            IndiceTemaAtual = Math.Max(0, Math.Min(IndiceTemaAtual, VisualAplicacao.Tema.Count - 1));
        }
    }
    //---------------------------
    // Padrão Singleton para gerir a instância única de Configuracoes
    //---------------------------
    public class AcederConfig
    {
        private static AcederConfig configAtual;
        public AppConfig AppConfig { get; set; }
        private AcederConfig()
        {
            AppConfig = Ficheiros.IniciarConfiguracoes();
            Configuracao_AtualizacaoForms.AtualizarTudo(); // Atualizar as configurações visuais após carregar todos os forms
            AppConfig.Validate();
        }
        public static AcederConfig ConfigAtual
        {
            get
            {
                if (configAtual == null)
                    configAtual = new AcederConfig();
                return configAtual;
            }
        }
    }
    public static class Ficheiros
    {
        public static string Caminho = "Pasta_Executavel";
        public static bool EnviarMensagens = false;
        public static bool EscreverVariaveisVaziasNoJson = true;
        public static int IdPerfilAtual = 0;

        public static AppConfig IniciarConfiguracoes()    // Subprograma que já atribui o valor de Caminho conforme a opção nas Settings
        {
            // Para recuperar as variáveis:
            // Verifica se as variáveis de ambiente já existem
            bool variaveisExistem = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CaminhoSalvamento", EnvironmentVariableTarget.User)) && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IdPerfilAtual", EnvironmentVariableTarget.User));
            string CaminhoGuardado = Caminho;
            if (variaveisExistem)
            {
                // Se existirem, carrega os valores
                 CaminhoGuardado = Environment.GetEnvironmentVariable("CaminhoSalvamento", EnvironmentVariableTarget.User);
                string idPerfil_String = Environment.GetEnvironmentVariable("IdPerfilAtual", EnvironmentVariableTarget.User);
                if (int.TryParse(idPerfil_String, out int IdPerfil_Int))
                    IdPerfilAtual = IdPerfil_Int;
                MessageBox.Show(CaminhoGuardado + IdPerfil_Int);

            }
            else
            {
                // Se não existirem, cria as variáveis de ambiente com os valores atuais
                Environment.SetEnvironmentVariable("CaminhoSalvamento", Caminho, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("IdPerfilAtual", IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);
            }

            switch (CaminhoGuardado) // Define o caminho com base na opção
            {
                case "Pasta_Executavel": Caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Perfil"); break;
                case "Pasta_Documentos": Caminho = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MenuBotoes", "Perfil"); break;
                case "Pasta_UserLocal": Caminho = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MenuBotoes", "Perfil"); break;
                case "Pasta_RecursoEmbutito": Caminho = "Pasta_RecursoEmbutito"; break;
                default: Caminho = Path.Combine(CaminhoGuardado, "Perfil"); break; // Se for outro valor, usa-o diretamente concatenando "Perfil"
            }
            AppConfig ConfiguracoesEmbutidas = CarregarConfiguracoesEmbutidas();
            if (ConfiguracoesEmbutidas == null)
            {
                VerificarPastaAoIniciar(CaminhoGuardado);
                ConfiguracoesEmbutidas = CarregarConfig(IdPerfilAtual);
            }

            ConfiguracoesEmbutidas = new AppConfig(ConfiguracoesEmbutidas);
            ConfiguracoesEmbutidas.Validate();
            return ConfiguracoesEmbutidas;
            //ConfigManager.Instancia.Config = CarregarConfig(IdPerfilAtual);  // Recarrega as configurações para atualizar o caminho
        }
        public static void VerificarPastaAoIniciar(string CaminhoGuardado)
        {
            // Primeiro, carrega as configurações para definir o caminho

            // Se a pasta já existir, tudo bem
            if (Directory.Exists(CaminhoGuardado))
                return;

            // Verifica a opção de caminho definida nas Settings

            switch (CaminhoGuardado)
            {
                case "Pasta_Executavel":
                case "Pasta_Documentos":
                case "Pasta_UserLocal":
                    // Cria a pasta automaticamente
                    Directory.CreateDirectory(Caminho);
                    LogUtility.EscreverNaLog($"Pasta criada automaticamente: {Caminho}");
                    break;
                default:
                    // Opção personalizada: mostra o caminho escolhido e pergunta se está correto
                    DialogResult resp = MessageBox.Show(
                        $"O caminho escolhido é:\n{Caminho}\nEstá correto?",
                        "Confirmação de Caminho",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resp == DialogResult.Yes)
                    {
                        Directory.CreateDirectory(Caminho);
                        MessageBox.Show($"Pasta criada: {Caminho}");
                    }
                    else
                    {
                        DialogResult resp2 = MessageBox.Show( // Pergunta se o usuário deseja mudar para "Pasta_Executavel"
                        "Deseja mudar o caminho para 'Pasta_Executavel'?",
                        "Alterar Caminho",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                        if (resp2 == DialogResult.Yes)
                        {
                            // Atualiza as Settings e a variável para Pasta_Executavel
                            Environment.SetEnvironmentVariable("CaminhoSalvamento", "Pasta_Executavel", EnvironmentVariableTarget.User);
                            Directory.CreateDirectory(Caminho);
                            MessageBox.Show($"Pasta criada: {Caminho}");
                            Application.Restart();
                        }
                        else
                        {
                            MessageBox.Show("Não é possível continuar sem um caminho válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                    break;
            }

            string[] subpastas =   // Criar as subpastas se não existirem
            {
                Path.Combine(Caminho, "ImagensPassoAPasso"),
                Path.Combine(Caminho, "ImagensPassoAPasso", "0"),
                Path.Combine(Caminho, "Documentos")
            };

            foreach (string subpasta in subpastas)
            {
                if (!Directory.Exists(subpasta))
                    Directory.CreateDirectory(subpasta);
            }
        }
        public static Image[] SalvarImagens(string[] arquivos)
        {
            Image[] imagens = new Image[arquivos.Length];
            for (int i = 0; i < arquivos.Length; i++)
            {
                imagens[i] = Image.FromFile(arquivos[i]);
            }
            return imagens;
        }
        private class ColorJsonConverter : JsonConverter<Color>
        {
            public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("R", value.R);
                writer.WriteNumber("G", value.G);
                writer.WriteNumber("B", value.B);
                writer.WriteEndObject();
            }

            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                int r = 0, g = 0, b = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "R":
                            r = reader.GetInt32();
                            break;
                        case "G":
                            g = reader.GetInt32();
                            break;
                        case "B":
                            b = reader.GetInt32();
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                return Color.FromArgb(255, r, g, b);
            }

        }
        private class FontJsonConverter : JsonConverter<Font>
        {
            public override void Write(Utf8JsonWriter writer, Font value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("Name", value.Name);
                writer.WriteNumber("Size", value.Size);
                writer.WriteString("Style", value.Style.ToString());
                writer.WriteEndObject();
            }
            public override Font Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                string nome = "Arial";
                float tamanho = 12f;
                FontStyle estilo = FontStyle.Regular;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    // Garante que estamos a ler um nome de propriedade
                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propertyName = reader.GetString();
                    reader.Read(); // Avança para o valor da propriedade

                    switch (propertyName)
                    {
                        case "Name":
                            nome = reader.GetString();
                            break;
                        case "Size":
                            tamanho = (float)reader.GetDouble();
                            break;
                        case "Style":
                            if (reader.TokenType == JsonTokenType.Number) // Se for número, converte diretamente
                            {
                                int intValue = reader.GetInt32();
                                estilo = (FontStyle)intValue;
                            }
                            else if (reader.TokenType == JsonTokenType.String) // Se for string, faz o parsing
                            {
                                string s = reader.GetString();
                                if (!Enum.TryParse(s, out estilo))
                                    estilo = FontStyle.Regular;
                            }
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                return new Font(nome, tamanho, estilo);
            }
        }
        public class DictionaryJsonConverter<TValue> : JsonConverter<Dictionary<string, TValue>>
        {
            public override Dictionary<string, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return JsonSerializer.Deserialize<Dictionary<string, TValue>>(ref reader, options);
            }
            public override void Write(Utf8JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
                    options = new JsonSerializerOptions(options) { DefaultIgnoreCondition = JsonIgnoreCondition.Never };

                foreach (var kvp in value)
                {
                    if (kvp.Value == null || kvp.Key == null)  // Ignora valores nulos manualmente
                        continue;
                    if (kvp.Key == "IsEmpty") // Ignora a chave "IsEmpty"
                        continue;

                    writer.WritePropertyName(kvp.Key);
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
        }
        public static string ObterCaminhoFicheiro(string motivo, string filtrarTipo = "*")
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = motivo;
                openFileDialog.Filter = string.IsNullOrWhiteSpace(filtrarTipo) || filtrarTipo == "*" ?
                    "Todos os ficheiros (*.*)|*.*" :
                    $"Ficheiros {filtrarTipo.ToUpper()} (*.{filtrarTipo})|*.{filtrarTipo}|Todos os ficheiros (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    return openFileDialog.FileName;
                return string.Empty;
            }
        }
        public static AppConfig CarregarConfig(int idPerfil)
        {
            if (Caminho == "Pasta_RecursoEmbutito")
                return new AppConfig(); // Retorna uma configuração padrão

            string caminhoFicheiro = Path.Combine(Caminho, $"Perfil_{idPerfil}.json");
            if (!File.Exists(caminhoFicheiro))
            {
                LogUtility.EscreverNaLog($"O ficheiro de configuração para o perfil {idPerfil} não foi encontrado.\nUsando configurações padrão.");
                return new AppConfig(); // Retorna uma configuração padrão
            }
            try
            {
                string json = File.ReadAllText(caminhoFicheiro);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    Converters =
                    {
                        new FontJsonConverter(),
                        new ColorJsonConverter(),
                        new DictionaryJsonConverter<object>()
                    }
                };

                AppConfig configCarregada = JsonSerializer.Deserialize<AppConfig>(json, options);
                if (configCarregada == null)
                    throw new Exception("Falha ao carregar a configuração. O ficheiro pode estar corrompido.");

                // Faz merge com os valores padrão e retorna a nova configuração
                return new AppConfig(configCarregada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar a configuração do perfil {idPerfil}:\n{ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new AppConfig(); // Retorna a configuração padrão em caso de erro
            }
        }
        public static void SalvarConfig(int idPerfil, AppConfig config)
        {
            if (config == null)
            {
                MessageBox.Show("Configuração inválida. O salvamento foi cancelado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(Caminho)) // Garante que a pasta existe
                Directory.CreateDirectory(Caminho);

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    Converters =
                    {
                        new FontJsonConverter(),
                        new ColorJsonConverter(),
                        new DictionaryJsonConverter<object>()
                    }
                };

                // Define a política de ignorar valores nulos ou não, conforme a flag
                if (EscreverVariaveisVaziasNoJson)
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                else
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(Path.Combine(Caminho, $"Perfil_{idPerfil}.json"), json);
                //Opcional: MessageBox.Show("Configuração salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a configuração do perfil {idPerfil}:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static AppConfig CarregarConfiguracoesEmbutidas()
        {
            string resourceName = "PerguntasFrequentesSuporte.Resources.ConfigPadrao.json";
            Assembly assembly = Assembly.GetExecutingAssembly();

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    return CarregarConfig(IdPerfilAtual);

                using var reader = new StreamReader(stream);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    Converters =
                    {
                        new FontJsonConverter(),
                        new ColorJsonConverter(),
                        new DictionaryJsonConverter<object>()
                    }
                };

                AppConfig configCarregada = JsonSerializer.Deserialize<AppConfig>(reader.ReadToEnd(), options);
                if (configCarregada != null)
                    return new AppConfig(configCarregada);
                else
                    return new AppConfig(CarregarConfig(IdPerfilAtual));
            }
            catch (Exception ex)
            {
                LogUtility.EscreverNaLog($"Erro ao carregar recurso: {ex.Message}");
                return new AppConfig(CarregarConfig(IdPerfilAtual));
            }
        }
    }
    public static class ClasseAuxiliar
    {
        public static int? ExtrairNumeroFinal(string nome)// Subprograma auxiliar para extrair o número final do nome de um controle
        {
            string digitos = "";
            for (int i = nome.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(nome[i]))
                    digitos = nome[i] + digitos;
                else
                    break;
            }
            if (string.IsNullOrEmpty(digitos))
                return null; // Retorna null se não houver números no final

            return Convert.ToInt32(digitos);
        }
        public static Button[] ObterBotoesOrdenados(Form form)  // Subprograma que percorre todos os controles do menu e retorna um array de botões ordenados
        {
            List<Button> botoes = new List<Button>();

            foreach (Control Controlo in form.Controls) // Percorre todos os controles do menu
            {
                if (Controlo is Button)
                    if (ExtrairNumeroFinal(Controlo.Name) != null)
                        botoes.Add((Button)Controlo);
            }

            botoes.Sort(delegate (Button A, Button B) // Ordena os botões pelo número que aparece no final do nome
            {
                int? numA = ExtrairNumeroFinal(A.Name);
                int? numB = ExtrairNumeroFinal(B.Name);
                if (numA < numB)
                    return -1;
                else
                {
                    if (numA > numB)
                        return 1;
                    else
                        return 0;
                }
            });

            return botoes.ToArray();
        }
    }
    public static class LogUtility
    {
        public static void EscreverNaLog(string message)
        {
            string logFile = "log.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao escrever no log: " + ex.Message);
            }
        }
    }
}

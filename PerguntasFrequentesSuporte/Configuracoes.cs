using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PerguntasFrequentesSuporte
{
    public partial class Configuracoes : Form 
        // Erro que não dá para editar listas ou arrays porque não encontra o pai,
        // quando se acede ao valor de alguma sub arvore o caminho fica apenas como subArvore.Variável e não como Arvore.SubArvore.Variável
        // falta fazer apresentar dicionários, onde cada chave aparece como um subvalor e o valor que se edita é o da valor associado á chave, e nunca se edita a chave em si
    {
        static AppConfig ConfiguracoesGlobais = AcederConfig.ConfigAtual.AppConfig;
        private ConfigGeral tempConfig;
        private AparenciaGeral tempAparencia;

        private static Dictionary<string, string[]> escolhasPreDefinidas = new Dictionary<string, string[]>
        {
            { "FontStyle", new[] { "Regular", "Bold", "Italic", "Underline", "Strikeout" } }
        };
        public Configuracoes()
        {
            InitializeComponent();
            // Configuração dos controles (TreeView, ListBox, Botões) deve ser feita no designer ou aqui
            // this.Load += Configuracoes_Load;
        }
        private void Configuracoes_Load(object sender, EventArgs e)
        {
            tempConfig = ConfiguracoesGlobais.ConfiguracaoAplicacao;
            tempAparencia = ConfiguracoesGlobais.VisualAplicacao;

            //tempConfig_Btn.Base.Fonte = InputBoxFont.Show("Selecione a fonte", tempConfig_Btn.Base.Fonte);

            // Constrói a TreeView a partir dos objetos temporários
            TreeViewConfig.Nodes.Clear();

            TreeNode nodeBtn = new TreeNode("ConfiguracaoAplicacao");
            nodeBtn.Tag = new ConfigItem { Localizacao = "ConfiguracaoAplicacao", Objeto = tempConfig };
            TreeViewConfig.Nodes.Add(nodeBtn);
            AdicionarMembrosRecursivo(tempConfig, nodeBtn);

            TreeNode nodeMenu = new TreeNode("VisualAplicacao");
            nodeMenu.Tag = new ConfigItem { Localizacao = "VisualAplicacao", Objeto = tempAparencia };
            TreeViewConfig.Nodes.Add(nodeMenu);
            AdicionarMembrosRecursivo(tempAparencia, nodeMenu);

            TreeViewConfig.CollapseAll();
        }

        // Se o membro for indesejado (para evitar "lixo")
        private bool ShouldSkipMember(MemberInfo member)
        {
            string name = member.Name.ToLowerInvariant();
            if (name.Contains("syncroot") ||
                name.Contains("longlength") ||
                name == "length" ||
                name == "rank" ||
                name == "isreadonly" ||
                name == "isfixedsize" ||
                name == "issynchronized" ||
                name.EndsWith("value__"))
            {
                return true;
            }
            return false;
        }

        // Subprograma recursivo para adicionar membros à TreeView

        private void AdicionarMembrosRecursivo(object Objeto, TreeNode parent)
        {
            if (Objeto == null)
                return;

            Type tipo = Objeto.GetType();

            // Se o objeto for uma lista ou array, cria um nó e adiciona cada elemento
            if (Objeto is IEnumerable<object> lista)
            {
                TreeNode listaNode = new TreeNode($"{FormatMemberName(parent.Text)} (Lista)");
                listaNode.Tag = new ConfigItem { Localizacao = parent.Text, Objeto = Objeto, MemberInfo = null };
                parent.Nodes.Add(listaNode);

                int index = 0;
                foreach (var item in lista)
                {
                    TreeNode itemNode = new TreeNode($"[{index}] {item}");
                    itemNode.Tag = new ConfigItem { Localizacao = $"{parent.Text}[{index}]", Objeto = item, MemberInfo = null };
                    listaNode.Nodes.Add(itemNode);

                    // Se o item da lista for um objeto complexo, chama a recursão
                    if (item != null && !item.GetType().IsPrimitive && item.GetType() != typeof(string) && !(item is Color))
                        AdicionarMembrosRecursivo(item, itemNode);

                    index++;
                }
                return;
            }

            // Se for um objeto do tipo Fonte, trata-o como folha (mostrar apenas FonteArray)
            if (Objeto is Font)
                return;  // Exibe somente o FonteArray

            // Se for do tipo Color, trata-o como folha
            if (Objeto is Color)
                return;
            if (Objeto.GetType().IsGenericType && Objeto.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return;

            // Processa propriedades
            foreach (PropertyInfo prop in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldSkipMember(prop))
                    continue;

                object valor = null;
                if (prop.GetIndexParameters().Length == 0) // Garante que não é uma propriedade indexadora
                {
                    try { valor = prop.GetValue(Objeto); } catch { MessageBox.Show(prop.Name); }
                }

                TreeNode node = new TreeNode($"{FormatMemberName(prop.Name)}");
                node.Tag = new ConfigItem { Localizacao = parent.Text + "." + prop.Name, Objeto = valor, MemberInfo = prop };
                parent.Nodes.Add(node);

                // Se o valor for complexo, chama a recursão
                if (valor != null && !prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string) && !(valor is Color))
                    AdicionarMembrosRecursivo(valor, node);
            }

            // Processa campos
            foreach (FieldInfo field in tipo.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldSkipMember(field))
                    continue;

                object valor = null;
                try { valor = field.GetValue(Objeto); } catch { }

                TreeNode node = new TreeNode($"{FormatMemberName(field.Name)}");
                node.Tag = new ConfigItem { Localizacao = parent.Text + "." + field.Name, Objeto = valor, MemberInfo = field };
                parent.Nodes.Add(node);

                if (valor != null && !field.FieldType.IsPrimitive && field.FieldType != typeof(string) && !(valor is Color))
                    AdicionarMembrosRecursivo(valor, node);
            }
        }

        // Formata o nome inserindo espaços antes das letras maiúsculas
        private string FormatMemberName(string name)
        {
            return name;
            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }

        // Formata o tipo para uma nomenclatura mais amigável (trata arrays)
        private string FormatTipo(string tipo)
        {
            // Se for array, processa o elemento e acrescenta "- Array"
            if (tipo.EndsWith("[]"))
            {
                string tipoBase = tipo.Substring(0, tipo.Length - 2);
                return $"{FormatTipo(tipoBase)} - Array";
            }

            // Remove prefixos indesejados (como "System." ou o namespace do projeto)
            if (tipo.StartsWith("System."))
                tipo = tipo.Substring("System.".Length);

            // Se o tipo contiver o namespace do projeto, remove-o (ajuste conforme necessário)
            if (tipo.StartsWith("PerguntasFrequentesSuporte."))
                tipo = tipo.Substring("PerguntasFrequentesSuporte.".Length);

            return tipo switch
            {
                "String" => "Texto",
                "Int32" => "Inteiro (32 bits)",
                "Int64" => "Inteiro (64 bits)",
                "Boolean" => "Verdadeiro ou Falso",
                "Color" => "Cor",
                "FontStyle" => "Estilo de Fonte",
                _ => tipo
            };
        }

        // Duplo clique na TreeView: edita o valor do membro selecionado
        private void TreeViewConfig_DoubleClick(object sender, EventArgs e)
        {
            // Esconde o menu (supondo que exista um formulário Menu)
            foreach (Form form in Application.OpenForms)
            {
                if (form is Menu)
                    form.Hide();
            }

            if (TreeViewConfig.SelectedNode == null)
                return;
            if (!(TreeViewConfig.SelectedNode.Tag is ConfigItem item))
                return;

            object ParenteDoObjeto = ObterObjetoPai(item.Localizacao);

            if (ParenteDoObjeto == null)
                MessageBox.Show($"Erro: Objeto pai não encontrado para '{item.Localizacao}'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Se for um caminho de imagem, abre o InputBoxImagem
            if (item.MemberInfo?.Name.Contains("Caminho") == true)
            {
                string novoCaminho = InputBoxImagem.Show(
                    $"Escolha a imagem para {FormatMemberName(TreeViewConfig.SelectedNode.Text)}:",
                    item.Objeto != null ? item.Objeto.ToString() : ""
                );

                if (!string.IsNullOrWhiteSpace(novoCaminho))
                {
                    // Define o diretório de destino
                    string diretorioDestino = Path.Combine(Application.StartupPath, "imagens");
                    if (!Directory.Exists(diretorioDestino))
                        Directory.CreateDirectory(diretorioDestino);

                    // Obtém o nome do arquivo e cria o novo caminho
                    string nomeArquivo = Path.GetFileName(novoCaminho);
                    string caminhoDestino = Path.Combine(diretorioDestino, nomeArquivo);

                    // Verifica se a imagem já existe
                    if (File.Exists(caminhoDestino))
                    {
                        DialogResult resultado = MessageBox.Show($"A imagem '{nomeArquivo}' já existe. Pretende substituí-la?", "Imagem existente", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (resultado == DialogResult.Yes)
                            File.Copy(novoCaminho, caminhoDestino, true);
                        else if (resultado == DialogResult.No)
                            return; // Mantém o valor antigo
                        else
                            return; // Cancela a edição
                    }
                    else
                        File.Copy(novoCaminho, caminhoDestino);


                    // Atualiza o valor no objeto real
                    if (item.MemberInfo is PropertyInfo prop)
                        prop.SetValue(ParenteDoObjeto, caminhoDestino);
                    else if (item.MemberInfo is FieldInfo field)
                        field.SetValue(ParenteDoObjeto, caminhoDestino);

                    item.Objeto = caminhoDestino;
                    
                    //TreeViewConfig.SelectedNode.Text = $"{FormatMemberName(item.Localizacao)} ({FormatTipo(item.Objeto.GetType().Name)}): {caminhoDestino}";
                }
            }
            // Se o valor for primitivo ou string
            else if (item.Objeto != null && (item.Objeto.GetType().IsPrimitive || item.Objeto is string))
            {
                string input = InputBoxTemp.Show($"Editar {FormatMemberName(item.Localizacao)}:", "Editar", item.Objeto?.ToString() ?? "");

                if (string.IsNullOrWhiteSpace(input))
                    input = null; // Permite definir valores nulos

                if (input != null)
                {
                    try
                    {
                        // Se a variável tem escolha pré-definida, chama o subprograma correspondente
                        if (escolhasPreDefinidas.ContainsKey(item.MemberInfo?.Name ?? ""))
                            input = ObterEscolhaPreDefinida(item.MemberInfo.Name, input);

                        object novoValor = Convert.ChangeType(input, item.Objeto?.GetType() ?? typeof(string));

                        if (ParenteDoObjeto == null)
                        {
                            MessageBox.Show($"Erro: Não foi possível encontrar o objeto pai para {item.Localizacao}");
                            return;
                        }

                        if (item.MemberInfo is PropertyInfo prop)
                            prop.SetValue(ParenteDoObjeto, novoValor);
                        else if (item.MemberInfo is FieldInfo field)
                            field.SetValue(ParenteDoObjeto, novoValor);


                        item.Objeto = novoValor;
                        TreeNode temp = TreeViewConfig.SelectedNode;
                        TreeViewConfig.SelectedNode = null;  // Remove temporariamente a seleção
                        TreeViewConfig.SelectedNode = temp;  // Restaura a seleção
                    }
                    catch (Exception ex)
                    {
                        LogUtility.EscreverNaLog("Erro ao converter o valor: " + ex.Message);
                    }
                }
            }
            // Se o valor for uma cor, chama o InputBoxColor
            else if (item.Objeto is Color)
            {
                Color novoValor = InputBoxColor.Show($"Escolha a nova cor de {FormatMemberName(TreeViewConfig.SelectedNode.Text)}:", (Color)item.Objeto);

                if (item.MemberInfo is PropertyInfo prop)
                    prop.SetValue(ParenteDoObjeto, novoValor);
                else if (item.MemberInfo is FieldInfo field)
                    field.SetValue(ParenteDoObjeto, novoValor);

                item.Objeto = novoValor;
                TreeNode temp = TreeViewConfig.SelectedNode;
                TreeViewConfig.SelectedNode = null;  // Remove temporariamente a seleção
                TreeViewConfig.SelectedNode = temp;  // Restaura a seleção
                //TreeViewConfig.SelectedNode.Text = $"{FormatMemberName(item.Localizacao)} ({FormatTipo(item.Objeto.GetType().Name)}): {novoValor}";
            }
        }
        private void TreeViewConfig_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBoxDetalhes.Items.Clear();
            if (e.Node.Tag is ConfigItem item)
            {
                listBoxDetalhes.Items.Add("Localização: " + item.Localizacao);

                string tipoFormatado;
                if (item.Objeto != null)
                {
                    // Se não for null, pega o tipo do objeto
                    if (item.Objeto is Array)
                    {
                        tipoFormatado = (item.Objeto as Array).Length > 0
                            ? $"{FormatTipo(item.Objeto.GetType().GetElementType().Name)} - Array"
                            : "Array";
                    }
                    else
                        tipoFormatado = FormatTipo(item.Objeto.GetType().Name);
                }
                else
                {
                    // Se for null, tenta obter o tipo pelo MemberInfo (propriedade ou campo)
                    if (item.MemberInfo is PropertyInfo prop)
                        tipoFormatado = FormatTipo(prop.PropertyType.Name);
                    else if (item.MemberInfo is FieldInfo field)
                        tipoFormatado = FormatTipo(field.FieldType.Name);
                    else
                        tipoFormatado = "null";
                }
                listBoxDetalhes.Items.Add("Tipo: " + tipoFormatado);
                listBoxDetalhes.Items.Add("Valor: " + (item.Objeto != null ? item.Objeto.ToString() : "null"));

                if (item.MemberInfo != null)
                {
                    var descAttr = item.MemberInfo.GetCustomAttribute<DescriptionAttribute>();
                    if (descAttr != null)
                        listBoxDetalhes.Items.Add("Descrição: " + descAttr.Description);
                }
            }
        }
        private string ObterEscolhaPreDefinida(string nome, string valorAtual)
        {
            if (escolhasPreDefinidas.ContainsKey(nome))
                return InputEscolha.Show($"Escolha um valor para {nome}:", "Escolha", escolhasPreDefinidas[nome], valorAtual);
            return valorAtual;
        }
        // Subprograma simplificado para obter o objeto pai com base na localização
        private object ObterObjetoPai(string localizacao)
        {
            object objetoAtual = ConfiguracoesGlobais;

            // Divide a localização pelos pontos
            string[] partes = localizacao.Split('.');

            for (int i = 0; i < partes.Length - 1; i++) // Última parte é a propriedade final
            {
                string parteAtual = partes[i];

                // Verifica se há um índice de array (ex.: "Botoes[0]")
                Match matchArray = Regex.Match(parteAtual, @"(.+?)\[(\d+)\]$");
                if (matchArray.Success)
                {
                    string nomeLista = matchArray.Groups[1].Value;
                    int indice = int.Parse(matchArray.Groups[2].Value);

                    // Obtém a propriedade que é uma lista
                    PropertyInfo prop = objetoAtual.GetType().GetProperty(nomeLista);
                    if (prop != null)
                    {
                        var lista = prop.GetValue(objetoAtual) as IList<object>;
                        if (lista != null && indice < lista.Count)
                        {
                            objetoAtual = lista[indice]; // Vai para o elemento correto da lista
                        }
                        else
                        {
                            return null; // Índice fora dos limites ou lista nula
                        }
                    }
                }
                // Verifica se é um dicionário (ex.: "TextoBotaoMostrarEsconder[BotoesVisiveis]")
                else if (parteAtual.Contains("[") && parteAtual.Contains("]"))
                {
                    Match matchDicionario = Regex.Match(parteAtual, @"(.+?)\[(.+?)\]$");
                    if (matchDicionario.Success)
                    {
                        string nomeDicionario = matchDicionario.Groups[1].Value;
                        string chave = matchDicionario.Groups[2].Value.Replace("\"", ""); // Remove aspas se houver

                        PropertyInfo prop = objetoAtual.GetType().GetProperty(nomeDicionario);
                        if (prop != null)
                        {
                            var dicionario = prop.GetValue(objetoAtual) as IDictionary<string, object>;
                            if (dicionario != null && dicionario.ContainsKey(chave))
                                objetoAtual = dicionario[chave]; // Vai para o valor correto do dicionário
                            else
                                return null; // Chave não encontrada
                        }
                    }
                }
                else
                {
                    // Se não for array nem dicionário, é uma propriedade normal
                    PropertyInfo prop = objetoAtual.GetType().GetProperty(parteAtual);
                    if (prop != null)
                        objetoAtual = prop.GetValue(objetoAtual);
                    else
                        return null; // Propriedade não encontrada
                }
            }

            return objetoAtual;
        }

        // Ao clicar em "Salvar Alterações", aplica as cópias temporárias aos dados originais
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Ficheiros.IdPerfilAtual++;
            // Environment.SetEnvironmentVariable("CaminhoSalvamento", Caminho, EnvironmentVariableTarget.User);
            Ficheiros.SalvarConfig(Ficheiros.IdPerfilAtual, ConfiguracoesGlobais);
            Environment.SetEnvironmentVariable("IdPerfilAtual", Ficheiros.IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);
            Application.Restart();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        // Classe para armazenar informações de cada nó
        public class ConfigItem
        {
            public string Localizacao { get; set; }
            public object Objeto { get; set; }
            public MemberInfo MemberInfo { get; set; }
        }
        public static class InputEscolha
        {
            public static string Show(string prompt, string title, string[] opcoes, string valorAtual)
            {
                Form form = new Form();
                Label label = new Label();
                ComboBox comboBox = new ComboBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;
                comboBox.Items.AddRange(opcoes);
                // Se o valorAtual estiver entre as opções, seleciona-o; caso contrário, seleciona o primeiro
                comboBox.SelectedItem = Array.Exists(opcoes, o => o.Equals(valorAtual)) ? valorAtual : opcoes[0];
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancelar";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 20, 372, 13);
                comboBox.SetBounds(12, 36, 372, 20);
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                label.AutoSize = true;
                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, comboBox, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult result = form.ShowDialog();
                return result == DialogResult.OK ? comboBox.SelectedItem.ToString() : valorAtual;
            }
        }

        // Implementação simples de InputBoxTemp (para texto)
        public static class InputBoxTemp
        {
            public static string Show(string prompt, string title, string defaultValue = "")
            {
                // Esconde o menu
                foreach (Form form2 in Application.OpenForms)
                {
                    if (form2 is Menu)
                        form2.Hide();
                    if (form2.Name.StartsWith("Input"))
                    {
                        form2.Close();
                        form2.Dispose();
                    }
                }


                Form form = new Form();
                Label label = new Label();
                TextBox textBoxErro = new TextBox(); //System.ArgumentException: 'Parameter is not valid.'
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;
                textBoxErro.Text = defaultValue;
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancelar";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 20, 372, 13);
                textBoxErro.SetBounds(12, 36, 372, 20);
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                label.AutoSize = true;
                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, textBoxErro, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = form.ShowDialog();

                // Mostra novamente o menu
                foreach (Form form3 in Application.OpenForms)
                {
                    if (form3 is Menu)
                        form3.Show();
                }

                return dialogResult == DialogResult.OK ? textBoxErro.Text : "";
            }
        }

        // Implementação de InputBoxColor para selecionar uma cor

        // Subprogramas de edição especial para certos tipos de configuração
        /*
        public static class EdicaoEspecial
        {
            public static BtnMudarEsconder_MostrarMenu EdicaoEspecialParaConfig_BtnMudarEsconder_MostrarMenu(BtnMudarEsconder_MostrarMenu atual)
            {
                MessageBox.Show("Edição especial para 'Config_BtnMudarEsconder_MostrarMenu' não implementada.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return atual;
            }

            public static JanelaMenu EdicaoEspecialParaConfig_Menu(JanelaMenu atual)
            {
                MessageBox.Show("Edição especial para 'Config_Menu' não implementada.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return atual;
            }

            public static JanelaPassoAPasso EdicaoEspecialParaConfig_PassoAPasso(JanelaPassoAPasso atual)
            {
                MessageBox.Show("Edição especial para 'Config_PassoAPasso' não implementada.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return atual;
            }
        }
        */
        private void comboBox1_Click(object sender, EventArgs    e)
        {
            BtnSalvar_Click(sender, e);
        }
    }
}

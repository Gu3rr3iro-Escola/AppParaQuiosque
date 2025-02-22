using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

public static class MessageBoxInterceptor
{
    // Variável de controlo para mostrar ou esconder MessageBoxes
    public static bool MostrarMensagens { get; set; } = true;

    static MessageBoxInterceptor()
    {
        // Hook para interceptar chamadas a MessageBox.Show()
        Application.ThreadException += (sender, e) =>
        {
            // Verifica se é uma chamada ao MessageBox
            var ex = e.Exception;
            if (ex.TargetSite.DeclaringType == typeof(MessageBox) &&
                ex.TargetSite.Name.StartsWith("Show"))
            {
                // Captura os parâmetros da MessageBox
                string titulo = "Mensagem Interceptada";
                string mensagem = ex.Message;

                // Se a variável estiver desligada, guarda no log e cancela a MessageBox
                if (!MostrarMensagens)
                {
                    GuardarNoLog(titulo, mensagem);
                }
                else
                {
                    // Caso contrário, mostra normalmente a MessageBox
                    MessageBox.Show("Esta Mensagem é enviada pelo Interceptor", titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show(mensagem, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        };
    }

    // Subprograma para guardar as informações no log.txt
    private static void GuardarNoLog(string titulo, string mensagem)
    {
        string caminhoLog = "log.txt";
        string dataHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string textoLog = $"{dataHora} | {titulo}: {mensagem}";

        try
        {
            File.AppendAllText(caminhoLog, textoLog + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao escrever no log: {ex.Message}");
        }
    }
}

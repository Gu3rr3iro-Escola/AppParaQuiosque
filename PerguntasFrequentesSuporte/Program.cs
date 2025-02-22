using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace PerguntasFrequentesSuporte
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Type interceptor = typeof(MessageBoxInterceptor);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Captura exce��es da UI principal (Windows Forms)
            Application.ThreadException += (sender, e) =>
            {
                TratarExcecaoGlobal(e.Exception);
            };

            // Captura exce��es n�o tratadas em threads secund�rias
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                TratarExcecaoGlobal(e.ExceptionObject as Exception);
            };

            try
            {
                using (var form = new Menu()) // Certifica-se que o form principal � corretamente descartado
                {
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                TratarExcecaoGlobal(ex);
            }

        }
        private static void TratarExcecaoGlobal(Exception ex)
        {
            if (ex != null)
                MessageBox.Show("Erro fatal: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FecharAplicacao();
        }

        private static void FecharAplicacao()
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    form.Dispose();  // Garante que todos os forms s�o fechados e liberados
                }

                Application.ExitThread();  // Fecha todas as threads de interface
                Environment.Exit(1);  // Fecha o processo completamente
            }
            catch
            {
                Process.GetCurrentProcess().Kill(); // Se falhar, for�a o encerramento imediato
            }
        }
    }
}
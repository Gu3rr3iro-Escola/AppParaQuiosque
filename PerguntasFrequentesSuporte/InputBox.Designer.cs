
namespace PerguntasFrequentesSuporte
{
    partial class InputBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CorDialog = new System.Windows.Forms.ColorDialog();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.SalvarFicheiro = new System.Windows.Forms.SaveFileDialog();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.EscolherPasta = new System.Windows.Forms.FolderBrowserDialog();
            this.FontDialog = new System.Windows.Forms.FontDialog();
            this.AbrirFicheiro = new System.Windows.Forms.OpenFileDialog();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.InputData = new System.Windows.Forms.TabPage();
            this.Calendario = new System.Windows.Forms.MonthCalendar();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Bool = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtMensagem = new System.Windows.Forms.Label();
            this.AbasInput = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.InputData.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.Bool.SuspendLayout();
            this.AbasInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // CorDialog
            // 
            this.CorDialog.AnyColor = true;
            this.CorDialog.FullOpen = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(463, 36);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(157, 23);
            this.comboBox1.TabIndex = 2;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(550, 270);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(94, 19);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // SalvarFicheiro
            // 
            this.SalvarFicheiro.FileName = "Salvar ficheiro";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(539, 382);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(161, 97);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(510, 206);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "Selecionar Cor";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(555, 114);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 23);
            this.textBox2.TabIndex = 12;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(659, 339);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 23);
            this.numericUpDown1.TabIndex = 11;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(659, 114);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(120, 76);
            this.checkedListBox1.TabIndex = 4;
            // 
            // FontDialog
            // 
            this.FontDialog.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            // 
            // AbrirFicheiro
            // 
            this.AbrirFicheiro.FileName = "Escolha um Ficheiro";
            this.AbrirFicheiro.ShowReadOnly = true;
            // 
            // InputData
            // 
            this.InputData.Controls.Add(this.Calendario);
            this.InputData.Location = new System.Drawing.Point(4, 24);
            this.InputData.Name = "InputData";
            this.InputData.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.InputData.Size = new System.Drawing.Size(385, 574);
            this.InputData.TabIndex = 3;
            this.InputData.Text = "Calendário";
            this.InputData.UseVisualStyleBackColor = true;
            // 
            // Calendario
            // 
            this.Calendario.Location = new System.Drawing.Point(12, 9);
            this.Calendario.MaxDate = new System.DateTime(2222, 12, 31, 0, 0, 0, 0);
            this.Calendario.MaxSelectionCount = 1;
            this.Calendario.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.Calendario.Name = "Calendario";
            this.Calendario.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(385, 574);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(385, 574);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "InputString";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(195, 23);
            this.textBox1.TabIndex = 9;
            // 
            // Bool
            // 
            this.Bool.Controls.Add(this.button2);
            this.Bool.Controls.Add(this.button1);
            this.Bool.Controls.Add(this.txtMensagem);
            this.Bool.Location = new System.Drawing.Point(4, 24);
            this.Bool.Name = "Bool";
            this.Bool.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Bool.Size = new System.Drawing.Size(385, 574);
            this.Bool.TabIndex = 0;
            this.Bool.Text = "Ativar ou Desativar";
            this.Bool.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.SpringGreen;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(3, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Ativado";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightCoral;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(84, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Desativar";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // txtMensagem
            // 
            this.txtMensagem.AutoSize = true;
            this.txtMensagem.Location = new System.Drawing.Point(35, 7);
            this.txtMensagem.Name = "txtMensagem";
            this.txtMensagem.Size = new System.Drawing.Size(110, 15);
            this.txtMensagem.TabIndex = 3;
            this.txtMensagem.Text = "Escolha uma opção";
            // 
            // AbasInput
            // 
            this.AbasInput.Controls.Add(this.Bool);
            this.AbasInput.Controls.Add(this.tabPage2);
            this.AbasInput.Controls.Add(this.tabPage1);
            this.AbasInput.Controls.Add(this.InputData);
            this.AbasInput.Location = new System.Drawing.Point(12, 12);
            this.AbasInput.Name = "AbasInput";
            this.AbasInput.SelectedIndex = 0;
            this.AbasInput.Size = new System.Drawing.Size(393, 602);
            this.AbasInput.TabIndex = 7;
            this.AbasInput.SelectedIndexChanged += new System.EventHandler(this.AbasInput_SelectedIndexChanged);
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 673);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.AbasInput);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.comboBox1);
            this.Name = "InputBox";
            this.Text = "InputBox";
            this.Load += new System.EventHandler(this.InputBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.InputData.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.Bool.ResumeLayout(false);
            this.Bool.PerformLayout();
            this.AbasInput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label txtMensagem;
        private System.Windows.Forms.MonthCalendar Calendario;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabControl AbasInput;
        private System.Windows.Forms.TabPage Bool;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.ColorDialog CorDialog;
        public System.Windows.Forms.SaveFileDialog SalvarFicheiro;
        public System.Windows.Forms.FolderBrowserDialog EscolherPasta;
        public System.Windows.Forms.FontDialog FontDialog;
        public System.Windows.Forms.OpenFileDialog AbrirFicheiro;
        public System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage InputData;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox2;
    }
}
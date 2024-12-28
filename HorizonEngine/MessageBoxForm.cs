using System;
using System.Windows.Forms;

namespace HorizonEngine
{
    public class MessageBoxForm : Form
    {
        private Button yesButton;
        private Button noButton;
        private Label messageLabel;

        public MessageBoxForm(string message)
        {
            // Inicializa os componentes do formulário
            this.Text = "Confirmação";
            this.Size = new System.Drawing.Size(300, 150);
            this.StartPosition = FormStartPosition.CenterParent;

            // Label com a mensagem
            messageLabel = new Label
            {
                Text = message,
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(250, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Botão "Sim"
            yesButton = new Button
            {
                Text = "Sim",
                Location = new System.Drawing.Point(40, 60),
                DialogResult = DialogResult.Yes
            };

            // Botão "Não"
            noButton = new Button
            {
                Text = "Não",
                Location = new System.Drawing.Point(150, 60),
                DialogResult = DialogResult.No
            };

            // Configura a caixa de diálogo para usar os botões
            this.Controls.Add(messageLabel);
            this.Controls.Add(yesButton);
            this.Controls.Add(noButton);

            this.AcceptButton = yesButton;
            this.CancelButton = noButton;
        }
    }
}

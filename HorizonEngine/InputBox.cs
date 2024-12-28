using System;
using System.Windows.Forms;

public class InputBox : Form
{
    private TextBox textBox;
    private Button okButton;
    public string InputText { get; private set; }

    // Construtor com parâmetro 'prompt' para uso geral
    public InputBox(string prompt)
    {
        InitializeComponent(prompt);
    }

    // Construtor sem parâmetro (caso queira usar 'using' sem passar o prompt)
    public InputBox()
    {
        InitializeComponent("Digite algo:");
    }

    private void InitializeComponent(string prompt)
    {
        this.Text = "Input Box";
        this.Width = 300;
        this.Height = 150;

        Label promptLabel = new Label
        {
            Text = prompt,
            Dock = DockStyle.Top,
            Height = 30
        };
        this.Controls.Add(promptLabel);

        textBox = new TextBox
        {
            Dock = DockStyle.Top
        };
        this.Controls.Add(textBox);

        okButton = new Button
        {
            Text = "OK",
            Dock = DockStyle.Bottom
        };
        okButton.Click += (sender, e) =>
        {
            InputText = textBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        };
        this.Controls.Add(okButton);
    }

    public static string Show(string prompt)
    {
        using (InputBox inputBox = new InputBox(prompt))
        {
            inputBox.ShowDialog();
            return inputBox.InputText;
        }
    }
}

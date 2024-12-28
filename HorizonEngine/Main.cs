using System;
using System.Windows.Forms;
using HorizonEngine;

namespace HorizonEngine
{
    public static class Program
    {
        // A variável gg deve ser acessada globalmente
        public static bool gg;

        // Ponto de entrada principal para a aplicação.
        [STAThread]
        public static void Main()
        {
            // Configurar o estilo visual da aplicação (necessário para aplicações de UI no Windows Forms)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Iniciar o formulário principal de Gerenciador de Projetos
            ProjectManagerUI projectManager = new ProjectManagerUI();
            Application.Run(projectManager);

            // Verificar se a variável gg está definida como true
            if (gg)
            {
                // Se o gg for verdadeiro, abrir o HorizonEditor
                HorizonEditor horizonEditor = new HorizonEditor();
                horizonEditor.ShowDialog(); // Use ShowDialog para manter o fluxo de execução correto
            }
        }
    }
}

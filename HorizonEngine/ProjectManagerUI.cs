using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Drawing;

namespace HorizonEngine
{
    public partial class ProjectManagerUI : MaterialForm
    {
        private string defaultIconPath = Path.Combine(Application.StartupPath, "default-icon.png");
        private FlowLayoutPanel projectFlowPanel;
        private string configFilePath = Path.Combine(Application.StartupPath, "config.json");
        private RichTextBox consoleOutput;
        private string engineName = "HorizonEngine";

        public ProjectManagerUI()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal500, Primary.Teal700, Primary.Teal100, Accent.LightBlue200, TextShade.WHITE);

            InitializeUI();
            LoadProjects();
        }

        private void InitializeUI()
        {
            this.Text = "HorizonEngine - Gerenciador de Projetos";
            this.Size = new System.Drawing.Size(1000, 700);

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(24, 24, 24)
            };

            var createProjectButton = new MaterialButton
            {
                Text = "Criar Projeto",
                Location = new Point(20, 10),
                Size = new Size(150, 40)
            };
            createProjectButton.Click += (s, e) => CreateProject();
            topPanel.Controls.Add(createProjectButton);

            this.Controls.Add(topPanel);

            projectFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10, 70, 10, 10)
            };
            this.Controls.Add(projectFlowPanel);

            consoleOutput = new RichTextBox
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10)
            };
            this.Controls.Add(consoleOutput);
        }

        private void CreateProject()
        {
            using (var inputDialog = new Form())
            {
                inputDialog.Text = "Nome do Projeto";
                inputDialog.Size = new Size(300, 150);
                inputDialog.StartPosition = FormStartPosition.CenterParent;

                var inputBox = new TextBox { Width = 200, Location = new Point(50, 20) };
                var confirmButton = new Button { Text = "Criar", DialogResult = DialogResult.OK, Location = new Point(100, 60) };

                inputDialog.Controls.Add(inputBox);
                inputDialog.Controls.Add(confirmButton);

                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string projectName = inputBox.Text.Trim();
                    if (!string.IsNullOrEmpty(projectName))
                    {
                        string projectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), engineName, projectName);

                        try
                        {
                            if (Directory.Exists(projectPath))
                            {
                                LogToConsole($"Erro: O diretório do projeto '{projectName}' já existe.");
                                return;
                            }

                            Directory.CreateDirectory(projectPath);
                            Directory.CreateDirectory(Path.Combine(projectPath, "World"));
                            Directory.CreateDirectory(Path.Combine(projectPath, "Assets"));
                            Directory.CreateDirectory(Path.Combine(projectPath, "Scripts"));

                            string worldJsonFilePath = Path.Combine(projectPath, "World", "World.json");
                            var viewportData = new { ViewportPosition = new { X = 0, Y = 0 } };
                            File.WriteAllText(worldJsonFilePath, JsonConvert.SerializeObject(viewportData, Formatting.Indented));

                            SaveLastOpenedProject(projectName);

                            LogToConsole($"Projeto '{projectName}' criado com sucesso!");
                            LoadProjects();
                        }
                        catch (Exception ex)
                        {
                            LogToConsole($"Erro ao criar o projeto '{projectName}': {ex.Message}");
                        }
                    }
                    else
                    {
                        LogToConsole("Erro: Nome do projeto não pode estar vazio.");
                    }
                }
            }
        }

        private void OpenProject(string projectFolder, string projectName)
        {
            string mainScriptPath = Path.Combine(projectFolder, "HorizonEditor.cs");

            // Verifique se o arquivo principal existe ou remova a verificação se não for necessário
            if (File.Exists(mainScriptPath))
            {
                try
                {
                    Program.gg = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    LogToConsole($"Erro ao abrir o projeto '{projectName}': {ex.Message}");
                }
            }
            else
            {
                Program.gg = false;
                // Caso não seja necessário o HorizonEditor.cs, você pode apenas permitir a abertura do projeto
                // LogToConsole($"Erro: O arquivo principal 'HorizonEditor.cs' não foi encontrado no projeto '{projectName}'.");
                LogToConsole($"Abrindo o projeto '{projectName}' sem arquivo 'HorizonEditor.cs'.");
                Program.gg = true;  // Permitir a abertura do projeto mesmo sem o arquivo
                this.Close();
            }
        }

        private void LoadProjects()
        {
            LogToConsole("Carregando projetos...");

            string projectsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), engineName);
            if (!Directory.Exists(projectsDirectory))
            {
                Directory.CreateDirectory(projectsDirectory);
            }

            projectFlowPanel.Controls.Clear();
            string[] projectFolders = Directory.GetDirectories(projectsDirectory);

            foreach (string projectFolder in projectFolders)
            {
                string projectName = Path.GetFileName(projectFolder);

                var projectPanel = new Panel
                {
                    Width = 200,
                    Height = 250,
                    Padding = new Padding(5),
                    Margin = new Padding(10),
                    BackColor = Color.FromArgb(30, 30, 30),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var projectLabel = new Label
                {
                    Text = projectName,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Width = 180,
                    Location = new Point(10, 130)
                };
                projectPanel.Controls.Add(projectLabel);

                var optionsButton = new MaterialButton
                {
                    Text = "...",
                    Location = new Point(160, 10),
                    Size = new Size(30, 30)
                };
                optionsButton.Click += (s, e) => ShowOptionsDialog(projectFolder, projectName);

                projectPanel.Controls.Add(optionsButton);
                projectPanel.Click += (s, e) => OpenProject(projectFolder, projectName);

                projectFlowPanel.Controls.Add(projectPanel);
            }
        }

        private void ShowOptionsDialog(string projectFolder, string projectName)
        {
            var optionsDialog = new Form
            {
                Text = "Opções do Projeto",
                Size = new Size(300, 150),
                StartPosition = FormStartPosition.CenterParent
            };

            var renameButton = new MaterialButton
            {
                Text = "Renomear Projeto",
                Location = new Point(50, 30),
                Size = new Size(200, 40)
            };
            renameButton.Click += (s, e) => RenameProject(projectFolder, projectName);

            var deleteButton = new MaterialButton
            {
                Text = "Excluir Projeto",
                Location = new Point(50, 80),
                Size = new Size(200, 40)
            };
            deleteButton.Click += (s, e) => DeleteProject(projectFolder, projectName);

            optionsDialog.Controls.Add(renameButton);
            optionsDialog.Controls.Add(deleteButton);

            optionsDialog.ShowDialog();
        }

        private void RenameProject(string projectFolder, string projectName)
        {
            using (var inputDialog = new Form())
            {
                inputDialog.Text = "Renomear Projeto";
                inputDialog.Size = new Size(300, 150);
                inputDialog.StartPosition = FormStartPosition.CenterParent;

                var inputBox = new TextBox { Width = 200, Location = new Point(50, 20) };
                var confirmButton = new Button { Text = "Renomear", DialogResult = DialogResult.OK, Location = new Point(100, 60) };

                inputDialog.Controls.Add(inputBox);
                inputDialog.Controls.Add(confirmButton);

                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string newProjectName = inputBox.Text.Trim();
                    if (!string.IsNullOrEmpty(newProjectName))
                    {
                        string newProjectFolder = Path.Combine(Path.GetDirectoryName(projectFolder), newProjectName);

                        try
                        {
                            Directory.Move(projectFolder, newProjectFolder);
                            LogToConsole($"Projeto renomeado de '{projectName}' para '{newProjectName}'.");

                            LoadProjects();
                        }
                        catch (Exception ex)
                        {
                            LogToConsole($"Erro ao renomear o projeto: {ex.Message}");
                        }
                    }
                    else
                    {
                        LogToConsole("Erro: O nome do projeto não pode ser vazio.");
                    }
                }
            }
        }

        private void DeleteProject(string projectFolder, string projectName)
        {
            var confirmDialog = MessageBox.Show($"Tem certeza que deseja excluir o projeto '{projectName}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmDialog == DialogResult.Yes)
            {
                try
                {
                    Directory.Delete(projectFolder, true);
                    LogToConsole($"Projeto '{projectName}' excluído com sucesso.");

                    LoadProjects();
                }
                catch (Exception ex)
                {
                    LogToConsole($"Erro ao excluir o projeto '{projectName}': {ex.Message}");
                }
            }
        }

        private void LogToConsole(string message)
        {
            consoleOutput.AppendText($"[{DateTime.Now}] {message}\n");
        }

        private void SaveLastOpenedProject(string projectName)
        {
            var config = new { LastOpenedProject = projectName };
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}

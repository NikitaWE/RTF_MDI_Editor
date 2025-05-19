using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RTF_MDI_Editor
{
    public class ChildForm : Form
    {
        private RichTextBox richTextBox;
        private string currentFile = "";
        private ContextMenuStrip contextMenu;

        public ChildForm()
        {
            Width = 800;
            Height = 600;
            Text = "Документ";

            // Настройка RichTextBox
            richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                AcceptsTab = true,
                Multiline = true,
                EnableAutoDragDrop = true,
                AllowDrop = true,
                DetectUrls = true,
                ScrollBars = RichTextBoxScrollBars.Both
            };

            // Обработка горячих клавиш
            richTextBox.KeyDown += RichTextBox_KeyDown;

            // Контекстное меню (ПКМ)
            InitializeContextMenu();

            Controls.Add(richTextBox);
        }

        // Обработка горячих клавиш: Ctrl+V, Ctrl+C, Ctrl+S
        private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.V)
                {
                    if (Clipboard.ContainsImage())
                    {
                        Image img = Clipboard.GetImage();
                        if (img != null)
                        {
                            Clipboard.SetImage(img);
                            richTextBox.Paste();
                            e.Handled = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.C)
                {
                    richTextBox.Copy();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.S)
                {
                    SaveFile();
                    e.Handled = true;
                }
            }
        }

        // Создание контекстного меню (ПКМ) с вырезанием и копированием
        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            var cutItem = new ToolStripMenuItem("Вырезать", null, (s, e) => richTextBox.Cut());
            var copyItem = new ToolStripMenuItem("Копировать", null, (s, e) => richTextBox.Copy());
            contextMenu.Items.AddRange(new[] { cutItem, copyItem });
            richTextBox.ContextMenuStrip = contextMenu;
        }

        // Загрузка файла (txt или rtf)
        public void LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext == ".rtf")
                    richTextBox.LoadFile(filePath);
                else
                    richTextBox.Text = File.ReadAllText(filePath);

                currentFile = filePath;
                Text = "Документ - " + Path.GetFileName(filePath);
            }
        }

        // Сохранение в текущий файл
        public void SaveFile()
        {
            if (!string.IsNullOrEmpty(currentFile))
            {
                SaveToFile(currentFile);
            }
            else
            {
                SaveFileAs();
            }
        }

        // Сохранение в новый файл
        public void SaveFileAs()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "RTF файлы|*.rtf|Текстовые файлы|*.txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                SaveToFile(saveDialog.FileName);
                currentFile = saveDialog.FileName;
                Text = "Документ - " + Path.GetFileName(currentFile);
            }
        }

        private void SaveToFile(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".rtf")
                richTextBox.SaveFile(path);
            else
                File.WriteAllText(path, richTextBox.Text);
        }

        // Вставка изображения вручную
        public void InsertImage()
        {
            var dialog = new OpenFileDialog { Filter = "Изображения|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(dialog.FileName);
                Clipboard.SetImage(img);
                richTextBox.Paste();
            }
        }
    }
}

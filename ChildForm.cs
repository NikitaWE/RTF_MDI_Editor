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
        private bool isModified = false;
        private ContextMenuStrip contextMenu;

        public ChildForm()
        {
            Width = 800;
            Height = 600;
            Text = "Документ";

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

            richTextBox.KeyDown += RichTextBox_KeyDown;
            richTextBox.TextChanged += (s, e) => isModified = true;

            InitializeContextMenu();

            Controls.Add(richTextBox);

            this.FormClosing += ChildForm_FormClosing;
        }

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

        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            var cutItem = new ToolStripMenuItem("Вырезать", null, (s, e) => richTextBox.Cut());
            var copyItem = new ToolStripMenuItem("Копировать", null, (s, e) => richTextBox.Copy());
            contextMenu.Items.AddRange(new[] { cutItem, copyItem });
            richTextBox.ContextMenuStrip = contextMenu;
        }

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
                isModified = false;
            }
        }

        public void SaveFile()
        {
            if (!string.IsNullOrEmpty(currentFile))
            {
                SaveToFile(currentFile);
                isModified = false;
            }
            else
            {
                SaveFileAs();
            }
        }

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
                isModified = false;
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

        public void InsertImage()
        {
            var dialog = new OpenFileDialog { Filter = "Изображения|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(dialog.FileName);
                Clipboard.SetImage(img);
                richTextBox.Paste();
                isModified = true;
            }
        }

        public void InsertTextFromFile()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы|*.txt;*.rtf|Все файлы|*.*",
                Title = "Выберите файл с текстом для вставки"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(openDialog.FileName).ToLower();
                string content = "";

                try
                {
                    if (ext == ".rtf")
                    {
                        using (var tempRtb = new RichTextBox())
                        {
                            tempRtb.LoadFile(openDialog.FileName);
                            content = tempRtb.Text;
                        }
                    }
                    else
                    {
                        content = File.ReadAllText(openDialog.FileName);
                    }

                    richTextBox.SelectedText = content;
                    isModified = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при вставке текста: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                var result = MessageBox.Show(
                    "Сохранить изменения перед закрытием?",
                    "Подтверждение выхода",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

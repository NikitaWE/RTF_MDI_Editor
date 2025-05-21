using System;
using System.Windows.Forms;

namespace RTF_MDI_Editor
{
    public class MainForm : Form
    {
        private MenuStrip menuStrip;

        public MainForm()
        {
            IsMdiContainer = true;
            Width = 1000;
            Height = 700;
            Text = "RTF MDI Редактор";

            menuStrip = new MenuStrip { Dock = DockStyle.Top };

            var fileMenu = new ToolStripMenuItem("Файл");
            fileMenu.DropDownItems.Add("Новый", null, NewFile);
            fileMenu.DropDownItems.Add("Открыть", null, OpenFile);
            fileMenu.DropDownItems.Add("Сохранить", null, SaveFile);
            fileMenu.DropDownItems.Add("Сохранить как", null, SaveFileAs);

            var insertMenu = new ToolStripMenuItem("Вставка");
            insertMenu.DropDownItems.Add("Вставить изображение", null, InsertImage);
            insertMenu.DropDownItems.Add("Вставить текст из файла", null, InsertText); // Обновлено

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(insertMenu);

            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
        }

        private void NewFile(object sender, EventArgs e)
        {
            var child = new ChildForm();
            child.MdiParent = this;
            child.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "RTF файлы|*.rtf" };
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var child = new ChildForm();
                child.MdiParent = this;
                child.LoadFile(openDialog.FileName);
                child.Show();
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (ActiveMdiChild is ChildForm child)
                child.SaveFile();
            else
                MessageBox.Show("Нет активного документа для сохранения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SaveFileAs(object sender, EventArgs e)
        {
            if (ActiveMdiChild is ChildForm child)
                child.SaveFileAs();
            else
                MessageBox.Show("Нет активного документа для сохранения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void InsertImage(object sender, EventArgs e)
        {
            if (ActiveMdiChild is ChildForm child)
                child.InsertImage();
            else
                MessageBox.Show("Нет активного документа для вставки изображения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void InsertText(object sender, EventArgs e)
        {
            if (ActiveMdiChild is ChildForm child)
                child.InsertTextFromFile(); // Новый метод
            else
                MessageBox.Show("Нет активного документа для вставки текста.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

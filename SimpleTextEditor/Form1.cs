using System.Runtime.InteropServices;

namespace SimpleTextEditor
{
    public partial class Form1 : Form
    {
        // Field to store the current file path
        private string? currentFilePath = null;

        public Form1()
        {
            InitializeComponent();
            // Set the form's default size on startup
            this.Size = new Size(1200, 1200); // Replace 800, 600 with your desired width and height

            // Subscribe to the TextBox TextChanged event to track unsaved changes
            textBox1.TextChanged += textBox1_TextChanged;

            // Check if the OS is macOS to adjust shortcut display
            bool isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            // Set up keyboard shortcuts and display strings
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.ShortcutKeyDisplayString = isMacOS ? "Command+O" : "Ctrl+O";

            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.ShortcutKeyDisplayString = isMacOS ? "Command+S" : "Ctrl+S";

            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.ShortcutKeyDisplayString = isMacOS ? "Command+Shift+S" : "Ctrl+Shift+S";

            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.ShortcutKeyDisplayString = isMacOS ? "Command+N" : "Ctrl+N";

            exitToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Q;
            exitToolStripMenuItem.ShortcutKeyDisplayString = isMacOS ? "Command+Q" : "Ctrl+Q";


            // Update the title for a new document on startup
            UpdateTitle();
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if there are unsaved changes
            if (textBox1.Modified)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to your current document?",
                          "Unsaved Changes",
                          MessageBoxButtons.YesNoCancel,
                          MessageBoxIcon.Warning);
                saveOnStateChangeIfModified(result);
            }

            // Clear the text box and reset the current file path
            textBox1.Clear();
            currentFilePath = null;

            // Reset the modified state of the TextBox and update the title
            textBox1.Modified = false;
            UpdateTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if there are unsaved changes
            if (textBox1.Modified)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to your current document?",
                          "Unsaved Changes",
                          MessageBoxButtons.YesNoCancel,
                          MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    // Clear the text box and reset the current file path
                    textBox1.Clear();
                    currentFilePath = null;

                    // Reset the modified state and update the title
                    textBox1.Modified = false;
                    UpdateTitle();
                }
                else
                {
                    saveOnStateChangeIfModified(result);
                }
            }

            // Show the Open File dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Read the content and display it in the TextBox
                    textBox1.Text = File.ReadAllText(openFileDialog.FileName);

                    // Set the current file path to the opened file
                    currentFilePath = openFileDialog.FileName;

                    // Reset the modified state and update the title
                    textBox1.Modified = false;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file. Original error: " + ex.Message);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if a file path is already specified
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                try
                {
                    // Save to the current file path
                    File.WriteAllText(currentFilePath, textBox1.Text);

                    // Reset the modified state and update the title
                    textBox1.Modified = false;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not save file. Original error: " + ex.Message);
                }
            }
            else
            {
                // If no file path, open Save As dialog
                SaveFileAs();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.Modified)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to your current document?",
                          "Unsaved Changes",
                          MessageBoxButtons.YesNoCancel,
                          MessageBoxIcon.Warning);
                saveOnStateChangeIfModified(result);
            }
            Application.Exit();
        }




        private void textBox1_TextChanged(object? sender, EventArgs e)
        {
            UpdateTitle();
        }

        // Update the form's title to reflect the file name and unsaved changes
        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
            this.Text = "SimpleTextEditor | " + fileName + (textBox1.Modified ? "*" : "");
        }

        // Helper method for Save As functionality
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Save the content of the TextBox to the selected file path
                    File.WriteAllText(saveFileDialog.FileName, textBox1.Text);

                    // Update the current file path to the new file
                    currentFilePath = saveFileDialog.FileName;

                    // Reset the modified state and update the title
                    textBox1.Modified = false;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not save file. Original error: " + ex.Message);
                }
            }
        }

        public void saveOnStateChangeIfModified(DialogResult result)
        {
            if (result == DialogResult.Yes)
            {
                // Save current document before executing operation
                if (string.IsNullOrEmpty(currentFilePath))
                {
                    SaveFileAs(); // Use Save As if no file path is set
                }
                else
                {
                    File.WriteAllText(currentFilePath, textBox1.Text);
                }
            }
            else if (result == DialogResult.Cancel)
            {
                // Cancel operation
                return;
            }
        }
    }
}

using csmaker.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class AddTeamDialog : Form
{
    public string TeamName { get; private set; } = "";
    private TextBox txtTeamName;
    private Button btnOK;
    private Button btnCancel;

    public AddTeamDialog()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Добавить команду";
        this.Size = new Size(400, 180);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblName = new Label
        {
            Text = "Название команды:",
            Location = new Point(20, 20),
            Size = new Size(340, 20)
        };

        txtTeamName = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25)
        };

        btnOK = new Button
        {
            Text = "OK",
            Location = new Point(190, 100),
            Size = new Size(80, 30),
            DialogResult = DialogResult.None
        };
        btnOK.Click += BtnOK_Click;

        btnCancel = new Button
        {
            Text = "Отмена",
            Location = new Point(280, 100),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        this.Controls.AddRange(new Control[] { lblName, txtTeamName, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void BtnOK_Click(object sender, EventArgs e)
    {
        var teamName = txtTeamName.Text.Trim();

        var validationResult = ValidationService.ValidateTeamName(teamName);
        if (!validationResult.IsValid)
        {
            MessageBox.Show(validationResult.Message, "Ошибка валидации",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ValidationService.IsTeamNameUnique(teamName))
        {
            MessageBox.Show("Команда с таким названием уже существует!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        TeamName = teamName;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
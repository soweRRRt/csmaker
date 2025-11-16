using csmaker.Services;
using System.Drawing;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class AddTeamDialog : Form
{
    public string TeamName { get; private set; } = "";
    private TextBox txtTeamName;

    public AddTeamDialog()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Добавить команду";
        this.Size = new Size(400, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblName = new Label
        {
            Text = "Название команды:",
            Location = new Point(20, 20),
            AutoSize = true
        };

        txtTeamName = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25)
        };

        var btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(190, 80),
            Size = new Size(80, 30)
        };

        var btnCancel = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Location = new Point(280, 80),
            Size = new Size(80, 30)
        };

        btnOK.Click += (s, e) =>
        {
            var teamName = txtTeamName.Text.Trim();

            var validationResult = ValidationService.ValidateTeamName(teamName);
            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.Message, "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!ValidationService.IsTeamNameUnique(teamName))
            {
                MessageBox.Show("Команда с таким названием уже существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            TeamName = teamName;
        };

        this.Controls.AddRange(new Control[] { lblName, txtTeamName, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}

using csmaker.Models;
using csmaker.Services;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class MovePlayerDialog : Form
{
    public Team? SelectedTeam { get; private set; }
    private ComboBox cmbTeam;
    private Player player;

    public MovePlayerDialog(Player player)
    {
        this.player = player;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = $"Переместить игрока: {player.Nickname}";
        this.Size = new Size(400, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblTeam = new Label
        {
            Text = "Новая команда:",
            Location = new Point(20, 20),
            AutoSize = true
        };

        cmbTeam = new ComboBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbTeam.Items.Add("[Без команды]");
        foreach (var team in DataService.Teams)
        {
            var info = $"{team.Name} ({team.Players.Count}/5)";
            cmbTeam.Items.Add(info);
        }
        cmbTeam.SelectedIndex = 0;

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
            var selected = cmbTeam.SelectedItem?.ToString();
            if (selected == "[Без команды]")
            {
                SelectedTeam = null;
            }
            else if (selected != null)
            {
                var teamName = selected.Substring(0, selected.LastIndexOf(" ("));
                SelectedTeam = DataService.Teams.FirstOrDefault(t => t.Name == teamName);
            }
        };

        this.Controls.AddRange(new Control[] { lblTeam, cmbTeam, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}
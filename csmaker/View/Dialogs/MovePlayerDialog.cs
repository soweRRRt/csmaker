using csmaker.Models;
using csmaker.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class MovePlayerDialog : Form
{
    public Team? SelectedTeam { get; private set; }
    private ComboBox cmbTeam;
    private Player player;
    private Button btnOK;
    private Button btnCancel;

    public MovePlayerDialog(Player player)
    {
        this.player = player;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = $"Переместить игрока: {player.Nickname}";
        this.Size = new Size(400, 180);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblTeam = new Label
        {
            Text = "Новая команда:",
            Location = new Point(20, 20),
            Size = new Size(340, 20)
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

        this.Controls.AddRange(new Control[] { lblTeam, cmbTeam, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void BtnOK_Click(object sender, EventArgs e)
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

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
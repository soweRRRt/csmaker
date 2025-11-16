using csmaker.Models;
using csmaker.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class AddPlayerDialog : Form
{
    public string PlayerNickname { get; private set; } = "";
    public string PlayerCountry { get; private set; } = "UA";
    public Team? SelectedTeam { get; private set; }

    private TextBox txtNickname;
    private ComboBox cmbCountry;
    private ComboBox cmbTeam;

    public AddPlayerDialog(Team? currentTeam)
    {
        SelectedTeam = currentTeam;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Добавить игрока";
        this.Size = new Size(400, 220);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblNickname = new Label
        {
            Text = "Никнейм:",
            Location = new Point(20, 20),
            AutoSize = true
        };

        txtNickname = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25)
        };

        var lblCountry = new Label
        {
            Text = "Страна:",
            Location = new Point(20, 75),
            AutoSize = true
        };

        cmbCountry = new ComboBox
        {
            Location = new Point(20, 100),
            Size = new Size(340, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var countries = CountryProvider.GetAllCountries();
        cmbCountry.Items.AddRange(countries.Cast<object>().ToArray());

        var defaultIndex = Array.IndexOf(countries, "UA");
        cmbCountry.SelectedIndex = defaultIndex >= 0 ? defaultIndex : 0;

        var lblTeam = new Label
        {
            Text = "Команда:",
            Location = new Point(20, 130),
            AutoSize = true
        };

        cmbTeam = new ComboBox
        {
            Location = new Point(20, 155),
            Size = new Size(340, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbTeam.Items.Add("[Без команды]");
        foreach (var team in DataService.Teams)
            cmbTeam.Items.Add(team.Name);

        if (SelectedTeam != null)
            cmbTeam.SelectedItem = SelectedTeam.Name;
        else
            cmbTeam.SelectedIndex = 0;

        var btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(190, 195),
            Size = new Size(80, 30)
        };

        var btnCancel = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Location = new Point(280, 195),
            Size = new Size(80, 30)
        };

        btnOK.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                MessageBox.Show("Введите никнейм!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            PlayerNickname = txtNickname.Text.Trim();
            PlayerCountry = cmbCountry.SelectedItem?.ToString() ?? "UA";

            var teamName = cmbTeam.SelectedItem?.ToString();
            SelectedTeam = teamName == "[Без команды]"
                ? null
                : DataService.Teams.FirstOrDefault(t => t.Name == teamName);
        };

        this.Controls.AddRange(new Control[] {
            lblNickname, txtNickname, lblCountry, cmbCountry,
            lblTeam, cmbTeam, btnOK, btnCancel
        });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}
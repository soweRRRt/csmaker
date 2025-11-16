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
    private Button btnOK;
    private Button btnCancel;

    public AddPlayerDialog(Team? currentTeam)
    {
        SelectedTeam = currentTeam;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Добавить игрока";
        this.Size = new Size(400, 280);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblNickname = new Label
        {
            Text = "Никнейм:",
            Location = new Point(20, 20),
            Size = new Size(340, 20)
        };

        txtNickname = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25)
        };

        var lblCountry = new Label
        {
            Text = "Страна:",
            Location = new Point(20, 80),
            Size = new Size(340, 20)
        };

        cmbCountry = new ComboBox
        {
            Location = new Point(20, 105),
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
            Location = new Point(20, 140),
            Size = new Size(340, 20)
        };

        cmbTeam = new ComboBox
        {
            Location = new Point(20, 165),
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

        btnOK = new Button
        {
            Text = "OK",
            Location = new Point(190, 210),
            Size = new Size(80, 30),
            DialogResult = DialogResult.None
        };
        btnOK.Click += BtnOK_Click;

        btnCancel = new Button
        {
            Text = "Отмена",
            Location = new Point(280, 210),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        this.Controls.AddRange(new Control[] {
            lblNickname, txtNickname, lblCountry, cmbCountry,
            lblTeam, cmbTeam, btnOK, btnCancel
        });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void BtnOK_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNickname.Text))
        {
            MessageBox.Show("Введите никнейм!", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        PlayerNickname = txtNickname.Text.Trim();
        PlayerCountry = cmbCountry.SelectedItem?.ToString() ?? "UA";

        var teamName = cmbTeam.SelectedItem?.ToString();
        SelectedTeam = teamName == "[Без команды]"
            ? null
            : DataService.Teams.FirstOrDefault(t => t.Name == teamName);

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
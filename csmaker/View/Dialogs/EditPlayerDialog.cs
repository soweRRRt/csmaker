using csmaker.Models;
using csmaker.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class EditPlayerDialog : Form
{
    public string PlayerCountry { get; private set; }
    private ComboBox cmbCountry;
    private Player player;
    private Button btnOK;
    private Button btnCancel;

    public EditPlayerDialog(Player player)
    {
        this.player = player;
        this.PlayerCountry = player.Country;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = $"Редактировать игрока: {player.Nickname}";
        this.Size = new Size(400, 180);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblCountry = new Label
        {
            Text = "Страна:",
            Location = new Point(20, 20),
            Size = new Size(340, 20)
        };

        cmbCountry = new ComboBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var countries = CountryProvider.GetAllCountries();
        cmbCountry.Items.AddRange(countries.Cast<object>().ToArray());

        cmbCountry.SelectedItem = player.Country;
        if (cmbCountry.SelectedIndex == -1)
            cmbCountry.SelectedIndex = 0;

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

        this.Controls.AddRange(new Control[] { lblCountry, cmbCountry, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void BtnOK_Click(object sender, EventArgs e)
    {
        PlayerCountry = cmbCountry.SelectedItem?.ToString() ?? player.Country;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
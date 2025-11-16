using csmaker.Models;
using csmaker.Services;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Dialogs;

public class EditPlayerDialog : Form
{
    public string PlayerCountry { get; private set; }
    private ComboBox cmbCountry;
    private Player player;

    public EditPlayerDialog(Player player)
    {
        this.player = player;
        this.PlayerCountry = player.Country;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = $"Редактировать игрока: {player.Nickname}";
        this.Size = new Size(400, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblCountry = new Label
        {
            Text = "Страна:",
            Location = new Point(20, 20),
            AutoSize = true
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
            PlayerCountry = cmbCountry.SelectedItem?.ToString() ?? player.Country;
        };

        this.Controls.AddRange(new Control[] { lblCountry, cmbCountry, btnOK, btnCancel });
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}
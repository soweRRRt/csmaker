using csmaker.Models;
using csmaker.Services;
using csmaker.Utilities;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsView = System.Windows.Forms.View;

namespace csmaker;

public partial class TeamsFormOld : Form
{
    private ListBox lstTeams;
    private ListView lvPlayers;
    private Label lblTeam;
    private ImageList imgCountries;
    private Button btnAddTeam;
    private Button btnAddPlayer;
    private Button btnMovePlayer;

    public TeamsFormOld()
    {
        InitializeComponent();
        LoadTeams();
    }

    private void InitializeComponent()
    {
        this.lstTeams = new ListBox();
        this.lvPlayers = new ListView();
        this.lblTeam = new Label();
        this.imgCountries = new ImageList();
        this.btnAddTeam = new Button();
        this.btnAddPlayer = new Button();
        this.btnMovePlayer = new Button();

        // lstTeams
        this.lstTeams.Location = new Point(12, 12);
        this.lstTeams.Size = new Size(200, 400);
        this.lstTeams.SelectedIndexChanged += LstTeams_SelectedIndexChanged;

        // lblTeam
        this.lblTeam.AutoSize = true;
        this.lblTeam.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.lblTeam.Location = new Point(230, 12);
        this.lblTeam.Text = "Выберите команду";

        // lvPlayers
        this.lvPlayers.Location = new Point(230, 45);
        this.lvPlayers.Size = new Size(350, 330);
        this.lvPlayers.View = WinFormsView.Details;
        this.lvPlayers.FullRowSelect = true;
        this.lvPlayers.SmallImageList = this.imgCountries;
        this.lvPlayers.Columns.Add("Ник", 200);
        this.lvPlayers.Columns.Add("Страна", 100);

        // imgCountries
        this.imgCountries.ImageSize = new Size(24, 16);
        this.imgCountries.ColorDepth = ColorDepth.Depth32Bit;

        // btnAddTeam
        this.btnAddTeam.Text = "Добавить команду";
        this.btnAddTeam.Location = new Point(12, 420);
        this.btnAddTeam.Size = new Size(200, 30);
        this.btnAddTeam.Click += BtnAddTeam_Click;

        // btnAddPlayer
        this.btnAddPlayer.Text = "Добавить игрока";
        this.btnAddPlayer.Location = new Point(230, 385);
        this.btnAddPlayer.Size = new Size(160, 30);
        this.btnAddPlayer.Click += BtnAddPlayer_Click;

        // btnMovePlayer
        this.btnMovePlayer.Text = "Переместить игрока";
        this.btnMovePlayer.Location = new Point(420, 385);
        this.btnMovePlayer.Size = new Size(160, 30);
        this.btnMovePlayer.Click += BtnMovePlayer_Click;

        // Form
        this.ClientSize = new Size(600, 460);
        this.Controls.AddRange(new Control[] {
            lstTeams, lvPlayers, lblTeam,
            btnAddTeam, btnAddPlayer, btnMovePlayer
        });
        this.Text = "Команды и игроки";
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void LoadTeams()
    {
        lstTeams.Items.Clear();
        foreach (var team in DataService.Teams)
            lstTeams.Items.Add(team.Name);
        lstTeams.Items.Add("[Без команды]");
    }

    private void LstTeams_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTeams.SelectedIndex < 0) return;

        string teamName = lstTeams.SelectedItem.ToString();
        Team? team = teamName == "[Без команды]" ? null : DataService.Teams.FirstOrDefault(t => t.Name == teamName);

        lblTeam.Text = team == null ? "Игроки без команды" : $"Команда: {team.Name}";
        lvPlayers.Items.Clear();
        imgCountries.Images.Clear();

        var players = team == null
            ? DataService.Players.Where(p => p.Team == null)
            : team.Players;

        foreach (var player in players)
        {
            var flag = player.GetCountryImage();
            imgCountries.Images.Add(player.Nickname, flag ?? new Bitmap(24, 16));

            var item = new ListViewItem(player.Nickname) { ImageKey = player.Nickname };
            item.SubItems.Add(player.Country);
            lvPlayers.Items.Add(item);
        }
    }

    private void BtnAddTeam_Click(object? sender, EventArgs e)
    {
        string? name = Prompt.ShowDialog("Введите название команды:", "Новая команда");
        if (string.IsNullOrWhiteSpace(name)) return;

        if (DataService.Teams.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Команда с таким названием уже существует!");
            return;
        }

        var team = new Team { Name = name };
        DataService.Teams.Add(team);
        DataService.GameData.Save("gameData.json");
        LoadTeams();
    }

    private void BtnAddPlayer_Click(object? sender, EventArgs e)
    {
        string? nickname = Prompt.ShowDialog("Введите ник игрока:", "Новый игрок");
        if (string.IsNullOrWhiteSpace(nickname)) return;

        string? country = Prompt.ShowDialog("Введите страну (например, UA):", "Новый игрок");
        if (string.IsNullOrWhiteSpace(country)) country = "??";

        var player = new Player { Nickname = nickname, Country = country };
        DataService.Players.Add(player);
        DataService.GameData.Save("gameData.json");
        LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
    }

    private void BtnMovePlayer_Click(object? sender, EventArgs e)
    {
        if (lvPlayers.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите игрока для перемещения.");
            return;
        }

        var playerName = lvPlayers.SelectedItems[0].Text;
        var player = DataService.Players.FirstOrDefault(p => p.Nickname == playerName);
        if (player == null) return;

        string? targetTeamName = Prompt.ShowDialog("Введите название команды (или оставьте пустым для снятия с команды):", "Переместить игрока");
        if (string.IsNullOrWhiteSpace(targetTeamName))
        {
            player.Team?.RemovePlayer(player);
            player.Team = null;
        }
        else
        {
            var team = DataService.Teams.FirstOrDefault(t => t.Name == targetTeamName);
            if (team == null)
            {
                MessageBox.Show("Команда не найдена.");
                return;
            }

            player.Team?.RemovePlayer(player);
            team.AddPlayer(player);
        }

        DataService.GameData.Save("gameData.json");
        LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
    }
}


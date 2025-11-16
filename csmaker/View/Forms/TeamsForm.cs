using csmaker.Models;
using csmaker.Services;
using csmaker.Utilities;
using csmaker.View.Dialogs;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace csmaker.View.Forms;

public partial class TeamsForm : Form
{
    private ListBox lstTeams;
    private ListView lvPlayers;
    private Label lblTeam;
    private Label lblTeamInfo;
    private ImageList imgCountries;
    private Button btnAddTeam;
    private Button btnDeleteTeam;
    private Button btnAddPlayer;
    private Button btnEditPlayer;
    private Button btnDeletePlayer;
    private Button btnMovePlayer;
    private Panel panelTeamActions;
    private Panel panelPlayerActions;
    private GroupBox grpTeams;
    private GroupBox grpPlayers;

    public TeamsForm()
    {
        InitializeComponent();
        LoadTeams();
    }

    private void InitializeComponent()
    {
        this.lstTeams = new ListBox();
        this.lvPlayers = new ListView();
        this.lblTeam = new Label();
        this.lblTeamInfo = new Label();
        this.imgCountries = new ImageList();
        this.btnAddTeam = new Button();
        this.btnDeleteTeam = new Button();
        this.btnAddPlayer = new Button();
        this.btnEditPlayer = new Button();
        this.btnDeletePlayer = new Button();
        this.btnMovePlayer = new Button();
        this.panelTeamActions = new Panel();
        this.panelPlayerActions = new Panel();
        this.grpTeams = new GroupBox();
        this.grpPlayers = new GroupBox();

        // grpTeams
        this.grpTeams.Text = "Команды (по рейтингу)";
        this.grpTeams.Location = new Point(12, 12);
        this.grpTeams.Size = new Size(280, 500);

        // lstTeams
        this.lstTeams.Location = new Point(10, 25);
        this.lstTeams.Size = new Size(260, 410);
        this.lstTeams.SelectedIndexChanged += LstTeams_SelectedIndexChanged;
        this.lstTeams.Font = new Font("Consolas", 9F);

        // panelTeamActions
        this.panelTeamActions.Location = new Point(10, 440);
        this.panelTeamActions.Size = new Size(260, 50);

        // btnAddTeam
        this.btnAddTeam.Text = "➕ Добавить";
        this.btnAddTeam.Location = new Point(0, 0);
        this.btnAddTeam.Size = new Size(125, 35);
        this.btnAddTeam.Click += BtnAddTeam_Click;
        this.btnAddTeam.FlatStyle = FlatStyle.Flat;
        this.btnAddTeam.BackColor = Color.FromArgb(76, 175, 80);
        this.btnAddTeam.ForeColor = Color.White;
        this.btnAddTeam.Cursor = Cursors.Hand;

        // btnDeleteTeam
        this.btnDeleteTeam.Text = "🗑️ Удалить";
        this.btnDeleteTeam.Location = new Point(135, 0);
        this.btnDeleteTeam.Size = new Size(125, 35);
        this.btnDeleteTeam.Click += BtnDeleteTeam_Click;
        this.btnDeleteTeam.FlatStyle = FlatStyle.Flat;
        this.btnDeleteTeam.BackColor = Color.FromArgb(244, 67, 54);
        this.btnDeleteTeam.ForeColor = Color.White;
        this.btnDeleteTeam.Cursor = Cursors.Hand;

        this.panelTeamActions.Controls.AddRange(new Control[] { btnAddTeam, btnDeleteTeam });
        this.grpTeams.Controls.AddRange(new Control[] { lstTeams, panelTeamActions });

        // grpPlayers
        this.grpPlayers.Text = "Игроки команды";
        this.grpPlayers.Location = new Point(305, 12);
        this.grpPlayers.Size = new Size(650, 500);

        // lblTeam
        this.lblTeam.AutoSize = true;
        this.lblTeam.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTeam.Location = new Point(15, 25);
        this.lblTeam.Text = "Выберите команду";
        this.lblTeam.ForeColor = Color.FromArgb(33, 150, 243);

        // lblTeamInfo
        this.lblTeamInfo.AutoSize = false;
        this.lblTeamInfo.Font = new Font("Segoe UI", 9F);
        this.lblTeamInfo.Location = new Point(15, 55);
        this.lblTeamInfo.Size = new Size(620, 22);
        this.lblTeamInfo.Text = "";
        this.lblTeamInfo.ForeColor = Color.Gray;

        // lvPlayers
        this.lvPlayers.Location = new Point(15, 85);
        this.lvPlayers.Size = new Size(620, 340);
        this.lvPlayers.View = System.Windows.Forms.View.Details;
        this.lvPlayers.FullRowSelect = true;
        this.lvPlayers.GridLines = true;
        this.lvPlayers.SmallImageList = this.imgCountries;
        this.lvPlayers.Columns.Add("Ник игрока", 250);
        this.lvPlayers.Columns.Add("Страна", 150);
        this.lvPlayers.Columns.Add("Статус", 180);
        this.lvPlayers.SelectedIndexChanged += LvPlayers_SelectedIndexChanged;

        // imgCountries
        this.imgCountries.ImageSize = new Size(24, 16);
        this.imgCountries.ColorDepth = ColorDepth.Depth32Bit;

        // panelPlayerActions
        this.panelPlayerActions.Location = new Point(15, 435);
        this.panelPlayerActions.Size = new Size(620, 50);

        // btnAddPlayer
        this.btnAddPlayer.Text = "➕ Добавить игрока";
        this.btnAddPlayer.Location = new Point(0, 0);
        this.btnAddPlayer.Size = new Size(150, 40);
        this.btnAddPlayer.Click += BtnAddPlayer_Click;
        this.btnAddPlayer.FlatStyle = FlatStyle.Flat;
        this.btnAddPlayer.BackColor = Color.FromArgb(76, 175, 80);
        this.btnAddPlayer.ForeColor = Color.White;
        this.btnAddPlayer.Cursor = Cursors.Hand;

        // btnEditPlayer
        this.btnEditPlayer.Text = "✏️ Редактировать";
        this.btnEditPlayer.Location = new Point(160, 0);
        this.btnEditPlayer.Size = new Size(150, 40);
        this.btnEditPlayer.Click += BtnEditPlayer_Click;
        this.btnEditPlayer.FlatStyle = FlatStyle.Flat;
        this.btnEditPlayer.BackColor = Color.FromArgb(33, 150, 243);
        this.btnEditPlayer.ForeColor = Color.White;
        this.btnEditPlayer.Cursor = Cursors.Hand;
        this.btnEditPlayer.Enabled = false;

        // btnMovePlayer
        this.btnMovePlayer.Text = "↔️ Переместить";
        this.btnMovePlayer.Location = new Point(320, 0);
        this.btnMovePlayer.Size = new Size(145, 40);
        this.btnMovePlayer.Click += BtnMovePlayer_Click;
        this.btnMovePlayer.FlatStyle = FlatStyle.Flat;
        this.btnMovePlayer.BackColor = Color.FromArgb(255, 152, 0);
        this.btnMovePlayer.ForeColor = Color.White;
        this.btnMovePlayer.Cursor = Cursors.Hand;
        this.btnMovePlayer.Enabled = false;

        // btnDeletePlayer
        this.btnDeletePlayer.Text = "🗑️ Удалить";
        this.btnDeletePlayer.Location = new Point(475, 0);
        this.btnDeletePlayer.Size = new Size(145, 40);
        this.btnDeletePlayer.Click += BtnDeletePlayer_Click;
        this.btnDeletePlayer.FlatStyle = FlatStyle.Flat;
        this.btnDeletePlayer.BackColor = Color.FromArgb(244, 67, 54);
        this.btnDeletePlayer.ForeColor = Color.White;
        this.btnDeletePlayer.Cursor = Cursors.Hand;
        this.btnDeletePlayer.Enabled = false;

        this.panelPlayerActions.Controls.AddRange(new Control[] {
            btnAddPlayer, btnEditPlayer, btnMovePlayer, btnDeletePlayer
        });
        this.grpPlayers.Controls.AddRange(new Control[] {
            lblTeam, lblTeamInfo, lvPlayers, panelPlayerActions
        });

        // Form
        this.ClientSize = new Size(970, 525);
        this.Controls.AddRange(new Control[] { grpTeams, grpPlayers });
        this.Text = "Управление командами и игроками";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(250, 250, 250);
        this.Font = new Font("Segoe UI", 9F);
    }

    private void LoadTeams()
    {
        lstTeams.Items.Clear();

        // Сортируем команды по рейтингу (по убыванию)
        var sortedTeams = DataService.Teams.OrderByDescending(t => t.VrsRating).ToList();

        int position = 1;
        foreach (var team in sortedTeams)
        {
            var rank = RatingHelpers.GetTeamRank(team.VrsRating);
            var displayText = $"#{position} {team.Name} [{team.VrsRating}]";
            lstTeams.Items.Add(displayText);
            position++;
        }

        lstTeams.Items.Add("[Без команды]");
    }

    private void LstTeams_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTeams.SelectedIndex < 0) return;

        string selectedText = lstTeams.SelectedItem.ToString();

        // Извлекаем название команды
        string teamName;
        Team? team = null;

        if (selectedText == "[Без команды]")
        {
            teamName = "[Без команды]";
        }
        else
        {
            // Формат: "#1 Natus Vincere [1850]"
            // Убираем позицию (#1) и рейтинг ([1850])
            var withoutPosition = selectedText.Substring(selectedText.IndexOf(' ') + 1); // Убираем "#1 "
            var bracketIndex = withoutPosition.LastIndexOf('[');
            teamName = bracketIndex > 0 ? withoutPosition.Substring(0, bracketIndex).Trim() : withoutPosition.Trim();

            team = DataService.Teams.FirstOrDefault(t => t.Name == teamName);
        }

        if (team == null)
        {
            lblTeam.Text = "Игроки без команды";
            var freeAgents = DataService.Players.Count(p => p.Team == null);
            lblTeamInfo.Text = $"Игроков: {freeAgents}";
        }
        else
        {
            lblTeam.Text = $"Команда: {team.Name}";

            // Формируем детальную информацию о команде
            var rank = RatingHelpers.GetTeamRank(team.VrsRating);
            var position = RatingHelpers.GetTeamPosition(team);
            var formStr = team.RecentForm.Count > 0
                ? string.Join("-", team.RecentForm)
                : "Нет матчей";

            lblTeamInfo.Text = $"#{position} • {rank} • VRS: {team.VrsRating} • " +
                             $"Игроков: {team.Players.Count}/5 • " +
                             $"Матчей: {team.MatchesPlayed} • " +
                             $"W/L/D: {team.Wins}/{team.Losses}/{team.Draws} • " +
                             $"WR: {team.WinRate:F1}% • " +
                             $"Форма: {formStr}";

            // Применяем цвет к lblTeam в зависимости от рейтинга
            lblTeam.ForeColor = RatingHelpers.GetRatingColor(team.VrsRating);
        }

        var players = team == null
            ? DataService.Players.Where(p => p.Team == null).ToList()
            : team.Players;

        lvPlayers.Items.Clear();
        imgCountries.Images.Clear();

        foreach (var player in players)
        {
            var flag = player.GetCountryImage();
            imgCountries.Images.Add(player.Nickname, flag ?? new Bitmap(24, 16));

            var item = new ListViewItem(player.Nickname) { ImageKey = player.Nickname };
            item.SubItems.Add(player.Country);
            item.SubItems.Add(team == null ? "Свободный агент" : "В команде");
            lvPlayers.Items.Add(item);
        }
    }

    private void LvPlayers_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool hasSelection = lvPlayers.SelectedItems.Count > 0;
        btnEditPlayer.Enabled = hasSelection;
        btnMovePlayer.Enabled = hasSelection;
        btnDeletePlayer.Enabled = hasSelection;
    }

    private void BtnAddTeam_Click(object? sender, EventArgs e)
    {
        var dialog = new AddTeamDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var team = new Team { Name = dialog.TeamName };
            DataService.Teams.Add(team);
            DataService.Save();
            LoadTeams();

            // Выбираем новую команду в списке
            var newTeamDisplay = lstTeams.Items.Cast<string>()
                .FirstOrDefault(item => item.Contains(dialog.TeamName));
            if (newTeamDisplay != null)
                lstTeams.SelectedItem = newTeamDisplay;
        }
    }

    private void BtnDeleteTeam_Click(object? sender, EventArgs e)
    {
        if (lstTeams.SelectedIndex < 0) return;

        string selectedText = lstTeams.SelectedItem.ToString();
        if (selectedText == "[Без команды]") return;

        // Извлекаем название команды
        string teamName;
        try
        {
            var withoutPosition = selectedText.Substring(selectedText.IndexOf(' ') + 1);
            var bracketIndex = withoutPosition.LastIndexOf('[');
            teamName = bracketIndex > 0 ? withoutPosition.Substring(0, bracketIndex).Trim() : withoutPosition.Trim();
        }
        catch
        {
            MessageBox.Show("Ошибка при определении команды", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var team = DataService.Teams.FirstOrDefault(t => t.Name == teamName);
        if (team == null)
        {
            MessageBox.Show($"Команда '{teamName}' не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить команду '{team.Name}'?\nИгроки команды станут свободными агентами.",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            // Сначала удаляем игроков из команды
            foreach (var player in team.Players.ToList())
            {
                team.RemovePlayer(player);
            }

            // Затем удаляем саму команду
            DataService.Teams.Remove(team);
            DataService.Save();
            LoadTeams();

            MessageBox.Show($"Команда '{team.Name}' успешно удалена!", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnAddPlayer_Click(object? sender, EventArgs e)
    {
        Team? currentTeam = null;
        if (lstTeams.SelectedIndex >= 0)
        {
            string selectedText = lstTeams.SelectedItem.ToString();
            if (selectedText != "[Без команды]")
            {
                var withoutPosition = selectedText.Substring(selectedText.IndexOf(' ') + 1);
                var bracketIndex = withoutPosition.LastIndexOf('[');
                var teamName = bracketIndex > 0 ? withoutPosition.Substring(0, bracketIndex).Trim() : withoutPosition.Trim();
                currentTeam = DataService.Teams.FirstOrDefault(t => t.Name == teamName);
            }
        }

        var dialog = new AddPlayerDialog(currentTeam);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            if (DataService.Players.Any(p => p.Nickname.Equals(dialog.PlayerNickname, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Игрок с таким ником уже существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var player = new Player
            {
                Nickname = dialog.PlayerNickname,
                Country = dialog.PlayerCountry
            };

            DataService.Players.Add(player);

            if (dialog.SelectedTeam != null)
            {
                if (dialog.SelectedTeam.Players.Count >= 5)
                {
                    MessageBox.Show("В команде уже 5 игроков!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    dialog.SelectedTeam.AddPlayer(player);
                }
            }

            DataService.Save();
            LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
        }
    }

    private void BtnEditPlayer_Click(object? sender, EventArgs e)
    {
        if (lvPlayers.SelectedItems.Count == 0) return;

        var playerName = lvPlayers.SelectedItems[0].Text;
        var player = DataService.Players.FirstOrDefault(p => p.Nickname == playerName);
        if (player == null) return;

        var dialog = new EditPlayerDialog(player);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            player.Country = dialog.PlayerCountry;
            DataService.Save();
            LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
        }
    }

    private void BtnMovePlayer_Click(object? sender, EventArgs e)
    {
        if (lvPlayers.SelectedItems.Count == 0) return;

        var playerName = lvPlayers.SelectedItems[0].Text;
        var player = DataService.Players.FirstOrDefault(p => p.Nickname == playerName);
        if (player == null) return;

        var dialog = new MovePlayerDialog(player);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            player.Team?.RemovePlayer(player);

            if (dialog.SelectedTeam != null)
            {
                var canAddResult = ValidationService.CanAddPlayerToTeam(dialog.SelectedTeam, player);
                if (!canAddResult.IsValid)
                {
                    MessageBox.Show(canAddResult.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dialog.SelectedTeam.AddPlayer(player);
            }

            DataService.Save();
            LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
        }
    }

    private void BtnDeletePlayer_Click(object? sender, EventArgs e)
    {
        if (lvPlayers.SelectedItems.Count == 0) return;

        var playerName = lvPlayers.SelectedItems[0].Text;
        var player = DataService.Players.FirstOrDefault(p => p.Nickname == playerName);
        if (player == null) return;

        var result = MessageBox.Show(
            $"Удалить игрока '{player.Nickname}'?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            player.Team?.RemovePlayer(player);
            DataService.Players.Remove(player);
            DataService.Save();
            LstTeams_SelectedIndexChanged(null!, EventArgs.Empty);
        }
    }
}
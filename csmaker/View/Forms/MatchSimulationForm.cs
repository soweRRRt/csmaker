using csmaker.Models;
using csmaker.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csmaker.View.Forms;

public partial class MatchSimulationForm : Form
{
    private ComboBox cmbTeam1;
    private ComboBox cmbTeam2;
    private ComboBox cmbFormat;
    private Button btnStartMatch;
    private Button btnClose;

    // Panels
    private Panel panelSetup;
    private Panel panelMatch;
    private Panel panelTeam1Info;
    private Panel panelTeam2Info;
    private Panel panelMatchInfo;

    // Labels для счета
    private Label lblTeam1Name;
    private Label lblTeam2Name;
    private Label lblTeam1Score;
    private Label lblTeam2Score;
    private Label lblMapScore;
    private Label lblCurrentMap;
    private Label lblMatchStatus;

    // Прогресс-бар раундов
    private ProgressBar pbRounds;

    // ListView для раундов
    private ListView lvRounds;

    // Данные
    private Team selectedTeam1;
    private Team selectedTeam2;
    private Match currentMatch;
    private Timer animationTimer;
    private int currentRound = 0;

    public MatchSimulationForm()
    {
        InitializeComponent();
        LoadTeams();
    }

    private void InitializeComponent()
    {
        this.Text = "Симуляция матча CS2";
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(20, 20, 30);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        InitializeSetupPanel();
        InitializeMatchPanel();

        // Таймер для анимации
        animationTimer = new Timer();
        animationTimer.Interval = 150; // Скорость анимации раундов
        animationTimer.Tick += AnimationTimer_Tick;

        this.Controls.Add(panelSetup);
        this.Controls.Add(panelMatch);

        panelMatch.Visible = false;
    }

    private void InitializeSetupPanel()
    {
        panelSetup = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(1100, 700),
            BackColor = Color.FromArgb(20, 20, 30)
        };

        // Заголовок
        var lblTitle = new Label
        {
            Text = "⚔️ СОЗДАНИЕ МАТЧА",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(0, 50),
            Size = new Size(1100, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Команда 1
        var lblTeam1 = new Label
        {
            Text = "КОМАНДА 1:",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(200, 180),
            AutoSize = true
        };

        cmbTeam1 = new ComboBox
        {
            Location = new Point(200, 220),
            Size = new Size(300, 35),
            Font = new Font("Segoe UI", 12),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(40, 40, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        // Команда 2
        var lblTeam2 = new Label
        {
            Text = "КОМАНДА 2:",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(600, 180),
            AutoSize = true
        };

        cmbTeam2 = new ComboBox
        {
            Location = new Point(600, 220),
            Size = new Size(300, 35),
            Font = new Font("Segoe UI", 12),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(40, 40, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        // Формат
        var lblFormat = new Label
        {
            Text = "ФОРМАТ МАТЧА:",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(400, 300),
            AutoSize = true
        };

        cmbFormat = new ComboBox
        {
            Location = new Point(400, 340),
            Size = new Size(300, 35),
            Font = new Font("Segoe UI", 12),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(40, 40, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        cmbFormat.Items.AddRange(new object[] { "Best of 1", "Best of 3", "Best of 5" });
        cmbFormat.SelectedIndex = 1;

        // Кнопка старта
        btnStartMatch = new Button
        {
            Text = "▶ НАЧАТЬ МАТЧ",
            Location = new Point(400, 450),
            Size = new Size(300, 60),
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStartMatch.FlatAppearance.BorderSize = 0;
        btnStartMatch.Click += BtnStartMatch_Click;

        panelSetup.Controls.AddRange(new Control[]
        {
            lblTitle, lblTeam1, cmbTeam1, lblTeam2, cmbTeam2,
            lblFormat, cmbFormat, btnStartMatch
        });
    }

    private void InitializeMatchPanel()
    {
        panelMatch = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(1100, 700),
            BackColor = Color.FromArgb(20, 20, 30)
        };

        // Панель информации о команде 1
        panelTeam1Info = CreateTeamPanel(50, true);

        // VS разделитель
        var lblVS = new Label
        {
            Text = "VS",
            Font = new Font("Segoe UI", 36, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(510, 50),
            Size = new Size(80, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Панель информации о команде 2
        panelTeam2Info = CreateTeamPanel(700, false);

        // Счет по картам
        lblMapScore = new Label
        {
            Text = "0 : 0",
            Font = new Font("Consolas", 48, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(420, 120),
            Size = new Size(260, 70),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Текущая карта
        lblCurrentMap = new Label
        {
            Text = "• MIRAGE •",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            Location = new Point(0, 210),
            Size = new Size(1100, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Счет по раундам
        var panelRoundScore = new Panel
        {
            Location = new Point(350, 260),
            Size = new Size(400, 80),
            BackColor = Color.FromArgb(30, 30, 40)
        };

        lblTeam1Score = new Label
        {
            Text = "0",
            Font = new Font("Consolas", 42, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            Location = new Point(50, 10),
            Size = new Size(120, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblColon = new Label
        {
            Text = ":",
            Font = new Font("Consolas", 42, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(180, 10),
            Size = new Size(40, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblTeam2Score = new Label
        {
            Text = "0",
            Font = new Font("Consolas", 42, FontStyle.Bold),
            ForeColor = Color.FromArgb(244, 67, 54),
            Location = new Point(230, 10),
            Size = new Size(120, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        panelRoundScore.Controls.AddRange(new Control[] { lblTeam1Score, lblColon, lblTeam2Score });

        // Прогресс-бар
        pbRounds = new ProgressBar
        {
            Location = new Point(250, 360),
            Size = new Size(600, 30),
            Maximum = 24,
            Value = 0,
            Style = ProgressBarStyle.Continuous
        };

        // Статус матча
        lblMatchStatus = new Label
        {
            Text = "Раунд 1/24 • Первый тайм",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(0, 400),
            Size = new Size(1100, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // ListView раундов
        lvRounds = new ListView
        {
            Location = new Point(50, 450),
            Size = new Size(1000, 150),
            View = System.Windows.Forms.View.Details,
            BackColor = Color.FromArgb(30, 30, 40),
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            FullRowSelect = true,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };
        lvRounds.Columns.Add("Раунд", 80);
        lvRounds.Columns.Add("Победитель", 400);
        lvRounds.Columns.Add("Счет", 200);
        lvRounds.Columns.Add("Тайм", 150);

        // Кнопка закрытия
        btnClose = new Button
        {
            Text = "✕ Закрыть",
            Location = new Point(450, 620),
            Size = new Size(200, 45),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Visible = false
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();

        panelMatch.Controls.AddRange(new Control[]
        {
            panelTeam1Info, lblVS, panelTeam2Info, lblMapScore,
            lblCurrentMap, panelRoundScore, pbRounds, lblMatchStatus,
            lvRounds, btnClose
        });
    }

    private Panel CreateTeamPanel(int x, bool isTeam1)
    {
        var panel = new Panel
        {
            Location = new Point(x, 40),
            Size = new Size(300, 150),
            BackColor = Color.FromArgb(30, 30, 40)
        };

        var lblName = new Label
        {
            Name = isTeam1 ? "lblTeam1Name" : "lblTeam2Name",
            Text = "Team Name",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = isTeam1 ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54),
            Location = new Point(10, 10),
            Size = new Size(280, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblRating = new Label
        {
            Text = "VRS: 1000",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(10, 50),
            Size = new Size(280, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblWinProb = new Label
        {
            Text = "50% шанс победы",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(10, 80),
            Size = new Size(280, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblForm = new Label
        {
            Text = "Форма: W-L-W-W-L",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(10, 110),
            Size = new Size(280, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        if (isTeam1)
        {
            lblTeam1Name = lblName;
        }
        else
        {
            lblTeam2Name = lblName;
        }

        panel.Controls.AddRange(new Control[] { lblName, lblRating, lblWinProb, lblForm });
        return panel;
    }

    private void LoadTeams()
    {
        cmbTeam1.Items.Clear();
        cmbTeam2.Items.Clear();

        foreach (var team in DataService.Teams.OrderByDescending(t => t.VrsRating))
        {
            var displayText = $"{team.Name} [VRS: {team.VrsRating}]";
            cmbTeam1.Items.Add(displayText);
            cmbTeam2.Items.Add(displayText);
        }

        if (cmbTeam1.Items.Count > 0) cmbTeam1.SelectedIndex = 0;
        if (cmbTeam2.Items.Count > 1) cmbTeam2.SelectedIndex = 1;
    }

    private async void BtnStartMatch_Click(object sender, EventArgs e)
    {
        if (cmbTeam1.SelectedIndex < 0 || cmbTeam2.SelectedIndex < 0)
        {
            MessageBox.Show("Выберите обе команды!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Получаем команды
        var team1Text = cmbTeam1.SelectedItem.ToString();
        var team2Text = cmbTeam2.SelectedItem.ToString();

        var team1Name = team1Text.Substring(0, team1Text.IndexOf(" [VRS:"));
        var team2Name = team2Text.Substring(0, team2Text.IndexOf(" [VRS:"));

        selectedTeam1 = DataService.Teams.FirstOrDefault(t => t.Name == team1Name);
        selectedTeam2 = DataService.Teams.FirstOrDefault(t => t.Name == team2Name);

        if (selectedTeam1 == selectedTeam2)
        {
            MessageBox.Show("Выберите разные команды!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Определяем формат
        BestOfFormat format = cmbFormat.SelectedIndex switch
        {
            0 => BestOfFormat.BO1,
            1 => BestOfFormat.BO3,
            2 => BestOfFormat.BO5,
            _ => BestOfFormat.BO3
        };

        // Переключаемся на панель матча
        panelSetup.Visible = false;
        panelMatch.Visible = true;

        // Обновляем информацию о командах
        UpdateTeamInfo();

        // Симулируем матч
        await SimulateMatchAsync(format);
    }

    private void UpdateTeamInfo()
    {
        lblTeam1Name.Text = selectedTeam1.Name.ToUpper();
        lblTeam2Name.Text = selectedTeam2.Name.ToUpper();

        var team1Panel = (Panel)lblTeam1Name.Parent;
        var team2Panel = (Panel)lblTeam2Name.Parent;

        ((Label)team1Panel.Controls[1]).Text = $"VRS: {selectedTeam1.VrsRating}";
        ((Label)team2Panel.Controls[1]).Text = $"VRS: {selectedTeam2.VrsRating}";

        double winProb = VrsRatingService.GetWinProbability(selectedTeam1, selectedTeam2);
        ((Label)team1Panel.Controls[2]).Text = $"{winProb:F1}% шанс победы";
        ((Label)team2Panel.Controls[2]).Text = $"{100 - winProb:F1}% шанс победы";

        string form1 = selectedTeam1.RecentForm.Count > 0 ? string.Join("-", selectedTeam1.RecentForm) : "Нет данных";
        string form2 = selectedTeam2.RecentForm.Count > 0 ? string.Join("-", selectedTeam2.RecentForm) : "Нет данных";

        ((Label)team1Panel.Controls[3]).Text = $"Форма: {form1}";
        ((Label)team2Panel.Controls[3]).Text = $"Форма: {form2}";
    }

    private async Task SimulateMatchAsync(BestOfFormat format)
    {
        currentMatch = MatchSimulator.SimulateMatch(selectedTeam1, selectedTeam2, format);

        int mapsToWin = ((int)format / 2) + 1;
        int team1MapWins = 0;
        int team2MapWins = 0;

        foreach (var mapResult in currentMatch.MapResults)
        {
            // Показываем карту
            lblCurrentMap.Text = $"• {mapResult.Map.DisplayName.ToUpper()} •";
            lblTeam1Score.Text = "0";
            lblTeam2Score.Text = "0";
            pbRounds.Value = 0;
            lvRounds.Items.Clear();

            await Task.Delay(1000);

            // Анимируем раунды
            currentRound = 0;
            int team1Score = 0;
            int team2Score = 0;

            for (int round = 1; round <= mapResult.Team1Score + mapResult.Team2Score; round++)
            {
                currentRound = round;

                // Определяем победителя раунда
                bool team1WonRound = team1Score < mapResult.Team1Score;

                if (team1WonRound)
                    team1Score++;
                else
                    team2Score++;

                // Обновляем UI
                lblTeam1Score.Text = team1Score.ToString();
                lblTeam2Score.Text = team2Score.ToString();
                pbRounds.Value = round;

                string half = round <= 12 ? "Первый тайм" : (round <= 24 ? "Второй тайм" : $"Овертайм {(round - 24) / 6 + 1}");
                lblMatchStatus.Text = $"Раунд {round}/{mapResult.Team1Score + mapResult.Team2Score} • {half}";

                var item = new ListViewItem(round.ToString());
                item.SubItems.Add(team1WonRound ? selectedTeam1.Name : selectedTeam2.Name);
                item.SubItems.Add($"{team1Score}:{team2Score}");
                item.SubItems.Add(half);
                item.ForeColor = team1WonRound ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54);
                lvRounds.Items.Add(item);
                lvRounds.EnsureVisible(lvRounds.Items.Count - 1);

                await Task.Delay(150);
            }

            // Обновляем счет по картам
            if (mapResult.Winner == selectedTeam1)
                team1MapWins++;
            else
                team2MapWins++;

            lblMapScore.Text = $"{team1MapWins} : {team2MapWins}";

            await Task.Delay(2000);

            // Проверяем, закончился ли матч
            if (team1MapWins >= mapsToWin || team2MapWins >= mapsToWin)
                break;
        }

        // Показываем результат
        ShowMatchResult();
    }

    private void ShowMatchResult()
    {
        lblMatchStatus.Text = $"🏆 МАТЧ ЗАВЕРШЕН! Победитель: {currentMatch.Winner.Name.ToUpper()}";
        lblMatchStatus.ForeColor = Color.FromArgb(255, 193, 7);
        lblMatchStatus.Font = new Font("Segoe UI", 14, FontStyle.Bold);

        btnClose.Visible = true;

        // Применяем изменения рейтинга
        int winnerScore = 0;
        int loserScore = 0;

        foreach (var mapResult in currentMatch.MapResults)
        {
            if (mapResult.Winner == currentMatch.Winner)
            {
                winnerScore += mapResult.Winner == selectedTeam1 ? mapResult.Team1Score : mapResult.Team2Score;
                loserScore += mapResult.Loser == selectedTeam1 ? mapResult.Team1Score : mapResult.Team2Score;
            }
        }

        VrsRatingService.ApplyMatchResult(currentMatch.Winner, currentMatch.Loser, winnerScore, loserScore);
        DataService.Save();

        MessageBox.Show(
            MatchService.GetMatchSummary(currentMatch),
            "Результат матча",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        // Для будущих анимаций
    }
}
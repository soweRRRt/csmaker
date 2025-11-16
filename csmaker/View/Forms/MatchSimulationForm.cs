using csmaker.Models;
using csmaker.Services;
using System;
using System.Collections.Generic;
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
    private Button btnBackToSetup;

    // Panels
    private Panel panelSetup;
    private Panel panelMatch;
    private Panel panelTeam1Info;
    private Panel panelTeam2Info;
    private Panel panelMatchInfo;
    private Panel panelStats;

    // Labels для счета
    private Label lblTeam1Name;
    private Label lblTeam2Name;
    private Label lblTeam1Score;
    private Label lblTeam2Score;
    private Label lblMapScore;
    private Label lblCurrentMap;
    private Label lblMatchStatus;

    // Расширенная статистика
    private Label lblTeam1Rating;
    private Label lblTeam2Rating;
    private Label lblTeam1WinProb;
    private Label lblTeam2WinProb;
    private Label lblTeam1Form;
    private Label lblTeam2Form;
    private Label lblMatchFormat;
    private Label lblMapNumber;
    private Label lblRatingChange;
    private Label lblExpectedScore;
    private Label lblMatchupQuality;

    // Статистика карты
    private Label lblMapCTAdvantage;
    private Label lblFirstHalfScore;
    private Label lblSecondHalfScore;
    private Label lblOvertimeInfo;

    // Статистика матча
    private Label lblTotalRounds;
    private Label lblTeam1TotalRounds;
    private Label lblTeam2TotalRounds;
    private Label lblTeam1CTRounds;
    private Label lblTeam2CTRounds;
    private Label lblTeam1TRounds;
    private Label lblTeam2TRounds;

    // Прогресс-бар раундов
    private ProgressBar pbRounds;
    private ProgressBar pbTeam1WinChance;
    private ProgressBar pbTeam2WinChance;

    // ListView для раундов
    private ListView lvRounds;
    private ListView lvMapResults;

    // Данные
    private Team selectedTeam1;
    private Team selectedTeam2;
    private Match currentMatch;
    private Timer animationTimer;
    private int currentRound = 0;
    private int team1TotalRounds = 0;
    private int team2TotalRounds = 0;
    private int team1CTRounds = 0;
    private int team2CTRounds = 0;
    private int team1TRounds = 0;
    private int team2TRounds = 0;

    public MatchSimulationForm()
    {
        InitializeComponent();
        LoadTeams();
    }

    private void InitializeComponent()
    {
        this.Text = "Симуляция матча CS2";
        this.Size = new Size(1400, 900);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(15, 15, 25);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        InitializeSetupPanel();
        InitializeMatchPanel();

        // Таймер для анимации
        animationTimer = new Timer();
        animationTimer.Interval = 120; // Скорость анимации раундов
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
            Size = new Size(1400, 900),
            BackColor = Color.FromArgb(15, 15, 25)
        };

        // Заголовок с градиентом
        var lblTitle = new Label
        {
            Text = "⚔️ СИМУЛЯЦИЯ МАТЧА CS2",
            Font = new Font("Segoe UI", 36, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            Location = new Point(0, 60),
            Size = new Size(1400, 70),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSubtitle = new Label
        {
            Text = "Выберите команды и формат для начала симуляции",
            Font = new Font("Segoe UI", 14),
            ForeColor = Color.FromArgb(180, 180, 180),
            Location = new Point(0, 140),
            Size = new Size(1400, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Панель команды 1
        var pnlTeam1Setup = CreateTeamSetupPanel(200, true);

        // VS разделитель
        var lblVS = new Label
        {
            Text = "VS",
            Font = new Font("Segoe UI", 48, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(640, 320),
            Size = new Size(120, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Панель команды 2
        var pnlTeam2Setup = CreateTeamSetupPanel(880, false);

        // Формат
        var lblFormat = new Label
        {
            Text = "📋 ФОРМАТ МАТЧА:",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(500, 580),
            Size = new Size(400, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        cmbFormat = new ComboBox
        {
            Location = new Point(550, 625),
            Size = new Size(300, 40),
            Font = new Font("Segoe UI", 13),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(40, 40, 55),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        cmbFormat.Items.AddRange(new object[] { "Best of 1 (1 карта)", "Best of 3 (до 2 побед)", "Best of 5 (до 3 побед)" });
        cmbFormat.SelectedIndex = 1;

        // Кнопка старта с эффектом
        btnStartMatch = new Button
        {
            Text = "▶ НАЧАТЬ СИМУЛЯЦИЮ",
            Location = new Point(500, 720),
            Size = new Size(400, 70),
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnStartMatch.FlatAppearance.BorderSize = 0;
        btnStartMatch.FlatAppearance.BorderColor = Color.FromArgb(46, 125, 50);
        btnStartMatch.Click += BtnStartMatch_Click;

        // Эффекты наведения
        btnStartMatch.MouseEnter += (s, e) => btnStartMatch.BackColor = Color.FromArgb(102, 187, 106);
        btnStartMatch.MouseLeave += (s, e) => btnStartMatch.BackColor = Color.FromArgb(76, 175, 80);

        panelSetup.Controls.AddRange(new Control[]
        {
            lblTitle, lblSubtitle, pnlTeam1Setup, lblVS, pnlTeam2Setup,
            lblFormat, cmbFormat, btnStartMatch
        });
    }

    private Panel CreateTeamSetupPanel(int x, bool isTeam1)
    {
        var panel = new Panel
        {
            Location = new Point(x, 240),
            Size = new Size(380, 300),
            BackColor = Color.FromArgb(25, 25, 40)
        };

        // Рамка
        panel.Paint += (s, e) =>
        {
            using (var pen = new Pen(isTeam1 ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54), 3))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        };

        var lblHeader = new Label
        {
            Text = isTeam1 ? "🔵 КОМАНДА 1" : "🔴 КОМАНДА 2",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 20),
            Size = new Size(340, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var combo = new ComboBox
        {
            Location = new Point(40, 80),
            Size = new Size(300, 40),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(40, 40, 55),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        if (isTeam1)
            cmbTeam1 = combo;
        else
            cmbTeam2 = combo;

        var lblInfo = new Label
        {
            Text = "Выберите команду из списка\nчтобы увидеть статистику",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(20, 140),
            Size = new Size(340, 140),
            TextAlign = ContentAlignment.TopCenter
        };

        // Обработчик выбора команды для отображения статистики
        combo.SelectedIndexChanged += (s, e) =>
        {
            if (combo.SelectedIndex >= 0)
            {
                var teamText = combo.SelectedItem.ToString();
                var teamName = teamText.Substring(0, teamText.IndexOf(" [VRS:"));
                var team = DataService.Teams.FirstOrDefault(t => t.Name == teamName);

                if (team != null)
                {
                    var rank = RatingHelpers.GetTeamRank(team.VrsRating);
                    var form = team.RecentForm.Count > 0 ? string.Join("-", team.RecentForm) : "Нет данных";
                    lblInfo.Text = $"VRS Рейтинг: {team.VrsRating}\n" +
                                  $"Ранг: {rank}\n" +
                                  $"Матчей: {team.MatchesPlayed}\n" +
                                  $"Побед/Поражений: {team.Wins}/{team.Losses}\n" +
                                  $"Винрейт: {team.WinRate:F1}%\n" +
                                  $"Форма: {form}";
                }
            }
        };

        panel.Controls.AddRange(new Control[] { lblHeader, combo, lblInfo });
        return panel;
    }

    private void InitializeMatchPanel()
    {
        panelMatch = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(1400, 900),
            BackColor = Color.FromArgb(15, 15, 25)
        };

        // Левая панель - Информация о командах
        var leftPanel = new Panel
        {
            Location = new Point(20, 20),
            Size = new Size(450, 850),
            BackColor = Color.FromArgb(20, 20, 35)
        };
        leftPanel.Paint += (s, e) => DrawPanelBorder(e.Graphics, leftPanel, Color.FromArgb(60, 60, 80));

        InitializeTeamStatsPanel(leftPanel);

        // Центральная панель - Счет и раунды
        var centerPanel = new Panel
        {
            Location = new Point(490, 20),
            Size = new Size(630, 850),
            BackColor = Color.FromArgb(20, 20, 35)
        };
        centerPanel.Paint += (s, e) => DrawPanelBorder(e.Graphics, centerPanel, Color.FromArgb(60, 60, 80));

        InitializeMatchCenterPanel(centerPanel);

        // Правая панель - Статистика матча
        var rightPanel = new Panel
        {
            Location = new Point(1140, 20),
            Size = new Size(240, 850),
            BackColor = Color.FromArgb(20, 20, 35)
        };
        rightPanel.Paint += (s, e) => DrawPanelBorder(e.Graphics, rightPanel, Color.FromArgb(60, 60, 80));

        InitializeMatchStatsPanel(rightPanel);

        panelMatch.Controls.AddRange(new Control[] { leftPanel, centerPanel, rightPanel });
    }

    private void InitializeTeamStatsPanel(Panel parent)
    {
        var lblHeader = new Label
        {
            Text = "📊 СТАТИСТИКА КОМАНД",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            Location = new Point(10, 10),
            Size = new Size(430, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Команда 1
        panelTeam1Info = CreateDetailedTeamPanel(10, 50, true);

        // VS разделитель
        var lblVS = new Label
        {
            Text = "⚔️",
            Font = new Font("Segoe UI", 36, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(190, 300),
            Size = new Size(70, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Команда 2
        panelTeam2Info = CreateDetailedTeamPanel(10, 370, false);

        // Head to Head
        var lblH2H = new Label
        {
            Text = "📈 СТАТИСТИКА ВСТРЕЧ",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 660),
            Size = new Size(430, 25)
        };

        lblMatchupQuality = new Label
        {
            Text = "Загрузка...",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(10, 690),
            Size = new Size(430, 60)
        };

        // Изменение рейтинга
        var lblRatingHeader = new Label
        {
            Text = "⭐ ПРОГНОЗ РЕЙТИНГА",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 760),
            Size = new Size(430, 25)
        };

        lblRatingChange = new Label
        {
            Text = "После матча будет обновлено",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(10, 790),
            Size = new Size(430, 50)
        };

        parent.Controls.AddRange(new Control[] {
            lblHeader, panelTeam1Info, lblVS, panelTeam2Info,
            lblH2H, lblMatchupQuality, lblRatingHeader, lblRatingChange
        });
    }

    private Panel CreateDetailedTeamPanel(int x, int y, bool isTeam1)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(430, 240),
            BackColor = Color.FromArgb(30, 30, 45)
        };

        panel.Paint += (s, e) =>
        {
            using (var pen = new Pen(isTeam1 ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        };

        var lblName = new Label
        {
            Name = isTeam1 ? "lblTeam1Name" : "lblTeam2Name",
            Text = "TEAM NAME",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = isTeam1 ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54),
            Location = new Point(10, 10),
            Size = new Size(410, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblRating = new Label
        {
            Name = isTeam1 ? "lblTeam1Rating" : "lblTeam2Rating",
            Text = "⭐ VRS: 1000",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            Location = new Point(10, 50),
            Size = new Size(410, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblWinProb = new Label
        {
            Name = isTeam1 ? "lblTeam1WinProb" : "lblTeam2WinProb",
            Text = "Шанс победы: 50.0%",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(10, 80),
            Size = new Size(410, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var pbWinChance = new ProgressBar
        {
            Name = isTeam1 ? "pbTeam1WinChance" : "pbTeam2WinChance",
            Location = new Point(40, 110),
            Size = new Size(350, 20),
            Maximum = 100,
            Value = 50,
            Style = ProgressBarStyle.Continuous,
            ForeColor = isTeam1 ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54)
        };

        var lblStats = new Label
        {
            Text = "📊 Статистика:",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 140),
            Size = new Size(400, 20)
        };

        var lblForm = new Label
        {
            Name = isTeam1 ? "lblTeam1Form" : "lblTeam2Form",
            Text = "Матчей: 0 | W/L: 0/0 | WR: 0%\nФорма: Нет данных",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 180),
            Location = new Point(15, 165),
            Size = new Size(400, 65)
        };

        if (isTeam1)
        {
            lblTeam1Name = lblName;
            lblTeam1Rating = lblRating;
            lblTeam1WinProb = lblWinProb;
            pbTeam1WinChance = pbWinChance;
            lblTeam1Form = lblForm;
        }
        else
        {
            lblTeam2Name = lblName;
            lblTeam2Rating = lblRating;
            lblTeam2WinProb = lblWinProb;
            pbTeam2WinChance = pbWinChance;
            lblTeam2Form = lblForm;
        }

        panel.Controls.AddRange(new Control[] { lblName, lblRating, lblWinProb, pbWinChance, lblStats, lblForm });
        return panel;
    }

    private void InitializeMatchCenterPanel(Panel parent)
    {
        // Счет по картам
        lblMapScore = new Label
        {
            Text = "0 : 0",
            Font = new Font("Consolas", 56, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(180, 20),
            Size = new Size(270, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblMatchFormat = new Label
        {
            Text = "BEST OF 3",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(0, 105),
            Size = new Size(630, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Текущая карта
        lblCurrentMap = new Label
        {
            Text = "• MIRAGE •",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            Location = new Point(0, 140),
            Size = new Size(630, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblMapNumber = new Label
        {
            Text = "Карта 1 из 3",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(0, 180),
            Size = new Size(630, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Счет по раундам
        var panelRoundScore = new Panel
        {
            Location = new Point(165, 220),
            Size = new Size(300, 100),
            BackColor = Color.FromArgb(30, 30, 45)
        };

        panelRoundScore.Paint += (s, e) => DrawPanelBorder(e.Graphics, panelRoundScore, Color.FromArgb(255, 193, 7));

        lblTeam1Score = new Label
        {
            Text = "0",
            Font = new Font("Consolas", 48, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            Location = new Point(30, 20),
            Size = new Size(100, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblColon = new Label
        {
            Text = ":",
            Font = new Font("Consolas", 48, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(130, 20),
            Size = new Size(40, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblTeam2Score = new Label
        {
            Text = "0",
            Font = new Font("Consolas", 48, FontStyle.Bold),
            ForeColor = Color.FromArgb(244, 67, 54),
            Location = new Point(170, 20),
            Size = new Size(100, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };

        panelRoundScore.Controls.AddRange(new Control[] { lblTeam1Score, lblColon, lblTeam2Score });

        // Прогресс-бар
        pbRounds = new ProgressBar
        {
            Location = new Point(15, 340),
            Size = new Size(600, 25),
            Maximum = 24,
            Value = 0,
            Style = ProgressBarStyle.Continuous
        };

        // Статус матча
        lblMatchStatus = new Label
        {
            Text = "Раунд 1/24 • Первый тайм",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(0, 375),
            Size = new Size(630, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Информация о таймах
        lblFirstHalfScore = new Label
        {
            Text = "1-й тайм: 0:0",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(15, 405),
            Size = new Size(200, 20)
        };

        lblSecondHalfScore = new Label
        {
            Text = "2-й тайм: 0:0",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(225, 405),
            Size = new Size(200, 20)
        };

        lblOvertimeInfo = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(255, 152, 0),
            Location = new Point(435, 405),
            Size = new Size(180, 20)
        };

        // ListView раундов
        lvRounds = new ListView
        {
            Location = new Point(15, 435),
            Size = new Size(600, 360),
            View = System.Windows.Forms.View.Details,
            BackColor = Color.FromArgb(25, 25, 40),
            ForeColor = Color.White,
            Font = new Font("Consolas", 9),
            FullRowSelect = true,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            BorderStyle = BorderStyle.FixedSingle
        };
        lvRounds.Columns.Add("№", 50);
        lvRounds.Columns.Add("Победитель", 280);
        lvRounds.Columns.Add("Счет", 120);
        lvRounds.Columns.Add("Период", 140);

        // Кнопки
        btnClose = new Button
        {
            Text = "✕ Закрыть",
            Location = new Point(330, 805),
            Size = new Size(140, 35),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Visible = false
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();

        btnBackToSetup = new Button
        {
            Text = "↶ Новый матч",
            Location = new Point(160, 805),
            Size = new Size(160, 35),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Visible = false
        };
        btnBackToSetup.FlatAppearance.BorderSize = 0;
        btnBackToSetup.Click += (s, e) =>
        {
            panelMatch.Visible = false;
            panelSetup.Visible = true;
            btnClose.Visible = false;
            btnBackToSetup.Visible = false;
        };

        parent.Controls.AddRange(new Control[]
        {
            lblMapScore, lblMatchFormat, lblCurrentMap, lblMapNumber, panelRoundScore,
            pbRounds, lblMatchStatus, lblFirstHalfScore, lblSecondHalfScore, lblOvertimeInfo,
            lvRounds, btnClose, btnBackToSetup
        });
    }

    private void InitializeMatchStatsPanel(Panel parent)
    {
        var lblHeader = new Label
        {
            Text = "📈 СТАТИСТИКА",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 215, 0),
            Location = new Point(10, 10),
            Size = new Size(220, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblTotalRoundsHeader = new Label
        {
            Text = "Всего раундов:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 50),
            Size = new Size(210, 20)
        };

        lblTotalRounds = new Label
        {
            Text = "0",
            Font = new Font("Consolas", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(15, 70),
            Size = new Size(210, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Команда 1 статистика
        var lblTeam1StatsHeader = new Label
        {
            Text = "🔵 Команда 1:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            Location = new Point(15, 110),
            Size = new Size(210, 20)
        };

        lblTeam1TotalRounds = new Label
        {
            Text = "Раундов: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 135),
            Size = new Size(210, 18)
        };

        lblTeam1CTRounds = new Label
        {
            Text = "CT раунды: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 155),
            Size = new Size(210, 18)
        };

        lblTeam1TRounds = new Label
        {
            Text = "T раунды: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 175),
            Size = new Size(210, 18)
        };

        // Команда 2 статистика
        var lblTeam2StatsHeader = new Label
        {
            Text = "🔴 Команда 2:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(244, 67, 54),
            Location = new Point(15, 210),
            Size = new Size(210, 20)
        };

        lblTeam2TotalRounds = new Label
        {
            Text = "Раундов: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 235),
            Size = new Size(210, 18)
        };

        lblTeam2CTRounds = new Label
        {
            Text = "CT раунды: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 255),
            Size = new Size(210, 18)
        };

        lblTeam2TRounds = new Label
        {
            Text = "T раунды: 0",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 275),
            Size = new Size(210, 18)
        };

        // Информация о карте
        var lblMapInfoHeader = new Label
        {
            Text = "🗺️ Карта:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 315),
            Size = new Size(210, 20)
        };

        lblMapCTAdvantage = new Label
        {
            Text = "CT преимущество: 50%",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(15, 340),
            Size = new Size(210, 18)
        };

        // Результаты по картам
        var lblMapsHeader = new Label
        {
            Text = "🏆 Карты:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 380),
            Size = new Size(210, 20)
        };

        lvMapResults = new ListView
        {
            Location = new Point(15, 405),
            Size = new Size(210, 430),
            View = System.Windows.Forms.View.Details,
            BackColor = Color.FromArgb(25, 25, 40),
            ForeColor = Color.White,
            Font = new Font("Consolas", 9),
            FullRowSelect = true,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            BorderStyle = BorderStyle.FixedSingle
        };
        lvMapResults.Columns.Add("Карта", 70);
        lvMapResults.Columns.Add("Счет", 130);

        parent.Controls.AddRange(new Control[]
        {
            lblHeader, lblTotalRoundsHeader, lblTotalRounds,
            lblTeam1StatsHeader, lblTeam1TotalRounds, lblTeam1CTRounds, lblTeam1TRounds,
            lblTeam2StatsHeader, lblTeam2TotalRounds, lblTeam2CTRounds, lblTeam2TRounds,
            lblMapInfoHeader, lblMapCTAdvantage, lblMapsHeader, lvMapResults
        });
    }

    private void DrawPanelBorder(Graphics g, Panel panel, Color color)
    {
        using (var pen = new Pen(color, 2))
        {
            g.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        }
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

        // Сброс статистики
        team1TotalRounds = 0;
        team2TotalRounds = 0;
        team1CTRounds = 0;
        team2CTRounds = 0;
        team1TRounds = 0;
        team2TRounds = 0;

        // Симулируем матч
        await SimulateMatchAsync(format);
    }

    private void UpdateTeamInfo()
    {
        lblTeam1Name.Text = selectedTeam1.Name.ToUpper();
        lblTeam2Name.Text = selectedTeam2.Name.ToUpper();

        lblTeam1Rating.Text = $"⭐ VRS: {selectedTeam1.VrsRating}";
        lblTeam2Rating.Text = $"⭐ VRS: {selectedTeam2.VrsRating}";

        double winProb = VrsRatingService.GetWinProbability(selectedTeam1, selectedTeam2);
        lblTeam1WinProb.Text = $"Шанс победы: {winProb:F1}%";
        lblTeam2WinProb.Text = $"Шанс победы: {100 - winProb:F1}%";

        pbTeam1WinChance.Value = Math.Min(100, Math.Max(0, (int)winProb));
        pbTeam2WinChance.Value = Math.Min(100, Math.Max(0, (int)(100 - winProb)));

        string form1 = selectedTeam1.RecentForm.Count > 0 ? string.Join("-", selectedTeam1.RecentForm) : "Нет данных";
        string form2 = selectedTeam2.RecentForm.Count > 0 ? string.Join("-", selectedTeam2.RecentForm) : "Нет данных";

        lblTeam1Form.Text = $"Матчей: {selectedTeam1.MatchesPlayed} | W/L: {selectedTeam1.Wins}/{selectedTeam1.Losses} | WR: {selectedTeam1.WinRate:F1}%\nФорма: {form1}";
        lblTeam2Form.Text = $"Матчей: {selectedTeam2.MatchesPlayed} | W/L: {selectedTeam2.Wins}/{selectedTeam2.Losses} | WR: {selectedTeam2.WinRate:F1}%\nФорма: {form2}";

        // Matchup качество
        var matchupDesc = VrsRatingService.GetMatchupDescription(selectedTeam1, selectedTeam2);
        var (h2h1, h2h2, totalH2H) = MatchService.GetHeadToHead(selectedTeam1, selectedTeam2);

        lblMatchupQuality.Text = $"Качество матчапа: {matchupDesc}\n" +
                                $"Предыдущие встречи: {h2h1}-{h2h2} (всего {totalH2H})\n" +
                                $"Разница рейтинга: {Math.Abs(selectedTeam1.VrsRating - selectedTeam2.VrsRating)} очков";
    }

    private async Task SimulateMatchAsync(BestOfFormat format)
    {
        currentMatch = MatchSimulator.SimulateMatch(selectedTeam1, selectedTeam2, format);

        lblMatchFormat.Text = $"BEST OF {(int)format}";

        int mapsToWin = ((int)format / 2) + 1;
        int team1MapWins = 0;
        int team2MapWins = 0;

        int mapIndex = 0;
        foreach (var mapResult in currentMatch.MapResults)
        {
            mapIndex++;

            // Показываем карту
            lblCurrentMap.Text = $"• {mapResult.Map.DisplayName.ToUpper()} •";
            lblMapNumber.Text = $"Карта {mapIndex} из {currentMatch.SelectedMaps.Count}";
            lblTeam1Score.Text = "0";
            lblTeam2Score.Text = "0";
            pbRounds.Value = 0;
            pbRounds.Maximum = mapResult.Team1Score + mapResult.Team2Score;
            lvRounds.Items.Clear();

            lblFirstHalfScore.Text = "1-й тайм: 0:0";
            lblSecondHalfScore.Text = "2-й тайм: 0:0";
            lblOvertimeInfo.Text = "";

            // Информация о карте
            var ctAdv = mapResult.Map.Name switch
            {
                "de_ancient" => 52,
                "de_anubis" => 50,
                "de_dust2" => 48,
                "de_inferno" => 51,
                "de_mirage" => 49,
                "de_nuke" => 55,
                "de_vertigo" => 53,
                _ => 50
            };
            lblMapCTAdvantage.Text = $"CT преимущество: {ctAdv}%";

            await Task.Delay(1500);

            // Анимируем раунды
            await AnimateMapAsync(mapResult);

            // Обновляем счет по картам
            if (mapResult.Winner == selectedTeam1)
                team1MapWins++;
            else
                team2MapWins++;

            lblMapScore.Text = $"{team1MapWins} : {team2MapWins}";

            // Добавляем результат карты
            var mapItem = new ListViewItem(mapResult.Map.DisplayName);
            mapItem.SubItems.Add($"{mapResult.Team1Score}:{mapResult.Team2Score}");
            mapItem.ForeColor = mapResult.Winner == selectedTeam1 ?
                Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54);
            lvMapResults.Items.Add(mapItem);

            await Task.Delay(2000);

            // Проверяем, закончился ли матч
            if (team1MapWins >= mapsToWin || team2MapWins >= mapsToWin)
                break;
        }

        // Показываем результат
        ShowMatchResult();
    }

    private async Task AnimateMapAsync(MapResult mapResult)
    {
        currentRound = 0;
        int team1Score = 0;
        int team2Score = 0;

        // Создаем реалистичную последовательность раундов
        var rounds = GenerateRoundSequence(mapResult.Team1Score, mapResult.Team2Score);

        int firstHalfTeam1 = 0, firstHalfTeam2 = 0;
        int secondHalfTeam1 = 0, secondHalfTeam2 = 0;

        for (int round = 1; round <= rounds.Count; round++)
        {
            currentRound = round;

            // Определяем победителя раунда
            bool team1WonRound = rounds[round - 1];

            if (team1WonRound)
            {
                team1Score++;
                team1TotalRounds++;

                // Учитываем сторону (Team1 на CT в первом тайме)
                if (round <= 12)
                {
                    team1CTRounds++;
                    firstHalfTeam1++;
                }
                else if (round <= 24)
                {
                    team1TRounds++;
                    secondHalfTeam1++;
                }
            }
            else
            {
                team2Score++;
                team2TotalRounds++;

                // Учитываем сторону (Team2 на T в первом тайме)
                if (round <= 12)
                {
                    team2TRounds++;
                    firstHalfTeam2++;
                }
                else if (round <= 24)
                {
                    team2CTRounds++;
                    secondHalfTeam2++;
                }
            }

            // Обновляем UI
            lblTeam1Score.Text = team1Score.ToString();
            lblTeam2Score.Text = team2Score.ToString();
            pbRounds.Value = Math.Min(round, pbRounds.Maximum);

            string half = round <= 12 ? "1-й тайм" : (round <= 24 ? "2-й тайм" : $"ОТ {(round - 24) / 6 + 1}");

            // Обновляем totalRounds динамически
            int estimatedTotal = rounds.Count;
            lblMatchStatus.Text = $"Раунд {round}/{estimatedTotal} • {half}";

            // Обновляем счет по таймам
            if (round <= 12)
            {
                lblFirstHalfScore.Text = $"1-й тайм: {firstHalfTeam1}:{firstHalfTeam2}";
            }
            else if (round <= 24)
            {
                lblSecondHalfScore.Text = $"2-й тайм: {secondHalfTeam1}:{secondHalfTeam2}";
            }

            if (mapResult.WentToOvertime && round > 24)
            {
                lblOvertimeInfo.Text = $"Овертайм x{mapResult.OvertimeCount}";
            }

            // Обновляем общую статистику
            lblTotalRounds.Text = (team1TotalRounds + team2TotalRounds).ToString();
            lblTeam1TotalRounds.Text = $"Раундов: {team1TotalRounds}";
            lblTeam1CTRounds.Text = $"CT раунды: {team1CTRounds}";
            lblTeam1TRounds.Text = $"T раунды: {team1TRounds}";
            lblTeam2TotalRounds.Text = $"Раундов: {team2TotalRounds}";
            lblTeam2CTRounds.Text = $"CT раунды: {team2CTRounds}";
            lblTeam2TRounds.Text = $"T раунды: {team2TRounds}";

            var item = new ListViewItem(round.ToString());
            item.SubItems.Add(team1WonRound ? selectedTeam1.Name : selectedTeam2.Name);
            item.SubItems.Add($"{team1Score}:{team2Score}");
            item.SubItems.Add(half);
            item.ForeColor = team1WonRound ? Color.FromArgb(33, 150, 243) : Color.FromArgb(244, 67, 54);
            lvRounds.Items.Add(item);
            lvRounds.EnsureVisible(lvRounds.Items.Count - 1);

            await Task.Delay(120);
        }
    }

    /// <summary>
    /// Генерирует реалистичную последовательность раундов, которая приведет к нужному счету
    /// </summary>
    private List<bool> GenerateRoundSequence(int finalTeam1Score, int finalTeam2Score)
    {
        var random = new Random();
        var sequence = new List<bool>();

        int team1Score = 0;
        int team2Score = 0;

        // Генерируем раунды пока не достигнем нужного счета
        while (true)
        {
            // Определяем, кто должен выиграть следующий раунд
            int team1Remaining = finalTeam1Score - team1Score;
            int team2Remaining = finalTeam2Score - team2Score;

            // Если одна из команд уже набрала свой счет, вторая добирает свой
            if (team1Remaining == 0)
            {
                sequence.Add(false); // Team2 выигрывает
                team2Score++;
            }
            else if (team2Remaining == 0)
            {
                sequence.Add(true); // Team1 выигрывает
                team1Score++;
            }
            else
            {
                // Обе команды еще не достигли своего счета
                // Случайно выбираем победителя с учетом оставшихся раундов
                double team1Chance = (double)team1Remaining / (team1Remaining + team2Remaining);

                if (random.NextDouble() < team1Chance)
                {
                    sequence.Add(true);
                    team1Score++;
                }
                else
                {
                    sequence.Add(false);
                    team2Score++;
                }
            }

            // Проверяем условие окончания
            // 1. В регулярном time: кто-то достиг 13 (и это не 12:12)
            int totalRounds = team1Score + team2Score;
            if (totalRounds <= 24)
            {
                if (team1Score >= 13 || team2Score >= 13)
                {
                    // Проверяем, не овертайм ли (12:12 -> продолжаем до 12:13 или 13:12)
                    if (team1Score == 12 && team2Score == 12)
                    {
                        // Это был бы овертайм, продолжаем
                        continue;
                    }
                    else
                    {
                        // Кто-то выиграл
                        break;
                    }
                }
            }
            else
            {
                // Овертайм: нужна разница в 2 раунда
                int diff = Math.Abs(team1Score - team2Score);
                if (diff >= 2)
                {
                    break;
                }
            }

            // Проверка на достижение финального счета (на всякий случай)
            if (team1Score == finalTeam1Score && team2Score == finalTeam2Score)
            {
                break;
            }
        }

        return sequence;
    }

    private void ShowMatchResult()
    {
        lblMatchStatus.Text = $"🏆 МАТЧ ЗАВЕРШЕН! Победитель: {currentMatch.Winner.Name.ToUpper()}";
        lblMatchStatus.ForeColor = Color.FromArgb(255, 215, 0);
        lblMatchStatus.Font = new Font("Segoe UI", 13, FontStyle.Bold);

        btnClose.Visible = true;
        btnBackToSetup.Visible = true;

        // Применяем изменения рейтинга
        int winnerScore = 0;
        int loserScore = 0;

        foreach (var mapResult in currentMatch.MapResults)
        {
            winnerScore += mapResult.Winner == selectedTeam1 ? mapResult.Team1Score : mapResult.Team2Score;
            loserScore += mapResult.Loser == selectedTeam1 ? mapResult.Team1Score : mapResult.Team2Score;
        }

        var ratingPreview = VrsRatingService.PreviewRatingChange(
            currentMatch.Winner, currentMatch.Loser, winnerScore, loserScore);

        lblRatingChange.Text = $"Победитель: {ratingPreview.WinnerOldRating} → {ratingPreview.WinnerNewRating} ({ratingPreview.WinnerChange:+0;-0})\n" +
                              $"Проигравший: {ratingPreview.LoserOldRating} → {ratingPreview.LoserNewRating} ({ratingPreview.LoserChange:+0;-0})";

        VrsRatingService.ApplyMatchResult(currentMatch.Winner, currentMatch.Loser, winnerScore, loserScore);
        DataService.Save();

        MessageBox.Show(
            MatchService.GetMatchSummary(currentMatch),
            "🏆 Результат матча",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        // Для будущих анимаций
    }
}
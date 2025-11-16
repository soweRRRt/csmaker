using System;
using System.Drawing;
using System.Windows.Forms;

namespace csmaker.View.Forms;

public partial class MainMenuForm : Form
{
    private Button btnTeams;
    private Button btnMatch;
    private Button btnHistory;
    private Button btnExit;
    private Label lblTitle;

    public MainMenuForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "CS Maker - Главное меню";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(20, 20, 30);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Заголовок
        lblTitle = new Label
        {
            Text = "🎮 CS MAKER",
            Font = new Font("Segoe UI", 48, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 193, 7),
            Location = new Point(0, 80),
            Size = new Size(800, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSubtitle = new Label
        {
            Text = "Менеджер команд Counter-Strike 2",
            Font = new Font("Segoe UI", 14),
            ForeColor = Color.FromArgb(200, 200, 200),
            Location = new Point(0, 170),
            Size = new Size(800, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Кнопки
        btnTeams = CreateMenuButton("👥 Управление командами", 250);
        btnTeams.Click += (s, e) =>
        {
            var teamsForm = new TeamsForm();
            teamsForm.ShowDialog();
        };

        btnMatch = CreateMenuButton("⚔️ Симуляция матча", 320);
        btnMatch.Click += (s, e) =>
        {
            var matchForm = new MatchSimulationForm();
            matchForm.ShowDialog();
        };

        btnHistory = CreateMenuButton("📊 История матчей", 390);
        btnHistory.Click += (s, e) =>
        {
            MessageBox.Show("Эта функция будет добавлена позже!", "Скоро",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        btnExit = CreateMenuButton("❌ Выход", 480);
        btnExit.BackColor = Color.FromArgb(244, 67, 54);
        btnExit.Click += (s, e) => Application.Exit();

        this.Controls.AddRange(new Control[]
        {
            lblTitle, lblSubtitle, btnTeams, btnMatch, btnHistory, btnExit
        });
    }

    private Button CreateMenuButton(string text, int y)
    {
        var btn = new Button
        {
            Text = text,
            Location = new Point(250, y),
            Size = new Size(300, 55),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;

        // Эффект наведения
        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(50, 170, 255);
        btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(33, 150, 243);

        return btn;
    }
}
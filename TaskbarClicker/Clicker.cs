using TaskbarClicker.Properties;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace TaskbarClicker
{
    internal class Clicker : ApplicationContext
    {
        private readonly Icon _buttonDefault = Resources._1, _buttonPressed = Resources._2;

        private readonly NotifyIcon _notifyIcon = new() { Visible = true };
        private readonly ToolStripMenuItem _scoreItem = new("");
        private readonly ToolStripMenuItem _exitButton = new("Exit", null, (object sender, EventArgs eventArgs) => Application.Exit());
        
        private ulong _score = User.Default.Score - 1;

        private Thread _buttonPressAnimationThread;

        public Clicker()
        {
            _scoreItem.Image = _buttonDefault.ToBitmap();
            _scoreItem.Click += OnScoreItemClick;

            ContextMenuStrip appIconMenu = new();
            appIconMenu.Items.AddRange([_scoreItem, _exitButton]);

            _notifyIcon.Icon = _buttonDefault;
            _notifyIcon.ContextMenuStrip = appIconMenu;
            _notifyIcon.MouseClick += OnNotifyIconClick;

            Application.ApplicationExit += OnExit;

            UpdateTexts();
        }

        private void OnNotifyIconClick(object sender, EventArgs eventArgs)
        {
            if (((MouseEventArgs)eventArgs).Button == MouseButtons.Left)
                OnScoreItemClick(sender, eventArgs);
        }

        private void OnScoreItemClick(object sender, EventArgs eventArgs)
        {
            UpdateTexts();
            StartButtonPressAnimation();
        }

        private void UpdateTexts() =>
            _scoreItem.Text = _notifyIcon.Text = (++_score).ToString();

        private void StartButtonPressAnimation()
        {
            if (!_notifyIcon.Icon.Equals(_buttonPressed))
                _notifyIcon.Icon = _buttonPressed;

            _buttonPressAnimationThread?.Abort();
            (_buttonPressAnimationThread = new(() => {
                Thread.Sleep(250);
                _notifyIcon.Icon = _buttonDefault;
            })).Start();
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            _notifyIcon.Visible = false;
            User.Default.Score = _score;
            User.Default.Save();
        }
    }
}

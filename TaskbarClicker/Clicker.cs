using TaskbarClicker.Properties;
using System;
using System.Windows.Forms;
using System.Threading;

namespace TaskbarClicker
{
    public class Clicker : ApplicationContext
    {
        private readonly NotifyIcon _appIcon = new() { Icon = Resources._1, Visible = true };
        private readonly ToolStripMenuItem _scoreItem = new("", Resources._1.ToBitmap());
        private ulong _score = User.Default.Score - 1;
        private Thread _latestThread;

        public Clicker()
        {
            ContextMenuStrip appIconMenu = new();
            appIconMenu.Items.AddRange(new ToolStripMenuItem[] { _scoreItem, new("Exit", null, OnExit) });
            _appIcon.ContextMenuStrip = appIconMenu;

            Application.ApplicationExit += OnExit;
            _scoreItem.Click += OnMouseClick;
            _appIcon.MouseClick += OnMouseClick;

            UpdateTexts();
        }

        private void OnMouseClick(object sender, EventArgs eventArgs)
        {
            _appIcon.Icon = Resources._2;
            
            try
            {
                if (((MouseEventArgs)eventArgs).Button == MouseButtons.Left) UpdateTexts();
            }
            catch (InvalidCastException) { UpdateTexts(); }

            _latestThread?.Abort(); 
            (_latestThread = new(ReturnButton)).Start();
        }

        private void UpdateTexts() => _scoreItem.Text = _appIcon.Text = (++_score).ToString();

        private void ReturnButton()
        {
            Thread.Sleep(250);
            _appIcon.Icon = Resources._1;
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            _appIcon.Visible = false;
            User.Default.Score = _score;
            User.Default.Save();
            Environment.Exit(0);
        }
    }
}

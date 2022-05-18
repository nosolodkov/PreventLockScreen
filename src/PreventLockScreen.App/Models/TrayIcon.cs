using System;
using System.Linq;
using System.Windows.Forms;

namespace PreventLockScreen.App.Models
{
    internal class TrayIcon : IDisposable
    {
        private const string MenuPreventSleepText = "Prevent sleep";
        private const string MenuAllowSleepText = "Allow sleep";

        private readonly NotifyIcon _notifyIcon;

        public EventHandler AllowSleepEventHandler { get; set; }
        public EventHandler PreventSleepEventHandler { get; set; }

        public TrayIcon()
        {
            _notifyIcon = new NotifyIcon();
        }

        internal void Initialize()
        {
            _notifyIcon.MouseDoubleClick += new MouseEventHandler(HandleMouseClick);
            _notifyIcon.Icon = Properties.Resources.locked;
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Power util allows to prevent idle-to-sleep";
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenu = BuildContextMenu();

            ShowBalloonTip(3000, "Power util", "Power util running", ToolTipIcon.Info);
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1 && e.Button == MouseButtons.Right)
            {
                Environment.Exit(1);
            }
        }

        internal void ShowBalloonTip(int timeout, string caption, string text, ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(timeout, caption, text, icon);
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

        private ContextMenu BuildContextMenu()
        {
            // Menu Item - Prevent
            var menuItemPrevent = new MenuItem()
            {
                Index = 0,
                Text = MenuPreventSleepText,
            };
            menuItemPrevent.Click += new EventHandler(PreventSleepEventHandler);
            menuItemPrevent.Checked = false;

            // Menu Item - Allow
            var menuItemAllow = new MenuItem()
            {
                Index = 1,
                Text = MenuAllowSleepText,
            };
            menuItemAllow.Click += new EventHandler(AllowSleepEventHandler);
            menuItemAllow.Checked = false;

            // Initialize contextMenu
            var retval = new ContextMenu();
            retval.MenuItems.AddRange(new MenuItem[]
            {
                menuItemPrevent,
                menuItemAllow
            });

            return retval;
        }

        internal void UpdateMenuItems(bool preventLockEnabled)
        {
            ShowBalloonTip(3000, "Power util", $"Prevent screen lock {(preventLockEnabled ? "enabled" : "disabled")}", ToolTipIcon.Info);

            foreach (MenuItem item in _notifyIcon.ContextMenu.MenuItems)
            {
                switch (item.Text)
                {
                    case MenuPreventSleepText:
                        item.Checked = preventLockEnabled;
                        break;
                    case MenuAllowSleepText:
                        item.Checked = !preventLockEnabled;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

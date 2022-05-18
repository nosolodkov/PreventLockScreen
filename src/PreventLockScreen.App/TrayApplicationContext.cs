using PreventLockScreen.App.Models;
using PreventLockScreen.App.Services;
using System.Windows.Forms;

namespace PreventLockScreen.App
{
    internal class TrayApplicationContext : ApplicationContext
    {
        private readonly TrayIcon _trayIcon;

        public TrayApplicationContext()
        {
            _trayIcon = new TrayIcon
            {
                AllowSleepEventHandler = (s, e) =>
                {
                    SleepPreventService.Instance.AllowSleep();
                    _trayIcon.UpdateMenuItems(SleepPreventService.Instance.PreventLockEnabled);
                },
                PreventSleepEventHandler = (s, e) =>
                {
                    SleepPreventService.Instance.PreventSleep();
                    _trayIcon.UpdateMenuItems(SleepPreventService.Instance.PreventLockEnabled);
                }
            };

            _trayIcon.Initialize();
            _trayIcon.UpdateMenuItems(SleepPreventService.Instance.PreventLockEnabled);
        }

        protected override void Dispose(bool disposing)
        {
            _trayIcon.Dispose();
            base.Dispose(disposing);
        }
    }
}

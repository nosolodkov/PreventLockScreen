using System;
using System.Runtime.InteropServices;

namespace PreventLockScreen.App.Services
{
    internal class SleepPreventService
    {
        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        #region Singleton pattern implementation

        private static readonly Lazy<SleepPreventService> lazy =
            new Lazy<SleepPreventService>(() => new SleepPreventService());

        public static SleepPreventService Instance { get { return lazy.Value; } }

        private SleepPreventService()
        {
            var prevState = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            SetThreadExecutionState(prevState);
            PreventLockEnabled = prevState == PreventSleepState;
        }

        #endregion Singleton pattern implementation

        public const EXECUTION_STATE PreventSleepState =
            EXECUTION_STATE.ES_CONTINUOUS |
            EXECUTION_STATE.ES_DISPLAY_REQUIRED | 
            EXECUTION_STATE.ES_SYSTEM_REQUIRED;

        public const EXECUTION_STATE AllowSleepState = EXECUTION_STATE.ES_CONTINUOUS;

        public bool PreventLockEnabled { get; private set; }

        /// <summary>
        /// Prevent Idle-to-Sleep (monitor not affected)
        /// </summary>
        internal void PreventSleep()
        {
            PreventLockEnabled = true;
            var prevState = SetThreadExecutionState(PreventSleepState);
        }

        /// <summary>
        /// Allow the system to idle to sleep normally.
        /// </summary>
        internal void AllowSleep()
        {
            PreventLockEnabled = false;
            var prevState = SetThreadExecutionState(AllowSleepState);
        }
    }
}

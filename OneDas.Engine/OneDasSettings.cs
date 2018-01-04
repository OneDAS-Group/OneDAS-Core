namespace OneDas.Engine
{
    public class OneDasSettings
    {
        public OneDasSettings()
        {
            this.AspBaseUrl = "http://0.0.0.0:32768";
            this.IsAnonymousAuthenticationEnabled = false;
            this.IsAutostartEnabled = false;
            this.OneDasName = "Prototype";

            this.AdGroupName = "Benutzer";
            this.ApplicationName = "OneDas.Engine";
            this.AutostartTaskPath = @"OneDas\OneDas.Engine";
            this.EventLogShortcutName = "OneDas EventLog";
            this.OneDasTimerShift = 2;
            this.ManagementServiceBaseAddress = "net.pipe://localhost/OneDas/ManagementService";
            this.MutexName = "{fcacd9a4-dc61-4179-b92c-e11538aaf85b}";
        }

        // unset, mutable
        public string BaseDirectoryPath { get; set; }
        public string CurrentProjectFilePath { get; set; }
        public string NewBaseDirectoryPath { get; set; }

        // preset, mutable
        public string AspBaseUrl { get; set; }
        public bool IsAnonymousAuthenticationEnabled { get; set; }
        public bool IsAutostartEnabled { get; set; }
        public string OneDasName { get; set; }

        // preset, immutable
        public string AdGroupName { get; private set; }
        public string ApplicationName { get; private set; }
        public string AutostartTaskPath { get; private set; }
        public string EventLogShortcutName { get; private set; }
        public int OneDasTimerShift { get; private set; }
        public string ManagementServiceBaseAddress { get; private set; }
        public string MutexName { get; private set; }
    }
}

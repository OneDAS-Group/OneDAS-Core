using OneDas.Engine;
using OneDas.Types.Settings;
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace OneDas.WebServer.Shell
{
    [RunInstaller(true)]
    public class OneDasServiceInstaller : Installer
    {
        public OneDasServiceInstaller()
        {
            this.AfterUninstall += this.OneDasServiceInstaller_AfterUninstall;
            this.Committed += this.OneDasServiceInstaller_Committed;

            // serviceProcessInstaller
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem
            };

            // serviceInstaller
            ServiceInstaller serviceInstaller = new ServiceInstaller()
            {
                DisplayName = ConfigurationManager<OneDasSettings>.Settings.ApplicationName,
                ServiceName = ConfigurationManager<OneDasSettings>.Settings.ApplicationName,
                StartType = ServiceStartMode.Automatic,
                DelayedAutoStart = true
            };
            serviceInstaller.Installers.Clear();

            // eventLogInstaller
            EventLogInstaller eventLogInstaller = new EventLogInstaller()
            {
                Log = GlobalSettings.EventLogName,
                Source = ConfigurationManager<OneDasSettings>.Settings.ApplicationName
            };

            // execute
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);

            if (!EventLog.SourceExists(ConfigurationManager<OneDasSettings>.Settings.ApplicationName))
            {
                this.Installers.Add(eventLogInstaller);
            }
        }

        private void OneDasServiceInstaller_Committed(object sender, InstallEventArgs e)
        {
            string fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)}\{ConfigurationManager<OneDasSettings>.Settings.EventLogShortcutName}.lnk";

            if (!File.Exists(fileName))
            {
                //IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)(new IWshRuntimeLibrary.WshShell()).CreateShortcut(fileName);
                //wshShortcut.TargetPath = "C:\\Windows\\System32\\eventvwr.exe";
                //wshShortcut.Arguments = "/c:" + Settings.Default.EventLogName;
                //wshShortcut.Save();
            }

            using (ServiceController servicecontroller = new ServiceController(ConfigurationManager<OneDasSettings>.Settings.ApplicationName))
            {
                servicecontroller.Start();
            }
        }

        private void OneDasServiceInstaller_AfterUninstall(object sender, InstallEventArgs e)
        {
            string fileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)}\{ConfigurationManager<OneDasSettings>.Settings.EventLogShortcutName}.lnk";

            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                }
            }
        }
    }
}
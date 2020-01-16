using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Management.Automation;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class PwshCommand
    {
        #region Fields

        private string _scriptFilePath;
        private ILogger _logger;

        #endregion

        public PwshCommand(string scriptFilePath, ILogger logger)
        {
            _scriptFilePath = scriptFilePath;
            _logger = logger;
        }
            
        public void Run()
        {
            using (PowerShell ps = PowerShell.Create())
            {
                // ensure FluentFTP lib is loaded
                _ = FtpRemoteExists.NoCheck;

                var vdsToolLogger = new VdsToolLogger(_logger);
                _logger.LogInformation($"Executing script '{_scriptFilePath}'.");

                ps.Runspace.SessionStateProxy.SetVariable("dbRoot", Environment.CurrentDirectory);
                ps.Runspace.SessionStateProxy.SetVariable("logger", vdsToolLogger);

                ps.AddScript(File.ReadAllText(_scriptFilePath))
                  .Invoke();
            }
        }
    }
}

% init
Initialize()
import System.*
import OneDas.Infrastructure.*
import OneDas.DataManagement.Connector.*
import OneDas.DataManagement.Infrastructure.*

% variable definition
hostName                = 'ID5052';
port                    = 8080;
dateTimeBegin           = DateTime(2020, 03, 01, 0, 0, 0, DateTimeKind.Utc);
dateTimeEnd             = DateTime(2020, 03, 02, 0, 0, 0, DateTimeKind.Utc);
targetDirectoryPath     = 'data';

channelNames = { ...
    '/IN_MEMORY/ALLOWED/TEST/T/1 s'
    '/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean'
};

% translate variable list into .NET string array
channels = NET.createGeneric('System.Collections.Generic.List', {'System.String'});

for channel = channelNames(:).'
    channels.Add(char(channel));
end

% download and extract
settings = LoadSettings( ...
    dateTimeBegin, dateTimeEnd, ...
    channels ...
);

DownloadAndExtract(hostName, port, targetDirectoryPath, settings)

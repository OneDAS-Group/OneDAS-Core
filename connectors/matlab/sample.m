% init
Initialize()
import System.*
import OneDas.Infrastructure.*
import OneDas.DataManagement.Infrastructure.*
import OneDas.DataManagement.Interface.*

% variable definition
hostName                = 'ID5052';
port                    = 8080;
dateTimeBegin           = DateTime(2019, 01, 01, 0, 0, 0, DateTimeKind.Utc);
dateTimeEnd             = DateTime(2019, 03, 25, 0, 0, 0, DateTimeKind.Utc);
targetDirectoryPath     = 'data';

channelNames = { ...
    '/ANY_EXTERNAL_DATABASE/TEST/TEST/varB/1 s_max';
    '/ANY_EXTERNAL_DATABASE/TEST/TEST/varB/1 s_mean';
};

% translate variable list into .NET string array
channels = NET.createGeneric('System.Collections.Generic.List', {'System.String'});

for channel = channelNames(:).'
    channels.Add(char(channel));
end

% download and extract
settings = DownloadSettings( ...
    dateTimeBegin, dateTimeEnd, ...
    FileGranularity.Day, ...
    channels ...
);

DownloadAndExtract(hostName, port, targetDirectoryPath, settings)

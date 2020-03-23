% init
Initialize()
import System.*
import OneDas.Infrastructure.*
import OneDas.DataManagement.Infrastructure.*

% settings
host            = 'localhost';
port         	= 8080;
username        = 'test@root.org';
password        = '#test0/User1'; % password = input('Please enter you password: ')
targetDir       = 'data';

dateTimeBegin 	= DateTime(2020, 03, 01, 0, 0, 0, DateTimeKind.Utc);
dateTimeEnd   	= DateTime(2020, 03, 02, 0, 0, 0, DateTimeKind.Utc);

% channels (must all be of the same sample rate)
channels = { ...
    '/IN_MEMORY/ALLOWED/TEST/T/1 s'
    '/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean'
};

% export data
connector = MatOneDasConnector(host, port, username, password); % or without authentication: ... = MatOneDasConnector(host, port)

connector.Export( ...
    dateTimeBegin, ...
    dateTimeEnd, ...
    FileFormat.MAT73, ...
    FileGranularity.Day, ...
    channels, ...
    targetDir ...
);

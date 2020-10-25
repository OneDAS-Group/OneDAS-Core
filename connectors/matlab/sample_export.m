%% init
Initialize()
import OneDas.Infrastructure.*
import OneDas.DataManagement.Infrastructure.*

%% settings
host            = 'localhost';
port         	= 80;
username        = 'test@root.org';
password        = '#test0/User1'; % password = input('Please enter your password: ')
secure          = false; % http + ws vs https + wss
targetDir       = 'data';

dateTimeBegin 	= datetime(2020, 02, 01, 0, 0, 0, 'TimeZone', 'UTC');
dateTimeEnd 	= datetime(2020, 02, 03, 0, 0, 0, 'TimeZone', 'UTC');

% channels (must all be of the same sample rate)
channels = { ...
    '/IN_MEMORY/ALLOWED/TEST/T1/1 s_mean'
    '/IN_MEMORY/ALLOWED/TEST/V1/1 s_mean'
};

%% export data
connector = MatOneDasConnector(host, port, secure, username, password); % or without authentication: ... = MatOneDasConnector(host, port, secure)

connector.Export( ...
    dateTimeBegin, ...
    dateTimeEnd, ...
    FileFormat.MAT73, ...
    FileGranularity.Day, ...
    channels, ...
    targetDir ...
);
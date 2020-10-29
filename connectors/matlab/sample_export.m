%% settings
scheme          = 'http';
host            = 'localhost';
port         	= 8080;
username        = 'test@root.org';
password        = '#test0/User1'; % password = input('Please enter your password: ')
targetDir       = 'data';

dateTimeBegin 	= datetime(2020, 02, 01, 0, 0, 0, 'TimeZone', 'UTC');
dateTimeEnd 	= datetime(2020, 02, 02, 0, 0, 0, 'TimeZone', 'UTC');

% channels (must all be of the same sample rate)
channels = { ...
    '/IN_MEMORY/TEST/ACCESSIBLE/T1/1 s_mean'
    '/IN_MEMORY/TEST/ACCESSIBLE/V1/1 s_mean'
};

%% export data
connector = OneDasConnector(scheme, host, port, username, password);
% without authentication: connector = OneDasConnector(scheme, host, port)

params.FileGranularity  = 'Day';    % Minute_1, Minute_10, Hour, Day, SingleFile
params.FileFormat       = 'MAT73';  % CSV, FAMOS, MAT73
params.ChannelPaths     = channels;

connector.Export(dateTimeBegin, dateTimeEnd, params, targetDir);
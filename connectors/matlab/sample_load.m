% init
Initialize()
import System.*

% settings
host            = 'localhost';
port         	= 8080;
username        = 'test@root.org';
password        = '#test0/User1'; % password = input('Please enter you password: ')

dateTimeBegin 	= DateTime(2020, 03, 01, 0, 0, 0, DateTimeKind.Utc);
dateTimeEnd   	= DateTime(2020, 03, 02, 0, 0, 0, DateTimeKind.Utc);

% channels (must all be of the same sample rate)
channels = { ...
    '/IN_MEMORY/ALLOWED/TEST/T/1 s'
    '/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean'
};

% load data
connector = MatOneDasConnector(host, port, username, password); % or without authentication: ... = MatOneDasConnector(host, port)

data = connector.Load( ...
    dateTimeBegin, ...
    dateTimeEnd, ...
    channels ...
);

data1 = data('/IN_MEMORY/ALLOWED/TEST/T/1 s');
data2 = data('/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean');

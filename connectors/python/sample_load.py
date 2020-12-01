# Requires:
# pip install aiohttp
# pip install asyncio
# pip install matplotlib

import asyncio
from datetime import datetime, timedelta, timezone

import matplotlib.pyplot as plt

from OneDasConnector import OneDasConnector

# settings
scheme = "http"
host = "localhost"
port = 8080
username = "test@onedas.org"
password = "#test0/User1" # password = input("Please enter your password: ")

begin = datetime(2020, 2, 1, 0, 0, tzinfo=timezone.utc)
end   = datetime(2020, 2, 2, 0, 0, tzinfo=timezone.utc)

# must all be of the same sample rate
channel_paths = [
    "/IN_MEMORY/TEST/ACCESSIBLE/T1/1 s_mean",
    "/IN_MEMORY/TEST/ACCESSIBLE/V1/1 s_mean"
]

# load data
connector = OneDasConnector(scheme, host, port, username, password) 
# without authentication: connector = OneDasConnector(scheme, host, port)

params = {
    "ChannelPaths": channel_paths,
}

data = asyncio.run(connector.load(begin, end, params))

# prepare plot
y1 = data["/IN_MEMORY/TEST/ACCESSIBLE/T1/1 s_mean"]
y2 = data["/IN_MEMORY/TEST/ACCESSIBLE/V1/1 s_mean"]

sampleRate = 1 # 1 Hz (adapt to your needs)
time = [begin + timedelta(seconds=i/sampleRate) for i in range(len(y1.values))]

# plot
r1 = plt.gca()
r1.plot(time, y1.values, color=(0, 0.4470, 0.7410))
r1.set_ylabel(f"{y1.name} / {y1.unit}")
r1.set_ylim(0, 30)

r2 = r1.twinx()
r2.plot(time, y2.values, color=(0.8500, 0.3250, 0.0980))
r2.set_ylabel(f"{y2.name} / {y2.unit}")
r2.set_ylim(0, 30)

plt.title("OneDAS Connector Sample")
plt.xlabel("Time")
plt.xlim(time[0], time[-1])
plt.grid()
plt.show()
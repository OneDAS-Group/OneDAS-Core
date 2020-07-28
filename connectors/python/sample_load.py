# Requires:
# pip install asyncio
# pip install signalrcore-async
# pip install matplotlib

import asyncio
from datetime import datetime, timedelta, timezone

import matplotlib.pyplot as plt

from OneDasConnector import OneDasConnector

async def main():

    # settings
    host = "localhost"
    port = 80
    username = "test@root.org"
    password = "#test0/User1" # password = input("Please enter your password: ")

    begin = datetime(2020, 2, 1, 0, 0, tzinfo=timezone.utc)
    end   = datetime(2020, 2, 3, 0, 0, tzinfo=timezone.utc)

    channels = [
        "/IN_MEMORY/ALLOWED/TEST/T1/1 s_mean",
        "/IN_MEMORY/ALLOWED/TEST/V1/1 s_mean"
    ]
  
    # load data
    connector = OneDasConnector(host, port, username, password) # or without authentication: ... = OneDasConnector(host, port)
    data = await connector.load(begin, end, channels)

    # prepare plot
    y1 = data["/IN_MEMORY/ALLOWED/TEST/T1/1 s_mean"]
    y2 = data["/IN_MEMORY/ALLOWED/TEST/V1/1 s_mean"]

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

# run event loop
loop = asyncio.get_event_loop()

if loop.is_running() == False:
    asyncio.run(main())
else:
    await main()

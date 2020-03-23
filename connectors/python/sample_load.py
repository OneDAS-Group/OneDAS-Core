import asyncio
from datetime import datetime, timedelta, timezone

import matplotlib.pyplot as plt

from OneDasConnector import OneDasConnector, SampleRateConverter


async def main():

    # settings
    begin = datetime.now(timezone.utc) + timedelta(days=-2)
    end = datetime.now(timezone.utc) + timedelta(days=-1)

    channels = [
        "/IN_MEMORY/ALLOWED/TEST/T/1 s",
        "/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean"
    ]

    # load data
    connector = OneDasConnector("localhost", 8080, "test@root.org", "#test0/User1") # or without authentication: ... = OneDasConnector("localhost", 8080)
    data = await connector.load(begin, end, channels)

    # prepare plot
    y = data["/IN_MEMORY/ALLOWED/TEST/T/1 s"]

    sampleRate = SampleRateConverter().convert(y.dataset_name)
    time = [begin + timedelta(seconds=i/sampleRate) for i in range(len(y.values))]

    # plot
    plt.plot(time, y.values)
    plt.title("OneDAS Connector Sample")
    plt.xlabel("Time")
    plt.xlim(time[0], time[-1])
    plt.ylabel(f"{y.name} / {y.unit}")
    plt.ylim(0, 30)
    plt.grid()
    plt.show()

asyncio.run(main())

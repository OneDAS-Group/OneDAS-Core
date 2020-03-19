import asyncio
from datetime import datetime, timedelta, timezone
from onedas_connector import onedas_connector

import matplotlib.pyplot as plt

async def main():

    # settings
    channels = [
        "/IN_MEMORY/ALLOWED/TEST/varB/1 s_max",
        "/IN_MEMORY/ALLOWED/TEST/varB/1 s_mean"
    ]

    begin = datetime.now(timezone.utc) + timedelta(days=-10)
    end = datetime.now(timezone.utc) + timedelta(days=-5)

    # get data
    connector = onedas_connector("localhost", 8080, "test@root.org", "#test0/User1")
    data = await connector.load(channels, begin, end)

    # prepare plot
    data1 = data["/IN_MEMORY/ALLOWED/TEST/varB/1 s_max"]
    values1 = data1["values"]
    time = [datetime.fromtimestamp(timestamp) for timestamp in values1]
    plt.plot(time, values1)
    plt.title("OneDAS Connector Sample")
    plt.xlabel("Time")
    plt.ylabel("Unix time")

    # plot
    plt.show()

asyncio.run(main())

import asyncio
from datetime import datetime, timedelta, timezone

import matplotlib.pyplot as plt

from OneDasConnector import (FileFormat, FileGranularity, OneDasConnector,
                             SampleRateConverter)


async def main():

    # settings
    host = "localhost"
    port = 8080
    username = "test@root.org"
    password = "#test0/User1" # password = input("Please enter you password: ")

    begin = datetime.now(timezone.utc) + timedelta(days=-2)
    end = datetime.now(timezone.utc) + timedelta(days=-1)

    channels = [
        "/IN_MEMORY/ALLOWED/TEST/T/1 s",
        "/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean"
    ]

    target_folder = "data"

    # export data
    connector = OneDasConnector(host, port, username, password) # or without authentication: ... = OneDasConnector(host, port)
    await connector.export(begin, end, FileFormat.MAT73, FileGranularity.Day, channels, target_folder)   

asyncio.run(main())

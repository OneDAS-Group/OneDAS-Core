# Requires:
# pip install asyncio
# pip install signalrcore-async

import asyncio
from datetime import datetime, timedelta, timezone

from OneDasConnector import FileFormat, FileGranularity, OneDasConnector


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

    target_folder = "data"

    # export data
    connector = OneDasConnector(host, port, username, password) # or without authentication: ... = OneDasConnector(host, port)
    await connector.export(begin, end, FileFormat.MAT73, FileGranularity.Day, channels, target_folder)   

# run event loop
loop = asyncio.get_event_loop()

if loop.is_running() == False:
    asyncio.run(main())
else:
    await main()

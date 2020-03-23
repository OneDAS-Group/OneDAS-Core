import asyncio
from datetime import datetime, timedelta, timezone

import matplotlib.pyplot as plt

from OneDasConnector import (FileFormat, FileGranularity, OneDasConnector,
                             SampleRateConverter)


async def main():

    # settings
    begin = datetime.now(timezone.utc) + timedelta(days=-2)
    end = datetime.now(timezone.utc) + timedelta(days=-1)

    channels = [
        "/IN_MEMORY/ALLOWED/TEST/T/1 s",
        "/IN_MEMORY/ALLOWED/TEST/unix_time2/1 s_mean"
    ]

    target_folder = "data"

    # export data
    connector = OneDasConnector("localhost", 8080, "test@root.org", "#test0/User1") # or without authentication: ... = OneDasConnector("localhost", 8080)
    await connector.export(begin, end, FileFormat.MAT73, FileGranularity.Day, channels, target_folder)   

asyncio.run(main())

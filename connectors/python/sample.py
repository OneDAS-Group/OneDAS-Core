import asyncio
from datetime import datetime, timedelta
from onedas_connector import onedas_connector


async def main():

    channels = [
        "/IN_MEMORY/ALLOWED/TEST/varB/1 s_max",
        "/IN_MEMORY/ALLOWED/TEST/varB/1 s_mean"
    ]
    begin = datetime.now() + timedelta(days=-10)
    end = datetime.now()
    connector = onedas_connector("localhost", 8080, "test@root.org", "#test0/User1")
    
    data = await connector.load(channels, begin, end)

asyncio.run(main())

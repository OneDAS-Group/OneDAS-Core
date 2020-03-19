import asyncio
import json
import logging
from datetime import datetime, timedelta

import requests
from signalrcore_async.hub_connection_builder import HubConnectionBuilder


class onedas_connector():

    buffer = None
    channels = None
    channel_index = None

    def __init__(self, host, port, username="default", password="default", secure=False):
        
        self.logger = logging.getLogger("onedas_connector")
        logging.basicConfig(level=logging.INFO)

        if secure:
            ws_protocol = "wss"
            http_protocol = "https"
        else:
            ws_protocol = "ws"
            http_protocol = "http"

        hub_url = f"{ws_protocol}://{host}:{port}/datahub"
        login_url = f"{http_protocol}://{host}:{port}/identity/account/generatetoken"

        if username == "default":
            self.connection = HubConnectionBuilder()\
                .with_url(hub_url)\
                .build()
        else:
            self.connection = HubConnectionBuilder()\
                .with_url(hub_url, options={
                    "access_token_factory": lambda: self._get_jwt_token(login_url, username, password)
                })\
                .build()

        # .configure_logging(logging.DEBUG)\
    
    async def load(self, channels, begin, end):

        self.connection.on("Downloader.ProgressChanged", self._on_progress_changed)
        self.channels = channels

        beginString = begin.replace(second=0).replace(microsecond=0).isoformat()
        endString = end.replace(second=0).replace(microsecond=0).isoformat()
        result = {}

        try:
            await self.connection.start()
            channelInfos = await self.connection.invoke("GetChannelInfos", [channels])

            for i, channel in enumerate(channels):
                self.buffer = []
                self.channel_index = i
                await self.connection.stream("StreamData", [channel, beginString, endString], self._on_next)
                channelInfos[i]["values"] = self.buffer
                result[channel] = channelInfos[i]

        finally:
            await self.connection.stop()

        return result

    def _on_next(self, data):
        self.buffer.extend(data)

    def _on_progress_changed(self, args):
        progress = args[0]
        progress = (self.channel_index + progress) / len(self.channels)
        self.logger.info(f"Progress: {progress * 100:.0f}%")

    def _get_jwt_token(self, url, username, password):
        data = json.dumps({"username": username, "password": password})
        response = requests.post(url, data=data)
        token = json.loads(response.content)
        return token
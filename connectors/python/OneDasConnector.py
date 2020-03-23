import asyncio
import json
import logging
import re
from datetime import datetime, timedelta
from enum import Enum
from pathlib import Path
from tempfile import NamedTemporaryFile
from typing import List
from zipfile import ZipFile

import requests
from signalrcore_async.hub_connection_builder import HubConnectionBuilder
from signalrcore_async.protocol.msgpack import MessagePackHubProtocol


class ChannelInfo():
    def __init__(self, dict, channel, data):
        self.name = dict["VariableNames"][-1]
        self.group = dict["VariableGroups"][-1]
        self.unit = dict["Units"][-1]
        self.transfer_functions = dict["TransferFunctions"]
        self.dataset_name = channel.split('/')[-1]
        self.values = data

class FileFormat(Enum):
    CSV = 1
    FAMOS = 2
    MAT73 = 3
    CSV2 = 4

class FileGranularity(Enum):
    Minute_1 = 60
    Minute_10 = 600
    Hour = 3600
    Day = 86400

class SampleRateConverter():

    def convert(self, sampleRate: str):

            # Hz
            matchHz = re.match(r"([0-9|\.]+)\sHz", sampleRate)

            if matchHz:
                return int(matchHz.group(1))

            # s
            matchT = re.match(r"([0-9|\.]+)\ss", sampleRate)

            if matchT:
                return 1 / int(matchT.group(1))

            # else
            raise Exception("Unknown sample rate string.")

class OneDasConnector():

    url = None
    buffer = None
    _channel_count = None
    _channel_index = None
    _is_stream = None

    def __init__(self, host, port, username="default", password="default", secure=False):
        
        self.host = host
        self.port = port
        self.secure = secure

        # logging
        self.logger = logging.getLogger("OneDasConnector")
        logging.basicConfig(level=logging.INFO)

        # connection
        self.connection = self._build_hub_connection(host, port, "datahub", username, password, secure)

        # callbacks
        self.connection.on("Downloader.ProgressChanged", self._on_progress_changed)

        # .configure_logging(logging.DEBUG)\
    
    async def load(self, begin, end, channels) -> List[ChannelInfo]:

        self._is_stream = True
        self._channel_count = len(channels)

        begin = begin.replace(second=0, microsecond=0)
        end = end.replace(second=0, microsecond=0)
        result = {}

        try:
            await self.connection.start()
            rawChannelInfos = await self.connection.invoke("GetChannelInfos", [channels])

            for i, channel in enumerate(channels):
                self.buffer = []
                self._channel_index = i
                await self.connection.stream("StreamData", [begin, end, channel], self._on_next)
                result[channel] = ChannelInfo(rawChannelInfos[i], channel, self.buffer)

        finally:
            await self.connection.stop()

        return result

    async def export(self, begin, end, format, granularity, channels, target_folder):

        self._is_stream = False
        self._channel_index = 0
        self._channel_count = len(channels)

        begin = begin.replace(second=0, microsecond=0)
        end = end.replace(second=0, microsecond=0)
        result = {}

        try:
            await self.connection.start()
            await self.connection.stream("ExportData", [begin, end, format, granularity, channels], self._on_next)

            # download and extract zip file
            with NamedTemporaryFile() as target_file:

                if self.secure:
                    url = f"https://{self.host}:{self.port}/{self.url}"
                else:
                    url = f"http://{self.host}:{self.port}/{self.url}"

                # download zip file
                self.logger.info(f"Downloading ZIP file from {url} to {target_file.name}.")
                source_file = requests.get(url, allow_redirects=True)
                target_file.write(source_file.content)

                # create target dir
                Path(target_folder).mkdir(parents=True, exist_ok=True)

                # unzip file
                self.logger.info(f"Extracting files to target folder '{target_folder}'.")   
                with ZipFile(target_file, "r") as zipFile:
                    zipFile.extractall(target_folder)

                self.logger.info(f"Done.")   

        finally:
            await self.connection.stop()

        return result

    def _on_next(self, data):
        if self._is_stream:
            self.buffer.extend(data)
        else:
            self.url = data

    def _on_progress_changed(self, args):
        message = args[1]
        progress = args[0]

        if (self._is_stream):
            progress = (self._channel_index + progress) / self._channel_count

        self.logger.info(f"{progress * 100:3.0f}%: {message}")

    def _get_jwt_token(self, url, username, password):
        request = json.dumps({"username": username, "password": password})
        response = requests.post(url, data=request)
        response.raise_for_status()
        token = json.loads(response.content)
        return token

    def _build_hub_connection(self, host, port, hub, username, password, secure):

        if secure:
            ws_protocol = "wss"
            http_protocol = "https"
        else:
            ws_protocol = "ws"
            http_protocol = "http"

        hub_url = f"{ws_protocol}://{host}:{port}/datahub"
        login_url = f"{http_protocol}://{host}:{port}/identity/account/generatetoken"

        if username == "default":
            return HubConnectionBuilder()\
                .with_url(hub_url)\
                .with_hub_protocol(MessagePackHubProtocol())\
                .build()
        else:
            return HubConnectionBuilder()\
                .with_url(hub_url, options={
                    "access_token_factory": lambda: self._get_jwt_token(login_url, username, password)
                })\
                .with_hub_protocol(MessagePackHubProtocol())\
                .build()

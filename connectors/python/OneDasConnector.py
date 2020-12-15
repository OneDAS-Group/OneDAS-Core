import asyncio
import json
import logging
import struct
from datetime import datetime, timedelta, timezone
from enum import Enum
from pathlib import Path
from tempfile import NamedTemporaryFile
from typing import List
from urllib.parse import quote, urlunsplit
from zipfile import ZipFile

import aiohttp
from aiohttp.client import ClientResponseError


class ChannelInfo():

    values = None

    def __init__(self, dict, channelPath):      
        self.id = dict["id"]
        self.name = dict["name"]
        self.group = dict["group"]
        self.unit = dict["unit"]
        self.transfer_functions = dict["transferFunctions"]
        self.dataset_name = channelPath.split("/")[-1]
        self.description = dict["description"]
        self.special_info = dict["specialInfo"]

class OneDasConnector():

    def __init__(self, scheme, host, port, username="", password=""):
        
        self.scheme = scheme
        self.host = host
        self.port = port
        self.username = username
        self.password = password
        self.token = None
        self.last_progress = None

        # logging
        self.logger = logging.getLogger("OneDasConnector")
        logging.basicConfig(level=logging.INFO)
   
    async def load(self, begin, end, params) -> List[ChannelInfo]:

        begin = begin.replace(tzinfo=timezone.utc)
        end = end.replace(tzinfo=timezone.utc)

        params["Begin"] = begin.strftime("%Y-%m-%dT%H:%M:%SZ")
        params["End"] = end.strftime("%Y-%m-%dT%H:%M:%SZ")

        result = {}

        try:
            self.logger.info("Streaming ... ")
            index = 0

            for i, channelPath in enumerate(params["ChannelPaths"]):
                channelInfo = await self._getChannelInfo(channelPath)
                channelInfo.values = await self._getDataStream( \
                    params, channelPath, index, len(params["ChannelPaths"]))
                result[channelPath] = channelInfo
                index += 1

            self.logger.info("Streaming ... Done.")

        except Exception as ex:
            self.logger.error(f"Streaming ... Fail. Reason: {str(ex)}")
            raise

        return result

    async def export(self, begin, end, params, target_folder):

        begin = begin.replace(tzinfo=timezone.utc)
        end = end.replace(tzinfo=timezone.utc)

        params["Begin"] = begin.strftime("%Y-%m-%dT%H:%M:%SZ")
        params["End"] = end.strftime("%Y-%m-%dT%H:%M:%SZ")

        await self._getDataFiles(params, target_folder)

    async def _getChannelInfo(self, channelPath) -> ChannelInfo :

        channelPathParts = channelPath.split("/")
        projectId = quote("/" + channelPathParts[1] + "/" + channelPathParts[2] + "/" + channelPathParts[3], safe="")
        channelId = quote(channelPathParts[4], safe="")

        url = urlunsplit(( \
            self.scheme, \
            f"{self.host}:{str(self.port)}", \
            "/api/v1" + "/projects/" + projectId + "/channels/" + channelId, \
            None, \
            None))

        async with aiohttp.ClientSession(raise_for_status=True) as session:
            response = await self._send(session, lambda session: session.get(url))
            jsonString = await response.text()
            response.close()
            return ChannelInfo(json.loads(jsonString), channelPath)

    async def _getDataStream(self, params, channelPath, current, total):

        channelPathParts = channelPath.split("/")

        projectId = quote(\
            "/" + channelPathParts[2] + \
            "/" + channelPathParts[3] + \
            "/" + channelPathParts[4], safe="")

        projectId = quote("/" + channelPathParts[1] + "/" + channelPathParts[2] + "/" + channelPathParts[3], safe="")
        channelId = quote(channelPathParts[4], safe="")
        datasetId = quote(channelPathParts[5], safe="")
        begin = params["Begin"]
        end = params["End"]

        url = urlunsplit(( \
            self.scheme, \
            f"{self.host}:{str(self.port)}", \
            "/api/v1/data" + \
            "?projectId=" + projectId +
            "&channelId=" + channelId +
            "&datasetId=" + datasetId +
            "&begin=" + params["Begin"] +
            "&end=" + params["End"],
            None, \
            None))

        async with aiohttp.ClientSession(raise_for_status=True) as session:
            response = await self._send(session, lambda session: session.get(url))
            contentLength = int(response.headers.get('content-length'))
            buffer = bytearray(contentLength)
            target = memoryview(buffer)
            targetOffset = 0

            async for source, _ in response.content.iter_chunks():
                size = len(source)
                target[targetOffset : targetOffset + size] = source
                targetOffset += size

                if (self.last_progress is None):
                    self.last_progress = datetime.now()
                
                if ((datetime.now() - self.last_progress) > timedelta(seconds=1)):
                    progress = (targetOffset / contentLength + current) / total * 100
                    self.logger.info(f"Streaming ... {progress:3.0f} %")
                    self.last_progress = datetime.now()
                
            response.close()
            fmt = f"<{len(buffer) // 8}d"
            return list(struct.unpack(fmt, buffer))
        
    async def _getDataFiles(self, params, target_folder):

        data = json.dumps(params)

        url = urlunsplit(( \
            self.scheme, \
            f"{self.host}:{str(self.port)}", \
            "/api/v1/jobs/export", \
            None, \
            None))

        async with aiohttp.ClientSession(raise_for_status=True) as session:
            headers = {
                "Content-Type": "application/json"
            }
            response = await self._send(session, lambda session: session.post(url, data=data, headers=headers))
            updateUrl = response.headers.get("Location")
            response.close()
        
        while (True):
            jobStatus = await self._getJobStatus(updateUrl)
            
            if jobStatus["status"] == "RanToCompletion":
                downloadUrl = jobStatus["result"]
                break

            elif jobStatus["status"] == "Canceled": 
                self.logger.error("The job has been cancelled.")
                raise Exception("The job has been cancelled.")

            elif jobStatus["status"] == "Faulted": 
                self.logger.error(f"The job is faulted. Reason: {jobStatus['exceptionMessage']}")
                raise Exception(f"The job is faulted. Reason: {jobStatus['exceptionMessage']}")

            elif jobStatus["progressMessage"]: 
                self.logger.info(f"{jobStatus['progress'] * 100:3.0f} %: {jobStatus['progressMessage']}")

            await asyncio.sleep(0.5)

        # download and extract zip file
        with NamedTemporaryFile() as target_file:

            # download
            self.logger.info(f"Downloading ...")

            async with aiohttp.ClientSession(raise_for_status=True) as session:
                response = await self._send(session, lambda session: session.get(downloadUrl))
                
                async for source, _ in response.content.iter_chunks():
                    target_file.write(source)

                response.close()
           
            self.logger.info(f"Downloading ... Done.")

            # unzip
            self.logger.info(f"Unzipping ...")

            with ZipFile(target_file, "r") as zipFile:
                zipFile.extractall(target_folder)

            self.logger.info(f"Unziping ... Done.")

    async def _getJobStatus(self, url):

        async with aiohttp.ClientSession(raise_for_status=True) as session:
            response = await self._send(session, lambda session: session.get(url))
            jsonString = await response.text()
            jobStatus = json.loads(jsonString)
            response.close()

        return jobStatus

    async def _getToken(self):

        url = urlunsplit(( \
            self.scheme, \
            f"{self.host}:{str(self.port)}", \
            "/api/v1/account/token", \
            None, \
            None))

        headers = {
            "Content-Type": "application/json"
        }

        data = {
            "Username": self.username,
            "Password": self.password
        }

        data = json.dumps(data)

        async with aiohttp.ClientSession(raise_for_status=True) as session:
            async with session.post(url, data=data, headers=headers) as response:
                token = await response.text()
                return token[1:-1]

    async def _send(self, session, action):

        if (self.username and not self.username.isspace()):

            # get token if there is no cached one
            if (not self.token):
                self.token = await self._getToken()

            session.headers["Authorization"] =  f"Bearer {self.token}"

            try:
                return await action(session)

            except ClientResponseError as ex:
                
                # get new token if current one is not valid anymore
                if (ex.status == 401):
                    self.token = await self._getToken()
                    session.headers["Authorization"] =  f"Bearer {self.token}"
                    return await action(session)

                else:
                    raise

        else:
            return await action(session)

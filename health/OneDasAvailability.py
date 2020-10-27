import json
import statistics
from datetime import datetime, timedelta, timezone
from typing import Dict

import requests
from signalrcore_async.hub_connection_builder import HubConnectionBuilder

from ..BaseTypes import Checker, CheckResult


class OneDasAvailabilityCheck(Checker):
    Type: str = "onedas-availability"
    Address: str
    Port: int
    Secure: bool
    ProjectName: str
    LimitDays: int
    Username: str
    Password: str

    def __init__(self, settings: Dict[str, str]):
        super().__init__(settings)
        self.Address = settings["address"]
        self.Port = int(settings["port"])
        self.Secure = settings["secure"].lower() == 'true'
        self.ProjectName = settings["project"]
        self.LimitDays = int(settings["past-days"])
        self.LimitPercentWarning = int(settings["limit-percent-warning"])
        self.LimitPercentError = int(settings["limit-percent-error"])

        if "username" in settings:
            self.Username = settings["username"]
        else:
            self.Username = None

        if "password" in settings:
            self.Password = settings["password"]
        else:
            self.Password = None        
                

    def GetName(self) -> str:
        return f"{self.ProjectName}"

    async def DoCheckAsync(self) -> CheckResult:

        if self.Secure:
            ws_protocol = "wss"
            http_protocol = "https"
        else:
            ws_protocol = "ws"
            http_protocol = "http"

        host = self.Address
        port = self.Port
        hub = "datahub"
        hub_url = f"{ws_protocol}://{host}:{port}/{hub}"

        if self._isNullOrWhiteSpace(self.Username) or self._isNullOrWhiteSpace(self.Password):
            connection = HubConnectionBuilder()\
                .with_url(hub_url)\
                .build()
        else:
            login_url = f"{http_protocol}://{host}:{port}/identity/account/generatetoken"

            connection = HubConnectionBuilder()\
                .with_url(hub_url, options={
                    "access_token_factory": lambda: self._get_jwt_token(login_url, self.Username, self.Password)
                })\
                .build()

        try:
            await connection.start()

            projectName = self.ProjectName
            date = datetime.utcnow().date()
            begin = date - timedelta(days=max(self.LimitDays + 1, 2))
            end = date - timedelta(days=1)
            availabilityStatistics = await connection.invoke("GetDataAvailabilityStatistics", [projectName, begin, end])
            availability = statistics.mean(availabilityStatistics["data"])
            message = f"Availability: {availability:.0f} % (last {self.LimitDays} days)"

            if availability < self.LimitPercentError:
                return self.Error(message)
            elif availability < self.LimitPercentWarning:
                return self.Warning(message)
            else:
                return self.Success(message)

        except Exception as ex:

            if "inactivity" in str(ex):
                return self.Success("OneDAS is in inactivity mode.")
            else:
                return self.Error("Could not communicate to OneDAS.")

        finally:
            await connection.stop()

    def _get_jwt_token(self, url, username, password):
        request = json.dumps({"username": username, "password": password})
        response = requests.post(url, data=request)
        response.raise_for_status()
        token = json.loads(response.content)
        return token

    def _isNullOrWhiteSpace(self, value: str):
        return not value or value.isspace()

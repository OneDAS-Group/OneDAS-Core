import statistics
from datetime import datetime, timedelta, timezone
from typing import Dict

from signalrcore_async.hub_connection_builder import HubConnectionBuilder

from ..BaseTypes import Checker, CheckResult


class OneDasAvailabilityCheck(Checker):
    Type: str = "onedas-availability"
    Address: str
    Port: int
    CampaignName: str
    LimitDays: int

    def __init__(self, settings: Dict[str, str]):
        super().__init__(settings)
        self.Address = settings["address"]
        self.Port = int(settings["port"])
        self.CampaignName = settings["campaign"]
        self.LimitDays = int(settings["past-days"])
        self.LimitPercentWarning = int(settings["limit-percent-warning"])
        self.LimitPercentError = int(settings["limit-percent-error"])

    def GetName(self) -> str:
        return f"{self.CampaignName}"

    async def DoCheckAsync(self) -> CheckResult:

        protocol = "ws"
        host = self.Address
        port = self.Port
        hub = "datahub"
        hub_url = f"{protocol}://{host}:{port}/{hub}"

        connection = HubConnectionBuilder()\
            .with_url(hub_url)\
            .build()

        try:
            await connection.start()

            campaignName = self.CampaignName
            date = datetime.utcnow().date()
            begin = date - timedelta(days=max(self.LimitDays + 1, 2))
            end = date - timedelta(days=1)
            availabilityStatistics = await connection.invoke("GetDataAvailabilityStatistics", [campaignName, begin, end])
            availability = statistics.mean(availabilityStatistics["data"])
            message = f"Availability: {availability:.0f} % (last {self.LimitDays} days)"

            if availability < self.LimitPercentError:
                return self.Error(message)
            elif availability < self.LimitPercentWarning:
                return self.Warning(message)
            else:
                return self.Success(message)

        except Exception as ex:

            if "maintenance" in str(ex):
                return self.Success("OneDAS is in maintenance mode.")
            else:
                return self.Error("Could not communicate to OneDAS.")

        finally:
            await connection.stop()

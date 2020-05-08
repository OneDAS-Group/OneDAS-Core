import statistics
from datetime import datetime, timezone
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
        self.LimitDays = int(settings["limit_days"])

    async def DoCheckAsync(self) -> CheckResult:

        protocol = "ws"
        host = self.Address
        port = self.Port
        hub = "datahub"
        hub_url = f"{protocol}://{host}:{port}/{hub}"

        connection = HubConnectionBuilder()\
            .with_url(hub_url)\
            .build()

        name = f"{self.CampaignName}"

        try:
            await connection.start()

            campaignName = self.CampaignName
            begin = datetime(2020, 3, 1, 0, 0, 0, tzinfo=timezone.utc)
            end = datetime(2020, 4, 1, 0, 0, 0, tzinfo=timezone.utc)
            availability = await connection.invoke("GetDataAvailabilityStatistics", [campaignName, begin, end])
            value = statistics.mean(availability["data"])

            return self.Success(name, f"Availability: {value:.0f} %")

        except Exception as ex:
            print(ex)
            return self.Error(name, "Could not communicate to OneDAS.")

        finally:
            await connection.stop()

function BlobSaveAs(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function FileSaveAs(filename, href) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = href;
    link.target = "_blank";
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

var plot;
var progress;
var currentIndex;
var chartContainerId = "visualize-chart-container";

async function UpdateChart(appState, chartEntries, start, end, count, dt, beginAtZero) {  

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/datahub")
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .build();

    await connection.start().then(async () => {

        var beginDate = new Date(start);
        var endDate = new Date(end);

        // sanity checks
        var element = document.getElementById(chartContainerId);

        if (!element)
            return;

        // for each chart entry
        appState.invokeMethodAsync('SetVisualizeProgress', 0);

        connection.on('Downloader.ProgressChanged', progress => {
            progress = (currentIndex + progress) / chartEntries.length;
            appState.invokeMethodAsync('SetVisualizeProgress', progress);
        });

        try {

            for (var i = 0; i < chartEntries.length; i++) {

                var chartEntry = chartEntries[i];
                var channelData = Array(count);
                var offset = 0;
                var currentIndex = i;

                var promise = new Promise(function (resolve, reject) {

                    connection.stream("StreamData", chartEntry.path, beginDate, endDate)
                        .subscribe({
                            next: (item) => {
                                for (var i = 0; i < item.length; i++) {
                                    if (isNaN(item[i]))
                                        channelData[offset + i] = null;
                                    else
                                        channelData[offset + i] = item[i];
                                }

                                offset += item.length;
                            },
                            complete: () => {
                                chartEntry.data = channelData;
                                resolve();
                            },
                            error: (err) => {
                                reject();
                            }
                        });
                });

                try {
                    await promise;
                } catch (e) {
                    console.log(`Fetching data from server failed with error: '${e}'`);
                    return;
                }
            }        
        }
        finally {
            appState.invokeMethodAsync('SetVisualizeProgress', -1);
        }

        var unixBeginDate = beginDate.getTime() / 1000;
        var timeData = Array.from({ length: count }, (x, i) => i * dt + unixBeginDate);

        // plot
        try {
            var opts = generateOpts(timeData, chartEntries, beginAtZero);
            var data = generateDataStructure(timeData, chartEntries);

            if (plot)
                plot.destroy();

            plot = new uPlot(opts, data, document.getElementById(chartContainerId));
        } catch (e) {
            //
        }

        connection.stop();
    });
}
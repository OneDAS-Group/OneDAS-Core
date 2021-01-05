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

function GetBrowserTimeZoneOffset(dateTime) {
    return new Date(dateTime).getTimezoneOffset();
}

var plot;
var progress;
var currentIndex;
var chartContainerId = "visualize-chart-container";

async function UpdateChart(userState, chartEntries, start, end, count, dt, beginAtZero) {  

    var beginDate = new Date(start);
    var endDate = new Date(end);

    // sanity checks
    var element = document.getElementById(chartContainerId);

    if (!element)
        return;

    // for each chart entry
    userState.invokeMethodAsync('SetVisualizeProgress', 0);

    try {

        for (var i = 0; i < chartEntries.length; i++) {

            var chartEntry = chartEntries[i];
            var channelData = Array(count);

            var channelPathParts = chartEntry.path.split("/");
            var projectId = encodeURIComponent('/' + channelPathParts[1] + '/' + channelPathParts[2] + '/' + channelPathParts[3]);
            var channelId = encodeURIComponent(channelPathParts[4]);
            var datasetId = encodeURIComponent(channelPathParts[5]);

            url = "/api/v1/data" +
                  "?projectId=" + projectId +
                  "&channelId=" + channelId +
                  "&datasetId=" + datasetId +
                  "&begin=" + start +
                  "&end=" + end;

            params = {
                method: "GET"
            }

            var response = await fetch(url, params);
            var contentLength = response.headers.get("Content-Length");
            var buffer = new ArrayBuffer(contentLength);
            var target = new Uint8Array(buffer);
            var targetOffset = 0
            var reader = response.body.getReader();

            while (true) {
                var chunk = await reader.read();

                if (chunk.done)
                    break;

                var source = chunk.value
                var size = source.length;

                for (var j = 0; j < size; j++) {
                    target[targetOffset + j] = source[j];
                }

                targetOffset += size;
                
                var progress = (targetOffset / contentLength + i) / chartEntries.length
                await userState.invokeMethodAsync('SetVisualizeProgress', progress);
            }

            var channelData = new Float64Array(buffer);

            // replace NaN by null
            for (var j = 0; j < channelData.length; j++) {
                if (isNaN(channelData[j]))
                    channelData[j] = null;
            }

            chartEntry.data = channelData;
        }        
    }
    finally {
        userState.invokeMethodAsync('SetVisualizeProgress', -1);
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
}
function generateOpts(timeData, chartEntries, beginAtZero) {

    var colormap = [
        [0.0000, 0.4470, 0.7410],
        [0.8500, 0.3250, 0.0980],
        [0.9290, 0.6940, 0.1250],
        [0.4940, 0.1840, 0.5560],
        [0.4660, 0.6740, 0.1880],
        [0.3010, 0.7450, 0.9330],
        [0.6350, 0.0780, 0.1840]
    ];

    // series
    var series = [];

    series.push({
        value: '{DD}.{MM}.{YYYY} {HH}:{mm}'
    });

    chartEntries.forEach((entry, i) => {

        colorCount = colormap.length;

        r = colormap[i % colorCount][0] * 255;
        g = colormap[i % colorCount][1] * 255;
        b = colormap[i % colorCount][2] * 255;

        var stroke = `rgb(${r}, ${g}, ${b})`;
        var fill = `rgba(${r}, ${g}, ${b}, 0.1)`;

        series.push({
            label: entry.name,
            stroke: stroke,
            fill: fill,
            scale: entry.unit,
            spanGaps: false,
            value: (self, rawValue) => formatNumber(rawValue, entry.unit)
        });
    });

    // calculate axes width and scales min/max
    plotParameters = calculatePlotParameters(chartEntries, timeData.length, timeData[0], timeData[timeData.length - 1], timeData[0], timeData[timeData.length - 1]);

    // axes
    var uniqueUnits = chartEntries.map(entry => entry.unit).filter(onlyUnique);
    var axes = [];

    axes.push({
        values: [
            [3600 * 24 * 365, "{YYYY}", 7, "", 1],
            [3600 * 24 * 28, "{MMM}", 7, "\n" + "{YYYY}", 1],
            [3600 * 24, "{DD}.{MM}", 7, "\n" + "{YYYY}", 1],
            [3600, "{HH}", 4, "\n" + "{DD}.{MM}", 1],
            [60, "{HH}:{mm}", 4, "\n" + "{DD}.{MM}", 1],
            [1, ":{ss}", 2, "\n" + "{DD}.{MM}" + " " + "{HH}:{mm}", 1],
            [1e-3, ":{ss}" + ".{fff}", 2, "\n" + "{DD}.{MM}" + " " + "{HH}:{mm}", 1]
        ]
    });

    uniqueUnits.forEach(unit => {
        axes.push({
            scale: unit,
            values: (self, ticks) => ticks.map(rawValue => rawValue.toPrecision(3) + " " + unit),
            grid: { show: false },
            size: plotParameters[unit].size,
            font: "12px monospace"
        });
    });

    // scales
    var scales = {};

    if (beginAtZero) {
        uniqueUnits.forEach(unit => {
            scales[unit] = {
                min: plotParameters[unit].min,
                max: plotParameters[unit].max
            };
        });
    }

    // canvas size
    var sizeData = getElementSizeById(chartContainerId);

    // opts
    return {
        width: sizeData[0] - 20,
        height: sizeData[1] - 100,
        plugins: [
            wheelZoomPlugin({ factor: 0.75 })
        ],
        series: series,
        axes: axes,
        scales: scales,
        tzDate: ts => uPlot.tzDate(new Date(ts * 1e3), 'Etc/UTC')
    };
}

function wheelZoomPlugin(opts) {

    let factor = opts.factor || 0.75;

    function clamp(nRange, nMin, nMax, fRange, fMin, fMax) {
        if (nRange > fRange) {
            nMin = fMin;
            nMax = fMax;
        }
        else if (nMin < fMin) {
            nMin = fMin;
            nMax = fMin + nRange;
        }
        else if (nMax > fMax) {
            nMax = fMax;
            nMin = fMax - nRange;
        }

        return [nMin, nMax];
    }

    return {
        hooks: {
            ready: u => {

                var plot = u.root.querySelector(".over");
                var rect = plot.getBoundingClientRect();

                var xMin = u.scales.x.min;
                var xMax = u.scales.x.max;
                var xRange = u.scales.x.max - u.scales.x.min;

                var yMin = [];
                var yMax = [];
                var yRange = [];

                Object.keys(u.scales).filter(key => key !== "x").forEach(function (key, index) {
                    scale = u.scales[key];
                    yMin.push(scale.min);
                    yMax.push(scale.max);
                    yRange.push(scale.max - scale.min);
                });

                plot.addEventListener("wheel", e => {
                    e.preventDefault();

                    var setScales = [];
                    var { left, top } = u.cursor;

                    // x scale
                    var oxRange = u.scales.x.max - u.scales.x.min;
                    var leftPct = left / rect.width;
                    var xVal = u.posToVal(left, "x");

                    var nxRange = e.deltaY < 0 ? oxRange * factor : oxRange / factor;
                    var nxMin = xVal - leftPct * nxRange;
                    var nxMax = nxMin + nxRange;
                    [nxMin, nxMax] = clamp(nxRange, nxMin, nxMax, xRange, xMin, xMax);

                    setScales.push(() => {
                        u.setScale("x", {
                            min: nxMin,
                            max: nxMax
                        });
                    });

                    // y scales
                    Object.keys(u.scales).filter(key => key !== "x").forEach(function (key, index) {

                        var scale = u.scales[key];

                        if (scale.min === Infinity || scale.max === -Infinity)
                            return;

                        var btmPct = 1 - top / rect.height;
                        var yVal = u.posToVal(top, key);
                        var oyRange = scale.max - scale.min;

                        var nyRange = e.deltaY < 0 ? oyRange * factor : oyRange / factor;
                        var nyMin = yVal - btmPct * nyRange;
                        var nyMax = nyMin + nyRange;
                        [nyMin, nyMax] = clamp(nyRange, nyMin, nyMax, yRange[index], yMin[index], yMax[index]);

                        setScales.push(() => {
                            u.setScale(key, {
                                min: nyMin,
                                max: nyMax
                            });
                        });
                    });

                    // apply changes
                    u.batch(() => {
                        setScales.forEach(setScale => setScale());
                    });
                });
            }
        }
    };
}

function calculatePlotParameters(chartEntries, arrayLength, x0, x1, x0c, x1c) {

    dx = x1 - x0;
    i0 = Math.round((x0c - x0) / dx * (arrayLength - 1));
    i1 = Math.round((x1c - x0) / dx * (arrayLength - 1));

    var uniqueUnits = chartEntries.map(entry => entry.unit).filter(onlyUnique);
    var plotParametersMap = {};

    uniqueUnits.forEach(unit => {

        var plotParameters;

        if (plotParametersMap.hasOwnProperty(unit))
            plotParameters = plotParametersMap[unit];

        chartEntries.filter(entry => entry.unit === unit).forEach(entry => {

            minMax = getMinMax(entry.data, i0, i1);
            minMax2 = uPlot.rangeNum(minMax[0], minMax[1], true);

            if (plotParameters) {
                plotParameters.min = Math.min(plotParameters.min, minMax2[0]);
                plotParameters.max = Math.max(plotParameters.max, minMax2[1]);
            }
            else {
                plotParameters = {
                    min: Math.min(0, minMax2[0]),
                    max: Math.max(0, minMax2[1])
                };

                plotParametersMap[unit] = plotParameters;
            }

            var minValue = formatNumber(plotParameters.min, unit).length;
            var maxValue = formatNumber(plotParameters.max, unit).length;

            plotParameters.size = Math.max(minValue, maxValue) * 9;
        });
    });

    return plotParametersMap;
}

function getMinMax(data, _i0, _i1) {

    var _min = Infinity;
    var _max = -Infinity;

    for (var i = _i0; i <= _i1; i++) {
        if (data[i] !== null) {
            _min = Math.min(_min, data[i]);
            _max = Math.max(_max, data[i]);
        }
    }

    if (_min === Infinity)
        _min = 0;

    if (_max === -Infinity)
        _max = 0;

    return [_min, _max];
}

function formatNumber(value, unit) {

    if (!value)
        return "NaN";

    formattedValue = value.toPrecision(3);

    if (unit)
        return formattedValue + " " + unit;
    else
        return formattedValue;
}

function generateDataStructure(timeData, chartEntries) {

    var dataStructure = chartEntries.map(entry => entry.data);
    dataStructure.unshift(timeData);

    return dataStructure;
}

function getElementSizeById(id) {
    var element = document.getElementById(id);

    if (element)
        return [element.offsetWidth, element.offsetHeight];
    else
        return undefined;
}

function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

function throttle(func, delay) {

    var timerId;

    if (timerId) {
        return;
    }

    timerId = setTimeout(function () {
        func();
        timerId = undefined;
    }, delay);
}

window.onresize = () => {

    var sizeData = getElementSizeById(chartContainerId);

    if (sizeData) {

        var size = {
            width: sizeData[0] - 50,
            height: sizeData[1] - 50
        };

        throttle(() => plot.setSize(size), 100);
    }
};

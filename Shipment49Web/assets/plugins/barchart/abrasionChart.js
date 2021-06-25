// Nvd3 JavaScripts

(function ($) {
    'use strict';

    // Combined Zones Data
    nv.addGraph(function () {
        var chart = nv.models.discreteBarChart()
            .x(function (d) { return d.label })
            .y(function (d) { return d.value })
            .showValues(true)
            .duration(250);

        chart.yAxis.axisLabel("No Of Ropes");
        chart.xAxis.axisLabel("Abrasion Rating");

        var data = $('#CombinedZonesData').val();
        data = JSON.parse(data);

        d3.select('#bar-chart2 svg')
            .datum(data)
            .call(chart);

        nv.utils.windowResize(chart.update);

        return chart;
    });

})(jQuery);
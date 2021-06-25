// Charjs JavaScripts

(function ($) {
    'use strict';

    var primary = '#7774e7',
        success = '#37c936',
        info = '#0f9aee',
        warning = '#ffcc00',
        danger = '#ff3c7e',
        primaryInverse = 'rgba(119, 116, 231, 0.1)',
        successInverse = 'rgba(55, 201, 54, 0.1)',
        infoInverse = 'rgba(15, 154, 238, 0.1)',
        warningInverse = 'rgba(255, 204, 0, 0.1)',
        dangerInverse = 'rgba(255, 60, 126, 0.1)',
        gray = '#f6f7fb',
        white = '#fff',
        dark = '#515365';

    var lblsarr = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    //Bar Chart
    var barChart1 = document.getElementById("bar-chart1");
    var barCtx1 = barChart1.getContext('2d');
    barChart1.height = 80;
    var barConfig1 = new Chart(barCtx1, {
        type: 'bar',
        data: {
            labels: lblsarr,
            datasets: [{
                label: 'External Abrasion - 1',
                backgroundColor: "#0f3876",
                borderColor: "#0f3876",
                pointBackgroundColor: "#0f3876",
                borderWidth: 2,
                data: [65, 59, 80, 81, 56, 55, 40, 37, 54, 76, 63, 62]
            },
            {
                label: 'Internal abrasion - 1',
                backgroundColor: "#dabf68",
                borderColor: "#dabf68",
                pointBackgroundColor: "#dabf68",
                borderWidth: 2,
                data: [28, 48, 40, 19, 86, 27, 90, 43, 65, 76, 87, 85]
            }]
        },

        options: {
            legend: {
                display: true
            }
        }
    });

    //Bar Chart
    var barChart2 = document.getElementById("bar-chart2");
    var barCtx2 = barChart2.getContext('2d');
    barChart2.height = 80;
    var barConfig2 = new Chart(barCtx2, {
        type: 'bar',
        data: {
            labels: lblsarr,
            datasets: [{
                label: 'External Abrasion - 2',
                backgroundColor: "#0f3876",
                borderColor: "#0f3876",
                pointBackgroundColor: "#0f3876",
                borderWidth: 2,
                data: [65, 59, 80, 81, 56, 55, 40, 37, 54, 76, 63, 62]
            },
            {
                label: 'Internal abrasion - 2',
                backgroundColor: "#dabf68",
                borderColor: "#dabf68",
                pointBackgroundColor: "#dabf68",
                borderWidth: 2,
                data: [28, 48, 40, 19, 86, 27, 90, 43, 65, 76, 87, 85]
            }]
        },

        options: {
            legend: {
                display: true
            }
        }
    });

})(jQuery);
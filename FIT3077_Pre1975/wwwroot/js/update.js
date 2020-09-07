/*
 Function to update patients' details
 */
function Update() {
    var IdList = [];

    /// get all the patients' IDs
    $('td').each(function () {
        if ($(this).hasClass('anchorDetail')) {
            IdList.push($(this).attr('data-id'));
        }
    });

    /// ajax request to update patients' details and update view accordingly
    $.ajax({
        url: '/PatientList/ResetView/',
        type: 'POST',
        traditional: true,
        success: function () {
            window.location.reload(false);
        }
    });
}

/*
 Function to update graph details
 */
function UpdateGraph(viewName) {

    /// ajax call update patient detail
    $.ajax({
        url: '/PatientList/ResetView/',
        type: 'POST',
        traditional: true,
        success: function () {
            UpdateSuccess(viewName);
        }
    });

    /// function to update graph after update data call is successful
    function UpdateSuccess(viewName) {

        /// ajax call to get new graph data from AppContext
        $.ajax({
            url: '/PatientList/UpdateGraphData/',
            dataType: "json",
            data: { ViewName: viewName },
            type: 'GET',
            traditional: true,
            success: function (result) {
                if (viewName === "Monitored Patients") {
                    CholesterolGraphCallback(result);
                } else if (viewName === "Historical Monitor") {
                    BloodPressureGraphCallback(result);
                }
            }
        });

        /// function to update cholesterol graph with new data
        function CholesterolGraphCallback(result) {
            if (chart.data.datasets.length > 0) {
                chart.data.labels = result["labels"];
                chart.data.datasets[0].data = result["data"];
                chart.update();
            }
        }

        /// function to update BP graph with new data
        function BloodPressureGraphCallback(result) {
            for (var i = 0; i < charts.length; i++) {
                if (chart.data.datasets.length > 0) {
                    charts[i].data.datasets[0].data = result[i];
                    charts[i].update();
                }
            }
        }
    }
}

/*
 Function to implement a customizable timer
 */
function Timer(fn, t) {
    var timerObj = setInterval(fn, t);

    this.stop = function () {
        if (timerObj) {
            clearInterval(timerObj);
            timerObj = null;
        }
        return this;
    }

    // start timer using current settings (if it's not already running)
    this.start = function () {
        if (!timerObj) {
            this.stop();
            timerObj = setInterval(fn, t);
        }
        return this;
    }

    // start with new or original interval, stop current interval
    this.reset = function (newT = t) {
        t = newT;
        return this.stop().start();
    }
}

var countdown;

/// ajax request to get current interval from AppContext
$.ajax({
    url: '/PatientList/GetUpdateInterval/',
    dataType: "json",
    data: {},
    traditional: true,
    success: function (result) {
        callbackInterval(result);
    }
});

/// Generate view elements (countdown timer, reset listener) with interval data
function callbackInterval(data) {
    countdown = data;
    intervalElement = document.getElementById("interval");
    if (intervalElement != null) {
        intervalElement.dataset.content = toString(countdown);
    }

    /// countdown timer to update data
    var timer = new Timer(function () {
        document.getElementById("countdownTimer").innerHTML = "Data updates after: " + countdown + " seconds.";
        document.getElementById("graphTimer").innerHTML = "Data updates after: " + countdown + " seconds.";
        countdown -= 1;
        if (countdown < 0) {
            if ($('#graphModal').hasClass('show')) {
                UpdateGraph($('#view-name').html());
            }
            else {
                Update();
            }
            countdown = data;
        }
    }, 1000);
    timer.start();

    /// reset timer listener with new interval input
    resetElement = document.getElementById("reset");
    if (resetElement != null) {
        resetElement.addEventListener("click", function (e) {
            var newInterval = parseInt(document.getElementById("interval").value);

            /// check for appropiate new update interval
            if (isNaN(newInterval) || !(Number.isInteger(newInterval)) || (newInterval <= 5)) {
                alert("Update interval should be an integer greater than 5.");
            }

            else {
                /// update new interval to AppContext
                $.ajax({
                    url: '/PatientList/SetUpdateInterval/',
                    data: { newInterval: document.getElementById("interval").value },
                    dataType: "json",
                    type: 'POST',
                    traditional: true
                });

                /// reset the current timer with new data
                countdown = newInterval;
                data = newInterval;
                document.getElementById("interval").dataset.content = document.getElementById("interval").value;
                timer.reset(1000);
            }
        });
    }
}
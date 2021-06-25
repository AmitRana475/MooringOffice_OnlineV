$(function () {
    $('.dtpicker').datepicker({
        dateFormat: 'dd-M-yy',
        changeMonth: true,
        changeYear: true,
        autoclose: true,
        maxDate: "+0D"
    });

    $('.importexport').multiselect({
        includeSelectAllOption: false,
        enableCaseInsensitiveFiltering: true,
        maxHeight: 350,
        numberDisplayed: 1,
        onChange: function (option, checked) {
            // Get selected options.
            var selectedOptions = $('.importexport option:selected');

            if (selectedOptions.length >= 1) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('.importexport option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('.importexport option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
        }
    });

    $('.listbox').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        maxHeight: 350,
        numberDisplayed: 1,
        onSelectAll: function () {
        }
    });

    $('#PortNames').change(function () {

        var portName = $("#PortNames option:selected").text();
        if (portName == "") {
            $('#PortFacilityNames').empty();
        }
        $.ajax({
            type: 'GET',
            url: '/data/analysis/GetFacilities?portName=' + portName,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.Result === true) {
                    $('#PortFacilityNames').empty();

                    $('#PortFacilityNames').append('<option value="">None Selected</option>');
                    for (var i = 0; i < response.Data.length; i++) {
                        $('#PortFacilityNames').append('<option value="' + response.Data[i] + '">' + response.Data[i] + '</option>');

                    }

                }
                else {
                    toastr["error"](response.Message);
                }
            },
            failure: function (response) {
                toastr["error"](response.Message);
            }
        });
    });

    //$('#postsearch').click(function (){
    //    var portName = $("#PortNames option:selected").text();
    //    $.ajax({
    //        type: 'GET',
    //        url: '/data/analysis/GetFacilities?portName=' + portName,
    //        cache: false,
    //        contentType: false,
    //        processData: false,
    //        success: function (response) {
    //            if (response.Result === true) {
                   

    //                $('#PortFacilityNames').append('<option value="">None Selected</option>');
    //                for (var i = 0; i < response.Data.length; i++) {
    //                    $('#PortFacilityNames').append('<option value="' + response.Data[i] + '">' + response.Data[i] + '</option>');

    //                }
    //                var portFacility = $("#PortFacilityNames option:selected").text();
    //                alert(portFacility);
    //                $('#PortFacilityNames').selectedOptions(portFacility);

    //            }
    //            else {
    //                toastr["error"](response.Message);
    //            }
    //        },
    //        failure: function (response) {
    //            toastr["error"](response.Message);
    //        }
    //    });
    //});

    $('#FleetNameIDs').change(function () {
        getVesselInfo();
    });

    $('#FleetTypeIDs').change(function () {
        getVesselInfo();
    });

    $('#TradeIDs').change(function () {
        getVesselInfo();
    });

    //$("#PermissionType").val('');
    //$("#divVessels").hide();
    //$("#divFleetTypes").hide();
    //$("#divFleetNames").hide();

    $("#rdVessel").click(function () {
        var radioValue = $("#rdVessel:checked").val();
        if (radioValue) {
            $("#PermissionType").val('VESSELS');
            $("#divVessels").show();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
        else {
            $("#PermissionType").val('');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
    });

    $("#rdFleetType").click(function () {
        var radioValue = $("#rdFleetType:checked").val();
        if (radioValue) {
            $("#PermissionType").val('FLEETTYPES');
            $("#divVessels").hide();
            $("#divFleetTypes").show();
            $("#divFleetNames").hide();
        }
        else {
            $("#PermissionType").val('');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
    });

    $("#rdFleetName").click(function () {
        var radioValue = $("#rdFleetName:checked").val();
        if (radioValue) {
            $("#PermissionType").val('FLEETNAMES');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").show();
        }
        else {
            $("#PermissionType").val('');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
    });

    $("#rdAll").click(function () {
        var radioValue = $("#rdAll:checked").val();
        if (radioValue) {
            $("#PermissionType").val('ALL');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
        else {
            $("#PermissionType").val('');
            $("#divVessels").hide();
            $("#divFleetTypes").hide();
            $("#divFleetNames").hide();
        }
    });

    $('#UserRole').change(function () {
        var userRole = $("#UserRole option:selected").text();
        if (userRole == "Admin") {
            $("#PermissionGroup").hide();
        }
        else {
            $("#PermissionGroup").show();
        }
    });
});

function getVesselInfo() {

    var fleetNames = "";
    $.each($("#FleetNameIDs option:selected"), function () {
        fleetNames = fleetNames + $(this).val() + ',';
    });

    var fleetTypes = "";
    $.each($("#FleetTypeIDs option:selected"), function () {
        fleetTypes = fleetTypes + $(this).val() + ',';
    });

    var trades = "";
    $.each($("#TradeIDs option:selected"), function () {
        trades = trades + $(this).val() + ',';
    });

    $.ajax({
        type: 'GET',
        url: '/data/analysis/GetVesselDetails?fleetNames=' + fleetNames + '&fleetTypes=' + fleetTypes + '&tradeAreas=' + trades,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#VesselIDs').empty();
                var count = response.Data.length;
                if (count > 0) {
                    for (var i = 0; i < count; i++) {
                        $('#VesselIDs').append('<option value="' + response.Data[i].VesselID + '" text="' + response.Data[i].VesselName + '">' + response.Data[i].VesselName + '</option>');
                    }
                    //$('#VesselIDs').multiselect('rebuild');
                }
                //else {
                //    toastr["error"]("No Vessels Found. Please modify selection.");
                //}
                $('#VesselIDs').multiselect('rebuild');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        error: function (response) {
            toastr["error"](response.Message);
        }
    });


    $.ajax({
        type: 'GET',
        url: '/WinchRotationSettings/WinchRotation/GetVesselDetails?fleetNames=' + fleetNames + '&fleetTypes=' + fleetTypes + '&tradeAreas=' + trades,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#VesselIDs').empty();
                var count = response.Data.length;
                if (count > 0) {
                    for (var i = 0; i < count; i++) {
                        $('#VesselIDs').append('<option value="' + response.Data[i].VesselID + '" text="' + response.Data[i].VesselName + '">' + response.Data[i].VesselName + '</option>');
                    }
                    //$('#VesselIDs').multiselect('rebuild');
                }
                //else {
                //    toastr["error"]("No Vessels Found. Please modify selection.");
                //}
                $('#VesselIDs').multiselect('rebuild');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        error: function (response) {
            toastr["error"](response.Message);
        }
    });
}

function showImage(imagePath) {
    $('#imgName').attr('src', imagePath);
    $('#ImageModal').modal('show');
}

function showImageIns(param) {
    var imgSrc = $(param).attr("src");
    //alert(imgSrc);
    $('#imgNameIns').attr('src', imgSrc);
    $('#ImageModalInspection').modal('show');
}

function getDamages(x,y) {
    var opid = x;  
    var imo = y;
   
    $.ajax({
        type: 'GET',
        url: '/data/analysis/ViewDamageRecords?opid=' + opid + '&imo=' + imo,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#ropeData').empty();
                var tr;
                for (var i = 0; i < response.Data.length; i++) {
                    tr = $('<tr/>');
                    tr.append("<td>" + response.Data[i].CertificateNumber + "</td>");
                    tr.append("<td>" + response.Data[i].AssignedNumber + "</td>");
                    tr.append("<td>" + response.Data[i].Location + "</td>");
                    tr.append("<td>" + response.Data[i].IncidentReport + "</td>");
                    tr.append("<td>" + response.Data[i].IncidentActlion + "</td>");
                    $('#ropeData').append(tr);
                }
                $('#DamageRopeModal').modal('show');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        failure: function (response) {
            toastr["error"](response.Message);
        }
    });
}

function getOperations(x, y) {
   
    var opid = x;
    var imo = y;  
   
    $.ajax({
        type: 'GET',
        url: '/data/analysis/GetOperationDetails?opid=' + opid + '&imo=' + imo,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#opDetails1').empty();
                $('#opDetails2').empty();
                $('#ropeDetails').empty();

                $('#opDetails1').append("<dt>Port Name</dt><dd>" + response.Data.PortName + "</dd>");
                $('#opDetails1').append("<dt>Berth Name</dt><dd>" + response.Data.BirthName + "</dd>");
                $('#opDetails1').append("<dt>Berth Type</dt><dd>" + response.Data.BirthType + "</dd>");
                $('#opDetails1').append("<dt>Fast DateTime</dt><dd>" + response.Data.FastDateTimeString + "</dd>");
                $('#opDetails1').append("<dt>Cast DateTime</dt><dd>" + response.Data.CastDateTimeString + "</dd>");
                $('#opDetails1').append("<dt>Mooring Type</dt><dd>" + response.Data.MooringType + "</dd>");
                $('#opDetails1').append("<dt>Draft Arrival FWD</dt><dd>" + response.Data.DraftArrivalFWD + "</dd>");
                $('#opDetails1').append("<dt>Draft Arrival AFT</dt><dd>" + response.Data.DraftArrivalAFT + "</dd>");
                $('#opDetails1').append("<dt>Draft Departure FWD</dt><dd>" + response.Data.DraftDepartureFWD + "</dd>");
                $('#opDetails1').append("<dt>Draft Departure AFT</dt><dd>" + response.Data.DraftDepartureAFT + "</dd>");
                $('#opDetails1').append("<dt>Depth at Birth</dt><dd>" + response.Data.DepthAtBerth + "</dd>");
                $('#opDetails1').append("<dt>Berth Side</dt><dd>" + response.Data.BerthSide + "</dd>");
                $('#opDetails1').append("<dt>Master Remarks</dt><dd>" + response.Data.PortDetails + "</dd>");

                $('#opDetails2').append("<dt>Vessel Condition</dt><dd>" + response.Data.VesselCondition + "</dd>");
                $('#opDetails2').append("<dt>Ship Access</dt><dd>" + response.Data.ShipAccess + "</dd>");
                $('#opDetails2').append("<dt>Range of Tide</dt><dd>" + response.Data.RangOfTide + "</dd>");
                $('#opDetails2').append("<dt>Wind Direction</dt><dd>" + response.Data.WindDirection + "</dd>");
                $('#opDetails2').append("<dt>Wind Speed</dt><dd>" + response.Data.WindSpeed + "</dd>");
                $('#opDetails2').append("<dt>Any Squall</dt><dd>" + response.Data.AnySquall + "</dd>");
                $('#opDetails2').append("<dt>Current Speed</dt><dd>" + response.Data.CurrentSpeed + "</dd>");
                $('#opDetails2').append("<dt>Berth Exposed SeaSwell</dt><dd>" + response.Data.Berth_exposed_SeaSwell + "</dd>");
                $('#opDetails2').append("<dt>Surging Observed</dt><dd>" + response.Data.SurgingObserved + "</dd>");
                $('#opDetails2').append("<dt>Any Affect Passing Traffic</dt><dd>" + response.Data.Any_Affect_Passing_Traffic + "</dd>");
                $('#opDetails2').append("<dt>Ship Fender Contact</dt><dd>" + response.Data.Ship_was_continuously_contact_with_fender + "</dd>");
                $('#opDetails2').append("<dt>Any Rope Damaged</dt><dd>" + response.Data.Any_Rope_Damaged + "</dd>");
                $('#opDetails2').append("<dt>Facility Name</dt><dd>" + response.Data.FacilityName + "</dd>");

                var tr;
                for (var i = 0; i < response.Data.RopeUsedInOperation.length; i++) {
                    tr = $('<tr/>');
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].AssignedNumber + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Location + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].CertificateNumber + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Lead + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Lead1 + "</td>");
                    $('#ropeDetails').append(tr);
                }
                var tr1;
                for (var j = 0; j < response.Data.RopeTailsUsedInOperation.length; j++) {
                    tr1 = $('<tr/>');
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].AssignedNumber + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Location + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].CertificateNumber + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Lead + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Lead1 + "</td>");
                    $('#ropeTailDetails').append(tr1);
                }

                $('#OperationModal').modal('show');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        failure: function (response) {
            toastr["error"](response.Message);
        }
    });
}


function getOperationsReport(x, y) {
    var opid = x;

    var imo = y;
   
    $.ajax({
        type: 'GET',
        url: '/data/reports/GetOperationDetail?opid=' + opid + '&imo=' + imo,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#opDetails1').empty();
                $('#opDetails2').empty();
                $('#ropeDetails').empty();

                $('#opDetails1').append("<dt>Port Name</dt><dd>" + response.Data.PortName + "</dd>");
                $('#opDetails1').append("<dt>Berth Name</dt><dd>" + response.Data.BirthName + "</dd>");
                $('#opDetails1').append("<dt>Berth Type</dt><dd>" + response.Data.BirthType + "</dd>");
                $('#opDetails1').append("<dt>Fast DateTime</dt><dd>" + response.Data.FastDateTimeString + "</dd>");
                $('#opDetails1').append("<dt>Cast DateTime</dt><dd>" + response.Data.CastDateTimeString + "</dd>");
                $('#opDetails1').append("<dt>Mooring Type</dt><dd>" + response.Data.MooringType + "</dd>");
                $('#opDetails1').append("<dt>Draft Arrival FWD</dt><dd>" + response.Data.DraftArrivalFWD + "</dd>");
                $('#opDetails1').append("<dt>Draft Arrival AFT</dt><dd>" + response.Data.DraftArrivalAFT + "</dd>");
                $('#opDetails1').append("<dt>Draft Departure FWD</dt><dd>" + response.Data.DraftDepartureFWD + "</dd>");
                $('#opDetails1').append("<dt>Draft Departure AFT</dt><dd>" + response.Data.DraftDepartureAFT + "</dd>");
                $('#opDetails1').append("<dt>Depth at Birth</dt><dd>" + response.Data.DepthAtBerth + "</dd>");
                $('#opDetails1').append("<dt>Berth Side</dt><dd>" + response.Data.BerthSide + "</dd>");
                $('#opDetails1').append("<dt>Master Remarks</dt><dd>" + response.Data.PortDetails + "</dd>");

                $('#opDetails2').append("<dt>Vessel Condition</dt><dd>" + response.Data.VesselCondition + "</dd>");
                $('#opDetails2').append("<dt>Ship Access</dt><dd>" + response.Data.ShipAccess + "</dd>");
                $('#opDetails2').append("<dt>Range of Tide</dt><dd>" + response.Data.RangOfTide + "</dd>");
                $('#opDetails2').append("<dt>Wind Direction</dt><dd>" + response.Data.WindDirection + "</dd>");
                $('#opDetails2').append("<dt>Wind Speed</dt><dd>" + response.Data.WindSpeed + "</dd>");
                $('#opDetails2').append("<dt>Any Squall</dt><dd>" + response.Data.AnySquall + "</dd>");
                $('#opDetails2').append("<dt>Current Speed</dt><dd>" + response.Data.CurrentSpeed + "</dd>");
                $('#opDetails2').append("<dt>Berth Exposed SeaSwell</dt><dd>" + response.Data.Berth_exposed_SeaSwell + "</dd>");
                $('#opDetails2').append("<dt>Surging Observed</dt><dd>" + response.Data.SurgingObserved + "</dd>");
                $('#opDetails2').append("<dt>Any Affect Passing Traffic</dt><dd>" + response.Data.Any_Affect_Passing_Traffic + "</dd>");
                $('#opDetails2').append("<dt>Ship Fender Contact</dt><dd>" + response.Data.Ship_was_continuously_contact_with_fender + "</dd>");
                $('#opDetails2').append("<dt>Any Rope Damaged</dt><dd>" + response.Data.Any_Rope_Damaged + "</dd>");
                $('#opDetails2').append("<dt>Facility Name</dt><dd>" + response.Data.FacilityName + "</dd>");

                var tr;
                for (var i = 0; i < response.Data.RopeUsedInOperation.length; i++) {
                    tr = $('<tr/>');
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].AssignedNumber + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Location + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].CertificateNumber + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Lead + "</td>");
                    tr.append("<td>" + response.Data.RopeUsedInOperation[i].Lead1 + "</td>");
                    $('#ropeDetails').append(tr);
                }
                var tr1;
                for (var j = 0; j < response.Data.RopeTailsUsedInOperation.length; j++) {
                    tr1 = $('<tr/>');
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].AssignedNumber + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Location + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].CertificateNumber + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Lead + "</td>");
                    tr1.append("<td>" + response.Data.RopeTailsUsedInOperation[j].Lead1 + "</td>");
                    $('#ropeTailDetails').append(tr1);
                }

                $('#OperationModal').modal('show');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        failure: function (response) {
            toastr["error"](response.Message);
        }
    });
}


function getRopeDetails(inspectionId, ropeTail, vesselId) {
    $.ajax({
        type: 'GET',
        //data: { vesselid: inspectDate, ropetail: tailid, vid: vid },
        url: '/data/reports/ViewInspDetails?inspectionId=' + inspectionId + '&ropeTail=' + ropeTail + '&vesselId=' + vesselId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#tableData').empty();
                var tr;
                for (var i = 0; i < response.Data.length; i++) {
                    tr = $('<tr/>');
                    tr.append("<td>" + response.Data[i].InspectDateString + "</td>");
                    tr.append("<td>" + response.Data[i].InspectBy + "</td>");
                    tr.append("<td>" + response.Data[i].RopeId + "</td>");
                    tr.append("<td>" + response.Data[i].AssignedNumber + "</td>");
                    tr.append("<td><a href='/Data/Reports/RopeDetail?cp=1&vid=" + response.Data[i].VesselID + "' target='_blank'>" + response.Data[i].CertificateNumber + "</a></td>");
                    tr.append("<td>" + response.Data[i].ExternalRating_A + "</td>");
                    tr.append("<td>" + response.Data[i].InternalRating_A + "</td>");
                    tr.append("<td>" + response.Data[i].AverageRating_A + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFAbrasion_A + "</td>");
                    tr.append("<td>" + response.Data[i].DistanceOutboard_A + "</td>");
                    tr.append("<td>" + response.Data[i].CutYarnCount_A + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFGlazing_A + "</td>");
                    tr.append("<td>" + response.Data[i].ExternalRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].InternalRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].AverageRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFAbrasion_B + "</td>");
                    tr.append("<td>" + response.Data[i].DistanceOutboard_B + "</td>");
                    tr.append("<td>" + response.Data[i].CutYarnCount_B + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFGlazing_B + "</td>");
                    tr.append("<td>" + response.Data[i].Chafe_guard_condition + "</td>");
                    tr.append("<td>" + response.Data[i].Twist + "</td>");

                    if (response.Data[i].Image1) {
                        tr.append("<td><a href='#'><img  style='width: 50px; height: 30px; border-radius: 5px;cursor: pointer;transition: 0.3s;' onclick='return showImageIns(this)' src='" + response.Data[i].Image1 + "'></a></td>");
                    }

                    if (response.Data[i].Image2) {
                        tr.append("<td><a href='#'><img style='width: 50px; height: 30px;border-radius: 5px;cursor: pointer;transition: 0.3s;' onclick='return showImageIns(this)' src='" + response.Data[i].Image2 + "'></a></td>");
                        //tr.append("<td><img style='width: 50px; height: 50px;' src='" + response.Data[i].Image2 + "'></td>");
                        //tr.append("<td>" + response.Data[i].Image2 + "</td>");
                    }


                    //tr.append("<td><img src='~/images/InspectionImages/'" + response.Data[i].Photo1 + "/></td>");
                    //tr.append("<td><img src='~/images/InspectionImages/'" + response.Data[i].Photo2 + "/></td>");
                    $('#tableData').append(tr);
                }
                $('#RopeModal').modal('show');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        failure: function (response) {
            toastr["error"](response.Message);
        }
    });
}

function getRopeTailDetails(inspectionId, ropeTail, vesselId) {
    $.ajax({
        type: 'GET',
        //data: { id: id, ropetail: tailid, vid: vid },
        url: '/data/reports/ViewInspDetails?inspectionId=' + inspectionId + '&ropeTail=' + ropeTail + '&vesselId=' + vesselId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.Result === true) {
                $('#tailData').empty();
                var tr;
                for (var i = 0; i < response.Data.length; i++) {
                    tr = $('<tr/>');
                    tr.append("<td>" + response.Data[i].InspectDateString + "</td>");
                    tr.append("<td>" + response.Data[i].InspectBy + "</td>");
                    tr.append("<td>" + response.Data[i].AssignedNumber + "</td>");
                    tr.append("<td><a href='/Data/Reports/RopeDetail?cp=1&vid=" + response.Data[i].VesselID + "' target='_blank'>" + response.Data[i].CertificateNumber + "</a></td>");
                    //tr.append("<td>" + response.Data[i].CertificateNumber + "</td>");
                    tr.append("<td>" + response.Data[i].ExternalRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].InternalRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].AverageRating_B + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFAbrasion_B + "</td>");
                    tr.append("<td>" + response.Data[i].DistanceOutboard_B + "</td>");
                    tr.append("<td>" + response.Data[i].CutYarnCount_B + "</td>");
                    tr.append("<td>" + response.Data[i].LengthOFGlazing_B + "</td>");
                    tr.append("<td>" + response.Data[i].Chafe_guard_condition + "</td>");
                    tr.append("<td>" + response.Data[i].Twist + "</td>");


                    if (response.Data[i].Image1) {
                        tr.append("<td><a href='#'><img  style='width: 50px; height: 30px; border-radius: 5px;cursor: pointer;transition: 0.3s;' onclick='return showImageIns(this)' src='" + response.Data[i].Image1 + "'></a></td>");
                    }

                    if (response.Data[i].Image2) {
                        tr.append("<td><a href='#'><img style='width: 50px; height: 30px;border-radius: 5px;cursor: pointer;transition: 0.3s;' onclick='return showImageIns(this)' src='" + response.Data[i].Image2 + "'></a></td>");
                        //tr.append("<td><img style='width: 50px; height: 50px;' src='" + response.Data[i].Image2 + "'></td>");
                        //tr.append("<td>" + response.Data[i].Image2 + "</td>");
                    }


                    $('#tailData').append(tr);
                }
                $('#RopeTailModal').modal('show');
            }
            else {
                toastr["error"](response.Message);
            }
        },
        failure: function (response) {
            toastr["error"](response.Message);
        }
    });
}


function DeleteAttachment(attachmentId) {
    var r = confirm("Are you sure you want to delete the attachment!");
    if (r == true) {
        $.ajax({
            type: 'GET',
            url: '/msps/detailsview/DeleteAttachment?id=' + attachmentId,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                debugger;
                if (response.Result === true) {
                    $('#attachmentBody').empty();
                    var tr;
                    for (var i = 0; i < response.Data.length; i++) {
                        tr = $('<tr/>');
                        tr.append("<td>" + response.Data[i].AttachmentName + "</td>");
                        tr.append("<td><a href='~/images/AttachFiles/filepath/'" + response.Data[i].ShipId + "/" + response.Data[i].Attachment
                            + ">Download</a> || <a href='#' onclick='return DeleteAttachment('" + response.Data[i].Id + ")>Delete</a></td>");
                        $('#attachmentBody').append(tr);
                    }
                }
                else {
                    toastr["error"](response.Message);
                }
            },
            failure: function (response) {
                toastr["error"](response.Message);
            }
        });
    }
}
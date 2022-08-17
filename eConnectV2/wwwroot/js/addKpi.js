(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (!CheckValidValue()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                if (!CheckWeightage()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();

$(document).ready(function () {
    $(function () {
        $('#btnSubmit,#btn_add').hide();
        $('.select2').select2();
        BindMyDraftKpi($('#FinYearCode').val(), $('#DeptCode').val());
        jQuery('#BottomTarget,#BasicTarget,#ChallengeTarget').on('input propertychange paste', function () {
            CheckValidValue();
        });
        jQuery('#Weightage').on('input propertychange paste', function () {
            CheckWeightage();
        });
        $('#CategoryCode').on('select2:select', function (e) {
            CheckValidValue();
        });
        $('#FinYearCode').change(function () {
            $('#FinYearName').val($("#FinYearCode option:selected").text());
            BindMyDraftKpi($('#FinYearCode').val(), $('#DeptCode').val());
        });
        $('#DeptCode').change(function () {
            BindMyDraftKpi($('#FinYearCode').val(), $('#DeptCode').val());
        });
        $('#btnSubmit').click(function () {
            var kpiNo = $('#KpiNo').val();
            var kpiType = $('#KpiType').val();
            $.ajax({
                url: 'KPI/SubmitKpi',
                type: "POST",
                data: { kpiNo: kpiNo, kpiType: kpiType },
                success: function (response) {
                    debugger;
                    if (response[0].flag == _errorStatus) {
                        Swal.fire("Info!", response[0].msg, "error");
                    } else {
                        $('#Weightage').val(0); //prevent to post button click after page load success
                        Swal.fire("Info!", response[0].msg, "success");
                        $('#btn_add').hide();
                        $('#btnSubmit').hide();
                        $('.subkpibtn').hide();
                        $('#btn-group-kpi').hide();
                        $('#WeightageMessage').hide();
                    }
                },
                error: function () {
                }
            });
        });
    });
});
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
setInputFilter(document.getElementById("Weightage"), function (value) {
    return /^-?\d*[.,]?\d{0,2}$/.test(value) && (value === "" || parseInt(value) <= 50);
});
setInputFilter(document.getElementById("BottomTarget"), function (value) {
    return /^-?\d*[.,]?\d{0,3}$/.test(value);
});
setInputFilter(document.getElementById("BasicTarget"), function (value) {
    return /^-?\d*[.,]?\d{0,3}$/.test(value);
});
setInputFilter(document.getElementById("ChallengeTarget"), function (value) {
    return /^-?\d*[.,]?\d{0,3}$/.test(value);
});
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function CheckValidValue() {
    var isValid = true;
    var msg = '';
    var kpiCategory = $('#CategoryCode').val();
    var btmt = $('#BottomTarget').val();
    var bst = $('#BasicTarget').val();
    var chlngt = $('#ChallengeTarget').val();
    $('#btmunit,#bscunit,#chlunit').text('');
    $('#BottomTarget').prop('readonly', false);
    $('#BasicTarget').prop('readonly', false);
    $('#ChallengeTarget').prop('readonly', false);
    if (kpiCategory == '1') { //lower is better
        if (btmt != '' && bst != '') {
            if (Math.round(btmt * 1000) > Math.round(bst * 1000)) {
                if (bst != '' && chlngt != '') {
                    if (Math.round(bst * 1000) > Math.round(chlngt * 1000)) {
                        msg = '';
                    }
                    else {
                        isValid = false;
                        msg = 'Challenge target should be less than basic target';
                    }
                }
            }
            else {
                isValid = false;
                msg = 'Basic target should be less than bottom target';
            }
        }
    }
    else if (kpiCategory == '2') { // higher is better
        if (btmt != '' && bst != '') {
            if (Math.round(btmt * 1000) < Math.round(bst * 1000)) {
                if (bst != '' && chlngt != '') {
                    if (Math.round(bst * 1000) < Math.round(chlngt * 1000)) {
                        msg = '';
                    }
                    else {
                        isValid = false;
                        msg = 'Challenge target should be greater than basic target';
                    }
                }
            }
            else {
                isValid = false;
                msg = 'Basic target should be greater than bottom target';
            }
        }
    }
    else if (kpiCategory == '3') { // 0 for all target
        $('#BottomTarget').val(0).prop('readonly', true);
        $('#BasicTarget').val(0).prop('readonly', true);
        $('#ChallengeTarget').val(0).prop('readonly', true);

    }
    else if (kpiCategory == '4') { // 1 for all target
        $('#BottomTarget').val(1).prop('readonly', true);
        $('#BasicTarget').val(1).prop('readonly', true);
        $('#ChallengeTarget').val(1).prop('readonly', true);
    }
    else if (kpiCategory == '5') { // 100% for all target
        $('#BottomTarget').val(100).prop('readonly', true);
        $('#BasicTarget').val(100).prop('readonly', true);
        $('#ChallengeTarget').val(100).prop('readonly', true);
        $('#btmunit,#bscunit,#chlunit').text('(%)')
    }
    else {
        msg = '';
    }
    if (!isValid) {
        $('#error-msg').text(msg);
    } else {
        $('#error-msg').text('');
    }
    return isValid;
}
function CheckWeightage() {
    var isValid = true;
    var msg = '';
    var weightage = $('#Weightage').val();
    if (weightage != '') {
        if (parseInt(weightage) >= 2 && parseInt(weightage) <= 50) {
            msg = '';
        }
        else {
            msg = "Weightage should be between 2 to 50";
            isValid = false;
        }
    }
    if (!isValid) {
        $('#error-msg1').text(msg);
    } else {
        $('#error-msg1').text('');
    }
    return isValid;
}

function BindMyDraftKpi(finYear, deptCode) {
    var weightage = 0;
    var requestUrl = 'KPI/GetMyDraftKpis';
    $.post(requestUrl,
        {
            finYear: finYear, deptCode: deptCode
        },
        function (data, status) {
            $('#dt-kpi tbody').empty();
            $('#KpiStatusCode').val(data.kpiStatusCode);
            $('#KpiNo').val(data.kpiNo);
            var row = "";
            var count = 0;
            var categoryname;
            var sign;
            var addpercentage = '';
            $.each(data.kpiList, function (index, item) {
                addpercentage = '';
                count = count + 1;
                weightage = (parseFloat(weightage) + parseFloat(item.weightage)).toFixed(2);
                if (item.categoryCode == '1') {
                    categoryname = 'Lower is better';
                    sign = '>';
                }
                else if (item.categoryCode == '2') {
                    categoryname = 'Higher is better';
                    sign = '<';
                }
                else if (item.categoryCode == '3') {
                    categoryname = '0 for all Target';
                    sign = '-';
                }
                else if (item.categoryCode == '4') {
                    categoryname = '1 for all Target';
                    sign = '-';
                }
                else if (item.categoryCode == '5') {
                    categoryname = '100% for all Target';
                    sign = '-';
                    addpercentage = '%';
                }
                else if (item.categoryCode == '6') {
                    categoryname = 'In terms of cost';
                    sign = '-';
                }
                else {
                    categoryname = '';
                    sign = ' ';
                }
                row += "<tr>";
                row += "<td>" + count + "</td>";
                row += "<td><a title='Delete' class='subkpibtn' href='javascript:void(0)' onclick='RemoveKpi(" + item.kpiNo + "," + item.srNo + "," + item.finYearCode + ")'><i class='fas fa-trash-alt ml-3 text-danger'></i></a></td>";
                row += "<td>" + item.finYearName + "</td>";
                row += "<td>" + item.kpiName + "</td>";
                row += "<td>" + categoryname + "</td>";
                row += "<td>" + item.respDeptName + "</td>";
                row += "<td class='text-center'>" + item.weightage + " % </td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "<td><span class='text-danger'>" + parseFloat(item.apR_BT.toString()) + addpercentage + "</span> " + sign + " <span class='text-warning'>" + parseFloat(item.apR_BS.toString()) + addpercentage + "</span> " + sign + " <span class='text-success'>" + parseFloat(item.apR_CH.toString()) + addpercentage + "</span></td>";
                row += "</tr>";
            });
            if (count == 0) {
                row += "<tr class='no-records-found text-left text-info'><td colspan='19'>No records found</td></tr>";
            }
            $("#dt-kpi tbody").html(row);
            $('#weightageNo').text(weightage);
            if (weightage < 101) {
                $('#progressWeightage').text('Weightage ' + weightage + '% Complete')
                $("#progress-bar").css("width", weightage + "%");
                $('#progress-bar').removeClass("bg-success").addClass("bg-info");
                $('#WeightageMessage').html('<span class="text-info"><i class="fas fa-info-circle"></i> Weightage must be 100% Complete </span>');
            }
            if (weightage == 100) {
                $('#btnSubmit').show();
                $('#progress-bar').removeClass("bg-info").addClass("bg-success");
                $('#WeightageMessage').html('<span class="text-success"><i class="fas fa-check-circle"></i> Weightage 100% Complete. <b>Please submit form </b></span>');
                $('#WeightageMessage').show();
            }
            else {
                $('#btnSubmit').hide();
            }
            if (data.kpiStatusCode == "1" || data.kpiStatusCode == "0") {
                $('#btn_add').show();
            }
            else {
                Swal.fire("Info!", 'This KPI is already submitted for selected financial Year.', "info");
                $('#btn_add').hide();
                $('#btnSubmit').hide();
                $('.subkpibtn').hide();
                $('#btn-group-kpi').hide();
                $('#WeightageMessage').hide();
            }
        });
    $("#KpiName").focus();
}

function RemoveKpi(kpiNo, srNo, finYear) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, remove it!"
    }).then(function (result) {
        if (result.value) {
            var requestUrl = 'KPI/RemoveKpi';
            $.ajax({
                url: requestUrl,
                type: "POST",
                data: { kpiNo: kpiNo, srNo: srNo, finYear: finYear },
                success: function (response) {
                    if (response[0].flag == _errorStatus) {
                        Swal.fire("Info!", response[0].msg, "error");
                    } else {
                        Swal.fire("Removed!", response[0].msg, "success").then(function () {
                            BindMyDraftKpi($('#FinYearCode').val(), $('#DeptCode').val());
                        })
                    }
                },
                error: function () {
                    Swal.fire("Oops...", "Something went wrong!", "error");
                }
            });
        }
    });
}
function GetFileSize() {
    var ext = $('#customFile').val().split('.').pop().toLowerCase();
    if ($.inArray(ext, ['txt', 'doc', 'docx', 'pdf', 'xls', 'xlsx', 'png', 'jpg', 'jpeg', 'gif', 'csv']) == -1) {
        document.getElementById('fp').innerHTML = 'Invalid extension!';
        $('#customFile').val('');
    }
    else {
        document.getElementById('fp').innerHTML = "";
    }
}
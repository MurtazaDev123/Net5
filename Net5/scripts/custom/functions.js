(function ($) {

    getFormattedDate = function (d) {
        'use strict';

        var day = moment(d);
        
        return day.format("DD-MMM-YYYY");


        //return d.getDate() + "-" + getMonth(d.getMonth()) + "-" + d.getFullYear();
    }

    getActualDate = function(d) {
        'use strict';

        var day = moment(d);

        return day.format("YYYY-MM-DD");


        //console.log('day', day);

        //return d.getFullYear() + "-" + parseInt(d.getMonth())+1 + "-" + d.getDate();
    }

    getActualTime = function (d) {
        'use strict';

        var time = moment(d);

        return time.format('H:mm');
    }

    getMonth = function (m) {
        'use strict';
        switch (m) {
            case 0:
                return "Jan";
            case 1:
                return "Feb";
            case 2:
                return "Mar";
            case 3:
                return "Apr";
            case 4:
                return "May";
            case 5:
                return "Jun";
            case 6:
                return "Jul";
            case 7:
                return "Aug";
            case 8:
                return "Sep";
            case 9:
                return "Oct";
            case 10:
                return "Nov";
            case 11:
                return "Dec";
            default:
                return "#invalid";

        }
    }

    getDataTableStatus = function (active) {
        'use strict';
        if (active == true)
            return "<i class='fa fa-check icon-sm text-success'></i> Active";
        else
            return "<i class='fa fa-times icon-sm text-danger'></i> In Active";
    }

    getDataTableSubStatus = function (active) {
        'use strict';
        if (active == 'Active')
            return "<i class='fa fa-check icon-sm text-success'></i> Active";
        else
            return "<i class='fa fa-times icon-sm text-danger'></i> " + active;
    }

    getDataTablePaymentStatus = function (active) {
        'use strict';
        if (active == 'succeeded')
            return "<i class='fa fa-check icon-sm text-success'></i> Succeeded";
        else
            return "<i class='fa fa-times icon-sm text-danger'></i> Failed";
    }

    getDataTableFeatured = function (featured) {
        'use strict';
        if (featured == true)
            return "<i class='fa fa-check icon-sm text-success'></i> Yes";
        else
            return "<i class='fa fa-times icon-sm text-danger'></i> No";
    }

    getDataTableDate = function (date) {
        'use strict';
        var re = /-?\d+/;
        var m = re.exec(date);
        var d = new Date(parseInt(m[0]));

        return "<span style='display: none;'>" + getActualDate(d) + "</span>" + getFormattedDate(d);
    }

    getConvertedDateFromApi = function (value) {
        'use strict';
        var re = /-?\d+/;
        var m = re.exec(value);
        var d = new Date(parseInt(m[0]));

        return d;
    }
    
    getFormattedDecimal = function (v) {
        'use strict';

        if (v.toString() == "NaN") {
            return "0.00";
        }

        var value = parseFloat(v.toString().replace(',', ''));
        //console.log(value);
        if (value != 0) {
            var parts = v.toString().replace(',', '').split(".");
            //console.log(parts.length);

            if (parts.length > 1) {
                return parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "." + (parts[1].length == 1 ? parts[1] + "0" : parts[1].substring(0, 2));
            } else {
                parts[0] = parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ".00";
            }

            //parts[0] = parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ".00";
            return parts.join(".");
        } else {
            return "0.00";
        }
    }

    getFormattedDecimal_2 = function (v) {
        'use strict';

        if (v.toString() == "NaN") {
            return "0.000";
        }

        var value = parseFloat(v.toString().replace(',', ''));
        //console.log(value);
        if (value != 0) {
            var parts = v.toString().replace(',', '').split(".");
            //console.log(parts.length);

            var zeros = "0";


            if (parts.length > 1) {
                return parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "." + (parts[1].length < 3 ? zeros.repeat(3-parts[1].length) : parts[1].substring(0, 2));
            } else {
                parts[0] = parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ".000";
            }

            //parts[0] = parts[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ".00";
            return parts.join(".");
        } else {
            return "0.000";
        }
    }

    getFormattedDecimalValue = function (v) {
        
        var return_value = getFormattedDecimal(v);
        if (return_value == "-")
            return 0;
        else
            return return_value;
    }

    getFormattedInteger = function (v) {
        'use strict';
        var value = parseInt(v.toString().replace(',', ''));
        if (value != 0) {
            v = v.toString().replace(',', '').split(".");
            return v.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "";
        } else {
            return "-";
        }
    }

    getConvertedInteger = function (labelValue) {

        // Nine Zeroes for Billions
        return Math.abs(Number(labelValue)) >= 1.0e+9

        ? Math.abs(Number(labelValue)) / 1.0e+9 + "B"
        // Six Zeroes for Millions 
        : Math.abs(Number(labelValue)) >= 1.0e+6

        ? Math.abs(Number(labelValue)) / 1.0e+6 + "M"
        // Three Zeroes for Thousands
        : Math.abs(Number(labelValue)) >= 1.0e+3

        ? Math.abs(Number(labelValue)) / 1.0e+3 + "K"

        : Math.abs(Number(labelValue));

    }

    getParameterByName = function (name, url) {
        'use strict';

        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    moveRecord = function(id, status, url, redirect_url) {

        var msg_content = 'Do you wish to move this record to archive?';
        var primary_btn_class = 'btn-primary';

        if (status == 1) {
            msg_content = 'Do you wish to restore this record to active?';
            primary_btn_class = 'btn-success';
        }
        
        $.confirm({
            title: 'Confirm ' + (status == 1 ? "Restore" : "Delete") + '!',
            content: msg_content,
            type: status == 2 ? 'red' : 'green',
            buttons: {
                confirm: {
                    text: 'Yes Move',
                    btnClass: primary_btn_class,
                    action: function () {

                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { 'id': id, 'status': status },
                            success: function (data) {

                                if (data["ErrorCode"] == '000') {
                                    SwalSuccesswithRedirect('Success!', 'Record move to archive Successfully.', redirect_url);
                                } else if (data["ErrorCode"] == '001') {
                                    SwalDanger('Video Cannot Delete!');
                                } else {
                                    SwalDanger((status == 1 ? "Restore" : "Delete"), 'Error in ' + (status == 2 ? 'delete' : 'restore') + ' record');
                                }
                            },
                            failure: function (data) {
                                SwalDanger((status == 1 ? "Restore" : "Delete"), 'Error in ' + (status == 2 ? 'delete' : 'restore') + ' record');
                            },
                            always: function () {

                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancel',
                    btnClass: 'btn-danger',
                    action: function () {
                        //$.alert('Canceled!');
                    }
                }
            }
        });
    }

    claimVideoStatus = function (id, status, url, redirect_url) {

        var msg_content = 'Do you wish to reject this claim?';
        var primary_btn_class = 'btn-primary';

        if (status == 1) {
            msg_content = 'Do you wish to approve this claim?';
            primary_btn_class = 'btn-success';
        }

        var status_text = (status == 1 ? "Approve" : "Reject");

        $.confirm({
            title: 'Confirm ' + status_text + '!',
            content: msg_content,
            type: status == 2 ? 'red' : 'green',
            buttons: {
                confirm: {
                    text: 'Yes ' + status_text,
                    btnClass: primary_btn_class,
                    action: function () {

                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { 'id': id, 'status': status },
                            success: function (data) {

                                if (data["ErrorCode"] == '000') {
                                    SwalSuccesswithRedirect('Success!', 'Claim video ' + status_text + ' Successfully.', redirect_url);
                                } else {
                                    SwalDanger(status_text, 'Error in ' + (status == 2 ? 'reject' : 'approve') + ' record');
                                }
                            },
                            failure: function (data) {
                                SwalDanger(status_text, 'Error in ' + (status == 2 ? 'reject' : 'approve') + ' record');
                            },
                            always: function () {

                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancel',
                    btnClass: 'btn-danger',
                    action: function () {
                        //$.alert('Canceled!');
                    }
                }
            }
        });
    }

    pendingVideoStatus = function (id, status, url, redirect_url) {

        var msg_content = 'Do you wish to reject this video?';
        var primary_btn_class = 'btn-primary';

        if (status == 1) {
            msg_content = 'Do you wish to approve this video?';
            primary_btn_class = 'btn-success';
        }

        var status_text = (status == 1 ? "Approve" : "Reject");

        $.confirm({
            title: 'Confirm ' + status_text + '!',
            content: msg_content,
            type: status == 2 ? 'red' : 'green',
            buttons: {
                confirm: {
                    text: 'Yes ' + status_text,
                    btnClass: primary_btn_class,
                    action: function () {

                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { 'id': id, 'status': status },
                            success: function (data) {

                                if (data["ErrorCode"] == '000') {
                                    SwalSuccesswithRedirect('Success!', 'Pending video ' + status_text + ' Successfully.', redirect_url);
                                } else {
                                    SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                                }
                            },
                            failure: function (data) {
                                SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                            },
                            always: function () {

                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancel',
                    btnClass: 'btn-danger',
                    action: function () {
                        //$.alert('Canceled!');
                    }
                }
            }
        });
    }

    pendingPartnerStatus = function (id, status, url, redirect_url) {

        var msg_content = 'Do you wish to reject this Partner?';
        var primary_btn_class = 'btn-primary';

        if (status == 1) {
            msg_content = 'Do you wish to approve this Partner?';
            primary_btn_class = 'btn-success';
        }

        var status_text = (status == 1 ? "Approve" : "Reject");

        $.confirm({
            title: 'Confirm ' + status_text + '!',
            content: msg_content,
            type: status == 2 ? 'red' : 'green',
            buttons: {
                confirm: {
                    text: 'Yes ' + status_text,
                    btnClass: primary_btn_class,
                    action: function () {

                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { 'id': id, 'status': status },
                            success: function (data) {

                                if (data["ErrorCode"] == '000') {
                                    SwalSuccesswithRedirect('Success!', 'Pending Partner ' + status_text + ' Successfully.', redirect_url);
                                } else {
                                    SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                                }
                            },
                            failure: function (data) {
                                SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                            },
                            always: function () {

                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancel',
                    btnClass: 'btn-danger',
                    action: function () {
                        //$.alert('Canceled!');
                    }
                }
            }
        });
    }

    pendingLiveTvStatus = function (id, status, url, redirect_url) {

        var msg_content = 'Do you wish to reject this Live TV?';
        var primary_btn_class = 'btn-primary';

        if (status == 1) {
            msg_content = 'Do you wish to approve this Live TV?';
            primary_btn_class = 'btn-success';
        }

        var status_text = (status == 1 ? "Approve" : "Reject");

        $.confirm({
            title: 'Confirm ' + status_text + '!',
            content: msg_content,
            type: status == 2 ? 'red' : 'green',
            buttons: {
                confirm: {
                    text: 'Yes ' + status_text,
                    btnClass: primary_btn_class,
                    action: function () {

                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { 'id': id, 'status': status },
                            success: function (data) {

                                if (data["ErrorCode"] == '000') {
                                    SwalSuccesswithRedirect('Success!', 'Pending Live TV ' + status_text + ' Successfully.', redirect_url);
                                } else {
                                    SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                                }
                            },
                            failure: function (data) {
                                SwalDanger(status_text, 'Error in ' + (status == 3 ? 'reject' : 'approve') + ' record');
                            },
                            always: function () {

                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancel',
                    btnClass: 'btn-danger',
                    action: function () {
                        //$.alert('Canceled!');
                    }
                }
            }
        });
    }

    SwalSuccesswithRedirect = function (title, text, redirect_url, modal_form) {
        'use strict';
        Swal.fire({
            icon: 'success',
            title: title,
            text: text
        }).then((result) => {

            if (modal_form !== undefined) {
                modal_form.modal('hide');
            }

            if (redirect_url !== undefined) {
                window.location = redirect_url;
            }

            
        });
    }

    SwalErrorwithRedirect = function (title, text, redirect_url, modal_form) {
        'use strict';
        Swal.fire({
            icon: 'error',
            title: title,
            text: text
        }).then((result) => {

            if (modal_form !== undefined) {
                modal_form.modal('hide');
            }

            if (redirect_url !== undefined) {
                window.location = redirect_url;
            }


        });
    }

    SwalDanger = function (title, text) {
        'use strict';
        Swal.fire({
            icon: 'error',
            title: title,
            text: text
        });
    }

    ToastSuccess = function (title, text) {
        'use strict';
        $.toast({
            heading: title,
            text: text,
            showHideTransition: 'slide',
            icon: 'info',
            bgColor: '#FF6D00',
            loaderBg: '#1D1D1D'
        })
    }

    ToastError = function (title, text) {
        'use strict';
        $.toast({
            heading: title,
            text: text,
            showHideTransition: 'slide',
            icon: 'error',
            bgColor: '#b22222',
            loaderBg: '#1D1D1D'
        })
    }

    ToastWarning = function (title, text) {
        'use strict';
        $.toast({
            heading: title,
            text: text,
            showHideTransition: 'slide',
            icon: 'warning',
            bgColor: '#b22222',
            loaderBg: '#1D1D1D'
        })
    }

    fillDropDown = function (service, control, default_value, selected_index, secondary_url, secondary_control, secondary_default_value, secondary_selected_index, apply_select2, dropdownParent) {
        
        'use strict';

        if (typeof selected_index == 'undefined') {
            selected_index = 0;
        }

        control.attr('disabled', 'disabled');
        control.prepend($('<option></option>').html('Loading...'));

        var param = {};
        if (typeof secondary_control != "undefined") {
            param = { 'parent_id': secondary_control.val() };
        }

        var has_select2 = false;

        if (apply_select2 !== undefined) {
            has_select2 = true;
        }

        $.ajax({
            type: "POST",
            url: service,
            data: param,
            async: false,
            success: function (data) {

                control.empty();
               

                var i = 0;

                if (i == selected_index) {
                    control.append($('<option>').text("Select").attr('value', "0").attr('selected', 'selected'));
                } else {
                    control.append($('<option>').text("Select").attr('value', "0"));
                }
                
                i++;

                $.each(data.data, function (index, value) {

                    if (value["Active"] == true) {
                        if (i == selected_index) {
                            control.append($('<option>').text(value["Title"]).attr('value', value["Id"]).attr('selected', 'selected'));
                        } else {
                            control.append($('<option>').text(value["Title"]).attr('value', value["Id"]));
                        }

                        i++;
                    }
                });

                
                if (typeof default_value != 'undefined') {

                    if (has_select2)
                        control.val(default_value).trigger('change');
                    else
                        control.val(default_value);
                }

                if (typeof secondary_url != 'undefined') {
                    if (secondary_url != "") {
                        
                        fillDropDown(secondary_url, secondary_control, secondary_default_value, secondary_selected_index, "", control, undefined, undefined, apply_select2);

                        control.on('change', function () {


                            fillDropDown(secondary_url, secondary_control, default_value, secondary_selected_index, "", control, undefined, undefined, apply_select2);
                        });
                    }
                }
            },
            failure: function (data) {
                console.log("Error during populate cities");
                console.log(data);
            },
            complete: function () {

                control.removeAttr('disabled');

                if (has_select2) {

                    if (dropdownParent !== undefined) {
                        control.select2({
                            dropdownParent: $("#modal_setup")
                        });
                    }
                    else {
                        control.select2();
                    }
                }
                    
            }
        });
    }


})(jQuery);
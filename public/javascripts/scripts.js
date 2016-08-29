function updateVirtualView() {
    var deferred = $.Deferred();
    var cb = function (xhr) {
        if (xhr.status == 200) {
            var txt = xhr.responseText.replace(new RegExp("&lt;", 'g'), "<").replace(new RegExp("&gt;", 'g'), ">");

            $("#contentBox").html(txt);
            updatePopovers();
            deferred.resolve("OK");
        }
    };
    sendQuery("/template/virtualitems", null, "GET", cb);
    return deferred.promise();
}

function changeVirtualView() {
    var cb = function (xhr) {
        if (xhr.status == 200) {
            updateContent(xhr.responseText);
            updatePopovers();
        }
    };
    var val = parseInt($("#listingType").val());
    var query = null;
    if (val === 0) {
        query = "/template/virtualitems";
    }
    else if (val === 1) {
        query = "/template/vieweditems";
    }
    else {
        query = "/template/deleteditems";
    }

    sendQuery(query, null, "GET", cb);
}

function changeSystemMessages() {
    var cb = function (xhr) {
        if (xhr.status == 200) {
            updateContent(xhr.responseText);
        }
    };
    var val = parseInt($("#listingType").val());
    var query = null;
    if (val === 0) {
        query = "/template/importantsystemmessages";
    }
    else if (val === 1) {
        query = "/template/allsystemmessages";
    }

    sendQuery(query, null, "GET", cb);
}

function updateSystemMessageCount() {
    var deferred = $.Deferred();
    var cb = function (xhr) {
        if (xhr.status === 200) {

            var cb2 = function (xhr2) {
                if (xhr2.status === 200) {
                    $("#systemMessagesBadge").remove();
                    $("#messages_link").append(xhr2.responseText);
                    deferred.resolve("OK");
                }
            }
            sendQuery("/template/smbadge/" + xhr.responseText, null, "GET", cb2);
        }
    };

    sendQuery("/systemmessages/getcount", null, "GET", cb);
    return deferred.promise();
}

$(document).ready(function () {
    var currentTab = "passwordChange";
    $('#settingsTabs a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');

        var item = this.toString();
        var hashtag = item.indexOf("#");
        var tab = item.substring(hashtag + 1);
        if (currentTab != null) {
            setHide(tab + "Success");
            setHide(tab + "Error");
        }

        currentTab = tab;
    });

    updatePopovers();
});


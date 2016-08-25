function setError(errid, succid, msg) {
    if(!$("#" + succid).hasClass("hide")) {
        $("#" + succid).addClass("hide");
    }
    $("#" + errid).removeClass("hide");
    $("#" + errid).html(msg);
}

function setSuccess(errid, succid, msg) {
    if(!$("#" + errid).hasClass("hide")) {
        $("#" + errid).addClass("hide");
    }
    $("#" + succid).removeClass("hide");
    $("#" + succid).html(msg);
}

function setHide(id) {
     if(!$("#" + id).hasClass("hide")) {
        $("#" + id).addClass("hide");
    }
}

function sendQuery(url, data, method, callback) {
    $.ajax({
        url: url,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        method: method
    }).complete(function(xhr, textStatus) {
        callback(xhr);
    });
}

function clearForm(id) {
    document.getElementById(id).reset();
}

function openModal(title, text) {
    $("#popupTitle").text(title);
    $("#popupBody").html(text);
    $("#popup").modal();
}

function updatePopovers() {
    $("button[id^='viewItem'").popover({
        trigger: 'focus',
        html: true
    });
}

function decreaseParentCount(parent) {
    if(parseInt(parent) === 0) {
        var count = parseInt($("#parentCount").text());
        $("#parentCount").text(count-1);
    }
    else {
        var count = parseInt($("#" + parent + "_count").text());
        $("#" + parent + "_count").text(count-1);
    }
}

function scrollToTop() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
}
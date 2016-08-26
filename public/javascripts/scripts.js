function updateVirtualView() {
    var cb = function(xhr) {
        if(xhr.status == 200) {
            var txt = xhr.responseText.replace(new RegExp("&lt;", 'g'), "<").replace(new RegExp("&gt;", 'g'), ">");
            
            $("#contentBox").html(txt);
            updatePopovers();
        }
    };
    sendQuery("/template/virtualitems", null, "GET", cb);
}

function changeVirtualView() {
    var cb = function(xhr) {
        if(xhr.status == 200) {
            updateContent(xhr.responseText);
            updatePopovers();
        }
    };
    var val = parseInt($("#listingType").val());
    var query = null;
    if(val === 0) {
        query = "/template/virtualitems";
    }  
    else if(val === 1) {
        query = "/template/vieweditems";
    }
    else {
        query = "/template/deleteditems";
    }

    sendQuery(query, null, "GET", cb);
}

$( document ).ready(function() {
    var currentTab = "passwordChange";
    $('#settingsTabs a').click(function(e) {
        e.preventDefault();
        $(this).tab('show');
        
        var item = this.toString();
        var hashtag = item.indexOf("#");
        var tab = item.substring(hashtag+1);
        if(currentTab != null) {
            setHide(tab + "Success");
            setHide(tab + "Error");
        }

        currentTab = tab;
    });

    updatePopovers();
});

function openMessage(id) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            openModal("System message", xhr.responseText, true);
        }
    };

    sendQuery("/systemmessages/get/" + id, null, "GET", cb);
}

function cleanMessages() {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            var cb2 = function(xhr2) {
                if(xhr2.status === 200) {
                    updateContent(xhr2.responseText);
                }
            };
            sendQuery("/systemmessages/getall", null, "GET", cb2);
        }
    };

    sendQuery("/systemmessages/clean", null, "GET", cb);
}

function deleteMessage(id) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setHide(id + "_tr");
        }
    };

    sendQuery("/systemmessages/delete/" + id, null, "DELETE", cb);
}
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
            var txt = xhr.responseText.replace(new RegExp("&lt;", 'g'), "<").replace(new RegExp("&gt;", 'g'), ">");
            
            $("#contentBox").html(txt);
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
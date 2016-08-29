function openMessage(id) {
    var cb = function (xhr) {
        if (xhr.status === 200) {
            openModal("System message", xhr.responseText, true);
            setHide(id + "_icon");
        }
    };

    sendQuery("/systemmessages/get/" + id, null, "GET", cb);
}

function cleanMessages() {
    var cb = function (xhr) {
        if (xhr.status === 200) {
            var cb2 = function (xhr2) {
                if (xhr2.status === 200) {
                    updateContent(xhr2.responseText);
                }
            };
            sendQuery("/template/importantsystemmessages", null, "GET", cb2);
        }
    };

    sendQuery("/systemmessages/clean", null, "GET", cb);
}

function deleteMessage(id) {
    var cb = function (xhr) {
        if (xhr.status === 200) {
            setHide(id + "_tr");
        }
    };

    sendQuery("/systemmessages/delete/" + id, null, "DELETE", cb);
}
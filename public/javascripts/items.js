function moveItem(id) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            openModal("Move", xhr.responseText);

            $("#popupSave").on('click', function() {
                var cb2 = function(xhr) {
                    if(xhr.status === 200) {
                        updateVirtualView();
                    }
                    $("#popupSave").off('click');
                };
                var data = {ParentId: $("#parentfolder").val()};
                sendQuery("/item/move/" + id, data, "PUT", cb2);
            });
        }
    };

    sendQuery("/template/move/" + id, null, "GET", cb);
}

function editItem(id) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            openModal("Rename", xhr.responseText);

            $("#popupSave").on('click', function() {
                var cb2 = function(xhr) {
                    if(xhr.status === 200) {
                        updateVirtualView();
                    }
                    $("#popupSave").off('click');
                };
                var data = {Name: $("#newname").val()};
                sendQuery("/item/edit/" + id, data, "PUT", cb2);
            });
        }
    };

    sendQuery("/template/edit/" + id, null, "GET", cb);
}

function createFolder() {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            openModal("New folder", xhr.responseText);

            $("#popupSave").on('click', function() {
                var cb2 = function(xhr) {
                    if(xhr.status === 200) {
                        updateVirtualView();
                    }
                    $("#popupSave").off('click');
                };
                var data = { Name:$("#foldername").val(), Parent: $("#parentfolder").val() };
                sendQuery("/item/create/", data, "POST", cb2);
            });
        }
    };

    sendQuery("/template/create/", null, "GET", cb);
}

function viewedItem(id, parent) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setHide(id + "_div");
            setHide(id + "_content");
            decreaseParentCount(parent);
        }
    };
    sendQuery("/item/viewed/" + id, null, "PUT", cb);
}

function deleteItem(id, parent) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setHide(id + "_div");
            setHide(id + "_content");
            decreaseParentCount(parent);
        }
    };
    sendQuery("/item/delete/" + id, null, "DELETE", cb);
}

function unviewedItem(id, parent) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setHide(id + "_div");
            setHide(id + "_content");
            decreaseParentCount(parent);
        }
    };
    sendQuery("/item/unviewed/" + id, null, "PUT", cb);
}

function undeleteItem(id, parent) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setHide(id + "_div");
            setHide(id + "_content");
            decreaseParentCount(parent);
        }
    };
    sendQuery("/item/undelete/" + id, null, "PUT", cb);
}
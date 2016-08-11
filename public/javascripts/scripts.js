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

function createUser() {
    var name = $("#name").val();
    var password = $("#password").val();
    var password2 = $("#passwordRepeat").val();
    var level = $("#level").val();
    if(name.length < 3) {
        setError("createUserError", "createUserSuccess", "Name is too short. Should be at least 3 digits.");
    }
    else if($("#password").val() !== $("#passwordRepeat").val()) {
        setError("createUserError","createUserSuccess", "Passwords do not match.");
    }
    else {
        var cb = function(xhr) {
            if(xhr.status == 200) {
                updateManageUsers("createUserError", "createUserSuccess");
                setTimeout(function() {setSuccess("createUserError","createUserSuccess", "User created.")}, 100);
            }
            else {
                setError("createUserError","createUserSuccess", "Username already exists.");
            }
        };

        var data = {name: name, password: password, level: level};
        sendQuery("/account/create", data, "POST", cb);
    }
}

function deleteUser(id, name) {
    $("#popupTitle").text("Confirmation");
    $("#popupBody").html("Are you sure you want to remove user " + name + "?");
    $("#popupSave").html("Remove");
    var cb = function(xhr) {
        if(xhr.status) {
            updateManageUsers("manageUsersError", "manageUsersSuccess");
            setTimeout(function() {setSuccess("manageUsersError", "manageUsersSuccess", "User " + name + " successfully removed.")}, 100);
        }
        else {
            setError("manageUsersError", "manageUsersSuccess", "Failed to remove " + name + ". Bad permissions.")
        }
        $("#popupSave").off('click');
    };
    $("#popupSave").on('click', function() {
        sendQuery("/account/delete/" + id, null, "DELETE", cb);
    });
}

function updateManageUsers(errid, succid) {
    var cb = function(xhr) {
        if(xhr.status == 200) {
            $("#manageUsers").html(xhr.responseText);
        }
        else {
            setError(errid, succid, "Failed to update manage users tab. Please reload manually.");
        }
    };
    sendQuery("/account/users", null, "GET", cb);
}

function setAdmin(id, value) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            if(parseInt(value) === 0) {
                $("#" + id + "_admin").addClass("hide");
                $("#" + id + "_notadmin").removeClass("hide");
            }
            else {
                $("#" + id + "_notadmin").addClass("hide");
                $("#" + id + "_admin").removeClass("hide");
            }
        }
    };
    sendQuery("/account/admin/" + id, {value: value}, "PUT", cb);
}

function setActive(id, value) {
    var cb = function(xhr) {
        if(xhr.status === 200) {
            console.log("value = " + parseInt(value));
            if(parseInt(value) === 0) {
                $("#" + id + "_active").addClass("hide");
                $("#" + id + "_inactive").removeClass("hide");
            }
            else {
                $("#" + id + "_inactive").addClass("hide");
                $("#" + id + "_active").removeClass("hide");
            }
        }
    };
    sendQuery("/account/active/" + id, {value: value}, "PUT", cb);
}

function updateAdminSettings() {
    var inputs = $("#adminSettings :input").not(":button");
    var data = [];
    for(var i = 0; i < inputs.length; i++) {
        data.push({ id:  inputs[i].id,
                    value: inputs[i].value
        });
    }
    var cb = function(xhr) {
        if(xhr.status === 200) {
            setSuccess("adminSettingsError", "adminSettingsSuccess", "Admin settings updated.");
        }
        else {
            setError("adminSettingsError", "adminSettingsSuccess", "Error updating admin settings.");
        }
    };
    sendQuery("/account/adminsettings", data, "POST", cb);
}

function updateUserPaths() {
    var inputs = $("#userPathsForm :checked").not(":button");
    var data = [];
    for(var i = 0; i < inputs.length; i++) {
        data.push({ id:  inputs[i].id,
                    value: inputs[i].value
        });
    }

    var cb = function(xhr) {
        if(xhr.status === 200) {
            setSuccess("userPathsError", "userPathsSuccess", "User paths updated.");
        }
        else {
            setError("userPathsError", "userPathsSuccess", "Failed to update user paths.");
        }
    }
    sendQuery("/account/userpaths", data, "POST", cb);
}

function changePassword() {
    if($("#newPassword").val() === $("#newPasswordConfirmation").val()) {
        var cb = function(xhr) {
            if(xhr.status === 200) {
                setSuccess("passwordError", "passwordSuccess", "Password successfully changed.");
            }
            else {
                setError("passwordError", "passwordSuccess", "Old password is incorrect.");
            }
        };
        var data = {OldPassword: $("#oldPassword").val(), NewPassword: $("#newPassword").val()};
        sendQuery("/account/password", data, "POST", cb);
    }
    else {
        setError("passwordError", "passwordSuccess", "Passwords do not match.");
    }
}

//TODO: REIMPLEMENT/FIX
function initModalForRename(id, value) {
    $("#popupTitle").text("Name Change");
    $("#popupBody").html('<form>'
        + '<fieldset class="form-group">'
        + '<label for="newname">New name</label>'
        + '<input type="text" class="form-control" id="newname" value="'+value+'">'
        + '</fieldset>');
    
    $("#popupSave").on('click', function() {
        $.ajax({
            url: "/changeItemName/" + id + "/" + $("#newname").val()
        }).done(function(data) {
            $("#"+id+"_name").text($("#newname").val());
            $("#popupSave").off('click');
        });
        
    });
}

function initModalForNewFolder() {
    $("#popupTitle").text("New Folder");
    var body = '<form>'
        + '<fieldset class="form-group">'
        + '<label for="foldername">Folder Name</label>'
        + '<input type="text" class="form-control" id="foldername">'
        + '</fieldset>'
        + '<fieldset class="form-group">'
        + '<label for="parentfolder">Parent Folder</label>'
        + '<select class="form-control" id="parentfolder">'
        + '<option value="0" selected>--No Parent--</option>';
    $("button").each(function(index){
        
        if($(this).data('target') !== undefined && Number($(this).data('target').substr(1))) {
            var id = Number($(this).data('target').substr(1));
            body += '<option value="' + id +'">'+ $("#"+id+"_name").text() +'</option>';
        }
        
    });
    body += '</select>'
    body += '</fieldset>'
    $("#popupBody").html(body);
    
    $("#popupSave").on('click', function() {
        $.ajax({
            url: "/createFolder/" + $("#parentfolder").val() + "/" + $("#foldername").val()
        }).done(function(data) {
            $.ajax({
                url: "/api/virtlist"
            }).done(function(html) {
                $("#maincontainer").html(html);
                $("#popupSave").off('click');
            });
        });
        
    });
}

function initModalForMove(id) {
    $("#popupTitle").text("Move Item");
    var body = '<form>'
        + '<fieldset class="form-group">'
        + '<label for="parentfolder">new Parent Folder</label>'
        + '<select class="form-control" id="parentfolder">'
        + '<option value="0" selected>--No Parent--</option>';
    $("button").each(function(index){
        
        if($(this).data('target') !== undefined && Number($(this).data('target').substr(1))) {
            var parent = Number($(this).data('target').substr(1));
            body += '<option value="' + parent +'">'+ $("#"+parent+"_name").text() +'</option>';
        }
        
    });
    body += '</select>'
    body += '</fieldset>'
    $("#popupBody").html(body);
    
    $("#popupSave").on('click', function() {
        $.ajax({
            url: "/moveItem/" + id + "/" + $("#parentfolder").val()
        }).done(function(data) {
            $.ajax({
                url: "/api/virtlist"
            }).done(function(html) {
                $("#maincontainer").html(html);
                $("#popupSave").off('click');
            });
        });
        
    });
}

$( document ).ready(function() {
    $('#settingsTabs a').click(function(e) {
        e.preventDefault();
        $(this).tab('show');
    });
});
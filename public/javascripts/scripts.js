function updateAdminSettings() {
    var inputs = $("#adminSettings :input").not(":button");
    var data = [];
    for(var i = 0; i < inputs.length; i++) {
        data.push({ id:  inputs[i].id,
                    value: inputs[i].value
        });
    }

    $.ajax({
            url: "/account/adminsettings",
            dataType: "json",
            contentType: "application/json; charset=utf-8c",
            data: JSON.stringify(data),
            method: "POST"

        }).complete(function(xhr, textStatus) {
            if(xhr.status === 200) {
                $("#error").html("User settings updated.");
            }
            else {
                $("#error").html("Error updating user settings.");
            }
        });
}

function updateUserSettings() {
    var inputs = $("#userSettings :input").not(":button");
    var data = [];
    for(var i = 0; i < inputs.length; i++) {
        data.push({ id:  inputs[i].id,
                    value: inputs[i].value
        });
    }

    $.ajax({
            url: "/account/usersettings",
            dataType: "json",
            contentType: "application/json; charset=utf-8c",
            data: JSON.stringify(data),
            method: "POST"

        }).complete(function(xhr, textStatus) {
            if(xhr.status === 200) {
                $("#error").html("User settings updated.");
            }
            else {
                $("#error").html("Error updating user settings.");
            }
        });
}

function changePassword() {
    if($("#newPassword").val() === $("#newPasswordConfirmation").val()) {
        $.ajax({
            url: "/account/settings",
            dataType: "json",
            contentType: "application/json; charset=utf-8c",
            data: JSON.stringify({OldPassword: $("#oldPassword").val(), NewPassword: $("#newPassword").val()}),
            method: "POST"

        }).complete(function(xhr, textStatus) {
            if(xhr.status === 200) {
                $("#error").html("Password CHANGED!!");
            }
            else {
                $("#error").html("BAD OLD PASSWORD");
            }
        });
    }
    else {
        $("#error").html("Passwords do not match");
    }
}


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
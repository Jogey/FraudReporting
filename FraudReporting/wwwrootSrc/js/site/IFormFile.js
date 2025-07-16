class IFormFile {

    //Variables
    el;
    elTable;
    elType;
    elMaximum;
    elProgress;
    elRequired;

    //Methods
    constructor(target) {
        let thisObj = this;

        //Assign elements
        this.el = $(target);
        this.elTable = document.getElementById(target.id + "Table");
        this.elType = document.getElementById(target.id).getAttribute("filetype");
        this.elMaximum = document.getElementById(target.id).getAttribute("maximum");
        this.elProgress = document.getElementById(target.id + "Progress");
        this.elRequired = $(target).attr('required') != null;

        //Assign events
        $(target).change(function (e) {
            var count = thisObj.elTable.getElementsByTagName('tbody')[0].querySelectorAll('.table-item').length;
            if (count < thisObj.elMaximum) {
                var file = document.getElementById(e.target.id).files[0];
                var data = new FormData();
                data.append('file', file);
                data.append('type', thisObj.elType);
                thisObj.Action_uploadfile(data);
            }
        }).click(function () {
            $(this).val("");
            $(this.elProgress).css("width", "0%");
        });
        for (let i = 0; i < $('.customfile-action').length; i++) {
            let id = $('.customfile-action')[i].id;
            $('#' + id).on('click', function (event) {
                thisObj.Action_deletefile(i + 1, id);
            });
        }
        thisObj.HTML_updatebutton();
    }

    Action_uploadfile(data) {
        let thisObj = this;
        let el = this.el;
        let elProgress = this.elProgress;

        el.prop('disabled', true);

        //Append AntiForgeryToken to payload
        var token = $('input[name="__RequestVerificationToken"]').val();
        data.append('__RequestVerificationToken', token);

        //HTTP request to controller
        $.ajax({
            xhr: function () {
                var xhr = new XMLHttpRequest();
                $(elProgress).addClass("progress-bar-animated");
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = (evt.loaded / evt.total * 100) / 1.5;
                        $(elProgress).css("width", percentComplete + "%");
                    }
                }, false);
                xhr.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = (evt.loaded / evt.total * 100);
                        $(elProgress).css("width", percentComplete + "%");
                    }
                }, false);
                return xhr;
            },
            type: 'POST',
            url: GLOBALROOTURL + '/ClientRequest/SaveFileUpload',
            data: data,
            processData: false,
            contentType: false,
            success: function (response) {
                //Add file to Table
                data.append('uniqueid', response);
                thisObj.HTML_addfiletotable(data);
                $(elProgress).css("width", "100%");
                window.setTimeout(function () {
                    $(elProgress).css("width", "0%");
                    $(elProgress).removeClass("progress-bar-animated");
                    el.prop('disabled', false);
                    thisObj.HTML_updatebutton();
                }, 500);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                $(elProgress).css("width", "100%");
                window.setTimeout(function () {
                    $(elProgress).css("width", "0%");
                    $(elProgress).removeClass("progress-bar-animated");
                    el.prop('disabled', false);
                    alert(xhr.responseText);
                }, 500);
            }
        });
    }

    Action_deletefile(tableRow, id) {
        let thisObj = this;

        //Fetch the body of the table
        let table = this.elTable.getElementsByTagName('tbody')[0];

        //Initially delete the row
        table.deleteRow((tableRow - 1));

        //If there are no more rows insert default row
        if (table.rows.length === 0) {
            var newRow = table.insertRow(table.rows.length);
            newRow.outerHTML = '<tr class="table-row"><th scope= "row"></th ><td></td><td><button type="button" class="customfile-action-none" disabled ><i class="fa fa-trash-alt"></i></button></td></tr >';
        } else {
            //Update existing rows with correct ids and attributes
            thisObj.HTML_updaterows();
        }

        //Instantiate payload to send to controller
        var data = new FormData();
        data.append('id', id);

        //Append AntiForgeryToken to payload
        var token = $('input[name="__RequestVerificationToken"]').val();
        data.append('__RequestVerificationToken', token);

        //Send a request to remove that file from the file uploads
        $.ajax({
            type: 'POST',
            url: GLOBALROOTURL + '/ClientRequest/DeleteFileUpload',
            contentType: "application/json; charset=utf-8",
            data: data,
            processData: false,
            contentType: false,
            success: function () {
                //File was removed
                thisObj.HTML_updatebutton();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //File was not found
                alert(thrownError);
            }
        });
    }

    HTML_addfiletotable(data) {
        let thisObj = this;

        //Fetch the body of the table
        var table = this.elTable.getElementsByTagName('tbody')[0];

        //Delete table row if its the starting row
        if (table.rows[0].cells[1].innerHTML.trim() === '') {
            table.deleteRow(0);
        }

        table = this.elTable.getElementsByTagName('tbody')[0];
        var newRow = table.insertRow(table.rows.length);
        newRow.outerHTML = '<tr class="table-row table-item"><th scope="row">' + table.rows.length + '</th ><td>' + data.get('file').name + '</td><td><button type="button" class="customfile-action" id="' + data.get('uniqueid') + '"><i class="fa fa-trash-alt"></i></button></td></tr >';
        let tableRow = table.rows.length;
        let id = data.get('uniqueid');
        $('#' + id).on('click', function (event) {
            thisObj.Action_deletefile(tableRow, id);
        });
    }

    HTML_updaterows() {
        let thisObj = this;

        //Fetch the body of the table
        var table = this.elTable.getElementsByTagName('tbody')[0];

        //Loops through existing rows and changes Id and onlick function attributes
        for (let i = 0; i < table.rows.length; i++) {

            //Replace table Id with correct number
            table.rows[i].cells[0].innerHTML = i + 1;

            //Grab the file Id that was an attribute in the delete method.
            let id = table.rows[i].cells[2].firstChild.getAttribute('id').toString();

            //Assign the fetched file Id to the inner HTML
            table.rows[i].cells[2].innerHTML = '<button type="button" class="customfile-action" id="' + id + '"><i class="fa fa-trash-alt"></i></button>';
            $('#' + id).off('click');
            $('#' + id).on('click', function (event) {
                thisObj.Action_deletefile(i + 1, id);
            });
        }
    }

    HTML_updatebutton() {
        let thisObj = this;
        var count = thisObj.elTable.getElementsByTagName('tbody')[0].querySelectorAll('.table-item').length;

        if (count >= thisObj.elMaximum) {
            thisObj.el.prop('disabled', true);
        }
        else {
            thisObj.el.prop('disabled', false);
        }

        //Workaround for client-side issue: file upload input is required but data is retrieved from the session and cannot set the value of the input (due to security reasons)
        if (this.elRequired) {
            if (count > 0) {
                thisObj.el.prop('required', false);
            }
            else {
                thisObj.el.prop('required', true);
            }
        }
    }
}

//Instantiate on page load if matching HTML is found
let IFormFileList = [];
$(function () {
    let elementsByClass = document.getElementsByClassName("IFormFile");
    for (let i = 0; i < elementsByClass.length; i++) {
        IFormFileList.push(new IFormFile(elementsByClass.item(i)));
    };
});
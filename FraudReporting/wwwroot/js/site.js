

//NOTE: The layout should have the constant GLOBALROOTURL defined - Used for transferring data from server-side to client side via APIs


class Apfco {

    static updateQueryString(key, value, url) { //Update query string parameters - Taken from here: https://stackoverflow.com/questions/5999118/add-or-update-query-string-parameter
        if (!url) url = window.location.href;
        var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi"),
            hash;

        if (re.test(url)) {
            if (typeof value !== 'undefined' && value !== null)
                return url.replace(re, '$1' + key + "=" + value + '$2$3');
            else {
                hash = url.split('#');
                url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
        }
        else {
            if (typeof value !== 'undefined' && value !== null) {
                var separator = url.indexOf('?') !== -1 ? '&' : '?';
                hash = url.split('#');
                url = hash[0] + separator + key + '=' + value;
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
            else
                return url;
        }
    }

    static stripQueryStringAndHashFromUrl(url) { //Example, changes http://localhost:61222/?test=123#NavContent to http://localhost:61222/ - From https://stackoverflow.com/questions/2540969/remove-querystring-from-url
        return url.split(/[?#]/)[0];
    }

    static sleep(milliseconds) { //Use in async functions to insert a delay before continuing processing. Example: await Apfco.Sleep(2000);
        return new Promise(function (resolve) {
            setTimeout(resolve, milliseconds);
        });
    }

    static replaceAll(inputString, findString, replaceString) { //Replace all instances of a value in a string - native string.replace() only works on first instance
        let inputStringRegEscaped = inputString.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
        return inputString.replace(new RegExp(findString, 'g'), replaceString);
    }

}
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
class ViewHomeEntryForm {

    //Methods
    constructor(target) {
        this.target = target;
        console.log('GLOBALROOTURL: ' + GLOBALROOTURL);
        console.log('View initialized');
        //this.sleepTest();
    }

    async sleepTest() {
        console.log('Sleep test A');
        await Apfco.sleep(2000);
        console.log('Sleep test B');
        await Apfco.sleep(2000);
        console.log('Sleep test C');
    }
}

//Instantiate on page load if matching HTML is found
let ViewHomeEntryFormList = [];
$(function () {
    if (document.body.classList.contains('cHome') && document.body.classList.contains('aEntryForm')) {
        let elementsByClass = document.getElementsByClassName("viewWrapper");
        for (let i = 0; i < elementsByClass.length; i++) {
            ViewHomeEntryFormList.push(new ViewHomeEntryForm(elementsByClass.item(i)));
        };
    }
});
class ViewSharedLayoutPrimary {

    //Variables
    target;


    //Methods
    constructor(target) {
        let thisObj = this;
        this.target = target;
        this.disableFormButtonsOnSubmit();
        this.wrapTrademarkSymbolsWithSuperscriptTag();
        console.log('LayoutPrimary initialized');
    }

    disableFormButtonsOnSubmit() {
        //Make it so every submit button found in every form on the site is diabled when the form is submitted (solves numerous issues)
        //Fixes include the "The provided anti-forgery token was meant for user "", but the current user is "ACT-2JFD6AMVSN7C"." which is caused by users double-clicking on the login button
        //Note that this is NOT for security, as users can diable JS. This is only for avoiding user-unfriendly or confusing error messages
        $(this.target).find('form').each(function () {
            $(this).submit(function (event) {
                $(this).find(':input[type=submit]').each(function () {
                    if (!$(this).attr('apfDontDisableOnSubmit')) { //Don't disable if the apfDontDisableOnSubmit="true" attribute exists on the button
                        $(this).prop('disabled', true);
                    }
                });
            });
        });
        $(this.target).find('form').each(function () {
            let currentForm = this; //Get the current form
            $(this).find(':input[type=submit]').each(function () {
                if (!$(this).attr('apfDontDisableOnSubmit')) { //Don't disable if the apfDontDisableOnSubmit="true" attribute exists on the button
                    $(this).click(function (event) {
                        $('<input>').attr({ //Create a hidden property for the clicked button to submit along with the form. Solves the issue with disabled buttons not getting submitted. This way you can find which button was clicked
                            type: 'hidden',
                            name: $(this).attr('name'),
                            value: $(this).val()
                        }).insertBefore($(currentForm).children().first()); //Append to the start of the current form, instead of next to the button itself or the end of the form. Otherwise it may mess with any last-child CSS selectors
                    });
                }
            });
        });
    }

    wrapTrademarkSymbolsWithSuperscriptTag() {
        //Wrap all registered trademark symbols with <sup>, as it applies to almost every design mockup we receive - see here https://stackoverflow.com/questions/19364581/adding-superscript-sup-tags-around-all-trademark-and-registered-trademark-symb
        let regexp = /[\xAE]/;
        $('body :not(script,sup)').contents().filter(function () {
            return this.nodeType === 3 && (regexp.test(this.nodeValue));
        }).replaceWith(function () {
            return this.nodeValue.replace(regexp, '<sup>$&</sup>');
        });
        $('body :not(script,sup)').contents().filter(function () {
            return this.nodeType === 3;
        }).replaceWith(function () {
            return this.nodeValue.replace(/[™®©]/g, '<sup>$&</sup>');
        });
    }
}

//On page load, take all found elements with the classname and create objects for them to initialize
let ViewSharedLayoutPrimaryList = [];
$(function () {
    let elementsByClass = document.getElementsByClassName("LayoutPrimary");
    for (let i = 0; i < elementsByClass.length; i++) {
        ViewSharedLayoutPrimaryList.push(new ViewSharedLayoutPrimary(elementsByClass.item(i)));
    };
});
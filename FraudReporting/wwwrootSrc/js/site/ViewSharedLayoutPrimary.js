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


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
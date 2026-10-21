mergeInto(LibraryManager.library, {

    DebugLog: function (str) {
        console.log(UTF8ToString(str));
    },

    SetToLocalStorage: function (key, value) {
        var keyString = UTF8ToString(key);
        var valueString = UTF8ToString(value);
        localStorage.setItem(keyString, valueString);
    },









    //Ignoring these for now
    GetFromLocalStorage: function (key, value) {
        var keyString = UTF8ToString(key);

        // convert to string
        var returnStr = keyString;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
    },

    RemoveFromLocalStorage: function (key){
        var keyString = UTF8ToString(keyString);
        localStorage.removeItem(key);
    },

    GetPropsFromURL: function (param) {
        //convert the param name to a string
        var paramName = UTF8ToString(param);

        //try to get the paramName in the params
        const parsedUrl = new URL(window.location.href);
        var val = parsedUrl.searchParams.get(paramName);
        console.log(val);

        //we return null if it's null/undefined/empty
        if(val === null || val === undefined || val === "") {
            console.log(`Prop ${paramName} doesn't exist`);
            val = "";
        }

        // convert to string
        var returnStr = val;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);

        //actually return
        return buffer;
    },

    GoToLoginPage: function ()  {
        window.location.replace("http://www.snowdrama.net");
    },
});


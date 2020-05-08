(function () {
    'use strict';
    try {
        var ___log_text = function (text) { ___api.log('[FILE___API]', text); };
        var ___log_key = function (key, text) { ___api.log('[FILE___API].' + key, text); };

        var ___api_call = function (api_name, paramenter, request, result_type) {
            var v = ___api.js_call('[FILE___API]', api_name, JSON.stringify(paramenter), JSON.stringify(request));
            if (result_type == 'text')
                return v;
            else
                return JSON.parse(v);
        };

        var ___request = [TEXT_REQUEST];
        var ___parameters = [TEXT_PARAMETER];

        //___log_text('This is from JavaScript...');

        [EXECUTE_TEXT_JS]

    } catch (e) {
        return { ok: false, name: '" + file___api + @"', error: e.message };
    }
})()
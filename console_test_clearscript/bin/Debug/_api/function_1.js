/*
 
 */
var result = { ok: true, data: null, text: 'custom json output...' };
try {
    //___log_text('Thinh LOG');

    var ofile = ___api_call('file_read', { file: '_api/function_1.sql' }, {});

    ___log_text(JSON.stringify(ofile));

    if (ofile.ok) {

        var para = {
            connect_string: 'server=192.168.10.54;database=Test;UId=sa; Password=dev@123;Connection Timeout=180',
            script_command: ofile.data
        };


        //___log_text(JSON.stringify(para));


        //var text_json = ___api_call('db_execute', para, {}, 'text');
        //___log_text(text_json);

        var o = ___api_call('db_execute', para, {});
        if (o.ok) {
            result.data = o.data;
        }

    }
} catch (e) {
    result.ok = false;
    result.message = e.message;
}

//___log_text(JSON.stringify(result));

return JSON.stringify(result);



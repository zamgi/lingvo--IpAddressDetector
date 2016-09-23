
$(document).ready(function () {
    var textOnChange = function () {
        var len = $("#text").text().length.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        $("#textLength").html("length of text: " + len + " characters");
    };

    $("#text").focus(textOnChange).change(textOnChange).keydown(textOnChange).keyup(textOnChange).select(textOnChange).focus();

    $('#mainPageContent').on('click', '#processButton', function () {
        if($(this).hasClass('disabled')) return false;

        var text = trim_text($('#text').val().toString());
        if (is_text_empty(text)) {
            alert("Enter the text to be processed.");
            $("#text").focus();
            return (false);
        }

        processing_start();

        $.ajax({
            type: "POST",
            url:  "RESTProcessHandler.ashx",
            data: {
                text: text
            },
            success: function (responce) {
                if (responce.err) {
                    processing_end();
                    $('#processResult').addClass('error').text(responce.err);
                } else {
                    $('#processResult').removeClass('error').text('');

                    if (responce.ips && responce.ips.length != 0) {
                        var html = '';
                        var startIndex = 0;
                        for (var i = 0, len = responce.ips.length; i < len; i++) {
                            var ip = responce.ips[ i ];
                            //text = text.insert( ip.startIndex + ip.length, '</span>' ).insert( ip.startIndex, '<span class="ip">' );
                            var ip_value = text.substr(ip.startIndex, ip.length);
                            html += text.substr(startIndex, ip.startIndex - startIndex) +
                                    '<span class="ip" title=' + ip_value + '>' + ip_value + '</span>';
                            startIndex = ip.startIndex + ip.length;
                        }
                        html += text.substr(startIndex, text.length - startIndex);
                        html = html.replaceAll('\r\n', '<br/>').replaceAll('\n', '<br/>').replaceAll('\t', '&nbsp;&nbsp;&nbsp;&nbsp;');

                        processing_end();
                        $('#processResult').html( html );
                        $('#resultCount').text('found IP(v4) addresses: ' + responce.ips.length);
                    } else {
                        processing_end();
                        $('#processResult').html('<div style="text-align: center; padding: 15px;"><b>IP(v4) адресов</b> в тексте не найденно</div>');
                    }

                    //---$('#text').html( text );
                }
            },
            error: function () {
                $('#processResult').empty();
                processing_end();
                $('#processResult').addClass('error').text('server error');
            }
        });
        
    });

    load_texts();

    function processing_start(){
        $('#text').addClass('no-change').attr('readonly');
        $('#processResult').removeClass('error').html('<div style="text-align: center">Processing...</div>');
        $('#processButton').addClass('disabled');
    };
    function processing_end(){
        $('#text').removeAttr('readonly').removeClass('no-change');
        $('#processResult').removeClass('error').text('');
        $('#resultCount').text('');
        $('#processButton').removeClass('disabled');
    };
    function trim_text(text) {
        return (text.replace(/(^\s+)|(\s+$)/g, ""));
    };
    function is_text_empty(text) {
        return (text.replace(/(^\s+)|(\s+$)/g, "") == "");
    };
    String.prototype.insert = function (index, str) {
        if (0 < index)
            return (this.substring(0, index) + str + this.substring(index, this.length));
        return (str + this);
    };
    String.prototype.replaceAll = function (token, newToken, ignoreCase) {
        var _token;
        var str = this + "";
        var i = -1;
        if (typeof token === "string") {
            if (ignoreCase) {
                _token = token.toLowerCase();
                while ((i = str.toLowerCase().indexOf(token, i >= 0 ? i + newToken.length : 0)) !== -1) {
                    str = str.substring(0, i) + newToken + str.substring(i + token.length);
                }
            } else {
                return this.split(token).join(newToken);
            }
        }
        return (str);
    };
    function load_texts() {
        $.ajax({
            type: "GET",
            url:  "LoadTextHandler.ashx",
            success: function (responce) {
                if (responce.text) {
                    $('#text').text(responce.text);
                    textOnChange();
                }
            }
        });
    };
});

$(document).ready(function () {
    var MAX_INPUTTEXT_LENGTH  = 100000,
        LOCALSTORAGE_TEXT_KEY = 'ip-text',
        DEFAULT_TEXT          = '392.0.2.1, Е192.0.2.1\n' +
'e.g., 172.16.254.1. Each part represent\n' +
'e.g., 172.16.254.12. Each part represent\n' +
'e.g., 172.16.254.123. Each part represent\n' +
'\n' +
'IP-адрес 01.01.010.01 (айпи-адрес, сокращение от англ. Internet Protocol Address) — уникальный сетевой адрес узла в компьютерной сети, построенной по протоколу IP. В сети Интернет требуется глобальная уникальность адреса; в случае работы в локальной сети требуется уникальность адреса в пределах сети. В версии протокола IPv4 IP-адрес имеет длину 4 байта.\n' +
'\n' +
'В 4-й версии IP-адрес представляет собой 32-битовое число. Удобной формой записи IP-адреса (IPv4) является запись в виде четырёх десятичных чисел значением от 0 до 255, разделённых точками, например, 192.168.0.60\n' +
'\n' +
'Структура[править | править исходный текст]\n' +
'IP-адрес состоит из двух частей: номера сети и номера узла. В случае изолированной сети её адрес может быть выбран администратором из специально зарезервированных для таких сетей блоков адресов (10.0.0.0/8, 172.16.0.0/12 или 192.168.0.0/16). Если же сеть должна работать как составная часть Интернета, то адрес сети выдаётся провайдером либо региональным интернет-регистратором (Regional Internet Registry, RIR). Согласно данным на сайте IANA,[1] существует пять RIR: ARIN, обслуживающий Северную Америку, а также Багамы, Пуэрто-Рико и Ямайку; APNIC, обслуживающий страны Южной, Восточной и Юго-Восточной Азии, а также Австралии и Океании; AfriNIC, обслуживающий страны Африки; LACNIC, обслуживающий страны Южной Америки и бассейна Карибского моря; и RIPE NCC, обслуживающий Европу, Центральную Азию, Ближний Восток. Региональные регистраторы получают номера автономных систем и большие блоки адресов у IANA, а затем выдают номера автономных систем и блоки адресов меньшего размера локальным интернет-регистраторам (Local Internet Registries, LIR), обычно являющимся крупными провайдерами.\n' +
'\n' +
'Сравнение типов адресации[править | править исходный текст]\n' +
'Иногда встречается запись IP-адресов вида 192.168.5.0/24. Данный вид записи заменяет собой указание диапазона IP-адресов. Число после косой черты означает количество единичных разрядов в маске подсети. Для приведённого примера маска подсети будет иметь двоичный вид 11111111 11111111 11111111 00000000 или то же самое в десятичном виде: 255.255.255.0. 24 разряда IP-адреса отводятся под номер сети, а остальные 32-24=8 разрядов полного адреса — под адреса хостов этой сети, адрес этой сети и широковещательный адрес этой сети. Итого, 192.168.5.0/24 означает диапазон адресов хостов от 192.168.5.1 до 192.168.5.254, а также 192.168.5.0 — адрес сети и 192.168.5.255 — широковещательный адрес сети. Для вычисления адреса сети и широковещательного адреса сети используются формулы:\n' +
'\n' +
'адрес сети = IP.любого_компьютера_этой_сети AND MASK (адрес сети позволяет определить, что компьютеры в одной сети)\n' +
'\n' +
'В протоколе IP существует несколько соглашений об особой интерпретации IP-адресов: если все двоичные разряды IP-адреса равны 1, то пакет с таким адресом назначения должен рассылаться всем узлам, находящимся в той же сети, что и источник этого пакета. Такая рассылка называется ограниченным широковещательным сообщением (limited broadcast). Если в поле номера узла назначения стоят только единицы, то пакет, имеющий такой адрес, рассылается всем узлам сети с заданным номером сети. Например, в сети 192.168.5.0 с маской 255.255.255.0 пакет с адресом 192.168.5.255 доставляется всем узлам этой сети. Такая рассылка называется широковещательным сообщением (direct broadcast).\n' +
'\n' +
'Адреса, используемые в локальных сетях, относят к частным. К частным относятся IP-адреса из следующих сетей:\n' +
'\n' +
'10.0.0.0/8\n' +
'172.16.0.0/12\n' +
'192.168.0.0/16\n' +
'Также для внутреннего использования:\n' +
'\n' +
'127.0.0.0/8\n' +
'169.254.0.0/16 — используется для автоматической настройки сетевого интерфейса в случае отсутствия DHCP.\n' +
'Полный список описания сетей для IPv4 представлен в RFC 3330 (заменён RFC 5735).\n' +
'\n' +
'Historical classful network architecture\n' +
'Class    Leading\n' +
'bits    Size of network\n' +
'number bit field    Size of rest\n' +
'bit field    Number\n' +
'of networks    Addresses\n' +
'per network    Start address    End address\n' +
'A    0    8    24    128 (27)    16,777,216 (224)    0.0.0.0    127.255.255.255\n' +
'B    10    16    16    16,384 (214)    65,536 (216)    128.0.0.0    191.255.255.255\n' +
'C    110    24    8    2,097,152 (221)    256 (28)    192.0.0.0    223.255.255.255\n' +
'Classful network design served its purpose in the startup stage of the Internet, but it lacked scalability in the face of the rapid expansion of the network in the 1990s. The class system of the address space was replaced with Classless Inter-Domain Routing (CIDR) in 1993. CIDR is based on variable-length subnet masking (VLSM) to allow allocation and routing based on arbitrary-length prefixes.\n' +
'\n' +
'IANA-reserved private IPv4 network ranges\n' +
'Start    End    No. of addresses\n' +
'24-bit block (/8 prefix, 1 × A)    10.0.0.0    10.255.255.255    16777216\n' +
'20-bit block (/12 prefix, 16 × B)    172.16.0.0    172.31.255.255    1048576\n' +
'16-bit block (/16 prefix, 256 × C)    192.168.0.0    192.168.255.255    65536\n' +
'Any user may use any of the reserved blocks. Typically, a network administrator will divide a block into subnets; for example, many home routers automatically use a default address range of 192.168.0.0 through 192.168.0.255 (192.168.0.0/24).\n' +
'\n' +
'IPv4 address exhaustion\n' +
'IPv4 address exhaustion is the decreasing supply of unallocated Internet Protocol Version 4 (IPv4) addresses available at the Internet Assigned Numbers Authority (IANA) and the regional Internet registries (RIRs) for assignment to end users and local Internet registries, such as Internet service providers. IANA\'s primary address pool was exhausted on 3 February 2011, when the last 5 blocks were allocated to the 5 RIRs.[5][6] APNIC was the first RIR to exhaust its regional pool on 15 April 2011, except for a small amount of address space reserved for the transition to IPv6, intended to be allocated in a restricted process.[7]\n' +
'\n' +
'The term subnet mask is only used within IPv4. Both IP versions however use the CIDR concept and notation. In this, the IP address is followed by a slash and the number (in decimal) of bits used for the network part, also called the routing prefix. For example, an IPv4 address and its subnet mask may be 192.0.2.1 and 255.255.255.0, respectively. The CIDR notation for the same IP address and subnet is 192.0.2.1/24, because the first 24 bits of the IP address indicate the network and subnet.\n' +
'\n' +
'Address autoconfiguration\n' +
'RFC 3330 defines an address block, 169.254.0.0/16, for the special use in link-local addressing for IPv4 networks. In IPv6, every interface, whether using static or dynamic address assignments, also receives a local-link address automatically in the block fe80::/10.\n' +
'\n' +
'Broadcast: In IPv4 it is possible to send data to all possible destinations ("all-hosts broadcast"), which permits the sender to send the data only once, and all receivers receive a copy of it. In the IPv4 protocol, the address 255.255.255.255 is used for local broadcast. In addition, a directed (limited) broadcast can be made by combining the network prefix with a host suffix composed entirely of binary 1s. For example, the destination address used for a directed broadcast to devices on the 192.0.2.0/24 network is 192.0.2.255. IPv6 does not implement broadcast addressing and replaces it with multicast to the specially-defined all-nodes multicast address.\n' +
'Multicast: A multicast address is associated with a group of interested receivers. In IPv4, addresses 224.0.0.0 through 239.255.255.255 (the former Class D addresses) are designated as multicast addresses.[11] IPv6 uses the address block with the prefix ff00::/8 for multicast applications. In either case, the sender sends a single datagram from its unicast address to the multicast group address and the intermediary routers take care of making copies and sending them to all receivers that have joined the corresponding multicast group.';

    var textOnChange = function () {
        var _len = $("#text").val().length; 
        var len = _len.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        var $textLength = $("#textLength");
        $textLength.html("length of text: " + len + " characters");
        if (MAX_INPUTTEXT_LENGTH < _len) $textLength.addClass("max-inputtext-length");
        else                             $textLength.removeClass("max-inputtext-length");
    };
    var getText = function ($text) {
        var text = trim_text($text.val().toString());
        if (is_text_empty(text)) {
            alert("Enter the text for processing.");
            $text.focus();
            return (null);
        }

        if (text.length > MAX_INPUTTEXT_LENGTH) {
            if (!confirm('Превышен рекомендуемый лимит ' + MAX_INPUTTEXT_LENGTH + ' символов (на ' + (text.length - MAX_INPUTTEXT_LENGTH) + ' символов).\r\nТекст будет обрезан, продолжить?')) {
                return (null);
            }
            text = text.substr(0, MAX_INPUTTEXT_LENGTH);
            $text.val(text);
            $text.change();
        }
        return (text);
    };

    $("#text").focus(textOnChange).change(textOnChange).keydown(textOnChange).keyup(textOnChange).select(textOnChange).focus();

    (function () {
        function isGooglebot() {
            return (navigator.userAgent.toLowerCase().indexOf('googlebot/') != -1);
        };
        if (isGooglebot())
            return;

        var text = localStorage.getItem(LOCALSTORAGE_TEXT_KEY);
        if (!text || !text.length) {
            text = DEFAULT_TEXT;
        }
        $('#text').text(text).focus();
    })();

    $('#mainPageContent').on('click', '#processButton', function () {
        if($(this).hasClass('disabled')) return (false);

        var text = getText( $("#text") );
        if (!text) return (false);

        processing_start();
        if (text != DEFAULT_TEXT) {
            localStorage.setItem(LOCALSTORAGE_TEXT_KEY, text);
        } else {
            localStorage.removeItem(LOCALSTORAGE_TEXT_KEY);
        }

        $.ajax({
            type: "POST",
            url:  "RESTProcessHandler.ashx",
            data: {
                text: text
            },
            success: function (responce) {
                if (responce.err) {
                    processing_end();
                    $('.result-info').addClass('error').text(responce.err);
                } else {
                    $('.result-info').removeClass('error').text('');

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
                        $('.result-info').hide();
                        $('#processResult').show().html( html );
                        $('#resultCount').text('found IP (v4) addresses: ' + responce.ips.length);
                    } else {
                        processing_end();
                        $('#processResult').show().html('<div style="text-align: center; padding: 15px;"><b>IP(v4) адресов</b> в тексте не найденно</div>');
                    }

                    //---$('#text').html( text );
                }
            },
            error: function () {
                processing_end();
                $('.result-info').addClass('error').text('server error');
            }
        });
        
    });

    function processing_start(){
        $('#text').addClass('no-change').attr('readonly', 'readonly').attr('disabled', 'disabled');
        $('.result-info').show().removeClass('error').html('<div style="text-align: center">Идет обработка...</div>');
        $('#processButton').addClass('disabled');
        $('#processResult, #resultCount').empty();
        $('#processResult').hide();
    };
    function processing_end(){
        $('#text').removeClass('no-change').removeAttr('readonly').removeAttr('disabled');
        $('.result-info').removeClass('error').text('');
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
});
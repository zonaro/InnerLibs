// --------------------------------------------------------
//Controlador de versÃ£o
//---------------------------------------------------------
var versao = new function () {

    this.numero = " 0.2.8";

    this.log = function () {
        console.log("InnerLibs versÃ£o" + this.numero + " - BEOWULF.");
    };

    this.info = function () {

        console.log("BEOWULF \n" +
			"Mitologia: Anglo-Saxon Mythology \n" +
			"Possibly means 'bee wolf' (in effect equal to 'bear') from Old English beo 'bee' and wulf 'wolf'. This is the name of the main character in the anonymous 8th-century epic poem 'Beowulf'. The poem tells how Beowulf slays the monster Grendel and its mother, but goes on to tell how he is killed in his old age fighting a dragon.");

    };

    return this;
};

//---------------------------------------------------------
//---------------------------------------------------------


// --------------------------------------------------------

/*
@name document.loaded @
@param loadedFunc @
@description MÃ©todo para executar uma funÃ§Ã£o quando o documento for carregado.
Este mÃ©todo extende o objeto global "document". @
@example N/A @
 */
document.loaded = function (loadedFunc) {

    document.addEventListener("DOMContentLoaded", loadedFunc);

};

var inner = {};

//@name inner.romanize() @
//@param N/A @
//@description Converte o valor especificado para nÃºmero romano. @
//@example N/A @
inner.romanize = function (string) {

    num = parseInt(string) || parseInt(string);

    if (!+num)
        return false;
    var digits = String(+num).split(""),
    key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
        "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
        "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"],
    roman = "",
    i = 3;
    while (i--)
        roman = (key[+digits.pop() + (i * 10)] || "") + roman;
    return Array(+digits.join("") + 1).join("M") + roman;

};


//@name inner.htmlencode() @
//@param N/A @
//@description MÃ©todo para codificar carÃ¡cteres HTML para suas respectivas entidades. @
//@example N/A @
inner.htmlencode = function (string) {

    return string.replace(/</g, '&lt;').replace(/>/g, '&gt;');

};


//@name inner.disableScriptsCache() @
//@param N/A @
//@description MÃ©todo para poder "desabilitar" o cachÃª que os scripts deixam, carregando-os dinÃ¢micamente. @
//@example N/A @

inner.disableScriptsCache = function () {

    var scripts = document.getElementsByTagName("script");
    var leng = scripts.length;
    //var modif = 0;
    for (i = 0; i < leng; i++) {

        if (scripts[0].innerHTML == "") {
            //modif = modif + 1;
            //console.log(modif);
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.async = true;
            script.src = scripts[0].src + "?v=" + Math.floor(Math.random() * 100000000000) + 1;
            var head = document.getElementsByTagName('head')[0];
            head.appendChild(script);
            scripts[0].parentNode.removeChild(scripts[0]);

        } else { }

    }

};

//@name inner.handleImgErrors() @
//@param imageURI @
//@description MÃ©todo para poder substituir todas as tags <img> com erro pela imagem especÃ­ficada pelo parÃ¢metro. @
//@example N/A @

inner.handleImgErrors = function (imageURI) {

    images = document.getElementsByTagName("img");
    for (i = 0; i < images.length; i++) {

        images[i].onerror = "";
        images[i].src = imageURI;
        return true;

    }

};

//@name inner.directObject() @
//@param N/A @
//@description MÃ©todo para poder transformar TODOS os elementos HTML com um respectivo ID automÃ¡ticamente em um objeto manipulÃ¡vel. @
//@example N/A @

inner.directObject = function () {

    var items = document.body.getElementsByTagName("*");

    for (var i = 0, len = items.length; i < len; i++) {
        objid = items[i].id;
        if (objid.indexOf("-") > -1) {
            index = objid.indexOf("-");
            objid = objid.replace("-", "");

        }
        window[objid] = items[i];

    }

};

//@name inner.ajax() @
//@param method, file, func, dataViaPost @
//@description MÃ©todo criado para realizar aquisiÃ§Ãµes AJAX de uma maneira mais simples e que seja crossbrowser. Com ele vocÃª pode fazer tanto aquisiÃ§Ãµes GET como POST. @
//@example N/A @

inner.ajax = function (method, file, func, dataViaPost) {

    method = method.toUpperCase();
    var xmlhttp;
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    } else { // code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            func(xmlhttp.responseText);

        }
    };
    xmlhttp.open(method, file, true);

    if (method == "POST") {

        xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        xmlhttp.send(dataViaPost);

    } else {

        xmlhttp.send();

    }

    return this;

};

//@name inner.notify() @
//@param title, msg, URL, icon @
//@description Cria uma janela de notificaÃ§Ã£o padrÃ£o do navegador. @
//@example N/A @

inner.notify = function (title, msg, URL, icon) {
    if (typeof nURL === 'undefined') {
        URL = '';
    }
    Notification.requestPermission(function () {
        var notification = new Notification(title, {
            icon: icon,
            body: msg
        });
        if (nURL.length) {
            notification.onclick = function () {
                window.open(nURL);
            };
        }

    });
};

//@name inner.getDomain() @
//@param url @
//@description MÃ©todo utilizado para poder obter o nome de um domÃ­nio a partir de uma URL passada como parÃ¢metro. @
//@example N/A @

inner.getDomain = function (url) {
    var a,
	domain;

    a = document.createElement("a");
    a.href = url;

    domain = a.hostname;
    domain = domain.replace(/^www\./, "");

    return domain;
};

//@name inner.removeAccents() @
//@param string @
//@description MÃ©todo que automÃ¡ticamente retira todos os acentos da string especificada e a retorna. @
//@example N/A @

inner.removeAccents = function (string) {
    if (typeof string === 'undefined') {
        string = '';
        console.log('Erro, string em branco');
    }
    var mapchar = {
        a: /[\xE0-\xE6]/g,
        A: /[\xC0-\xC6]/g,
        e: /[\xE8-\xEB]/g,
        E: /[\xC8-\xCB]/g,
        i: /[\xEC-\xEF]/g,
        I: /[\xCC-\xCF]/g,
        o: /[\xF2-\xF6]/g,
        O: /[\xD2-\xD6]/g,
        u: /[\xF9-\xFC]/g,
        U: /[\xD9-\xDC]/g,
        c: /\xE7/g,
        C: /\xC7/g,
        n: /\xF1/g,
        N: /\xD1/g,
    };

    for (var letter in mapchar) {
        var expression = mapchar[letter];
        string = string.replace(expression, letter);
    }

    return string;
};


//@name inner.returnBase64() @
//@param confs [objeto] @
//@description Retorna o cÃ³digo base64 de acordo com os parÃ¢metros passados, se for string, retorna a string codificada, se for imagem, retorna o cÃ³digo base64 da imagem. @
//@example N/A @

inner.returnBase64 = function (confs) {
    if (confs.type == "image") {
        var img = new Image();
        img.crossOrigin = 'Anonymous';
        img.onload = function () {
            var canvas = document.createElement('CANVAS');
            var ctx = canvas.getContext('2d');

            canvas.height = img.height;
            canvas.width = img.width;
            ctx.drawImage(img, 0, 0);
            imgCode = canvas.toDataURL(confs.outputFormat);
            imgCode = imgCode.split(/data:(.*?),/);
            return imgCode[2];
        };

        img.src = confs.img;
        return img.onload();
    } else if (confs.type == "string") {

        return window.btoa(confs.contents);

    } else if (confs.type == "canvas") {

        return document.querySelector(confs.contents).toDataURL(confs.outputFormat);

    }

}

function log(str) { console.log(str); }

//Classe Cookies
inner.cookies = {};

//@name inner.cookies.createCookie() @
//@param name [String], value [String], days [Number] @
//@description Cria um cookie com os parÃ¢metros passados. @
//@example N/A @

inner.cookies.createCookie = function (cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
    log(document.cookie);
    log(expires);

};

//@name inner.cookies.getCookie() @
//@param name [String] @
//@description LÃª um cookie com os parÃ¢metros passados. @
//@example var usuario = inner.cookies.getCookie('loginUsuario'); @
inner.cookies.getCookie = function (cookieName) {
    try {
        var cookieReturn;
        var cookies = document.cookie;
        var cookieId = cookies.split(";");
        for (i = 0; i < cookieId.length; i++) {
            cookieSplit = cookieId[i].split("=");
            if (cookieSplit[0].split(" ").join("") == cookieName) {
                cookieReturn = cookieSplit[1];
            }

        }
        return cookieReturn;

    } catch (e) {
        return e;
    }

}; // final getCookie

//@name inner.cookies.deleteCookie() @
//@param cookieId [String] @
//@description Deleta o cookie com o nome passado. @
//@example N/A @

inner.cookies.deleteCookie = function (cookieId) {

    document.cookie = cookieId + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC";
};

//@name inner.require() @
//@param FileName [String] @
//@description Faz uma requisiÃ§Ã£o a um arquivo e retorna a resposta. @
//@example var noticias = inner.require('noticias.php'); @
inner.require = function (FileName) {
    inner.ajax("get", FileName, function (response) { return response; });
};

// --------------------------------------------------------


inner.clipboard = {}

inner.clipboard.text = "";

inner.clipboard.copy = function (stringToCopy) {
    try {
        var txtarea = document.createElement("TEXTAREA");
        txtarea.value = stringToCopy;
        txtarea.style.zIndex = "-9999999999999999999999";
        txtarea.style.position = "absolute";
        txtarea.style.visibility = "hidden";
        txtarea.style.marginLeft = "10000000000000000000000000000000000px";
        document.body.appendChild(txtarea);
        txtarea.select();
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Copying text command was ' + msg);
        document.body.removeChild(txtarea);
    } catch (err) {
        console.log('Oops, unable to copy');
    }
}

inner.clipboard.cut = function () {
    try {
        var successful = document.execCommand('cut');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('cutting text command was ' + msg);
        document.body.removeChild(txtarea); s
    } catch (err) {
        console.log('Oops, unable to cut');
    }
}

inner.clipboard.addOnCopy = function () {
    var textToInsert = inner.clipboard.text,
        copytext = window.getSelection() + textToInsert;

    if (window.clipboardData) {
        window.clipboardData.setData('Text', copytext);
    }
}

//@name inner.decodeNumericEntities() @
//@param str [String] @
//@description Decodifica uma string que contÃ©m carÃ¡cteres de acentos codificados como exemplo: &#226; | &#194; @
//@example var titulo = inner.decodeNumericEntities("Sou um enc&#244;modo."); //titulo = Sou um encÃ´modo. @
inner.decodeNumericEntities = function (str) {
    return str.replace(/&#(\d+);/g, function (match, dec) {
        return String.fromCharCode(dec);
    });
};


//@name inner.encodeNumericEntities() @
//@param str [String] @
//@description Codifica uma string que contÃ©m carÃ¡cteres de acentos codificados como exemplo: Ã¢ | Ã´ @
//@example var titulo = inner.decodeNumericEntities("Sou um encÃ´modo."); //titulo = Sou um enc&#244;modo. @
inner.encodeNumericEntities = function (str) {
    var buf = [];
    for (var i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }
    return buf.join('');
};

//@name inner.TextToDOM() @
//@param string [String] @
//@description Transforma um texto direto em DOM, podendo ser manipulÃ¡vel antes de ser inserido no documento. @
//@example var documento = inner.TextToDOM('<div id="container"><h1 class="titulo">Titulo</h1><p class="descricao">DescriÃ§Ã£o prÃ©via</p></div>'); documento.getElementById("container").innerHTML = "Troca de conteÃºdo!"; @
inner.TextToDOM = function (string) {
    var el = document.createElement('html');
    el.innerHTML = string;
    return el;
};

inner.verticalAlign = function (SelEl) {

    var e = window.innerHeight,
    i = e;
    var n = document.querySelectorAll(SelEl)[0].offsetHeight;
    console.log(n);
    var o = n;
    console.log(o);
    var r = Math.ceil(o / 2);
    console.log(r);
    var a = Math.ceil(i / 2),
    d = Math.ceil(a) - Math.ceil(r);
    console.log(d),
    jQuery(SelEl).css("padding-top", d + "px");

}


/* EM CONSTRUÃ‡ÃƒO */
/* 
inner.validate = function(campo, tipo) {
		
}

	inner.returnPixelsToCenter = function(elementTarget) {
	
	var wheight = window.innerHeight;
	var height = wheight - 100;
	
	var target = document.querySelectorAll(elementTarget)[0].offsetHeight;
	
	var subtrair = Math.ceil(target / 2);
	
	var subtrairTotalHeight = Math.ceil(height / 2);
	
	var res = Math.ceil(subtrairTotalHeight) - Math.ceil(subtrair);
	
	
	return res;
	
	};


	function isScrolledIntoView(elem)
	{
    var $elem = $(elem);
    var $window = $(window);
	
    var docViewTop = $window.scrollTop();
    var docViewBottom = docViewTop + $window.height();
	
    var elemTop = $elem.offset().top;
    var elemBottom = elemTop + $elem.height();
	
    return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
	}
 */

/* X EM CONSTRUÃ‡ÃƒO */
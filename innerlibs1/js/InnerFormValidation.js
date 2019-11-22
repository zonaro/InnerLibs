
const _phoneMask = (input) => {
    let text = input.value
    text = text.replace(/\D/g, "");
    text = text.replace(/^(\d{1,2})/g, "($1");
    text = text.replace(/^(\(\d{2})(\d+)/g, "$1) $2");
    text = text.replace(/^(\(\d{2}\) \d{4})(\d{1,4})$/g, "$1-$2");
    text = text.replace(/^(\(\d{2}\) \d{5})(\d{4})$/g, "$1-$2");
    if (/\([\d]{2}\) [\d]{5}-[\d]{4}$/g.test(text)) {
        input.maxLength = text.length;
    }
    input.value = text;
};

const _dateMask = (input) => {
    let text = input.value;
    text = text.replace(/\D/g, "");
    text = text.replace(/^(\d{2})(\d+)/g, "$1/$2");
    text = text.replace(/^(\d{2}\/\d{2})(\d{1,4})$/g, "$1/$2");
    if (/^[\d]{2}\/[\d]{2}\/[\d]{4}$/g.test(text)) {
        input.maxLength = text.length;
    }
    input.value = text;
};

const _cpfMask = (input) => {
    let text = input.value;
    text = text.replace(/\D/g, "");
    text = text.replace(/^(\d{3})(\d+)/g, "$1.$2");
    text = text.replace(/^(\d{3}\.\d{3})(\d+)/g, "$1.$2");
    text = text.replace(/^(\d{3}\.\d{3}\.\d{3})(\d{1,2})$/g, "$1-$2");
    if (/^[\d]{3}\.[\d]{3}\.[\d]{3}-[\d]{2}$/g.test(text)) {
        input.maxLength = text.length;
    }
    input.value = text;
};

const _cepMask = (input) => {
    let text = input.value
    text = text.replace(/\D/g, "");
    text = text.replace(/^(\d{5})(\d{1,3})$/g, "$1-$2");
    if (/^[\d]{5}-[\d]{3}$/g.test(text)) {
        input.maxLength = text.length;
    }
    input.value = text;
};

const _cnpjMask = (input) => {
    let text = input.value;
    text = text.replace(/\D/g, "");
    text = text.replace(/^(\d{2})(\d+)/, "$1.$2");
    text = text.replace(/^(\d{2}\.\d{3})(\d+)/g, "$1.$2");
    text = text.replace(/^(\d{2}\.\d{3}\.\d{3})(\d+)/g, "$1/$2");
    text = text.replace(/^(\d{2}\.\d{3}\.\d{3}\/\d{4})(\d{1,2})$/g, "$1-$2");
    if (/^[\d]{2}\.[\d]{3}\.[\d]{3}\/[\d]{4}-[\d]{2}$/g.test(text)) {
        input.maxLength = text.length;
    }
    input.value = text;
};

const _onlyNumbers = (input) => {
    let text = input.value;
    text = text.replace(/\D/g, "");
    input.value = text;
}; 

jQuery.fn.isValid = function () {
    var results = [];

    if (jQuery(this).length > 1 || jQuery(this).prop('tagName') == 'FORM') {
        var elements = [];
        var config = Array.prototype.slice.call(arguments)[0];
        jQuery(this).find(":input" + (config || "")).each(function () {
            results.push(jQuery(this).isValid());
            elements.push(jQuery(this));
        });

        for (var i = 0; i < results.length; i++) {
            if (results[i] === false) {
                return false;
            }
        }
        return true;
    } else {
        var valids = Array.prototype.slice.call(arguments);
        var value = this.val();
        var type = this.attr("type");
        if (arguments.length < 1) {
            var classes = (this.attr('class') || 'v_noclass').split(" ");
            for (var i = 0; i < classes.length; i++) {
                valids.push("" + classes[i]);
            }
        }
        for (var i = 0; i < valids.length; i++) {
            if (jQuery(this).prop("disabled") == false) {
                switch (valids[i].toLowerCase()) {
                    case "number":
                    case "num":
                        if (jQuery.trim(value) === "") {
                            results.push(true);
                            break;
                        }
                        results.push(!isNaN(value.replace(',', '.')));
                        break;
                    case "mail":
                    case "email":
                    case "e-mail":
                        if (jQuery.trim(value) === "") {
                            results.push(true);
                            break;
                        }
                        var re = /^[\w-]+(\.[\w-]+)*@[\w]+(\.[a-z]{2,6})*(\.[a-z]{2,6})$/gi;
                        results.push(re.test(value));
                        break;
                    case "required":
                    case "req":
                    case "obg":
                        if (type == 'checkbox' || type == 'radio') {
                            results.push(jQuery(this).is(':checked'));
                        } else {
                            results.push(!(!value || jQuery.trim(value) === ""));
                        }
                        break;
                    case "url":
                    case "link":
                        if (jQuery.trim(value) === "") {
                            results.push(true);
                            break;
                        }
                        var re = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;
                        results.push(re.test(value));
                        break;
                    case "data":
                    case "date":
                        if (jQuery.trim(value) === "") {
                            results.push(true);
                            break;
                        }
                        if (type == 'text') {
                            var comp = value.split(' ')[0].split('/');
                            if (comp.length == 3) {
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                var date = new Date(y, m, d);
                                results.push(date.getFullYear() == y && date.getMonth() == m && date.getDate() == d);
                            } else {
                                results.push(false);
                            }
                        } else {
                            results.push(true);
                        }
                        break;
                    default:
                        var c = valids[i];
                        if (c.startsWith("eq:") || c.startsWith("equal:")) {
                            var selector = c.split(':')[1] || jQuery(this).data("eq");
                            var valor1 = jQuery(this).val();
                            var valor2 = jQuery(selector).val() || jQuery(selector).text();
                            results.push(valor1 == valor2);
                        } else {
                            results.push(true);
                        }
                        if (~c.indexOf(" or ")) {
                            var any = false;
                            var allchecks = c.split(" or ");
                            for (var i = 0; i < allchecks.length; i++) {
                                allchecks[i] = allchecks[i].split(" ").join("");
                                any = jQuery(this).isValid(allchecks[i]);
                                if (any == true) { break; }
                            }
                            results.push(any);
                        } else {
                            results.push(true);
                        }
                        if (~c.indexOf(" to ")) {
                            var allnums = c.split(" to ");
                            if (allnums[0] > allnums[1]) {
                                results.push(jQuery(this).isValid("after " + allnums[1]) && jQuery(this).isValid("before " + allnums[0]));
                            } else {
                                results.push(jQuery(this).isValid("after " + allnums[0]) && jQuery(this).isValid("before " + allnums[1]));
                            }
                        } else {
                            results.push(true);
                        }

                        if (c.startsWith("after")) {
                            var mynumber = jQuery(this).val();
                            if (jQuery.trim(mynumber) === "") {
                                results.push(true);
                                break;
                            }
                            var num = c.split("after").join("");
                            if ((num.indexOf("today") || num.indexOf("/")) && jQuery(this).isValid("date")) {
                                var comp = mynumber.split('/');
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                mynumber = +new Date(y, m, d);
                                if (num == 'today') {
                                    num = today;
                                } else {
                                    comp = num.split('/');
                                    d = parseInt(comp[0], 10);
                                    m = parseInt(comp[1], 10) - 1;
                                    y = parseInt(comp[2], 10);
                                    num = +new Date(y, m, d);
                                }
                            }

                            results.push(parseFloat(mynumber) >= parseFloat(num));
                        } else {
                            results.push(true);
                        }

                        if (c.startsWith("before")) {
                            var mynumber = jQuery(this).val();
                            if (jQuery.trim(mynumber) === "") {
                                results.push(true);
                                break;
                            }
                            var num = c.split("before").join("");
                            if ((num.indexOf("today") || num.indexOf("/")) && jQuery(this).isValid("date")) {
                                var comp = mynumber.split('/');
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                mynumber = +new Date(y, m, d);
                                if (num == 'today') {
                                    num = today;
                                } else {
                                    comp = num.split('/');
                                    d = parseInt(comp[0], 10);
                                    m = parseInt(comp[1], 10) - 1;
                                    y = parseInt(comp[2], 10);
                                    num = +new Date(y, m, d);
                                }
                            }

                            results.push(parseFloat(mynumber) <= parseFloat(num));
                        } else {
                            results.push(true);
                        }
                        break;
                }
            } else {
                results.push(true);
            }
        }

        for (var i = 0; i < results.length; i++) {
            if (results[i] === false) {
                jQuery(this).addClass('error');
                jQuery(this).closest('.form-group').addClass('has-error');
                eval(jQuery(this).attr('data-invalidcallback') || "void(0)");
                return false;
            }
        }
        jQuery(this).removeClass('error');
        jQuery(this).closest('.form-group').removeClass('has-error');
        return true;
    }
};


jQuery(document).ready(function () {
    jQuery('form.validate, form[data-validate="true"], form[data-validation="true"]').on('submit', function () {
        return jQuery(this).isValid();
    });

    jQuery(".mask.phone, .mask.tel, [type='tel'].mask").on('input', function () {
        _phoneMask(this);
    });

    jQuery(".mask.cpf").on('input', function () {
        _cpfMask(this);
    });

    jQuery(".mask.cep").on('input', function () {
        _cpfMask(this);
    });

    jQuery(".mask.cnpj").on('input', function () {
        _cnpjMask(this);
    });

    jQuery(".mask.date, .mask.data").on('input', function () {
        _dateMask(this);
    });

    jQuery(".mask.num, .mask.number").on('input', function () {
        _onlyNumbers(this);
    });
});
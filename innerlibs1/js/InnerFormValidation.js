$.fn.isValid = function () {
    if ($(this).length > 1 || $(this).prop('tagName') == 'FORM') {
        var results = []
        var elements = []
        var config = Array.prototype.slice.call(arguments)[0];
        $(this).find(":input" + (config || "")).each(function () {
            results.push($(this).isValid())
            elements.push($(this))
        });

        for (var i = 0; i < results.length; i++) {
            if (results[i] === false) {
                return false
            }
        }
        return true
    } else {
        var valids = Array.prototype.slice.call(arguments);
        var results = []
        var value = this.val()
        var type = this.attr("type")
        if (arguments.length < 1) {
            var attr = this.attr('class');
            if (typeof attr == typeof undefined) {
                this.attr('class', 'v_noclass');
            }
            var classes = $(this).attr("class").split(" ")
            for (var i = 0; i < classes.length; i++) {
                valids.push("" + classes[i])
            }
        }
        for (var i = 0; i < valids.length; i++) {
            
            if ($(this).prop("disabled") == false) {
                switch (valids[i].toLowerCase()) {
                    case "number":
                    case "num":
                        if ($.trim(value) === "") {
                            results.push(true)
                            break;
                        }
                        results.push(!isNaN(value.replace(',', '.')));
                        break;
                    case "mail":
                    case "email":
                    case "e-mail":
                        if ($.trim(value) === "") {
                            results.push(true)
                            break;
                        }
                        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
                        results.push(re.test(value));
                        break;
                    case "required":
                    case "req":
                    case "obg":
                        if (type == 'checkbox' || type == 'radio') {
                            results.push($(this).is(':checked'))
                        } else {
                            results.push(!(!value || $.trim(value) === ""));
                        }
                        break;
                    case "url":
                    case "link":
                        if ($.trim(value) === "") {
                            results.push(true)
                            break;
                        }
                        var re = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;
                        results.push(re.test(value));
                        break;
                    case "data":
                    case "date":
                        if ($.trim(value) === "") {
                            results.push(true)
                            break;
                        }
                        if (type == 'text') {
                            var comp = value.split(' ')[0].split('/');
                            if (comp.length == 3) {
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                var date = new Date(y, m, d);
                                results.push(date.getFullYear() == y && date.getMonth() == m && date.getDate() == d)
                            } else {
                                results.push(false)
                            }
                        } else {
                            results.push(true)
                        }
                        break;
                    default:
                        var c = valids[i]
                        if (c.startsWith("eq:") || c.startsWith("equal:")) {
                            var selector = c.split(':')[1] || $(this).data("eq")
                            var valor1 = $(this).val()
                            var valor2 = $(selector).val() || $(selector).text()
                            results.push(valor1 == valor2)
                        } else {
                            results.push(true)
                        }
                        if (~c.indexOf(" or ")) {
                            var any = false
                            var allchecks = c.split(" or ")
                            for (var i = 0; i < allchecks.length; i++) {
                                allchecks[i] = allchecks[i].split(" ").join("")
                                any = $(this).isValid(allchecks[i])
                                if (any == true) { break; }
                            }
                            results.push(any)
                        } else {
                            results.push(true)
                        }
                        if (~c.indexOf(" to ")) {
                            var allnums = c.split(" to ")
                            if (allnums[0] > allnums[1]) {
                                results.push($(this).isValid("after " + allnums[1]) && $(this).isValid("before " + allnums[0]))
                            } else {
                                results.push($(this).isValid("after " + allnums[0]) && $(this).isValid("before " + allnums[1]))
                            }
                        } else {
                            results.push(true)
                        }

                        if (c.startsWith("after")) {
                            var mynumber = $(this).val()
                            if ($.trim(mynumber) === "") {
                                results.push(true)
                                break;
                            }
                            var num = c.split("after").join("")
                            if ((num.indexOf("today") || num.indexOf("/")) && $(this).isValid("date")) {
                                var comp = mynumber.split('/');
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                mynumber = +new Date(y, m, d)
                                if (num == 'today') {
                                    num = today
                                } else {
                                    comp = num.split('/');
                                    d = parseInt(comp[0], 10);
                                    m = parseInt(comp[1], 10) - 1;
                                    y = parseInt(comp[2], 10);
                                    num = +new Date(y, m, d)
                                }
                            }

                            results.push(parseFloat(mynumber) >= parseFloat(num))
                        } else {
                            results.push(true)
                        }

                        if (c.startsWith("before")) {
                            var mynumber = $(this).val()
                            if ($.trim(mynumber) === "") {
                                results.push(true)
                                break;
                            }
                            var num = c.split("before").join("")
                            if ((num.indexOf("today") || num.indexOf("/")) && $(this).isValid("date")) {
                                var comp = mynumber.split('/');
                                var d = parseInt(comp[0], 10);
                                var m = parseInt(comp[1], 10) - 1;
                                var y = parseInt(comp[2], 10);
                                mynumber = +new Date(y, m, d)
                                if (num == 'today') {
                                    num = today
                                } else {
                                    comp = num.split('/');
                                    d = parseInt(comp[0], 10);
                                    m = parseInt(comp[1], 10) - 1;
                                    y = parseInt(comp[2], 10);
                                    num = +new Date(y, m, d)
                                }
                            }

                            results.push(parseFloat(mynumber) <= parseFloat(num))
                        } else {
                            results.push(true)
                        }
                        break;
                }
            } else {
                results.push(true)
            }
            

        }

        for (var i = 0; i < results.length; i++) {
            if (results[i] === false) {
                $(this).addClass('error')
                $(this).closest('.form-group').addClass('has-error')
                return false
            }
        }
        $(this).removeClass('error')
        $(this).closest('.form-group').removeClass('has-error')
        return true;
    }
};
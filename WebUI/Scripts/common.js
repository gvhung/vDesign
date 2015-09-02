$.fn.pbaButton = function (clickHandler) {
    var btn = {
        click: function () {
            clickHandler();
        }
    };

    var isEnabled = true;
    var self = this;

    btn.enable = function (boolEnable) {
        isEnabled = boolEnable;

        if (boolEnable) {
            $(self).removeClass('k-state-disabled');
        } else {
            $(self).addClass('k-state-disabled');
        }
    }

    $(this).click(function () {
        if (isEnabled) {
            btn.click();
        }
    });

    $(this).data('pbaButton', btn);

    return btn;
};

$.fn.pbaToolbar = function (eventHandler) {
    $(this).on('click', function () {
        eventHandler($(this).attr("id"));
    });
}

$.fn.pbaSearchBox = function (eventHandler) {

    var self = this;
    var input = $(self).find('input');
    var width = input.width();
    var span = $(self).find('.cancel-search');
    var searchButton = $(self).find('span.icon-search');

    var searchBox = {
        search: function (str) {
            eventHandler({ sender: self, str: str });
        },
        clear: function () {
            input.val('');
            input.trigger('blur');
            eventHandler({ sender: self, str: '' });
        }
    };

    searchButton.bind('click', function () {
        //$(self).find('div.search-tools').fadeIn(500);
        //searchButton.fadeOut(100);
        input.focus();
    });

    span.bind('click', function () {
        searchBox.clear();
    });

    //var timeoutId = 0;
    //input.bind("keyup", function () {
    //    clearTimeout(timeoutId);
    //    var str = input.val();
    //    if (str) {
    //        timeoutId = setTimeout(function () {
    //            searchBox.search(input.val());
    //        }, 1500);
    //    } else {
    //        searchBox.search(str);
    //    }
    //});

    input.bind("keypress", function (e) {
        if (e.keyCode === 13 || e.which === 13) {
            searchBox.search(input.val());
        }
    });

    //input.bind("cut copy paste", function (event) {
    //    searchBox.search(input.val());
    //});

    input.bind('focus', function () {
        //input.animate({ width: width * 2 }, 400);

    });

    input.bind('blur', function () {
        if (!input.val()) {
            //$(self).find('div.search-tools').fadeOut(100);
            //searchButton.fadeIn(500);
            //input.animate({ width: width }, 300);
        } else {
            //input.trigger('focus');
        }
    })

    return searchBox;
};

$.fn.pbaExport = function (clickHandler) {
    var exporter = {
        click: function (type) {
            clickHandler(type);
        }
    };

    var self = this;

    var types = $(self).find('[data-type]');

    $.each(types, function (i, v) {
        $(v).bind('click', function () {
            exporter.click($(v).data('type'));
        })
    });

    return exporter;
};


$.fn.pbaForm = function (options) {
    var self = this;

    var settings = $.extend({
        buttons: [],
        footerBtnsClass: "footer-buttons"
    }, options);

    var form = {
        element: self,
        model: null,
        parentForm: null,
        nameModel: "",
        validator: null
    };

    if (settings.wrap) {
        if ($(this).find("." + settings.wrap).length == 0) {
            $(this).wrap("<div class='common-form " + settings.wrap + "'></div>");
        }
    }

    if (settings.model) {
        form.model = settings.model;
    }

    if (settings.nameModel) {
        form.nameModel = settings.nameModel + ".";
    }

    if (settings.attrBind) {
        $(self).find("[data-bind]").each(function () {
            if (form.nameModel != "") {
                var str = $(this).attr("data-bind").replace(" ", "");

                str = str.replace("alt:", "alt: " + form.nameModel);
                str = str.replace("src:", "src: " + form.nameModel);
                str = str.replace("checked:", "checked: " + form.nameModel);
                str = str.replace("href:", "href: " + form.nameModel);
                str = str.replace("html:", "html: " + form.nameModel);
                str = str.replace("source:", "source: " + form.nameModel);
                str = str.replace("text:", "text: " + form.nameModel);
                str = str.replace("value:", "value: " + form.nameModel);

                $(this).attr("data-bind", str);
            }
        });
    }

    if (settings.buttons && settings.buttons.length > 0) {
        var buttonsFooter = false;

        $.each(settings.buttons, function (key, value) {
            if (!buttonsFooter) {
                buttonsFooter = $("<div class='" + settings.footerBtnsClass + "'></div>").appendTo($(self));
            }

            $("<a class='btn " + value.cssClass + "' data-bind='click:" + value.click + "' href='#'><span class='k-icon " + value.icon + "'></span>" + value.title + "</a>").appendTo(buttonsFooter);
        });
    }


    //-----------Validator----------//
    if (settings.validate) {
        form.validator = $(self).data("kendoValidator");

        if (!form.validator) {
            $(this).kendoValidator();
            form.validator = $(self).data("kendoValidator");
        }
    }

    form.validate = function () {
        if (form.validator != null)
            return form.validator.validate();
        else
            false;
    }

    //-----------Bind----------//
    form.bind = function (model) {
        if (this.validator != null) {
            this.validator.hideMessages();
        }

        if (model) {
            this.model = model;
        }

        $(self).trigger("onBeforeBind", this);

        kendo.bind(self, this.model);

        $(self).trigger("onAfterBind", this);

        form.model.bind("change", function (e) {
            $(self).trigger("onChange", {
                sender: form,
                field: e.field.replace(form.nameModel, ""),
            });
        });

        return this;
    }

    form.unbind = function () {
        kendo.unbind(self);
        this.model = null;
    }   

    //-----------Model----------//
    form.getModel = function () {
        return this.model.get(this.nameModel);
    }

    form.setModel = function (model) {
        return this.model.set(this.nameModel, model);
    }

    form.getPr = function (pr) {
        return this.model.get(this.nameModel + pr);
    }

    form.setPr = function (pr, val) {
        this.model.set(this.nameModel + pr, val);
    }

    $(this).data('pbaForm', form);

    $(self).keydown(function (e) {
        if (e.which == 13 && e.target.nodeName.toLowerCase() != "textarea") {

            e.preventDefault();

            var wnd = $(self).closest(".k-window");

            if (wnd.length > 0) {
                if (settings.buttons) {

                } else {

                }
            }
        }
    });

    //events
    form.onResize = function (wnd) {
        $(self).trigger("onResize", {
            sender: form,
            wnd: wnd,
        });
    };

    form.onTabShown = function (tabID) {
        $(self).trigger("onTabShown", {
            sender: form,
            tabID: tabID,
        });
    };

    return form;
};

$.fn.pbaActionBar = function () {
    var self = this;
    var $self = $(this);

    var actionBar = {
        toolbarID: null,
        listViewID: null,
    };

    actionBar.toolbarID = $self.closest(".w-custom-toolbar").attr("data-toolbarID");
    actionBar.listViewID = $("#" + actionBar.toolbarID).closest("#list-view").find("[data-role=grid]").attr("id");

    $self.data('pbaActionBar', actionBar);

    return actionBar;
};

$.fn.extend({
    insertAtCaret: function (myValue) {
        var elem = this[0];
        if (document.selection) {
            elem.focus();
            sel = document.selection.createRange();
            sel.text = myValue;
            elem.focus();
        } else if (elem.selectionStart || elem.selectionStart == '0') {
            var startPos = elem.selectionStart;
            var endPos = elem.selectionEnd;
            var scrollTop = elem.scrollTop;
            elem.value = elem.value.substring(0, startPos) + myValue + elem.value.substring(endPos, elem.value.length);
            elem.focus();
            elem.selectionStart = startPos + myValue.length;
            elem.selectionEnd = startPos + myValue.length;
            elem.scrollTop = scrollTop;
        } else {
            elem.value += myValue;
            elem.focus();
        }
    }
});
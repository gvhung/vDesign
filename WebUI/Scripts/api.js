var pbaAPI = {
    console: function (args) {
        var t = arguments;
        try {
            window.console && console.log(kendo.format.apply(this, t));
        } catch (e) {
            window.console && console.log(kendo.format("Some error was detected: {0}", e));
        }
    },
    objectstatus: {
        nochanges: 0,
        created: 1,
        modified: 2,
        deleted: 3
    },
    msg: function (msg, type, autoHideAfter) {
        var $el = $("#notification__");

        if ($el) {
            $("body").append("<div id='notification__'>");

            $el = $("#notification__");
        }

        var notification = $el.data("kendoNotification");

        if (!notification) {
            notification = $el.kendoNotification({
                position: {
                    pinned: true,
                    top: 5,
                    right: 5
                },
                show: function (e) {
                    e.element.closest(".k-animation-container").css("z-index", "999999");
                },
                autoHideAfter: autoHideAfter || 2000,
                stacking: "down",
                templates: [
                    {
                        type: "info",
                        template:
                            "<div class='notification info'>" +
                                "<p>#= message #</p>" +
                                "</div>"
                    }, {
                        type: "error",
                        template:
                            "<div class='notification error'>" +
                                "<p>#= message #</p>" +
                                "</div>"
                    }, {
                        type: "upload-success",
                        template:
                            "<div class='notification upload-success'>" +
                                "<p>#= message #</p>" +
                                "</div>"
                    }
                ]

            }).data("kendoNotification");
        }

        if (type === "error") {
            notification.options.autoHideAfter = autoHideAfter || 6000;
            console.log("error: " + msg);
        } else {
            notification.options.autoHideAfter = autoHideAfter || 2000;
        }

        notification.show({
            message: msg
        }, type);
    },
    errorMsg: function (msg, autoHideAfter) {
        this.msg(msg, "error", autoHideAfter);
    },
    infoMsg: function (msg, autoHideAfter) {
        this.msg(msg, "info", autoHideAfter);
    },
    infoUploadSuccess: function (msg, autoHideAfter) {
        this.msg(msg, "upload-success", autoHideAfter);
    },
    errorsMsg: function (errors) {
        if (errors) {
            var message = "";

            $.each(errors, function (key, value) {
                if ('errors' in value) {
                    $.each(value.errors, function () {
                        message += this + "\n";
                    });
                }
            });

            this.errorMsg(message);
        }
    },
    confirm: function (title, msg, callbackTrue, callbackFalse) {
        var $w = $("#wnd_confirm__");

        if (!$w.length) {
            $("body").append(
                '<div id="wnd_confirm__" class="k-popup-edit-form k-window-content k-content">' +
                    '<div class="k-edit-form-container">' +
                        '<p class="k-popup-message"></p>' +
                        '<div class="k-edit-buttons k-state-default">' +
                            '<a class="btn k-button btn-primary" id="ok" href="#"><span class="k-icon k-update"></span>Да</a>' +
                            '<a class="btn k-button btn-default" id="cancel" href="#"><span class="k-icon k-cancel"></span>Нет</a>' +
                        '</div>' +
                    '</div>' +
                '</div>');

            $w = $("#wnd_confirm__");

            $w.kendoWindow({
                title: "",
                width: 400,
                modal: true,
                visible: false
            });
        }

        var wnd = $w.data("kendoWindow");

        wnd.title(pbaAPI.truncateStr(title, 25));

        $w.find("p.k-popup-message").html(msg);

        $w.find("a#ok").off().on("click", function () {
            wnd.close();
            if (callbackTrue) callbackTrue();
        });
        $w.find("a#cancel").off().on("click", function () {
            wnd.close();
            if (callbackFalse) callbackFalse();
        });

        wnd.center();
        wnd.open();
    },
    getRandomInt: function (min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    },
    getValueByLang: function (xml, lang) {
        return $(xml).find(lang).text();
    },
    imageHelpers: {
        src: function (img, id) {
            img = $(img);

            if (img.length) {
                var width = img.attr("width");
                var height = img.attr("height");

                img.attr("src", this.getsrc(id, width, height));
            }
        },
        getsrc: function (id, width, height, defImage) {
            return application.url.GetFiles("GetImage", { id: id, width: width, height: height, defImage: defImage });
        },
        getImageSrc: function (img, w, h, defImage) {
            if (img) {
                return application.url.GetFiles("GetImage", { id: img.FileID, width: w, height: h, defImage: defImage });
            }
            return application.url.GetFiles("GetImage", { id: null, width: w, height: h, defImage: defImage });
        }
    },
    getUserStr: function (user, onlyImage, w, h) {
        var html = ' ';

        if (user) {
            if (user.Image) {
                html += '<img data-user-image=' + user.ID + ' class="img-circle" src="' + pbaAPI.imageHelpers.getsrc(user.Image.FileID, h || 32, w || 32, "NoPhoto") + '">';
            } else {
                html += '<img data-user-image=' + user.ID + ' class="img-circle" src="' + pbaAPI.imageHelpers.getsrc(null, h || 32, w || 32, "NoPhoto") + '">';
            }

            if (!onlyImage)
                html += "&nbsp;&nbsp;" + pbaAPI.htmlEncode(user.FullName);
        }

        return html;
    },
    getHrefFile: function (fileid) {
        return application.url.GetFiles("GetFile", { fileid: fileid });
    },
    getFileWidget: function (id, callback) {
        $.get(application.url.GetFileData("GetWidget"), { id: id }, function (res) { callback(res.html); });
    },
    toClientTemplate: function (html) {
        return html.replace("\"#", "\"\\\#").replace("'#", "'\\\#");
    },
    getIDs: function (arr) {
        var _arr = arr || [];

        var ids = new Array(_arr.length);

        for (var i = 0; i < _arr.length; i++) {
            if (_arr[i]) {
                ids[i] = _arr[i].ID;
            }
        }

        return ids;
    },
    getPrVal: function (obj, prs, def) {
        var arrprs = prs.split(".");

        for (var i = 0; i < arrprs.length; i++) {
            var pr = arrprs[i];

            if (pr in obj) {
                obj = obj[pr];

                if (!obj) return def;

            } else {
                return def;
            }
        }

        if (obj)
            return pbaAPI.htmlEncode(obj);
        else
            return obj;
    },
    getCollectionPrVal: function (obj, prs, def) {
        var res = def;

        if (obj) {

            res = "<ul>";

            for (var i = 0; i < obj.length; i++) {
                res += "<li>" + pbaAPI.getPrVal(obj[i], prs, "") + "</li>";
            }

            res += "</ul>";
        }

        return res;
    },
    replaceFormPlaceholders: function (form, params) {
        var initializedParams = {};
        $.each(params, function (i, p) {
            initializedParams[i] = params[i].replace(/\[(.*?)\]/g, function (g1, g2) {
                return form.getPr(g2);
            });
        });

        return initializedParams;
    },

    replaceObjectPlaceholders: function (obj, params) {
        var initializedParams = {};
        $.each(params, function (i, p) {
            initializedParams[i] = params[i].replace(/\[(.*?)\]/g, function (g1, g2) {
                return obj[g2];
            });
        });

        return initializedParams;
    },

    replaceListPlaceholders: function (currentObject, params) {
        var initializedParams = {};
        $.each(params, function (i, p) {
            initializedParams[i] = params[i].replace(/\[(.*?)\]/g, function (g1, g2) {
                return currentObject[g2];
            });
        });

        return initializedParams;
    },
    extension: function (fileName) {
        if (!fileName || typeof fileName !== 'string')
            return null;

        var arr = fileName.toLowerCase().split('.');

        return arr[arr.length - 1];
    },
    _fileTypes: {
        text: 'txt,doc,rtf,log,tex,msg,text,wpd,wps,docx,page',
        table: 'csv,dat,tar,xml,vcf,pps,key,ppt,pptx,sdf,gbr,ged',
        sound: 'mp3,m4a,waw,wma,mpa,iff,aif,ra,mid,m3v',
        video: 'e-3gp,shf,avi,asx,mp4,e-3g2,mpg,asf,vob,wmv,mov,srt,m4v,flv,rm',
        image: 'png,psd,psp,jpg,tif,tiff,gif,bmp,tga,thm,yuv,dds',
        vector: 'ai,eps,ps,svg',
        exdoc: 'pdf,pct,indd',
        spreadsheet: 'xlr,xls,xlsx',
        database: 'db,dbf,mdb,pdb,sql,aacd',
        executable: 'app,exe,com,bat,apk,jar,hsf,pif,vb,cgi',
        code: 'css,js,php,xhtml,htm,html,asp,cer,jsp,cfm,aspx,rss,csr,less',
        font: 'otf,ttf,font,fnt,eot,woff',
        archive: 'zip,zipx,rar,targ,sitx,deb,e-7z,pkg,rpm,cbr,gz',
        mount: 'dmg,cue,bin,iso,hdf,vcd',
        system: 'bak,tmp,ics,msi,cfg,ini,prf'
    },
    fileType: function (fileName) {
        var ext = this.extension(fileName);

        if (!ext) return null;

        for (var key in this._fileTypes) {
            if (this._fileTypes.hasOwnProperty(key)) {
                if (this._fileTypes[key].split(',').indexOf(ext) !== -1) {
                    return key;
                }
            }
        }

        return null;
    },
    extensionClass: function (fileName) {
        var ext = this.extension(fileName);

        if (!ext) return null;

        var knownType = !!this.fileType(ext);

        return knownType
            ? 'filetype filetype-' + ext
            : 'default-file';
    },
    replaceUrlParametr: function (uri, key, value) {
        var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
        separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }

        return uri + separator + key + "=" + value;
    },
    addUrlParametrs: function (uri, params) {
        if (params) {
            for (var key in params) {
                uri = this.replaceUrlParametr(uri, key, params[key]);
            }
        }
        return uri;
    },
    guidGenerator: function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    },

    truncateStr: function (str, max_len) {
        if (str) {
            if (str.length > max_len) {
                str = str.substr(0, max_len - 3);

                var idx = str.lastIndexOf(' ');

                //Средняя длина русского слова составляет 5-8 символа
                if (max_len - idx <= 10) {
                    str = str.substr(0, idx);
                }

                str += "...";
            }
        } else {
            str = "";
        }

        return str;
    },
    gridBaseObjectColumnFilterUi: function (params) {
        var wrapGrid = params.grid;
        var mnemonic = params.mnemonic;
        var colName = params.colName;
        var lookuppropery = params.lookuppropery;
        var $element = params.element;
        var hideBtnOpenDialog = params.hideBtnOpenDialog;
        var isBasecollection = params.isBasecollection;

        var filter = wrapGrid.getFilter();

        var field = colName;
        var fieldCol = colName;

        if (!isBasecollection) {
            field += ".ID";
            fieldCol += "." + lookuppropery;
        }

        var placeholder = "Введите значение...";

        var $form = $element.closest("form");
        var $select = $form.find("select");
        var $btnFilter = wrapGrid.element().find("th[data-field='" + fieldCol + "']").find(".k-grid-filter");
        var $btnSubmit = $form.find("button[type=submit]");

        $select.removeAttr("data-bind");
        $element.removeAttr("data-bind");

        $element
            .wrap($("<div>", { css: { position: "relative" } }))
            .wrap($("<div>", { css: { float: "left", width: "264px" } }))
            .parent().parent()
                .append($("<a>", { "class": "k-button openDialog", title: "Выбрать", html: "...", css: { width: "30px", padding: "2px", margin: "3px" } })
                    .click(function (e) {
                        if ($(this).hasClass("k-state-disabled")) return;

                        pbaAPI.openModalDialog(mnemonic,
                            function (res) {
                                var ids = pbaAPI.getIDs(res);

                                var multiSelect = $element.data("kendoMultiSelect");

                                multiSelect.value(ids);

                                setTimeout(function () {
                                    $btnFilter.click();
                                }, 300);
                            },
                            {
                                title: "ВЫБОР",
                                callbackCancel: function () {
                                    setTimeout(function () {
                                        $btnFilter.click();
                                    }, 300);
                                }
                            })
                    }))
                .append($("<div>", { "class": "clear" }))
                .append($("<input id='isNull' type='checkbox' style='margin: 10px;' />").click(function (e) {

                    if ($(this).is(':checked')) {
                        $element.data("kendoMultiSelect").enable(false);
                        $element.data("kendoMultiSelect").value("");
                        $form.find(".k-button.openDialog").addClass("k-state-disabled");

                    } else {
                        $element.data("kendoMultiSelect").enable(true);
                        $form.find(".k-button.openDialog").removeClass("k-state-disabled");
                    }
                })).append("<span>Пустые</span>");

        if (hideBtnOpenDialog) {
            $form.find(".k-button.openDialog").hide();
        }

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: application.url.GetStandart("FilterBaseObject_Read"),
                    data: function () {
                        var val = $element.data("kendoMultiSelect").input.val();

                        if (val == placeholder) val = "";

                        return {
                            startswith: val,
                            mnemonicCollection: wrapGrid.mnemonic(),
                            property: colName,
                        }
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true
        });

        $element.kendoMultiSelect({
            autoBind: false,
            placeholder: "Введите значение...",
            dataTextField: lookuppropery,
            dataValueField: "ID",
            dataSource: dataSource,
            filter: "startswith"
        });

        var multiSelect = $element.data("kendoMultiSelect");

        $btnSubmit.click(function (e) {
            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            var filters = [];

            var values = multiSelect.value();

            var operator = $select.data("kendoDropDownList").value();

            if ($form.find("input#isNull").is(':checked')) {
                filters.push({
                    field: field,
                    value: "null",
                    operator: operator
                });
            } else {
                for (var i = 0; i < values.length; i++) {
                    filters.push({
                        field: field,
                        value: values[i],
                        operator: operator
                    });
                }
            }

            if (filters.length > 0) {
                filter.filters.push({
                    filters: [{
                        field: fieldCol,
                        value: "''",
                        operator: "neq"
                    }],
                    logic: "and",
                    _field_: fieldCol,
                });

                filter.filters.push({
                    filters: filters,
                    logic: operator == "eq" ? "or" : "and",
                    _field_: fieldCol,
                });
            }

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });

        $form.find("button[type=reset]").click(function (e) {
            multiSelect.value("");

            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            multiSelect.enable(true);

            $form.find("input#isNull").prop("checked", false);
            $form.find(".k-button.openDialog").removeClass("k-state-disabled");

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });
    },
    gridStringColumnFilterUi: function (wrapGrid, property, $element) {
        $element.css({ width: "100%" });

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: application.url.GetStandart("Filter_Read"),
                    data: function () {
                        return {
                            startswith: $element.val(),
                            mnemonic: wrapGrid.mnemonic(),
                            property: property,
                            propertyIsBaseObject: false,
                        }
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true
        });

        $element.kendoAutoComplete({
            dataSource: dataSource,
            filter: "startswith"
        });
    },
    gridColumnFilterUi: function (values, $element, dataTextField, dataValueField) {
        $element.css({ width: "100%" });

        $element.kendoDropDownList({
            dataTextField: dataTextField || "Text",
            dataValueField: dataValueField || "Value",
            optionLabel: "-значение-",
            dataSource: values
        });
    },
    gridEnumColumnFilterUi: function (wrapGrid, fieldCol, typeEnum, json, $element, dataTextField, dataValueField) {
        var $form = $element.closest("form");
        var $select = $form.find("select");
        var $btnFilter = wrapGrid.element().find("th[data-field='" + fieldCol + "']").find(".k-grid-filter");
        var $btnSubmit = $form.find("button[type=submit]");

        $select.remove();
        $element.remove();

        var $buttons = $("<div>", {
            attr: {
                "class": "btn-group-vertical filter-selector",
            },
            css: {
                width: "250px",
                padding: "12px",
            }
        }).insertAfter($form.find("div.k-filter-help-text"));

        var values = $.parseJSON(json);
        var template = kendo.template(templates.enumValueTemplate(typeEnum));

        for (var i = 0; i < values.length; i++) {
            var val = values[i];

            $buttons.append($("<button>", {
                "type": 'button',
                "class": 'btn btn-default',
                "data-toggle": "buttons",
                "data-val": val.Name,

                css: {
                    "text-align": "left",
                },
                html: $(template(val)).css("padding-left", "20px"),
            }).click(function () {
                var $btn = $(this);
                $btn.toggleClass("t-selected");
            }));
        }

        var filter = wrapGrid.getFilter();

        $.each(filter.filters, function (i, gf) {
            if (gf.filters) {
                $.each(gf.filters, function (j, filter) {
                    if (filter.field === fieldCol) {
                        $buttons.find("button[data-val='" + filter.value + "']").click();
                        gf["_field_"] = fieldCol;
                    }
                });
            }
        });

        $btnSubmit.click(function (e) {
            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            var filters = [];

            $buttons.find("button.active").each(function (i, btn) {
                var $btn = $(btn);
                var val = $(btn).attr("data-val");

                filters.push({
                    field: fieldCol,
                    value: val,
                    operator: "eq"
                });
            });

            if (filters.length > 0) {
                filter.filters.push({
                    filters: filters,
                    logic: "or",
                    _field_: fieldCol
                });
            }

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });

        $form.find("button[type=reset]").click(function (e) {
            $buttons.find("button.active").each(function (i, btn) {
                $(btn).click();
            });

            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });
    },
    gridPeriodColumnFilterUi: function (wrapGrid, fieldCol, format, $element) {
        var $form = $element.closest("form");
        var $select = $form.find("select");
        var $btnFilter = wrapGrid.element().find("th[data-field='" + fieldCol + "']").find(".k-grid-filter");
        var $btnSubmit = $form.find("button[type=submit]");
        var filter = wrapGrid.getFilter();

        $select.remove();
        $element.remove();

        var $content = $("<div>", {

        }).insertAfter($form.find("div.k-filter-help-text"));

        $content
            .append($("<input>", { id: "dtm1", placeholder: "Начало", css: { width: "100%" } }))
            .append($("<input>", { id: "dtm2", placeholder: "Окончание", css: { width: "100%" } }));

        var $dtm1 = $content.find("#dtm1");
        var $dtm2 = $content.find("#dtm2");

        $dtm1.kendoDateTimePicker({
            format: format,
        });

        $dtm2.kendoDateTimePicker({
            format: format,
        });

        $btnSubmit.click(function (e) {
            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            var filters = [];

            var val1 = $dtm1.data("kendoDateTimePicker").value();

            if (val1) {
                filters.push({
                    field: fieldCol,
                    value: val1,
                    operator: "gte"
                });
            }
            var val2 = $dtm2.data("kendoDateTimePicker").value();

            if (val2) {
                filters.push({
                    field: fieldCol.replace(".Start", ".End"),
                    value: val2,
                    operator: "lte"
                });
            }

            if (filters.length > 0) {
                filter.filters.push({
                    filters: filters,
                    logic: "and",
                    _field_: fieldCol,
                });
            }

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });

        $form.find("button[type=reset]").click(function (e) {
            $dtm1.data("kendoDateTimePicker").value(null);
            $dtm2.data("kendoDateTimePicker").value(null);

            for (var i = 0; i < filter.filters.length; i++) {
                if (filter.filters[i]["_field_"] === fieldCol) {
                    filter.filters.splice(i, 1);
                    i--;
                }
            }

            wrapGrid.bind();

            $btnFilter.click();

            return false;
        });
    },
    isEmpty: function (obj) {
        if (obj == null || obj == undefined) return true;

        for (var key in obj) {
            return false;
        }

        return true;
    },
    openModalDialog: function (mnemonic, callbackSelect, params) {
        var config = application.viewModelConfigs.getConfig(mnemonic);

        var _params = $.extend({
            title: null,
            width: null,
            height: null,
            callbackCancel: null,
            searchStr: null,
            sysFilter: null,
            maximize: false,
        }, params);

        var wid = this.guidGenerator();

        $("body").append("<div id='" + wid + "'></div>");

        var $w = $("#" + wid);

        var getparams = { mnemonic: mnemonic, typeDialog: "Modal" };

        if (_params.searchStr != null)
            getparams.searchStr = _params.searchStr;

        if (_params.sysFilter != null)
            getparams.sysFilter = _params.sysFilter;

        $w.kendoWindow({
            width: _params.width || $(window).width() - 100,
            height: _params.height || $(window).height() - 100,
            title: _params.title || config.ListView.Title || config.Title,
            actions: [
                    "Maximize",
                    "Close"
            ],
            content: application.url.GetStandart("GetDialog", getparams),
            modal: true,
            visible: false,
            refresh: function (e) {
                var wnd = e.sender;

                if (!wnd["_init_wnd_content"]) {
                    var dialogID = wnd.element.find("#DialogID").val();

                    var dialog = window[dialogID];

                    dialog.resize(wnd.element.height() - 40);

                    var $btnSelect = wnd.element.find("#btnSelect");
                    var $btnCancel = wnd.element.find("#btnCancel");

                    if (callbackSelect == null) {
                        $btnSelect.hide();
                        $btnCancel.find(".button-text").html("Закрыть");
                    }

                    $btnCancel.click(function () {
                        wnd.close();
                    });

                    if (callbackSelect != null) {
                        var typeListView = wnd.element.find("#TypeListView").val();

                        switch (typeListView) {
                            case "Grid":
                                var gridID = wnd.element.find("#GridID").val();

                                var grid = window[gridID];

                                //переопределяем
                                grid.dblclick = function () {
                                    $btnSelect.click();
                                };

                                $btnSelect.click(function () {
                                    var items = grid.getSelectDataItem();

                                    if (items != null && items.length > 0) {

                                        callbackSelect(items);

                                        wnd.close();
                                    } else {
                                        pbaAPI.errorsMsg("Выберите элемент");
                                    }
                                });

                                if (_params.callbackCancel) {
                                    $btnCancel.click(function () {
                                        _params.callbackCancel();
                                    });
                                }

                                break;

                            case "Tree":
                                var treeID = wnd.element.find("#TreeViewID").val();

                                var tree = window[treeID];

                                $btnSelect.click(function () {
                                    var id = tree.getSelectID();

                                    if (id != null) {
                                        $.get(application.url.GetStandart("Get"), { mnemonic: mnemonic, id: id }, function (res) {
                                            callbackSelect([res.model]);
                                            wnd.close();
                                        });
                                    } else {
                                        pbaAPI.errorsMsg("Выберите элемент");
                                    }
                                });

                                if (_params.callbackCancel) {
                                    $btnCancel.click(function () {
                                        _params.callbackCancel();
                                    });
                                }

                                break;

                            case "Custom":
                                var customdialog = wnd.element
                                    .find("[custom-dialog]")
                                    .data('dialog');


                                console.log(!!params.multiselect);

                                customdialog.init({
                                    multiselect: !!params.multiselect
                                });

                                $btnSelect.click(function () {
                                    var items = customdialog.getSelected();
                                    console.log(items);
                                    if (items && items.length) {
                                        callbackSelect(items);
                                        wnd.close();
                                    } else {
                                        pbaAPI.errorMsg("Выберите элемент");
                                    }
                                });

                                if (_params.callbackCancel) {
                                    $btnCancel.click(function () {
                                        _params.callbackCancel();
                                    });
                                }

                                break;
                        }
                    }
                } else {
                    var dialogID = wnd.element.find("#DialogID").val();
                    var dialog = window[dialogID];

                    dialog.resize(wnd.element.height() - 40);
                }

                wnd["_init_wnd_content"] = true;
            },
            close: function (e) {
                $w.empty();
            }
        });

        var wnd = $w.data("kendoWindow");
        wnd.center();
        wnd.open();

        //if (_params.maximize)
        //    wnd.maximize();
    },
    wndID: function (id, mnemonic) {
        if (id && id.indexOf("wnd_") == 0) {
            return id;
        }

        var wid;

        if (id) {
            wid = mnemonic.split(",")[0];
            wid = "wnd_" + wid.replace(/\./g, "_") + "_" + id;
        } else {
            wid = "wnd_" + this.guidGenerator().replace(/\-/g, "");
        }

        return wid;
    },

    initViewModel: function (mnemonic, params) {
        var config = $.extend({
            DetailView: {
                Title: null,
                Width: null,
                Height: null,
            },
        }, application.viewModelConfigs.getConfig(mnemonic));

        var _params = $.extend({
            wid: null,
            title: null,
            width: null,
            height: null,
        }, params);

        _params.wid = this.wndID(_params.wid, mnemonic);

        var $w = $("#" + _params.wid);

        if ($w.length == 0) {
            $("body").append("<div id='" + _params.wid + "'class='view-model-window wnd-loading-content'></div>");

            $w = $("#" + _params.wid);
        }

        if (!$w.data("kendoWindow")) {
            $w.kendoWindow({
                width: _params.width || config.DetailView.Width || $(window).width() - 100,
                height: _params.height || config.DetailView.Height || $(window).height() - 100,
                title: _params.title || config.DetailView.Title || config.Title,
                actions: _params.actions || ["Maximize", "Close"],
                content: application.url.GetStandart("GetPartialViewModel",
                    {
                        mnemonic: mnemonic,
                        typeDialog: "Modal"
                    }),
                modal: true,
                visible: false
            });
        }

        return $w;
    },
    openViewModelEx: function (mnemonic, params) {
        mnemonic = mnemonic.split(",")[0].trim();

        var config = $.extend({
            DetailView: {
                Title: null,
                Width: null,
                Height: null,
            },
        }, application.viewModelConfigs.getConfig(mnemonic));

        var _params = $.extend({
            wid: null,
            title: null,
            width: null,
            height: null,
            actions: null,
            buttons: {},
            isMaximaze: config.DetailView.IsMaximaze || Math.min($(window).width(), $(window).height()) < 768,
            id: 0,
            ids: null,
            entity: null,
            entities: null,
            toSave: null,
            initNewEntity: null,
            beforeSave: null,
            callback: function (e) { },
            isReadOnly: false,
            parentForm: null,
            hideToolbar: false,
            customQueryGetParams: null,
            customQuerySaveParams: null,
        }, params);

        _params.wid = this.wndID(_params.wid, mnemonic);

        if (_params.toSave == null)
            _params.toSave = _params.entity == null;

        var wnd = this.initViewModel(mnemonic, _params).data("kendoWindow");

        wnd.unbind("close");
        wnd.bind("close", function (e) {
            var wnd = e.sender;

            var $dialog = wnd.element.find(".dialog-vm");

            var dialog = $dialog.data("dialogVM");;

            dialog.element().hide();

            if (dialog.changeObjects.length > 0) {
                _params.callback({
                    type: "save",
                    model: dialog.getCurrentModel(),
                    changeObjects: dialog.changeObjects
                });

                dialog.destroy();
            } else {
                _params.callback({
                    type: "close",
                    model: dialog.getCurrentModel(),
                    changeObjects: []
                });
            }
        });

        wnd.unbind("resize");
        wnd.bind("resize", function (e) {
            e.sender.element.find("form").each(function (indx, elform) {
                var $form = $(elform);

                if ($form.is(":visible")) {
                    $form.data("pbaForm").onResize(e.sender);
                }
            });
        });

        var dialogID = wnd.element.find("#DialogID").val();

        var entities = {};

        if (_params.id) {
            entities[_params.id] = { model: null, order: 0 };
        } else if (_params.entity) {
            if (!("ID" in _params.entity))
                _params.entity.ID = _params.entity.uid || pbaAPI.guidGenerator();

            entities[_params.entity.ID] = { model: _params.entity, order: 0 };
        }

        if ($.isArray(_params.ids)) {
            for (i = 0; i < _params.ids.length; i++) {
                var id = _params.ids[i];

                if (id && id != 0) {
                    entities[id] = { model: null, order: i };
                }
            }
        } else if (_params.entities) {
            for (var i = 0; i < _params.entities.length; i++) {

                var entity = _params.entities[i];

                if (!("ID" in entity))
                    entity.ID = entity.uid || pbaAPI.guidGenerator();

                entities[entity.ID] = { model: entity, order: i };
            }
        }

        var dialogParams = {
            wnd: wnd,
            currentID: _params.entity ? _params.entity.ID : _params.id,
            entities: entities,
            parentForm: _params.parentForm,
            isReadOnly: _params.isReadOnly,
            toSave: _params.toSave,
            hideToolbar: _params.hideToolbar,
            buttons: _params.buttons,
            customQueryParams: {
                get: _params.customQueryGetParams,
                save: _params.customQuerySaveParams
            },
            events: {
                initNewEntity: _params.initNewEntity,
                beforeSave: _params.beforeSave,
                save: function (e) {
                    if (_params.callback)
                        _params.callback({
                            type: "save",
                            model: e.sender.changeObjects[e.sender.changeObjects.length - 1],
                            changeObjects: e.sender.changeObjects
                        });

                    e.sender.destroy();
                    wnd.close();
                }
            }
        };

        if (dialogID) {
            window[dialogID].initDialog(dialogParams);
        } else {
            wnd.bind("refresh", function (e) {
                var wnd = e.sender;

                var $dialogID = wnd.element.find("#DialogID");

                dialogID = $dialogID.val();

                window[dialogID].initDialog(dialogParams);
            });
        }

        wnd.center();
        wnd.pin();
        wnd.open();

        if (_params.isMaximaze)
            wnd.maximize();

        return wnd.element;
    },
    openViewModel: function (mnemonic, wid, id, callback) {
        return this.openViewModelEx(mnemonic, { id: id, wid: wid, callback: callback });
    },
    openViewModelReadOnly: function (mnemonic, wid, obj, callback) {
        return this.openViewModelEx(mnemonic, { entity: obj, wid: wid, isReadOnly: true, callback: callback });
    },
    openViewModelForObject: function (mnemonic, wid, obj, callback) {
        return this.openViewModelEx(mnemonic, { wid: wid, entity: obj, callback: callback, toSave: false });
    },


    initWizardViewModel: function (mnemonic, params) {
        var $content = application.getContent();

        var config = application.viewModelConfigs.getConfig(mnemonic);

        var _params = $.extend({
            wid: null,
            title: null,
            width: null,
            height: null,
        }, params);

        _params.wid = this.wndID(_params.wid, mnemonic);

        var $w = $("#" + _params.wid);

        if ($w.length == 0) {
            $("body").append("<div id='" + _params.wid + "'class='view-model-window wnd-loading-content'></div>");

            $w = $("#" + _params.wid);

            $w.kendoWindow({
                width: _params.width || config.DetailView.Width || $content.width(),
                height: _params.height || config.DetailView.Height || $content.height(),
                title: _params.title || config.DetailView.Title || config.Title,
                actions: [
                    "Maximize",
                    "Close"
                ],
                content: application.url.GetWizard("GetViewModel",
                    {
                        mnemonic: mnemonic,
                        typeDialog: "Modal"
                    }),
                modal: true,
                visible: false
            });
        }

        return $w;
    },

    openWizardViewModelEx: function (mnemonic, params) {
        var $content = application.getContent();

        mnemonic = mnemonic.split(",")[0].trim();

        var config = application.viewModelConfigs.getConfig(mnemonic);

        var _params = $.extend({
            wid: null,
            title: null,
            width: null,
            height: null,
            isMaximaze: config.DetailView.IsMaximaze,
            id: 0,
            ids: null,
            entity: null,
            entities: null,
            toSave: null,
            initNewEntity: null,
            nextStep: null,
            onNextStep: null,
            beforeSave: null,
            callback: function (e) { },
            isReadOnly: false,
            parentForm: null,
            hideToolbar: false,
            customQueryGetParams: null,
            customQuerySaveParams: null,
        }, params);

        _params.wid = this.wndID(_params.wid, mnemonic);

        if (_params.toSave == null)
            _params.toSave = _params.entity == null;

        var wnd = this.initWizardViewModel(mnemonic, _params).data("kendoWindow");

        wnd.bind("close", function (e) {
            var wnd = e.sender;

            var $dialogID = wnd.element.find("#DialogID");

            var dialogID = $dialogID.val();

            var dialog = window[dialogID];

            dialog.element().hide();

            if (dialog.changeObjects.length > 0) {
                _params.callback({
                    type: "save",
                    model: dialog.changeObjects[dialog.changeObjects.length - 1],
                    changeObjects: dialog.changeObjects
                });

                dialog.destroy();
            }
        });

        wnd.bind("resize", function (e) {
            e.sender.element.find("form").each(function (indx, elform) {
                var $form = $(elform);

                if ($form.is(":visible")) {
                    $form.data("pbaForm").onResize(e.sender);
                }
            });
        });

        var dialogID = wnd.element.find("#DialogID").val();

        var entities = {};


        if (_params.id) {
            entities[_params.id] = { model: null, order: 0 };
        } else if (_params.entity) {
            if (!("ID" in _params.entity))
                _params.entity.ID = _params.entity.uid || pbaAPI.guidGenerator();

            entities[_params.entity.ID] = { model: _params.entity, order: 0 };
        }

        if ($.isArray(_params.ids)) {
            for (i = 0; i < _params.ids.length; i++) {
                var id = _params.ids[i];

                if (id && id != 0) {
                    entities[id] = { model: null, order: i };
                }
            }
        } else if (_params.entities) {
            for (var i = 0; i < _params.entities.length; i++) {
                var entity = _params.entities[i];

                if (!("ID" in entity))
                    entity.ID = entity.uid || pbaAPI.guidGenerator();

                entities[entity.ID] = { model: entity, order: i };
            }
        }

        var dialogParams = {
            wnd: wnd,
            currentID: _params.entity ? _params.entity.ID : _params.id,
            entities: entities,
            parentForm: _params.parentForm,
            isReadOnly: _params.isReadOnly,
            toSave: _params.toSave,
            hideToolbar: _params.hideToolbar,
            customQueryParams: {
                get: _params.customQueryGetParams,
                save: _params.customQuerySaveParams
            },
            events: {
                initNewEntity: _params.initNewEntity,
                nextStep: function (e) {
                    if (_params.nextStep) {
                        _params.nextStep(e);
                    } else {

                    }
                },
                onNextStep: function (e) {
                    if (_params.onNextStep) {
                        _params.onNextStep(e);
                    } else {

                    }
                },
                beforeSave: _params.beforeSave,
                save: function (e) {
                    if (_params.callback)
                        _params.callback({
                            type: "save",
                            model: e.sender.changeObjects[e.sender.changeObjects.length - 1],
                            changeObjects: e.sender.changeObjects
                        });

                    e.sender.destroy();
                    wnd.close();
                }
            }
        };

        if (dialogID) {
            window[dialogID].initDialog(dialogParams); //init dialog
        } else {
            wnd.bind("refresh", function (e) {
                var wnd = e.sender;

                var $dialogID = wnd.element.find("#DialogID");

                dialogID = $dialogID.val();

                window[dialogID].initDialog(dialogParams); //init dialog
            });
        }

        wnd.center();
        wnd.open();

        if (_params.isMaximaze)
            wnd.maximize();

        return wnd.element;

    },


    getFilePreviewHtml: function (file) {
        if (file) {
            var href = pbaAPI.getHrefFile(file.FileID);
            var ext = pbaAPI.extensionClass(file.FileName);
            var fileType = pbaAPI.fileType(file.FileName);

            // image icon
            if (fileType === 'image') {
                return '<a class="imageModal" title="Открыть изображение" data-title="' + (file.parent().Title || file.FileName) + '" href="#" data-id="' + file.FileID + '" data-key="' + file.Key + '">\
                            <img class="file-icon" src="' + pbaAPI.imageHelpers.getsrc(file.FileID, 48, 48) + '" width="48" height="48" alt=""/>\
                        </a>';

                // other KNOWN filetype file icon
            } else {

                return '<a title="' + (ext.indexOf('docx') === -1 ? 'Скачать' : 'Просмотр') + '" href="' + href + '">\
                            <span class="file-icon ' + ext + '"></span>\
                        </a>';

            }
            // unknown filetype file icon
        } else {

            return '<a title="Скачать" href="' + href + '">\
                        <span class="file-icon default-file"></span>\
                    </a>';

        }
    },
    downloadImage: function (id, callback) {
        if (id) {
            var a = document.createElement('a');
            var img = new Image();

            a.href = document.location.protocol
                    + '//'
                    + document.location.host
                    + pbaAPI.getHrefFile(id);
            a.target = '_self';

            img.onload = function () {
                a.click();

                callback && callback();
            }

            img.src = a.href;
        }
    },
    showImage: function (id, title) {
        if (id) {
            var $modal = $('<div>\
                                <img src="" alt="" />\
                                <div>\
                                    <a class="btn k-button btn-default close_button" href="#"><span class="halfling halfling-remove"></span>&nbsp;Закрыть</a>\
                                    <a class="btn k-button btn-default download_button" href="#"><span class="halfling halfling-download"></span>&nbsp;Скачать</a>\
                                    <div style="clear:both"></div>\
                                </div>\
                            </div>').appendTo('body');
            var maxWidth = window.innerWidth * .8;
            var maxHeight = window.innerHeight * .8;
            var fileHref = pbaAPI.getHrefFile(id);
            var btnDownload = $modal.find('.download_button');
            var btnClose = $modal.find('.close_button');

            // setup widget
            $modal.kendoWindow({
                title: title || '',
                modal: true,
                resizable: false,
                visible: false,
                deactivate: function () {
                    this.destroy();
                }
            });

            // preload image
            var image = new Image();
            image.onload = function () {
                var wnd = $modal.getKendoWindow(),
                    img = $modal.find('img')[0],
                    width,
                    height;

                // to keep image proportions
                if (image.width <= maxWidth && image.height <= maxHeight) {
                    width = image.width;
                    height = image.height;
                } else {
                    width = maxWidth;
                    height = width / image.width * image.height;
                    if (height > maxHeight) {
                        height = maxHeight;
                        width = height / image.height * image.width;
                    }
                }

                // setup image attributes
                img.width = width;
                img.height = height;
                img.src = image.src;

                // buttons styling and binding
                btnDownload.add(btnClose).css({
                    display: 'block',
                    width: 100,
                    margin: '0 5px',
                    float: 'right'
                }).parent().css('margin-top', 8);

                btnDownload.attr('href', fileHref);

                btnClose.click(function () {
                    wnd.close();
                });

                // show modal
                wnd.center();
                wnd.open();
            }
            image.src = fileHref;
        }
    },
    showDoc: function (fileid, title) {
        if (fileid) {

            var $modal = $('<div >').appendTo('body');

            $modal.kendoWindow({
                title: title || '',
                modal: true,
                resizable: false,
                visible: false,
                activate: function () {
                },
                deactivate: function () {
                    this.destroy();
                }
            });

            $.get("/FileData/ShowDoc/" + fileid, function (data) {
                $modal.html(data);
                //$modal.find(".close-btn").click($modal.close());

                $modal.getKendoWindow().maximize().center().open();
            });

        }
    },
    translate: function (text, sl, tl, success) {
        $.ajax({
            url: 'https://translate.yandex.net/api/v1.5/tr.json/translate',
            data: {
                key: "trnsl.1.1.20141221T090334Z.1adf7703a7c35a22.d50641aab4d4417719f2be7f7b578abc8ce3ebdd",
                text: text,  // text to translate
                lang: sl + '-' + tl
            }, success: function (result) {
                if (result.text) {
                    success(result.text[0]);
                }
            }, error: function (XMLHttpRequest, errorMsg, errorThrown) {
                pbaAPI.errorMsg(errorMsg);
            }
        });
    },
    isFunction: function (obj) {
        return !!(obj && obj.constructor && obj.call && obj.apply);
    },

    openWorkflowTimelineModal: function (objectType, objectID, workflowID, showCurrentStages) {
        var kendoWindow = $("<div />").kendoWindow({
            width: $(window).width(),
            height: $(window).height(),
            title: "История движения объекта",
            content: "/BusinessProcess/TimeLine?objectType=" + objectType + "&objectid=" + objectID + "&workflowId=" + workflowID + "&showcurrentstages=" + showCurrentStages,
            resizable: false,
            maximize: true,
            actions: ["Close"],
            modal: true,
            deactivate: function () {
                this.destroy();
            },
        });

        var wnd = kendoWindow.data("kendoWindow");
        kendoWindow.addClass("overflowscroll");
        wnd.center().open().maximize();
    },

    htmlEncode: function (val) {
        if (val)
            return kendo.htmlEncode(val);

        return val;
    }

};
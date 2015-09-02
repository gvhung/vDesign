var WrapWidget = kendo.Class.extend({
    composite: null,
    init: function (id, desc, type) {
        this.id = id;
        this.desc = desc;
        this.type = type;
    },
    widget: function () {
        if (this.id && this.type) {
            return $("#" + this.id).data(this.type);
        } else {
            return null;
        }
    },
    element: function () {
        if (this.widget())
            return this.widget().element;
        else
            return null;
    },
    onNeighbourWidgetChanged: function (e) { },
    destroy: function () {
        if (this.widget())
            this.widget().destroy();
    },
    getKeyCookie: function (mnemonic, key) {
        return mnemonic + "." + key;
    },
    getCookie: function (mnemonic, key, def) {
        return $.cookie(this.getKeyCookie(mnemonic, key)) || def;
    },
    setCookie: function (mnemonic, key, val) {
        $.cookie(this.getKeyCookie(mnemonic, key), val);
    }
});

var CompositeControl = WrapWidget.extend({
    init: function (id, desc) {
        this.id = id;
        this.widgets = {};
        WrapWidget.fn.init.call(this, id, desc || "Composite");
    },
    element: function () {
        return $("#" + this.id);
    },
    destroy: function () {
        this.onWidgetChanged({
            sender: this,
            event: "destroy"
        });
    },
    // obj /WrapWidget/ - виджет
    registerWidget: function (obj) {
        if (obj === this) {
            throw new Error("Циклическая ссылка");
        }
        this.widgets[obj.id] = obj;
        obj.composite = this;
    },
    // obj /WrapWidget/ - виджет
    removeWidget: function (obj) {
        delete this.widgets[obj.id]
    },
    // -> obj /WrapWidget/ - виджет
    // name /string/ - имя виджета
    getWidget: function (name) {
        for (var id in this.widgets) {
            if (this.widgets[id].desc === name) {
                return this.widgets[id];
            }
        }

        return null;
    },
    // Вызывается при изменении любого зарегистрированного в композите виджета
    // e.sender /WrapWidget/ - источник
    // e.event /string/ - событие
    // e.params /object/ - параметры
    onWidgetChanged: function (e) {
        // Оповестим все виджиты
        for (var id in this.widgets) {
            if (this.widgets[id] !== e.sender) {
                this.widgets[id].onNeighbourWidgetChanged(e);
            }
        }
        if (e.sender !== this) {
            this.onChildWidgetChanged(e);
        }
    },
    onChildWidgetChanged: function (e) { },
});

var WrapViewModel = WrapWidget.extend({
    init: function (id, desc, typeDialog) {
        this.typeDialog = typeDialog;
        WrapWidget.fn.init.call(this, id, desc, "pbaForm");
    },
    widget: function () {
        return $("#" + this.id).find("form").data(this.type);
    },
    isModal: function () {
        return this.typeDialog == "Modal";
    },
    element: function () {
        return $("#" + this.id).find("form");
    },
});

var WrapTreeView = WrapWidget.extend({
    init: function (id, desc) {
        this.lock = false;
        WrapWidget.fn.init.call(this, id, desc, "kendoTreeView");
    },
    root: function () {
        return this.widget().root;
    },
    select: function (item) {
        if (item) {
            item = $(item);
            
            this.widget().select(item);

            this.onselect();
        }

        return $(this.widget().select());
    },
    onselect: function (source) {
        if (this.composite != null) {

            var $source;

            if (source)
                $source = $(source);
            else
                $source = this.select();

            this.composite.onWidgetChanged(
                {
                    sender: this,
                    event: "select",
                    params: { dataItem: this.widget().dataItem($source), select: $source }
                });
        }
    },
    expand: function (selectNode) {
        this.widget().expand(selectNode);
    },
    append: function (item, node) {
        this.widget().append(item, node);
    },
    remove: function (item) {
        this.widget().remove(item);
    },
    resize: function (height) {
        var $element = this.widget().element;
        $element.height(height);
    },
    getByUid: function (uid) {
        return this.widget().dataSource.getByUid(uid);
    },
    refresh: function () {
        this.widget().dataSource.read();
    },
    setUrlParametr: function (key, val) {
        var url = this.widget().dataSource.transport.options.read.url;

        this.widget().dataSource.transport.options.read.url = pbaAPI.replaceUrlParametr(url, key, val);
        this.widget().options.dataSource.transport.read.url = pbaAPI.replaceUrlParametr(url, key, val);
    },
});

var WrapGrid = WrapWidget.extend({
    init: function (id, desc) {
        this.currentRow = null;
        this.scrollTop = 0;
        WrapWidget.fn.init.call(this, id, desc, "kendoGrid");
    },
    content: function () {
        return this.widget().content;
    },
    table: function () {
        return this.widget().table;
    },
    tbody: function () {
        return this.widget().tbody;
    },
    resize: function (height) {
        var _fault = 2; //Погрешность, возможно связанняа с border-width
        var $grid = this.widget().element;

        var $content = $grid.find(".k-grid-content");

        $content.height(height - ($grid.outerHeight() - $content.outerHeight(true)) - _fault);
    },
    select: function (select) {
        if (select) {
            this.widget().select(select);
            this.onselect();
        }

        var $select = $(this.widget().select());

        return $select;
    },
    onselect: function () {
        if (this.composite != null) {

            var $select = $(this.widget().select());

            this.composite.onWidgetChanged(
                {
                    sender: this,
                    event: "select",
                    params: { dataItem: this.widget().dataItem($select), select: $select }
                });
        }
    },
    clearSelection: function(){
        this.widget().clearSelection();
    },
    selectUID: function () {
        var $select = this.select();

        if ($select.length > 0) {
            return this.select().attr("data-uid");
        }

        return null;
    },
    selectID: function () {
        var uid = this.selectUID();

        if (uid) {
            return this.getByUid(uid).ID;
        }

        return null;
    },
    activeRow: function () {
        var g = this.widget();

        var boolSelect = false;

        if (this.currentRow != null) {
            var data = g.dataSource.data();

            for (var i = 0; i < data.length; i++) {
                if (this.currentRow == data[i].ID) {
                    var row = g.tbody.find("tr[data-uid='" + data[i].uid + "']");

                    if (row.length > 0) {
                        g.select(row);

                        if (g.content)
                            g.content.scrollTop(this.scrollTop);

                        boolSelect = true;
                        break;
                    }
                }
            }
        }

        if (!boolSelect) {
            g.select(g.tbody.find("tr:eq(0)"));

            if (g.content)
                g.content.scrollTop(0);
        }
    },
    initCurrentRow: function () {
        var g = this.widget();
        var $select = g.select();

        if ($select.length > 0) {
            var item = g.dataItem($select);

            this.currentRow = item.ID;

            if (g.content)
                this.scrollTop = g.content.scrollTop();
        } else {
            this.currentRow = null;
            this.scrollTop = 0;
        }
    },
    setContentType: function (contentType) {
        var g = this.widget();
        g.dataSource.transport.options.update.contentType = contentType;
        g.dataSource.transport.options.create.contentType = contentType;
        g.dataSource.transport.options.destroy.contentType = contentType;

        g.dataSource.transport.parameterMap = function (options, type) {
            if (type !== "read") {
                return kendo.stringify(options);
            }

            return g.dataSource.transport.options.parameterMap.call(g, options, type);
        };
    },
    getTextByValue: function (data, column) {
        var key = "_values_collection_col_" + column;
        var grid = this.widget();
        var numcol = null;

        if (typeof column == 'number') {
            numcol = column;
            column = grid.columns[i].field;

        } else {
            for (var i = 0; i < grid.columns.length; i++) {
                if (grid.columns[i].field == column) {
                    numcol = i;
                    break;
                }
            }
        }

        if (!data[column]) {
            return "";
        }

        if (numcol) {
            if (!this[key]) {
                var collection = {};
                var valuesCollection = grid.options.columns[numcol].values;

                for (var value in valuesCollection) {
                    collection[valuesCollection[value].value] = valuesCollection[value].text;
                }

                this[key] = collection;
            }

            return this[key][data[column]];
        }
    },
    getByUid: function (uid) {
        return this.widget().dataSource.getByUid(uid);
    },
    dataSourceRead: function () {
        this.widget().dataSource.read();
    },
    dataItem: function (item) {
        return this.widget().dataItem(item);
    },
    getFilter: function () {
        if (!this.widget().dataSource.filter()) {
            this.widget().dataSource.filter({ filters: [], logic: "and" });
        }

        return this.widget().dataSource.filter();
    },
    clearFilter: function () {
        this.widget().dataSource.filter({});
    },
    setUrlParametr: function (key, val) {
        var url = this.widget().dataSource.transport.options.read.url;
        
        this.widget().dataSource.transport.options.read.url = pbaAPI.replaceUrlParametr(url, key, val);
        this.widget().options.dataSource.transport.read.url = pbaAPI.replaceUrlParametr(url, key, val);
    },
    removeRow: function (item) {
        this.widget().removeRow(item);
    }
});

var WrapScheduler = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoScheduler");
    },
    content: function () {
        return this.widget().content;
    },
    resize: function (height) {
        var $element = this.widget().element;

        $element.height(height);

        this.widget().resize();
    },
    getByUid: function (uid) {
        return this.widget().dataSource.getByUid(uid);
    },
    dataSourceRead: function () {
        this.widget().dataSource.read();
    },
    dataItem: function (item) {
        return this.widget().dataItem(item);
    },
    getFilter: function () {
        return this.widget().dataSource.filter();
    },
    occurrenceByUid: function (uid) {
        return this.widget().occurrenceByUid(uid);
    },
    openRecurringDialog: function (params) {
        var wid = "recurring_dialog_" + this.id;

        var $w = $("#" + wid);

        if ($w.length == 0) {
            $("body").append(
                '<div id="' + wid + '" class="k-popup-edit-form k-window-content k-content">' +
                    '<div class="common-form k-edit-form-container">' +
                        '<p class="k-popup-message">Изменить повтор. событие?</p>' +
                        '<div class="k-edit-buttons k-state-default">' +
                            '<a class="k-button k-scheduler-current" href="#">Только текущую запись</a>' +
                            '<a class="k-button k-scheduler-series" href="#">Все записи данной серии</a>' +
                        '</div>' +
                    '</div>' +
                '</div>');

            $w = $("#" + wid);

            $w.find("a.k-scheduler-current").on("click", function () { $w.data("kendoWindow").close(); params.current(); });
            $w.find("a.k-scheduler-series").on("click", function () { $w.data("kendoWindow").close(); params.series(); });

            $w.kendoWindow({
                title: "Редактировать",
                modal: true,
                visible: false
            });
        }

        var wnd = $w.data("kendoWindow");

        wnd.center();
        wnd.open();
    },
    setUrlParametr: function (key, val) {
        var url = this.widget().dataSource.transport.options.read.url;

        this.widget().dataSource.transport.options.read.url = pbaAPI.replaceUrlParametr(url, key, val);
    },
});

var WrapGantt = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoGantt");
    },
    content: function () {
        return this.widget().content;
    },
    resize: function (height) {
        var $element = this.widget().element;

        $element.height(height - 22);

        this.widget().resize();
    },
    getByUid: function (uid) {
        return this.widget().dataSource.getByUid(uid);
    },
    getByID: function (id) {
        var data = this.data();

        for (var i = 0; i < data.length; i++) {
            if (id == data[i].ID) {
                return data[i];
            }
        }
    },
    dataSourceRead: function () {
        this.widget().dataSource.read();
    },
    dataItem: function (item) {
        return this.widget().dataItem(item);
    },
    getFilter: function () {
        return this.widget().dataSource.filter();
    },
    setUrlParametr: function (key, val) {
        var url = this.widget().dataSource.transport.options.read.url;

        this.widget().dataSource.transport.options.read.url = pbaAPI.replaceUrlParametr(url, key, val);
    },
    data: function () {
        return this.widget().dataSource.data();
    },
    select: function ($task) {
        this.widget().select($task);
    }
});

var WrapSplitter = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoSplitter");
    },
    resize: function (height) {
        this.widget().element
            .height(height)
            .trigger("resize");

        if (this.composite != null) {

            this.composite.onWidgetChanged(
                {
                    sender: this,
                    event: "resize",
                    params: { height: height }
                });
        }
    },
    toggle: function () {
        this.widget().toggle('.k-pane:first');
    },
    collapse: function () {
        this.widget().collapse('.k-pane:first');
    },
    expand: function () {
        this.widget().expand('.k-pane:first');
    }
});

var WrapWindow = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoWindow");
    },
    open: function () {
        this.widget().open();
    },
    center: function () {
        this.widget().center();
    },
    close: function () {
        this.widget().close();
    }
});

var WrapToolbar = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoToolBar");
    },
    enable: function (idbtn, enable) {
        this.widget().enable(idbtn, enable);
    },
    popupEl: function () {
        return this.widget().popup.element;
    }
});

var WrapContextMenu = WrapWidget.extend({
    init: function (id, desc) {
        WrapWidget.fn.init.call(this, id, desc, "kendoContextMenu");
    },
    enable: function (idbtn, enable) {
        this.widget().enable(idbtn, enable);
    },
    open: function () {
        this.widget().open();
    },
    close: function () {
        this.widget().close();
    }
});
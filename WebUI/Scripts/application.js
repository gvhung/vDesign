var application = {
    hostPrefix: "", //window.location.origin,
    url: {
        GetStandart: function (action, params) {
            return pbaAPI.addUrlParametrs(application.hostPrefix + "/Standart/" + action, params);
        },
        GetHCategory: function (action, params) {
            return pbaAPI.addUrlParametrs(application.hostPrefix + "/HCategory/" + action, params);
        },
        GetFiles: function (action, params) {
            return pbaAPI.addUrlParametrs(application.hostPrefix + "/Files/" + action, params);
        },
        GetFileData: function (action, params) {
            return pbaAPI.addUrlParametrs(application.hostPrefix + "/FileData/" + action, params);
        },
        GetWizard: function(action, params) {
            return pbaAPI.addUrlParametrs(application.hostPrefix + "/Wizard/" + action, params);
        }
    },
    getContent: function () {
        var $content = $("#content:first");

        if ($content.length == 0)
            $content = $(window);

        return $content;
    },
    getContentPosition: function () {
        return this.getContent().position();
    },
    //ViewModelConfig 
    //  Mnemonic /string/ - мнемоника сущности -> User
    //  TypeEntity /string/ - тип сущности -> Base.Security.User
    //  Icon /string/ - css class -> halfling halfling-user
    //  Title /string/ - общее наименование -> Пользователи
    //  ListView {}
    //      -- Title /string/ - наименование для списка -> Список пользователей
    //      -- Columns {}
    //          -- Name /string/ - имя колонки
    //          -- Hidden /bool/ - нужно ли скрывать колонку
    //  DetailView {}
    //      -- Title /string/ - наименование для формы -> Пользователь
    //      -- Width /int/ -
    //      -- Height /int/ -
    viewModelConfigs: {
        _configs: null,
        _mnemonicIdx: {},
        _typeEntityIdx: {},
        init: function (configs) {
            if (this._configs == null) {
                this._configs = configs;

                for (var i = 0; i < configs.length; i++) {
                    this._mnemonicIdx[configs[i].Mnemonic] = i;

                    var type = configs[i].TypeEntity;

                    if (!(type in this._typeEntityIdx)) {
                        this._typeEntityIdx[type] = i;
                    }
                }
            }
        },
        getTypes: function () {
            var configs = [];

            for (var type in this._typeEntityIdx) {

                var config = this._configs[this._typeEntityIdx[type]];

                configs.push({
                    Value: type,
                    Text: config.Title
                });
            }

            return configs;
        },
        getMnemonics: function () {
            var configs = [];

            for (var i = 0; i < this._configs.length; i++) {

                var config = this._configs[i];

                configs.push({
                    Value: config.Mnemonic,
                    Text: config.Title
                });
            }

            return configs;
        },
        getConfig: function (key) {
            if (key in this._typeEntityIdx) {
                return this._configs[this._typeEntityIdx[key]];
            }

            if (key in this._mnemonicIdx) {
                return this._configs[this._mnemonicIdx[key]];
            }

            return {};
        }
    },
    _enums: {},
    initEnumValues: function (typeEnum, json) {
        var key = typeEnum;

        if (!this._enums[key]) {
            var values;

            if ($.isArray(json)) {
                values = json;
            } else {
                values = $.parseJSON(json);
            }

            this._enums[key] = {};

            for (var i = 0; i < values.length; i++) {
                var obj = values[i];
                this._enums[key][obj.Value] = { Name: obj.Name, Text: obj.Text };
            }
        }
    },
    getEnumName: function (typeEnum, val) {
        var key = typeEnum;

        this.checkEnumType(key);

        if (this._enums[key] && this._enums[key][val])
            return this._enums[key][val].Name;
        else
            return "";
    },
    getEnumText: function (typeEnum, val) {
        var key = typeEnum;

        this.checkEnumType(key);

        if (this._enums[key] && this._enums[key][val])
            return this._enums[key][val].Text;
        else
            return "";
    },
    checkEnumType: function (typeEnum) {
        var key = typeEnum;

        if (!this._enums[key]) {
            //TODO:
        }
    },
}
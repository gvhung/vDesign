(function (win) {
    var colors = {
        grey:   "#999999",
        cyan:   "#5BC0DE",
        blue:   "#428BCA",
        green:  "#5CB85C",
        orange: "#F0AD4E",
        red:    "#FE4C58",
        blueDark: "#2A5999"
    };
    var icons = {
        info:  "halfling halfling-info-sign",
        book:  "glyphicon glyphicon-log-book",
        chart: "glyphicon glyphicon-pie-chart",
        pin:   "glyphicon glyphicon-pin-flag",
        check: "halfling halfling-check"
    };

    var npa = {
        Stage: {
             "0": { "Description": "Не определен", "Name": "Undefined" },
            "10": { "Description": "Уведомление", "Name": "Notification" },
            "20": { "Description": "Текст", "Name": "Text" },
            "30": { "Description": "Оценка", "Name": "Procedure" },
            "40": { "Description": "Завершение", "Name": "Finalzation" },
            "50": { "Description": "Принятие", "Name": "Complete" }
        },
        Status: {
             "0": { "Description": "Разработка", "Name": "Undefined" },
            "10": { "Description": "Подготовка к обсуждению", "Name": "PreDiscussion" },
            "20": { "Description": "Идет обсуждение", "Name": "Discussion" },
            "30": { "Description": "Обсуждение завершено", "Name": "EndDiscussion" },
            "50": { "Description": "Разработка завершена", "Name": "Complete" },
           "100": { "Description": "Отказ от продолжения разработки", "Name": "Rejected" }
        },
        Result: {
            "0": { "Description": "Не определено", "Name": "Undefined" },
            "1": { "Description": "Положительное", "Name": "Positive" },
            "2": { "Description": "Отрицательное", "Name": "Negative" }
        }
    };

    npa.Stage[0].Color = colors.grey;
    npa.Stage[10].Color = colors.cyan;
    npa.Stage[20].Color = colors.blue;
    npa.Stage[30].Color = colors.orange;
    npa.Stage[40].Color = colors.blue;
    npa.Stage[50].Color = colors.green;

    npa.Stage[0].Icon = icons.info;
    npa.Stage[10].Icon = icons.info;
    npa.Stage[20].Icon = icons.book;
    npa.Stage[30].Icon = icons.chart;
    npa.Stage[40].Icon = icons.pin;
    npa.Stage[50].Icon = icons.check;
    npa.Stage.getValue = getValue.bind(npa.Stage);

    npa.Status[0].Color = colors.grey;
    npa.Status[10].Color = colors.blueDark;
    npa.Status[20].Color = colors.orange;
    npa.Status[30].Color = colors.blue;
    npa.Status[50].Color = colors.green;
    npa.Status[100].Color = colors.red;
    npa.Status.getValue = getValue.bind(npa.Status);

    npa.Result[0].Color = colors.grey;
    npa.Result[2].Color = colors.red;
    npa.Result[1].Color = colors.green;
    npa.Result.getValue = getValue.bind(npa.Result);

    function getValue(propertyName) {
        for (var key in this) {
            if (this.hasOwnProperty(key) && key !== 'getValue' && this[key].Name === propertyName) {
                return parseInt(key);
            }
        }
        return -1;
    }

    if (!win.pba) win.pba = {};
    win.pba.npa = npa;

}(window));

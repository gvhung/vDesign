var templates = {
    uploadFile: function () {
        return kendo.template(
            "<span class='k-progress' style='width: 100%;'></span>" +
            "<a #= name.fileid != null ? 'href=\"' + pbaAPI.getHrefFile(name.fileid) + '\"' : '' # title='Скачать...'><span style='width: 48px; height: 48px;' class='k-icon #= pbaAPI.extensionClass(name.filename) #'></span></a>" +
            "<span style='min-width: 50%;' class='k-filename' title='#=name.filename#'>#=name.filename#</span>" +
            "<strong class='k-upload-status'>" +
                "<button type='button' class='k-button k-button-bare k-upload-action'>" +
                    "<span class='k-icon k-i-close k-delete' title='Удалить'></span>" +
                "</button>" +
            "</strong>");
    },
    enumValueTemplate: function (typeEnum) {
        return "<span class='enum-" + typeEnum + "' data-val='#=data.Name#'>#=data.Text#</span>"
    },
    enumTemplate: function (typeEnum) {
        return "<span class='enum-" + typeEnum + "' data-val='#=data.Name#'>#=data.Text#</span>"
    }
};
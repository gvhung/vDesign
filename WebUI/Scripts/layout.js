$(function () {
    var usersCache = {};

    $('body').on('mouseleave', '[data-user-image]', function (e) {
        var self = $(this);
        self.removeClass('has-popover');
        self.popover('destroy');
    });

    $('body').on('mouseenter', '[data-user-image]', function (e) {

        $('[data-user-image]').popover('destroy');

        var self = $(this);
        self.addClass('has-popover');
        var id = self.data('user-image');

        var deffered = $.Deferred().done(function (str) {
            if (self.hasClass('has-popover')) {
                self.popover({
                    html: true,
                    animation: false,
                    trigger: 'manual',
                    content: str,
                    container: 'body'
                }).popover('show');
            } else {
                $('[data-user-image]').popover('destroy');
            }
        });

        if (usersCache[id]) {
            deffered.resolve(usersCache[id]);
        } else {
            $.get("/Users/GetUser", { id: self.data('user-image') }, function (d) {
                deffered.resolve(renderPopover(d.model));
            });
        }
    });

    var renderPopover = function (user) {
        var html = '<div class="thumbnail">' +
            '<img style="width: 200px; height: 200px;" width="200" height="200" src="/Files/GetImage?id=' + (user.Image != null ? user.Image.FileID : null) + '&width=200&height=200&defImage=NoPhoto">' +
            '</div>' +
            '<dl>' +
            '<dt>ФИО</dt>' +
            '<dd>' + pbaAPI.htmlEncode(user.FullName) + '</dd>';

        html += '<dt>Тип аккаунта</dt>' +
                '<dd>' + pbaAPI.htmlEncode(user.UserCategoryName) + '</dd>';

        if (user.Email) {
            html += '<dt>Email</dt>' +
                    '<dd>' + pbaAPI.htmlEncode(user.Email) + '</dd>';
        }

        if (user.Post) {
            html += '<dt>Должность</dt>' +
                    '<dd>' + pbaAPI.htmlEncode(user.Post.Title) + '</dd>';
        }

        if (user.OfficePhone) {
            html += '<dt>Рабочий телефон</dt>' +
                    '<dd>' + pbaAPI.htmlEncode(user.OfficePhone) + '</dd>';
        }

        if (user.PersonPhone) {
            html += '<dt>Личный телефон</dt>' +
                    '<dd>' + pbaAPI.htmlEncode(user.PersonPhone) + '</dd>';
        }

        html += '</dl>';

        usersCache[user.ID] = html;

        return html;
    }
});
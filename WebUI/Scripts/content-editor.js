(function () {
    function createHead(widget) {
        var $handle = $('<div />', {
            'class': 'widget-heandle'
        });

        $handle.append($("<i />", {
            'class': 'halfling halfling-move'
        }));

        var $delete = $('<a href="#"><i class="halfling halfling-remove"></i></a>').click(function () {
            widget.remove.call(widget);
        });

        var $edit = $('<a href="#"><i class="halfling halfling-cog"></i></a>').click(function () {
            widget.edit.call(widget);
        });

        var $toolbar = $('<div />', {
            'class': 'pull-right content-widget-toolbar'
        }).append($edit).append($delete);

        var $head = $('<div />', {
            'class': 'widget-head'
        }).append($handle, $toolbar);

        return $head;
    }

    var contentWidget = kendo.Class.extend({
        init: function (element, model) {
            this.element = element;
            this.toolbar = createHead(this);

            this.element.prepend(this.toolbar);

            var uid = element.attr('data-uid');
            var parentUid = element.attr('data-parent-uid');

            if (!uid) {
                uid = pbaAPI.guidGenerator();
                element.attr('data-uid', uid);
            }

            if (!parentUid) {
                parentUid = element.closest('[data-widget-name]').attr('uid');
            }

            var options = element.attr('data-options');
            if (options) {
                model = JSON.parse(options);
            }

            this.bind(model);

            this.uid = uid;
            this.parentUid = parentUid;

            element.data('widget', this);
        },
        bind: function (model) {
            var viewModel = kendo.observable({ model: model });
            this.viewModel = viewModel;
            kendo.bind(this.element, viewModel);
        },
        edit: function () {
            var self = this;

            pbaAPI.openViewModelEx(this.mnemonic, {
                wid: self.wrap,
                entity: JSON.parse(JSON.stringify(self.viewModel.get('model'))),
                callback: function (e) {
                    if (e.type === "save") {
                        self.viewModel.set('model', e.model);
                    }
                }
            });
        },
        remove: function () {
            this.element.remove();
        },
        onAppend: function (target, source) {
            source.parentUid = this.uid;

            source.element.attr('data-parent-uid', this.uid);

            if (target.hasClass("droptarget"))
                target.append(source.element);
            else
                target.replaceWith(source.element);
        },
        prepareToSave: function (element) {
            if (!element) {
                element = this.element;
                this.toolbar.remove();
            } else {
                element.find('.widget-head').remove();
            }

            element.attr('data-options', JSON.stringify(this.viewModel.model.toJSON()));
        },
        toHtml: function (element) {
            if (!element) {
                element = this.element;
                this.toolbar.remove();

            } else {
                element.find('.widget-head').remove();
            }

            element
                .removeAttr('data-bind')
                .removeAttr('data-template')
                .find('*')
                .removeAttr('data-bind');
            element.replaceWith(element.children());
        },
        createElement: function (html, name) {
            return $('<div />')
                .html(html)
                .attr('data-widget-name', name)
                .data('widget', this)
                .attr('data-uid', this.uid);
        },
    });

    window['contentEditor'] = {
        ContentWidget: contentWidget
    }

    kendo.data.binders.customstyle = kendo.data.Binder.extend({
        init: function (target, bindings, options) {
            kendo.data.Binder.fn.init.call(this, target, bindings, options);

            this._lookups = [];
            for (var key in this.bindings.customstyle.path) {
                this._lookups.push({
                    css: key.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase(),
                    key: key,
                    path: this.bindings.customstyle.path[key]
                });
            }
        },
        refresh: function () {
            var lookup, value;

            for (var i = 0; i < this._lookups.length; i++) {
                lookup = this._lookups[i];
                this.bindings.customstyle.path = lookup.path;
                value = this.bindings.customstyle.get();

                $(this.element).css(lookup.css, value);
            }
        }
    });

    kendo.data.binders.content_img = kendo.data.Binder.extend({
        init: function (target, bindings, options) {
            kendo.data.Binder.fn.init.call(this, target, bindings, options);

            this._lookups = {};
            for (var key in this.bindings.content_img.path) {
                this._lookups[key] = this.bindings.content_img.path[key];
            }
        },
        refresh: function () {
            this.bindings.content_img.path = this._lookups['original'];
            var original = this.bindings.content_img.get();

            this.bindings.content_img.path = this._lookups['width'];
            var width = original ? null : this.bindings.content_img.get();

            this.bindings.content_img.path = this._lookups['height'];
            var height = original ? null : this.bindings.content_img.get();

            this.bindings.content_img.path = this._lookups['url'];
            var url = this.bindings.content_img.get();

            if (!url) {
                this.bindings.content_img.path = this._lookups['file'];
                var file = this.bindings.content_img.get();

                if (file && file.File) {
                    url = pbaAPI.imageHelpers.getsrc(file.File.FileID, width, height) + '&type=frame';
                } else {
                    url = pbaAPI.imageHelpers.getsrc(null, width, height);
                }
            }

            var $elem = $(this.element);

            $elem.attr('src', url);
            //$elem.attr('width', width);
            //$elem.attr('height', height);
            //$elem.css('max-height', height);
            $elem.css('max-width', '100%');
            $elem.css('max-height', !original ? height : '100%');
        }
    });

    kendo.data.binders.content_file = kendo.data.Binder.extend({
        refresh: function () {
            var file = this.bindings["content_file"].get();

            var $anchor = $(this.element);

            if (file && file.File && file.File.FileID != "00000000-0000-0000-0000-000000000000") {
                $anchor
                    .attr('href', pbaAPI.getHrefFile(file.File.FileID))
                    .attr('title', file.Description)
                    .attr('file-type', file.Extension)
                    .attr('file-size', file.Size)
                    .html(file.Title);
            } else {
                $anchor
                    .attr('href', "#")
                    .attr('title', 'двойной клик для выбора файла')
                    .removeAttr('file-type')
                    .removeAttr('file-size')
                    .html(' двойной клик для выбора файла');
            }
        }
    });

    kendo.data.binders.external_video = kendo.data.Binder.extend({
        refresh: function () {
            var url = this.bindings["external_video"].get();
            var html = '<div class="no-video"><i class="halfling halfling-film"></i> Видео отсутсвует (укажите корректный URL)<div>';

            if (url) {
                var embed = url.replace(/(?:http:\/\/)?(?:www\.)?(?:youtube\.com|youtu\.be)\/(?:watch\?v=)?(.+)/g, '<iframe width="420" height="345" src="http://www.youtube.com/embed/$1" frameborder="0" allowfullscreen></iframe>')

                if (embed) {
                    html = embed;
                }
            }

            $(this.element).html(html);
        }
    });
})();


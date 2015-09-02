(function( $, undefined ) {

    var newGUID = (function () {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                       .toString(16)
                       .substring(1);
        }
        return function () {
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                   s4() + '-' + s4() + s4() + s4();
        }
    })();

    var $body = $('body');

    var SpreadBorderDialog = kendo.Class.extend({
        init: function( $wrapper ){
            if ( ! this._checkForComponents() )
                return;

            var self = this;

            this._initDom( $wrapper );

            this._initGrid();

            this._initKendo();

            this._grid.secureRepaint = function(){
                self._grid.sheet.clearSelection();
                self._grid.sheet.isPaintSuspended( false );
                self._grid.wijspread( 'refresh' );
                self._grid.spread.repaint();
                self._grid.sheet.isPaintSuspended( true );
            };

            // style: { left: { color: string, style: number }, top{}, right{}, bottom{}, vertical{}, horizontal{} }
            this.applyStyle = function applyStyle( sheet, style, triggerArea ){

                var cellsPerBorder = {
                    top: [ [ 1, 1 ], [ 1, 2 ] ],
                    right: [ [ 1, 2 ], [ 2, 2 ] ],
                    bottom: [ [ 2, 1 ], [ 2, 2 ] ],
                    left: [ [ 1, 1 ], [ 2, 1 ] ],
                    vertical: [ [ 1, 1 ], [ 2, 1 ] ],
                    horizontal: [ [ 1, 1 ], [ 1, 2 ] ]
                };

                // clear other selections
                if( triggerArea && triggerArea === 'bound' ){
                    
                    sheet.getCell( 1, 1 )
                        .borderRight( undefined )
                        .borderBottom( undefined );
                    sheet.getCell( 2, 1 ).borderRight( undefined );
                    sheet.getCell( 1, 2 ).borderBottom( undefined );
                }

                for( var name in style ){

                    if( style.hasOwnProperty( name )){

                        var borderName = name === 'vertical'
                            ? 'Right' : name === 'horizontal'
                            ? 'Bottom' : ( name[0].toUpperCase() + name.substr( 1 ));

                        for( var i = 0; i < 2; i++ ){

                            var cell = sheet.getCell( cellsPerBorder[ name ][i][0],
                                cellsPerBorder[ name ][i][1] );

                            var style_before = cell[ 'border' + borderName ]();

                            var style_value = typeof style[ name ].style === 'string'
                                ? $.wijmo.wijspread.LineStyle[ style[name].style ]
                                : style[ name ].style;

                            // to trigger on/off on click top/right/bottom/left areas
                            // if style "before" and "to replace" are the same (color/style_value)
                            if( style_before &&
                                style_before.style !== 0 &&
                                style_value !== 0 &&
                                /top|right|bottom|left/.test( triggerArea ) &&
                                style_before.color === style[ name ].color && 
                                style_before.style === style_value ){

                                style_value = 0;
                            }

                            sheet.getCell( cellsPerBorder[ name ][i][0],
                                cellsPerBorder[ name ][i][1] )[ 'border' + borderName ](
                                    style_value === 0 
                                        ? undefined
                                        : new $.wijmo.wijspread.LineBorder(
                                            style[ name ].color,
                                            style_value
                                ));
                        }
                    }
                }

                self._grid.secureRepaint();
            }
        },
        destroy: function(){
            try{ 
                this._grid && this._grid.wijspread( 'destroy' );
            } catch (e) { /* EMPTY */ }
        
            this._wrapper.remove();
        },
        update: function(){

            // DIRTY HACK
            this._grid.height( 282 );
            this._grid.width( 282 );

            this._grid.css( 'padding', this._grid.width() * .03 );

            if ( this._grid.initCells )
                this._grid.initCells();

            this._grid.secureRepaint();
        },
        reset: function(){
            // set all borders style to 0
            var defaultBorder = new $.wijmo.wijspread.LineBorder( undefined );
            this._grid.sheet.getCells( 1, 1, 2, 2 )
                .borderTop( defaultBorder )
                .borderRight( defaultBorder )
                .borderBottom( defaultBorder )
                .borderLeft( defaultBorder );
            this._grid.secureRepaint();

            // set value of dropdown to 0
            this._dropdown.data( 'kendoDropDownList' ).value( 0 );

            // set value of palette to 0
            this._palette.data( 'kendoColorPalette' ).value( '#000' );

            $( this ).data( 'modified', {
                top: false,
                right: false,
                bottom: false,
                left: false,
                horizontal: false,
                vertical: false
            });

            this._buttonApply.addClass( 'disabled' );
        },
        reflect: function( data ){

            this.applyStyle( this._grid.sheet, data, 'all' );

        },

        _checkForComponents: function(){
            function check( value, nameOfUnit ){
                if ( ! value ){
                    console.error( 'SpreadBorderDialog: ' + nameOfUnit + ' is missing.' );
                    return false;
                }
                return true;
            }

            if ( ! check( $.fn.wijspread, 'wijspread' )) return false;
            if ( ! check( $.fn.kendoDropDownList, 'kendoDropDownList' )) return false;
            if ( ! check( $.fn.kendoColorPalette, 'kendoColorPalette' )) return false;

            return true;
        },
        _initDom: function( $wrapper ){
            this._parent = $wrapper;

            this._wrapper = $( '<div class="pbawijspread_borderDialog"></div>' ).appendTo( $wrapper );
            this._grid = $( '<div class="pbawijspread_grid"></div>' ).appendTo( this._wrapper );
            this._areas = $( '<div class="pbawijspread_areas"></div>' ).appendTo( this._grid );
            this._selectionButtons = $( '<div class="pbawijspread_selectionButtons"></div>' ).appendTo( this._wrapper );
            this._buttonEmpty = $('<a title="очистить" class="btn btn-default empty" href="#"><img src="/Scripts/SpreadJS/images/empty-icon.png" alt="" /></a>').appendTo(this._selectionButtons);
            this._buttonAll = $( '<a title="все границы" class="btn btn-default all" href="#"><img src="/Scripts/SpreadJS/images/all-icon.png" alt="" /></a>' ).appendTo( this._selectionButtons );
            this._buttonBound = $('<a title="по контуру" class="btn btn-default bound" href="#"><img src="/Scripts/SpreadJS/images/bound-icon.png" alt="" /></a>').appendTo(this._selectionButtons);
            this._controls = $( '<div class="pbawijspread_controls"></div>' ).appendTo( this._wrapper );
            this._dropdown = $( '<input class="pbawijspread_kendoDropDownList" />' ).appendTo( this._controls );
            this._palette = $( '<div class="pbawijspread_kendoColorPalette"></div>' ).appendTo( this._controls );
            this._buttons = $( '<div class="pbawijspread_buttons"></div>' ).appendTo( this._wrapper );
            this._buttonCancel = $( '<a class="btn btn-default" href="#" style="margin-right:10px;"><span class="k-icon k-cancel"></span>отмена</a>' ).appendTo( this._buttons );
            this._buttonApply = $( '<a class="btn btn-primary" href="#"><span class="k-icon k-update"></span>применить</a>' ).appendTo( this._buttons );
        },
        _initGrid: function(){
            var self = this;
            function initSpread(){
                var f = false;

                self._grid.wijspread({ sheets: 1 });

                self._grid.spread = self._grid.wijspread( 'spread' );
                self._grid.sheet = self._grid.spread.getActiveSheet();

                // turn off scrollbars, tabstrip, headers
                self._grid.spread.showHorizontalScrollbar( f )
                    .showVerticalScrollbar( f )
                    .tabStripVisible( f );
                self._grid.sheet.setColumnHeaderVisible( f );
                self._grid.sheet.setRowHeaderVisible( f );

                // turn off user zoom
                self._grid.spread.allowUserZoom = false;
            }
            function initSheet(){
                // 3x3 'cause of left and top borders cutting
                self._grid.sheet.setColumnCount( 4 );
                self._grid.sheet.setRowCount( 4 );
                self._grid.sheet.allowUndo( false )
                    .canUserDragDrop( false )
                    .setIsProtected( true );
                self._grid.sheet.clearSelection();
            }
            function initCells(){
                var viewport = $.wijmo.wijspread.SheetArea.viewport;
                var noBorder = new $.wijmo.wijspread.LineBorder( 'rgba(255,255,255,0)', 1 );

                self._grid.sheet.getCells( 0, 0, 0, 3 ).borderRight( noBorder ).locked( true );
                self._grid.sheet.getCells( 0, 0, 3, 0 ).borderBottom( noBorder ).locked( true );
                self._grid.sheet.getCells( 3, 0, 3, 3 ).borderRight( noBorder ).locked( true );
                self._grid.sheet.getCells( 0, 3, 3, 3 ).borderBottom( noBorder ).locked( true );

                self._grid.sheet.getColumn( 0 ).width( 3 );
                self._grid.sheet.getColumn( 3 ).width( 3 );
                self._grid.sheet.getRow( 0 ).height( 3 );
                self._grid.sheet.getRow( 3 ).height( 3 );

                self._grid.sheet.getCells( 1, 1, 2, 2, viewport ).text( 'text' ).hAlign( 1 ).vAlign( 1 ).locked( true );
                self._grid.sheet.getColumns( 1, 2 ).width( self._grid.width() / 2 - 2 );
                self._grid.sheet.getRows( 1, 2 ).height( self._grid.height() / 2 - 2 );
            }
            function initAreas(){
                var positions = [
                    'top', 
                    'bottom', 
                    'left', 
                    'right', 
                    'center area_center_middle', 
                    'center area_center_top', 
                    'center area_center_bottom',
                    'center area_center_left',
                    'center area_center_right'
                ];

                for (var i = 0; i < positions.length; i++){
                    $( '<div class="area area_'+positions[ i ]+'"></div>' )
                        .appendTo( self._areas );
                }
            }
            function bindAreasHover(){
                var $center_areas = self._areas.find( '.area.area_center' );

                // center_all_hover
                $center_areas.filter( '.area_center_middle' ).hover(function(){
                    $center_areas.parent().addClass( 'center_all_hover' );
                }, function(){
                    $center_areas.parent().removeClass( 'center_all_hover' );
                });

                // center_vertical_hover
                $center_areas.filter( '.area_center_top, .area_center_bottom' ).hover(function(){
                    $center_areas.parent().addClass( 'center_vertical_hover' );
                }, function(){
                    $center_areas.parent().removeClass( 'center_vertical_hover' );
                });

                // center_horizontal_hover
                $center_areas.filter( '.area_center_left, .area_center_right' ).hover(function(){
                    $center_areas.parent().addClass( 'center_horizontal_hover' );
                }, function(){
                    $center_areas.parent().removeClass( 'center_horizontal_hover' );
                });

                // empty_hover
                self._selectionButtons.find( '.empty' ).hover(function(){
                    $center_areas.parent().addClass( 'all_hover action_empty' );
                }, function(){
                    $center_areas.parent().removeClass( 'all_hover action_empty' );
                });

                // all_hover
                self._selectionButtons.find( '.all' ).hover(function(){
                    $center_areas.parent().addClass( 'all_hover' );
                }, function(){
                    $center_areas.parent().removeClass( 'all_hover' );
                });

                // bound_hover
                self._selectionButtons.find( '.bound' ).hover(function(){
                    $center_areas.parent().addClass( 'bound_hover' );
                }, function(){
                    $center_areas.parent().removeClass( 'bound_hover' );
                });
            }
            function bindAreasClick(){
                // clickedAreaName = all/bound/center/horizontal/vertical/left/top/right/bottom
                // borderStyle = solid/dotted .....
                // color = red/rgb(255,200,100)/#123 .....
                // return = { left: { style: 'solid', color: '#000' }, top: { ... } ... }
                function makeStyle( clickedAreaName, borderStyle, color ){

                    var style = {};

                    switch( clickedAreaName ){
                        case 'all':
                        case 'center':
                        case 'horizontal':
                            style.horizontal = {
                                color: color, style: borderStyle
                            };
                            if( clickedAreaName === 'horizontal' ) break;
                        case 'vertical':
                            style.vertical = {
                                color: color, style: borderStyle
                            };
                            if( clickedAreaName === 'vertical' ||
                                clickedAreaName === 'center' ) break;
                        case 'bound':
                        case 'top':
                            style.top = {
                                color: color, style: borderStyle
                            };
                            if( clickedAreaName === 'top' ) break;
                        case 'right':
                            style.right = {
                                color: color, style: borderStyle
                            };
                            if( clickedAreaName === 'right' ) break;
                        case 'bottom':
                            style.bottom = {
                                color: color, style: borderStyle
                            };
                            if( clickedAreaName === 'bottom' ) break;
                        case 'left':
                            style.left = {
                                color: color, style: borderStyle
                            };
                            break;
                    }

                    return style;
                }

                self._areas.children().add( self._selectionButtons.children() ).click(function( e ){

                    var area = $( this );
                    var areaClass = /^area (area_[a-z_]+)( (area_[a-z_]+))?$/.exec( area.attr( 'class' ) );
                    var parentState = /^pbawijspread_areas( ([a-z_]+))?( ([a-z_]+))?$/.exec( self._areas.attr( 'class' ) );

                    areaClass = areaClass && areaClass[ 3 ] || areaClass && areaClass[ 1 ] || undefined;
                    parentState = parentState[ 2 ] + ( parentState[ 3 ] || '' );

                    var clickedAreaName;

                    if( ! parentState || parentState === 'undefined' )
                        clickedAreaName = areaClass.replace( /area_([a-z]+)/, '$1' );
                    else if( parentState.indexOf( 'center_all' ) !== -1 )
                        clickedAreaName = 'center';
                    else if( parentState.indexOf( 'center' ) !== -1 )
                        clickedAreaName = parentState.replace( /center_([a-zA-Z]+)_hover/, '$1' );
                    else if( parentState.indexOf( 'bound' ) !== -1 )
                        clickedAreaName = 'bound';
                    else clickedAreaName = 'all';

                    // change modified state
                    (function(){
                        var modified = $( self ).data( 'modified' );
                        var modified_areas;

                        if( /center|all|bound/.test( clickedAreaName )){
                            if( clickedAreaName.indexOf( 'center' ) !== -1 ) 
                                modified_areas = [ 'vertical', 'horizontal' ];
                            else if ( clickedAreaName.indexOf( 'all' ) !== -1 )
                                modified_areas = [ 'top', 'right', 'bottom', 'left', 'horizontal', 'vertical' ];
                            else
                                modified_areas = [ 'top', 'right', 'bottom', 'left' ];
                        } else modified_areas = [ clickedAreaName ];

                        for( var i = 0; i < modified_areas.length; i++ )
                            modified[ modified_areas[i] ] = true;

                        $( self ).data( 'modified', modified );

                        self._buttonApply.removeClass( 'disabled' );
                    })();

                    self.applyStyle(
                        self._grid.sheet,
                        makeStyle( 
                            clickedAreaName,
                            (parentState.indexOf( 'empty' ) !== -1
                                ? 0
                                : self._dropdown.data( 'kendoDropDownList' ).value()),
                            self._palette.data( 'kendoColorPalette' ).value()
                        ),
                        areaClass || clickedAreaName);

                    e.preventDefault();
                });
            }
            function getData(){
                var modified = $( self ).data( 'modified' );
                var sheet = self._grid.sheet;
                var lCell = sheet.getCell( 1, 1 );
                var rCell = sheet.getCell( 2, 2 );
                var data = {};

                // only modified borders are applies to selection
                if( modified.top )          data.top = lCell.borderTop();
                if( modified.right )        data.right = rCell.borderRight();
                if( modified.bottom )       data.bottom = rCell.borderBottom();
                if( modified.left )         data.left = lCell.borderLeft();
                if( modified.vertical )     data.vertical = lCell.borderRight();
                if( modified.horizontal )   data.horizontal = lCell.borderBottom();

                return data;
            }
            function bindDialogButtons(){
                if( ! self._buttonCancel.data( 'isbinded' ))
                    self._buttonCancel.click(function( e ){

                        e.preventDefault();

                        $( this ).data( 'isbinded', true );

                        $( self ).trigger( 'cancel' );
                    });
                if( ! self._buttonApply.data( 'isbinded' ))
                    self._buttonApply.click(function( e ){

                        e.preventDefault();

                        $( this ).data( 'isbinded', true );

                        $( self ).trigger( 'apply', [ getData() ]);
                    });
            }

            this._grid.initCells = initCells;

            this._grid.height( this._grid.width() );
            self._grid.css( 'padding', self._grid.width() * .03 );

            initSpread();
            initSheet();
            initCells();
            initAreas();
            bindAreasHover();
            bindAreasClick();
            bindDialogButtons();
        },
        _initKendo: function(){
            var self = this;
            function initKendoDropDownList(){
                var data = [
                    { text: 'нет линии', value: 'empty' },
                    { text: '', value: 'thin' },
                    { text: '', value: 'medium' },
                    { text: '', value: 'thick' },
                    { text: '', value: 'double' },
                    { text: '', value: 'dashed' },
                    { text: '', value: 'mediumDashed' },
                    { text: '', value: 'dotted' }
                    // { text: '', value: 'dashDot' },
                    // { text: '', value: 'mediumDashDot' },
                    // { text: '', value: 'slantedDashDot' },
                    // { text: '', value: 'dashDotDot' },
                    // { text: '', value: 'mediumDashDotDot' },
                    // { text: '', value: 'hair' },
                ];

                self._dropdown.kendoDropDownList({
                    dataTextField: 'text',
                    dataValueField: 'value',
                    dataSource: data,
                    valueTemplate: '<div class="kendoDashTypeElement #:data.value#">#:data.text#</div>',
                    template: '<div class="kendoDashTypeElement #:data.value#">#:data.text#</div>',
                    index: 0
                });
            }
            function initKendoColorPalette(){
                self._palette.kendoColorPalette({
                    palette: [ '#000', '#999', '#00D', '#0D0', '#0DD', '#D00', '#E0E', '#FA0' ],
                    tileSize: 30,
                    value: '#000',
                });
            }

            initKendoDropDownList();
            initKendoColorPalette();
        }
    });

    $.fn.pbaSpread = function ( options ) {

        var self = this;

        this.wrapper = this;
        this.inner = null;
        this.undoManager = null;
        this.headerBar = null;
        this.positionBox = null;
        this.formulaBox = null;
        this.fileElement = null;
        this.kendoCalendar = null;
        this.opts = null;
        this.borderDialogWindow = null;
        this.borderDialog = null;

        this.spread = null;
        
        this.wijspread = null;
        this.sheetArea = null;
        this.viewport = null;

        this._getClickTarget = function( clientPosition ) {

            var sheet = self.spread.getActiveSheet();
            var offset = self.inner.offset();
            return sheet.hitTest(
                clientPosition.pageX - offset.left,
                clientPosition.pageY - offset.top
            );
        };

        // уточнить выделение в зависимости от
        // выделения до вызова контекстного меню
        // и места его вызова (перерисовка выделения)
        this._contextMenuResolveSelection = function( clientPosition ){
            
            var sheet = self.spread.getActiveSheet();
            var target = self._getClickTarget( clientPosition );
            var targetType = target.hitTestType;
            var selection = sheet.getSelections();

            if ( targetType === self.viewport ) {

                // click in viewport
                if ( sheet.getSelections().find( target.row, target.col ) === null ) {

                    // click on place which is not in current selection
                    sheet.clearSelection();
                    sheet.endEdit();
                    sheet.setActiveCell( target.row, target.col );
                }

            } else if ( targetType === self.sheetArea.colHeader ) {

                // click in column header - force select whole column
                sheet.setSelection( 0, target.col, sheet.getRowCount(), 1 );

            } else if ( targetType === self.sheetArea.rowHeader ) {

                // click in row header - force select whole row
                sheet.setSelection( target.row, 0, 1, sheet.getColumnCount() );

            } else if ( targetType === self.sheetArea.corner ) {

                // click on corner - force select whole table
                sheet.setSelection( 0, 0, sheet.getRowCount(), sheet.getColumnCount() );
            }
        };
        this._getActualCellRange = function( cellRange, rowCount, columnCount ) {
            if (cellRange.col == -1 && cellRange.row == -1) return new self.wijspread.Range(0, 0, columnCount, rowCount);
            else if (cellRange.col == -1) return new self.wijspread.Range(cellRange.row, 0, cellRange.rowCount, rowCount);
            else if (cellRange.row == -1) return new self.wijspread.Range(0, cellRange.col, columnCount, cellRange.colCount);
            return cellRange;
        };
        this._getSelectionMatrix = function( cellRange, rowCount, columnCount ){
            var sheet = self.spread.getActiveSheet();
            var matrix = [];
            var count = 0;

            for( var row = 0; row < rowCount; row++ )
                matrix[ row ] = new Array( columnCount );

            for( var n = 0; n < cellRange.length; n++ ){
                var sel = self._getActualCellRange( cellRange[n], rowCount, columnCount );
                for( var selrow = 0; selrow < sel.rowCount; selrow++ ){
                    for( var selcol = 0; selcol < sel.colCount; selcol++ ){
                        matrix[ sel.row + selrow ][ sel.col + selcol ] = true;
                        count++;
                    }
                }
            }
            matrix.count = count;

            return matrix;
        };
        this._hideLoading = function() {
            $( '#loaderDiv_' + this.opts.GUID ).remove();
            $( '#loaderSpan_' + this.opts.GUID ).remove();
        };
        this._init = function ( parameters ) {
            this.opts = {
                // user can redefine this value (string)
                GUID: newGUID(),

                sheets: 1,
                rows: 100,
                columns: 100,

                positionBox: true,
                formulaBox: true,

                kendo: {
                    calendar: true,
                    contextMenu: true,
                    borderDialog: true
                },

                // url to jquery.wijspread.js
                wijspreadJsUrl: '',

                // links to dom objects
                // to auto-execute appropriate action
                // after click on that object
                buttons: {
                    clear: null,
                    cut: null,
                    copy: null,
                    paste: null,
                    borderDialog: null,
                    bold: null,
                    italic: null,
                    underline: null,
                    crossline: null,
                    overline: null,
                    alignLeft: null,
                    alignCenter: null,
                    alignRight: null,
                    indent: null,
                    outdent: null,
                    'add-column-before': null,
                    'add-column-after': null,
                    'add-row-before': null,
                    'add-row-after': null,
                    'delete-column': null,
                    'delete-row': null,
                    undo: null,
                    redo: null,
                    print: null,
                    importFile: null

                    // ...
                },
                events: {
                    onImportComplete: function () { },
                    onError: function () { }
                },

                importServiceUrl: ''
            };

            if ( parameters && typeof parameters === 'object' )
                $.extend( this.opts, parameters );
            else if ( parameters && typeof parameters === 'string' )
                this.opts.wijspreadJsUrl = parameters;

            if (this.opts.wijspreadJsUrl)
                this._loadScript( this.opts.wijspreadJsUrl );

            this.wijspread = $.wijmo.wijspread;
            this.sheetArea = $.wijmo.wijspread.SheetArea;
            this.viewport = $.wijmo.wijspread.SheetArea.viewport;

            function _domInitialization() {

                self.addClass( 'pbawijspread_wrapper' );
                self.inner = $('<div class="pbawijspread_inner"></div>')
                    .appendTo( self )
                    .attr( 'id', 'ss_' + self.opts.GUID );

                if ( self.opts.formulaBox || self.opts.positionBox ) {

                    self.headerBar = $( '<div class="pbawijspread_headerBar"></div>' ).prependTo( self );

                    if ( self.opts.formulaBox )
                        self.formulaBox = $( '<div class="pbawijspread_formulaBox" contenteditable="true" spellcheck="false"></div>' ).appendTo( self.headerBar );

                    if ( self.opts.positionBox )
                        self.positionBox = $( '<input class="pbawijspread_positionBox" type="text" disabled="disabled" />' ).prependTo( self.headerBar );
                }

                if ( self.opts.importServiceUrl !== '' ) {

                    self.fileElement = $( '<input class="pbawijspread_fileElement" type="file" accept=".xlsx,.xls" />' ).appendTo( self );
                }

                if ( self.opts.kendo.calendar || self.opts.kendo.contextMenu || self.opts.kendo.borderDialog ) {

                    if ( $.fn.kendoCalendar ) {

                        self.kendoCalendar = $( '<div class="pbawijspread_kendoCalendar"></div>' )
                            .appendTo( $body )
                            .kendoCalendar({
                                value: new Date(),
                                change: function(){

                                    self.kendoCalendar.trigger( 
                                        'dateIsChanged', this
                                    ).css({
                                        left: '',
                                        top: ''
                                    }).removeClass( 'active' );
                                }
                            });
                    } else {

                        console.warn( 'pbaSpread: Kendo is not loaded for kendoCalendar init!' );
                    }

                    if ( $.fn.kendoContextMenu ) {

                        self.kendoContextMenu = $( '<ul class="pbawijspread_kendoContextMenu"></ul>' )
                            .appendTo( self )
                            .append( '<li id="context_menu_action-cut"><span class="glyphicon glyphicon-scissors"></span>вырезать</li>' )
                            .append( '<li id="context_menu_action-copy"><span class="glyphicon glyphicon-file-export"></span>копировать</li>' )
                            .append( '<li id="context_menu_action-paste"><span class="glyphicon glyphicon-file-import"></span>вставить</li>' )

                            .append( '<li class="k-separator"></li>' )
                            .append( '<li><span class="glyphicon glyphicon-plus"></span>добавить колонку\
                                        <ul>\
                                            <li id="context_menu_action-add-column-before"><span class="halfling halfling-chevron-left"></span>слева</li>\
                                            <li id="context_menu_action-add-column-after"><span class="halfling halfling-chevron-right"></span>справа</li>\
                                        </ul>\
                                      </li>' )
                            .append( '<li id="context_menu_action-delete-column"><span class="glyphicon glyphicon-minus"></span>удалить колонку</li>' )
                            
                            .append( '<li class="k-separator"></li>' )
                            .append( '<li><span class="glyphicon glyphicon-plus"></span>добавить строку\
                                        <ul>\
                                            <li id="context_menu_action-add-row-before"><span class="halfling halfling-chevron-up"></span>сверху</li>\
                                            <li id="context_menu_action-add-row-after"><span class="halfling halfling-chevron-down"></span>снизу</li>\
                                        </ul>\
                                      </li>' )
                            .append( '<li id="context_menu_action-delete-row"><span class="glyphicon glyphicon-minus"></span>удалить строку</li>' )

                            .kendoContextMenu({
                                orientation: 'vertical',
                                target: '#ss_' + self.opts.GUID,
                                select: function( e ){

                                    var pattern = /context_menu_action-([a-zA-Z_\-]+)/;

                                    if ( ! e.item.id || ! pattern.test( e.item.id ) ) {

                                        console.warn( 'This context menu action is not implemented.' );
                                        return;
                                    }

                                    self.action( pattern.exec( e.item.id )[ 1 ] );
                                },
                                open: function( e ){

                                    if ( e.event ) {

                                        // context menu is opened ( and it is NOT child element )
                                        var openedAt = { 
                                            pageX: e.event.clientX,
                                            pageY: e.event.clientY
                                        };

                                        self.kendoContextMenu.isOpened = true;

                                        self._contextMenuResolveSelection( openedAt );
                                    }
                                },
                                close: function( e ){

                                    if ( e.event ) {

                                        // context menu is closed ( and it is NOT child element )
                                        self.kendoContextMenu.isOpened = false;
                                    }
                                }
                            });

                        self.kendoContextMenu.isOpened = false;
                        self.inner.click(function(){

                            self.kendoContextMenu.isOpened && self.kendoContextMenu.data('kendoContextMenu').close();
                        });

                    } else {

                        console.warn( 'pbaSpread: Kendo is not loaded for kendoContextMenu init!' );
                    }

                    if ( $.fn.kendoWindow ){

                        self.borderDialogWindow = $( '<div class="pbawijspread_borderDialogWindow"></div>' ).appendTo( self );
                        self.borderDialog = new SpreadBorderDialog( self.borderDialogWindow );
                        self.borderDialogWindow.kendoWindow({
                            width: 350,
                            height: 'auto',
                            modal: true,
                            visible: false
                        });
                        
                    } else {

                        console.warn( 'pbaSpread: Kendo is not loaded for kendoWindow init!' );
                    }
                }
            }

            function _wijspreadFieldInitialization() {

                try {

                    self.inner.wijspread({ sheetCount: self.opts.sheets });
                    self.spread = self.inner.wijspread( 'spread' );

                } catch ( e ) {

                    console.error( 'spread init error: ' + e.message );
                    return false;
                }

                if ( ! self.spread ) {

                    console.error( 'spread init error.' );
                    return false;
                }

                self.undoManager = self.spread.undoManager();

                self.setSheetSize( self.opts.columns, self.opts.rows );
            }

            function _wijspreadAdditionalFieldsInitialization() {

                var needUpdate = false;

                if ( self.positionBox ) {

                    self.spread.bind( 'EnterCell', self._selectionChanged );
                    self.positionBox.val( 'A1' );

                    needUpdate = true;
                }

                if ( self.formulaBox ) {

                    var fbx = new self.wijspread.FormulaTextBox( self.formulaBox[ 0 ] );
                    fbx.spread( self.spread );

                    needUpdate = true;
                }

                return needUpdate;
            }

            function _buttonBindingsInitialization() {

                for ( var name in self.opts.buttons ) {
                    (function(){

                        var _name = name;
                        var _button = self.opts.buttons[ _name ];
                        if ( _button ) {
                            _button.click(function(){ 
                                
                                self.action( _name ) });
                        }
                    })();
                }
            }

            function _otherBindingsInitialization() {

                // kendoCalendar initialization
                if ( self.kendoCalendar ) {

                    self.spread.bind( 'EditStarted', function( event, data ){

                        var cell = self.spread.getActiveSheet().getCell( data.row, data.col );
                        var kc = self.kendoCalendar;

                        if ( cell.value() instanceof Date ) {

                            var offset = self.inner.offset();
                            kc.css({
                                left: offset.left + ( self.inner.width() / 2 ) - ( kc.width() / 2 ),
                                top: offset.top + ( self.inner.height() / 2 ) - ( kc.height() / 2 )
                            }).addClass(
                                'active'
                            ).one( 
                                'dateIsChanged', function( event2, data2 ){

                                    cell.value( data2._value );

                                    self.formulaBox.find( 
                                        '.gcsj-func-color-text' 
                                    ).text( data2._value.toString().substring(0, 16) );

                                    $( document ).unbind( 'click', _documentClickHandlerClosure );

                            }).data( 'kendoCalendar' ).value( cell.value() );
                        }

                        $( document ).click( _documentClickHandlerClosure );

                        function _documentClickHandlerClosure( event ){

                            var $t = $( event.target );

                            if ( ! $t.hasClass( 'pbawijspread_kendoCalendar' ) || ! $t.closest( '.pbawijspread_kendoCalendar' ).length ){

                                $( document ).unbind( 'click', _documentClickHandlerClosure );
                                self.kendoCalendar.unbind( 
                                    'dateIsChanged'
                                ).removeClass( 'active' );
                            }
                        }
                    });
                }

                if ( self.opts.buttons.undo || self.opts.buttons.redo )
                    $( self.undoManager ).change( self._undoManagerStatementChanged );
            }


            _domInitialization();

            _wijspreadFieldInitialization();

            _wijspreadAdditionalFieldsInitialization() && self.update();

            _buttonBindingsInitialization();

            _otherBindingsInitialization();

            return self;
        };
        this._loadScript = function( path ) {
            $.ajax({
                url: path,
                dataType: 'script',
                async: false
            });
        };
        this._onImportError = function( error ) {

            alert( error );
        };
        this._onImportSuccess = function( responseText ) {

            var spreadJson;
            
            try {
                // TODO: Временно! Убрать замену по регулярке.
                spreadJson = JSON.parse( responseText.replace( /\n/g, '' ));
            }
            catch ( e ) {
                self.opts.events.onError( 'Ошибка при парсинге данных таблицы!' );
                self.action( 'clear' );
                return;
            }

            if ( spreadJson.spread ) {

                var sheet;
                var color;
                var fixed;

                self.spread.fromJSON( spreadJson.spread );

                sheet = self.spread.getActiveSheet();

                /* selection color fix */
                function rgbaFix( colorString ) {

                    if ( /^rgba[\(\)0-9\,\.]+$/.test( colorString )) {

                        var values = colorString.split( ',' );
                        if ( values.length > 4 ) {

                            values[ 3 ] += '.' + values[ 4 ];
                            values.length = 4;

                            return values.join( ',' );
                        }
                    }

                    return null;
                }

                // back color
                color = sheet.selectionBackColor();
                fixed = rgbaFix(color);
                if (color !== fixed) sheet.selectionBackColor(fixed);

                // border color
                color = sheet.selectionBorderColor();
                fixed = rgbaFix(color);
                if (color !== fixed) sheet.selectionBorderColor(fixed);
                /* end of fix */

                this.setSheetSize(50, 200, sheet);
            } else {
                /* spreadJson.spread == null */

                this._onImportError(( spreadJson.error || 'unknown import error' ));
            }
        };
        this._selectionChanged = function( sender, args ) {

            var sheet = self.spread.getActiveSheet();
            var position = sheet.getText( 0, sheet.getActiveColumnIndex(), self.sheetArea.colHeader ) +
                sheet.getText( sheet.getActiveRowIndex(), 0, self.sheetArea.rowHeader );

            if (self.positionBox) self.positionBox.val( position );
        };
        this._showLoading = function( callback ) {

            this.inner.css( 'position', 'relative' );
            var width = this.inner.width() + 2,
                height = this.inner.height();

            $( '<span id="loaderSpan_' + this.opts.GUID + '"><span style="display:inline-block;"></span>Загрузка...</span>' )
                .css({
                    left: width / 2 - 70,
                    top: height / 2 - 30,
                    position: 'absolute',
                    color: '#4f4f4f',
                    background: '#fff',
                    border: '1px solid #a8a8a8',
                    borderRadius: '3px',
                    '-webkit-border-radius': '3px',
                    'box-shadow': '0 0 10px rgba(0,0,0,.25)',
                    fontFamily: 'Arial, sans-serif',
                    fontSize: '20px',
                    padding: '.4em'
                })
                .appendTo( this.inner );
            $( '<div id="loaderDiv_' + this.opts.GUID + '"></div>' )
                .css({
                    background: '#2d5972',
                    opacity: .3,
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: width,
                    height: height
                })
                .appendTo( this.inner );
        };
        this._fillReflect = function( sheet, selectionMatrix, variousStyle, rowCount, colCount ){
            var dirs = [
                'top', 'right', 'bottom', 'left', 'horizontal', 'vertical'
            ];
            var reflectData = {};
            var tempStyle;

            // fill reflectData with nulls
            for( var i = 0; i < dirs.length; i++ )
                reflectData[ dirs[i] ] = null;

            for( var row = 0; row < rowCount; row++ ){
                for( var col = 0; col < colCount; col++ ){
                    if( selectionMatrix[ row ][ col ] ){
                        for( var dir = 0; dir < dirs.length; dir++ ){
                            var DIR = dirs[ dir ];
                            var BORDER = 'border' + DIR.charAt(0).toUpperCase() + DIR.substr(1);
                            var cur_refl = reflectData[ DIR ];
                            if(
                                DIR === 'top' && ( !row || !selectionMatrix[ row-1 ][ col ] ) ||
                                DIR === 'right' && ( col === colCount-1 || !selectionMatrix[ row ][ col+1 ] ) ||
                                DIR === 'bottom' && ( row === rowCount-1 || !selectionMatrix[ row+1 ][ col ] ) ||
                                DIR === 'left' && ( !col || !selectionMatrix[ row ][ col-1 ] ) ||
                                DIR === 'horizontal' && selectionMatrix.count > 1 && ( row < rowCount-1 && selectionMatrix[ row+1 ][ col ] ) ||
                                DIR === 'vertical' && selectionMatrix.count > 1 && ( col < colCount-1 && selectionMatrix[ row ][ col+1 ] )
                                ){

                                if( DIR.charAt(0) === 'h' || DIR.charAt(0) === 'v' )
                                    BORDER = 'border' + ( DIR === 'horizontal' ? 'Bottom' : 'Right' );

                                if( cur_refl === null ){

                                    reflectData[ DIR ] = sheet.getCell( row, col )[ BORDER ]();

                                } else if ( cur_refl === undefined || !cur_refl.various ){

                                    tempStyle = sheet.getCell( row, col )[ BORDER ]();

                                    if( cur_refl !== tempStyle ){

                                        if( !cur_refl && tempStyle || cur_refl && !tempStyle 
                                            || 
                                            ( cur_refl.color !== tempStyle.color || cur_refl.style !== tempStyle.style )){

                                            reflectData[ DIR ] = { various: true };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for( var name in reflectData ){
                if( reflectData.hasOwnProperty( name )){

                    if( !reflectData[ name ])
                        delete reflectData[ name ];

                    else if( reflectData[ name ].various )
                        reflectData[ name ] = variousStyle;
                }
            }

            return reflectData;
        };
        this.action = function( command ) {
            if ( ! this.spread ) {

                console.error( 'spread is not initialized!' );
                return null;
            }

            var self = this;
            var sheet = this.spread.getActiveSheet();
            var sels = sheet.getSelections();
            var rowCount = sheet.getRowCount();
            var columnCount = sheet.getColumnCount();

            switch ( command ) {

                case 'clear':
                    self.destroy();
                    self._init( options );
                    break;
                case 'cut':
                case 'copy':
                case 'paste':
                    self.wijspread.SpreadActions[ command ].call( sheet );
                    break;
                case 'borderDialog':

                    self.borderDialogWindow.data( 'kendoWindow' ).open().center();
                    self.borderDialog.reset();

                    var variousStyle = new $.wijmo.wijspread.LineBorder( 'rgba(0,0,0,0.2)', 10 );
                    var selMatrix = self._getSelectionMatrix( sels, rowCount, columnCount );
                    var reflectData = self._fillReflect( sheet, selMatrix, variousStyle, rowCount, columnCount );

                    self.borderDialog.reflect( reflectData );

                    $( self.borderDialog )
                        .unbind( 'cancel' )
                        .on( 'cancel', function(){

                        self.borderDialogWindow.data( 'kendoWindow' ).close();

                    });

                    $( self.borderDialog )
                        .unbind( 'apply' )
                        .on( 'apply', function( e, data ){

                        var selMatrix = self._getSelectionMatrix( sels, rowCount, columnCount );

                        sheet.isPaintSuspended( true );

                        for( var row = 0; row < rowCount; row++ ){
                            for( var col = 0; col < columnCount; col++ ){

                                if( ! selMatrix[ row ][ col ] ) continue;

                                if( data.hasOwnProperty( 'top' )){

                                    if( row === 0 || ! selMatrix[ row - 1 ][ col ] ){

                                        sheet.getCell( row, col ).borderTop( data.top );
                                        if( row > 0 ){

                                            sheet.getCell( row - 1, col ).borderBottom( data.top );

                                        }
                                    }

                                } 
                                if ( data.hasOwnProperty( 'right' )){

                                    if( col === columnCount - 1 || ! selMatrix[ row ][ col + 1 ] ){

                                        sheet.getCell( row, col ).borderRight( data.right );
                                        if( col < columnCount - 1 ){

                                            sheet.getCell( row, col + 1 ).borderLeft( data.right );

                                        }
                                    }

                                }
                                if ( data.hasOwnProperty( 'bottom' )){
                                    
                                    if( row === rowCount - 1 || ! selMatrix[ row + 1 ][ col ] ){

                                        sheet.getCell( row, col ).borderBottom( data.bottom );
                                        if( row < rowCount - 1 ){

                                            sheet.getCell( row + 1, col ).borderTop( data.bottom );

                                        }
                                    }

                                }
                                if ( data.hasOwnProperty( 'left' )){
                                    
                                    if( col === 0 || ! selMatrix[ row ][ col - 1 ] ){

                                        sheet.getCell( row, col ).borderLeft( data.left );
                                        if( col > 0 ){

                                            sheet.getCell( row, col - 1 ).borderRight( data.left );

                                        }
                                    }

                                }
                                if ( data.hasOwnProperty( 'horizontal' )){

                                    if( row < rowCount - 1 && selMatrix[ row + 1 ][ col ] ){

                                        sheet.getCell( row, col ).borderBottom( data.horizontal );
                                        sheet.getCell( row + 1, col ).borderTop( data.horizontal );

                                    }

                                }
                                if ( data.hasOwnProperty( 'vertical' )){
                                    
                                    if( col < columnCount - 1 && selMatrix[ row ][ col + 1 ] ){

                                        sheet.getCell( row, col ).borderRight( data.vertical );
                                        sheet.getCell( row, col + 1 ).borderLeft( data.vertical );

                                    }                                    

                                }
                            }
                        }

                        self.borderDialogWindow.data( 'kendoWindow' ).close();

                        sheet.isPaintSuspended( false );

                    });
                    break;
                case 'bold':
                case 'italic':
                    var style = document.createElement('DIV').style;
                    var font = sheet.getCell( sheet.getActiveRowIndex(), sheet.getActiveColumnIndex(), self.viewport ).font();
                    var propertyName = command === 'bold' ? 'fontWeight' : 'fontStyle';

                    sheet.isPaintSuspended( true );

                    if ( font != undefined ) style.font = font;
                    else style.font = '10pt Arial';

                    style[ propertyName ] = style[ propertyName ] === command ? '' : command;

                    for ( var n = 0; n < sels.length; n++ ) {

                        var sel = self._getActualCellRange( sels[n], rowCount, columnCount );
                        sheet.getCells( sel.row, sel.col, sel.row + sel.rowCount - 1, sel.col + sel.colCount - 1, self.viewport ).font( style.font );
                    }

                    sheet.isPaintSuspended( false );

                    break;
                case 'underline':
                case 'crossline':
                case 'overline':
                    var decorSysName =
                        command === 'underline' || command === 'overline'
                            ? ( command.charAt(0).toUpperCase() + command.substring(1) )
                            : 'LineThrough';
                    var decor = self.wijspread.TextDecorationType[ decorSysName ];

                    sheet.isPaintSuspended( true );

                    for ( var n = 0; n < sels.length; n++ ) {

                        var sel = self._getActualCellRange( sels[n], rowCount, columnCount );
                        var textDecoration = sheet.getCell( sel.row, sel.col, self.viewport ).textDecoration();
                        if ( (textDecoration & decor) === decor ) 
                            textDecoration = textDecoration - decor;
                        else 
                            textDecoration = textDecoration | decor;

                        sheet.getCells( sel.row, sel.col, sel.row + sel.rowCount - 1, sel.col + sel.colCount - 1, self.viewport ).textDecoration( textDecoration );
                    }

                    sheet.isPaintSuspended( false );

                    break;
                case 'alignLeft':
                case 'alignCenter':
                case 'alignRight':
                    var align = self.wijspread.HorizontalAlign[ command.replace('align', '').toLowerCase() ];

                    sheet.isPaintSuspended( true );

                    for ( var n = 0; n < sels.length; n++ ) {

                        var sel = self._getActualCellRange( sels[n], rowCount, columnCount );
                        sheet.getCells( sel.row, sel.col, sel.row + sel.rowCount - 1, sel.col + sel.colCount - 1, self.viewport ).hAlign( align );
                    }

                    sheet.isPaintSuspended( false );

                    break;
                case 'indent':
                case 'outdent':
                    var offset = command === 'indent' ? 1 : -1;

                    sheet.isPaintSuspended( true );

                    for ( var n = 0; n < sels.length; n++ ) {

                        var sel = self._getActualCellRange( sels[n], rowCount, columnCount );

                        for ( var i = 0; i < sel.rowCount; i++ ) {
                            for ( var j = 0; j < sel.colCount; j++ ) {

                                var cell = sheet.getCell( i + sel.row, j + sel.col, self.viewport );
                                var indent = cell.textIndent();

                                if ( isNaN(indent) )
                                    indent = 0;

                                cell.textIndent(indent + offset);
                            }
                        }
                    }
                    sheet.isPaintSuspended( false );

                    break;
                case 'add-column-before':
                case 'add-column-after':
                case 'add-row-before':
                case 'add-row-after':
                case 'delete-column':
                case 'delete-row':
                    var commandSplit = command.split('-');
                    var offset = commandSplit[ 0 ] === 'delete' || commandSplit[ 2 ] === 'before' ? 0 : 1;
                    var item = commandSplit[ 1 ][ 0 ].toUpperCase() + commandSplit[ 1 ].substring(1);
                    var methodName = commandSplit[ 0 ] + item + 's';

                    sheet[ methodName ]( sheet[ 'getActive'+item+'Index' ]() + offset, 1 );
                    break;
                case 'undo':
                case 'redo':
                    if ( self.undoManager[ 'can' + command[ 0 ].toUpperCase() + command.substr( 1 ) ]() )
                        self.undoManager[ command ]();
                    break;
                case 'print':
                    if ( window.print ) {

                        var heightBefore = self.inner.height();
                        var $parentBefore = self.inner.parent();
                        var rowCountBefore = rowCount;
                        var columnCountBefore = columnCount;

                        // BEFORE PRINT DIALOG
                        sheet.isPaintSuspended( true );

                        self.inner.appendTo( $body )
                            .width( 1500 ).height( 1500 )
                            .css({
                                position: 'absolute',
                                top: 0,
                                left: 0,
                                zIndex: 999999999999
                            })
                            .addClass( 'pbawijspread_print' );

                        sheet.setGridlineOptions({
                            showHorizontalGridline: false,
                            showVerticalGridline: false
                        });
                        self.spread.showHorizontalScrollbar( false );
                        self.spread.showVerticalScrollbar( false );
                        self.spread.tabStripVisible( false );
                        sheet.setColumnHeaderVisible( false );
                        sheet.setRowHeaderVisible( false );
                        sheet.clearSelection();

                        self.inner.wijspread( 'refresh' );

                        self.setSheetSize( 50, 200 );

                        sheet.isPaintSuspended( false );

                        window.print();

                        // AFTER PRINT DIALOG
                        sheet.isPaintSuspended( true );

                        self.inner.appendTo( $parentBefore )
                            .removeAttr( 'style' )
                            .height( heightBefore )
                            .removeClass( 'pbawijspread_print' );

                        sheet.setGridlineOptions({
                            showHorizontalGridline: true,
                            showVerticalGridline: true
                        });
                        self.spread.showHorizontalScrollbar( true );
                        self.spread.showVerticalScrollbar( true );
                        self.spread.tabStripVisible( true );
                        sheet.setColumnHeaderVisible( true );
                        sheet.setRowHeaderVisible( true );

                        sheet.setColumnCount( columnCountBefore );
                        sheet.setRowCount( rowCountBefore );

                        self.inner.wijspread( 'refresh' );

                        sheet.isPaintSuspended( false );

                    } else {

                        // window.print === undefined
                        console.warn( 'This browser doesn\'t provide print-feature...' );
                    }
                    break;
                case 'importFile':
                    self.importFile();
                    break;
                default:
                    console.warn( 'action ' + command + ' is not implemented.' );
                    break;
            }
        };
        this.destroy = function() {

            if ( self.headerBar ) {

                if ( self.positionBox ) {

                    self.spread.unbind( 'EnterCell', self._selectionChanged );
                    self.positionBox = null;
                }

                if ( self.formulaBox ) {

                    self.spread._attachedFormulaTextBox.destroy();
                    self.formulaBox = null;
                }

                self.headerBar.remove();
            }

            if ( self.borderDialogWindow ) {

                self.borderDialogWindow.data( 'kendoWindow' ).destroy();
                self.borderDialog.destroy();
                self.borderDialogWindow.remove();
            }

            try { self.inner.wijspread( 'destroy' ); }
            catch(e) { /* EMPTY */ }

            // buttons unbinding
            for ( var name in self.opts.buttons ) {

                var button = self.opts.buttons[ name ];

                if( button && button.off )
                    button.off( 'click' );

            }

            if ( self.fileElement ) self.fileElement.remove();

            self.inner.remove();

            self.removeClass('pbawijspread_wrapper');

            self.data( 'pbaSpread', null );
        };
        this.fromJSON = function( obj ) {

            if ( this.spread ) this.spread.fromJSON( obj );

            return this;
        };
        this.importFile = function() {
            
            var formData;

            this.fileElement[ 0 ].files.length = 0;

            if ( this.opts.importServiceUrl != '' ) {

                $( this.fileElement ).one( 'change', function(){

                    if ( self.fileElement[ 0 ].files.length ) {

                        self._showLoading();

                        formData = new FormData();
                        formData.append( 'file', self.fileElement[ 0 ].files[ 0 ] );

                        $.ajax({
                            url: self.opts.importServiceUrl,
                            type: 'POST',
                            success: function( data, textStatus, jqXHR ){ 
                                
                                self._onImportSuccess( jqXHR.responseText ) 
                                self._hideLoading();
                                self.fileElement[ 0 ].value = '';
                                self.opts.events.onImportComplete();

                            },
                            error: function( jqXHR, textStatus, errorThrown ){ 

                                self._onImportError( errorThrown ) 
                                self._hideLoading();
                                self.fileElement[ 0 ].value = '';
                                self.opts.events.onImportComplete();
                            },
                            data: formData,
                            async: false,
                            cache: false,
                            contentType: false,
                            processData: false,
                            headers: { Accept: 'application/json' }
                        });
                    }
                });

                this.fileElement.trigger( 'click' );
            }
        };
        this.setSheetSize = function( columns, rows ) {

            var sheet = self.spread.getActiveSheet();
            var columnCount = sheet.getColumnCount();
            var rowCount = sheet.getRowCount();
            columns > columnCount && sheet.setColumnCount( columns, self.viewport );
            rows > rowCount && sheet.setRowCount( rows, self.viewport );
        };
        this.toJSON = function() {

            return this.spread
                ? this.spread.toJSON()
                : null;
        };
        this.update = function() {

            if ( ! this.spread ) {

                console.error( 'spread is not initialized!' );
                return null;
            }

            var newHeight = this.height();

            if ( this.headerBar )
                newHeight -= this.headerBar.outerHeight();

            this.inner
                .height( newHeight )
                .wijspread( 'refresh' );

            if ( this.borderDialog )
                this.borderDialog.update();

            return this;
        };

        this._init( options );

        this.data( 'pbaSpread', this );

        return this;
    };

})( jQuery );
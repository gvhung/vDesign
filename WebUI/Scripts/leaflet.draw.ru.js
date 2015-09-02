L.drawLocal = {
    draw: {
        toolbar: {
            actions: {
                title: 'Отменить рисование',
                text: 'Отмена'
            },
            undo: {
                title: 'Удалить последнюю точку',
                text: 'Удалить последнюю точку'
            },
            buttons: {
                polyline: 'Ломанная',
                polygon: 'Полигон',
                rectangle: 'Квадрат',
                circle: 'Круг',
                marker: 'Точка'
            }
        },
        handlers: {
            circle: {
                tooltip: {
                    start: 'Click and drag to draw circle.'
                }
            },
            marker: {
                tooltip: {
                    start: 'Нажмите на карту, чтобы отметить точку'
                }
            },
            polygon: {
                tooltip: {
                    start: 'Нажмите для начала рисования',
                    cont: 'Нажмите для продолжения рисования',
                    end: 'Соедените с первой точкой для завершения'
                }
            },
            polyline: {
                error: '<strong>Error:</strong> shape edges cannot cross!',
                tooltip: {
                    start: 'Нажмите для начала рисования',
                    cont: 'Нажмите для продолжения рисования',
                    end: 'Соедените с последней точкой для завершения'
                }
            },
            rectangle: {
                tooltip: {
                    start: 'Click and drag to draw rectangle.'
                }
            },
            simpleshape: {
                tooltip: {
                    end: 'Release mouse to finish drawing.'
                }
            }
        }
    },
    edit: {
        toolbar: {
            actions: {
                save: {
                    title: 'Сохранить изменения',
                    text: 'Сохранить'
                },
                cancel: {
                    title: 'Отменить все изменения',
                    text: 'Отменить'
                }
            },
            buttons: {
                edit: 'Изменить объект',
                editDisabled: 'Нет объектов для изменения',
                remove: 'Удалить объекты',
                removeDisabled: 'Нет объектов для изменения'
            }
        },
        handlers: {
            edit: {
                tooltip: {
                    text: 'Переместите для редактирования',
                    subtext: 'Нажмите отменить для отмены'
                }
            },
            remove: {
                tooltip: {
                    text: 'Нажмите на объект для удаления'
                }
            }
        }
    }
};
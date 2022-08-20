//New dialog window
var BinkeDialog = new function () {

    var todo = null;

    var isSubModal = false;

    function getCustomDialog(title) {
        var naDialog = $("#rw-dialog");
        if (naDialog.length === 0) {
            naDialog = $('<div id="rw-dialog" class="modal fade" style="z-index: 99999;"></div>');
            naDialog.append('<div class="modal-dialog"><div class="modal-content"></div></div>');
            var naDialogHeader = $('<div class="modal-header bg-primary" />').appendTo($(".modal-content", naDialog));
            naDialogHeader.append('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
            naDialogHeader.append('<h4 class="modal-title">RealNex Dialog</h4>');

            $('<div class="modal-body"><p class="message-placeholder"></p></div>').appendTo($('.modal-content', naDialog));
            //$('<div class="modal-footer dialog-overlay-toolbar" />').appendTo($('.modal-content', naDialog));
            naDialog.appendTo("body");

            naDialog.on("hidden.bs.modal", function (e) {
                var $backgroundFrame = $("#backgroundFrame");
                if ($backgroundFrame.length > 0) {
                    $backgroundFrame.hide();
                }

                $(".message-placeholder", e.currentTarget).empty();
                $(".dialog-overlay-toolbar", e.currentTarget).empty();

                if (isSubModal) {
                    if (!$("body").hasClass("modal-open")) {
                        $("body").addClass("modal-open");
                    }
                }

                if (todo != null) {
                    todo();
                }

                todo = null;
            });

            naDialog.on("shown.bs.modal", function (e) {
                naDialog.modal("handleUpdate");
                var $backgroundFrame = $("#backgroundFrame");
                if ($backgroundFrame.length > 0) {
                    $backgroundFrame.show();
                }
            });
        }

        if (!$(".modal-footer.dialog-overlay-toolbar", naDialog).length)
            $('<div class="modal-footer dialog-overlay-toolbar" />').appendTo($(".modal-content", naDialog));

        $(".message-placeholder", naDialog).css("height", "");
        $(".modal-dialog", naDialog).css("width", "");
        $(".modal-body", naDialog).css("height", "");

        isSubModal = $("body").hasClass("modal-open");

        $(".modal-title", naDialog).html(title);

        return naDialog;
    }

    function getCustomAlertDialog(title, msg, alertCallBack) {
        var rwAlertDialog = getCustomDialog(title);

        $('.message-placeholder', rwAlertDialog).html(msg);

        $('.dialog-overlay-toolbar', rwAlertDialog).empty();
        $('.dialog-overlay-toolbar', rwAlertDialog).append('<button type="button" class="btn btn-primary ok-btn"><strong>Ok</strong></button>');

        $('.ok-btn', rwAlertDialog).unbind('click').click(function () {
            rwAlertDialog.modal('hide');
            if (typeof alertCallBack === "function") {
                todo = alertCallBack;
            }
        });

        return rwAlertDialog;
    };

    function getCustomWarningDialog(msg) {
        var rwWarningDialog = getCustomDialog('Warning');

        $('.message-placeholder', rwWarningDialog).html(msg);

        $('.dialog-overlay-toolbar', rwWarningDialog).empty();
        $('.dialog-overlay-toolbar', rwWarningDialog).append('<button class="btn btn-primary ok-btn"><strong>Ok</strong></button>');

        $('.ok-btn', rwWarningDialog).unbind('click').click(function () {
            rwWarningDialog.modal('hide');
        });

        return rwWarningDialog;
    };

    function getCustomErrorDialog(msg) {
        var rwErrorDialog = getCustomDialog('Error');

        $('.message-placeholder', rwErrorDialog).html(msg);

        $('.dialog-overlay-toolbar', rwErrorDialog).empty();
        $('.dialog-overlay-toolbar', rwErrorDialog).append('<button type="button" class="btn btn-primary ok-btn"><strong>Ok</strong></button>');

        $('.ok-btn', rwErrorDialog).unbind('click').click(function () {
            rwErrorDialog.modal('hide');
        });

        return rwErrorDialog;
    };

    function getCustomConfirmationDialog(title, msg, yesCallBack, noCallBack) {

        var rwConfirmationDialog = getCustomDialog(title != null ? title : 'Warning');

        $('.message-placeholder', rwConfirmationDialog).html(msg);

        $('.dialog-overlay-toolbar', rwConfirmationDialog).empty();
        $('.dialog-overlay-toolbar', rwConfirmationDialog).append('<button type="button" class="btn btn-primary yes-btn"><strong>Yes</strong></button>');
        $('.dialog-overlay-toolbar', rwConfirmationDialog).append('<button type="button" class="btn btn-default no-btn"><strong>No</strong></button>');

        $('.yes-btn', rwConfirmationDialog).unbind('click').click(function () {
            if (typeof yesCallBack === "function") {
                todo = yesCallBack;
            }
            $(this).closest('.modal').modal('hide');
            return false;
        });
        $('.no-btn', rwConfirmationDialog).unbind('click').click(function () {
            if (typeof noCallBack === "function") {
                todo = noCallBack;
            }
            $(this).closest('.modal').modal('hide');
            return false;
        });

        return rwConfirmationDialog;
    };

    function getRwChoiceDialog(title, msg, buttons, callbacks) {

        var rwConfirmationDialog = getCustomDialog(title != null ? title : 'Warning');

        $('.message-placeholder', rwConfirmationDialog).html(msg);

        $('.dialog-overlay-toolbar', rwConfirmationDialog).empty();

        for (var i = 0; i < buttons.length; i++) {
            var btn = $('<button type="button" class="btn btn-primary"><strong>' + buttons[i] + '</strong></button>');
            $('.dialog-overlay-toolbar', rwConfirmationDialog).append(btn);
            btn.data('clickCallBack', callbacks[i]);

            btn.unbind('click').click(function () {
                var clbk = $(this).data('clickCallBack');
                if (typeof clbk === "function") {
                    todo = function () { clbk(); };
                }

                $(this).closest('.modal').modal('hide');

                return false;
            });
        }

        return rwConfirmationDialog;
    };

    function getRwCustomChoiceDialog(title, msg, controls) {
        var rwConfirmationDialog = getCustomDialog(title != null ? title : 'Warning');

        $('.message-placeholder', rwConfirmationDialog).html(msg);

        $('.dialog-overlay-toolbar', rwConfirmationDialog).empty();

        for (var i = 0; i < controls.length; i++) {
            var ctrl = $(controls[i].markup);
            $('.dialog-overlay-toolbar', rwConfirmationDialog).append(ctrl);
            ctrl.data('clickCallBack', controls[i].callback);

            if (!ctrl.is("a")) {
                ctrl.unbind('click').click(function () {
                    var clbk = $(this).data('clickCallBack');
                    if (typeof clbk === "function") {
                        todo = function () { clbk(); };
                    }

                    $(this).closest('.modal').modal('hide');

                    return false;
                });
            }
        }

        return rwConfirmationDialog;
    };

    function getRwFrameDialog(title, frameSource, isRend) {

        var rwConfirmationDialog = getCustomDialog(title != null ? title : 'Realnex Dialog');
        var source = frameSource != null ? frameSource : 'about:blank';
        $('.message-placeholder', rwConfirmationDialog)
            .html('<iframe style="width:100%;height:100%;border:none;" id="rw-dialog-frame"></iframe>');

        var iframe = $('.message-placeholder', rwConfirmationDialog).find('iframe');
        iframe.one('load', function () { iframe.prop('src', source); })
        iframe.prop('src', 'loading.html' + (isRend === true ? '?t=rend' : ''));

        $('.message-placeholder', rwConfirmationDialog).css('height', '100%');
        $('.dialog-overlay-toolbar', rwConfirmationDialog).remove();

        return rwConfirmationDialog;
    };

    this.Alert = function (dialogMsg) {
        try {
            var dlg = getCustomAlertDialog('Alert', dialogMsg);
            dlg.modal('show');
        } catch (err) {
            alert(dialogMsg);
        }
    };

    this.AdvancedAlert = function (title, dialogMsg, callBack) {
        var dlg = getCustomAlertDialog(title, dialogMsg, callBack);
        dlg.modal('show');
    };

    this.Notification = function (dialogMsg) {
        try {
            var dlg = getCustomAlertDialog('Notification', dialogMsg);
            dlg.modal('show');
        } catch (err) {
            alert(dialogMsg);
        }
    };

    this.Warning = function (dialogMsg) {
        try {
            var dlg = getCustomWarningDialog(dialogMsg);
            dlg.modal('show');
        } catch (err) {
            alert(dialogMsg);
        }
    };

    this.Error = function (dialogMsg) {
        try {
            var dlg = getCustomErrorDialog(dialogMsg);
            dlg.modal('show');
        } catch (err) {
            alert(dialogMsg);
        }
    };

    this.Confirmation = function (title, msg, yesCallBack, noCallBack) {
        try {
            var dlg = getCustomConfirmationDialog(title, msg, yesCallBack, noCallBack);
            dlg.modal('show');
        } catch (err) {
            if (confirm(msg)) {
                if (yesCallBack) {
                    yesCallBack();
                }
            } else {
                if (noCallBack) {
                    noCallBack();
                }
            }
        }
    };

    this.Choice = function (title, msg, buttons, callbacks) {
        var dlg = getRwChoiceDialog(title, msg, buttons, callbacks);
        dlg.modal('show');
    };

    this.CustomChoice = function (title, msg, controls) {
        var dlg = getRwCustomChoiceDialog(title, msg, controls);
        dlg.modal('show');
    };

    this.Frame = function (title, frameSource, frameWidth, frameHeight, loadComplete, isRend) {
        var dlg = getRwFrameDialog(title, frameSource, isRend);
        frameWidth = frameWidth || 600;
        frameHeight = frameHeight || 400;

        dlg.find('.modal-dialog').css({ width: frameWidth + 30 });
        dlg.find('.modal-body').css({ height: frameHeight + 30 });

        if (loadComplete && typeof loadComplete === "function")
            $('#rw-dialog-frame').on('load', function () {
                loadComplete();
            });

        return dlg.modal('show');
    };
}
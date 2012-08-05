﻿$(function () {
    $(".ckbox").click(function (ev) {
        $.post("/OrgMemberDialog/CheckBoxChanged/" + $(this).attr("id"), {
            ck: $(this).is(":checked")
        });
        return true;
    });
    $(".bt").button();
    $("a.display").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post($(this).attr('href'), null, function (ret) {
            $.displayEdit(f, ret);
        });
        return false;
    });
    $("a.delete").live("click", function (ev) {
        if (confirm("are you sure?"))
            $.post($(this).attr("href"), null, function (ret) {
                self.parent.RebindMemberGrids($("#from").val());
            });
        return false;
    });
    $("a.move").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (confirm("are you sure?"))
            $.post($(this).attr('href'), null, function (ret) {
                self.parent.RebindMemberGrids($("#from").val());
            });
        return false;
    });
    $("form.DisplayEdit a.submitbutton").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret.substring(0, 5) != "error")
                $.displayEdit(f, ret);
            self.parent.RebindMemberGrids($("#from").val());
        });
        return false;
    });
    $.displayEdit = function (f, ret) {
        $(f).html(ret).ready(function () {
            var acopts = {
                minChars: 3,
                matchContains: 1
            };
            $(".datepicker").datepicker();
            $(".bt").button();
            $("a.move").tooltip({
                showBody: "|",
                showURL: false
            });

        });
    }
});


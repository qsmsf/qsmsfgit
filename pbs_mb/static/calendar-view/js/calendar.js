/**
 * Created by lenovo on 2016/4/13 0013.
 */

var defaultMonthArray = [
    "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"
];
var defaultWeekDayTexts = [
    "日", "一", "二", "三", "四", "五", "六"
];
var defaultHighLightStyle = {
    background: {
        normal: "",
        highLight: "lightseagreen"
    }
};
var defaultBottomBarWidgets = [
    {
        label: "span",
        attrs: {
            id: "defaultBottomBarText",
            style: "margin-left: 6%;"
        },
        outerattrs: {

        },
        init: function() {
            var today = new Date();
            var year = today.getFullYear();
            var month = today.getMonth();
            var date = today.getDate();
            var day = today.getDay();
            var text = "今天是" + year + "年" + (month + 1) + "月" + date + "日  星期" + defaultWeekDayTexts[day];
            $("#defaultBottomBarText").html(text);
        }
    }
];

function onDateClickDefault() {

}

function onMonthChangeDefault() {

}

var monthArray;
var weekDayTexts;
var highLightStyle;
var bottomBarWidgets;
var onDateClickCustom;
var onMonthChangeCustom;

var currYear;
var currMonth;
var highLightDate;

function setCalendarView(querySelector, options) {
    setOptions(options);

    /*
     * first line content: YEAR MONTH
     * for example: 2010年 五月
     */
    var firstLine = document.createElement("table");
    firstLine.className = "calendar-title-line";
    var tr = document.createElement("tr");

    var td = document.createElement("td");
    var a = document.createElement("a");
    a.className = "calendar-switch-left";
    a.setAttribute("onclick", "currMonth--;onMonthChange();");
    td.appendChild(a);
    tr.appendChild(td);

    td = document.createElement("td");
    td.className = "calendar-title-content";
    tr.appendChild(td);

    td = document.createElement("td");
    a = document.createElement("a");
    a.className = "calendar-switch-right";
    a.setAttribute("onclick", "currMonth++;onMonthChange();");
    td.appendChild(a);
    tr.appendChild(td);

    firstLine.appendChild(tr);
    $(querySelector).append(firstLine);

    /*
     * second line content: week day text
     * for example: 日 一 二 三 四 五 六
     */
    var secondLine = document.createElement("table");
    secondLine.className = "calendar-week-index";
    tr = document.createElement("tr");
    for (var i = 0; i < 7; i ++) {
        td = document.createElement("td");
        td.innerHTML = weekDayTexts[i];
        tr.appendChild(td);
    }
    secondLine.appendChild(tr);
    $(querySelector).append(secondLine);

    /*
     * calendar date rows
     */
    var dateRows = document.createElement("table");
    dateRows.className = "calendar-dates";
    $(querySelector).append(dateRows);

    /*
     * calendar bottom bar
     */
    var bottomBar = document.createElement("table");
    bottomBar.className = "calendar-bottom-bar";
    tr = document.createElement("tr");

    for (var i = 0; i < bottomBarWidgets.length; i++) {
        if (bottomBarWidgets[i].label == undefined) {
            continue;
        }

        td = document.createElement("td");
        var widget = document.createElement(bottomBarWidgets[i].label);

        if (bottomBarWidgets[i].attrs != undefined) {
            for (var key in bottomBarWidgets[i].attrs) {
                widget.setAttribute(key, bottomBarWidgets[i].attrs[key]);
            }
        }

        td.appendChild(widget);

        if (bottomBarWidgets[i].outerattrs != undefined) {
            for (var key in bottomBarWidgets[i].outerattrs) {
                //td.attr(key, bottomBarWidgets[i].outerattrs[key]);
                td.setAttribute(key, bottomBarWidgets[i].outerattrs[key]);
            }
        }

        tr.appendChild(td);
    }
    bottomBar.appendChild(tr);
    $(querySelector).append(bottomBar);

    for (var i = 0; i < bottomBarWidgets.length; i++) {
        if (bottomBarWidgets[i].init) {
            bottomBarWidgets[i].init();
        }
    }

    /*
     * init calendar data
     */
    initCalendar();
}

function setOptions(options) {
    if (options.monthArray) {
        monthArray = options.monthArray;
    } else {
        monthArray = defaultMonthArray;
    }

    if (options.weekDayTexts) {
        weekDayTexts = options.weekDayTexts;
    } else {
        weekDayTexts = defaultWeekDayTexts;
    }

    if (options.highLightStyle) {
        highLightStyle = options.highLightStyle;
    } else {
        highLightStyle = defaultHighLightStyle;
    }

    if (options.bottomBarWidgets) {
        bottomBarWidgets = options.bottomBarWidgets;
    } else {
        bottomBarWidgets = defaultBottomBarWidgets;
    }

    if (options.onDateClick) {
        onDateClickCustom = options.onDateClick;
    } else {
        onDateClickCustom = onDateClickDefault;
    }

    if (options.onMonthChange) {
        onMonthChangeCustom = options.onMonthChange;
    } else {
        onMonthChangeCustom = onMonthChangeDefault;
    }
}

function initCalendar() {
    var today = new Date();
    currYear = today.getFullYear();
    currMonth = today.getMonth();
    setCalendar(currYear, currMonth);
    setCalendarHighLight(today.getDate());
}

function setCalendar(year, month) {
    var dayInMonth = getMaxDateOfMonth(year, month);

    var date1 = new Date(year, month, 1);
    currYear = date1.getFullYear();
    currMonth = date1.getMonth();
    var startWeekDay = date1.getDay();

    $(".calendar-title-content").html(currYear + '年&nbsp;&nbsp;&nbsp;&nbsp;' + monthArray[currMonth]);

    var rows = getCalendarRowCount(startWeekDay, dayInMonth);
    var htmlStr = "";
    for (var i = 0; i < rows; i++) {
        htmlStr += getCalendarRowHtml(i, startWeekDay, dayInMonth);
    }
    $(".calendar-dates").html(htmlStr);
}

function getMaxDateOfMonth(year, month) {
    var date1 = new Date(year, month, 1);
    var date2 = new Date(year, month + 1, 1);
    var dayInMonth = (date2 - date1) / 86400000;
    return dayInMonth;
}

function getCalendarRowCount(startWeekDay, dayInMonth) {
    var max = startWeekDay + dayInMonth;
    if (max == 28) {
        return 4;
    } else if (max > 28 && max < 36) {
        return 5;
    } else {
        return 6;
    }
}

function getCalendarRowHtml(row, startWeekDay, dayInMonth) {
    var html = '<tr>';
    for (var i = 0; i < 7; i++) {
        var index = i + row * 7;
        var date = index - startWeekDay + 1;
        if (date > 0 && date <= dayInMonth) {
            html += ('<td date="' + date + '" onclick="onDateClick(' + date + ');"><span>' + date + '</span></td>');
        } else {
            html += '<td></td>';
        }
    }
    html += '</tr>';

    return html;
}

function getCalendarYear() {
    return currYear;
}

function getCalendarMonth() {
    return currMonth;
}

function getCalendarHighLight() {
    return highLightDate;
}

function setCalendarHighLight(date) {
    var date1 = new Date(currYear, currMonth, 1);
    var date2 = new Date(currYear, currMonth + 1, 1);
    var dayInMonth = (date2 - date1) / 86400000;
    if (date > dayInMonth) {
        date = dayInMonth;
    }

    for (var key in highLightStyle) {
        $("td[date='" + highLightDate + "']").css(key, highLightStyle[key].normal);
        $("td[date='" + date + "']").css(key, highLightStyle[key].highLight);
    }
//    $("td[date='" + highLightDate + "']").css("background", "");
//    $("td[date='" + date + "']").css("background", "lightseagreen");
    highLightDate = date;
}

function onDateClick(date) {
    setCalendarHighLight(date);
    onDateClickCustom(date);
}

function onMonthChange() {
    setCalendar(currYear, currMonth);
    onMonthChangeCustom();
    setCalendarHighLight(highLightDate);
}

function setWidgetAttr(querySelector, attrs) {
    if (attrs == undefined) {
        return;
    }

    for (var key in attrs) {
        $(querySelector).attr(key, attrs[key]);
    }
}

function setWidgetHtml(querySelector, html) {
    $(querySelector).html(html);
}
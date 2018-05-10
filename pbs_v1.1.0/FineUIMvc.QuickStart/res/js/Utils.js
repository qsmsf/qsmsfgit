
function renderUnit(value) {
    return getUnitName(value);
}

function renderUser(value) {
    var userName = getUserFullName(value);
    return userName;
}

function renderGender(value, params) {
    return value == 1 ? '男' : '女';
}

function renderState(value) {
    switch (value) {
        case 1001:
            return "未提交";
            break;
        case 1002:
            return "待审核";
            break;
        case 1003:
            return "审核失败";
            break;
        case 1004:
            return "审核成功";
            break;
        case 1000:
            return "已删除";
            break;
        case 2001:
            return "现场方位"
            break;
        case 2002:
            return "现场概貌"
            break;
        case 2003:
            return "重点部位"
            break;
        case 2004:
            return "现场制图"
            break;
        case 2005:
            return "其他"
            break;
        default:
            return "未知";
    }
}
function renderCheckField(value) {
    if (value == "1")
        return true;
    else
        return false;
}
function getUnitName(value) {
    var unitName = '', unit;
    if (value) {
        for (var i = 0, count = window._Units.length; i < count; i++) {
            unit = window._Units[i];
            if (value == unit[0]) {
                unitName = unit[1];
                break;
            }
        }
    }
    return unitName;
}

function getUserFullName(value) {
    var userName = '', user;    
    if (value) {
        for (var i = 0, count = window._Users.length; i < count; i++) {
            user = window._Users[i];
            if (value == user[0]) {
                userName = user[1];
                break;
            }
        }
    }
    return userName;
}
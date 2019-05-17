
//在JavaScript中，正则表达式只能使用"/"开头和结束，不能使用双引号
//生成验证码
function returnCode() {
    var code = "";
    var codeLength = 4;//验证码的长度  
    var random = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
   'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');//随机数  
    for (var i = 0; i < codeLength; i++) {//循环操作  
        var index = Math.floor(Math.random() * 36);//取得随机数的索引（0~35）  
        code += random[index];//根据索引取得随机数加到code上  
    }
    return code;
}
//检查日期类型
function CheckDate(dateValue, title) {
    var timeExpr = /^\d{4}-(((0[13578]|1[02])-(0[1-9]|[1-2]\d|3[01]))|((0[469]|11)-(0[1-9]|[1-2]\d|30))|(02-(0[1-9]|1\d|2\d)))$/;
    if (!timeExpr.exec(dateValue)) {
        alert(title + "只能为yyyy-MM-dd(2011-01-01)");
        return false;
    }
    return true;
}
//只能输入数字x是数字返回false，返回true表示非数字
function check_Number(loginName) {
    //定义正则表达式部分
    if (isNaN(Number(loginName))) {
        //alert("只能输入工号或学号");
            return false;
    }
    return true;
    //var numExpr = /^[0-9]*$/;
    //if (!loginName.exec(numExpr)) {
    //    alert("只能输入工号或学号");
    //    return false;
    //}
    //return true;
}
//验证电话和手机
function CheckTelephone(phoneValue) {
    var timeExpr = /^(((\(\d{3}\)|\d{3}-)?\d{8})|((\(\d{4,5}\)|\d{4,5}-)?\d{7,8})|(\d{11}))$/;
    if (!timeExpr.exec(phoneValue)) {
        //alert("电话输入格式错误");
        return false;
    }
    return true;
}
function TextBoxSetFocus(txtBoxID) {
    document.getElementById('" + txtBoxID + "').focus();
    document.getElementById('" + txtBoxID + "').select();
}
//验证E-mail地址([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})
function CheckEmail(email) {
    if (!/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(email)) {
        //alert("E-mail地址格式错误!");
        return false;
    }
    return true;

}
//验证手机号
function CheckPhone(phone) {
    //if (phone == "") {
    //    alert("手机号码不能为空!");
    //    return false;
    //}
    if (isNaN(phone)) {
        alert("手机号只能为数字型字符!");
        return false;
    }
    if (phone.length != 11) {
        alert("手机号码只能为11位!");
        return false;
    }
    if (!(/^1[3|5|8][0-9]\d{4,8}$/.test(telNo.value))) {
        alert("无效的手机号,前七位必须正确!");
        telNo.select();
        return false;
    }
    return true;

}
//验证身份证号码(^\d{15}$)|
function CheckIdentify(idNo) {
    if (idNo != "") {
        if (!(/(^\d{17}([0-9]|X)$)/.test(idNo))) {
            alert("输入的身份证号长度不对，或者号码不符合规定！");
            return false;
        }
    }
    return true;
}
//判断是否为汉字
function checkChineseCharacter(str) {
    var ret = true;
    for (var i = 0; i < str.length; i++)
        ret = ret && (str.charCodeAt(i) >= 10000);
    return ret;
}


//   //验证日期类型
//    function checkDate(date){    
//        var Expression = /^((((1[6-9]|[2-9]\d)\d{2})(\/|\-)(0?[13578]|1[02])(\/|\-)(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})(\/|\-)(0?[13456789]|1[012])(\/|\-)(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})(\/|\-)0?2(\/|\-)(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$/;
//        var objExp = new RegExp(Expression);
//        if(objExp.test(date)==true)
//            return true
//        else return false;
//    }
//    判断字符串是否以指定字符串开始

function checkOccur(startStr, inStr) {
    var Expression = eval("/^" + startStr + "/");
    var objExp = new RegExp(Expression);
    return objExp.test(inStr);

}
//验证字符串长度，一个汉字占2位，9个字符加一个汉字占11位
function checkstr(str, digit) {
    var n = 0;
    for (i = 0; i < str.length; i++) {
        var leg = str.charCodeAt(i);
        if (leg > 255) {
            n += 2;
        } else {
            n += 1;
        }
    }
    if (n > digit) {
        return true;
    } else {
        return false;
    }

}
//comments.aspx.
function onft(e) {
    e.style.border = "1px #61a4da solid";
    e.style.color = "#000";

}
function onbt(e) {
    e.style.border = "1px #bebee1 solid";
    e.style.color = "#666";
}

function checkInput(e) {
    var txtName = e.name;
    var indx = txtName.substring(6, txtName.indexOf("_"));
    var max = parseInt(txtName.substring(txtName.indexOf("_") + 1));
    var err = "只能输入0和" + max + "之间的分数";

    var def = Number(e.value);
    document.getElementById('img' + indx).style.visibility = "visible";
    document.getElementById('img' + indx).src = "images/cuo.png";
    if (!isNaN(def))
        e.value = def;
    if (isNaN(def) || def > max || def < 0) {
        e.select();
        e.focus();
        alert(err)
        return;
    }
    else {
        if (def >= 0) {
            showSum();
            document.getElementById('img' + indx).src = "images/dui.png";
        }
        else
            document.getElementById('img' + indx).style.visibility = "hidden";
    }
}
function sumScores() {
    var o = document.getElementsByTagName("INPUT")
    var sum = 0;
    var e = null;
    var err = "";
    for (i = 0; i < o.length; i++) {
        if (o[i].type == "text") {
            var txtName = o[i].name;
            var max = parseInt(txtName.substring(txtName.indexOf("_") + 1));

            var def = Number(o[i].value);
            if (isNaN(def) || def > max || def < 0) {
                if (e == null) {
                    e = o[i];
                    err = "只能输入0和" + max + "之间的分数";
                    break;
                }
            }
            //                    else
            //                        sum +=def;
        }

    }
    //        document.getElementById("lblTotalScore").innerText=sum;
    //        document.getElementById('HiddenField1').value=sum ; 

    if (e != null) {
        e.select();
        alert(err)
        return -1;
    }
    sum = document.getElementById("lblTotalScore").innerText;
    return sum;
}
function showSum() {
    var o = document.getElementsByTagName("INPUT")
    var sum = 0;
    for (i = 0; i < o.length; i++) {
        if (o[i].type == "text") {
            var txtName = o[i].name;
            var max = parseInt(txtName.substring(txtName.indexOf("_") + 1));
            var def = Number(o[i].value);
            if (!isNaN(def) && def <= max && def > 0)
                sum += def;
        }

    }
    document.getElementById("lblTotalScore").innerText = sum;
    document.getElementById('HiddenField1').value = sum;
}
function checkSubmitSelt() {
    var ret = sumScores();
    if (ret == 0) {
        alert("总分不能为零，提交失败");
        return false;
    }
    else if (ret == -1)
        return false;
    else
        return confirm('提交之后不能修改，确认提交吗？');
}
//6. 验证用户名密码

//要求用户名由3-10位的字母、数字和下划线组成

///^(\w){3,10}$/

//密码由6-20位的字母、数字、下划线和点“.”组成并且首字符为字母

///^[A-Za-z]{1}([A-Za-z0-9]|[._]){5,19}$/

//7. 验证有效的URL

///http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/

//8. 验证金额和数量

//金额：isNaN(form1.dj.value)

//数量：/^[1-9]+(\d*$)/


var area = document.getElementById('box');
var cont1 = document.getElementById('cont1');
var cont2 = document.getElementById('cont2');

area.scrollTop = 0;
// 克隆cont1给cont2
cont2.innerHTML = cont1.innerHTML;

function myScroll() {
    if (area.scrollTop >= cont1.scrollHeight) {
        area.scrollTop = 0;
    } else {
        area.scrollTop++;
    }
}

var time = 50;
var interval = setInterval('myScroll()', time);

area.onmouseover = function () {
    clearInterval(interval);
};

area.onmouseout = function () {
    // 继续执行之前的定时器
    interval = setInterval('myScroll()', time);
};
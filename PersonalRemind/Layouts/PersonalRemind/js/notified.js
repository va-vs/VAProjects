/*********************************************************************************************************
This CS_ONLINE plugin was designed by jason.leung. Please feel free to contact me if any problems. Thanks.
You could link to shlzh1984.gicp.net to leave a message. Thank you for your support.
Email: 147430218@qq.com
SinaWeibo: @切片面包
*********************************************************************************************************/
// Public functions
function myEvent(obj,ev,fn)
{
	if(obj.attachEvent)
	{
		obj.attachEvent('on'+ev,fn);
	}
	else
	{
		obj.addEventListener(ev,fn,false);
	}
};
function getbyClass(oParent,sClass)
{
	var value=[];
	var tmp=oParent.getElementsByTagName('*');
	for (var i=0; i<tmp.length; i++)
	{
		if (tmp[i].className==sClass)
		{
			value.push(tmp[i]);
		}
	}
	return value;
};
function getStyle(obj,name)
{
	if (obj.currentStyle)
	{
		return obj.currentStyle[name];
	}
	else
	{
		return getComputedStyle(obj,false)[name];
	}
};
function startMoving(obj,json,fnEnd)
{
	clearInterval(obj.timer);
	obj.timer=setInterval(function()
	{
		var cur=0;
		var bStop=true;
		for (var attr in json)
		{
			if (attr == 'opacity')
			{
				cur=Math.round(parseFloat(getStyle(obj,attr))*100);
			}
			else
			{
				cur=parseInt(getStyle(obj,attr));
			}
			var speed=(json[attr]-cur)/5;
			speed=speed>0?Math.ceil(speed):Math.floor(speed);
			if (cur!=json[attr])bStop=false;
			if (attr == 'opacity')
			{
				obj.style.filter='alpha(opacity:'+cur+speed+')';
				obj.style.opacity=(cur+speed)/100;
			}
			else
			{
				obj.style[attr]=cur+speed+'px';
			}
		}
		if (bStop)
		{
			clearInterval(obj.timer);
			if (fnEnd)fnEnd();
		}
	}, 30);
}
function cs_options (oParent,sClass,tagName)
{
	var _this=this;
	this.cs_options_ul=getbyClass(oParent,sClass)[0];
	this.cs_options_li=this.cs_options_ul.getElementsByTagName(tagName);
	this.cs_context=getbyClass(document.body,'cs_context');
	for (var i=0; i<this.cs_options_li.length; i++)
	{
		this.cs_options_li[i].index=i;
		this.cs_options_li[i].onmouseover=function ()
		{
			_this.clearStyle();
			_this.select(this);
		}
	}
}
cs_options.prototype.select=function (now)
{
	this.cs_options_li[now.index].style.color='yellow';
	this.cs_options_li[now.index].style.borderBottom='3px solid #ff8400';
	this.cs_context[now.index].style.display='block';
}
cs_options.prototype.clearStyle=function ()
{
	for (var i=0; i<this.cs_options_li.length; i++)
	{
		this.cs_options_li[i].style.color='';
		this.cs_options_li[i].style.borderBottom='';
		this.cs_context[i].style.display='none';
	}
}
function cs_product (id,id2)
{
	this.cs_products_ul=document.getElementById(id).getElementsByTagName('ul')[0];
	this.cs_products_li=this.cs_products_ul.getElementsByTagName('li');
	this.cs_products_num_li=document.getElementById(id2).getElementsByTagName('li');
	var _this=this;
	for (var i=0; i<this.cs_products_num_li.length; i++)
	{
		this.cs_products_num_li[i].index=i;
		this.cs_products_num_li[i].onmouseover=function ()
		{
			for (var i=0; i<_this.cs_products_num_li.length; i++)
			{
				_this.cs_products_num_li[i].style.background='';
			}
			this.style.background='#ff6600';
			_this.select(this);
		}
	}
}
cs_product.prototype.select=function (now)
{
	var _this=this;
	this.cs_products_ul.style.width=this.cs_products_li.length*this.cs_products_li[0].offsetWidth+'px';
	startMoving(_this.cs_products_ul,{left:now.index*-this.cs_products_li[0].offsetWidth});
}
// CS_ONLINE functions
myEvent(window,'load',function()
{
	var cs_online=document.getElementById('cs_online');
	var cs_title=getbyClass(cs_online,'cs_title')[0];
	var second=3;				//Open this plugin automatically after 2 seconds later.
	var timer=null;	
	var opened=0;
	new cs_options (cs_online,'cs_options','li');
	new cs_product ('cs_product','cs_product_num');
	cs_title.onclick=function ()
	{
		opened++;
		if (opened%2)
		{
			startMoving(cs_online,{right:-452});
		}
		else
		{
			startMoving(cs_online,{right:0});
		}
	}
	timer=setTimeout(function()
	{
		startMoving(cs_online,{right:0});
	}, second*1000);
});
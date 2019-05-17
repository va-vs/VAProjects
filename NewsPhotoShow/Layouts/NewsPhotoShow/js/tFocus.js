function addLoadEvent(func)
{var oldonload=window.onload;
 if(typeof window.onload != 'function'){window.onload=func;}
 else{window.onload=function(){oldonload();func();}}} 
function Focus(){
  function byid(id) {return document.getElementById(id);}
  function bytag(tag,obj){return (typeof obj=='object'?obj:byid(obj)).getElementsByTagName(tag);}
  
  var timer = null;
  var oFocus = byid('tFocus');

  var ismart_b=byid('ismart_v');
  var oPic = byid('tFocus-pic');
  var oPicLis = bytag('li',oPic);
  var oFPica=bytag('a',oPic);
  var oPicimage=bytag('img',oPic);
  var oBtn = byid('tFocus-btn');
  var oBtnLis = bytag('li',oBtn);
  var iActive = 0; 

/* inlize()这一段代码 透明度百分之百 */
  function inlize(){oPicLis[0].style.filter = 'alpha(opacity:100)'; oPicLis[0].style.opacity = 100;
           var txtiSmart=oPicimage[iActive].getAttribute('alt');
     ismart_b.setAttribute('value',txtiSmart);  
	  var txtiSmart_img=oPicimage[iActive].getAttribute('value');
      ismart_b.onclick=function(){
      if(ismart_b.onclick=1){window.location.href=txtiSmart_img}
         } ;            
   };
   
  for(var i=0;i<oPicLis.length;i++){
	  oBtnLis[i].sIndex = i;
	  oBtnLis[i].onclick = function(){
		  if(this.sIndex==iActive)return;iActive=this.sIndex;changePic();

  }};
  byid('tFocus-leftbtn').onclick=byid('prev').onclick = function(){iActive--;if(iActive==-1){iActive=oPicLis.length-1;}changePic();
     var txtiSmart=oPicimage[iActive].getAttribute('alt');
     ismart_b.setAttribute('value',txtiSmart);
        var txtiSmart_img=oPicimage[iActive].getAttribute('value');
         ismart_b.onclick=function(){
             if(ismart_b.onclick=1){window.location.href=txtiSmart_img} ; 
         } ;  
                                                        
  };
  byid('tFocus-rightbtn').onclick=byid('next').onclick= function(){iActive++;if(iActive==oPicLis.length){iActive=0;}changePic();   
  var txtiSmart=oPicimage[iActive].getAttribute('alt');
     ismart_b.setAttribute('value',txtiSmart);
   var txtiSmart_img=oPicimage[iActive].getAttribute('value');
         ismart_b.onclick=function(){
             if(ismart_b.onclick=1){window.location.href=txtiSmart_img} ; 
           }   ;   
         }  ; 
  
  byid('markLeft').onmouseover=byid('prev').onmouseover=function (){doMove(byid('prev'), 'opacity', 100);}
  byid('markLeft').onmouseout=byid('prev').onmouseout=function (){doMove(byid('prev'), 'opacity', 0);}	
  byid('markRight').onmouseover=byid('next').onmouseover=function (){doMove(byid('next'), 'opacity', 100);}	
  byid('markRight').onmouseout=byid('next').onmouseout=function (){doMove(byid('next'), 'opacity', 0);}	
  
  function changePic(){
	  for(var i=0;i<oPicLis.length;i++)
	  {
		  doMove(oPicLis[i],'opacity',0);
		  oBtnLis[i].className = '';
		  };
		  doMove(oPicLis[iActive],'opacity',100);
		  oBtnLis[iActive].className = 'active';

  if(iActive==0){
	  doMove(bytag('ul',oBtn)[0],'left',0);}
	  else if(iActive>=oPicLis.length-2){
		  doMove(bytag('ul',oBtn)[0],
		  'left',-(oPicLis.length-4)*(oBtnLis[0].offsetWidth+14));
  }else{doMove(bytag('ul',oBtn)[0],'left',-(iActive-1)*(oBtnLis[0].offsetWidth+14)); }
      };
function txt(){
	var txtiSmart=oPicimage[iActive].getAttribute('alt');
     ismart_b.setAttribute('value',txtiSmart);	
   var txtiSmart_img=oPicimage[iActive].getAttribute('value');
     ismart_b.onclick=function(){
             if(ismart_b.onclick=1){window.location.href=txtiSmart_img}
           }        
	}
  
  /*自动播放 到头循环 */  
  function autoplay(){if(iActive>=oPicLis.length-1){iActive=0;}else{iActive++;}changePic();txt();};
  /*3秒调用函数一次 */  
  aTimer = setInterval(autoplay,3000);

  inlize();
  function getStyle(obj, attr){if(obj.currentStyle){return obj.currentStyle[attr];}else{return getComputedStyle(obj, false)[attr];}
  	};
  
  function doMove(obj,attr,iTarget)
  {clearInterval(obj.timer);
  obj.timer=setInterval(function (){var iCur=0;if(attr=='opacity'){iCur=parseInt(parseFloat(getStyle(obj, attr))*100);}
  else{iCur=parseInt(getStyle(obj, attr));}var iSpeed=(iTarget-iCur)/8;iSpeed=iSpeed>0?Math.ceil(iSpeed):Math.floor(iSpeed);if(iCur==iTarget){clearInterval(obj.timer);}
  else{if(attr=='opacity'){obj.style.filter='alpha(opacity:'+(iCur+iSpeed)+')';obj.style.opacity=(iCur+iSpeed)/100;}else{obj.style[attr]=iCur+iSpeed+'px';}}}, 30)};
  byid('tFocus').onmouseover = function(){clearInterval(aTimer);}
  byid('tFocus').onmouseout = function(){aTimer = setInterval(autoplay,3000);}
}

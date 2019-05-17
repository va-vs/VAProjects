$(function(){
		$(".item1 li").hover(
			function(){
				var that=this;	
				item1Timer=setTimeout(function(){
					$(that).find("div").animate({"top":0,"height":204},300,function(){
						$(that).find("p").fadeIn(200);
					});
				},100);
			},
			function(){
				var that=this;	
				clearTimeout(item1Timer);
				$(that).find("p").fadeOut(200);
				$(that).find("div").animate({"top":160,"height":50},300);
			}
		)
});
	
/* 2 */
$(function(){
	$(".item2").hover(
		function(){
			var that=this;
			item2Timer=setTimeout(function(){
				$(that).find('div.caption').slideDown(300);
				$(that).find('.item2-txt').fadeOut(200);
			},100);
			
		},
		function(){
			var that=this;
			clearTimeout(item2Timer);
			$(that).find('div.caption').slideUp(300);
			$(that).find('.item2-txt').fadeIn(200);
		}
	);
});

/* 3 */
$(function(){
	$(".item3").hover(
		function(){
			var that=this;
			item3Timer=setTimeout(function(){
				$(that).find("div").animate({width:148,height:148},300,function(){
					$(that).find("h2").fadeOut(200);
					$(that).find("dl").fadeIn(200);
				});
			},100);
			
			},
		function(){
			var that=this;
			clearTimeout(item3Timer);
			$(that).find("dl").fadeOut(200);
			$(that).find("div").stop().animate({width:0,height:0},300);
			$(that).find("h2").fadeIn(200);
			}
		)
});

/* 4 */
$(function(){
	var move = -50;
	var zoom = 1.5;
	$(".item4").each(function(){
		var that=this
		$(that).bind({
			mouseenter:function(){
				item4Timer=setTimeout(function(){
					width = $(that).width() * zoom;
					height = $(that).height() * zoom;
					$(that).find('img').animate({'width':width, 'height':height, 'top':move, 'left':move},500);
					$(that).find('div.caption').fadeIn(500);
					$(that).find('.item4-txt').fadeOut(500);
				},200);
			},
			mouseleave:function(){
				clearTimeout(item4Timer);
				$(that).find('img').animate({'width':$(that).width(), 'height':$(that).height(), 'top':'0', 'left':'0'},500);	
				$(that).find('div.caption').fadeOut(500);
				$(that).find('.item4-txt').fadeIn(500);
				}
		});
	})	
});

/* 5 */
$(function(){
	$(".item5").hover(
		function(){
			var that=this;
			item5Timer=setTimeout(function(){
				$(that).find("div").animate({width:157,height:203},300,function(){
					$(that).find("h2").fadeOut(200);
					$(that).find("dl").fadeIn(200);
				});
			},100);
			
			},
		function(){
			var that=this;
			clearTimeout(item5Timer);
			$(that).find("dl").fadeOut(200);
			$(that).find("div").stop().animate({width:0,height:0},300);
			$(that).find("h2").fadeIn(200);
			}
		)
	
});

/* 6 */
$(function(){
	$(".item6").hover(
		function(){
			var that=this;
			item6Timer=setTimeout(function(){
				$(that).find("div").animate({width:157,height:203,left:0,top:0},300,function(){
					$(that).find("h2").fadeOut(200);
					$(that).find("dl").fadeIn(200);
				});
			},100);
			
			},
		function(){
			var that=this;
			clearTimeout(item6Timer);
			$(that).find("dl").fadeOut(200);
			$(that).find("div").stop().animate({width:0,height:0,left:78,top:101},300);
			$(that).find("h2").fadeIn(200);
			}
		)
});
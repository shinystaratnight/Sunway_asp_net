!function(a){function t(){n||(a("."+o).live("click",e).live("change",e).live("focusin",e).live("focusout",l),bound=!0,n=!0)}function e(){var t=a(this),e=a(t.data(i));e.css("display","none")}function l(){var t=this;setTimeout(function(){var e=a(t);a(e.data(i)).css("top",e.position().top+"px").css("left",e.position().left+"px").css("display",e.val()?"none":"block")},200)}var o="PLACEHOLDER-INPUT",i="PLACEHOLDER-LABEL",n=!1,s={labelClass:"placeholder"},d=document.createElement("input");return"placeholder"in d?(a.fn.placeholder=a.fn.unplaceholder=function(){},void delete d):(delete d,a.fn.placeholder=function(n){t();var d=a.extend(s,n);this.each(function(){var t=Math.random().toString(32).replace(/\./,""),n=a(this),s=a('<label style="position:absolute;display:none;top:0;left:0;"></label>');n.attr("placeholder")&&n.data(o)!==o&&(n.attr("id")||(n.attr("id")="input_"+t),s.attr("id",n.attr("id")+"_placeholder").data(o,"#"+n.attr("id")).attr("for",n.attr("id")).addClass(d.labelClass).addClass(d.labelClass+"-for-"+this.tagName.toLowerCase()).addClass(i).text(n.attr("placeholder")),n.data(i,"#"+s.attr("id")).data(o,o).addClass(o).after(s),e.call(this),l.call(this))})},void(a.fn.unplaceholder=function(){this.each(function(){var t=a(this),e=a(t.data(i));t.data(o)===o&&(e.remove(),t.removeData(o).removeData(i).removeClass(o))})}))}(jQuery);
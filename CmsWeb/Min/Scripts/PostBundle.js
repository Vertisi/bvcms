function AddSelected(n){$("#searchDialog").dialog("close");var t=$("tr[cid="+n.cid+"]");$("a.pid",t).text(n.pid),$("td.name",t).text(n.name),$("a.edit",t).click()}$(function(){$("#pid").blur(function(){var n,i,t;if($(this).val()=="")return!1;if($(this).val()=="d")return n=$("#bundle > tbody > tr:first"),i=$("a.pid",n).text(),$("#name").val($("td.name",n).text()),$("#checkno").val($("td.checkno",n).text()),$("#notes").val($("td.notes",n).text()),$("#amt").focus(),$(this).val($.trim(i)),!0;t=$("#pbform").serialize(),$.post("/PostBundle/GetName/",t,function(n){n=="not found"?($.growlUI("PeopleId","Not Found"),$("#name").focus(),$("#pid").val("")):($("#name").val(n),$("#amt").focus())})}),$(".bt").button(),$("td.name").tooltip({showBody:"|"}),$("#name").autocomplete("/PostBundle/Names",{minChars:3,matchContains:!1,mustMatch:!0,width:200,selectFirst:!1,autoFill:!1,formatItem:function(n){return n[0]+" ("+n[2]+")<br />"+n[3]},formatResult:function(n){return n[0]}}),$("#name").result(function(n,t){t&&$("#pid").val(t[1]),this.value===""&&($.growlUI("Name","Not Found"),$("#pid").val(""))}),$.Stripe=function(){$("#bundle tbody tr").removeClass("alt"),$("#bundle tbody tr:even").addClass("alt")},$.Stripe();var n=!0;$("#notes").keydown(function(t){n&&(t.keyCode==9||t.keyCode==13)&&!t.shiftKey&&(t.preventDefault(),n=!1,$.PostRow({scroll:!1}))}),$("#pid").keydown(function(n){n.keyCode==13&&(n.preventDefault(),$.browser.msie||$("#pid").blur(),$("#name").focus())}),$("#name").keydown(function(n){n.keyCode==13&&(n.preventDefault(),$("#amt").focus())}),$("#amt").keydown(function(n){n.keyCode==13&&(n.preventDefault(),$("#fund").focus())}),$("#fund").keydown(function(n){n.keyCode==13&&(n.preventDefault(),$("#checkno").focus())}),$("#checkno").keydown(function(n){n.keyCode==13&&(n.preventDefault(),$("#notes").focus())}),$("a.update").click(function(n){n.preventDefault(),$.PostRow({scroll:!0})}),$("a.edit").live("click",function(n){var t,i;n.preventDefault(),t=$(this).closest("tr"),$("#editid").val(t.attr("cid")),$("#pid").val($("a.pid",t).text()),$("#name").val($("td.name",t).text()),$("#fund").val($("td.fund",t).attr("val")),$("#pledge").attr("checked",$("td.fund",t).attr("pledge")=="true"),i=$("#amt"),i.val($("td.amt",t).attr("val")),$("#checkno").val($("td.checkno",t).text()),$("#notes").val($("td.notes",t).text()),t.hide(),i.val()=="0.00"&&i.val(""),i.focus(),$("a.edit").hide(),$("a.split").hide(),$("a.delete").hide(),$.Stripe()}),$("a.split").live("click",function(n){var i,t,r;if(n.preventDefault(),i=prompt("Amount to split out",""),i=parseFloat(i),isNaN(i))return!1;t=$(this).closest("tr"),r={pid:$("a.pid",t).text(),name:$("td.name",t).text(),fund:$("td.fund",t).attr("val"),pledge:$("td.fund",t).attr("pledge"),amt:i,splitfrom:t.attr("cid"),checkno:$("td.checkno",t).text(),notes:$("td.notes",t).text(),id:$("#id").val()},$.PostRow({scroll:!0,q:r})}),$("a.delete").live("click",function(n){var t,i;n.preventDefault(),confirm("are you sure?")&&(t=$(this).closest("tr"),$("#editid").val(t.attr("cid")),i=$("#pbform").serialize(),$.post("/PostBundle/DeleteRow/",i,function(n){t.remove(),$("#totalitems").text(n.totalitems),$("#itemcount").text(n.itemcount),$("#editid").val(""),$.Stripe()}))}),$("a.pid").live("click",function(n){n.preventDefault();var t=$("#searchDialog");$("iframe",t).attr("src",this.href),t.dialog("open")}),$("#bundle").bind("mousedown",function(n){$(n.target).hasClass("clickEdit")?$(n.target).editable(function(t){return $.post("/PostBundle/Edit/",{id:n.target.id,value:t},function(n){$("#totalitems").text(n.totalitems),$("#itemcount").text(n.itemcount),$("#a"+n.cid).text(n.amt)}),t},{indicator:"<img src='/images/loading.gif'>",tooltip:"Click to edit...",style:"display: inline",width:"4em",height:25}):$(n.target).hasClass("clickSelect")&&$(n.target).editable("/PostBundle/Edit/",{indicator:'<img src="/images/loading.gif">',loadtype:"post",loadurl:"/PostBundle/Funds/",type:"select",submit:"OK",style:"display: inline"})}),$.PostRow=function(t){var u,r,i;if(!t.q){if(u=parseFloat($("#amt").val()),!u>0){$.growlUI("Contribution","Cannot post, No Amount");return}t.q=$("#pbform").serialize()}r="/PostBundle/PostRow/",i=$("#editid").val(),i&&(r="/PostBundle/UpdateRow/"),$.post(r,t.q,function(r){var e,u,f;r&&($("#totalitems").text(r.totalitems),$("#itemcount").text(r.itemcount),e=$("#pid").val(),i?(u=$('tr[cid="'+i+'"]'),u.replaceWith(r.row),u=$('tr[cid="'+i+'"]')):t.q.splitfrom?(u=$('tr[cid="'+t.q.splitfrom+'"]'),$("#a"+t.q.splitfrom).text(r.othersplitamt),$(r.row).insertAfter(u),u=$('tr[cid="'+r.cid+'"]')):($("#bundle tbody").prepend(r.row),u=$("#bundle tbody tr:first")),$("td.name",u).tooltip({showBody:"|"}),$("a.edit").show(),$("a.split").show(),$("a.delete").show(),$(".bt",u).button(),$("#editid").val(""),$("#entry input").val(""),$("#fund").val($("#fundid").val()),$.Stripe(),$("#pid").focus(),n=!0,f=u.offset().top-60,t.scroll==!0&&$("html,body").animate({scrollTop:f},1e3),u.effect("highlight",{},3e3))})},$("#searchDialog").dialog({title:"Search/Add Contributor",bgiframe:!0,autoOpen:!1,width:712,height:630,modal:!0,overlay:{opacity:.5,background:"black"},close:function(){$("iframe",this).attr("src","")}}),$("#totalitems").text($("#titems").val()),$("#totalcount").text($("#tcount").val())})
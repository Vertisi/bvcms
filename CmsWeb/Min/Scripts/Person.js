function RebindMemberGrids(){$.updateTable($("#current-tab form")),$.updateTable($("#pending-tab form")),$("#memberDialog").dialog("close")}function RebindUserInfoGrid(){$.updateTable($("#user-tab form")),$("#memberDialog").dialog("close")}$(function(){var t=$("#address-tab").tabs(),n;$("#enrollment-tab").tabs(),$("#member-tab").tabs(),$("#growth-tab").tabs(),$("#system-tab").tabs(),n=$("#main-tab").tabs(),$(".submitbutton,.bt").button(),t.tabs("select",$("#addrtab").val()),$("#dialogbox").SearchPeopleInit({overlay:{background:"#000",opacity:.3}}),$("#clipaddr").live("click",function(){var t=$("#addrhidden")[0],n;return t.createTextRange&&(n=t.createTextRange(),n&&n.execCommand("Copy")),!1}),$("#deleteperson").click(function(){var n=$(this).attr("href");return confirm("Are you sure you want to delete?")&&$.post(n,null,function(n){n!="ok"?($.blockUI({message:"delete Failed: "+n}),$(".blockOverlay").attr("title","Click to unblock").click($.unblockUI)):($.blockUI({message:"person deleted"}),$(".blockOverlay").attr("title","Click to unblock").click(function(){$.unblockUI(),window.location="/"}))}),!1}),$("a.deloptout").live("click",function(n){n.preventDefault();var t=$(this).attr("href");confirm("Are you sure you want to delete?")&&$.post(t,{},function(n){n!="ok"?$.growlUI("failed",n):($.updateTable($("#user-tab form")),$.growlUI("Success","OptOut deleted"))})}),$("#moveperson").click(function(n){return $("#dialogbox").SearchPeople(n,function(n,t){window.location="/Merge?PeopleId1="+$("#PeopleId").val()+"&PeopleId2="+t}),!1}),$.editable.addInputType("datepicker",{element:function(n){var i=$("<input>");return n.width!="none"&&i.width(n.width),n.height!="none"&&i.height(n.height),i.attr("autocomplete","off"),$(this).append(i),i},plugin:function(n){var i=this;n.onblur="ignore",$(this).find("input").datepicker().bind("click",function(){return $(this).datepicker("show"),!1}).bind("dateSelected",function(){$(i).submit()})}}),$.editable.addInputType("multiselect",{element:function(n){var i=$('<select multiple="multiple" />');return n.width!="none"&&i.width(n.width),n.size&&i.attr("size",n.size),$(this).append(i),i},content:function(n){var r,u;for(r in n)u=$("<option />").val(r).text(r),n[r]==!0&&u.attr("selected",!0),$("select",this).append(u);$("select",this).multiselect({close:function(){var i=$("select").val()},position:{my:"left bottom",at:"left top"}})}}),$.extraEditable=function(n){$(".editarea",n).editable("/Person/EditExtra/",{type:"textarea",submit:"OK",rows:5,width:300,indicator:'<img src="/images/loading.gif">',tooltip:"Click to edit..."}),$(".clickEdit",n).editable("/Person/EditExtra/",{indicator:"<img src='/images/loading.gif'>",tooltip:"Click to edit...",style:"display: inline",width:"300px",height:25,submit:"OK"}),$(".clickDatepicker").editable("/Person/EditExtra/",{type:"datepicker",tooltip:"Click to edit...",style:"display: inline",width:"300px",submit:"OK"}),$(".clickSelect",n).editable("/Person/EditExtra/",{indicator:'<img src="/images/loading.gif">',loadurl:"/Person/ExtraValues/",loadtype:"POST",type:"select",submit:"OK",style:"display: inline"}),$(".clickCheckbox",n).editable("/Person/EditExtra",{type:"checkbox",onblur:"ignore",submit:"OK"}),$(".clickMultiselect",n).editable("/Person/EditExtra",{indicator:'<img src="/images/loading.gif">',loadurl:"/Person/ExtraValues2/",loadtype:"POST",type:"multiselect",submit:"OK",onblur:"ignore",style:"display: inline"})},$.getTable=function(n,t){return t=t||n.serialize(),$.post(n.attr("action"),t,function(t){$(n).html(t).ready(function(){$("table.grid > tbody > tr:even",n).addClass("alt"),$(".bt").button(),$(".datepicker").datepicker(),$.extraEditable("#extravalues"),$(".tooltip",n).tooltip({showURL:!1,showBody:"|"})})}),!1},$("#memberDialog").dialog({title:"Member Dialog",bgiframe:!0,autoOpen:!1,width:600,height:550,modal:!0,overlay:{opacity:.5,background:"black"},close:function(){$("iframe",this).attr("src","")}}),$("form a.membertype").live("click",function(n){n.preventDefault();var t=$("#memberDialog");$("iframe",t).attr("src",this.href),t.dialog("open")}),$("#previous-tab form a.membertype").live("click",function(n){n.preventDefault();var t=$("#memberDialog");$("iframe",t).attr("src",this.href),t.dialog("open")}),$(".CreateAndGo").click(function(){return confirm($(this).attr("confirm"))&&$.post($(this).attr("href"),null,function(n){window.location=n}),!1}),$("#enrollment-link").click(function(){$.showTable($("#current-tab form"))}),$("#previous-link").click(function(){$.showTable($("#previous-tab form"))}),$("#pending-link").click(function(){$.showTable($("#pending-tab form"))}),$("#attendance-link").click(function(){$.showTable($("#attendance-tab form"))}),$("#contacts-link").click(function(){$("#contacts-tab form").each(function(){$.showTable($(this))})}),$("#member-link").click(function(){var n=$("#memberdisplay");$("table",n).size()==0&&($.post(n.attr("action"),null,function(t){$(n).html(t).ready(function(){$.UpdateForSection(n),ShowMemberExtras()})}),$.showTable($("#extras-tab form")),$.extraEditable("#extravalues"))}),$("#system-link").click(function(){$.showTable($("#user-tab form"))}),$("#changes-link").click(function(){$.showTable($("#changes-tab form"))}),$("#volunteer-link").click(function(){$.showTable($("#volunteer-tab form"))}),$("#duplicates-link").click(function(){$.showTable($("#duplicates-tab form"))}),$("#optouts-link").click(function(){$.showTable($("#optouts-tab form"))}),$("#family table.grid > tbody > tr:even").addClass("alt"),$("#recreg-link").click(function(n){var t,i;return(n.preventDefault(),t=$("#recreg-tab form"),$("table",t).size()>0)?!1:(i=t.serialize(),$.post(t.attr("action"),i,function(n){$(t).html(n)}),!1)}),$("a.displayedit").live("click",function(n){n.preventDefault();var t=$(this).closest("form");return $.post($(this).attr("href"),null,function(n){$(t).html(n).ready(function(){$.UpdateForSection(t)})}),!1}),$.UpdateForSection=function(n){var t={minChars:3,matchContains:1};return $("#Employer",n).autocomplete("/Person/Employers",t),$("#School",n).autocomplete("/Person/Schools",t),$("#Occupation",n).autocomplete("/Person/Occupations",t),$("#NewChurch,#PrevChurch",n).autocomplete("/Person/Churches",t),$(".datepicker").datepicker({dateFormat:"m/d/yy",changeMonth:!0,changeYear:!0,buttonImage:"/images/calendar.gif",buttonImageOnly:!0}),$(".submitbutton,.bt").button(),$(".dropdown",n).hoverIntent(dropdownshow,dropdownhide),$("#verifyaddress").click(function(){var n=$(this).closest("form"),t=n.serialize();return $.post($(this).attr("href"),t,function(t){confirm(t.address+"\nUse this Address?")&&($("#Address1",n).val(t.Line1),$("#Address2",n).val(t.Line2),$("#City",n).val(t.City),$("#State",n).val(t.State),$("#Zip",n).val(t.Zip))}),!1}),!1},$("form.DisplayEdit a.submitbutton").live("click",function(n){var t,i;if(n.preventDefault(),t=$(this).closest("form"),$(t).valid())return i=t.serialize(),$.post($(this).attr("href"),i,function(n){$(t).html(n).ready(function(){var n=$("#businesscard");$.post($(n).attr("href"),null,function(t){$(n).html(t)}),$(".dropdown",t).hoverIntent(dropdownshow,dropdownhide),$(".submitbutton,.bt").button()})}),!1}),$("#future").live("click",function(n){n.preventDefault();var t=$(this).closest("form"),i=t.serialize();$.post($(t).attr("action"),i,function(n){$(t).html(n)})}),$("form.DisplayEdit").submit(function(){return $("#submitit").val()?!0:!1}),$.validator.setDefaults({highlight:function(n){$(n).addClass("ui-state-highlight")},unhighlight:function(n){$(n).removeClass("ui-state-highlight")},rules:{NickName:{maxlength:15},Title:{maxlength:10},First:{maxlength:25,required:!0},Middle:{maxlength:15},Last:{maxlength:100,required:!0},Suffix:{maxlength:10},AltName:{maxlength:100},Maiden:{maxlength:20},HomePhone:{maxlength:20},CellPhone:{maxlength:20},WorkPhone:{maxlength:20},EmailAddress:{maxlength:150},School:{maxlength:60},Employer:{maxlength:60},Occupation:{maxlength:60},WeddingDate:{date:!0},DeceasedDate:{date:!0},Grade:{number:!0},Address1:{maxlength:40},Address2:{maxlength:40},City:{maxlength:30},Zip:{maxlength:15},FromDt:{date:!0},ToDt:{date:!0}}}),$("#addrf").validate(),$("#addrp").validate(),$("#basic").validate(),$(".atck").live("change",function(){var t=$(this);$.post("/Meeting/MarkAttendance/",{MeetingId:$(this).attr("mid"),PeopleId:$(this).attr("pid"),Present:t.is(":checked")},function(n){if(n.error)t.attr("checked",!t.is(":checked")),alert(n.error);else{var i=t.closest("form"),r=i.serialize();$.post($(i).attr("action"),r,function(n){$(i).html(n)})}})}),$("#newvalueform").dialog({autoOpen:!1,buttons:{Ok:function(){var i=$("input[name='typeval']:checked").val(),n=$("#fieldname").val(),t=$("#fieldvalue").val();n&&$.post("/Person/NewExtraValue/"+$("#PeopleId").val(),{field:n,type:i,value:t},function(n){n.startsWith("error")?alert(n):($.getTable($("#extras-tab form")),$.extraEditable("#extravalues")),$("#fieldname").val(""),$("#fieldvalue").val("")}),$(this).dialog("close")}}}),$("#newextravalue").live("click",function(n){n.preventDefault();var t=$("#newvalueform");t.dialog("open")}),$("a.deleteextra").live("click",function(n){return n.preventDefault(),confirm("are you sure?")&&$.post("/Person/DeleteExtra/"+$("#PeopleId").val(),{field:$(this).attr("field")},function(n){n.startsWith("error")?alert(n):($.getTable($("#extras-tab form")),$.extraEditable("#extravalues"))}),!1}),$("a.reverse").live("click",function(n){n.preventDefault();var t=$(this).closest("form");$.post("/Person/Reverse",{id:$("#PeopleId").val(),field:$(this).attr("field"),value:$(this).attr("value"),pf:$(this).attr("pf")},function(n){$(t).html(n)})}),$.editable.addInputType("checkbox",{element:function(){var i=$('<input type="checkbox">');return $(this).append(i),$(i).click(function(){var n=$(i).attr("checked")?"True":"False";$(i).val(n)}),i},content:function(n){var f=n=="True"?!0:!1,r=$(":input:first",this),u;$(r).attr("checked",f),u=$(r).attr("checked")?"True":"False",$(r).val(u)}})})
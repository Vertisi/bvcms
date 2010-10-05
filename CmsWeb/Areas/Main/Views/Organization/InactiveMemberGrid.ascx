﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CmsWeb.Models.OrganizationPage.MemberModel>" %>
<% Html.RenderPartial("ExportToolBar"); %>
&nbsp;<div style="clear: both"></div>
<p style="bottom-margin:5px">
Count: <strong><%=Model.Count() %></strong>
<a class="filtergroupslink" href="#">Filter by name/group<%= Model.isFiltered ? "(filtered)" : "" %></a>
<% if(Page.User.IsInRole("Edit"))
   { %>
<a href="/OrgMembersDialog/Index/<%=Model.OrganizationId %>?inactives=true" title="Update Inactive Members" class="memberdialog">update</a>
<% } %>
</p>
<% Html.RenderPartial("MemberGrid", Model); %>
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CmsData;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using UtilityExtensions;
using Dapper;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin,manageemails")]
    public class UsersCanEmailForController : CmsStaffController
    {
        public ActionResult Index()
        {
            var q = from cf in DbUtil.Db.PeopleCanEmailFors
                    orderby cf.PersonCanEmail.Name2, cf.OnBehalfOfPerson.Name2
                    select cf;
            return View(q);
        }
        public ActionResult PersonCanEmailForList(int id)
        {
            Response.NoCache();
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.SubmitChanges();
            if (id > 0)
            {
                var q = from cf in DbUtil.Db.PeopleCanEmailFors
                        where cf.CanEmail == id
                        select cf.OnBehalfOf;
                t.PersonTags.Add(new TagPerson { PeopleId = id });
                foreach (var pid in q)
                    t.PersonTags.Add(new TagPerson { PeopleId = pid });
                DbUtil.Db.SubmitChanges();
                return Redirect("/SearchUsers?ordered=true&topid=" + id);
            }
            return Redirect("/SearchUsers?ordered=true");
        }

        [HttpPost]
        public ActionResult UpdatePersonCanEmailForList(int id, int topid0)
        {
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var selected_pids = (from p in t.People(DbUtil.Db)
                                 where p.PeopleId != id
                                 select p.PeopleId).ToArray();
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.Tags.DeleteOnSubmit(t);
            DbUtil.Db.SubmitChanges();
            if (topid0 == id)
            {
                var cn = new SqlConnection(DbUtil.Db.Connection.ConnectionString);
                cn.Open();
                cn.Execute("delete PeopleCanEmailFor where CanEmail = @id", new {id});
            }
            foreach(var pid in selected_pids)
                DbUtil.Db.PeopleCanEmailFors.InsertOnSubmit(new PeopleCanEmailFor{ CanEmail = id, OnBehalfOf = pid});
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
    }
}

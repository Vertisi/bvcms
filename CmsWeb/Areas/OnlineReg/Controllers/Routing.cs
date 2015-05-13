using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private ActionResult RouteRegistration(OnlineRegModel m, int pid, bool? showfamily)
        {
            if(pid == 0)
                return View(m);

            var link = RouteExistingRegistration(m, pid);
            if (link.HasValue())
                return Redirect(link);

            OnlineRegPersonModel p = null;
            if (showfamily != true)
            {
                // No need to pick family, so prepare first registrant ready to answer questions
                p = m.LoadExistingPerson(pid, 0);
                p.ValidateModelForFind(ModelState, 0);
                if (m.masterorg == null)
                {
                    if (m.List.Count == 0)
                        m.List.Add(p);
                    else
                        m.List[0] = p;
                }
            }
            if (!ModelState.IsValid)
                return View(m);


            link = RouteManageGivingSubscriptionsPledgeVolunteer(m);
            if(link.HasValue())
                if (m.ManageGiving()) // use Direct ActionResult instead of redirect
                    return ManageGiving(m.Orgid.ToString(), m.testing);
                else
                    return Redirect(link);

            // check for forcing show family, master org, or not found
            if (showfamily == true || p.org == null || p.Found != true) 
                return View(m);

            // ready to answer questions, make sure registration is ok to go
            if (!m.SupportMissionTrip)
                p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
            if (p.IsFilled)
                ModelState.AddModelError(m.GetNameFor(mm => mm.List[0].Found), "Sorry, but registration is closed.");

            p.FillPriorInfo();
            p.SetSpecialFee();

            m.HistoryAdd("index, pid={0}, !showfamily, p.org, found=true".Fmt(pid));
            return View(m);
        }

        private string RouteManageGivingSubscriptionsPledgeVolunteer(OnlineRegModel m)
        {
            TempData["PeopleId"] = Util.UserPeopleId;
            if (m.ManageGiving())
                return "/OnlineReg/ManageGiving/{0}".Fmt(m.Orgid);
            if (m.ManagingSubscriptions())
                return "/OnlineReg/ManageSubscriptions/{0}".Fmt(m.masterorgid);
            if (m.OnlinePledge())
                return "/OnlineReg/ManagePledge/{0}".Fmt(m.Orgid);
            if (m.ChoosingSlots())
                return "/OnlineReg/ManageVolunteer/{0}".Fmt(m.Orgid);
            TempData.Remove("PeopleId");
            return null;
        }

        private string RouteExistingRegistration(OnlineRegModel m, int? pid = null)
        {
            var existingRegistration = m.GetExistingRegistration(pid ?? Util.UserPeopleId ?? 0);
            if (existingRegistration == null) 
                return null;
            TempData["PeopleId"] = m.UserPeopleId = Util.UserPeopleId;
            return "/OnlineReg/Existing/" + existingRegistration.DatumId;
        }
        private ActionResult RouteSpecialLogin(OnlineRegModel m)
        {
            if (Util.UserPeopleId == null)
                throw new Exception("Util.UserPeopleId is null on login");

            var link = RouteExistingRegistration(m);
            if(link.HasValue())
                return Content(link);

            m.CreateAnonymousList();
            m.UserPeopleId = Util.UserPeopleId;

            if (m.OnlineGiving())
                return RegisterFamilyMember(Util.UserPeopleId.Value, m);

            link = RouteManageGivingSubscriptionsPledgeVolunteer(m);
            if (link.HasValue())
                return Content(link); // this will be used for a redirect in javascript

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Controllers;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public OnlineRegModel()
        {
            HttpContext.Current.Items["OnlineRegModel"] = this;
        }

        public OnlineRegModel(HttpRequestBase req, int? id, bool? testing, string email, bool? login, string source)
            : this()
        {
#if DEBUG
            DebugCleanUp();
#endif
            Orgid = id;
            if (req.Url != null)
                URL = req.Url.OriginalString;

            if (DbUtil.Db.Roles.Any(rr => rr.RoleName == "disabled"))
                throw new Exception("Site is disabled for maintenance, check back later");
            if (!id.HasValue)
                throw new Exception("no organization");

            MobileAppMenuController.Source = source;

            if (org == null && masterorg == null)
                throw new Exception("invalid registration");

            if (masterorg != null)
            {
                if (!UserSelectClasses(masterorg).Any())
                    throw new Exception("no classes available on this org");
            }
            else if (org != null)
            {
                if ((org.RegistrationTypeId ?? 0) == RegistrationTypeCode.None)
                    throw new Exception("no registration allowed on this org");
            }
            this.testing = testing == true || DbUtil.Db.Setting("OnlineRegTesting", Util.IsDebug() ? "true" : "false").ToBool();

            // the email passed in is valid or they did not specify login
            if (AllowAnonymous && (Util.ValidEmail(email) || login != true)) 
                nologin = true;

            if (nologin)
                CreateAnonymousList();
            else
                List = new List<OnlineRegPersonModel>();

            // prepopulate their email address they passed in
            if (Util.ValidEmail(email))
                List[0].EmailAddress = email;

            HistoryAdd("index");
        }
        public void CreateAnonymousList()
        {
            List = new List<OnlineRegPersonModel>
                {
                    new OnlineRegPersonModel
                        {
                            orgid = Orgid,
                            masterorgid = masterorgid,
#if DEBUG
                            FirstName = "David",
                            LastName = "Carroll", // + DateTime.Now.Millisecond,
                            DateOfBirth = "5/30/52",
                            EmailAddress = "david@bvcms.com",
#endif
                        }
                };
        }
    }
}

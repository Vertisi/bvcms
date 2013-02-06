using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Main.Models.Report;
using CmsData.Codes;
using System.Collections;

namespace CmsWeb.Models
{
	public class MeetingModel
	{
		public CmsData.Meeting meeting;
		public CmsData.Organization org;

		public bool showall { get; set; }
		public bool currmembers { get; set; }
		public bool showregister { get; set; }
		public bool showregistered { get; set; }
		public bool sortbyname { get; set; }

		public MeetingModel(int id)
		{
		    var i = (from m in DbUtil.Db.Meetings
		            where m.MeetingId == id
		            select new
		                       {
		                           org = m.Organization,
		                           m
		                       }).Single();
		    meeting = i.m;
		    org = i.org;
		}
		public IEnumerable<RollsheetModel.AttendInfo> Attends(bool sorted = false)
		{
			return RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value, sorted, currmembers);
		}
		public IEnumerable<RollsheetModel.AttendInfo> VisitAttends(bool sorted = false)
		{
			var q = RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value, sorted);
			return q.Where(vv => !vv.Member);
		}
		public string AttendCreditType()
		{
			if (meeting.AttendCredit == null)
				return "Every Meeting";
			return meeting.AttendCredit.Description;
		}
		public bool HasRegistered()
		{
			return meeting.Attends.Any(aa => aa.Commitment != null);
		}
		public static IEnumerable AttendCommitments()
		{
			var q = CmsData.Codes.AttendCommitmentCode.GetCodePairs();
			return q.ToDictionary(k => k.Key.ToString(), v => v.Value);
		}
	}
}
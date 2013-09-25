/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CmsData;
using System.Data.Linq;
using UtilityExtensions;
using System.Collections;

namespace CmsWeb.Areas.Search.Models
{
    public class MailingController
    {
        public bool UseTitles { get; set; }

        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchIndividualList(string sortExpression, Guid QueryId)
        {
            var q = DbUtil.Db.PeopleQuery(QueryId);
            var q2 = from p in q
                     where p.DeceasedDate == null
                     where p.PrimaryBadAddrFlag != 1
                     where p.DoNotMailFlag == false
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name,
                         Name = p.Name,
                         LastName = p.LastName,
                         PeopleId = p.PeopleId,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }
        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> GroupByAddress(Guid QueryId)
        {
            var q = DbUtil.Db.PeopleQuery(QueryId);
            var q2 = from p in q
                     where p.DeceasedDate == null
                     where p.PrimaryBadAddrFlag != 1
                     where p.DoNotMailFlag == false
                     group p by p.FamilyId into g
                     let one = g.First().Family.HeadOfHousehold
                     let last = one.LastName
                     orderby one.ZipCode
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = one.PrimaryAddress,
                         Address2 = one.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(one.PrimaryCity, one.PrimaryState, one.PrimaryZip),
                         City = one.PrimaryCity,
                         State = one.PrimaryState,
                         Zip = one.PrimaryZip,
                         LabelName = Regex.Replace(string.Join(", ", g.Select(vv => vv.PreferredName)), "(.*)(,)([^,]*$)", "$1 &$3", RegexOptions.IgnoreCase),
                         Name = one.Name,
                         LastName = one.LastName,
                         CellPhone = "",
                         HomePhone = one.HomePhone,
                     };
            return q2;
        }

        private int _FamilyCount;
        public int FamilyCount(int startRowIndex, int maximumRows, string sortExpression, Guid QueryId)
        {
            return _FamilyCount;
        }

        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchFamilyList(int startRowIndex, int maximumRows, string sortExpression, Guid QueryId)
        {
            var q1 = DbUtil.Db.PeopleQuery(QueryId);
            var q = from f in DbUtil.Db.Families
                    where q1.Any(p => p.FamilyId == f.FamilyId)
                    select f.People.Single(fm => fm.PeopleId == f.HeadOfHouseholdId);

            _FamilyCount = q.Count();
            var q2 = from h in q
                     let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == h.SpouseId)
                     where h.DeceasedDate == null
                     where h.PrimaryBadAddrFlag != 1
                     where h.DoNotMailFlag == false
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = h.PrimaryAddress,
                         Address2 = h.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(h.PrimaryCity, h.PrimaryState, h.PrimaryZip),
                         City = h.PrimaryCity,
                         State = h.PrimaryState,
                         Zip = h.PrimaryZip,
                         LabelName = (h.Family.CoupleFlag == 1 ? (UseTitles ? (h.TitleCode != null ? h.TitleCode + " and Mrs. " + h.Name
                                                                                                    : "Mr. and Mrs. " + h.Name)
                                                                             : h.PreferredName + " and " + spouse.PreferredName + " " + h.LastName + (h.SuffixCode.Length > 0 ? ", " + h.SuffixCode : "")) :
                                      h.Family.CoupleFlag == 2 ? ("The " + h.Name + " Family") :
                                      h.Family.CoupleFlag == 3 ? ("The " + h.Name + " Family") :
                                      h.Family.CoupleFlag == 4 ? (h.Name + " & Family") :
                                      UseTitles ? (h.TitleCode != null ? h.TitleCode + " " + h.Name : h.Name) : h.Name),
                         Name = h.Name,
                         LastName = h.LastName,
                         CellPhone = h.CellPhone,
                         HomePhone = h.HomePhone,
                         PeopleId = h.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            if (maximumRows == 0)
                return q2;
            return q2.Skip(startRowIndex).Take(maximumRows);
        }
        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchFamilyMembers(string sortExpression, Guid QueryId)
        {
			var q = DbUtil.Db.PeopleQuery(QueryId);
			var q2 = from pp in q
					 group pp by pp.FamilyId into g
					 from p in g.First().Family.People
					 where p.DeceasedDate == null
					 let famname = g.First().Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).LastName
					 orderby famname, p.FamilyId, p.PositionInFamilyId, p.GenderId
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name),
                         Name = p.Name,
                         LastName = p.LastName,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
			return q2;
        }
        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchFamilyList(string sortExpression, Guid QueryId)
        {
            int startRowIndex = 0;
            int maximumRows = 0;
            return FetchFamilyList(startRowIndex, maximumRows, sortExpression, QueryId);
        }

        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchParentsOfList(string sortExpression, Guid QueryId)
        {
            var q = DbUtil.Db.PeopleQuery(QueryId);
            var q2 = from p in q
                     where p.DeceasedDate == null
                     where p.PrimaryBadAddrFlag != 1
                     where p.DoNotMailFlag == false
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (p.PositionInFamilyId == 30 ? ("Parents Of " + p.Name) : UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name),
                         Name = p.Name,
                         LastName = p.LastName,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        private const string STR_Couples = "couples";

        private static IQueryable<Person> EliminateCoupleDoublets(IQueryable<Person> q)
        {
            var q1 = from p in q
                     // exclude wife who has a husband who is already in the list
                     where !(p.GenderId == 2 && p.SpouseId != null && q.Any(pp => pp.PeopleId == p.SpouseId))
                     where p.DeceasedDate == null
                     where (p.BadAddressFlag == null || p.BadAddressFlag == false)
                     where p.DoNotMailFlag == false
                     select p;
            return q1;
        }

        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchCouplesEitherList(string sortExpression, Guid QueryId)
        {
            var q = DbUtil.Db.PopulateSpecialTag(QueryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                     let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         Name = p.Name,
                         LastName = p.LastName,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        public IEnumerable<CmsWeb.Models.MailingController.MailingInfo> FetchCouplesBothList(string sortExpression, Guid QueryId)
        {
            var q = DbUtil.Db.PopulateSpecialTag(QueryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                     // get spouse if in the query
                     let spouse = q.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new CmsWeb.Models.MailingController.MailingInfo
                     {
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         Name = p.Name,
                         LastName = p.LastName,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        public IQueryable<CmsWeb.Models.MailingController.MailingInfo> ApplySort(IQueryable<CmsWeb.Models.MailingController.MailingInfo> query, string sortExpression)
        {
            switch (sortExpression)
            {
                case "Name":
                    return query.OrderBy(mi => mi.LastName);
                case "Zip":
                    return query.OrderBy(mi => mi.Zip);
                //break;
                default:
                    break;
            }
            return query;
        }

        public IEnumerable FetchExcelCouplesBoth(Guid QueryId, int maximumRows)
        {
            var q = DbUtil.Db.PopulateSpecialTag(QueryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                     // get spouse if in the query
                     let spouse = q.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new
                     {
                         PeopleId = p.PeopleId,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         FirstName = p.PreferredName,
                         FirstNameSpouse = spouse != null ? spouse.PreferredName : "",
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         EmailSpouse = spouse != null ? spouse.EmailAddress : "",
                         HomePhone = p.Family.HomePhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         Employer = p.EmployerOther,
                     };
            return q2.Take(maximumRows);
        }

        public IEnumerable FetchExcelCouplesEither(Guid QueryId, int maximumRows)
        {
            var q = DbUtil.Db.PopulateSpecialTag(QueryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                     let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new
                     {
                         PeopleId = p.PeopleId,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         FirstName = p.PreferredName,
                         FirstNameSpouse = spouse != null ? spouse.PreferredName : "",
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         EmailSpouse = spouse != null ? spouse.EmailAddress : "",
                         MemberStatus = p.MemberStatus.Description,
                         Employer = p.EmployerOther,
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                     };
            return q2.Take(maximumRows);
        }

        public IEnumerable FetchExcelFamily(Guid QueryId, int maximumRows)
        {
            var qp = DbUtil.Db.PeopleQuery(QueryId);

            var q = from f in DbUtil.Db.Families
                    where qp.Any(p => p.FamilyId == f.FamilyId)
                    select f.People.Single(fm => fm.PeopleId == f.HeadOfHouseholdId);

            return (from h in q
                    let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == h.SpouseId)
                    where h.DeceasedDate == null
                    where h.PrimaryBadAddrFlag != 1
                    where h.DoNotMailFlag == false
                    select new
                    {
                        LabelName = (h.Family.CoupleFlag == 1 ? (UseTitles ? (h.TitleCode != null ? h.TitleCode + " and Mrs. " + h.Name
                                                                                                   : "Mr. and Mrs. " + h.Name)
                                                                            : h.PreferredName + " and " + spouse.PreferredName + " " + h.LastName + (h.SuffixCode.Length > 0 ? ", " + h.SuffixCode : "")) :
                                     h.Family.CoupleFlag == 2 ? ("The " + h.Name + " Family") :
                                     h.Family.CoupleFlag == 3 ? ("The " + h.Name + " Family") :
                                     h.Family.CoupleFlag == 4 ? (h.Name + " & Family") :
                                     UseTitles ? (h.TitleCode != null ? h.TitleCode + " " + h.Name : h.Name) : h.Name),
                        Name = h.Name,
                        LastName = h.LastName,
                        Address = h.PrimaryAddress,
                        Address2 = h.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ4(h.PrimaryCity, h.PrimaryState, h.PrimaryZip),
                        City = h.PrimaryCity,
                        State = h.PrimaryState,
                        Zip = h.PrimaryZip,
                    }).Take(maximumRows);
        }

        public IEnumerable FetchExcelParents(Guid QueryId, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(QueryId);
            var q2 = from p in q
                     where p.DeceasedDate == null
                     where p.PrimaryBadAddrFlag != 1
                     where p.DoNotMailFlag == false
                     let hohemail = p.Family.HeadOfHousehold.EmailAddress
                     select new
                     {
                         LabelName = (p.PositionInFamilyId == 30 ? ("Parents Of " + p.Name) : p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name),
                         Name = p.Name,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         ParentEmail = (p.PositionInFamilyId == 30 ?
                            (hohemail != null && hohemail != "" ?
                                hohemail
                                : p.Family.HeadOfHouseholdSpouse.EmailAddress)
                            : p.EmailAddress)
                     };
            return q2.Take(maximumRows);
        }
    }
}

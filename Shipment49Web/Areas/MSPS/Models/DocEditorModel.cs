using MSMPmodule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Models
{
    public class DocEditorModel
    {
        public Revision Revisions { get; set; }

        public DocsPages DocumentPages { get; set; }


    }



    public class DocsPagesModel
    {
        [Key]
        public int Id { get; set; }

        public int ShipId { get; set; }
        // public int? DocsID { get; set; }

        public int Mid { get; set; }
        public int Subid { get; set; }
        public int SubSubid { get; set; }
        //public string PageTitle { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
        [AllowHtml]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }
    public class TempDocsPageModel
    {
        public TempDocsPage tempdoc { get; set; }

    }


    public class DocumentIndex
    {
        public Pager Pager { get; set; }

        public List<RevisionDetails> docListing { get; set; }

        public Int32? Total { get; set; }
    }

    public class MenuIndex
    {
        public Pager Pager { get; set; }

        public List<MenusDetails> menuListing { get; set; }

        public List<SubMenusDetails> submenuListing { get; set; }
        public List<SubSubMenusDetails> subsubmenuListing { get; set; }

        public Int32? Total { get; set; }
    }
    public class TotalCount
    {
        public Int32? Total { get; set; }
    }
    public class SubSubMenusDetails
    {
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string SubSubMenuName { get; set; }
        public int SubId { get; set; }
        public int MId { get; set; }
        public int SubSubId { get; set; }
    }
    public class SubMenusDetails
    {
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public int SubId { get; set; }
        public int MId { get; set; }
    }
    public class SmartMenusDetails
    {
        public string HtmlContent { get; set; }

        public string SmartMenuContent { get; set; }

        public string DynamicMenuText { get; set; }
        public string DynamicMenuTextEdit { get; set; }
        // public string HyperLink { get; set; }
        public int Id { get; set; }
        public int MaxId { get; set; }

    }
    public class MenusDetails
    {

        public string MenuName { get; set; }
        public int MId { get; set; }
    }
    public class RevisionDetails
    {
        public decimal RNumber { get; set; }
        public string PageTitle { get; set; }
        public int docsid { get; set; }
    }

    public class ContentDetails
    {
        public int Id { get; set; }

        public int MId { get; set; }
        public int SubId { get; set; }
        public int SubSubId { get; set; }
        public int DocsId { get; set; }
        public string PageTitle { get; set; }
        public string ShipId { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        [AllowHtml]
        public string Content1 { get; set; }
        public string ContentPath { get; set; }
        public string CreateBy { get; set; }
        public string Status { get; set; }

        public decimal? RNumber { get; set; }
        public string RPrefix { get; set; }

        public string RevisionType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReviseDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string RevisionText { get; set; }
    }
    public class DetailsViewModel
    {
        public int Mid { get; set; }
        public int Subid { get; set; }
        public int SubSubid { get; set; }
        public string Content { get; set; }
        public string BreadCrumb { get; set; }
        public string Content1 { get; set; }
        public int ShipId { get; set; }
        public string HtmlContent { get; set; }

        public string SmartMenuContent { get; set; }
        public int Id { get; set; }

        public ShipSpecificModel shipmodel { get; set; }

        public RevisionModel.MasterRevisionModel masterrivisionmodel { get; set; }
        public List<ShipSpecificModel> shipattListing { get; set; }
        public List<RevisionModel.MasterRevisionModel> masterrivision { get; set; }

        public List<SelectListItem> RevList { get; set; }

        public List<ApproveIndex> revdetails { get; set; }
    }
    public class ApproveIndex
    {
        public Pager Pager { get; set; }

        public List<ContentDetails> contentListing { get; set; }

        public Int32? Total { get; set; }
    }

    public class RevisionIndex
    {
        public Pager Pager { get; set; }

        public List<RevisionListing> revisionListing { get; set; }

        public List<RevisionView> revisionViewListing { get; set; }

        public string Content { get; set; }
        public string Content1 { get; set; }
        public Int32? Total { get; set; }
    }
    public class RevisionListing
    {
        public int Id { get; set; }
        public int DocsId { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public decimal RNumber { get; set; }
        public string CreatedBy { get; set; }
        public string DocumentName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }



    }

    public class RevisionView
    {
        public int Id { get; set; }
        public int DocsId { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public decimal RNumber { get; set; }
        public string CreatedBy { get; set; }
        public string DocumentName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }



    }

    public class Child2
    {
        public int deleted { get; set; }
        public int @new { get; set; }
        public int slug { get; set; }
        public string href { get; set; }
        public int type { get; set; }
        public string text { get; set; }
        public string prefix { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Child
    {
        public Child()
        {
            children = new List<Child2>();
        }

        public int deleted { get; set; }
        public int @new { get; set; }
        public int slug { get; set; }
        public string href { get; set; }
        public int type { get; set; }
        public string text { get; set; }
        public string prefix { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<Child2> children { get; set; }
    }

    public class RootObject
    {
        public RootObject()
        {
            children = new List<Child>();
        }

        public int deleted { get; set; }
        public int @new { get; set; }
        public int slug { get; set; }
        public string href { get; set; }
        public int type { get; set; }
        public string text { get; set; }
        public string prefix { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<Child> children { get; set; }
    }
}
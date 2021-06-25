using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuLayer
{
    [Table("TblMenu")]
    public class Menu
    {
        public Menu()
        {
            Menus1 = new List<SubMenu>();
            // Menus2 = new List<SubSubMenu>();

        }

        [Key]
        public int MId { get; set; }
        public string MenuName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string AreaName { get; set; }
        public string Role { get; set; }
        public List<SubMenu> Menus1 { get; set; }
        // [NotMapped]
        // public List<SubSubMenu> Menus2 { get; set; }


    }

    [Table("TblSubMenu")]
    public class SubMenu
    {
        public SubMenu()
        {
            Menus2 = new List<SubSubMenu>();

        }
        [Key]
        public int SubId { get; set; }
        public int MId { get; set; }
        public string SubMenuName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string AreaName { get; set; }
        public string Role { get; set; }
        public List<SubSubMenu> Menus2 { get; set; }
    }

    [Table("TblSubSubMenu")]
    public class SubSubMenu
    {

        [Key]
        public int SubSubId { get; set; }
        public int SubId { get; set; }
        public int MId { get; set; }
        public string SubSubMenuName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string AreaName { get; set; }
        public string Role { get; set; }

    }

    [Table("tblSmartMenus")]
    public class SmartMenu
    {

        [Key]

        public int Id { get; set; }
        public string SmartMenuContent { get; set; }
        public string HtmlContent { get; set; }
        public string SmartMenuContentExport { get; set; }
        //public string HtmlContentExport { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    [Table("tblShipSpecificContent")]
    public class ShipSpecificContent
    {

        [Key]

        public int Id { get; set; }
        public int MId { get; set; }
        public string ShipId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    [Table("tblShipSpecificAttachment")]
    public class ShipSpecificAttachment
    {

        [Key]

        public int Id { get; set; }
        public string AttachmentName { get; set; }
        public string Attachment { get; set; }
        public int MId { get; set; }
        public string ShipId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }

    [Table("tblMasterRevision")]
    public class MasterRevision
    {

        [Key]

        public int Id { get; set; }
        public string MasterRevisionNo { get; set; }
        public string RevisionsIncluded { get; set; }
        public DateTime? CreatedDate { get; set; }


    }

    public class UserRole
    {
        public static int UserNc { get; set; }
        public static int UserCNc { get; set; }
        public static bool CheckAnalysis { get; set; }
        public static string FullName { get; set; }
        public static string username { get; set; }
        public static string username1 { get; set; }
        public static List<Menu> GetMenu { get; set; }
    }

    public enum CommonType { RopeManufacturer = 1, FleetName, FleetType, TradePlatform }

    [Table("tblCommon")]
    public class MasterCommon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public CommonType Type { get; set; }
        //[NotMapped]
        //public string ViewName { get; set; }
    }

    [Table("MooringRopeType")]
    public class RopeTypeCommon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RopeType { get; set; }
       
    }

    //public enum MooringRopeType { SteelWire = 1, Polypropylene, Polyster, Nylon, Syntheticfibre, Braided, Hemp, Sisal, Coir, Manila, Polythylene, HMPE, Polysteel, Kapa }

    [Table("tblRopeInspectionSetting")]
    public class RopeInspectionSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MooringRopeType { get; set; }
        [Required]
        public int ManufacturerType { get; set; }
        public int? MaximumRunningHours { get; set; }
        public int? MaximumMonthsAllowed { get; set; }
        public int? EndToEndMonth { get; set; }
        public int? RotationOnWinches { get; set; }
        [Required]
        public decimal Rating1 { get; set; }
        [Required]
        public decimal Rating2 { get; set; }
        [Required]
        public decimal Rating3 { get; set; }
        [Required]
        public decimal Rating4 { get; set; }
        [Required]
        public decimal Rating5 { get; set; }
        [Required]
        public decimal Rating6 { get; set; }
        [Required]
        public decimal Rating7 { get; set; }

    }

    [Table("tblRopeTailInspectionSetting")]
    public class RopeTailInspectionSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MooringRopeType { get; set; }
        [Required]
        public int ManufacturerType { get; set; }
        public int? MaximumRunningHours { get; set; }
        public int? MaximumMonthsAllowed { get; set; }
        [Required]
        public decimal Rating1 { get; set; }
        [Required]
        public decimal Rating2 { get; set; }
        [Required]
        public decimal Rating3 { get; set; }
        [Required]
        public decimal Rating4 { get; set; }
        [Required]
        public decimal Rating5 { get; set; }
        [Required]
        public decimal Rating6 { get; set; }
        [Required]
        public decimal Rating7 { get; set; }

    }

    [Table("tblWinchRotationSetting")]
    public class WinchRotationSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MooringRopeType { get; set; }
        [Required]
        public int ManufacturerType { get; set; }
        [Required]
        public int MaximumRunningHours { get; set; }
        [Required]
        public int MaximumMonthsAllowed { get; set; }
        [Required]
        public string LeadFrom { get; set; }
        [Required]
        public string LeadTo { get; set; }
        [Required]
        public int VesselID { get; set; }
        

    }



    //public enum EquipmentType { JoiningShackle = 1, FireWire, Ropetail, MessengerRope, RopeStopper, ChainStopper, ChafeGuard, WinchBreakTestKit }

    [Table("tblLooseEquipInspectionSetting")]
    public class LooseEquipInspectionSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EquipmentType { get; set; }

        [Required]
        public int InspectionFrequency { get; set; }
        public int? MaximumRunningHours { get; set; }
        public int? MaximumMonthsAllowed { get; set; }

    }

    //[Table("tblVessel")]
    //public class VesselInfo
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    [Required]
    //    public string Name { get; set; }
    //    [Required]
    //    public int ImoNo { get; set; }
    //    public string Flag { get; set; }
    //    public int FleetName { get; set; }
    //    public int FleetType { get; set; }
    //    [DisplayName("Trade Platform")]
    //    public int TradePlatform { get; set; }
    //    public string DateBuilt { get; set; }
    //}

    public enum NotificationType
    {

        [Description("Rope Inspection")]
        One = 1,
        [Description("Inspection")]
        Two = 2,
        [Description("Rating 6")]
        Three = 3,
        [Description("Rating 7")]
        Four = 4,
        [Description("Rope running hours exceeded")]
        Five = 5,
        [Description("Rope replacement due")]
        Six = 6,
        [Description("Loose equipment inspection")]
        Seven = 7,
        [Description("Loose equipment running hours exceeded")]
        Eight = 8,
        [Description("Loose equipment replacement due")]
        Nine = 9,
        [Description("Cropped")]
        Ten = 10,
        [Description("Spliced")]
        Eleven = 11,
        [Description("Damaged Rope")]
        Twelve = 12,
        [Description("Out of Service Rope")]
        Thirteen = 13,
        [Description("Damaged loose equipment")]
        Fourteen = 14,
        [Description("Discarded loose equipment")]
        Fifteen = 15,
        [Description("Disposed Rope")]
        Sixteen = 16,
        [Description("Disposed loose equipment")]
        Seventeen = 17
    }

    [Table("tblNotification")]
    public class NotificationInfo
    {
        [Key]
        public int Id { get; set; }
        public string Notification { get; set; }
        public int NotificationId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string ShipActionTaken { get; set; }
        public bool Acknowledge { get; set; }
        public string AckRecord { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }

        public int VesselId { get; set; }
    }
}

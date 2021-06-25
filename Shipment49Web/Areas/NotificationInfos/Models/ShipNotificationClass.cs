using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.NotificationInfos.Models
{
    public class ShipNotificationClass
    {
        public int Id { get; set; }
        public int VesselId { get; set; }
        public int RopeId { get; set; }

        public int NotificationType { get; set; }
        public string LooseCertificateNum { get; set; }
        //public int NotificationAlertType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Acknowledge { get; set; }
        public string Notification { get; set; }
        public string AckRecord { get; set; }


        public int OfficeComments { get; set; }
        public int ShipComments { get; set; }

        public string ShipC { get; set; }

        public string OfficeC { get; set; }

        public string ShipCmnt { get; set; }

        public string OfficeCmnt { get; set; }
        public int? IsRead { get; set; }

        public Int32? totalComment { get; set; }
        public Int32? totalNotA { get; set; }
        public Int32? totalA { get; set; }
        public List<ShipNotificationClass> ShipnotificationList { get; set; }

        public DateTime? SearchFrom { get; set; }
        public DateTime? SearchTo{ get; set; }

        public string Archive { get; set; } = "NotificationsListingkk_Archive";
    }
}
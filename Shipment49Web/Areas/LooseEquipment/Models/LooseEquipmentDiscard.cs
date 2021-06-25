using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reports;

namespace Shipment49Web.Areas.LooseEquipment.Models
{
    public class LooseEquipmentDiscard
    {
        public LooseEquipmentDiscard()
        {
            GetAllLE_Detail = new List<SelectListItem>();
            ReasonOutofServices  = new List<OutofServiceR>();
            DamageObservedLists = new List<DamageObserved>();
            MooringOperationsLists = new List<MOperationBirthDetail>();

        }
        public int Id { get; set; }
        public int LooseETypeId { get; set; }

        public DateTime OutofServiceDate { get; set; }

        public List<SelectListItem> GetAllLE_Detail { get; set; }

        public List<OutofServiceR> ReasonOutofServices { get; set; }

        public List<DamageObserved> DamageObservedLists { get; set; }

        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Shipment49Web
{
    public partial class alertscaller : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(GetGizmosSvcAsync));
        }

        private async Task GetGizmosSvcAsync()
        {
            //  var gizmoService = new GizmoService();
            //GizmosGridView.DataSource = await gizmoService.GetGizmosAsync();
            //GizmosGridView.DataBind();

            NotificationAlert controller = new NotificationAlert();
            await controller.MasterCall_Notification();
        }


    }
}
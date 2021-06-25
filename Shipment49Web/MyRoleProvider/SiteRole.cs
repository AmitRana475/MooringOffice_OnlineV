using CertificationLayer;
using MenuLayer;
using NotificationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using UserLayer;

namespace Shipment49Web.MyRoleProvider
{
    public class SiteRole : RoleProvider
    {

        //private readonly  ShipmentContaxt emp;

        //delegate int NumberChanger(List<NotificationClass> b);
        //delegate int NumberChanger1(List<CertificationClass> b);
        //delegate string NumberChanger2(List<LoginEmp> b);

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        private readonly Func<ShipmentContaxt, List<NotificationClass>> UserNc = c => c.Notifications.ToList();
        private readonly Func<ShipmentContaxt, List<CertificationClass>> UserCNc = c => c.Certifications.ToList();
        private readonly Func<ShipmentContaxt, List<LoginEmp>> BMdata = c => c.LoginEmps.ToList();
        private readonly Func<ShipmentContaxt, List<UserClass>> BMdata1 = c => c.Users.ToList();



        public override string[] GetRolesForUser(string username)
        {

            try
            {
                using (ShipmentContaxt emp = new ShipmentContaxt())
                {
                    emp.Configuration.ProxyCreationEnabled = false;

                    /*
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    
                    NumberChanger nc = delegate (List<NotificationClass> b)
                    {
                        return b.Select(p => new { p.VesselId, p.NonConfirmity, p.AcknDate }).Where(x => x.NonConfirmity != null && x.AcknDate == null).Count();
                    };

                    NumberChanger1 ncc = delegate (List<CertificationClass> b)
                    {
                        return b.Select(p => new { p.VesselId, p.AlertFrequency, p.AcknDate }).Where(x => x.AlertFrequency != null && x.AcknDate == null).Count();
                    };

                    NumberChanger2 uname = delegate (List<LoginEmp> b)
                    {
                        return b.Where(x => x.UserName == username).FirstOrDefault().Role;
                    };

                    UserRole.UserNc = nc(UserNc(emp));
                    UserRole.UserCNc = ncc(UserCNc(emp));
                    UserRole.username = uname(BMdata(emp));


                    string bms11 = sw1.ElapsedMilliseconds.ToString();
                    sw1.Stop();
                    */

                    //..................................



                    if (username.ToLower() == "admin")
                    {
                        UserRole.UserNc = UserNc.Invoke(emp).Select(p => new { p.VesselId, p.NonConfirmity, p.AcknDate }).Where(x => x.NonConfirmity != null && x.AcknDate == null).Count();
                        UserRole.UserCNc = UserCNc.Invoke(emp).Select(p => new { p.VesselId, p.AlertFrequency, p.AcknDate }).Where(x => x.AlertFrequency != null && x.AcknDate == null).Count();

                        string data = BMdata.Invoke(emp).Where(x => x.UserName == username).FirstOrDefault().UserRole;
                        string[] result = { data };
                        UserRole.username = data;
                        UserRole.FullName = data;
                        return result;


                    }
                    else
                    {
                        //string data = BMdata1.Invoke(emp).Where(x => x.FullName == username).FirstOrDefault().Role;
                        string data = BMdata1.Invoke(emp).Where(x => x.EmailId == username).FirstOrDefault().Role;
                        string[] result = { data };
                        UserRole.username = data;
                        UserRole.FullName =  BMdata1.Invoke(emp).Where(x => x.EmailId.Equals(username)).Select(x => x.FullName).FirstOrDefault();

                        //var vsname = BMdata1.Invoke(emp).Where(x => x.FullName.Equals(username)).Select(x => x.VesselName).FirstOrDefault();
                        var vsname = BMdata1.Invoke(emp).Where(x => x.EmailId.Equals(username)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        UserRole.UserNc = UserNc.Invoke(emp).Select(p => new { p.VesselId, p.NonConfirmity, p.AcknDate, p.VesselName }).Where(x => vesselname1.Contains(x.VesselName) && x.NonConfirmity != null && x.AcknDate == null).Count();
                        UserRole.UserCNc = UserCNc.Invoke(emp).Select(p => new { p.VesselId, p.AlertFrequency, p.AcknDate, p.VesselName }).Where(x => vesselname1.Contains(x.VesselName) && x.AlertFrequency != null && x.AcknDate == null).Count();

                        return result;
                    }



                }
            }
            catch
            {
                using (ShipmentContaxt emp = new ShipmentContaxt())
                {
                    emp.Configuration.ProxyCreationEnabled = false;

                    if (username.ToLower() == "admin")
                    {
                        string data = BMdata.Invoke(emp).Where(x => x.UserName == username).FirstOrDefault().UserRole;
                        string[] result = { data };
                        UserRole.username = data;
                        return result;
                    }
                    else
                    {
                        // string data = BMdata1.Invoke(emp).Where(x => x.FullName == username).FirstOrDefault().Role;
                        string data = BMdata1.Invoke(emp).Where(x => x.EmailId == username).FirstOrDefault().Role;
                        string[] result = { data };
                        UserRole.username = data;
                        return result;
                    }
                }
            }

        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }


    }
}
using System.Web.Configuration;
using System.Web.Mvc;

namespace Shipment49Web.Common
{
    public class PartialCacheAttribute : OutputCacheAttribute
    {
        public PartialCacheAttribute(string cacheProfileName)
        {
            OutputCacheSettingsSection outputCacheSettingsSection =
            (OutputCacheSettingsSection)WebConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
            OutputCacheProfile outputCacheProfile = outputCacheSettingsSection.OutputCacheProfiles[cacheProfileName];
            Duration = outputCacheProfile.Duration;
            VaryByParam = outputCacheProfile.VaryByParam;
            
        }
    }
}
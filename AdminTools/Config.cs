using System.Collections.Generic;
using System.ComponentModel;

namespace AdminTools
{
    public class Config
    {
        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;

        [Description("Allows for overriding command permissions by their names.")]
        public Dictionary<string, PlayerPermissions> PermissionOverrides { get; set; } = new()
        {
            {
                "example", PlayerPermissions.Broadcasting
            },
            {
                "other", PlayerPermissions.FacilityManagement
            }
        };
    }
}

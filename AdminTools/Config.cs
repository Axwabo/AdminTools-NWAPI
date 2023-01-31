using System.Collections.Generic;
using System.ComponentModel;

namespace AdminTools
{
    public sealed class Config
    {
        [Description("Should the tutorial class be in God Mode? Default: true")]
        public bool GodTuts { get; set; } = true;

        [Description("Allows for overriding command permissions by their lowercase names.")]
        public Dictionary<string, PlayerPermissions> PermissionOverrides { get; set; } = new()
        {
            {
                "example", PlayerPermissions.Broadcasting
            },
            {
                "other", PlayerPermissions.FacilityManagement
            }
        };

        [Description("If the jail command should be registered. Default: true")]
        public bool RegisterJailCommand { get; set; } = true;

        [Description("If a person is unjailed after the warhead detonates, should they be teleported to the surface? Default: true")]
        public bool TeleportUnjailedToSurfaceAfterWarheadDetonation { get; set; } = true;

        [Description("A list of custom jail positions relative to Surface")]
        public List<SerializableVector> CustomJailPositions { get; set; } = new()
        {
            new SerializableVector(130.4693f, -7.594038f, 21.48689f),
            new SerializableVector(161.1646f, 18.50996f, -12.87285f),
            new SerializableVector(107.5201f, 13.08837f, -13.64667f)
        };
    }
}

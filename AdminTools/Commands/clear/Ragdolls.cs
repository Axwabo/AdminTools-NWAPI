﻿using CommandSystem;
using Mirror;
using System;

namespace AdminTools.Commands.Cleanup
{
    class Ragdolls : ICommand
    {
        public string Command { get; } = "ragdolls";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Cleans up ragdolls on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission(PlayerPermissions.FacilityManagement))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: cleanup ragdolls";
                return false;
            }

            foreach (BasicRagdoll doll in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>())
                NetworkServer.Destroy(doll.gameObject);

            response = "Ragdolls have been cleaned up now";
            return true;
        }
    }
}

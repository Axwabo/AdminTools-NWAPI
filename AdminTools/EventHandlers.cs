using AdminTools.Commands.Dummy;
using AdminTools.Enums;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.NonAllocLINQ;
using VoiceChat;
using Object = UnityEngine.Object;

namespace AdminTools
{

    public sealed class EventHandlers
    {
        private static Plugin _pl;

        public EventHandlers(Plugin plugin) => _pl = plugin;

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        public void OnDoorOpen(AtPlayer player, DoorVariant door, bool canOpen)
        {
            if (player.PryGateEnabled)
                door.TryPryOpen(player);
        }

        public static string FormatArguments(ArraySegment<string> sentence, int index)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            foreach (string word in sentence.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }
            string msg = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return msg;
        }

        public static void SpawnDummyModel(Player p, Vector3 position, Quaternion rotation, RoleTypeId role, Vector3 scale, out int dummyIndex)
        {
            dummyIndex = 0;
            GameObject clone = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            ReferenceHub hub = clone.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(new NullConnection(hub.PlayerId), clone);
            CharacterClassManager ccm = hub.characterClassManager;
            ccm.GodMode = true;
            hub.roleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin);
            hub.nicknameSync.Network_myNickSync = "Dummy";
            Transform t = clone.transform;
            t.localScale = scale;
            t.position = position;
            t.rotation = rotation;
            hub.TryOverridePosition(position, rotation.eulerAngles);
            List<GameObject> list = Plugin.DumHubs.GetOrAdd(p, GameObjectListFactory);
            list.Add(clone);
            dummyIndex = list.Count;
            if (dummyIndex != 1)
                dummyIndex = list.Count;
        }
        private static List<GameObject> GameObjectListFactory() => new();

        public static IEnumerator<float> SpawnBodies(Player player, IRagdollRole ragdollRole, int count)
        {
            PlayerRoleBase role = ragdollRole as PlayerRoleBase;
            if (role == null)
                yield break;
            for (int i = 0; i < count; i++)
            {

                Transform t = player.GameObject.transform;
                GameObject gameObject = Object.Instantiate(ragdollRole.Ragdoll.gameObject, t.position, t.rotation);
                if (gameObject.TryGetComponent(out BasicRagdoll component))
                {
                    component.NetworkInfo = new RagdollData(null, new UniversalDamageHandler(0.0f, DeathTranslations.Unknown), role.RoleTypeId, t.position, t.rotation, "SCP-343", NetworkTime.time);
                }

                NetworkServer.Spawn(gameObject);
                yield return Timing.WaitForOneFrame;
            }
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void OnRoleChange(AtPlayer player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (!_pl.Config.GodTuts)
                return;
            if (newRole == RoleTypeId.Tutorial)
            {
                player.WasInGodMode = player.IsGodModeEnabled;
                player.IsGodModeEnabled = true;
            }
            else
                player.IsGodModeEnabled = player.WasInGodMode;
        }

        public static GameObject SpawnWorkbench(AtPlayer p, Vector3 size, out int benchIndex)
        {
            Vector3 position = CalculateBenchPosition(p);
            Vector3 rotation = p.GameObject.transform.rotation.eulerAngles;
            rotation.x += 180;
            rotation.z += 180;
            return SpawnWorkbench(p, position, Quaternion.Euler(rotation), size, out benchIndex);
        }

        public static Vector3 CalculateBenchPosition(Player p) => p.Position + p.ReferenceHub.PlayerCameraReference.forward * 2 + Vector3.down * 0.5f;

        public static GameObject SpawnWorkbench(AtPlayer p, Vector3 position, Quaternion rotation, Vector3 size, out int benchIndex)
        {
            try
            {
                Log.Debug("Spawning workbench");
                benchIndex = 0;
                GameObject bench = Object.Instantiate(WorkstationPrefab, position, rotation);
                bench.transform.localScale = size;
                NetworkServer.Spawn(bench);
                List<GameObject> list = p.Workbenches;
                list.Add(bench);
                benchIndex = list.Count;
                if (benchIndex != 1)
                    benchIndex = list.Count;
                if (!bench.TryGetComponent(out WorkstationController _))
                    bench.AddComponent<WorkstationController>();
                return bench;
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(SpawnWorkbench)}: {e}");
                benchIndex = -1;
                return null;
            }
        }

        public static GameObject WorkstationPrefab => NetworkClient.prefabs.Values.FirstOrDefault(o => o.TryGetComponent(out WorkstationController _));

        public static void SetPlayerScale(Player target, Vector3 scale)
        {
            GameObject go = target.GameObject;
            if (go.transform.localScale == scale)
                return;
            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                go.transform.localScale = scale;
                foreach (Player player in Player.GetPlayers())
                {
                    NetworkServer.SendSpawnMessage(identity, player.Connection);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        public static void SetPlayerScale(Player target, float scale) => SetPlayerScale(target, Vector3.one * scale);

        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            const int maxAmount = 50;
            int current = 0;
            bool godMode = player.IsGodModeEnabled;
            while (player.GameObject != null && player.Role != RoleTypeId.Spectator)
            {
                player.Position += Vector3.up * speed;
                current++;
                if (current >= maxAmount)
                {
                    player.IsGodModeEnabled = false;
                    Handlers.CreateThrowable(ItemType.GrenadeHE).SpawnActive(player.Position, .5f, player);
                    player.Kill("Went on a trip in their favorite rocket ship.");
                    player.IsGodModeEnabled = godMode;
                    yield break;
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> DoJail(Player player, bool skipAdd = false)
        {
            Dictionary<AmmoType, ushort> ammo = player.Ammo();
            List<ItemType> items = player.ReferenceHub.inventory.UserInventory.Items.Select(x => x.Value.ItemTypeId).ToList();
            if (!skipAdd)
            {
                Plugin.JailedPlayers.Add(new Jailed
                {
                    Health = player.Health,
                    Position = GetSafeRelativePosition(player),
                    Items = items,
                    Role = player.Role,
                    UserId = player.UserId,
                    CurrentRound = true,
                    Ammo = ammo,
                    GodMode = player.IsGodModeEnabled
                });
            }

            yield return Timing.WaitForSeconds(0.2f);
            player.SetRole(RoleTypeId.Tutorial);
        }

        public static IEnumerator<float> DoJail(Player player, Vector3 positionOverride, bool skipAdd = false)
        {
            IEnumerator<float> enumerator = DoJail(player, skipAdd);
            while (enumerator.MoveNext())
                yield return enumerator.Current;
            yield return Timing.WaitForSeconds(0.2f);
            player.Position = positionOverride + Vector3.up;
        }

        public static IEnumerator<float> DoUnJail(Player player)
        {
            Jailed jail = Plugin.JailedPlayers.Find(j => j.UserId == player.UserId);
            if (jail == null)
                yield break;
            bool posSet = false;
            Vector3 pos = Vector3.zero;
            try
            {
                pos = jail.Position.Position;
                posSet = true;
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(DoUnJail)} failed getting position: {e}");
            }
            if (jail.CurrentRound && posSet && VerifyDetonationPosition(ref pos))
            {
                player.SetRole(jail.Role);
                yield return Timing.WaitForOneFrame;
                try
                {
                    player.ResetInventory(jail.Items);
                    player.Health = jail.Health;
                    player.Position = pos;
                    player.IsGodModeEnabled = jail.GodMode;
                    foreach (KeyValuePair<AmmoType, ushort> kvp in jail.Ammo)
                        player.AddAmmo(kvp.Key.GetItemType(), kvp.Value);
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(DoUnJail)}: {e}");
                }
            }
            else
            {
                player.SetRole(RoleTypeId.Spectator);
            }
            Plugin.JailedPlayers.Remove(jail);
        }
        private static RelativePosition GetSafeRelativePosition(Player player)
        {
            RelativePosition relative = new(player.Position);
            return !WaypointBase.TryGetWaypoint(relative.WaypointId, out WaypointBase waypoint)
                || waypoint is not ElevatorWaypoint { _elevator._lastDestination: { } dest }
                    ? relative
                    : new RelativePosition(dest.transform.TransformPoint(Vector3.forward) + Vector3.up);
        }
        private static bool VerifyDetonationPosition(ref Vector3 pos)
        {
            if (!Warhead.IsDetonated || !AlphaWarheadController.CanBeDetonated(pos))
                return true;
            if (!_pl.Config.TeleportUnjailedToSurfaceAfterWarheadDetonation)
                return false;
            RoomIdentifier room = Object.FindObjectsOfType<RoomIdentifier>().FirstOrDefault(r => r.Name == RoomName.Outside);
            if (room == null)
                return false;
            SafeTeleportPosition safe = room.transform.root.GetComponentInChildren<SafeTeleportPosition>();
            if (safe == null || safe.SafePositions.Length == 0)
                return false;
            pos = safe.SafePositions.RandomItem().position;
            return true;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerVerified(Player player)
        {
            try
            {
                if (Enumerable.Any(Plugin.JailedPlayers, j => j.UserId == player.UserId))
                    Timing.RunCoroutine(DoJail(player, true));

                if (File.ReadAllText(_pl.OverwatchFilePath).Contains(player.UserId))
                {
                    Log.Debug($"Putting {player.UserId} into overwatch.");
                    Timing.CallDelayed(1, () => player.IsOverwatchEnabled = true);
                }

                if (File.ReadAllText(_pl.HiddenTagsFilePath).Contains(player.UserId))
                {
                    Log.Debug($"Hiding {player.UserId}'s tag.");
                    Timing.CallDelayed(1, () => player.SetBadgeVisibility(true));
                }

                if (Plugin.RoundStartMutes.Count == 0 || player.ReferenceHub.serverRoles.RemoteAdmin || !Plugin.RoundStartMutes.Contains(player.UserId))
                    return;

                Log.Debug($"Muting {player.UserId} (no RA).");
                player.SetMuteFlag(VcMuteFlags.LocalRegular, true);
            }
            catch (Exception e)
            {
                Log.Error($"Player Join: {e}");
            }
        }

        [PluginEvent(ServerEventType.RoundStart)]
        public void OnRoundStart() => ClearRoundStartMutes();

        public static void ClearRoundStartMutes()
        {
            foreach (Player p in Plugin.RoundStartMutes.Select(Player.Get))
                p.SetMuteFlag(VcMuteFlags.LocalRegular, false);

            Plugin.RoundStartMutes.Clear();
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        public void OnRoundEnd(RoundSummary.LeadingTeam leadingTeam)
        {
            ListExtensions.ForEach(Extensions.Players, p => p.WasInGodMode = false);
            try
            {
                HashSet<string> overwatchRead = File.ReadAllLines(_pl.OverwatchFilePath).ToHashSet();
                HashSet<string> tagsRead = File.ReadAllLines(_pl.HiddenTagsFilePath).ToHashSet();

                foreach (Player player in Player.GetPlayers())
                {
                    string userId = player.UserId;
                    if (player.IsOverwatchEnabled)
                        overwatchRead.Add(userId);
                    else
                        overwatchRead.Remove(userId);

                    if (player.IsBadgeHidden())
                        tagsRead.Add(userId);
                    else
                        tagsRead.Remove(userId);
                }

                Log.Debug($"The following users are in overwatch:\n{string.Join("\n", overwatchRead)}");
                Log.Debug($"The following have their tag hidden:\n{string.Join("\n", tagsRead)}");
                File.WriteAllLines(_pl.OverwatchFilePath, overwatchRead);
                File.WriteAllLines(_pl.HiddenTagsFilePath, tagsRead);
            }
            catch (Exception e)
            {
                Log.Error($"Round End:\n{e}");
            } finally
            {
                // Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
                ListExtensions.ForEach(Plugin.JailedPlayers, jail => jail.CurrentRound = false);
            }

        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        public void OnPlayerInteractingDoor(AtPlayer player, DoorVariant door, bool canOpen)
        {
            if (player.BreakDoorsEnabled)
                door.BreakDoor();
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnPlayerDamage(Player target, AtPlayer attacker, DamageHandlerBase handler)
        {
            if (attacker is { InstantKillEnabled: true })
            {
                handler.SetAmount(-1);
            }
        }
    }
}

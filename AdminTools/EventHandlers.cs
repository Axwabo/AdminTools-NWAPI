using AdminTools.Enums;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.NonAllocLINQ;
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

        public static void SpawnDummyModel(Player ply, Vector3 position, Quaternion rotation, RoleTypeId role, float x, float y, float z, out int dummyIndex)
        {
            dummyIndex = 0;
            GameObject obj = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
            ccm._hub.roleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin);
            ccm.GodMode = true;
            obj.GetComponent<NicknameSync>().Network_myNickSync = "Dummy";
            obj.GetComponent<QueryProcessor>()._hub.Network_playerId = new RecyclablePlayerId(9999);
            Transform t = obj.transform;
            t.localScale = new Vector3(x, y, z);
            t.position = position;
            t.rotation = rotation;
            NetworkServer.Spawn(obj);
            List<GameObject> objs = Plugin.DumHubs.GetOrAdd(ply, GameObjectListFactory);
            objs.Add(obj);
            dummyIndex = objs.Count;
            if (dummyIndex != 1)
                dummyIndex = objs.Count;
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

        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnPlayerDestroyed(AtPlayer player)
        {
            if (Plugin.RoundStartMutes.Remove(player.UserId))
                player.Unmute(true);
        }

        public static void SpawnWorkbench(Player ply, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
        {
            try
            {
                Log.Debug("Spawning workbench");
                benchIndex = 0;
                GameObject bench = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
                rotation.x += 180;
                rotation.z += 180;
                Offset offset = new()
                {
                    position = position,
                    rotation = rotation,
                    scale = Vector3.one
                };
                bench.gameObject.transform.localScale = size;
                NetworkServer.Spawn(bench);
                List<GameObject> objs = Plugin.BenchHubs.GetOrAdd(ply, GameObjectListFactory);
                objs.Add(bench);
                benchIndex = Plugin.BenchHubs[ply].Count;

                if (benchIndex != 1)
                    benchIndex = objs.Count;
                bench.transform.localPosition = offset.position;
                bench.transform.localRotation = Quaternion.Euler(offset.rotation);
                if (!bench.TryGetComponent(out WorkstationController _))
                    bench.AddComponent<WorkstationController>();
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(SpawnWorkbench)}: {e}");
                benchIndex = -1;
            }
        }

        public static void SetPlayerScale(Player target, Vector3 scale)
        {
            GameObject go = target.GameObject;
            if (go.transform.localScale == scale)
                return;
            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                go.transform.localScale = scale;

                ObjectDestroyMessage destroyMessage = new()
                {
                    netId = identity.netId
                };

                foreach (Player player in Player.GetPlayers())
                {
                    if (player.GameObject == go)
                        continue;
                    NetworkConnection connection = player.Connection;
                    connection.Send(destroyMessage);
                    NetworkServer.SendSpawnMessage(identity, connection);
                }

                target.ReferenceHub.roleManager._sendNextFrame = true;
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
                    Handlers.CreateThrowable(ItemType.GrenadeHE).SpawnActive(player.Position, .1f, player);
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
                    Position = player.Position,
                    Items = items,
                    Role = player.Role,
                    UserId = player.UserId,
                    CurrentRound = true,
                    Ammo = ammo
                });
            }

            if (player.IsOverwatchEnabled)
                player.IsOverwatchEnabled = false;
            yield return Timing.WaitForSeconds(0.2f);
            player.SetRole(RoleTypeId.Tutorial);
        }

        public static IEnumerator<float> DoUnJail(Player player)
        {
            Jailed jail = Plugin.JailedPlayers.Find(j => j.UserId == player.UserId);
            if (jail.CurrentRound)
            {
                player.SetRole(jail.Role);
                yield return Timing.WaitForSeconds(0.5f);
                try
                {
                    player.ResetInventory(jail.Items);
                    player.Health = jail.Health;
                    player.Position = jail.Position;
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

                if (Plugin.RoundStartMutes.Count == 0 || player.ReferenceHub.serverRoles.RemoteAdmin || !Plugin.RoundStartMutes.Add(player.UserId))
                    return;

                Log.Debug($"Muting {player.UserId} (no RA).");
                player.Mute();
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
                p?.Unmute(true);

            Plugin.RoundStartMutes.Clear();
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        public void OnRoundEnd(RoundSummary.LeadingTeam leadingTeam)
        {
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

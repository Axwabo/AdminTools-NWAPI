# AdminTools

A port of EXILED's SCP:SL plugin made for the new offical Northwood API. Gives server owners more useful commands.

# Installation

## Automatic

**CAUTION: USING THE PLUGIN MANAGER BUT EVEN INSTALLING ANY PLUGIN CAN BREAK THE SERVER AND MAY POTENTIALLY CONTAIN
HARMFUL CODE. USE PLUGINS AND THE BUILT-IN PLUGIN MANAGER AT YOUR OWN RISK!**

1. Enable the plugin manager in the server console (run `p` then `p confirm`)
2. Run `p install Axwabo/AdminTools-NWAPI` in the server console
3. Restart your server

## Manual

1. Download the `AdminTools.dll` from the [latest release](https://github.com/Axwabo/AdminTools-NWAPI/releases/latest)
2. Put the DLL in your `plugins` folder (windows: `%appdata%/SCP Secret Laboratory/PluginAPI/plugins/<port>/`)
3. Restart your server

## Commands and Descriptions

You can override command permissions in the config. The default ones are listed here.

### adminbroadcast (aka abc)

Sends a message to all currently online staff on the server

Permssions: **Broadcasting**

### ahp

Sets a user or users Artificial HP to a specified value

Permissions: **PlayersManagement**

### ball

Spawns a bouncy ball (SCP-018) on a user or all users

Permissions: **GivingItems**

### bench (aka sw, wb, workbench)

Spawns a workbench on all users or a user

Permissions: **RespawnEvents**

### atstrip (aka stp)

Clears a user or users inventories instantly

Permissions: **PlayersManagement**

### breakdoors (aka bd)

Manage break doors properties for users

Permissions: **ForceclassWithoutRestrictions**

### cfig

Manages reloading permissions and configs

Permissions: **ServerConfigs**

### dropitem (aka drop, dropi)

Drops a specified amount of a specified item on either all users or a user

Permissions: **GivingItems**

### dropsize (aka drops)

Same as drop item but can also modify the size of the item dropped.

Permissions: **GivingItems**

### dummy (aka dum)

Spawns a dummy character on all users on a user

Permissions: **RespawnEvents**

### enums (aka enum)

Lists all enums AdminTools uses

Permissions: N/A

### expl (aka boom)

Explodes a specified user or everyone instantly

Permissions: **ForceclassToSpectator**

### grenade (aka gn)

Spawns a frag/flash/scp018 grenade on a user or users

Permissions: **GivingItems**

### instakill (aka ik)

Manage instant kill properties for users

Permissions: **ForceclassWithoutRestrictions**

### inventory (aka inv)

Manages player inventories

Permissions: **PlayersManagement**

### jail

Jails or unjails a user

Permissions: **KickingAndShortTermBanning**

### atkill

Kills everyone or a user instantly

Permissions: **ForceclassToSpectator**

### pmute

Mutes everyone from speaking or by intercom in the server

Permissions: **PlayersManagement**

### punmute

Unmutes everyone from speaking or by intercom in the server

Permissions: **PlayersManagement**

### position (aka pos)

Modifies or retrieves the position of a user or all users

Permissions: **PlayersManagement**

### prygate

Gives the ability to pry gates to players, clear the ability from players, and shows who has the ability

Permissions: **FacilityManagement**

### randomtp

Randomly teleports a user or all users to a random room in the facility

Permissions: **PlayersManagement**

### reg

Manages regeneration properties for users

Permissions: **PlayersManagement**

### rocket

Sends players high in the sky and explodes them

Permissions: **ForceclassWithoutRestrictions**

### scale

Scales all users or a user by a specified value

Permissions: **Effects**

### size

Sets the size of all users or a user

Permissions: **Effects**

### spawnragdoll (aka ragdoll, rd, rag, doll)

Spawns a specified number of ragdolls on a user

Permissions: **RespawnEvents**

### tags

Hides or shows staff tags in the server

Permissions: **SetGroup**

### targetghost

Makes a player invisible to another player or all players

Permissions: **Effects**

### teleportx (aka tpx)

Teleports all users or a user to another user

Permissions: **PlayersManagement**
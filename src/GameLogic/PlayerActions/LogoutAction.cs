﻿// <copyright file="LogoutAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Views.Login;

/// <summary>
/// Action to log the player out of the game.
/// </summary>
public class LogoutAction
{
    /// <summary>
    /// Logs out the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="logoutType">Type of the logout.</param>
    public async ValueTask LogoutAsync(Player player, LogoutType logoutType)
    {
        player.CurrentMap?.RemoveAsync(player);
        player.Party?.KickMySelfAsync(player);
        player.SelectedCharacter = null;
        player.MagicEffectList.ClearAllEffects();
        player.PersistenceContext.SaveChanges();
        if (logoutType == LogoutType.CloseGame)
        {
            await player.DisconnectAsync();
        }
        else
        {
            if (logoutType == LogoutType.BackToCharacterSelection)
            {
                player.PlayerState.TryAdvanceTo(PlayerState.Authenticated);
            }

            await player.InvokeViewPlugInAsync<ILogoutPlugIn>(p => p.LogoutAsync(logoutType)).ConfigureAwait(false);
        }
    }
}
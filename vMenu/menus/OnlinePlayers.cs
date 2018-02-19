﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using NativeUI;

namespace vMenuClient
{
    public class OnlinePlayers
    {
        // Menu variable, will be defined in CreateMenu()
        private UIMenu menu;
        private static Notification Notify = new Notification();
        private static Subtitles Subtitle = new Subtitles();
        private CommonFunctions cf = new CommonFunctions();

        // Public variables (getters only), return the private variables.
        //public bool PlayerGodMode { get; private set; } = false;
        //public bool PlayerInvisible { get; private set; } = false;
        //public bool PlayerStamina { get; private set; } = true;
        //public bool PlayerSuperJump { get; private set; } = false;
        //public bool PlayerNoRagdoll { get; private set; } = false;
        //public bool PlayerNeverWanted { get; private set; } = false;
        //public bool PlayerIsIgnored { get; private set; } = false;
        //public bool PlayerFrozen { get; private set; } = false;


        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu(GetPlayerName(PlayerId()), "Online Players", MainMenu.MenuPosition);
            UpdatePlayerlist();
        }

        public void UpdatePlayerlist()
        {
            
            if (menu.MenuItems.Count > 0)
            {
                menu.MenuItems.Clear();
            }

            PlayerList pl = new PlayerList();

            foreach (Player p in pl)
            {
                UIMenuItem playerItem = new UIMenuItem(p.Name, "[" + (p.Handle < 10 ? "0" : "") + p.Handle + "] " + p.Name + " (Server ID: " + p.ServerId + ")");
                menu.AddItem(playerItem);

                menu.OnItemSelect += (sender, item, index) =>
                {
                    if (item == playerItem)
                    {
                        // Create the player object.
                        Player player = new Player(int.Parse(item.Description.Substring(1, 1).ToString()));

                        // Create the menu for the player.
                        UIMenu PlayerMenu = new UIMenu(player.Name, "[" + (player.Handle < 10 ? "0" : "") + player.Handle + "] " + player.Name + " (Server ID: " + player.ServerId + ")", MainMenu.MenuPosition)
                        {
                            ScaleWithSafezone = false
                        };

                        PlayerMenu.SetBannerType(new UIResRectangle(new System.Drawing.PointF(0f, 0f), new System.Drawing.SizeF(0f, 0f), System.Drawing.Color.FromArgb(38, 38, 38)));
                        PlayerMenu.ControlDisablingEnabled = false;
                        PlayerMenu.MouseEdgeEnabled = false;

                        // Create all player options buttons.
                        UIMenuItem teleportBtn = new UIMenuItem("Teleport to Player", "Teleport to this player.");
                        UIMenuItem teleportInVehBtn = new UIMenuItem("Teleport into Vehicle", "Telepor into the player's vehicle.");
                        UIMenuItem setWaypointBtn = new UIMenuItem("Set waypoint", "Set a waypoint to this player.");
                        UIMenuItem spectateBtn = new UIMenuItem("Spectate Player", "Spectate this player.");
                        UIMenuItem summonBtn = new UIMenuItem("Summon Player", "Teleport the player in front of you.");
                        UIMenuItem killBtn = new UIMenuItem("Kill Player", "Kill the other player!");
                        UIMenuItem kickPlayerBtn = new UIMenuItem("Kick Player", "Kick the player from the server.");
                        kickPlayerBtn.SetRightBadge(UIMenuItem.BadgeStyle.Alert);

                        // Add all buttons to the player options submenu.
                        PlayerMenu.AddItem(teleportBtn);
                        PlayerMenu.AddItem(teleportInVehBtn);
                        PlayerMenu.AddItem(setWaypointBtn);
                        PlayerMenu.AddItem(spectateBtn);
                        PlayerMenu.AddItem(summonBtn);
                        PlayerMenu.AddItem(killBtn);
                        PlayerMenu.AddItem(kickPlayerBtn);
                        
                        // Add the player menu to the menu pool.
                        MainMenu._mp.Add(PlayerMenu);

                        // Set the menu invisible.
                        menu.Visible = false;
                        // Set the player menu visible.
                        PlayerMenu.Visible = true;


                        // If a button is pressed in the player's options menu.
                        PlayerMenu.OnItemSelect += (sender2, item2, index2) =>
                        {
                            // Teleport button is pressed.
                            if (item2 == teleportBtn)
                            {
                                cf.TeleportToPlayerAsync(player.Handle, false);
                            }
                            // Teleport in vehicle button is pressed.
                            else if (item2 == teleportInVehBtn)
                            {
                                cf.TeleportToPlayerAsync(player.Handle, true);
                            }
                            // Set waypoint button is pressed.
                            else if (item2 == setWaypointBtn)
                            {
                                World.WaypointPosition = GetEntityCoords(GetPlayerPed(player.Handle), true);
                                Notify.Info("A new waypoint has been set to " + player.Name, false, false);
                            }
                            // Spectate player button is pressed.
                            else if (item2 == spectateBtn)
                            {
                                if (player.Handle == PlayerId())
                                {
                                    Notify.Error("You can ~h~not ~w~spectate yourself!");
                                }
                                else
                                {
                                    //cf.Spectate(playerIndex);
                                    Notify.Error("This feature is not implemented yet!", true, false);
                                }
                            }
                            // Summon player button is pressed.
                            else if (item2 == summonBtn)
                            {
                                cf.SummonPlayer(player);
                            }
                            // Kill player button is pressed.
                            else if (item2 == killBtn)
                            {
                                cf.KillPlayer(player);
                            }
                            // Kick player button is pressed.
                            else if (item2 == kickPlayerBtn)
                            {
                                //TriggerServerEvent("vMenu:KickPlayer", GetPlayerServerId(playerIndex));

                                Notify.Error("Todo: trigger server event using another class.");
                                PlayerMenu.Visible = false;

                                UpdatePlayerlist();

                                menu.RefreshIndex();

                                menu.Visible = true;
                            }
                        };

                        PlayerMenu.OnMenuClose += (sender3) =>
                        {
                            PlayerMenu.Visible = false;
                            menu.Visible = true;
                        };
                    }
                };
            };
        
        
            //MainMenu._mp.Add(menu);

        }

        /// <summary>
        /// Checks if the menu exists, if not then it creates it first.
        /// Then returns the menu.
        /// </summary>
        /// <returns>The Online Players Menu</returns>
        public UIMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
                return menu;
            }
            else
            {
                UpdatePlayerlist();
                return menu;
            }
        }
    }
}

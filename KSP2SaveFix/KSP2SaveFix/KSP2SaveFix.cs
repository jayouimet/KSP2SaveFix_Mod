using SpaceWarp.API.Mods;
using SpaceWarp.API;
using SpaceWarp.UI;
using SpaceWarp.API.Game;
using SpaceWarp.API.Assets;
using SpaceWarp.API.UI.Appbar;
using SpaceWarp;
using KSP.game;
using BepInEx;
using KSP.UI.Binding;
using KSP.Sim.impl;
using UnityEngine;
using System.Collections.Generic;
using SpaceWarp.API.UI;
using KSP.Game;

namespace KSP2SaveFix
{
    [BepInPlugin("com.github.jayouimet.KSP2SaveFix", "KSP2SaveFix", "1.0.1")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KSP2SaveFix : BaseSpaceWarpPlugin
    {
        private bool drawUI;
        private Rect windowRect;
        private bool loaded;
        private short frameCounter;

        private static KSP2SaveFix Instance { get; set; }

        public override void OnInitialized()
        {
            base.OnInitialized();

            if (loaded)
            {
                Destroy(this);
            }
            Instance = this;
            frameCounter = 0;
        }

        public override void OnPostInitialized() { loaded = true; }

        private void LateUpdate()
        {
            // Only update every 20 frames
            if (loaded) { 
            frameCounter++;
                if (frameCounter > 20)
                {
                    frameCounter = 0;
                    // So it doesn't start looking for vessels before we loaded a save
                    if (!(GameManager.Instance.Game.ViewController is null) && !(GameManager.Instance.Game.ViewController.Universe is null))
                    {
                        // Collect vessels
                        List<VesselComponent> vessels = GameManager.Instance.Game.ViewController.Universe.GetAllVessels();
                        for (int i = 0; i < vessels.Count; i++)
                        {
                            // Get the control owner part
                            PartComponent controlOwner = vessels[i].GetControlOwner();
                            // If the control owner was null, we need to reset it to a command module
                            if (controlOwner is null)
                            {
                                Logger.LogInfo("Control 0wner not found for  " + vessels[i].GlobalId);
                                // Gather command modules
                                List<PartComponentModule_Command> partModules = vessels[i].SimulationObject.PartOwner.GetPartModules<PartComponentModule_Command>();
                                // Set ownership to the first command module
                                if (partModules.Count > 0)
                                {
                                    Logger.LogInfo("Set control to " + partModules[0].Part.GlobalId);
                                    vessels[i].SetControlOwner(partModules[0].Part);
                                }
                                else
                                {
                                    // Otherwise try to set it to the root part, whatever it is
                                    if (vessels[i].SimulationObject.PartOwner != null)
                                    {
                                        Logger.LogInfo("Set control to " + vessels[i].SimulationObject.PartOwner.RootPart.GlobalId);
                                        vessels[i].SetControlOwner(vessels[i].SimulationObject.PartOwner.RootPart);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

using SpaceWarp.API.Mods;
using SpaceWarp.API.AssetBundles;
using SpaceWarp.API;
using KSP.UI.Binding;
using KSP.Sim.impl;
using UnityEngine;
using System.Collections.Generic;

namespace KSP2SaveFix
{
    [MainMod]
    public class KSP2SaveFix : Mod
    {
        public GUISkin _spaceWarpUISkin;

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

            loaded = true;
            Instance = this;
            frameCounter = 0;

            // Example of using the asset loader, were going to load the SpaceWarp GUI skin.
            // [FORMAT]: space_warp/[assetbundle_name]/[folder_in_assetbundle]/[file.type]
            ResourceManager.TryGetAsset(
                "space_warp/swconsoleui/swconsoleUI/spacewarpConsole.guiskin",
                out _spaceWarpUISkin
            );

            // Register the mod's button on the SpaceWarp application bar.
            SpaceWarpManager.RegisterAppButton(
                "KSP2SaveFix",
                "BTN-KSP2SaveFix",
                SpaceWarpManager.LoadIcon(),
                ToggleButton
            );
        }

        private void ToggleButton(bool toggle)
        {
            drawUI = toggle;
            GameObject.Find("BTN-KSP2SaveFix")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        }

        public void OnGUI()
        {
            // Set the GUI skin to the SpaceWarp GUI skin.
            GUI.skin = _spaceWarpUISkin;

            if (drawUI)
            {
                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    windowRect,
                    FillWindow, // The method we call. 
                    "KSP2SaveFix",
                    GUILayout.Height(350),
                    GUILayout.Width(350)
                );
            }
        }

        private static void FillWindow(int windowID)
        {
            GUILayout.Label("KSP2SaveFix - Built with Space-Warp");
            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }

        private void LateUpdate()
        {
            // Only update every 20 frames
            frameCounter++;
            if (frameCounter > 20)
            {
                frameCounter = 0;
                // So it doesn't start looking for vessels before we loaded a save
                if (!(Instance.Game.ViewController is null) && !(Instance.Game.ViewController.Universe is null))
                {
                    // Collect vessels
                    List<VesselComponent> vessels = Instance.Game.ViewController.Universe.GetAllVessels();
                    for (int i = 0; i < vessels.Count; i++)
                    {
                        // Get the control owner part
                        PartComponent controlOwner = vessels[i].GetControlOwner();
                        // If the control owner was null, we need to reset it to a command module
                        if (controlOwner is null)
                        {
                            Logger.Info("Control 0wner not found for  " + vessels[i].GlobalId);
                            // Gather command modules
                            List<PartComponentModule_Command> partModules = vessels[i].SimulationObject.PartOwner.GetPartModules<PartComponentModule_Command>();
                            // Set ownership to the first command module
                            if (partModules.Count > 0)
                            {
                                Logger.Info("Set control to " + partModules[0].Part.GlobalId);
                                vessels[i].SetControlOwner(partModules[0].Part);
                            }
                            else
                            {
                                // Otherwise try to set it to the root part, whatever it is
                                if (vessels[i].SimulationObject.PartOwner != null)
                                {
                                    Logger.Info("Set control to " + vessels[i].SimulationObject.PartOwner.RootPart.GlobalId);
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

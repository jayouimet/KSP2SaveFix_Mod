# KSP2SaveFix_Mod
A mod that aims to fix the save game bug and the launch from VAB button not working.

Every 20 frames, this mod makes sure that every vessel has a ControlOwnerPart. If not, it goes through its command modules and sets the first one as control owner.
If no command modules are found (should not happen), it sets control ownership to the RootPart of the vessel.

## For SpaceWarp 0.4.0
Thank you to ultimaxx9 for the conversion!
Download and extract the zipped file in releases (release 1.0.1) and place the "KSP2SaveFix" folder into the BepInEx -> plugins folder.

## For SpaceWarp 0.3.0
Download and extract the zipped file in releases (pre-release 0.0.1) and place the "KSP2SaveFix" folder into the "Mods" folder of the SpaceWarp mod loader.

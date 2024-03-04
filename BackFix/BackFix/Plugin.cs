using API;
using BackFix.Patches;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace BackFix.BepInEx;

[BepInPlugin(Module.GUID, Module.Name, Module.Version)]
[BepInDependency("Localia.RealBackBonus", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BasePlugin {
    public override void Load() {
        APILogger.Log("Plugin is loaded!");
        harmony = new Harmony(Module.GUID);

        APILogger.Log("Debug is " + (ConfigManager.Debug ? "Enabled" : "Disabled"));

        ClassInjector.RegisterTypeInIl2Cpp<BackFixPatches.SyncPosition>();

        if (IL2CPPChainloader.Instance.Plugins.TryGetValue("Localia.RealBackBonus", out _)) {
            APILogger.Log("Localia's true back mod is installed.");
        } else {
            harmony.PatchAll(typeof(KnifeFixPatch));
        }
        harmony.PatchAll(typeof(BackFixPatches));
    }

    private static Harmony? harmony;
}
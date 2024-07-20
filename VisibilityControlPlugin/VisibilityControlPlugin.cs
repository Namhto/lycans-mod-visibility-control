using BepInEx;
using VisibilityControlPlugin.domain;
using VisibilityControlPlugin.patch;

namespace VisibilityControlPlugin;

[BepInPlugin("com.namhto.mods.lycans.visibility-control", "Visibility Control", "1.0.0")]
[BepInProcess("Lycans.exe")]
public class VisibilityControlPlugin : BaseUnityPlugin
{
    public static Settings Settings;

    private void Awake()
    {
        Log.Init(Logger);
        Log.Info("Initializing Visibility Control");
        Settings = new Settings(Config);
        GameSettingsMenuPatch.Hook();
        NetworkingPatch.Hook();
    }
}
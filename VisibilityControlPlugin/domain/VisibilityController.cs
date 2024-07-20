using System.Linq;
using Fusion;
using Helpers.Collections;
using UnityEngine;

namespace VisibilityControlPlugin.domain;

[NetworkBehaviourWeaved(3)]
public class VisibilityController : NetworkBehaviour
{
    private float _initialPointLightIntensity;

    private float _initialAmbientIntensity;

    private bool _initialFogDisabled;

    private float _pointLightIntensity;

    [Networked(OnChanged = nameof(PointLightIntensityChanged))]
    [NetworkedWeaved(0, 1)]
    public unsafe float PointLightIntensity
    {
        get => ReadWriteUtilsForWeaver.ReadFloat(Ptr, 0.01f);
        set => ReadWriteUtilsForWeaver.WriteFloat(Ptr, 99.99f, value);
    }

    public static void PointLightIntensityChanged(Changed<VisibilityController> changed)
    {
        changed.Behaviour._pointLightIntensity = changed.Behaviour.PointLightIntensity;
        Log.Info("PointLightIntensity: " + changed.Behaviour._pointLightIntensity);
    }

    private float _ambientIntensity;

    [Networked(OnChanged = nameof(AmbientIntensityChanged))]
    [NetworkedWeaved(1, 1)]
    public unsafe float AmbientIntensity
    {
        get => ReadWriteUtilsForWeaver.ReadFloat(Ptr + 1, 0.01f);
        set => ReadWriteUtilsForWeaver.WriteFloat(Ptr + 1, 99.99f, value);
    }

    public static void AmbientIntensityChanged(Changed<VisibilityController> changed)
    {
        changed.Behaviour._ambientIntensity = changed.Behaviour.AmbientIntensity;
        Log.Info("AmbientIntensity: " + changed.Behaviour._ambientIntensity);
    }

    private bool _fogDisabled;

    [Networked(OnChanged = nameof(FogDisabledChanged))]
    [NetworkedWeaved(2, 1)]
    public unsafe bool FogDisabled
    {
        get => ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 2);
        set => ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 2, value);
    }

    public static void FogDisabledChanged(Changed<VisibilityController> changed)
    {
        changed.Behaviour._fogDisabled = changed.Behaviour.FogDisabled;
        Log.Info("FogDisabled: " + changed.Behaviour._fogDisabled);
    }

    public override void Spawned()
    {
        BindToSettings();
        On.GameManager.Rpc_Transition += (orig, self) =>
        {
            orig(self);
            ApplyVisibilitySettings();
        };
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        VisibilityControlPlugin.Settings.ClearListeners();
    }

    private void BindToSettings()
    {
        var settings = VisibilityControlPlugin.Settings;
        if (Runner.IsServer)
        {
            settings.AddPointLightIntensityListener(value => PointLightIntensity = value);
            settings.AddAmbientIntensityListener(value => AmbientIntensity = value);
            settings.AddForDisabledListener(value => FogDisabled = value);
        }

        PointLightIntensity = settings.GetPointLightIntensity();
        AmbientIntensity = settings.GetAmbientIntensity();
        FogDisabled = settings.GetFogDisabled();
    }

    private void ApplyVisibilitySettings()
    {
        if (GameManager.LocalGameState != GameState.EGameState.Play)
            return;
        
        var lightingManager = GameManager.Instance.GetComponent<LightingManager>();
        if (!lightingManager.IsNight)
        {
            Log.Info("Changing visibility settings");

            _initialPointLightIntensity = lightingManager._sceneLights.First().intensity;
            _initialAmbientIntensity = RenderSettings.ambientIntensity;
            _initialFogDisabled = !RenderSettings.fog;

            lightingManager._sceneLights.ForEach(light => light.intensity = _pointLightIntensity);
            RenderSettings.ambientIntensity = _ambientIntensity;
            RenderSettings.fog = !_fogDisabled;
        }
        else
        {
            Log.Info("Reverting visibility settings");

            lightingManager._sceneLights.ForEach(light => light.intensity = _initialPointLightIntensity);
            RenderSettings.ambientIntensity = _initialAmbientIntensity;
            RenderSettings.fog = !_initialFogDisabled;
        }
    }
}
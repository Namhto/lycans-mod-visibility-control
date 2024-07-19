using Fusion;
using Helpers.Collections;
using UnityEngine;

namespace VisibilityControlPlugin;

[NetworkBehaviourWeaved(3)]
public class VisibilityController : NetworkBehaviour
{
    private float _initialAmbientIntensity;
    
    private bool _initialFogDisabled;
    
    [Networked] private float PointLightIntensity { get; set; }

    [Networked] private float AmbientIntensity { get; set; }

    [Networked] private bool FogDisabled { get; set; }

    public override void Spawned()
    {
        VisibilityControlPlugin.Settings.AddPointLightIntensityListener(value =>
        {
            if (Runner.IsServer)
            {
                PointLightIntensity = value;
                Log.Debug("PointLightIntensity: " + value);
            }
        });
        VisibilityControlPlugin.Settings.AddAmbientIntensityListener(value =>
        {
            if (Runner.IsServer)
            {
                AmbientIntensity = value;
                Log.Debug("AmbientIntensity: " + value);
            }
        });
        VisibilityControlPlugin.Settings.AddForDisabledListener(value =>
        {
            if (Runner.IsServer)
            {
                FogDisabled = value;
                Log.Debug("FogDisabled: " + value);
            }
        });
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

    private void ApplyVisibilitySettings()
    {
        var lightingManager = GameManager.Instance.GetComponent<LightingManager>();
        if (!lightingManager.IsNight)
        {
            Log.Debug("Setting visibility settings");
            
            _initialAmbientIntensity = RenderSettings.ambientIntensity;
            _initialFogDisabled = !RenderSettings.fog;
            
            lightingManager._sceneLights.ForEach(light => light.intensity = PointLightIntensity);
            RenderSettings.ambientIntensity = AmbientIntensity;
            RenderSettings.fog = !FogDisabled;
        }
        else
        {
            Log.Debug("Reverting visibility settings");
            
            RenderSettings.ambientIntensity = _initialAmbientIntensity;
            RenderSettings.fog = !_initialFogDisabled;
        }
    }
}
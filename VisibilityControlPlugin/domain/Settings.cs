using System;
using System.Collections.Generic;

namespace VisibilityControlPlugin;

using BepInEx.Configuration;

public class Settings(ConfigFile config)
{
    private readonly ConfigEntry<float> _pointLightIntensity = config.Bind(
        "General",
        "PointLightIntensity",
        0.5f,
        "Intensity of the point lights"
    );

    private readonly List<Action<float>> _pointLightIntensityListeners = [];

    private readonly ConfigEntry<float> _ambientIntensity = config.Bind(
        "General",
        "AmbientIntensity",
        0.3f,
        "Intensity of the ambient light during nighttime"
    );

    private readonly List<Action<float>> _ambientIntensityListeners = [];

    private readonly ConfigEntry<bool> _fogDisabled = config.Bind(
        "General",
        "FogDisabled",
        false,
        "Is fog disabled"
    );

    private readonly List<Action<bool>> _fogDisabledListeners = [];

    public float GetDefaultPointLightIntensity()
    {
        return (float)_pointLightIntensity.DefaultValue;
    }

    public float GetPointLightIntensity()
    {
        return _pointLightIntensity.Value;
    }

    public void SetPointLightIntensity(float value)
    {
        _pointLightIntensity.Value = value;
        _pointLightIntensityListeners.ForEach(listener => listener(value));
    }

    public void AddPointLightIntensityListener(Action<float> listener)
    {
        _pointLightIntensityListeners.Add(listener);
    }

    public float GetDefaultAmbientIntensity()
    {
        return (float)_ambientIntensity.DefaultValue;
    }

    public float GetAmbientIntensity()
    {
        return _ambientIntensity.Value;
    }

    public void SetAmbientIntensity(float value)
    {
        _ambientIntensity.Value = value;
        _ambientIntensityListeners.ForEach(listener => listener(value));
    }

    public void AddAmbientIntensityListener(Action<float> listener)
    {
        _ambientIntensityListeners.Add(listener);
    }

    public bool GetDefaultFogDisabled()
    {
        return (bool)_fogDisabled.DefaultValue;
    }

    public bool GetFogDisabled()
    {
        return _fogDisabled.Value;
    }

    public void SetFogDisabled(bool value)
    {
        _fogDisabled.Value = value;
        _fogDisabledListeners.ForEach(listener => listener(value));
    }

    public void AddForDisabledListener(Action<bool> listener)
    {
        _fogDisabledListeners.Add(listener);
    }

    public void ClearListeners()
    {
        _pointLightIntensityListeners.Clear();
        _ambientIntensityListeners.Clear();
        _fogDisabledListeners.Clear();
    }
}
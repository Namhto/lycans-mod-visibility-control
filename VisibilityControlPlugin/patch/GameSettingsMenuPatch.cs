using UnityEngine.Events;

namespace VisibilityControlPlugin.patch;

using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public static class GameSettingsMenuPatch
{
    public static void Hook()
    {
        RegisterTranslations();
        On.GameManager.Start += (orig, self) =>
        {
            orig(self);
            var settings = VisibilityControlPlugin.Settings;
            AddSection(self.gameUI, "VC_LABEL");
            AddSlider(
                self.gameUI,
                "VC_POINT_LIGHT",
                settings.SetPointLightIntensity,
                settings.GetPointLightIntensity(),
                settings.GetDefaultPointLightIntensity()
            );
            AddSlider(
                self.gameUI,
                "VC_AMBIENT",
                settings.SetAmbientIntensity,
                settings.GetAmbientIntensity(),
                settings.GetDefaultAmbientIntensity()
            );
            AddToggle(
                self.gameUI,
                "VC_FOG",
                settings.SetFogDisabled,
                settings.GetFogDisabled(),
                settings.GetDefaultFogDisabled()
            );
            MakeSettingsMenuScrollable(self.gameUI);
        };
    }

    private static void RegisterTranslations()
    {
        var localizationTableEn = LocalizationSettings.StringDatabase.GetTable(
            "UI Text",
            LocalizationSettings.AvailableLocales.GetLocale("en")
        );
        var localizationTableFr = LocalizationSettings.StringDatabase.GetTable(
            "UI Text",
            LocalizationSettings.AvailableLocales.GetLocale("fr")
        );

        localizationTableEn.AddEntry("VC_LABEL", "Visibility Control");
        localizationTableFr.AddEntry("VC_LABEL", "Contrôle de la visibilité");

        localizationTableEn.AddEntry("VC_FOG", "Disable fog");
        localizationTableFr.AddEntry("VC_FOG", "Désactiver le brouillard");

        localizationTableEn.AddEntry("VC_POINT_LIGHT", "Intensity of local lights (lanterns, camp fires, etc)");
        localizationTableFr.AddEntry("VC_POINT_LIGHT", "Intensité des lumières locales (lanternes, feux de camp, etc)");

        localizationTableEn.AddEntry("VC_AMBIENT", "Intensity of ambient light");
        localizationTableFr.AddEntry("VC_AMBIENT", "Intensité de la lumière ambiente");

        localizationTableEn.AddEntry("VC_ALLOW_NO_PLUGIN", "Allow players to join without this mod");
        localizationTableFr.AddEntry("VC_ALLOW_NO_PLUGIN", "Autoriser les joueurs à rejoindre sans ce mod");

        LocalizationSettings.Instance.OnSelectedLocaleChanged += _ => RegisterTranslations();
    }

    private static void AddSection(GameUI gameUi, string label)
    {
        var newSection = Object.Instantiate(
            gameUi.settingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/SettingsGroup/GraphicsTitle"),
            gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/LayoutGroup")
        );
        newSection.GetComponentInChildren<LocalizeStringEvent>().SetEntry(label);
    }

    private static void AddSlider(
        GameUI gameUi,
        string label,
        UnityAction<float> onValueChangedListener,
        float initialValue,
        float defaultValue
    )
    {
        var transform = Object.Instantiate(
            gameUi.settingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/SettingsGroup/MasterVolumeSetting"),
            gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/LayoutGroup")
        );
        transform.GetComponentInChildren<LocalizeStringEvent>().SetEntry(label);
        var slider = transform.GetComponentInChildren<Slider>();
        slider.onValueChanged = new Slider.SliderEvent();
        slider.onValueChanged.AddListener(onValueChangedListener);
        On.GameSettingsUI.ResetSettings += (orig, self) =>
        {
            orig(self);
            slider.SetValueWithoutNotify(defaultValue);
            onValueChangedListener(defaultValue);
        };
        slider.SetValueWithoutNotify(initialValue);
    }

    private static void AddToggle(
        GameUI gameUi,
        string label,
        UnityAction<bool> onValueChangedListener,
        bool initialValue,
        bool defaultValue
    )
    {
        var transform = Object.Instantiate(
            gameUi.settingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/SettingsGroup/VSyncSettings"),
            gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/LayoutGroup")
        );
        transform.GetComponentInChildren<LocalizeStringEvent>().SetEntry(label);
        var toggle = transform.GetComponentInChildren<Toggle>();
        toggle.onValueChanged = new Toggle.ToggleEvent();
        toggle.onValueChanged.AddListener(onValueChangedListener);
        On.GameSettingsUI.ResetSettings += (orig, self) =>
        {
            orig(self);
            toggle.SetIsOnWithoutNotify(defaultValue);
            onValueChangedListener(defaultValue);
        };
        toggle.SetIsOnWithoutNotify(initialValue);
    }

    private static void MakeSettingsMenuScrollable(GameUI gameUi)
    {
        var holder = gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder");
        holder.gameObject.AddComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(-20, -10);
        holder.gameObject.AddComponent<Mask>().showMaskGraphic = false;
        holder.Find("QuitButton").SetParent(holder.parent);

        var layoutGroup = holder.Find("LayoutGroup");

        var scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(holder);
        scrollView.transform.SetAsFirstSibling();

        var scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.content = layoutGroup.GetComponent<RectTransform>();
        scrollRect.viewport = holder.GetComponent<RectTransform>();

        layoutGroup.SetParent(scrollView.transform);
        layoutGroup.gameObject.AddComponent<ContentSizeFitter>();
        layoutGroup.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

        foreach (Transform child in layoutGroup)
        {
            child.GetComponent<LayoutElement>().minHeight = 40;
        }
    }
}
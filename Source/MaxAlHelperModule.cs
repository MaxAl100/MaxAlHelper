using System;
using Celeste.Mod.MaxAlHelper.AuspiciousFixes;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper;

public class MaxAlHelperModule : EverestModule
{
    public static MaxAlHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(MaxAlHelperModuleSettings);
    public static MaxAlHelperModuleSettings Settings => (MaxAlHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(MaxAlHelperModuleSession);
    public static MaxAlHelperModuleSession Session => (MaxAlHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(MaxAlHelperModuleSaveData);
    public static MaxAlHelperModuleSaveData SaveData => (MaxAlHelperModuleSaveData)Instance._SaveData;

    public MaxAlHelperModule()
    {
        Instance = this;
    #if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(MaxAlHelperModule), LogLevel.Verbose);
    #else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(MaxAlHelperModule), LogLevel.Info);
    #endif
    }

    public override void Load()
    {
        // TODO: apply any hooks that should always be active
        
        // Initialize the DashNegator fix
        DashNegatorTemplateFix.Initialize();
        
        // Register the template component for DashNegator
        if (Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "auspicioushelper" }) &&
            Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "FactoryHelper" }))
        {
            try
            {
                // Use the template IOP to register our custom component
                var templateIopType = GetAuspiciousHelperType("Celeste.Mod.auspicioushelper.iop.TemplateIop");
                if (templateIopType != null)
                {
                    var customClarifyField = templateIopType.GetField("customClarify", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (customClarifyField?.GetValue(null) is System.Action<string, System.Func<Level, LevelData, Vector2, EntityData, Component>> customClarify)
                    {
                        customClarify.Invoke("FactoryHelper/DashNegator", CreateDashNegatorComponent);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, nameof(MaxAlHelperModule), $"Failed to register DashNegator template component: {ex}");
            }
        }
    }

    public override void Unload()
    {
        // TODO: unapply any hooks applied in Load()
        
        DashNegatorTemplateFix.Uninitialize();
    }

    public class DashNegatorTemplateComponent : Component
    {
        // Your existing implementation
        public Entity DashNegator { get; private set; }

        public DashNegatorTemplateComponent(Entity dashNegator) : base(true, false)
        {
            DashNegator = dashNegator;
        }
    }

    private static Type GetFactoryHelperType(string typeName)
    {
        foreach (var module in Everest.Modules)
        {
            if (module.Metadata.Name == "FactoryHelper")
            {
                return module.GetType().Assembly.GetType(typeName);
            }
        }
        return null;
    }

    private static Type GetAuspiciousHelperType(string typeName)
    {
        foreach (var module in Everest.Modules)
        {
            if (module.Metadata.Name == "auspicioushelper")
            {
                return module.GetType().Assembly.GetType(typeName);
            }
        }
        return null;
    }
}
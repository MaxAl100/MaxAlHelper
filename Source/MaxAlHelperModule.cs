using System;
using System.Collections.Generic;
using System.Diagnostics;
using Celeste.Mod.MaxAlHelper.AuspiciousFixes;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;

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
        typeof(TemplateIop).ModInterop();
        DashNegatorTemplateRegistration.RegisterDashNegatorTemplate();
    }



    public override void Unload()
    {
        // TODO: unapply any hooks applied in Load()
    }
    

}
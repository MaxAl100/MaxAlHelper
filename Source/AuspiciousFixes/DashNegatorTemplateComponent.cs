using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.AuspiciousFixes;

public class DashNegatorTemplateComponent : TemplateIop.TemplateChildComponent
{
    private Entity dashNegator;
    private static FieldInfo turretSolidsField;

    static DashNegatorTemplateComponent()
    {
        EverestModuleMetadata factoryHelper = new EverestModuleMetadata { Name = "FactoryHelper" };
        Logger.Log(LogLevel.Info, "MaxAlHelper", $"Checking for {factoryHelper.Name} dependency...");
        if (Everest.Loader.DependencyLoaded(factoryHelper))
        {
            Logger.Log(LogLevel.Info, "MaxAlHelper", "FactoryHelper is loaded, initializing DashNegatorTemplateComponent.");
            if (Everest.Loader.TryGetDependency(factoryHelper, out EverestModule factoryModule))
            {
                var factoryAssembly = factoryModule.GetType().Assembly;
                var dashNegatorType = factoryAssembly.GetType("FactoryHelper.Entities.DashNegator");
                turretSolidsField = dashNegatorType?.GetField("_turretSolids", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }

    public DashNegatorTemplateComponent(Entity entity) : base(entity)
    {
        dashNegator = entity;

        AddSelf = (List<Entity> entities) =>
        {
            entities.Add(dashNegator);

            if (turretSolidsField != null)
            {
                var turretSolids = turretSolidsField.GetValue(dashNegator) as Solid[];
                if (turretSolids != null)
                {
                    foreach (var solid in turretSolids)
                    {
                        entities.Add(solid);
                    }
                }
            }
        };
    }
}

// Extension method to easily create the component (no assembly reference needed)
public static class EntityTemplateExtensions
{
    public static TemplateIop.TemplateChildComponent CreateDashNegatorTemplateComponent(this Entity entity)
    {
        // Check if this is actually a DashNegator using reflection
        if (entity.GetType().FullName == "FactoryHelper.Entities.DashNegator")
        {
            return new DashNegatorTemplateComponent(entity);
        }
        return null;
    }
}

public static class DashNegatorTemplateRegistration
{
    public static void RegisterDashNegatorTemplate()
    {
        TemplateIop.customClarify?.Invoke("FactoryHelper/DashNegator", (Level l, LevelData d, Vector2 offset, EntityData e) =>
        {
            Entity ent = Level.EntityLoaders["FactoryHelper/DashNegator"](l, d, offset, e);
            TemplateIop.TemplateChildComponent comp = new DashNegatorTemplateComponent(ent);
            return comp;
        });
    }
}
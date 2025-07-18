using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.MaxAlHelper.AuspiciousFixes
{
    public static class DashNegatorTemplateFix
    {
        private static bool _isInitialized = false;
        private static Hook _dashNegatorAddedHook;
        private static Hook _dashNegatorRemovedHook;

        public static void Initialize()
        {
            if (_isInitialized) return;
            
            if (!Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "auspicioushelper" }) ||
                !Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "FactoryHelper" }))
            {
                return;
            }

            try
            {
                Type dashNegatorType = null;
                foreach (var module in Everest.Modules)
                {
                    if (module.Metadata.Name == "FactoryHelper")
                    {
                        dashNegatorType = module.GetType().Assembly.GetType("FactoryHelper.Entities.DashNegator");
                        break;
                    }
                }

                if (dashNegatorType == null)
                {
                    Logger.Log(LogLevel.Warn, "MaxAlHelper", "Could not find FactoryHelper.Entities.DashNegator type");
                    return;
                }

                MethodInfo addedMethod = dashNegatorType.GetMethod("Added", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo removedMethod = dashNegatorType.GetMethod("Removed", BindingFlags.Public | BindingFlags.Instance);

                if (addedMethod != null && removedMethod != null)
                {
                    _dashNegatorAddedHook = new Hook(addedMethod, OnDashNegatorAdded);
                    _dashNegatorRemovedHook = new Hook(removedMethod, OnDashNegatorRemoved);
                    
                    Logger.Log(LogLevel.Info, "MaxAlHelper", "Successfully hooked DashNegator for template compatibility");
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "MaxAlHelper", $"Failed to initialize DashNegator template fix: {ex}");
            }
        }

        public static void Uninitialize()
        {
            _dashNegatorAddedHook?.Dispose();
            _dashNegatorRemovedHook?.Dispose();
            _isInitialized = false;
        }

        private static void OnDashNegatorAdded(Action<Entity, Scene> orig, Entity self, Scene scene)
        {
            orig(self, scene);

            // Check if this entity is part of a template
            var templateComponent = self.Components.Get<TemplateChildComponent>();
            if (templateComponent != null)
            {
                // If it already has a template component, we don't need to add another one
                return;
            }

            // Get the turret solids via reflection
            var turretSolidsField = self.GetType().GetField("_turretSolids", BindingFlags.NonPublic | BindingFlags.Instance);
            if (turretSolidsField?.GetValue(self) is Solid[] turretSolids)
            {
                // Create and add our custom component
                var component = new DashNegatorTemplateComponent(self, turretSolids);
                self.Add(component);
            }
        }

        private static void OnDashNegatorRemoved(Action<Entity, Scene> orig, Entity self, Scene scene)
        {
            orig(self, scene);
        }
    }

    public class DashNegatorTemplateComponent : Component
    {
        public Entity Entity { get; private set; }
        private Solid[] _turretSolids;
        public bool ParentVisible { get; set; } = true;
        public bool ParentCollidable { get; set; } = true;
        public bool ParentActive { get; set; } = true;

        public DashNegatorTemplateComponent(Entity entity, Solid[] turretSolids) : base(false, false)
        {
            Entity = entity;
            _turretSolids = turretSolids;
        }

        public void OnAddTo(Scene scene)
        {
            if (_turretSolids != null)
            {
                foreach (var solid in _turretSolids)
                {
                    if (solid != null)
                    {
                        scene.Add(solid);
                    }
                }
            }
        }

        public void OnAddSelf(List<Entity> entities)
        {
            entities.Add(Entity);
            
            if (_turretSolids != null)
            {
                foreach (var solid in _turretSolids)
                {
                    if (solid != null)
                    {
                        entities.Add(solid);
                    }
                }
            }
        }

        public void OnReposition(Vector2 position, Vector2 liftSpeed)
        {
            Vector2 offset = position - Entity.Position;
            Entity.Position = position;
            
            if (_turretSolids != null)
            {
                for (int i = 0; i < _turretSolids.Length; i++)
                {
                    if (_turretSolids[i] != null)
                    {
                        _turretSolids[i].Position += offset;
                        _turretSolids[i].LiftSpeed = liftSpeed;
                    }
                }
            }
        }

        public void OnChangeStatus(int visible, int collidable, int active)
        {
            if (visible != 0) ParentVisible = visible > 0;
            if (collidable != 0) ParentCollidable = collidable > 0;
            if (active != 0) ParentActive = active > 0;

            if (visible != 0)
            {
                Entity.Visible = ParentVisible;
            }

            if (collidable != 0 && _turretSolids != null)
            {
                foreach (var solid in _turretSolids)
                {
                    if (solid != null)
                    {
                        solid.Collidable = ParentCollidable;
                    }
                }
            }

            if (active != 0)
            {
                Entity.Active = ParentActive;
                
                if (_turretSolids != null)
                {
                    foreach (var solid in _turretSolids)
                    {
                        if (solid != null)
                        {
                            solid.Active = ParentActive;
                        }
                    }
                }
            }
        }

        public void OnDestroy(bool particles)
        {
            if (_turretSolids != null)
            {
                foreach (var solid in _turretSolids)
                {
                    if (solid != null)
                    {
                        solid.RemoveSelf();
                    }
                }
            }

            Entity.RemoveSelf();
        }
    }
}
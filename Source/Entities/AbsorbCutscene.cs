using System;
using System.Collections;
using System.Threading;
using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    internal class AbsorbCutscene : CutsceneEntity
    {
        private Player player;
        private AbsorbCutsceneStarter starter;

        public AbsorbCutscene(Player player, AbsorbCutsceneStarter starter)
        {
            this.player = player;
            this.starter = starter;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
        }

        private IEnumerator Cutscene(Level level)
        {
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;
            Audio.SetMusic(null);
            if (starter.walkBelowTeleport)
            {
                yield return player.DummyWalkToExact((int)starter.Position.X + (int)starter.targetAbsorbPosition.X);
                yield return 0.25f;
                yield return player.DummyWalkToExact((int)starter.Position.X - starter.walkAroundDistance + (int)starter.targetAbsorbPosition.X);
                yield return 0.5f;
                yield return player.DummyWalkToExact((int)starter.Position.X + starter.walkAroundDistance + (int)starter.targetAbsorbPosition.X);
                yield return 0.25f;
            }
            else
            {
                yield return player.DummyWalkToExact((int)starter.Position.X);
                yield return 0.25f;
                yield return player.DummyWalkToExact((int)starter.Position.X - starter.walkAroundDistance);
                yield return 0.5f;
                yield return player.DummyWalkToExact((int)starter.Position.X + starter.walkAroundDistance);
                yield return 0.25f;
            }

            player.Facing = Facings.Left;
            player.DummyAutoAnimate = true;
            Vector2 offset = (Engine.Scene as Level).LevelOffset;
            if (starter.zoomIn)
            {
                Vector2 zoomTarget = new Vector2(
                    starter.Position.X + starter.targetAbsorbPosition.X + starter.targetRoomTransition.X - offset.X,
                    starter.Position.Y + starter.targetAbsorbPosition.Y + starter.targetRoomTransition.Y - offset.Y
                );
                Add(new Coroutine(level.ZoomTo(zoomTarget, 2.3f, 10f)));
            }
            yield return 0.25f;
            player.ForceStrongWindHair.X = -1f;
            yield return player.DummyWalkToExact((int)player.X + 12);
            player.Facing = Facings.Right;
            player.DummyAutoAnimate = false;
            player.DummyGravity = false;
            player.Sprite.Play("runWind");
            while (player.Sprite.Rate > 0f)
            {
                player.MoveH(player.Sprite.Rate * 10f * Engine.DeltaTime * starter.animationSpeedMult);
                player.MoveV((0f - (1f - player.Sprite.Rate)) * 6f * Engine.DeltaTime * starter.animationSpeedMult);
                player.Sprite.Rate -= Engine.DeltaTime * 0.15f * starter.animationSpeedMult;
                yield return null;
            }
            yield return 0.5f;
            player.Sprite.Play("fallFast");
            player.Sprite.Rate = 1f;
            Vector2 target = starter.Position + starter.targetAbsorbPosition - new Vector2(0f, 7f);
            Vector2 from = player.Position;
            for (float p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime * 2f)
            {
                player.Position = from + (target - from) * Ease.SineInOut(p2);
                yield return null;
            }
            yield return 0.1f;
            player.ForceStrongWindHair.X = 0f;
            SpotlightWipe.FocusPoint = player.Position - offset + starter.targetRoomTransition;
            yield return 0.1f;
            player.ForceStrongWindHair.X = 0f;
            SpotlightWipe.FocusPoint = player.Position - offset + starter.targetRoomTransition;
            
            // Create the wipe dynamically based on the selected type - IMPROVED VERSION
            ScreenWipe wipe = CreateWipe(starter.wipeType, level);
            
            // Set the wipe directly on the level's Wipe property for one-time use
            level.Wipe = wipe;
        }

        private ScreenWipe CreateWipe(string wipeTypeName, Level level)
        {
            if (string.IsNullOrEmpty(wipeTypeName))
            {
                return new SpotlightWipe(level, false, () => {
                    Thread.Sleep(100);
                    EndCutscene(level);
                });
            }

            // Special case for MaxHelpingHand's custom wipe

            // Uncomplete, didn't manage to get it working

            // if (wipeTypeName.StartsWith("MaxHelpingHand/CustomWipe:"))
            // {
            //     Type customWipeType = null;

            //     foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //     {
            //         customWipeType = assembly.GetType("Celeste.Mod.MaxHelpingHand.CustomWipe");
            //         if (customWipeType != null)
            //             break;
            //     }

            //     if (customWipeType != null)
            //     {
            //         try
            //         {
            //             // Constructor: CustomWipe(Level level, bool wipeIn, string wipeId, Action onComplete)
            //             object wipe = Activator.CreateInstance(customWipeType, level, false, wipeTypeName, new Action(() => {
            //                 Thread.Sleep(100);
            //                 EndCutscene(level);
            //             }));

            //             return wipe as ScreenWipe;
            //         }
            //         catch (Exception e)
            //         {
            //             Logger.Log("MaxAlHelper", $"Failed to instantiate CustomWipe: {e}");
            //         }
            //     }
            // }

            // Attempt to instantiate as a normal wipe class
            Type wipeType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                wipeType = assembly.GetType(wipeTypeName);
                if (wipeType != null)
                    break;
            }

            if (wipeType != null)
            {
                try
                {
                    return (ScreenWipe)Activator.CreateInstance(wipeType, level, false, new Action(() => {
                        Thread.Sleep(100);
                        EndCutscene(level);
                    }));
                }
                catch (Exception e)
                {
                    Logger.Log("MaxAlHelper", $"Failed to instantiate wipe type {wipeTypeName}: {e}");
                }
            }

            // Final fallback
            return new SpotlightWipe(level, false, () => {
                Thread.Sleep(100);
                EndCutscene(level);
            });
        }


        public override void OnEnd(Level level)
        {
            level.OnEndOfFrame += delegate
            {
                if (!string.IsNullOrEmpty(starter.setFlag))
                {
                    level.Session.SetFlag(starter.setFlag);
                }
                if (WasSkipped)
                {
                    level.Remove(player);
                    level.UnloadLevel();
                    Audio.SetMusic(null);
                    level.Session.Level = starter.targetRoom;
                    level.Session.FirstLevel = false;
                    level.Session.RespawnPoint = GetSpawnPoint(level, starter.targetRoom, starter.targetSpawnId);
                    level.LoadLevel(Player.IntroTypes.None);
                    level.Wipe.Cancel();
                }
                else
                {
                    level.Remove(player);
                    level.EndCutscene();
                    level.UnloadLevel();
                    level.Session.Level = starter.targetRoom;
                    level.Session.FirstLevel = false;
                    level.Session.RespawnPoint = GetSpawnPoint(level, starter.targetRoom, starter.targetSpawnId);
                    if (starter.introType == "Fall") level.LoadLevel(Player.IntroTypes.Fall);
                    else if (starter.introType == "Transition") level.LoadLevel(Player.IntroTypes.Transition);
                    else if (starter.introType == "Respawn") level.LoadLevel(Player.IntroTypes.Respawn);
                    else if (starter.introType == "WalkInRight") level.LoadLevel(Player.IntroTypes.WalkInRight);
                    else if (starter.introType == "WalkInLeft") level.LoadLevel(Player.IntroTypes.WalkInLeft);
                    else if (starter.introType == "Jump") level.LoadLevel(Player.IntroTypes.Jump);
                    else if (starter.introType == "WakeUp") level.LoadLevel(Player.IntroTypes.WakeUp);
                    else if (starter.introType == "ThinkForABit") level.LoadLevel(Player.IntroTypes.ThinkForABit);
                    else if (starter.introType == "TempleMirrorVoid") level.LoadLevel(Player.IntroTypes.TempleMirrorVoid);
                    else level.LoadLevel(Player.IntroTypes.None);
                    Audio.SetMusic(null);
                    level.Camera.Y -= 8f;
                    if (!WasSkipped && level.Wipe != null)
                    {
                        level.Wipe.Cancel();
                    }
                }
            };
        }

        private Vector2 GetSpawnPoint(Level level, string targetRoom, string spawnId)
        {
            // Get the target room's data first
            LevelData levelData = level.Session.MapData.Levels.Find(l => l.Name == targetRoom);
            if (levelData == null)
            {
                // If we can't find the target room, return a safe default
                return Vector2.Zero;
            }

            // If no specific spawn ID is provided or it's empty, use the target room's default spawn
            if (string.IsNullOrEmpty(spawnId) || spawnId == "default")
            {
                // Use the target room's first spawn point
                if (levelData.Spawns != null && levelData.Spawns.Count > 0)
                {
                    return levelData.Spawns[0];
                }
                // If no spawns defined, use the room's bounds center or top-left
                return new Vector2(levelData.Bounds.Left + 8, levelData.Bounds.Top + 8);
            }

            // Try to find the specific spawn point by ID in the target room
            foreach (var entity in levelData.Entities)
            {
                if (entity.Name == "player" && entity.Attr("id", "") == spawnId)
                {
                    // Entity positions are already in world coordinates for the room
                    return entity.Position;
                }
            }

            // If spawn ID not found, fall back to the target room's default spawn
            if (levelData.Spawns != null && levelData.Spawns.Count > 0)
            {
                return levelData.Spawns[0];
            }
            
            // Ultimate fallback - safe position in target room
            return new Vector2(levelData.Bounds.Left + 8, levelData.Bounds.Top + 8);
        }
    }
}
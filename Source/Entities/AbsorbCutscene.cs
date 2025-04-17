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
            Add(new Coroutine(level.ZoomTo(new Vector2(starter.Position.X + starter.targetAbsorbPosition.X - offset.X, starter.Position.Y + starter.targetAbsorbPosition.Y - offset.Y), 2.3f, 10f)));
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
            SpotlightWipe spotWipe = new SpotlightWipe(base.Scene, wipeIn: false, delegate
            {
                Thread.Sleep(100);
                EndCutscene(level);
            });
            level.Add(spotWipe);
        }

        public override void OnEnd(Level level)
        {
            level.OnEndOfFrame += delegate
            {
                if (WasSkipped)
                {
                    level.Remove(player);
                    level.UnloadLevel();
                    Audio.SetMusic(null);
                    level.Session.Level = starter.targetRoom;
                    level.Session.FirstLevel = false;
                    level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Bottom));
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
                    level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Bottom));
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
    }
}

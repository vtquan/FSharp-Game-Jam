// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using GameJam.Core;
using Stride.Physics;

namespace GameJam.Player
{
    public class PlayerInput : SyncScript
    {
        /// <summary>
        /// Raised every frame with the intended direction of movement from the player.
        /// </summary>
        // TODO Should not be static, but allow binding between player and controller
        private bool jumpButtonDown = false;

        public float DeadZone { get; set; } = 0.25f;

        public CameraComponent Camera { get; set; }

        /// <summary>
        /// Multiplies move movement by this amount to apply aim rotations
        /// </summary>
        public float MouseSensitivity = 100.0f;

        public List<Keys> KeysLeft { get; } = new List<Keys>();

        public List<Keys> KeysRight { get; } = new List<Keys>();

        public List<Keys> KeysUp { get; } = new List<Keys>();

        public List<Keys> KeysDown { get; } = new List<Keys>();

        public List<Keys> KeysJump { get; } = new List<Keys>();

        public override void Update()
        {
            // Character movement: should be camera-aware
            {
                // Left stick: movement
                var moveDirection = Input.GetLeftThumbAny(DeadZone);
                var isKeyPress = false;

                // Keyboard: movement
                if (KeysLeft.Any(key => Input.IsKeyDown(key)))
                {
                    isKeyPress = true;
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Left", Entity));
                    moveDirection += -Vector2.UnitX;
                }
                if (KeysRight.Any(key => Input.IsKeyDown(key)))
                {
                    isKeyPress = true;
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Right", Entity));
                    moveDirection += +Vector2.UnitX;
                }
                if (KeysUp.Any(key => Input.IsKeyDown(key)))
                {
                    isKeyPress = true;
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Up", Entity));
                    moveDirection += +Vector2.UnitY;
                }
                if (KeysDown.Any(key => Input.IsKeyDown(key)))
                {
                    isKeyPress = true;
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Down", Entity));
                    moveDirection += -Vector2.UnitY;
                }
                if (KeysJump.Any(key => Input.IsKeyDown(key)))
                {
                    isKeyPress = true;
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Jump", Entity));
                }
                else if (Entity.Get<CharacterComponent>().IsGrounded)
                {
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Grounded", Entity));
                }
                else if (Entity.Get<CharacterComponent>().IsGrounded == false)
                {
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("Airborne", Entity));
                }
                if (isKeyPress == false)
                {
                    Events.PlayerEventKey.Broadcast(new Tuple<string, Entity>("NoMovement", Entity));
                }
            }

            // Camera rotation: left-right rotates the camera horizontally while up-down controls its altitude
            {
                // Right stick: camera rotation
                var cameraDirection = Input.GetRightThumbAny(DeadZone);
                var isDeadZoneRight = cameraDirection.Length() < DeadZone;
                if (isDeadZoneRight)
                    cameraDirection = Vector2.Zero;
                else
                    cameraDirection.Normalize();

                // Mouse-based camera rotation. Only enabled after you click the screen to lock your cursor, pressing escape cancels this
                if (Input.IsMouseButtonDown(MouseButton.Left))
                {
                    Input.LockMousePosition(true);
                    Game.IsMouseVisible = false;
                }
                if (Input.IsKeyPressed(Keys.Escape))
                {
                    Input.UnlockMousePosition();
                    Game.IsMouseVisible = true;
                }
                if (Input.IsMousePositionLocked)
                {
                    cameraDirection += new Vector2(Input.MouseDelta.X, -Input.MouseDelta.Y)*MouseSensitivity;
                }

                // Broadcast the camera direction directly, as a screen-space Vector2
                Events.CameraDirectionEventKey.Broadcast(cameraDirection);
            }

            // Jumping: don't bother with jump restrictions here, just pass the button states
            {
                // Controller: jumping
                var isJumpDown = Input.IsGamePadButtonDownAny(GamePadButton.A);
                var didJump = (!jumpButtonDown && isJumpDown);
                jumpButtonDown = isJumpDown;

                // Keyboard: jumping
                didJump |= (KeysJump.Any(key => Input.IsKeyPressed(key)));
            }
        }
    }
}

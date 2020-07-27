﻿using Client.Managers;
using GameClient.Managers;
using GameClient.Managers.UI;
using GameClient.Scenes;
using GameClient.Types.Components.Components;
using GameServer.General;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.UI;
using Server.Managers;
using Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Direction = Nez.Direction;

namespace GameClient.Types.Components.SceneComponents
{
    class InputComponent : SceneComponent
    {
        float timer = 0;
        public Camera Camera { get; set; }

        private float currentMouseWheelValue, previousMouseWheelValue;
        private Entity Entity;
        public static Skin skin = Skin.CreateDefaultSkin();

        public static Direction direction = Direction.Right;
        public static bool IsMoving = false;

        public InputComponent(Entity entity, Camera camera)
        {
            Entity = entity;
            Camera = camera;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
        }

        public override void Update()
        {
            timer += Time.DeltaTime;

            if (timer >= ConstantValues.UpdateFrequency)
            {
                timer = 0;
                CheckForInput();
            }
            base.Update();
        }
        public void CheckForInput()
        {
            if (ClientNetworkManager.client.ConnectionStatus != NetConnectionStatus.Connected || Core.Scene is LoginScene)
                return;

            //Update Keyboard

            if (Input.CurrentKeyboardState.GetPressedKeys().Length > 0)
                SendKeyboardRequest(new KeyboardState(KeyboardChange()));
            else
                IsMoving = false;
            //Update mouse
            if (Input.LeftMouseButtonPressed || Input.RightMouseButtonPressed || Input.MousePositionDelta != Point.Zero)
                MouseChange();

            previousMouseWheelValue = currentMouseWheelValue;
            currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;
            ScrollChange();
        }

        //TODO: Change to customizable keybindings later
        private Keys[] KeyboardChange()
        {
            KeyboardState newState = Input.CurrentKeyboardState;
            KeyboardState OldKeyboardState = Input.PreviousKeyboardState;

            List<Keys> keys = new List<Keys>();
            float speed = 500;
            var dir = Vector2.Zero;

            //might be usable later for abilities and more.
            if (newState.IsKeyDown(Keys.T) && !OldKeyboardState.IsKeyDown(Keys.T))
            {
                keys.Add(Keys.T);
            }

            //Generated inventory
            if (newState.IsKeyDown(Keys.I) && OldKeyboardState.IsKeyUp(Keys.I))
            {
                List<Window> windows = (Scene as MainScene).UICanvas.Stage.FindAllElementsOfType<Window>();
                bool exists = false;
                windows.ForEach(i =>
                {
                    if (i.GetTitleLabel().GetText().Equals("Inventory"))
                    {
                        exists = true;
                    }
                });
                if (exists)
                {
                    UIManager.FindElementByStringAndRemove("Inventory", Scene);
                }
                else
                {
                    UIManager.GenerateInventoryWindow(skin, Scene, new Vector2(-1, -1), -1, -1);
                }
            }

            if (newState.IsKeyDown(Keys.C) && OldKeyboardState.IsKeyUp(Keys.C))
            {
                List<Window> windows = (Scene as MainScene).UICanvas.Stage.FindAllElementsOfType<Window>();
                bool exists = false;
                windows.ForEach(i =>
                {
                    if (i.GetTitleLabel().GetText().Equals("Character Information"))
                    {
                        exists = true;
                    }
                });
                if (exists)
                {
                    UIManager.FindElementByStringAndRemove("Character Information", Scene);
                }
                else
                {
                    UIManager.GenerateCharacterWindow(skin, Scene, new Vector2(-1, -1), -1, -1);
                }

            }

            if (newState.IsKeyDown(Keys.S) && !OldKeyboardState.IsKeyDown(Keys.S))
            {
                direction = Direction.Down;

                IsMoving = true;
            }
            else if (newState.IsKeyDown(Keys.S) && OldKeyboardState.IsKeyDown(Keys.S))
            {
                // the player is holding the key down
                //LoginManagerClient.GetCharacter()._pos.Y += 3*60f * Time.DeltaTime;
                //LoginManagerClient.GetCharacter()._pos.Y += 1f;
                keys.Add(Keys.S);
                dir.Y = 1f;

                IsMoving = true;
            }
            else if (!newState.IsKeyDown(Keys.S) && OldKeyboardState.IsKeyDown(Keys.S))
            {
                // the player was holding the key down, but has just let it go

            }

            if (newState.IsKeyDown(Keys.W) && !OldKeyboardState.IsKeyDown(Keys.W))
            {
                direction = Direction.Up;

                IsMoving = true;
            }
            else if (newState.IsKeyDown(Keys.W) && OldKeyboardState.IsKeyDown(Keys.W))
            {
                // the player is holding the key down
                //LoginManagerClient.GetCharacter()._pos.Y -= 3*60f * Time.DeltaTime;
                //LoginManagerClient.GetCharacter()._pos.Y -= 1f;
                dir.Y = -1f;
                keys.Add(Keys.W);
                IsMoving = true;

            }
            else if (!newState.IsKeyDown(Keys.W) && OldKeyboardState.IsKeyDown(Keys.W))
            {
                // the player was holding the key down, but has just let it go

            }

            if (newState.IsKeyDown(Keys.A) && !OldKeyboardState.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                IsMoving = true;
            }
            else if (newState.IsKeyDown(Keys.A) && OldKeyboardState.IsKeyDown(Keys.A))
            {
                // the player is holding the key down
                //LoginManagerClient.GetCharacter()._pos.X -= 3*60f * Time.DeltaTime;
                //LoginManagerClient.GetCharacter()._pos.X -= 1f;
                keys.Add(Keys.A);
                dir.X = -1f;
                IsMoving = true;
            }
            else if (!newState.IsKeyDown(Keys.A) && OldKeyboardState.IsKeyDown(Keys.A))
            {
                // the player was holding the key down, but has just let it go

            }

            if (newState.IsKeyDown(Keys.D) && !OldKeyboardState.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                IsMoving = true;

            }
            else if (newState.IsKeyDown(Keys.D) && OldKeyboardState.IsKeyDown(Keys.D))
            {
                // the player is holding the key down
                //LoginManagerClient.GetCharacter()._pos.X += 3*60f * Time.DeltaTime;
                //LoginManagerClient.GetCharacter()._pos.X += 1f;
                keys.Add(Keys.D);
                dir.X = 1f;
                IsMoving = true;
            }
            else if (!newState.IsKeyDown(Keys.D) && OldKeyboardState.IsKeyDown(Keys.D))
            {
                // the player was holding the key down, but has just let it go

            }
            var movement = dir * speed * Time.DeltaTime;
            if (movement != Vector2.Zero)
                LoginManagerClient.GetCharacter()._pos += movement;
            return keys.ToArray();
        }

        private void MouseChange()
        {

            Vector2 pos = Entity.Scene.Camera.MouseToWorldPoint();
            List<Entity> entities = Scene.FindEntitiesWithTag(7);
            foreach (var entity in entities)
            {
                SpriteAnimator sp = entity.GetComponent<SpriteAnimator>();

                Rectangle rect = sp.CurrentAnimation.Sprites.ElementAt(0).SourceRect;
                Rectangle rectangle = new Rectangle(entity.Position.ToPoint(), new Point((int)(rect.Width * entity.Scale.X), (int)(rect.Height * entity.Scale.Y)));
                if (rectangle.Contains(pos))
                {
                    
                    if (Input.LeftMouseButtonPressed)
                    {
                        MessageTemplate template = new MessageTemplate(Entity.Name, MessageType.Target);
                        MessageManager.AddToQueue(template);

                        if (Scene is MainScene)
                        {
                            MainScene scene = Scene as MainScene;
                            Label component = new Label("Target: " + entity.Name).SetFontScale(5).SetFontColor(Color.Red);
                            scene.Table.Add(component).SetRow();
                        }
                    }
                }
            }
        }

        public void ScrollChange()
        {
            if (currentMouseWheelValue > previousMouseWheelValue)
            {
                Camera.ZoomIn(.05f);
            }

            if (currentMouseWheelValue < previousMouseWheelValue)
            {
                Camera.ZoomOut(.05f);
            }

            if (Entity != null)
            {
                FollowCamera fc = Entity.GetComponent<FollowCamera>();
                if (fc != null && fc.Camera != null)
                {
                    //So the camera is centered after zooming in our out
                    fc.Follow(Entity, FollowCamera.CameraStyle.LockOn);

                }
            }

        }
        private void SendKeyboardRequest(KeyboardState keyboardState)
        {
            string messageS = Newtonsoft.Json.JsonConvert.SerializeObject(keyboardState.GetPressedKeys());
            MessageManager.AddToQueue(new MessageTemplate(messageS, MessageType.Movement));
        }

    }
}

﻿using Client.Managers;
using GameClient.Managers.UI.Elements;
using GameClient.Scenes;
using GameClient.Types.Item;
using GameServer.Types.Item;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using Server.Managers;
using Server.Types;
using System;
using System.Runtime.Remoting;

namespace GameClient.Managers.UI
{
    class UIManager
    {
        public static Window GenerateInventoryWindow(Skin skin, Stage stage)
        {
            Window window = new Window("Inventory", skin).SetMovable(true).SetResizable(true);
            window.SetResizeBorderSize(20);

            var textButtonStyle = new ImageButtonStyle
            {
                Up = new PrimitiveDrawable(Color.DimGray, 6, 2),
                Over = new PrimitiveDrawable(Color.DimGray),
                Down = new PrimitiveDrawable(Color.DimGray),
                PressedOffsetX = 0,
                PressedOffsetY = 0
            };

            int count = 0;
            foreach (var i in LoginManagerClient.GetCharacter().Inventory)
            {
                if (count % 5 == 0)
                    window.Row();

                if (i != null)
                {
                    ItemButton imButton = new ItemButton(textButtonStyle);
                    imButton.item = i;
                    imButton.Add(new Label(i.Name)).Fill().Expand().Left().SetAlign(Align.BottomLeft);
                    imButton.SetTouchable(Touchable.Enabled);
                    imButton.OnHovered += delegate { OnHovered(imButton, i, skin, stage); };
                    imButton.OnExited += delegate { OnExit(imButton); };
                    imButton.OnMoved += delegate { OnMovedAndHovered(imButton); };
                    imButton.OnClicked += delegate { OnClicked(imButton, count); };
                    window.Add(imButton).Pad(4).Fill().Expand();
                }
                count++;
            }
            window.Pack();
            window.DebugAll();

            return window;
        }

        public static Window GenerateCharacterWindow(Skin skin, Stage stage)
        {
            Window window = new Window("Character Information", skin).SetMovable(true).SetResizable(true);
            window.SetResizeBorderSize(20);

            var textButtonStyle = new ImageButtonStyle
            {
                Up = new PrimitiveDrawable(Color.DimGray, 6, 2),
                Over = new PrimitiveDrawable(Color.DimGray),
                Down = new PrimitiveDrawable(Color.DimGray),
                PressedOffsetX = 0,
                PressedOffsetY = 0
            };

            int count = 0;
            foreach (var i in LoginManagerClient.GetCharacter().Equipment)
            {
                if (count % 4 == 0)
                    window.Row();

                if (i != null)
                {
                    ItemButton imButton = new ItemButton(textButtonStyle);
                    imButton.item = i;
                    imButton.Add(new Label(i.Name)).Fill().Expand().Left().SetAlign(Align.BottomLeft);
                    imButton.SetTouchable(Touchable.Enabled);
                    imButton.OnHovered += delegate { OnHovered(imButton, i, skin, stage); };
                    imButton.OnExited += delegate { OnExit(imButton); };
                    imButton.OnMoved += delegate { OnMovedAndHovered(imButton); };
                    window.Add(imButton).Pad(4).Fill().Expand();
                }
                count++;
            }
            window.Pack();
            window.DebugAll();
            return window;
        }
        #region Mouse hover item

        private static void OnMovedAndHovered(ItemButton obj)
        {
            if (obj.HoverWindow != null)
            {
                Vector2 tempPos = CalculatePosition(obj.HoverWindow);

                obj.HoverWindow.SetBounds(tempPos.X, tempPos.Y, obj.HoverWindow.GetWidth(), obj.HoverWindow.GetHeight());
            }
        }

        private static void OnExit(ItemButton obj)
        {
            if (obj.HoverWindow != null)
            {
                obj.HoverWindow.SetVisible(false);
                obj.HoverWindow.SetIsVisible(false);
                obj.HoverWindow.Remove();
            }
        }

        private static void OnHovered(ItemButton obj, WeaponItem item, Skin skin, Stage stage)
        {

            Window window = new Window("Inventory", skin);
            window.GetTitleLabel().SetVisible(false);
            window.GetTitleLabel().SetIsVisible(false);

            window.Add(new Label(item.Name, skin).SetFontScale(1.5f)).Expand().SetRow();
            window.Add(new Label("Attributes", skin)).SetRow();
            window.Add(new Label("Intelligence:" + item.Intelligence.ToString(), skin)).Fill().Expand().SetRow();
            window.Add(new Label("Strength:" + item.Strength.ToString(), skin)).Fill().Expand().SetRow();
            window.Add(new Label("Dexterity:" + item.Dexterity.ToString(), skin)).Fill().Expand().SetRow();

            window.Add(new Label("Reqs", skin)).SetRow();
            window.Add(new Label("Intelligence:" + item.IntelligenceReq.ToString(), skin)).Fill().Expand().SetRow();
            window.Add(new Label("Strength:" + item.StrengthReq.ToString(), skin)).Fill().Expand().SetRow();
            window.Add(new Label("Dexterity:" + item.DexterityReq.ToString(), skin)).Fill().Expand().SetRow();

            Vector2 tempPos = CalculatePosition(window);
            window.SetPosition(tempPos.X, tempPos.Y);
            stage.AddElement(window);
            obj.HoverWindow = window;
            obj.HoverWindow.SetVisible(true);
            obj.HoverWindow.SetIsVisible(true);
        }

        private static void OnClicked(ItemButton obj, int index)
        {
            if (Input.LeftMouseButtonReleased)
            {
                if (obj.GetParent() is Window)
                {
                    //Window w = obj.GetParent() as Window;
                    //int ind = w.GetChildren().FindIndex(i => i.Equals(obj));
                    
                }

                int ind = -1;
                for (int i = 0; i < LoginManagerClient.GetCharacter().GetEquipment().Length - 1; i++)
                {
                    WeaponItem item = LoginManagerClient.GetCharacter().GetInventory()[i];
                    if (obj.item != null && item != null && item.Name.Equals(obj.item.Name))
                    {
                        ind = i;
                    }
                }
                if (ind != -1)
                {
                    MessageTemplate template = new MessageTemplate(ind.ToString(), MessageType.EquipItem);
                    MessageManager.AddToQueue(template);

                }
            }
        }

        #endregion

        //HELP METHODS
        private static Vector2 CalculatePosition(Window window)
        {
            return new Vector2(Input.ScaledMousePosition.X, Input.ScaledMousePosition.Y - 30 - window.GetHeight());
        }

        public static bool FindElementByString(string str, Scene scene)
        {
            Element element = null;
            element = (scene as MainScene).UICanvas.Stage.GetElements().Find(i =>
            {
                if (i is Window)
                {
                    return (i as Window).GetTitleLabel().GetText().Equals(str);
                }
                return false;
            });

            //Removes every window if present
            if (element != null && element is Window)
            {
                foreach (var item in (element as Window).GetChildren())
                {
                    if (item is ItemButton && (item as ItemButton).HoverWindow != null)
                    {
                        (item as ItemButton).HoverWindow.Remove();
                    }
                }

            }
            if (element != null)
                element.Remove();
            return element == null;
        }
    }
}

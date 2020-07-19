﻿using GameClient.Types.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace GameClient.General
{
    class TextureContainer
    {
        public static HashSet<Texture2DE> Texture2DEList = new HashSet<Texture2DE>();
        public static SpriteAtlas ItemAtlas;
        public static SpriteAtlas UIAtlas;
        public void LoadTextures()
        {
            ItemAtlas = Core.Content.LoadSpriteAtlas("Assets/Items/Items.atlas");
            UIAtlas = Core.Content.LoadSpriteAtlas("Assets/UI/UI.atlas");
            Texture2D tempTexture = Core.Content.Load<Texture2D>("images/playerTexture");
            Texture2DE tempT2DE = new Texture2DE(tempTexture.GraphicsDevice, tempTexture.Width, tempTexture.Height);
            int count = tempTexture.Width * tempTexture.Height;
            Color[] data = new Color[count];
            tempTexture.GetData(0, tempTexture.Bounds, data, 0, count);
            tempT2DE.SetData(data);

            Texture2DEList.Add(tempT2DE.SetID(2));
        }
        
        public static Texture2DE GetTextureByID(int ID)
        {
            return Texture2DEList.FirstOrDefault(t => t.ID.Equals(ID));
        }
    }
}

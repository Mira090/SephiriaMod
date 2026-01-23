using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public static class CustomSpriteAsset
    {
        public static TMP_SpriteAsset SpriteAsset;
        public static Sprite NewKeywordSprite;

        public static Sprite Planet;
        public static Sprite BinaryPlanet;
        public static Sprite IceExecution;
        public static Sprite FireExecution;
        public static Sprite LightningExecution;
        public static Sprite Assasination;

        public static int width = 10;
        public static int height = 10;
        public static void InitSprites()
        {
            NewKeywordSprite = SpriteLoader.LoadSprite(ModUtil.UIPath + "Keyword");
            NewKeywordSprite.texture.wrapMode = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeU = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeV = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeW = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.name = "ModKeyword";
            int max = 100 - height;

            Planet = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "Planet", new Rect(0, max, width, height));
            BinaryPlanet = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "BinaryPlanet", new Rect(0, height * 1, width, height));
            IceExecution = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "IceExecution", new Rect(0, height * 2, width, height));
            FireExecution = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "FireExecution", new Rect(0, height * 3, width, height));
            LightningExecution = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "LightningExecution", new Rect(0, height * 4, width, height));
            Assasination = SpriteLoader.CreateSprite(NewKeywordSprite.texture, "Assasination", new Rect(0, height * 5, width, height));

        }
        public static void InitSpriteAsset()
        {
            if (SpriteAsset != null)
                return;
            int max = 100 - height;
            SpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            SpriteAsset.name = "Keyword";
            SpriteAsset.spriteSheet = NewKeywordSprite.texture;
            SpriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));
            SpriteAsset.material.mainTexture = NewKeywordSprite.texture;
            SpriteAsset.spriteInfoList = new();
            int id = 0;
            int unicodeStart = 0xE000;
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max, name = "Planet", sprite = Planet });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 1, name = "BinaryPlanet", sprite = BinaryPlanet });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 2, name = "IceExecution", sprite = IceExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 3, name = "FireExecution", sprite = FireExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 4, name = "LightningExecution", sprite = LightningExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 5, name = "Assasination", sprite = Assasination });
            foreach (var sprite in SpriteAsset.spriteInfoList)
            {
                sprite.id = id++;
                sprite.scale = 1.1f;
                sprite.width = width;
                sprite.height = height;
                sprite.yOffset = 9;
                //if (sprite.x == 0)
                    //sprite.xOffset = -1;
                sprite.xAdvance = 10;
                sprite.unicode = unicodeStart + id - 1;
                sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(sprite.name);
            }

            SpriteAsset.UpdateLookupTables();

            TMP_Settings.instance.SetDefaultSpriteAsset(SpriteAsset);

            int count = 0;
            foreach (var item in SpriteAsset.spriteCharacterTable)
            {
                item.glyph = SpriteAsset.spriteGlyphTable[count];
                count++;
            }
        }
    }
}

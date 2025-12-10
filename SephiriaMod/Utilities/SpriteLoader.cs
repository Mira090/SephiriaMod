using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Utilities
{
    public class SpriteLoader
    {
        public static Sprite LoadSprite(string name)
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\CustomImages\" + name + ".png";
            if (!File.Exists(path))
            {
                Melon<Core>.Logger.Msg(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite LoadSprite(string name, Rect rect)
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\CustomImages\" + name + ".png";
            if (!File.Exists(path))
            {
                Melon<Core>.Logger.Msg(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                rect,
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite CreateSprite(Texture2D tex, string name, Rect rect)
        {
            var sprite = Sprite.Create(
                tex,
                rect,
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        /// <summary>
        /// ピボットがLoadSprite()と違う
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite LoadSpritePath(string path)
        {
            if (!File.Exists(path))
            {
                Melon<Core>.Logger.Msg(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.2f), 16
            );
            sprite.name = Path.GetFileNameWithoutExtension(path);
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        [Obsolete("一部画像が正しく保存されない。保存機能は別プロジェクトに移行")]
        public static void SaveSprite(Sprite sprite, string name)
        {
            if (sprite == null)
                return;
            SaveSprite(sprite.texture, name);
        }
        [Obsolete("一部画像が正しく保存されない。保存機能は別プロジェクトに移行")]
        public static void SaveSprite(Texture2D texture, string name)
        {
            if (texture == null)
                return;
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\Outputs\" + name + ".png";
            //var copyTexture = new Texture2D(texture.width, texture.height, texture.format, false);
            // テクスチャのコピー
            //Graphics.CopyTexture(texture, copyTexture);

            //テクスチャの外側を定義
            //Texture2D tex = new Texture2D(copyTexture.width, copyTexture.height);
            //ピクセル情報を入れる処理
            //tex.SetPixels(copyTexture.GetPixels());
            //エンコード処理
            var png = texture.EncodeToPNG();
            File.WriteAllBytes(path, png);
        }
    }
}

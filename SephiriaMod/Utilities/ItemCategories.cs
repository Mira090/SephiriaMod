using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SephiriaMod.Utilities
{
    public class ItemCategories
    {
        static ItemCategories()
        {
            var type = typeof(ItemCategories);
            var pros = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(string));
            foreach( var pro in pros)
            {
                pro.SetValue(type, pro.Name.ToSephiriaUpperId());
                //Melon<Core>.Logger.Msg("Categories: " + pro.Name.ToSephiriaUpperId());
            }
        }
        public static string Charging { get; private set; }
        public static string Companion { get; private set; }
        public static string DarkCloud { get; private set; }
        public static string Ember { get; private set; }
        public static string Favorites { get; private set; }
        public static string FlameSword { get; private set; }
        public static string Frost { get; private set; }
        public static string Glacier { get; private set; }
        public static string Guardian { get; private set; }
        public static string Magitech { get; private set; }
        public static string Mystic { get; private set; }
        public static string None { get; private set; }
        public static string Planet { get; private set; }
        public static string Precision { get; private set; }
        public static string Shadow { get; private set; }
        public static string Sturdy { get; private set; }
        public static string WindSong { get; private set; }
        public static string Academy { get; private set; }
        public static string Lake { get; private set; }
        //public static string Purgatory { get; private set; }
        public static string Vitality { get; private set; }
        public static string Stargaze { get; private set; }
        public static string SkySong { get; private set; }
        public static string Drunk { get; private set; }
    }
}

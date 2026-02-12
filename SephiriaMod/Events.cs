using HarmonyLib;
using MelonLoader;
using Mirror;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;
using static SephiriaMod.Core;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using SephiriaMod.Items;
using SephiriaMod.Utilities;
using TMPro;

namespace SephiriaMod
{
    public static class Events
    {
        public static event Action<WeaponSimple_Crossbow> OnSubAttackCrossbow;
        public static event Action<WeaponSimple_Katana> OnSubAttackKatana;
        public static event Action<WeaponSimple_GreatSword> OnSubAttackGreatSword;
        public static event Action<Charm_PallasCard, int> OnPallasSpawnChance;
        public static event Action<Charm_PallasAce, int> OnAceSpawnChance;
        public static event Action<string, uint, int> OnValueRecieved;
        public static Dictionary<string, string> CustomBulletDestroyModuleName = new();
        public static Dictionary<string, Action<BulletDestroyModule, uint, bool, Vector3, float, float>> CustomBulletDestroyModule = new();
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreBasicAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreSpecialAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreDashAttack;

        public static EventReference HealSound { get; } = RuntimeManager.PathToEventReference("event:/Scene/healPotion_Small01");
        public static EventReference PerkSound { get; } = RuntimeManager.PathToEventReference("event:/System/talentPerk");
        static Events()
        {
            OnValueRecieved += (string command, uint netId, int value) =>
            {
                //Melon<Core>.Logger.Msg($"Mod Chat({command}): {netId} To {value}");
            };
        }


        public static void CommandValue(UnitAvatar player, NewItemOwnInstance item, int value)
        {
            if ((bool)DungeonManager.Instance)
            {
                if(item.Charm)
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/value {item.Charm.netId} {value}");
                if(item.StoneTablet)
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/value {item.StoneTablet.netId} {value}");
            }
            else
            {
                Melon<Core>.Logger.Msg("CommandValue() DungeonManager.Instance is null!!");
            }
        }
        public static void ChatSound(UnitAvatar player, string name)
        {
            if ((bool)DungeonManager.Instance)
            {
                DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/sound {name}");
            }
            else
            {
                Melon<Core>.Logger.Msg("CommandValue() DungeonManager.Instance is null!!");
            }
        }
        public static void PlaySound(UnitAvatar player, string message)//  /sound 
        {
            var id = message.Remove(0, 7);

            try
            {
                EventInstance eventInstance = RuntimeManager.CreateInstance(id);
                eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
            }
            catch (EventNotFoundException e)
            {

            }
            var param = id.Split(' ');
            if(param.Length == 4)
            {
                var list = new List<int>();
                foreach(var p in param)
                {
                    if(Int32.TryParse(p, out var value))
                    {
                        list.Add(value);
                    }
                }
                if(list.Count == 4)
                {
                    var Guid = new FMOD.GUID();
                    Guid.Data1 = list[0];
                    Guid.Data2 = list[1];
                    Guid.Data3 = list[2];
                    Guid.Data4 = list[3];

                    var refe = new EventReference();
                    refe.Guid = Guid;
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Log", $"path {Guid.GUIDToPath()}");
                    try
                    {
                        EventInstance eventInstance = RuntimeManager.CreateInstance(refe);
                        eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                        eventInstance.start();
                        eventInstance.release();
                    }
                    catch(EventNotFoundException e)
                    {
                        DungeonManager.Instance.Chat(player as PlayerAvatar, "Log", $"the sound event is not Found!");
                    }
                }
            }

            foreach (var sound in typeof(Events).GetProperties().Where(p => p.PropertyType == typeof(EventReference)))
            {
                if (id != sound.Name)
                    continue;
                EventInstance eventInstance = RuntimeManager.CreateInstance((EventReference)sound.GetValue(typeof(Events)));
                eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
                return;
            }
        }

        [HarmonyPatch(typeof(UI_TitleLobby), ("Start"))]
        public static class UI_TitleLobbyPatch
        {
            public static GameObject ModListObject = null;
            public static RectTransform ModListTransform = null;
            public static TextMeshProUGUI ModListText = null;
            static void Postfix(UI_TitleLobby __instance)
            {
                ModListObject = new GameObject("ModListObject");
                ModListObject.transform.SetParent(__instance.transform);
                ModListText = ModListObject.AddComponent<TextMeshProUGUI>();
                //ModListObject.AddComponent<UI_LocalizationFontChanger>();
                ModListTransform = ModListObject.transform as RectTransform;
                ModListTransform.localPosition = Vector3.zero;
                ModListTransform.localScale = Vector3.one * 0.75f;
                ModListTransform.anchorMin = new Vector2(0f, 1f);
                ModListTransform.anchorMax = new Vector2(0f, 1f);
                ModListTransform.pivot = new Vector2(0f, 1f);
                ModListTransform.anchoredPosition = new Vector2(0f, 0f);


                StringBuilder sb = new("Mods\n");
                foreach (var melon in MelonMod.RegisteredMelons)
                {
                    sb.AppendLine("<size=150%>" + melon.Info.Name + "</size> <alpha=#AA>v" + melon.Info.Version + "\nby " + melon.Info.Author + "<alpha=#FF>");
                }
                ModListText.text = sb.ToString();
                ModListText.fontSize = 8;
                ModListText.enableWordWrapping = false;
                ModListText.margin = Vector4.one * 12f;
                ModListText.raycastTarget = false;
            }
        }
        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.GetMysticPotItems), [typeof(EItemRarity)])]
        public static class UnitAvatarGetMysticPotItemsPatch
        {
            static void Postfix(EItemRarity targetRarity, ref ItemEntity[] __result, UnitAvatar __instance)
            {
                if(targetRarity == EItemRarity.Legend)
                {
                    var temp = __result.ToList();
                    temp.Remove(Data.WarCrime.ItemEntity);
                    __result = temp.ToArray();
                }
                else if(targetRarity == EItemRarity.Common)
                {
                    var temp = __result.ToList();
                    temp.Remove(Data.Malice.ItemEntity);
                    __result = temp.ToArray();
                }
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Crossbow), nameof(WeaponSimple_Crossbow.SubAttackButtonDown))]
        public static class WeaponSimple_CrossbowPatch
        {
            static void Postfix(WeaponSimple_Crossbow __instance)
            {
                OnSubAttackCrossbow?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Katana), nameof(WeaponSimple_Katana.SubAttackButtonDown))]
        public static class WeaponSimple_KatanaPatch
        {
            static void Postfix(WeaponSimple_Katana __instance)
            {
                OnSubAttackKatana?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_GreatSword), nameof(WeaponSimple_GreatSword.SubAttackButtonUp))]
        public static class WeaponSimple_GreatSwordPatch
        {
            static void Postfix(WeaponSimple_GreatSword __instance)
            {
                OnSubAttackGreatSword?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(Charm_PallasCard), "OnBeginAttackAnimation", new Type[] { typeof(int) })]
        public static class Charm_PallasCardOnBeginAttackAnimationPatch
        {
            static void Prefix(int idx, Charm_PallasCard __instance)
            {
                if (!__instance.WeaponController || !__instance.WeaponController.currentWeapon || !__instance.throwIntervalTimer.Check())
                {
                    return;
                }
                OnPallasSpawnChance?.Invoke(__instance, idx);
            }
        }
        public static void InvokeOnAceSpawnChance(int idx, Charm_PallasAce __instance)
        {
            OnAceSpawnChance?.Invoke(__instance, idx);
        }
        [HarmonyPatch(typeof(DungeonManager), "UserCode_RpcChat__PlayerAvatar__String__String", new Type[] { typeof(PlayerAvatar), typeof(string), typeof(string) })]
        public static class DungeonManagerChatPatch
        {
            static bool Prefix(PlayerAvatar avatar, string name, string message, ref DungeonManager __instance)
            {
                if (name == "Mod" && message.StartsWith("/"))
                {
                    Melon<Core>.Logger.Msg($"Mod Chat({avatar.Name}): {message}");
                    if (message.StartsWith("/sound"))
                    {
                        PlaySound(avatar, message);
                    }
                    if (avatar.isOwned)
                    {
                        if (message == "/stargaze" && UIManager.Instance != null)
                        {
                            UIManager.Instance.GetElement<UI_SystemMessage>().Open(Charm_StargazeTablet.Notice.ToString(), 2.7f);

                            EventInstance eventInstance = RuntimeManager.CreateInstance(PerkSound);
                            eventInstance.set3DAttributes(avatar.transform.position.To3DAttributes());
                            eventInstance.start();
                            eventInstance.release();
                        }
                        if(message == "/sacrifice" && UIManager.Instance != null)
                        {
                            UIManager.Instance.GetElement<UI_SystemMessage>().Open(Charm_CompanionSacrifice.Notice.ToString(), 2.7f);

                            EventInstance eventInstance = RuntimeManager.CreateInstance(PerkSound);
                            eventInstance.set3DAttributes(avatar.transform.position.To3DAttributes());
                            eventInstance.start();
                            eventInstance.release();
                        }
                        if(message == "/dash")
                        {
                            if (avatar.CurrentDashModule.currentDashCount > 0)
                                avatar.CurrentDashModule.currentDashCount--;
                            avatar.Dash(avatar.NetworkaimObject.transform.position);
                        }
                        if (message == "/dash_heal")
                        {
                            if (avatar.CurrentDashModule.currentDashCount > 0)
                                avatar.CurrentDashModule.currentDashCount--;
                        }
                        if (message.StartsWith("/value"))
                        {
                            var data = message.Split(' ');
                            if(data.Length == 3 && uint.TryParse(data[1], out uint netId) && int.TryParse(data[2], out int value) )
                            {
                                OnValueRecieved?.Invoke(data[0], netId, value);
                            }
                            else
                            {
                                Melon<Core>.Logger.Msg("Error: " + message);
                            }
                        }
                    }
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(DungeonManager), "UserCode_RpcBulletDestroyed__UInt32__Boolean__String__Vector3__Single__Single", new Type[] { typeof(uint), typeof(bool), typeof(string), typeof(Vector3), typeof(float), typeof(float) })]
        public static class DungeonManagerBulletDestroyPatch
        {
            static bool Prefix(uint ownerNetId, bool canBeTransparentOnMultiplayer, string bulletName, Vector3 position, float height, float angle, DungeonManager __instance)
            {
                if(CustomBulletDestroyModule.ContainsKey(bulletName))
                {
                    var name = CustomBulletDestroyModuleName[bulletName];
                    Bullet prefab = Bullet.Pool.GetPrefab(name);
                    if ((bool)prefab && (bool)prefab.DestroyModule)
                    {
                        CustomBulletDestroyModule[bulletName]?.Invoke(prefab.DestroyModule, ownerNetId, canBeTransparentOnMultiplayer, position, height, angle);
                        return false;
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(UnitAvatar), "RpcShowDamageParticle", [typeof(Vector2), typeof(string), typeof(Color), typeof(int), typeof(bool), typeof(UnitAvatar), typeof(UnitAvatar)])]
        public static class UnitAvatarRpcShowDamageParticlePatch
        {
            static void Prefix(Vector2 position, ref string msg, ref Color color, int fontSize, bool isPrivate, UnitAvatar self, UnitAvatar attacker, UnitAvatar __instance)
            {
                //Melon<Core>.Logger.Msg("RpcShowDamageParticle");
                if(color.a == 0 && color.r == 1 && color.g == 0 && color.b == 0)
                {
                    msg = "<sprite=\"Keyword\" name=Assasination>" + msg;
                    color = new Color(1, 0, 0);
                }
                else if(color.a == 0)
                {
                    //Melon<Core>.Logger.Msg("RpcShowDamageParticle blue!");
                    msg = msg.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = new Color(0.2784f, 0.5529f, 0.9804f);
                }
            }
        }
        [HarmonyPatch(typeof(UI_DamageParticle), nameof(UI_DamageParticle.SetDamage))]
        public static class UI_DamageParticlePatch
        {
            static void Prefix(ref string damage, ref Color color, UI_DamageParticle __instance)
            {
                if (color.a == 0 && color.r == 1 && color.g == 0 && color.b == 0)
                {
                    damage = "<sprite=\"Keyword\" name=Assasination>" + damage;
                    color = new Color(1, 0, 0);
                }
                else if (color.a == 0)
                {
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = new Color(0.2784f, 0.5529f, 0.9804f);
                }
            }
        }
        [HarmonyPatch]
        public class WeaponControllerSimplePatch
        {
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing), [typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixBasic(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }

            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwingFullyManual), [typeof(NewWeaponFireData), typeof(int), typeof(Vector2), typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixBasicFullyManual(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing_ManualDirection), [typeof(int), typeof(Vector2), typeof(string)])]
            [HarmonyPrefix]
            public static void PrefixBasicManualDirection(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateDashAttackSwing), [typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponControllerSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwing), [typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwingFullyManual), [typeof(NewWeaponFireData), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
        }
        [HarmonyPatch]
        public class WeaponSimplePatch
        {
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateDashAttackProjectile), [typeof(int), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(float), typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectile), [typeof(int), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponSimple __instance)
            {
                //Melon<Core>.Logger.Msg("PrefixSpecial");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectileFullyManual), [typeof(NewWeaponFireData), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int)])]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponSimple __instance)
            {
                //Melon<Core>.Logger.Msg("PrefixSpecialFullyManual");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
        }
    }
}

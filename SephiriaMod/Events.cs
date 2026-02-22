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
                Melon<Core>.Logger.Warning("CommandValue() DungeonManager.Instance is null!!");
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
                Melon<Core>.Logger.Warning("CommandValue() DungeonManager.Instance is null!!");
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
                                Melon<Core>.Logger.Warning("Error: " + message);
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
                var text = __instance.GetText();
                text.enableVertexGradient = false;
                if (color.a == 0 && color.r == 1 && color.g == 0 && color.b == 0)
                {
                    damage = "<sprite=\"Keyword\" name=Assasination>" + damage;
                    color = new Color(1, 0, 0);
                }
                else if (color == ModUtil.FourGradation)
                {
                    text.colorGradient = new VertexGradient()
                    {
                        bottomLeft = Color.white,
                        bottomRight = Color.cyan,
                        topLeft = new Color(0.2f, 0.5f, 1f),
                        topRight = new Color(1, 1, 0),
                    };
                    text.enableVertexGradient = true;
                    color = Color.white;
                }
                else if (color == ModUtil.FourGradationMagicExecution)
                {
                    text.colorGradient = new VertexGradient()
                    {
                        bottomLeft = Color.white,
                        bottomRight = Color.cyan,
                        topLeft = new Color(0.2f, 0.5f, 1f),
                        topRight = new Color(1, 1, 0),
                    };
                    text.enableVertexGradient = true;
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = Color.white;
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

        [HarmonyPatch(typeof(Charm_Basic), nameof(Charm_Basic.GetItemCategory), [])]
        public static class CharmGetItemCategoryPatch
        {
            static void Postfix(Charm_Basic __instance, ref IEnumerable<string> __result)
            {
                if (__instance is not Charm_Magic magic)
                    return;
                if(magic.NetworkAvatar != null && magic.NetworkAvatar.GetCustomStatUnsafe("AddGrimoire".ToUpperInvariant()) > 0 && !__result.Contains(ItemCategories.Grimoire))
                {
                    __result = [..__result, ItemCategories.Grimoire];
                }
            }
        }


        public static readonly string AdditionalShop = "AdditionalShop".ToUpperInvariant();
        public static readonly string AdditionalShopLegendary = "AdditionalShopLegendary".ToUpperInvariant();
        public static readonly string AdditionalShopInventory = "AdditionalShopInventory".ToUpperInvariant();
        public static readonly string AdditionalMoney = "AdditionalMoney".ToUpperInvariant();

        public static readonly string ReplenishmentCharm = "ReplenishmentCharm".ToUpperInvariant();
        public static readonly string ReplenishmentTablet = "ReplenishmentTablet".ToUpperInvariant();

        public static int DefaultAdditionalShop = 0;
        public static int DefaultAdditionalShopLegendary = 0;
        public static int DefaultAdditionalMoney = 0;

        public static int DefaultReplenishmentCharm = 0;
        public static int DefaultReplenishmentTablet = 0;

        [HarmonyPatch(typeof(UnitAI_NewBasic), nameof(UnitAI_NewBasic.SetSocialID))]
        public static class SetSocialIDPatch
        {
            static void Postfix(UnitAI_NewBasic __instance, string socialID, string nameSource, EPersonality personality, EFactionAlignment alignment, string roleName, EProceduralMerchantType merchant, int startingMoney, ItemMetadata[] startingItems)
            {
                if (merchant == EProceduralMerchantType.None)
                    return;
                System.Random random = new System.Random(__instance.Avatar.RandomID);

                int more = DefaultAdditionalShop;
                int legendary = DefaultAdditionalShopLegendary;
                int money = DefaultAdditionalMoney;
                List<int> inventory = [];
                foreach (var connection in NetworkServer.connections.Values)
                {
                    if (connection == null || connection.identity == null || connection.identity.gameObject == null)
                        return;
                    if (!connection.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                        return;
                    more += player.GetCustomStatUnsafe(AdditionalShop);
                    money += player.GetCustomStatUnsafe(AdditionalMoney);
                    legendary += player.GetCustomStatUnsafe(AdditionalShopLegendary);
                    inventory.Add(player.GetCustomStatUnsafe(AdditionalShopInventory));
                }
                if (more < 0)
                    more = 0;
                if (legendary < 0)
                    legendary = 0;

                if ((bool)DungeonManager.Instance && DungeonManager.Instance.dungeonEnvironment.TryGetValue("RedMerchant", out var value2) && value2 > 0 && __instance.Avatar.faction == "Merchant")
                {
                    return;
                }

                if (money > 0)
                {
                    __instance.Avatar.AddMoney(money);
                    Melon<Core>.Logger.Msg($"{__instance.name}: Add {money} leafs");
                }
                if(legendary > 0 && merchant != EProceduralMerchantType.VendorButNoItem)
                {
                    List<ItemMetadata> legendaries = new List<ItemMetadata>();

                    if (merchant == EProceduralMerchantType.Vendor || merchant == EProceduralMerchantType.MerchantUnionVendor || merchant == EProceduralMerchantType.SmallVendor)
                    {
                        Events.AddTradingCharms(random, legendaries, legendary, EItemRarity.Legend);
                    }
                    else if(merchant == EProceduralMerchantType.PotionVendor)
                    {
                        int potion = random.Next(0, 2) switch
                        {
                            0 => 35,
                            _ => 36,
                        };
                        legendaries.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(potion), 1));
                    }

                    if ((bool)__instance.NetworkMySafe)
                    {
                        __instance.NetworkMySafe.GenerateItemInInventory(legendaries.ToArray());
                    }
                    else if ((bool)__instance.Avatar.Inventory)
                    {
                        __instance.Avatar.Inventory.AddItems(legendaries.ToArray());
                    }

                    Melon<Core>.Logger.Msg($"{__instance.name}: Add {legendaries.Count} legendary items");
                }


                List<ItemMetadata> invlist = new List<ItemMetadata>();
                foreach (var inv in inventory)
                {
                    if (inv <= 0)
                        continue;
                    if(random.Next(0, 100) < inv)
                    {
                        invlist.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(Data.AddInventory.Id), 1));
                    }
                }

                if(invlist.Count > 0)
                {
                    if ((bool)__instance.NetworkMySafe)
                    {
                        __instance.NetworkMySafe.GenerateItemInInventory(invlist.ToArray());
                    }
                    else if ((bool)__instance.Avatar.Inventory)
                    {
                        __instance.Avatar.Inventory.AddItems(invlist.ToArray());
                    }
                }

                if (more <= 0)
                    return;

                Melon<Core>.Logger.Msg($"{__instance.name}: Add {more} items");

                List<ItemMetadata> list = new List<ItemMetadata>();
                switch (merchant)
                {
                    case EProceduralMerchantType.Vendor:
                        {
                            int charms = random.Next(0, more + 1);
                            int stoneTablets = more - charms;
                            UnitAI_NewBasic.AddTradingItems(random, list, charms, stoneTablets);
                            break;
                        }
                    case EProceduralMerchantType.MerchantUnionVendor:
                        {
                            int charms2 = random.Next(0, more + 1);
                            int stoneTablets2 = more - charms2;
                            UnitAI_NewBasic.AddTradingItems(random, list, charms2, stoneTablets2);
                            break;
                        }
                    case EProceduralMerchantType.SmallVendor:
                        {
                            int charms3 = more;
                            UnitAI_NewBasic.AddTradingItems(random, list, charms3, 0);
                            break;
                        }
                    case EProceduralMerchantType.PotionVendor:
                        {
                            for (int q = 0; q < more; q++)
                            {
                                switch (UnityEngine.Random.Range(0, 8))
                                {
                                    case 1:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(36), 1));
                                        break;
                                    case 2:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(30), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(31), 1));
                                        break;
                                    case 3:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(33), 1));
                                        break;
                                    case 4:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(28), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(29), 1));
                                        break;
                                    case 5:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(28), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(30), 1));
                                        break;
                                    case 6:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(34), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(27), 1));
                                        break;
                                    case 7:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(34), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(38), 1));
                                        break;
                                    default:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(35), 1));
                                        break;
                                }
                            }
                            break;
                        }
                    case EProceduralMerchantType.VendorButNoItem:
                        //list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(1), 1));
                        break;
                }

                if ((bool)__instance.NetworkMySafe)
                {
                    __instance.NetworkMySafe.GenerateItemInInventory(list.ToArray());
                }
                else if ((bool)__instance.Avatar.Inventory)
                {
                    __instance.Avatar.Inventory.AddItems(list.ToArray());
                }
            }
        }
        [HarmonyPatch(typeof(UnitAI_NewBasic), nameof(UnitAI_NewBasic.AddReplenishmentItemsClientside))]
        public static class AddReplenishmentItemsClientsidePatch
        {
            static void Prefix(UnitAI_NewBasic __instance, ref int charms, ref int stoneTablets)
            {
                if (NetworkClient.localPlayer == null || NetworkClient.localPlayer.gameObject == null)
                    return;
                if (!NetworkClient.localPlayer.TryGetComponent<PlayerAvatar>(out var player))
                    return;
                charms += DefaultReplenishmentCharm;
                stoneTablets += DefaultReplenishmentTablet;
                charms += player.GetCustomStatUnsafe(ReplenishmentCharm);
                stoneTablets += player.GetCustomStatUnsafe(ReplenishmentTablet);
                if (charms < 0)
                    charms = 0;
                if (stoneTablets < 0)
                    stoneTablets = 0;
            }
        }
        [HarmonyPatch(typeof(UI_ShopPanel), "UpdateReplenishmentIcon")]
        public static class UpdateReplenishmentIconPatch
        {
            static void Postfix(UI_ShopPanel __instance)
            {
                __instance.replenishmentZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 14 + __instance.replenishmentIcons.Count * 40);
            }
        }
        [HarmonyPatch(typeof(UI_ScrollToSelection), "ScrollRectToLevelSelection")]
        public static class ScrollRectToLevelSelectionPatch
        {
            static bool Prefix(UI_ScrollToSelection __instance)
            {
                var target = typeof(UI_ScrollToSelection).GetProperty("CurrentTargetRectTransform", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as RectTransform;
                if (target != null && target.gameObject.name == "ReplenishmentZone")
                    return false;
                return true;
            }
        }

        public static void AddTradingCharms(System.Random itemSeed, List<ItemMetadata> list, int charms, EItemRarity rarity)
        {
            if (charms <= 0)
                return;

            List<int> items = new List<int>();
            foreach (PlayerSpawner multiplayer in PlayerSpawner.MultiplayerList)
            {
                if (!multiplayer)
                {
                    continue;
                }

                PlayerAvatar playerAvatar = multiplayer.PlayerAvatar;
                multiplayer.GetComponent<WeaponControllerSimple>();
                foreach (int unlockedCharm in multiplayer.unlockedCharms)
                {
                    ItemEntity itemEntity = ItemDatabase.FindItemById(unlockedCharm);
                    if ((bool)itemEntity && !itemEntity.isDual)
                    {
                        Charm_Basic charm_Basic = (itemEntity.resourcePrefab ? itemEntity.resourcePrefab.GetComponent<Charm_Basic>() : null);
                        if ((bool)charm_Basic && !charm_Basic.isWeaponRelatedCharm && !itemEntity.cannotBeReward && !playerAvatar.Inventory.HasItem(itemEntity, out var _, out var _, out var _))
                        {
                            if (itemEntity.rarity == rarity)
                                items.Add(unlockedCharm);
                        }
                    }
                }
            }

            for (int i = 0; i < charms; i++)
            {
                int elementAt = items[itemSeed.Next(0, items.Count)];
                ItemEntity itemEntity2 = ItemDatabase.FindItemById(elementAt);

                if (itemEntity2 != null)
                {
                    list.Add(new ItemMetadata(itemSeed.Next(), itemEntity2, 1));
                }
            }
        }

    }
}

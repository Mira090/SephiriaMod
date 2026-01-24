using FMOD;
using HarmonyLib;
using MelonLoader;
using Mirror;
using SephiriaMod.Items;
using SephiriaMod.Registries;
using SephiriaMod.Utilities;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;

[assembly: MelonInfo(typeof(SephiriaMod.Core), "SephiriaMod", "0.7.1", "Mira", null)]
[assembly: MelonGame("TEAMHORAY", "Sephiria")]

namespace SephiriaMod
{
    public class Core : MelonMod
    {
        public static string ItemId
        {
            get
            {
                var sb = new StringBuilder();
                foreach(var line in ItemIdDic.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    sb.AppendLine(line.Replace('^', '	'));
                }
                return sb.ToString();
            }
        }
        public static string StatusId
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var status in StatusIdDic.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    var name = KeywordDatabase.Convert(status.StatusName, useColor: false, useSprite: false, true);
                    var desc = KeywordDatabase.Convert(status.StatusDescription, useColor: false, useSprite: false, true);
                    if (name == $"Status_{status.StatusName}_Name")
                        name = string.Empty;
                    if (desc == $"Status_{status.StatusDescription}_Description")
                        desc = string.Empty;
                    //name = status.StatusName;
                    //desc = status.StatusDescription;
                    var line = $"{status.id}^{name.ToNoTag()}^{desc.ToNoTag()}";
                    sb.AppendLine(line.Replace('^', '	'));
                }
                return sb.ToString();
            }
        }
        public static Dictionary<int, string> ItemIdDic = new Dictionary<int, string>();
        public static Dictionary<string, StatusEntity> StatusIdDic = new Dictionary<string, StatusEntity>();
        /// <summary>
        /// Melonが登録された後に呼び出されます。このコールバックはMelonLoaderが完全に初期化されるまで待機します。このコールバック以降は、ゲーム/Unity の参照を安全に行うことができます。 
        /// </summary>
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
        /// <summary>
        /// OnInitializeMelonの後に呼び出されます。このコールバックは、Unity が最初の「Start」メッセージを呼び出すまで待機します。
        /// </summary>
        public override void OnLateInitializeMelon()
        {
            CustomSpriteAsset.InitSprites();

            Data.Init();
            LoggerInstance.Msg("LateInitialized.");
            /*
            var guid = new FMOD.GUID();
            guid.Data1 = -1656330595;
            guid.Data2 = 1271005578;
            guid.Data3 = -200955986;
            guid.Data4 = -1225937978;
            Melon<Core>.Logger.Msg("Heal Sound: " + guid.GUIDToPath());*/

        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if(sceneName == "MainWorld")
            {
                CustomSpriteAsset.InitSpriteAsset();
            }
        }
        [HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), new Type[] { typeof(string), typeof(Type) })]
        class ResourcesLoadAllPatch
        {
            private static void ModifyItemEntity(ItemEntity item)
            {
                if (item.name.Contains("3022_MagicCharm_MeteorShower"))
                {
                    var prefab = item.resourcePrefab;
                    if (prefab.TryGetComponent<Charm_Magic>(out var charm))
                    {
                        if(charm.skill.magicPrefab.TryGetComponent<ActiveSkill_MeteorShower>(out var skill))
                        {
                            //skill.meteorFireTimer = new Timer(0.25f);
                            //skill.numberOfMeteorsByLevel = new int[3] { 24, 30, 36 };
                            skill.meteorFireTimer = new Timer(0.05f);
                            skill.numberOfMeteorsByLevel = new int[9] { 20, 25, 30, 35, 40, 45, 50, 60, 100 };//3 Level => DPS 1750（number100, Fire 51）
                            skill.damagePercentByLevel = new float[6] { 80f, 90f, 100f, 110f, 120f, 130f};
                            charm.skill.cooldownTime = 31f;//26 => ??
                            charm.skill.mpCostsByLevel = new int[9] { 23, 33, 37, 46, 51, 57, 66, 82, 102 };//35, 38, 41, 44
                            charm.maxLevel = 8;
                        }
                    }
                }
                if (item.name.Contains("1039_AutoMagic"))
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if (item.name.Contains("1123_FinalHP") || item.name.Contains("1124_FinalMP"))
                {
                    item.categories = new List<string> { ItemCategories.Lake, ItemCategories.Vitality };
                }
                if (item.name.Contains("1196_TouchOfLiving") || item.name.Contains("1158_EnhancementPotionCap") || item.name.Contains("1005_HeartShapedCarrot"))
                {
                    item.categories = new List<string> { ItemCategories.Vitality };
                }
                if (item.name.Contains("1017_ShieldEarring"))
                {
                    item.categories = new List<string> { ItemCategories.Guardian, ItemCategories.Vitality };
                }
                //if (item.isDual)
                    //Melon<Core>.Logger.Msg(item.aName.ToString() + "(" + item.name + ")");
                if(item.name == "1255_PlanetFire" || item.name == "1256_PlanetIce" || item.name == "1257_PlanetLightning")
                {
                    //item.name == "1254_PlanetComet"
                    //item.activeType = EItemActiveType.Default;
                }
                if (item.name.Contains("1166_MagicCritical") || item.name.Contains("1169_MagicDamageBySpeed") || item.name.Contains("1075_GoldIsDamage") || item.name.Contains("1165_RylieWatch"))
                {
                    var prefab = item.resourcePrefab;
                    if (prefab.TryGetComponent<Charm_Basic>(out var charm))
                    {
                        charm.isUniqueEffect = false;
                    }
                }
                if(item.name == "1117_TuningFork")
                {
                    /*
                    if(item.resourcePrefab.TryGetComponent<Charm_TuningForks>(out var charm))
                    {
                        charm.effectsString = [new LocalizedString("Charm_TuningForksMkII_Effect"), new LocalizedString("Charm_TuningForksMkII_Effect2")];
                        var component = item.resourcePrefab.AddComponent<Charm_TuningForksMkII>();
                        component.effectsString = charm.effectsString;
                        component.damageColor = charm.damageColor;
                        component.valueHUD_ID = charm.valueHUD_ID;
                        component.maxLevel = charm.maxLevel;
                        component.isUniqueEffect = charm.isUniqueEffect;
                        component.relatedWeapon = charm.relatedWeapon;
                        UnityEngine.Object.Destroy(charm);
                    }*/
                    //item.isDual = false;
                }
                if (item.name == "1076_Jeongseok")//魔法の定法
                {
                    /*
                    if (item.resourcePrefab.TryGetComponent<Charm_StatusInstance>(out var charm))
                    {
                        charm.stats[2] = Data.CreateStatusGroup("FIRE_DAMAGE", 1, 2, 4, 7, 10);
                    }*/
                }
                if (item.id == 1235)//突き指南書
                {
                    item.categories = [ItemCategories.Sturdy, ItemCategories.SkySong];
                }
                if (item.id == 1252)//常緑のマント
                {
                    item.categories = [ItemCategories.Shadow, ItemCategories.SkySong];
                }
                if (item.id == 1149)//金色のマント
                {
                    item.categories = [ItemCategories.WindSong, ItemCategories.SkySong];
                }
                if (item.id == 1082 || item.id == 1011 || item.id == 1093)//圧迫マント 風草のスカーフ いばらの茂み
                {
                    item.categories = [ItemCategories.SkySong];
                }
                if (item.id == 1172)//パラスのカード
                {
                    item.categories = [ItemCategories.Fortune];
                }
                if(item.id == 1262)//神秘の振り子
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if(item.id == 1265 || item.id == 1266 || item.id == 1274 || item.id == 1275 || item.id == 1276)//MPShield、MagicMP、フォールトファインダーニードル、さすらいの人の首飾り、獣の心臓
                {
                    //item.activeType = EItemActiveType.Default;
                }

                if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_Basic>(out var c))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? "(絆)" : "")}^{c.GetType().Name}";
                }
                else if(item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<StoneTablet>(out var t))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? "(絆)" : "")}^{t.GetType().Name}";
                }
                else if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<PotionEffect>(out var p))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? "(絆)" : "")}^{p.GetType().Name}";
                }
                else
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? "(絆)" : "")}^{(item.resourcePrefab == null ? "プレハブ無し" : "プレハブ有り")}";
                }
            }
            private static void ModifyItemCategoryEntity(ItemCategoryEntity category)
            {

            }
            private static void ModifyStatusEntity(StatusEntity status)
            {
                if(status.id == "HP_POTION_BONUS")
                {
                    status.divideForDisplay = 1;
                }
                StatusIdDic[status.id] = status;
            }
            private static void ModifyMiracle(Miracle miracle)
            {
                if(miracle.id == "Scholar")
                {
                    miracle.categories = [ItemCategories.Lake];
                }
                if(miracle.id == "IntelligenceAgent")
                {
                    miracle.categories = [ItemCategories.SkySong];
                }
                if(miracle.id == "Guard")
                {
                    miracle.categories = [ItemCategories.Vitality];
                }
                if (miracle.id == "Berserker")
                {
                    miracle.categories = [ItemCategories.Drunk];
                }
            }
            private static void ModifyKeywordEntity(KeywordEntity keyword)
            {
                if(keyword.keyword == "Toughness")
                {
                    keyword.displayDetails = true;
                }
            }
            static void Postfix(string path, Type systemTypeInstance, ref UnityEngine.Object[] __result)
            {
                //Melon<Core>.Logger.Msg("Postfix: (" + systemTypeInstance.ToString() + ") " + path);
                if (systemTypeInstance == typeof(ItemEntity) && path == "Item")
                {
                    var list = __result.ToList();

                    Data.Register(list);

                    foreach (var item in list)
                    {
                        if (item is ItemEntity entity)
                            ModifyItemEntity(entity);
                    }

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(ItemCategoryEntity) && path == "ItemCategory")
                {
                    var list = __result.ToList();


                    foreach (var item in list)
                    {
                        if (item is ItemCategoryEntity entity)
                        {
                            if (Data.DefaultEnableSound.IsNull)
                            {
                                Data.DefaultEnableSound = entity.enableSound;
                            }
                        }
                    }

                    Data.RegisterCombos(list);

                    foreach (var item in list)
                        if (item is ItemCategoryEntity entity)
                            ModifyItemCategoryEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(EffectHUDEntity) && path == "EffectHUD")
                {
                    var list = __result.ToList();

                    Data.RegisterEffectHUDs(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(DamageIdEntity) && path == "DamageId")
                {
                    //一回
                    var list = __result.ToList();

                    Data.RegisterDamageIds(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(StatusEntity) && path == "Status")
                {
                    //複数回
                    var list = __result.ToList();

                    Data.RegisterStatuses(list);

                    foreach (var item in list)
                        if (item is StatusEntity entity)
                            ModifyStatusEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(KeywordEntity) && path == "Keyword")
                {
                    var list = __result.ToList();

                    var magicExecution = ScriptableObject.CreateInstance<KeywordEntity>();
                    magicExecution.name = "MagicExecution";
                    magicExecution.keyword = "MagicExecution";
                    magicExecution.visualText = new LocalizedString("Status_MagicExecution_Name");
                    magicExecution.description = new LocalizedString("Status_MagicExecution_Description");
                    magicExecution.detailedValue = new LocalizedString();
                    magicExecution.displayDetails = true;
                    foreach (var item in list)
                        if (item is KeywordEntity entity)
                        {
                            if(entity.keyword == "MagicDamageBonus")
                            {
                                magicExecution.keywordImage = entity.keywordImage;
                                magicExecution.textColor = entity.textColor;
                            }
                            else if(entity.keyword == "Elemental_Chaos")
                            {
                                magicExecution.connectedDetailEntities = [entity];
                            }
                        }

                    Melon<Core>.Logger.Msg("New Keyword: " + magicExecution.visualText.ToString());
                    list.Add(magicExecution);

                    var stargazeLevel = ScriptableObject.CreateInstance<KeywordEntity>();
                    stargazeLevel.name = "StargazeLevel";
                    stargazeLevel.keyword = "StargazeLevel";
                    stargazeLevel.visualText = new LocalizedString("Status_StargazeLevel_Name");
                    stargazeLevel.description = new LocalizedString("Status_StargazeLevel_Description");
                    stargazeLevel.detailedValue = new LocalizedString();
                    stargazeLevel.displayDetails = true;
                    stargazeLevel.textColor = Color.white;

                    Melon<Core>.Logger.Msg("New Keyword: " + stargazeLevel.visualText.ToString());
                    list.Add(stargazeLevel);

                    var invLevel = ScriptableObject.CreateInstance<KeywordEntity>();
                    invLevel.name = "InvLevel";
                    invLevel.keyword = "InvLevel";
                    invLevel.visualText = new LocalizedString("Status_InvLevel_Name");
                    invLevel.description = new LocalizedString("Status_InvLevel_Description");
                    invLevel.detailedValue = new LocalizedString();
                    invLevel.displayDetails = true;
                    invLevel.textColor = Color.white;
                    Melon<Core>.Logger.Msg("New Keyword: " + invLevel.visualText.ToString());
                    list.Add(invLevel);

                    var binaryPlanet = ScriptableObject.CreateInstance<KeywordEntity>();
                    binaryPlanet.name = "BinaryPlanet";
                    binaryPlanet.keyword = "BinaryPlanet";
                    binaryPlanet.visualText = new LocalizedString("Status_BinaryPlanet_Name");
                    binaryPlanet.description = new LocalizedString("Status_BinaryPlanet_Description");
                    binaryPlanet.detailedValue = new LocalizedString();
                    binaryPlanet.displayDetails = true;
                    binaryPlanet.textColor = new Color(0.7f, 0.4f, 0.1f);
                    binaryPlanet.keywordImage = CustomSpriteAsset.BinaryPlanet;
                    Melon<Core>.Logger.Msg("New Keyword: " + binaryPlanet.visualText.ToString());
                    list.Add(binaryPlanet);

                    var assasination = ScriptableObject.CreateInstance<KeywordEntity>();
                    assasination.name = "Assasination";
                    assasination.keyword = "Assasination";
                    assasination.visualText = new LocalizedString("Status_Assasination_Name");
                    assasination.description = new LocalizedString("Status_Assasination_Description");
                    assasination.detailedValue = new LocalizedString();
                    assasination.displayDetails = true;
                    assasination.textColor = new Color(0.9f, 0.1f, 0.1f);
                    assasination.keywordImage = CustomSpriteAsset.Assasination;
                    Melon<Core>.Logger.Msg("New Keyword: " + assasination.visualText.ToString());
                    list.Add(assasination);

                    foreach (var item in list)
                        if (item is KeywordEntity entity)
                            ModifyKeywordEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(CostumeEntity) && path == "Costume")
                {
                    var list = __result.ToList();

                    Data.RegisterCostume(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(CostumeSkinEntity) && path == "CostumeSkin")
                {
                    var list = __result.ToList();

                    Data.RegisterCostumeSkin(list);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(GameObject) && path == "Miracle")
                {
                    var list = __result.ToList();

                    Data.RegisterMiracles(list);

                    foreach (var item in list)
                        if (item is GameObject entity && entity.TryGetComponent<Miracle>(out var miracle))
                            ModifyMiracle(miracle);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(WeaponEntity) && path == "Weapon")
                {
                    var list = __result.ToList();

                    Data.RegisterWeapons(list);

                    __result = list.ToArray();
                }
            }
        }
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.GetItemDropWeight), new Type[] { typeof(ItemEntity) })]
        public static class GridInventoryGetItemDropWeightPatch
        {
            static void Postfix(ItemEntity entity, ref int __result, ref GridInventory __instance)
            {
                int bonus = 0;
                if (__instance.itemDropBonusBySemantic.TryGetValue("TABLET", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                    __result += bonus;
                }
                if (__instance.itemDropBonusBySemantic.TryGetValue("DISCONNECT", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    if(entity.id == 2049)//断絶
                    {
                        //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                        __result += bonus;
                    }
                }
                if (__instance.itemDropBonusBySemantic.TryGetValue("NO_DISCONNECT", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    if (entity.id != 2049)//断絶
                    {
                        //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                        __result += bonus;
                    }
                }
            }
        }
        /// <summary>
        /// アイテムのInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SetItem))]
        public static class InstantiateStoneTabletPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"InstantiateStoneTabletPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateStoneTabletPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Melon<Core>.Logger.Msg($"CustomInstantiate: {original.name}");
                foreach (var modItem in Data.All)
                {
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if(ob.TryGetComponent<Charm_Basic>(out var charm))
                        {
                            charm.enabled = true;
                        }
                        if (ob.TryGetComponent<StoneTablet>(out var tablet))
                        {
                            tablet.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// コンボ効果（カテゴリー）のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SearchSetEffectInInventory))]
        public static class InstantiateItemCategoryPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"InstantiateItemCategoryPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateItemCategoryPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                Melon<Core>.Logger.Msg($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Combos)
                {
                    //Melon<Core>.Logger.Msg($"A: {modItem.ResourcePrefab.name}");
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        Melon<Core>.Logger.Msg($"Bypassing Instantiate for2: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<ComboEffectBase>(out var combo))
                        {
                            combo.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// 奇跡のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(MiracleController), "LocalAddMiracle")]
        public static class InstantiateMiraclePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"InstantiateMiraclePatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateMiraclePatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static Miracle CustomInstantiate(Miracle original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                Melon<Core>.Logger.Msg($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Miracles)
                {
                    //Melon<Core>.Logger.Msg($"A: {modItem.ResourcePrefab.name}");
                    if (original.gameObject.name == modItem.Prefab.name)
                    {
                        Melon<Core>.Logger.Msg($"Bypassing Instantiate for3: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        [HarmonyPatch(typeof(WeaponControllerSimple), "LocalEquipWeapon")]
        public static class InstantiateWeaponPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"InstantiateWeaponPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateWeaponPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                Melon<Core>.Logger.Msg($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Weapons)
                {
                    //Melon<Core>.Logger.Msg($"A: {modItem.ResourcePrefab.name}");
                    if (original.name == modItem.MainWeaponPrefab.name)
                    {
                        Melon<Core>.Logger.Msg($"Bypassing Instantiate for3: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        [HarmonyPatch(typeof(NetworkClient), "SpawnPrefab")]
        public static class InstantiateNetworkClientPatch
        {
            public static event Func<uint, GameObject> OnGetPrefab;
            public static event Func<GameObject, Vector3, Quaternion, GameObject> OnInstantiate;
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"InstantiateNetworkClientPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject), typeof(Vector3), typeof(Quaternion) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateNetworkClientPatch), nameof(CustomInstantiate));
                var replacement2 = AccessTools.Method(typeof(InstantiateNetworkClientPatch), nameof(CustomGetPrefab));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        Melon<Core>.Logger.Msg($"Transpiler?");
                        code.operand = replacement;
                    }
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi2 && mi2.Name == nameof(NetworkClient.GetPrefab))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        Melon<Core>.Logger.Msg($"Transpiler!");
                        code.operand = replacement2;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original, Vector3 position, Quaternion rotation)
            {
                if(original == null)
                    return UnityEngine.Object.Instantiate(original, position, rotation);

                var o = OnInstantiate?.Invoke(original, position, rotation);
                if (o != null)
                {
                    //Melon<Core>.Logger.Msg($"CustomInstantiate: {o.name}");
                    return o;
                }
                //Melon<Core>.Logger.Msg($"CustomInstantiate: {original.name}");
                foreach(var modItem in Data.All)
                {
                    if(original.name == modItem.ResourcePrefab.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<Charm_Basic>(out var charm))
                        {
                            charm.enabled = true;
                        }
                        if (ob.TryGetComponent<StoneTablet>(out var tablet))
                        {
                            tablet.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Combos)
                {
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<ComboEffectBase>(out var combo))
                        {
                            combo.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Miracles)
                {
                    if (original.name == modItem.Prefab.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<Miracle>(out var miracle))
                        {
                            miracle.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Weapons)
                {
                    if (modItem.WeaponEntity == null)
                        continue;
                    if (original.name == modItem.MainWeaponPrefab.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        //if (ob.TryGetComponent<Miracle>(out var miracle))
                        {
                            //miracle.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Buffs)
                {
                    if (original.name == modItem.name)
                    {
                        //Melon<Core>.Logger.Msg($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original, position, rotation);
            }

            // 差し替え用メソッド
            public static bool CustomGetPrefab(uint assetId, out GameObject prefab)
            {
                //Melon<Core>.Logger.Msg($"CustomGetPrefab: {assetId}");
                var on = OnGetPrefab?.Invoke(assetId);
                if(on != null)
                {
                    prefab = on;
                    return true;
                }

                if (assetId != 0 && !NetworkClient.prefabs.TryGetValue(assetId, out _))
                {
                    foreach(var modItem in Data.All)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Melon<Core>.Logger.Msg($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.ResourcePrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Combos)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Melon<Core>.Logger.Msg($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.ResourcePrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Miracles)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Melon<Core>.Logger.Msg($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.Prefab;
                        return true;
                    }
                    foreach (var modItem in Data.Weapons)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Melon<Core>.Logger.Msg($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.MainWeaponPrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Buffs)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Melon<Core>.Logger.Msg($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.gameObject;
                        return true;
                    }
                }

                    // 通常の GetPrefab
                return NetworkClient.GetPrefab(assetId, out prefab);
            }
        }
        [HarmonyPatch(typeof(GameDataLoader), "Awake")]
        public static class GameDataLoaderAwakePatch
        {
            static void Postfix(GameDataLoader __instance)
            {
                if (__instance == null || __instance != ModUtil.GetGameDataLoader())
                    return;
                Melon<Core>.Logger.Msg("Mod GameData Loading...");
                //CustomCostumeDatabase.Initialize();
                CustomCostumeDatabase.LoadAllStartingItems(CostumeDatabase.GetAll());
            }
        }
        [HarmonyPatch(typeof(GameDataLoader), "OnDestroy")]
        public static class GameDataLoaderOnDestroyPatch
        {
            static void Prefix(GameDataLoader __instance)
            {
                if (__instance == null || __instance != ModUtil.GetGameDataLoader())
                    return;
                Melon<Core>.Logger.Msg("Mod GameData Destroying...");
                //CustomCostumeDatabase.Destroy();
            }
        }
        //[HarmonyPatch(typeof(PlayerAvatar), "UpdateCostumeOutfit", [typeof(string)])]
        [Obsolete("CustomCustomeEntityのデバッグ用Patch")]
        public static class PlayerAvatarUpdateCostumeOutfit
        {
            static bool Prefix(string skinID, PlayerAvatar __instance)
            {
                Melon<Core>.Logger.Msg("UpdateCostumeOutfit Pre: " + skinID);

                if (!__instance.TopdownActor.body)
                {
                    return true;
                }
                if (__instance.GetCurrentCostumeObject())
                {
                    __instance.GetCurrentCostumeObject().Unequip(__instance.isServer);
                    Animator2D_Basic component = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                    if (component)
                    {
                        component.SetAnimationSpeed("MOVE", null);
                        component.SetAnimationSpeed("MOVE_BACK", null);
                        component.SetAnimationSpeed("GREATSWORDSWING_LOWER", null);
                        component.SetAnimationSpeed("GREATSWORDSWING_UPPER", null);
                    }
                }


                CustomCostumeEntity costumeSkinEntity = CustomCostumeDatabase.FindCostumeByID(skinID);

                Melon<Core>.Logger.Msg("UpdateCostumeOutfit: " + skinID);
                if (costumeSkinEntity == null)
                    return true;
                Melon<Core>.Logger.Msg("UpdateCostumeOutfit New: " + costumeSkinEntity.costumeName);
                PlayerAvatarCostume component2 = costumeSkinEntity.costumePrefab.GetComponent<PlayerAvatarCostume>();
                __instance.SetCurrentCostumeObject(UnityEngine.Object.Instantiate<PlayerAvatarCostume>(component2, __instance.TopdownActor.body));
                __instance.GetCurrentCostumeObject().transform.localPosition = new Vector3(0, 0.4f, 0);
                __instance.GetCurrentCostumeObject().transform.localEulerAngles = Vector3.zero;
                if (__instance.GetCurrentCostumeObject().waterReflectionObject)
                {
                    __instance.GetCurrentCostumeObject().waterReflectionObject.SetParent(__instance.TopdownActor.bodyWrapper.transform.parent);
                    __instance.GetCurrentCostumeObject().waterReflectionObject.GetComponent<SyncLocalPosition>().syncTarget = __instance.TopdownActor.body.transform;
                }
                __instance.GetCurrentCostumeObject().Equip(__instance, __instance.isServer);
                __instance.moveFxPrefab = __instance.GetCurrentCostumeObject().moveFxPrefab;
                __instance.GetCurrentCostumeObject().hitbox.combatBehaviour = __instance;
                __instance.GetWeaponController().SetShoulderPositionScale(__instance.GetCurrentCostumeObject().bodyScale);
                __instance.stencilSolidColor = __instance.GetCurrentCostumeObject().stencilSolid;
                __instance.TopdownActor.bodyRenderer = __instance.GetCurrentCostumeObject().GetComponent<SpriteRenderer>();
                __instance.TopdownActor.animator = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                if (__instance.TopdownActor.animator)
                {
                    __instance.TopdownActor.animator.SetTransition(__instance.SetAnimationTransition(true, true));//(costumeSkinEntity.canLookBack, costumeSkinEntity.containsAttackAnimation)
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE_BACK", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_LOWER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_UPPER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                }
                __instance.TopdownRigidbody.MovementCollider.radius = __instance.GetCurrentCostumeObject().movementColliderRadius;
                return false;
            }
            static void P(string skinID, PlayerAvatar __instance)
            {
                CustomCostumeEntity costumeSkinEntity = CustomCostumeDatabase.FindCostumeByID(skinID);

                Melon<Core>.Logger.Msg("UpdateCostumeOutfit: " + skinID);
                if (costumeSkinEntity == null)
                    return;
                Melon<Core>.Logger.Msg("UpdateCostumeOutfit New: " + costumeSkinEntity.costumeName);
                PlayerAvatarCostume component2 = costumeSkinEntity.costumePrefab.GetComponent<PlayerAvatarCostume>();
                __instance.SetCurrentCostumeObject(UnityEngine.Object.Instantiate<PlayerAvatarCostume>(component2, __instance.TopdownActor.body));
                __instance.GetCurrentCostumeObject().transform.localPosition = Vector3.zero;
                __instance.GetCurrentCostumeObject().transform.localEulerAngles = Vector3.zero;
                if (__instance.GetCurrentCostumeObject().waterReflectionObject)
                {
                    __instance.GetCurrentCostumeObject().waterReflectionObject.SetParent(__instance.TopdownActor.bodyWrapper.transform.parent);
                    __instance.GetCurrentCostumeObject().waterReflectionObject.GetComponent<SyncLocalPosition>().syncTarget = __instance.TopdownActor.body.transform;
                }
                __instance.GetCurrentCostumeObject().Equip(__instance, __instance.isServer);
                __instance.moveFxPrefab = __instance.GetCurrentCostumeObject().moveFxPrefab;
                __instance.GetCurrentCostumeObject().hitbox.combatBehaviour = __instance;
                __instance.GetWeaponController().SetShoulderPositionScale(__instance.GetCurrentCostumeObject().bodyScale);
                __instance.stencilSolidColor = __instance.GetCurrentCostumeObject().stencilSolid;
                __instance.TopdownActor.bodyRenderer = __instance.GetCurrentCostumeObject().GetComponent<SpriteRenderer>();
                __instance.TopdownActor.animator = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                if (__instance.TopdownActor.animator)
                {
                    __instance.TopdownActor.animator.SetTransition(__instance.SetAnimationTransition(false, false));//(costumeSkinEntity.canLookBack, costumeSkinEntity.containsAttackAnimation)
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE_BACK", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_LOWER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_UPPER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                }
                __instance.TopdownRigidbody.MovementCollider.radius = __instance.GetCurrentCostumeObject().movementColliderRadius;
            }
        }
    }
}
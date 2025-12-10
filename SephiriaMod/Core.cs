using FMOD;
using HarmonyLib;
using MelonLoader;
using Mirror;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using TMPro;
using System.Text;
using SephiriaMod.Items;
using SephiriaMod.Utilities;
using SephiriaMod.Registries;

[assembly: MelonInfo(typeof(SephiriaMod.Core), "SephiriaMod", "0.6.4", "Mira", null)]
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
        public static Dictionary<int, string> ItemIdDic = new Dictionary<int, string>();
        public Sprite NewKeywordSprite;
        /// <summary>
        /// Melonが登録された後に呼び出されます。このコールバックはMelonLoaderが完全に初期化されるまで待機します。このコールバック以降は、ゲーム/Unity の参照を安全に行うことができます。 
        /// </summary>
        public override void OnInitializeMelon()
        {
            // T は対象の ScriptableObject クラス
            //SephiriteSpawner a;
            //PassiveObject_StartingItem
            LoggerInstance.Msg("Initialized.");
        }
        /// <summary>
        /// OnInitializeMelonの後に呼び出されます。このコールバックは、Unity が最初の「Start」メッセージを呼び出すまで待機します。
        /// </summary>
        public override void OnLateInitializeMelon()
        {
            KeywordImages.InitSprites();
            //NetworkManager.singleton.spawnPrefabs.Add(ResourcesLoadAllPatch.StoneTabletMalice);
            //NetworkManager.singleton.spawnPrefabs.Add(ResourcesLoadAllPatch.CharmPurgatory);
            //NetworkManager.singleton.spawnPrefabs.Add(ResourcesLoadAllPatch.CharmReservedMPEvasion);
            //ResourcesLoadAllPatch.StoneTabletMalice.SetActive(false);
            //var a2 = typeof(NetworkIdentity).GetField("hasSpawned");
            //a2.SetValue(ResourcesLoadAllPatch.StoneTabletMalice.GetComponent<NetworkIdentity>(), false);
            Data.Init();
            LoggerInstance.Msg("LateInitialized.");
            //ModEvent.Charm_TuningForksPatch.Init();
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
                //return;
                //TMP_SpriteAsset keywords = TMP_Settings.defaultSpriteAsset;
                //foreach (var keyword in keywords.spriteCharacterTable)
                //LoggerInstance.Msg($"keyword[{keyword.name}]: {keyword.glyph.glyphRect.x}, {keyword.glyph.glyphRect.y}");
                //keywords.spriteSheet = NewKeywordSprite.texture;
                //keywords.UpdateLookupTables();

                KeywordImages.InitSpriteAsset();
            }
        }
        [HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), new Type[] { typeof(string), typeof(Type) })]
        class ResourcesLoadAllPatch
        {
            //public static GameObject StoneTabletMalice = CreateStoneTablet("Malice", "O 1\nUP -1\nDOWN -1\nLEFT -1\nRIGHT -1");
            //public static GameObject CharmPurgatory = CreateCharmStatus("Purgatory", 3, CreateStatusGroup("DARK_CLOUD_RESTORE_DURING_BATTLE", 2, 5, 8), CreateStatusGroup("BURN_STACK", 0, 1, 1));
            //public static GameObject CharmReservedMPEvasion = CreateCharmStatus<Charm_ReservedMPEvasion>("Reserved_MP_Evasion", 4, new LocalizedString("Item_Charm_Reserved_MP_Evasion_Effect"), CreateStatusGroup("EVASION", 200, 300, 400, 500, 700), CreateStatusGroup("PHYSICAL_DAMAGE", 0, 0, 1, 2, 3));

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
                /*
                if (item.name.Contains("1193_BurnExplosion"))
                {
                    var prefab = item.resourcePrefab;
                    if (prefab.TryGetComponent<Charm_BurnExplosion>(out var charm))
                    {
                        charm.explodeRadius = 5f;//3 =>
                        charm.damageByLevel = new int[6] { 7, 16, 25, 34, 43, 52};//7の倍数 => 
                        //charm.explosionFxPrefab = UnityEngine.Object.Instantiate(charm.explosionFxPrefab);
                        //charm.explosionFxPrefab.transform.localScale = Vector3.one * 2;
                    }
                }*/
                if (item.name.Contains("1185_Burn") || item.name.Contains("1213_MagmaBead") || item.name.Contains("1128_FlameBall") || item.name.Contains("1153_FlamePlantRoot"))
                {
                    //item.categories.Clear();
                    //item.categories = new List<string> { ItemCategories.Purgatory };
                }
                if (item.name.Contains("1214_FireFeather") || item.name.Contains("1018_WarmStone") || item.name.Contains("1193_BurnExplosion"))
                {
                    //item.categories = new List<string> { ItemCategories.Ember, ItemCategories.Purgatory };
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
                    item.activeType = EItemActiveType.Default;
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
                    }
                    //item.isDual = false;
                }
                if(item.name == "1076_Jeongseok")//魔法の定法
                {
                    if (item.resourcePrefab.TryGetComponent<Charm_StatusInstance>(out var charm))
                    {
                        charm.stats[2] = Data.CreateStatusGroup("FIRE_DAMAGE", 1, 2, 4, 7, 10);
                    }
                }
                if(item.id == 1235)//突き指南書
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
                if(item.id == 1262)
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if(item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_Basic>(out var c))
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
                /*
                if(category.id == ItemCategories.Precision)
                {
                    category.setStatus = new ItemCategoryEntity.SetTarget[5] {
                new ItemCategoryEntity.SetTarget() { itemCount = 2, status = "CRITICAL/400" },
                new ItemCategoryEntity.SetTarget() { itemCount = 3, status = "CRITICAL/800" },
                new ItemCategoryEntity.SetTarget() { itemCount = 4, status = "CRITICAL/1200" },
                new ItemCategoryEntity.SetTarget() { itemCount = 5, status = "CRITICAL/1600" },
                new ItemCategoryEntity.SetTarget() { itemCount = 6, status = "CRITICAL/2000" }};
                }*/
            }
            private static void ModifyStatusEntity(StatusEntity status)
            {
                if(status.id == "HP_POTION_BONUS")
                {
                    status.divideForDisplay = 1;
                }
                //Melon<Core>.Logger.Msg($"{new LocalizedString("Status_" + status.StatusName + "_Name")}({status.id}): {new LocalizedString("Status_" + status.StatusDescription + "_Description")}");
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
                // 例えば GameObject プレハブを追加したい場合
                if (systemTypeInstance == typeof(ItemEntity) && path == "Item")
                {
                    var list = __result.ToList();

                    Data.Register(list);
                    /*
                    var riteki = ScriptableObject.CreateInstance<ItemEntity>();
                    riteki.name = "Item_StoneTablet_Malice";
                    riteki.activeType = EItemActiveType.Default;
                    riteki.aName = new LocalizedString("Item_StoneTablet_Malice_Name");
                    //riteki.aFlavorText = null;
                    riteki.categories = new();
                    riteki.cost = 800;
                    riteki.id = 8000;
                    riteki.itemBehaviour = ItemEntity.EItemBehaviour.None;
                    riteki.rarity = EItemRarity.Common;
                    riteki.type = EItemType.StoneTablet;
                    riteki.sapphirePrice = 4;
                    //riteki.resourcePrefab = StoneTabletMalice;
                    riteki.iconInWorld = IconInWorldTablet;

                    //list.Add(riteki);

                    var purgatory = ScriptableObject.CreateInstance<ItemEntity>();
                    purgatory.name = "Item_Charm_Purgatory";
                    purgatory.activeType = EItemActiveType.Default;
                    purgatory.aName = new LocalizedString("Item_Charm_Purgatory_Name");
                    purgatory.aFlavorText = new LocalizedString("Item_Charm_Purgatory_FlavorText");
                    purgatory.categories = new() { "DARKCLOUD", "PURGATORY" };
                    purgatory.cost = 600;
                    purgatory.id = 8001;
                    purgatory.itemBehaviour = ItemEntity.EItemBehaviour.None;
                    purgatory.rarity = EItemRarity.Uncommon;
                    purgatory.type = EItemType.Charm;
                    purgatory.sapphirePrice = 3;
                    //purgatory.resourcePrefab = CharmPurgatory;
                    purgatory.icon = SpriteLoader.LoadSprite("Charm_Purgatory");
                    purgatory.iconInWorld = IconInWorldCharm;

                    //list.Add(purgatory);
                    //list.Add(CreateCharmItemEntity("Reserved_MP_Evasion", ["STURDY", "SHADOW"], EItemRarity.Uncommon, CharmReservedMPEvasion));*/

                    foreach (var item in list)
                    {
                        //Melon<Core>.Logger.Msg("tablet: " + item.ToString());
                        /*
                        if(item.ToString().Contains("Foundation"))
                        {
                            //Melon<Core>.Logger.Msg("contains");
                            if (item is ItemEntity entity)
                            {
                                //Melon<Core>.Logger.Msg("entity");
                                if (entity.resourcePrefab != null && entity.resourcePrefab.TryGetComponent<StoneTablet>(out StoneTablet t2))
                                {
                                    //riteki.resourcePrefab = GameObject.Instantiate(entity.resourcePrefab);
                                    if (!NetworkManager.singleton.spawnPrefabs.Contains(riteki.resourcePrefab))
                                    {
                                        //NetworkManager.singleton.spawnPrefabs.Add(riteki.resourcePrefab);
                                        //NetworkServer.Spawn(riteki.resourcePrefab);
                                    }
                                    //riteki.resourcePrefab.name = "StoneTablet-Malice";
                                    //Melon<Core>.Logger.Msg("inventory: " + t2);
                                    if (riteki.resourcePrefab.TryGetComponent<StoneTablet>(out StoneTablet t))
                                    {
                                        //t.query = "O 1\nUP -1\nDOWN -1\nLEFT -1\nRIGHT -1";
                                        //Melon<Core>.Logger.Msg("not null: " + t2.Inventory != null);
                                        //Melon<Core>.Logger.Msg(t2.Inventory.ToString());
                                        //tablet.Connect(t2.Inventory);
                                    }
                                    if(riteki.resourcePrefab.TryGetComponent(out NetworkIdentity identity))
                                    {
                                        //Melon<Core>.Logger.Msg("id1: " + identity.assetId);
                                        //var a = typeof(NetworkIdentity).GetProperty(nameof(identity.assetId));
                                        //uint creatureAssetId = NetworkIdentity.AssetGuidToUint(Malice);
                                        //a.SetValue(identity, creatureAssetId);

                                        //var a2 = typeof(NetworkIdentity).GetField("hasSpawned");
                                        //a2.SetValue(identity, false);
                                    }
                                    if (entity.resourcePrefab.TryGetComponent(out NetworkIdentity i2))
                                    {
                                        //Melon<Core>.Logger.Msg("id2: " + i2.assetId);
                                    }
                                }
                            }
                            //tablet.SetDirty();
                        }*/
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
                    /*
                    var burnSpeed = ScriptableObject.CreateInstance<ItemCategoryEntity>();
                    burnSpeed.id = "PURGATORY";
                    burnSpeed.name = "Purgatory";
                    burnSpeed.categoryName = new LocalizedString("ItemCategory_Purgatory");
                    burnSpeed.setStatus = new ItemCategoryEntity.SetTarget[5] {
                new ItemCategoryEntity.SetTarget() { itemCount = 2, status = "BURN_SPEED/10" },
                new ItemCategoryEntity.SetTarget() { itemCount = 3, status = "BURN_SPEED/20" },
                new ItemCategoryEntity.SetTarget() { itemCount = 4, status = "BURN_SPEED/30" },
                new ItemCategoryEntity.SetTarget() { itemCount = 5, status = "BURN_SPEED/40" },
                new ItemCategoryEntity.SetTarget() { itemCount = 6, status = "BURN_SPEED/50" }};
                    //burnSpeed.categoryIcon
                    list.Add(burnSpeed);
                    __result = list.ToArray();

                    var max_hp = ScriptableObject.CreateInstance<ItemCategoryEntity>();
                    max_hp.id = "VITALITY";
                    max_hp.name = "Vitality";
                    max_hp.categoryName = new LocalizedString("ItemCategory_Vitality");
                    max_hp.setStatus = new ItemCategoryEntity.SetTarget[5] {
                new ItemCategoryEntity.SetTarget() { itemCount = 2, status = "MAX_HP/5" },
                new ItemCategoryEntity.SetTarget() { itemCount = 3, status = "MAX_HP/10" },
                new ItemCategoryEntity.SetTarget() { itemCount = 4, status = "MAX_HP/15" },
                new ItemCategoryEntity.SetTarget() { itemCount = 5, status = "MAX_HP/20" },
                new ItemCategoryEntity.SetTarget() { itemCount = 6, status = "MAX_HP/25" }};
                    list.Add(max_hp);*/

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
                    binaryPlanet.keywordImage = KeywordImages.BinaryPlanet;
                    Melon<Core>.Logger.Msg("New Keyword: " + binaryPlanet.visualText.ToString());
                    list.Add(binaryPlanet);

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
            private static ItemEntity CreateCharmItemEntity(string name, List<string> categories, EItemRarity rarity, GameObject resource)
            {
                var entity = ScriptableObject.CreateInstance<ItemEntity>();
                entity.name = "Item_Charm_" + name;
                entity.activeType = EItemActiveType.Default;
                entity.aName = new LocalizedString("Item_Charm_"+ name + "_Name");
                entity.aFlavorText = new LocalizedString("Item_Charm_"+ name + "_FlavorText");
                entity.categories = categories;
                entity.cost = rarity switch
                {
                    EItemRarity.Common => 400,
                    EItemRarity.Uncommon => 600,
                    EItemRarity.Rare => 800,
                    EItemRarity.Legend => 1100,
                    _ => 200
                };
                entity.id = 8002;
                entity.itemBehaviour = ItemEntity.EItemBehaviour.None;
                entity.rarity = rarity;
                entity.type = EItemType.Charm;
                entity.sapphirePrice = rarity switch
                {
                    EItemRarity.Common => 2,
                    EItemRarity.Uncommon => 3,
                    EItemRarity.Rare => 4,
                    EItemRarity.Legend => 5,
                    _ => 1
                };
                entity.resourcePrefab = resource;
                entity.icon = SpriteLoader.LoadSprite(name);
                //entity.iconInWorld = IconInWorldCharm;
                return entity;
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
        /*        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SetItem))]
                class StoneTabletPatch
                {
                    static void Prefix(int instanceID, ItemEntity item, sbyte quantity, sbyte x, sbyte y, int rotation, ref GridInventory __instance)
                    {
                        //Melon<Core>.Logger.Msg("ApplyEffect: " + __instance.Inventory.stoneTablets.Count + ":" + __instance.Inventory.engravings.Count);
                        Melon<Core>.Logger.Msg("Prefix1: " + __instance.stoneTablets.Count);

                        Melon<Core>.Logger.Msg($"Prefix2({instanceID}): {item.Name} ({x}, {y})");


                        foreach (var tab in __instance.stoneTablets)
                        {
                            //Melon<Core>.Logger.Msg($"Prefix3: {tab.Key.x}, {tab.Key.y}:{tab.Value}");
                        }
                        if(item.Name == "利敵")
                        {
                            //Melon<Core>.Logger.Msg($"Prefix4: {item.resourcePrefab.GetComponent<StoneTablet>().Inventory == null}");
                        }
                    }
                    static void Postfix(ref GridInventory __instance)
                    {
                        Melon<Core>.Logger.Msg($"Postfix: ");
                        foreach (var tab in __instance.stoneTablets)
                        {
                            Melon<Core>.Logger.Msg($"{tab.Key.x}, {tab.Key.y}:{tab.Value}");
                        }
                    }
                }*/
        /*
        [HarmonyPatch]
        public class InstantiatePatch
        {
            static System.Reflection.MethodBase TargetMethod()
            {
                // refer to C# reflection documentation:
                return typeof(UnityEngine.Object).GetMethod(nameof(UnityEngine.Object.Instantiate), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).MakeGenericMethod(typeof(GameObject));
            }
            static void Prefix(GameObject original)
            {
                Melon<Core>.Logger.Msg($"Instantiate");
                if (original is GameObject go)
                {
                    if (go == null)
                        return;
                    Melon<Core>.Logger.Msg($"Instantiate: {go.name}");
                    if (go.name.Contains("StoneTablet-Malice")) // 対象判定
                    {

                    }
                }
            }
        }*/
        /*        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(UnityEngine.Object) })]
                public class DestroyPatch
                {
                    static void Prefix(UnityEngine.Object obj)
                    {
                        if (obj is GameObject go && go.name.Contains("Malice"))
                        {
                            Melon<Core>.Logger.Msg($"[DestroyPatch] Destroy called on {go.name}\n{Environment.StackTrace}");
                        }
                    }
                }
                [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.DestroyImmediate), new Type[] { typeof(UnityEngine.Object) })]
                public class DestroyImmediatePatch
                {
                    static void Prefix(UnityEngine.Object obj)
                    {
                        if (obj is GameObject go && go.name.Contains("Malice"))
                        {
                            Melon<Core>.Logger.Msg($"[DestroyPatch] DestroyImmediate called on {go.name}\n{Environment.StackTrace}");
                        }
                    }
                }*/
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
                Melon<Core>.Logger.Msg($"InstantiateMiraclePatch Transpiler");
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
        //[HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.StartBattle))]
        [Obsolete]
        public static class UnitAvatarStartBattlePatch
        {
            static void Prefix(UnitAvatar __instance)
            {
                if (__instance is PlayerAvatar)
                {
                    //Melon<Core>.Logger.Msg("StartBattle: " + __instance.Name);
                }
            }
        }
        /*
        [HarmonyPatch(typeof(NetworkClient), "SpawnPrefab")]
        public static class GetPrefabNetworkClientPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"GetPrefabNetworkClientPatch Transpiler");

                var replacement = AccessTools.Method(typeof(InstantiateNetworkClientPatch), nameof(CustomGetPrefab));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(NetworkClient.GetPrefab))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static bool CustomGetPrefab(uint assetId, out GameObject prefab)
            {
                Melon<Core>.Logger.Msg($"CustomGetPrefab: {assetId}");

                // 通常の GetPrefab
                return NetworkClient.GetPrefab(assetId, out prefab);
            }
        }*/
        /*
        [HarmonyPatch(typeof(LibraryFloorGenerator), ("DrawRoomInstance"), new Type[] { typeof(LibraryFloorRoomInstance), typeof(System.Random) })]
        public static class Patch
        {
            static bool Prefix(LibraryFloorRoomInstance instance, System.Random propRand, ref LibraryFloorGenerator __instance)
            {
                Melon<Core>.Logger.Msg("Debug: 1");
                Vector3Int vector3Int = new Vector3Int(instance.pos.x, instance.pos.y, 0);
                Vector2Int vector2Int = new Vector2Int(instance.Size.x, instance.Size.y);
                Vector2Int vector2Int2 = new Vector2Int(vector2Int.x + 2, vector2Int.y + 2);
                Melon<Core>.Logger.Msg("Debug: 2");
                if (__instance.floorArea_LB().x > vector3Int.x)
                {
                    Melon<Core>.Logger.Msg("Debug: 3");
                    __instance.SetfloorArea_LBX(vector3Int.x);
                }

                if (__instance.floorArea_LB().y > vector3Int.y)
                {
                    Melon<Core>.Logger.Msg("Debug: 4");
                    __instance.SetfloorArea_LBY(vector3Int.y);
                }

                if (__instance.floorArea_RT().x < vector3Int.x + vector2Int.x)
                {
                    Melon<Core>.Logger.Msg("Debug: 5");
                    __instance.SetfloorArea_RTX(vector3Int.x + vector2Int.x);
                }

                if (__instance.floorArea_RT().y < vector3Int.y + vector2Int.y)
                {
                    Melon<Core>.Logger.Msg("Debug: 6");
                    __instance.SetfloorArea_RTY(vector3Int.y + vector2Int.y);
                }

                Melon<Core>.Logger.Msg("Debug: 7");
                Texture2D texture2D = new Texture2D(vector2Int2.x, vector2Int2.y, TextureFormat.RGBA32, mipChain: false, linear: false)
                {
                    filterMode = FilterMode.Point
                };
                Color32[] array = new Color32[vector2Int2.x * vector2Int2.y];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new Color32(0, 0, 0, 0);
                }
                Melon<Core>.Logger.Msg("Debug: 8");

                texture2D.SetPixels32(array);
                Melon<Core>.Logger.Msg("Debug: 9");
                int[][] array2 = new int[vector2Int2.x][];
                for (int j = 0; j < vector2Int2.x; j++)
                {
                    array2[j] = new int[vector2Int2.y];
                }

                Melon<Core>.Logger.Msg("Debug: 10");
                for (int k = 0; k < vector2Int.x; k++)
                {
                    for (int l = 0; l < vector2Int.y; l++)
                    {
                        Vector3Int pos = vector3Int + new Vector3Int(k, l, 0);
                        bool flag = false;
                        Melon<Core>.Logger.Msg("Debug: 11");
                        TileBase groundTile = instance.Metadata.mainLayer.GetGroundTile(k, l);
                        __instance.SetGroundTile(pos, groundTile);
                        Melon<Core>.Logger.Msg("Debug: 12");
                        if ((bool)groundTile)
                        {
                            Melon<Core>.Logger.Msg("Debug: 13");
                            GroundTileEntity groundTileEntity = TileDatabase.FindGroundTile(groundTile);
                            Melon<Core>.Logger.Msg("Debug: 14");
                            if ((bool)groundTileEntity && (groundTileEntity.type == GroundTileEntity.Type.Pit || groundTileEntity.type == GroundTileEntity.Type.Water))
                            {
                                flag = true;
                            }
                        }

                        Melon<Core>.Logger.Msg("Debug: 15");
                        TileBase upperGroundTile = instance.Metadata.mainLayer.GetUpperGroundTile(k, l);
                        Melon<Core>.Logger.Msg("Debug: 16");
                        __instance.SetUpperGroundTile(pos, upperGroundTile);
                        Melon<Core>.Logger.Msg("Debug: 17");
                        if ((bool)upperGroundTile)
                        {
                            Melon<Core>.Logger.Msg("Debug: 18");
                            GroundTileEntity groundTileEntity2 = TileDatabase.FindGroundTile(upperGroundTile);
                            if ((bool)groundTileEntity2 && (groundTileEntity2.type == GroundTileEntity.Type.Pit || groundTileEntity2.type == GroundTileEntity.Type.Water))
                            {
                                flag = true;
                            }
                            Melon<Core>.Logger.Msg("Debug: 19");
                        }

                        Melon<Core>.Logger.Msg("Debug: 20");
                        TileBase wallTile = instance.Metadata.mainLayer.GetWallTile(k, l);
                        Melon<Core>.Logger.Msg("Debug: 21");
                        __instance.SetWallTile(pos, wallTile);
                        Melon<Core>.Logger.Msg("Debug: 22");
                        TileBase cliffTile = instance.Metadata.mainLayer.GetCliffTile(k, l);
                        Melon<Core>.Logger.Msg("Debug: 23");
                        __instance.SetCliffTile(pos, cliffTile);
                        Melon<Core>.Logger.Msg("Debug: 24");
                        if (flag)
                        {
                            array2[k + 1][l + 1] = 2;
                        }
                        else if (((bool)groundTile || (bool)upperGroundTile) && !wallTile && !cliffTile)
                        {
                            array2[k + 1][l + 1] = 1;
                        }
                        else
                        {
                            array2[k + 1][l + 1] = 0;
                        }
                        Melon<Core>.Logger.Msg("Debug: 25");
                    }
                }

                Melon<Core>.Logger.Msg("Debug: 26");
                for (int m = 0; m < vector2Int.x; m++)
                {
                    for (int n = 0; n < vector2Int.y; n++)
                    {
                        Melon<Core>.Logger.Msg("Debug: 27");
                        if (array2[m + 1][n + 1] == 2)
                        {
                            texture2D.SetPixel(m + 1, n + 1, Color.gray);
                        }
                        else if (array2[m + 1][n + 1] == 1)
                        {
                            texture2D.SetPixel(m + 1, n + 1, new Color(0.1f, 0.1f, 0.25f, 0.6f));
                        }
                        Melon<Core>.Logger.Msg("Debug: 28");
                    }
                }

                Melon<Core>.Logger.Msg("Debug: 29");
                for (int num = 0; num < vector2Int2.x; num++)
                {
                    for (int num2 = 0; num2 < vector2Int2.y; num2++)
                    {
                        int num3 = array2[num][num2];
                        bool flag2 = false;
                        for (int num4 = -1; num4 <= 1; num4++)
                        {
                            for (int num5 = -1; num5 <= 1; num5++)
                            {
                                if (num + num4 >= 0 && num + num4 < vector2Int2.x && num2 + num5 >= 0 && num2 + num5 < vector2Int2.y)
                                {
                                    int num6 = array2[num + num4][num2 + num5];
                                    if (num3 == 0 && num6 != 0)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (flag2)
                        {
                            Melon<Core>.Logger.Msg("Debug: 30");
                            texture2D.SetPixel(num, num2, Color.white);
                        }
                    }
                }
                Melon<Core>.Logger.Msg("Debug: 31");

                if (__instance.roomPassagePairList().TryGetValue(instance, out var value))
                {
                    Melon<Core>.Logger.Msg("Debug: 32");
                    foreach (PassageData item in value)
                    {
                        Melon<Core>.Logger.Msg("Debug: 33");
                        if (__instance.hiddenRoomInstances().Contains(item.parent) || __instance.hiddenRoomInstances().Contains(item.connect))
                        {
                            Melon<Core>.Logger.Msg("Debug: 34");
                            continue;
                        }
                        Melon<Core>.Logger.Msg("Debug: 35");

                        int num7 = item.dir switch
                        {
                            0 => 0,
                            2 => 0,
                            1 => 1,
                            3 => 1,
                            _ => -1,
                        };
                        Melon<Core>.Logger.Msg("Debug: 36");
                        if (item.parent == instance)
                        {
                            Melon<Core>.Logger.Msg("Debug: 37");
                            Vector2Int vector2Int3 = item.startPoint - item.parent.pos;
                            if (num7 == 0)
                            {
                                Melon<Core>.Logger.Msg("Debug: 38");
                                for (int num8 = 0; num8 < __instance.passageSize(); num8++)
                                {
                                    Melon<Core>.Logger.Msg("Debug: 39");
                                    texture2D.SetPixel(vector2Int3.x + num8 + 1, vector2Int3.y + 1, Color.green);
                                }
                                Melon<Core>.Logger.Msg("Debug: 40");
                            }
                            else
                            {
                                Melon<Core>.Logger.Msg("Debug: 41");
                                for (int num9 = 0; num9 < __instance.passageSize(); num9++)
                                {
                                    Melon<Core>.Logger.Msg("Debug: 42");
                                    texture2D.SetPixel(vector2Int3.x + 1, vector2Int3.y - num9 + 1, Color.green);
                                }
                                Melon<Core>.Logger.Msg("Debug: 43");
                            }
                        }
                        else
                        {
                            Melon<Core>.Logger.Msg("Debug: 44");
                            if (item.connect != instance)
                            {
                                Melon<Core>.Logger.Msg("Debug: 45");
                                continue;
                            }
                            Melon<Core>.Logger.Msg("Debug: 46");

                            Vector2Int vector2Int4 = item.endPoint - item.connect.pos;
                            Melon<Core>.Logger.Msg("Debug: 47");
                            if (num7 == 0)
                            {
                                Melon<Core>.Logger.Msg("Debug: 48");
                                for (int num10 = 0; num10 < __instance.passageSize(); num10++)
                                {
                                    Melon<Core>.Logger.Msg("Debug: 49");
                                    texture2D.SetPixel(vector2Int4.x + num10 + 1, vector2Int4.y + 1, Color.green);
                                }
                                Melon<Core>.Logger.Msg("Debug: 50");
                            }
                            else
                            {
                                Melon<Core>.Logger.Msg("Debug: 51");
                                for (int num11 = 0; num11 < __instance.passageSize(); num11++)
                                {
                                    Melon<Core>.Logger.Msg("Debug: 52");
                                    texture2D.SetPixel(vector2Int4.x + 1, vector2Int4.y - num11 + 1, Color.green);
                                }
                                Melon<Core>.Logger.Msg("Debug: 53");
                            }
                        }
                    }
                }

                Melon<Core>.Logger.Msg("Debug: 54");
                texture2D.Apply();
                Melon<Core>.Logger.Msg("Debug: 55");
                __instance.managedTextures().Add(texture2D);
                Melon<Core>.Logger.Msg("Debug: 56");
                __instance.roomTraps()[instance] = new List<ITrap>();
                Melon<Core>.Logger.Msg("Debug: 57");
                LibraryFloorMetadataBaker.PropMetadata[] mainProps = instance.Metadata.mainProps;
                Melon<Core>.Logger.Msg("Debug: 58");
                foreach (LibraryFloorMetadataBaker.PropMetadata propMetadata in mainProps)
                {
                    Melon<Core>.Logger.Msg("Debug: 59");
                    PropEntity propEntity = PropDatabase.FindPropById(propMetadata.id);
                    Melon<Core>.Logger.Msg("Debug: 60");
                    Vector3 vector = new Vector3(propMetadata.x, propMetadata.y);
                    Melon<Core>.Logger.Msg("Debug: 61");
                    XElement options = propMetadata.options;
                    Melon<Core>.Logger.Msg("Debug: 62");
                    GameObject gameObject = __instance.CreateProp(propRand.Next(), propEntity, __instance.transform.position + vector + vector3Int, Vector3.one, options);
                    Melon<Core>.Logger.Msg("Debug: 63");
                    if (propEntity.type == PropEntity.PropType.Breakable)
                    {
                        Melon<Core>.Logger.Msg("Debug: 64");
                        __instance.allBreakableProps().Add(gameObject);
                    }

                    Melon<Core>.Logger.Msg("Debug: 65");
                    if (gameObject.TryGetComponent<ITrap>(out var component))
                    {
                        Melon<Core>.Logger.Msg("Debug: 66");
                        __instance.roomTraps()[instance].Add(component);
                    }
                }

                Melon<Core>.Logger.Msg("Debug: 66");
                HUDMapData value2 = new HUDMapData
                {
                    name = instance.Metadata.name,
                    bottomLeft = (Vector2)__instance.transform.position + (Vector2)instance.pos,
                    topRight = (Vector2)__instance.transform.position + (Vector2)instance.pos + instance.Size,
                    sprite = texture2D
                };
                Melon<Core>.Logger.Msg("Debug: 67");
                __instance.hudMap().Add(instance, value2);

                Melon<Core>.Logger.Msg("Debug: 68");

                return false;
            }
        }*/

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
using FMODUnity;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using BuffStatus = CharacterBuff_StatusInstance.Status;
using SephiriaMod.Items;
using SephiriaMod.Combos;
using SephiriaMod.Utilities;
using SephiriaMod.Weapons;
using SephiriaMod.Registries;
using SephiriaMod.Buffs;
using SephiriaMod.Items.Eternal;

namespace SephiriaMod
{
    public static class Data
    {
        public static List<ModItem> All { get; private set; } = new();
        public static List<ModComboEffect> Combos { get; private set; } = new();
        public static List<ModEffectHUD> EffectHUDs { get; private set; } = new();
        public static List<ModMiracle> Miracles { get; private set; } = new();
        public static List<ModCustomStatus> Statuses { get; private set; } = new();
        public static List<ModWeapon> Weapons { get; private set; } = new();
        public static List<CharacterBuffMod> Buffs { get; private set; } = new();
        public static List<ModKeyword> Keywords { get; private set; } = new();
        public static List<ModPassive> Passives { get; private set; } = new();
        public static List<string> AllResourcePrefabNames { get; private set; }
        /// <summary>
        /// Item_Malice_Name
        /// 利敵
        /// </summary>
        public static ModStoneTablet Malice { get; } = ModStoneTablet.Create("Malice", "O 4\nUP -1\nDOWN -1\nLEFT -1\nRIGHT -1", false).SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_Bitterness_Name
        /// 辛辣
        /// </summary>
        //public static ModStoneTablet Bitterness { get; } = ModStoneTablet.Create("Bitterness", "UP -1\nDOWN -1\nLEFT +3\nRIGHT +3", false).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_War_Crime_Name
        /// 戦犯
        /// </summary>
        public static ModStoneTablet WarCrime { get; } = ModStoneTablet.Create("War_Crime", "O 8\nUP -2\nDOWN -2\nLEFT -2\nRIGHT -2\nDIAUPLEFT -1\r\nDIAUPRIGHT -1\r\nDIADOWNLEFT -1\r\nDIADOWNRIGHT -1", false).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Transcendent_Name
        /// 超絶
        /// </summary>
        //public static ModStoneTablet Transcendent { get; } = ModStoneTablet.Create("Transcendent", "UP 5\nDOWN 5\nLEFT -2\nRIGHT -2", false).SetRarity(EItemRarity.Legend).SetCannotBeReward();
        /// <summary>
        /// Item_Reserved_MP_Evasion_Name
        /// 鉄扇
        /// Item_Reserved_MP_Evasion_FlavorText
        /// 影を起こす扇子。
        /// Item_Reserved_MP_Evasion_Effect
        /// <tag=MP>を{MANA}<tag=ReservedMP>して以下の効果を得る
        /// </summary>
        public static ModCharmStatus ReservedMPEvasion { get; } = ModCharmStatus.Create<Charm_ReservedMPEvasion>("Reserved_MP_Evasion", 4, CreateStatusGroup("EVASION", 200, 300, 400, 500, 700), CreateStatusGroup("PHYSICAL_DAMAGE", 0, 0, 1, 2, 3))
            .SetCategory(ItemCategories.Sturdy, ItemCategories.Shadow).SetSimpleEffect().SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Max_HP_Name
        /// 活力のお守り
        /// Item_Max_HP_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharmStatus MaxHP { get; } = ModCharmStatus.Create("Max_HP", 1, CreateStatusGroup("MAX_HP", 5, 10))
            .SetCategory(ItemCategories.Vitality).SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_Revive_Player_Haste_Name
        /// 紅い涙
        /// Item_Revive_Player_Haste_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharmStatus RevivePlayerHaste { get; } = ModCharmStatus.Create("Revive_Player_Haste", 3, CreateStatusGroup("REVIVE_PLAYER_HASTE", 20, 30, 40, 50), CreateStatusGroup("MAX_HP", 10, 20, 30, 40))
            .SetCategory(ItemCategories.Vitality).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Add_Star_Ruby_Name
        /// ルビーの原石
        /// Item_Add_Star_Ruby_FlavorText
        /// 輝く、その時を待っている
        /// Item_Add_Star_Ruby_Effect
        /// ポーションを{QUEST}回飲むと、{REWARD}に変わります\n[ポーションを飲んだ回数：{CURRENT}]
        /// </summary>
        public static ModCharmStatus AddStarRuby { get; } = ModCharmStatus.Create<Charm_AddStarRuby>("Add_Star_Ruby", 0, CreateStatusGroup("HP_POTION_BONUS", 20))
            .SetCategory(ItemCategories.Vitality).SetSimpleEffect().SetIsUniqueEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Create_Regen_Potion_Name
        /// 再生の水筒
        /// Item_Create_Regen_Potion_FlavorText
        /// こまめな水分補給を忘れずに。
        /// Item_Create_Regen_Potion_Effect
        /// {REQUIRE}回ステージを移動するごとに{ITEM}を獲得する
        /// </summary>
        public static ModCharm CreateRegenPotion { get; } = ModCharmStatus.Create<Charm_CreateRegenPotion>("Create_Regen_Potion", 2, CreateStatusGroup("FINAL_HP", 5, 10, 20))
            .SetCategory(ItemCategories.Vitality).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Max_HP_Attack_Name
        /// 溢れる生命
        /// Item_Max_HP_Attack_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Max_HP_Attack_Effect
        /// 敵にダメージを与える時、追加で最大<tag=HP>の{PERCENT}%のダメージを与える\n[ダメージ：{DAMAGE}]
        /// </summary>
        public static ModCharmStatus MaxHPAttack { get; } = ModCharmStatus.Create<Charm_MaxHPAttack>("Max_HP_Attack", 5, CreateStatusGroup("MAX_HP", 5, 10, 15, 20, 25, 30), CreateStatusGroup("DEFENSE", -5, -5, -10, -10, -20, -20))
            .SetCategory(ItemCategories.Vitality).SetSimpleEffect().SetIsUniqueEffect().SetDamageId().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Kill_Luck_Name
        /// 無音の羽ペン
        /// Item_Kill_Luck_FlavorText
        /// そのインクは風に乗って伝わる。
        /// Item_Kill_Luck_Effect
        /// 敵を{DIVIDE}回倒すごとに<tag=Luck>が{LUCK}増加する\n[現在の追加幸運：{CURRENT}(<tag=Luck>{LUCK}×{COUNT}回/{DIVIDE})]
        /// </summary>
        public static ModCharmStatus KillLuck { get; } = ModCharmStatus.Create<Charm_Kill_Luck>("Kill_Luck", 3, CreateStatusGroup("ATTACK_SPEED", 0, 4, 8, 8))
            .SetCategory(ItemCategories.Fortune, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsDual();

        /// <summary>
        /// Item_Legendary_Mania_Name
        /// 英雄の剣
        /// Item_Legendary_Mania_FlavorText
        /// 伝説を紡ぐ剣
        /// Item_Legendary_Mania_Effect
        /// バッグの伝説アーティファクトの数だけ以下の効果を得る\n[現在の伝説アーティファクト数：{COUNT}個]
        /// </summary>
        public static ModCharm LegendaryMania { get; } = ModCharm.Create<Charm_LegendaryMania>("Legendary_Mania", 2, true)
            .SetSimpleEffect().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Level_Distributer_Name
        /// 石版の欠片
        /// Item_Level_Distributer_FlavorText
        /// まだ熱い。
        /// Item_Level_Distributer_Effect
        /// このアーティファクトのレベルと同じだけ、上の枠を+1する。
        /// </summary>
        public static ModCharm LevelDistributer { get; } = ModCharm.Create<Charm_LevelDistributer>("Level_Distributer", 6, false)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_More_Stone_Tablet_Name
        /// 羅針盤
        /// Item_More_Stone_Tablet_FlavorText
        /// 星の向きを示す道具。
        /// Item_More_Stone_Tablet_Effect
        /// 報酬で{ITEM_TYPE}が出現する確率が{DROP_PERCENT}増加
        /// </summary>
        public static ModCharm MoreStoneTablet { get; } = ModCharm.Create<Charm_MoreStoneTablet>("More_Stone_Tablet", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_Double_Debuff_Stack_Name
        /// 夜空の香水
        /// Item_Double_Debuff_Stack_FlavorText
        /// 景色も変わる星空の香り。
        /// </summary>
        public static ModCharm ChaosDamage { get; } = ModCharm.Create<Charm_ChaosDamage>("Chaos_Damage", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetEffects("Charm_FrostiumRing_Effect").SetDamageId().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Double_Debuff_Stack_Name
        /// 稲妻の彗星
        /// Item_Double_Debuff_Stack_FlavorText
        /// 星の力が注がれたエネルギーの塊。
        /// </summary>
        public static ModCharm DoubleDebuffStack { get; } = ModCharmStatus.Create<Charm_VariableMaxLevel>("Double_Debuff_Stack", 6,
            CreateStatusGroup("BURN_STACK", 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 5),
            CreateStatusGroup("ELECTRIC_STACK", 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 5))
            .SetCategory(ItemCategories.Stargaze).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Create_Stone_Tablet_Name
        /// 流れ星の結晶
        /// Item_Create_Stone_Tablet_FlavorText
        /// 隕石が形を変えていく。
        /// Item_Create_Stone_Tablet_Effect
        /// 敵を{QUEST}回倒すたびに、このアイテムがある枠を-1して石版{REWARD}を獲得する。\n[敵を倒した回数：{CURRENT}]
        /// Item_Create_Stone_Tablet_Effect2
        /// {MAX}回発動するとこの効果は失われる。\n[現在の発動回数：{COUNT}]
        /// </summary>
        public static ModCharm CreateStoneTablet { get; } = ModCharmStatus.Create<Charm_CreateStoneTablet>("Create_Stone_Tablet", 0, CreateStatusGroupHide("EXP_DROP", 0, 5, 10, 15, 20))
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffects(2).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Triple_Attack_Debuff_Name
        /// 魔法仕掛けの天球儀
        /// Item_Triple_Attack_Debuff_FlavorText
        /// 夜空に染まった球体。
        /// Item_Triple_Attack_Debuff_Effect
        /// 次に火属性のダメージを与えた時、<tag=Burn>デバフを付与し、次に氷属性のダメージを与えた時、<tag=Frostbite>を付与する。この次に雷属性のダメージを与えた時、<tag=Electric>を付与し、最初に戻る。[クールタイム：{INTERVAL}秒]
        /// </summary>
        public static ModCharm TripleAttackDebuff { get; } = ModCharmStatus.Create<Charm_TripleAttackDebuff>("Triple_Attack_Debuff", 6,
            CreateStatusGroup("FIRE_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15),
            CreateStatusGroup("ICE_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15),
            CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15))
            .SetCategory(ItemCategories.Stargaze).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Chaos_Attack_Name
        /// 三体模型
        /// Item_Chaos_Attack_FlavorText
        /// 三つの天体の軌道。
        /// Item_Chaos_Attack_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{CHANCE}の確率で追加の<tag=Elemental_Chaos>ダメージを与える。\n[ダメージ：{DAMAGE}(<tag=FireDamage>{PERCENT}+<tag=IceDamage>{PERCENT}+<tag=LightningDamage>{PERCENT})]
        /// </summary>
        public static ModCharm ChaosAttack { get; } = ModCharm.Create<Charm_ChaosAttack>("Chaos_Attack", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetDamageId().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Stargaze_Tablet_Name
        /// 星見の石版
        /// Item_Stargaze_Tablet_FlavorText
        /// 昼でもくっきりと星が見える。
        /// Item_Stargaze_Tablet_Effect
        /// 石版を手に入れるたびに破壊し、そのレアリティに応じたポイントを得る。\n{QUEST}ポイント得るごとに{COUNT}回、周囲8マスのうちアーティファクトがないランダムな枠1つを+{REWARD}する。\n[現在の石版ポイント：{CURRENT}]
        /// Item_Stargaze_Tablet_Effect2
        /// <tag=WeaponAction_SpecialAttack>を<tag=Elemental_Chaos>属性に変え、バッグのアーティファクトのレベル合計の{POWER_DAMAGE}のダメージを追加する。\n[追加ダメージ：{DAMAGE}(アーティファクトのレベル合計{POWER}×{POWER_DAMAGE})]
        /// Item_Stargaze_Tablet_Notice
        /// 石版が破壊されました
        /// </summary>
        public static ModCharm StargazeTablet { get; } = ModCharmStatus.Create<Charm_StargazeTablet>("Stargaze_Tablet", 4, CreateStatusGroupHide("SPECIAL_ATTACK_DAMAGE", 0, 0, 0, 0, 0, 12, 18, 24, 32))
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffects(1).SetIsUniqueEffect().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Copy_Academy_Name
        /// 原典
        /// Item_Copy_Academy_FlavorText
        /// 知識無き者の脳を蝕む
        /// Item_Copy_Academy_Effect
        /// 戦闘中に魔法を{QUEST}回使用すると、下の枠にある固有でないアカデミーアーティファクトに変わります。\n[魔法を使用した回数：{CURRENT}]
        /// </summary>
        public static ModCharm CopyAcademy { get; } = ModCharmStatus.Create<Charm_CopyAcademy>("Copy_Academy", 1, CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 20, 40))
            .SetCategory(ItemCategories.Academy).SetIsUniqueEffect().SetEffects("Charm_MagicianCoin_Effect", "Item_Copy_Academy_Effect").SetRarity(EItemRarity.Rare);

        /// <summary>
        /// Item_Auto_Buff_Name
        /// 簡易自律魔法陣
        /// Item_Auto_Buff_FlavorText
        /// IndexOutOfRangeException...
        /// Item_Auto_Buff_Effect
        /// 下の枠にあるバフ魔法の<tag=Magic>を{COOLDOWN}秒ごとに自動発動する。
        /// </summary>
        public static ModCharm AutoBuff { get; } = ModCharm.Create<Charm_AutoBuff>("Auto_Buff", 1, false)
            .SetCategory(ItemCategories.Academy).SetEffects("Charm_MagicianCoin_Effect", "Item_Auto_Buff_Effect").SetRarity(EItemRarity.Uncommon);

        /// <summary>
        /// Item_Auto_Magic_Legend_Name
        /// 試用自律魔法陣
        /// Item_Auto_Magic_Legend_FlavorText
        /// NullReferenceException...
        /// Item_Auto_Magic_Legend_Effect
        /// 上の枠にある<tag=Magic>を{COOLDOWN}秒遅れて自動発動する。
        /// </summary>
        public static ModCharm AutoMagicLegend { get; } = ModCharm.Create<Charm_AutoMagicLegend>("Auto_Magic_Legend", 5, false)
            .SetCategory(ItemCategories.Academy).SetSimpleEffect().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Many_Grimoire_Name
        /// 元素学の写本
        /// Item_Many_Grimoire_FlavorText
        /// 集めた知識が役に立つ時。
        /// Item_Many_Grimoire_Effect
        /// バッグの<tag=Magic>の数だけ<tag=FireDamage><tag=IceDamage><tag=LightningDamage>+{STATUS}
        /// Item_Many_Grimoire_Effect2
        /// 現在の<tag=Magic>数：{COUNT}個
        /// </summary>
        public static ModCharm ManyGrimoire { get; } = ModCharm.Create<Charm_ManyGrimoire>("Many_Grimoire", 4, false)
            .SetCategory(ItemCategories.Academy, ItemCategories.Elemental).SetEffects("Item_Many_Grimoire_Effect", "Item_Many_Grimoire_Effect2").SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();

        /// <summary>
        /// Item_Fire_Cooldown_Recovery_Name
        /// 烈火の原子時計
        /// Item_Fire_Cooldown_Recovery_FlavorText
        /// 摩擦で回りにくくなってしまった。
        /// Item_Fire_Cooldown_Recovery_Effect
        /// {REQUIRE}回火属性ダメージを与えるたびにすべての<tag=Magic>のクールタイムを少しだけ加速させる。
        /// </summary>
        public static ModCharm FireCooldownRecovery { get; } = ModCharmStatus.Create<Charm_FireCooldownRecovery>("Fire_Cooldown_Recovery", 3, CreateStatusGroup("BURN_SPEED", 5, 10, 15, 20))
            .SetCategory(ItemCategories.Academy, ItemCategories.Ember).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);

        /// <summary>
        /// Item_Bond_Maker_Name
        /// 恋結びのリボン
        /// Item_Bond_Maker_FlavorText
        /// 淡い絆を結ぶ。
        /// Item_Bond_Maker_Effect
        /// このアーティファクトのレベルが{LEVEL}になると、このアイテムと左右の枠にあるアーティファクトは壊れる。左右の枠にあったアーティファクトのカテゴリーに含まれるランダムな絆アーティファクト1つを獲得する。
        /// </summary>
        public static ModCharm BondMaker { get; } = ModCharmStatus.Create<Charm_BondMaker>("Bond_Maker", 3, CreateStatusGroup("BOSS_REWARD_DICE", 1, 2), CreateStatusGroup("LUCK", 4, 8))
            .SetCategory().SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Wound_Weapon_Name
        /// 血の入れ墨
        /// Item_Wound_Weapon_FlavorText
        /// 消えることのない呪われた傷跡。
        /// Item_Wound_Weapon_Effect2
        /// 敵を<tag=WeaponAction_DirectAttack>が命中した時、<tag=Debuff_Wound>を付与する。
        /// Item_Wound_Weapon_Effect3
        /// <tag=Debuff_Wound>スタック {STACK}
        /// Item_Wound_Weapon_Effect4
        /// 敵にデバフを付与するたびに<tag=WeaponAction_SpecialAttack>のコスト減少 {COST}\n<tag=WeaponAction_SpecialAttack>をするとリセットされる。\n[現在のコスト減少量：{CURRENT}]
        /// Item_Wound_Weapon_Effect
        /// <tag=BasicAttackDamage>を0%にする
        /// </summary>
        public static ModCharm WoundWeapon { get; } = ModCharmStatus.Create<Charm_WoundWeapon>("Wound_Weapon", 6, CreateStatusGroup("ATTACK_SPEED", 4, 8, 12, 16, 20, 25, 30))//, CreateStatusGroup("BASIC_ATTACK_DAMAGE", -100, -125, -150, -175, -200), CreateStatusGroup("DASH_ATTACK_DAMAGE", -200)
            .SetCategory(ItemCategories.Curse).SetRelatedWeapon(EWeaponType.GreatSword).SetIsUniqueEffect().SetSimpleEffects(4).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Max_MP_Power_Name
        /// 水神の目
        /// Item_Max_MP_Power_FlavorText
        /// 水を生み出す竜の宝石。
        /// Item_Max_MP_Power_Effect
        /// MP再生を最終MPに変換する。
        /// Item_Max_MP_Power_Effect2
        /// <tag=WeaponAction_SpecialAttack>でダメージを与える時、最大<tag=MP>の{PERCENT}を消費して、空の<tag=MP>数値の{DAMAGE}のダメージを追加する。\n[次の追加ダメージ：およそ{MP}(空の<tag=MP>数値{DAMAGE})]
        /// </summary>
        public static ModCharm MaxMPPower { get; } = ModCharmStatus.Create<Charm_MaxMPPower>("Max_MP_Power", 4, CreateStatusGroup("MP_REGEN", 5, 10, 20, 20), CreateStatusGroup("MAX_MP", 5, 10, 20, 20))
            .SetCategory(ItemCategories.Lake).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Follower_Died_Heal_Name
        /// 友情のリストバンド
        /// Item_Follower_Died_Heal_FlavorText
        /// 契約とは程遠い小さな、それでも太い糸。
        /// Item_Follower_Died_Heal_Effect
        /// HP消費以外によるダメージで<tag=Follower>が倒れた時、プレイヤーの<tag=HP>を回復する。（上限を超過して回復する）\n[HP回復量：倒れた<tag=Follower>の最大<tag=HP>{HEAL}]
        /// </summary>
        public static ModCharm FollowerDiedHeal { get; } = ModCharm.Create<Charm_CompanionDiedHeal>("Follower_Died_Heal", 4, true)
            .SetCategory(ItemCategories.Companion).SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Pallas_Joker_Name
        /// パラスのジョーカー
        /// Item_Pallas_Joker_FlavorText
        /// 夢の中で失くしたカードがここにある。
        /// Item_Pallas_Joker_Effect
        /// 周囲8マスにあるパラスのカードを強化（強化されたパラスのカードは発射確率とダメージが増加）
        /// Item_Pallas_Joker_Effect2
        /// パラスのカードの発射確率が100％を超えた場合、超過した分の確率が追加のカードの発射確率に変換される。\n[発射するカードの枚数：{COUNT}]
        /// </summary>
        public static ModCharm PallasJoker { get; } = ModCharmStatus.Create<Charm_PallasJoker>("Pallas_Joker", 4, CreateStatusGroup("LUCK", 3, 6, 9, 12, 15))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffects(2).SetDamageId().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Throw_Grimoire_Name
        /// 魔導書キャノン
        /// Item_Throw_Grimoire_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        //public static ModCharm ThrowGrimoire { get; } = ModCharm.Create<Charm_ThrowGrimoire>("Throw_Grimoire", 4, true)
        //.SetCategory(ItemCategories.Academy).SetSimpleEffects(2).SetDamageId().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Electric_Stun_Name
        /// ビリビリクリームクロワッサン
        /// Item_Electric_Stun_FlavorText
        /// 痺れるカスタードクリーム入りクロワッサン。
        /// Item_Electric_Stun_Effect
        /// 感電を付与するたびに、感電が付与されていない敵に雷属性ダメージを与える時に、気絶させる確率が{PERCENT}増加する（気絶させると確率はリセットされる）\n[現在の気絶確率：{CURRENT}]
        /// </summary>
        public static ModCharm ElectricStun { get; } = ModCharmStatus.Create<Charm_ElectricStun>("Electric_Stun", 3, CreateStatusGroup("ELECTRIC_STACK", 0, 1, 1, 2))
            .SetCategory(ItemCategories.Magitech).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Transcendent_Charm_Name
        /// 断絶の魔石
        /// Item_Transcendent_Charm_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Transcendent_Charm_Effect
        /// 報酬で{ITEM_TYPE}が出現する確率が{DROP_PERCENT}増加
        /// Item_Transcendent_Charm_Effect2
        /// インベントリに横一列に断絶が並ぶと、それらを超絶に変える
        /// Item_Transcendent_Charm_Effect3
        /// バッグの断絶の数だけ<tag=PhysicalDamage> {DAMAGE}
        /// Item_Transcendent_Charm_Effect4
        /// 現在の断絶の数：{COUNT}個
        /// Item_Transcendent_Charm_Effect5
        /// 断絶と超絶は星見の石版に破壊されない
        /// </summary>
        //public static ModCharm TranscendentCharm { get; } = ModCharmStatus.Create<Charm_Transcendent>("Transcendent_Charm", 5, CreateStatusGroup("LUCK", 5, 6, 7, 8))
        //.SetCategory(ItemCategories.Mystic).SetIsUniqueEffect().SetSimpleEffects(5).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Attack_Speed_Name
        /// 風のお守り
        /// Item_Attack_Speed_FlavorText
        /// 嵐の前の静かな風。
        /// </summary>
        public static ModCharm AttackSpeed { get; } = ModCharmStatus.Create("Attack_Speed", 2, CreateStatusGroup("ATTACK_SPEED", 2, 4, 8))
            .SetCategory(ItemCategories.WindSong).SetIsUniqueEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_Dash_Speed_Name
        /// 白いプロペラ
        /// Item_Dash_Speed_FlavorText
        /// 空高く舞うフレーバーテキスト募集中
        /// </summary>
        public static ModCharm DashSpeed { get; } = ModCharmStatus.Create("Dash_Speed", 3, CreateStatusGroup("DASH_SPEED", 10, 20, 30, 40), CreateStatusGroup("DASH_RECOVERY_SPEED", 5, 10, 20, 30))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Dash_Attack_Scale_Up_Name
        /// 楽譜「空」
        /// Item_Dash_Attack_Scale_Up_FlavorText
        /// 空が吹く。
        /// Item_Dash_Attack_Scale_Up_Effect
        /// <tag=WeaponAction_DashAttack>の範囲{RANGE}増加
        /// </summary>
        public static ModCharm DashAttackScaleUp { get; } = ModCharmStatus.Create<Charm_DashAttackScaleUp>("Dash_Attack_Scale_Up", 4, CreateStatusGroup("DASH_COUNT", 0, 0, 1, 1, 1))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Dash_Attack_More_Name
        /// 追撃の籠手
        /// Item_Dash_Attack_More_FlavorText
        /// 青く透き通った空から逃れることはできない。
        /// Item_Dash_Attack_More_Effect
        /// <tag=WeaponAction_DashAttack>が命中した時、{PERCENT}の確率で<tag=DashCount>が1回復する（クールタイム{COOLDOWN}秒）
        /// </summary>
        public static ModCharm DashAttackMore { get; } = ModCharmStatus.Create<Charm_DashAttackMore>("Dash_Attack_More", 3, CreateStatusGroup("DASH_ATTACK_DAMAGE", 20, 30, 40, 50))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_On_Damaged_Cosume_Dash_Name
        /// 翔ける羽根飾り
        /// Item_On_Damaged_Cosume_Dash_FlavorText
        /// 傷を癒す不死鳥の羽根。
        /// Item_On_Damaged_Cosume_Dash_Effect
        /// ダメージを受けた時、<tag=DashCount>を{DASH}消費することで受けたダメージの{PERCENT}を回復する（クールタイム{COOLDOWN}秒、<tag=DashRecovery>が適用されます）
        /// </summary>
        public static ModCharm OnDamagedCosumeDash { get; } = ModCharmStatus.Create<Charm_OnDamagedCosumeDash>("On_Damaged_Cosume_Dash", 5, CreateStatusGroup("DASH_RECOVERY_SPEED", 0, 5, 5, 10, 10, 20))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Double_Dash_Name
        /// 晴れ雲
        /// Item_Double_Dash_FlavorText
        /// 透き通った雲。
        /// Item_Double_Dash_Effect
        /// ダッシュした時、<tag=DashCount>が1回復し、もう一度ダッシュする
        /// </summary>
        public static ModCharm DoubleDash { get; } = ModCharmStatus.Create<Charm_DoubleDash>("Double_Dash", 2, CreateStatusGroup("PHYSICAL_DAMAGE", 2, 3, 5))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Fixed_Move_Speed_Name
        /// 歪んだ靴
        /// Item_Fixed_Move_Speed_FlavorText
        /// 靴があなたを履いている。
        /// Item_Fixed_Move_Speed_Effect
        /// <tag=MoveSpeed>が{PERCENT}に固定されます
        /// </summary>
        public static ModCharm FixedMoveSpeed { get; } = ModCharmStatus.Create<Charm_FixedMoveSpeed>("Fixed_Move_Speed", 3, CreateStatusGroup("DASH_ATTACK_DAMAGE", 20, 40, 80, 200))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Another_Execution_Name
        /// ウリエルの手斧
        /// Item_Another_Execution_FlavorText
        /// 手に持って使うには少し小さい。
        /// Item_Another_Execution_Effect
        /// 攻撃の<tag=CriticalChance>が100％を超えた場合、超過した分の確率が<tag=MagicExecution>の発生率に変換される。
        /// Item_Another_Execution_Effect2
        /// <tag=MagicExecution>の<tag=IgnoreDefence>が{PERCENT}増加する
        /// </summary>
        public static ModCharm AnotherExecution { get; } = ModCharmStatus.Create<Charm_AnotherExecution>("Another_Execution", 3, CreateStatusGroup("CRITICAL", 250, 500, 750, 1000))
            .SetCategory(ItemCategories.Precision).SetIsUniqueEffect().SetSimpleEffects(1).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Electric_Critical_Name
        /// 雷鳴の槌
        /// Item_Electric_Critical_FlavorText
        /// 騒音の苦情があったため今は使われていません。
        /// Item_Electric_Critical_Effect
        /// <tag=WeaponAction_DirectAttack>または雷属性攻撃<sprite=\"Keyword\" name=LightningDamage>の<tag=CriticalChance>が雷属性ダメージの{PERCENT}増加\n[増加確率：+{CRITICAL}%]
        /// </summary>
        public static ModCharm ElectricCritical { get; } = ModCharmStatus.Create<Charm_ElectricCritical>("Electric_Critical", 5)//, CreateStatusGroup("LIGHTNING_DAMAGE", 0, 2, 4, 6, 8, 10)
            .SetCategory(ItemCategories.Precision, ItemCategories.Magitech).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Separate_Direct_Attack_Name
        /// 惜別のアレキサンドライト
        /// Item_Separate_Direct_Attack_FlavorText
        /// 赤と青の扉。
        /// Item_Separate_Direct_Attack_Effect
        /// 元のダメージが{DAMAGE}以上の<tag=WeaponAction_DirectAttack>を2回に分割する
        /// </summary>
        public static ModCharm SeparateDirectAttack { get; } = ModCharmStatus.Create<Charm_SeparateDirectAttack>("Separate_Direct_Attack", 4, CreateStatusGroup("FINAL_WEAPONDAMAGE", 5, 8, 12, 17, 25), CreateStatusGroup("ATTACK_SPEED", 2, 4, 6, 8, 12))
            .SetCategory(ItemCategories.Sturdy, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Evasion_Frost_Name
        /// 氷の簪
        /// Item_Evasion_Frost_FlavorText
        /// 冷たい表情を伴った簪。
        /// Item_Evasion_Frost_Effect
        /// <tag=Evasion>するたびに右の枠の<tag=FrostRelic>アーティファクトのチャージを少しだけ進める
        /// Item_Evasion_Frost_Effect2
        /// <tag=Evasion>1につき{PERCENT}の確率で、<tag=FrostRelic>発動時に即時チャージされる（現在：{CURRENT}）
        /// </summary>
        public static ModCharm EvasionFrost { get; } = ModCharmStatus.Create<Charm_EvasionFrost>("Evasion_Frost", 2, CreateStatusGroup("EVASION", 200, 400, 700))
            .SetCategory(ItemCategories.Frost, ItemCategories.Shadow).SetIsUniqueEffect().SetSimpleEffects(2).SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Super_Meteor_Name
        /// メテオライトの指輪
        /// Item_Super_Meteor_FlavorText
        /// 煌びやかな装飾がお気に入り。
        /// Item_Super_Meteor_Effect
        /// 隕石の落下速度 {SPEED}
        /// Item_Super_Meteor_Effect2
        /// 隕石のダメージ増幅 {DAMAGE}
        /// Item_Super_Meteor_Effect3
        /// <tag=Burn>ダメージを除く<tag=FireDamage>を与えた時、{PERCENT}の確率で<tag=FireDamage>{METEOR}の隕石1個を落とす（クールタイム1秒、<tag=AttackSpeed>が適用されます）
        /// </summary>
        public static ModCharm SuperMeteor { get; } = ModCharmStatus.Create<Charm_SuperMeteor>("Super_Meteor", 5, CreateStatusGroup("ATTACK_SPEED", 2, 4, 6, 8, 12, 16))
            .SetCategory(ItemCategories.Ember, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffects(3).SetIsDual().SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_Blood_Mp_Name
        /// 血の祭壇
        /// Item_Blood_Mp_FlavorText
        /// かつて魔力で甦りを果たそうとした者がいた。
        /// Item_Blood_Mp_Effect
        /// 血の魔力：<tag=MP>の代わりに<tag=HP>を消費する
        /// Item_Blood_Mp_Effect2
        /// <tag=Magic>ダメージに空の<tag=HP>数値の{PERCENT}のダメージを追加する
        /// 戦闘中、<tag=MPRegen>を<tag=HPRegen>に変換する
        /// </summary>
        public static ModCharm BloodMp { get; } = ModCharmStatus.Create<Charm_BloodMP>("Blood_Mp", 3, CreateStatusGroup("MAGIC_DAMAGE_BONUS", 8, 16, 25, 50), CreateStatusGroup("MAX_HP", 5, 10, 20, 40))
            .SetCategory(ItemCategories.Academy, ItemCategories.Vitality).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Inventory_Power_Name
        /// カジノチップ
        /// Item_Inventory_Power_FlavorText
        /// ここでは何の価値も無い。
        /// Item_Inventory_Power_Effect
        /// 周囲8枠にあるカジノチップ以外のアーティファクトをカジノチップに変える
        /// Item_Inventory_Power_Effect2
        /// 他のアーティファクトをカジノチップに変えた時、元のアーティファクトのカテゴリーに基づいて以下の効果からランダムに獲得する（発動するとこの効果を失う）
        /// </summary>
        public static ModCharm InventoryPower { get; } = ModCharmStatus.Create<Charm_InventoryPower>("Inventory_Power", 0, false)
            .SetCategory().SetSimpleEffects(2).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Pallas_Ace_Name
        /// パラスのエース
        /// Item_Pallas_Ace_FlavorText
        /// スペードがどこかに行ってしまったんだ。
        /// Item_Pallas_Ace_Effect
        /// ダッシュ攻撃時に{DEFAULT}%の確率でカードを発射
        /// Item_Pallas_Ace_Effect2
        /// <tag=Luck>1ごとに発射確率が{CHANCE}増加（現在：{CURRENT}）\n[ダメージ：{DAMAGE}]
        /// </summary>
        public static ModCharm PallasAce { get; } = ModCharmStatus.Create<Charm_PallasAce>("Pallas_Ace", 4, true)
            .SetCategory(ItemCategories.Fortune).SetSimpleEffects(2).SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_First_Heal_Name
        /// 酔狂のお守り
        /// Item_First_Heal_FlavorText
        /// このお守りを見て思い出す。
        /// Item_First_Heal_Effect
        /// このアーティファクトを初めて獲得した時、プレイヤーの<tag=HP>を{HEAL}回復する
        /// </summary>
        public static ModCharm FirstHeal { get; } = ModCharmStatus.Create<Charm_FirstHeal>("First_Heal", 1, CreateStatusGroup("DEFENSE", -2, -3))
            .SetCategory(ItemCategories.Drunk).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_Drunk_Elemental_Name
        /// 虹の欠片
        /// Item_Drunk_Elemental_FlavorText
        /// 辛酸と共に零れた七色の欠片。
        /// </summary>
        public static ModCharm DrunkElemental { get; } = ModCharmStatus.Create("Drunk_Elemental", 3, CreateStatusGroup("DEFENSE", -2, -3, -5, -7),
            CreateStatusGroup("PHYSICAL_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("FIRE_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("ICE_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("LIGHTNING_DAMAGE", 1, 2, 3, 5))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_Revive_Heal_Name
        /// 御神酒
        /// Item_Revive_Heal_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_Revive_Heal_Effect
        /// 他のプレイヤーによって復活した時、復活させたプレイヤーのHPをこのプレイヤーの<tag=Defence>{DEFENSE}ごとに{HEAL}回復する
        /// </summary>
        public static ModCharm ReviveHeal { get; } = ModCharmStatus.Create<Charm_ReviveHeal>("Revive_Heal", 3, CreateStatusGroup("DEFENSE", -2, -5, -8, -12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Other_Defense_Name
        /// 先陣の盃
        /// Item_Other_Defense_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_Other_Defense_Effect
        /// 他の全てのプレイヤーの<tag=Defense> {DEFENSE}
        /// </summary>
        public static ModCharm OtherDefense { get; } = ModCharmStatus.Create<Charm_OtherDefense>("Other_Defense", 5, CreateStatusGroup("DEFENSE", -1, -2, -4, -8, -10, -12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Half_HP_Name
        /// 百の薬
        /// Item_Half_HP_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_Half_HP_Effect
        /// HPが50%以上の時、<tag=FinalDamage> {DAMAGE}
        /// Item_Half_HP_Effect2
        /// HPが50%未満の時、<tag=Toughness> {TOUGHNESS}
        /// </summary>
        public static ModCharm HalfHP { get; } = ModCharmStatus.Create<Charm_HalfHP>("Half_HP", 3, CreateStatusGroup("DEFENSE", -2, -4, -6, -8))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_All_Ignore_Defense_Name
        /// リシュリューの高級ワイン
        /// Item_All_Ignore_Defense_FlavorText
        /// 何年たっても動物を惑わせる香り。
        /// Item_All_Ignore_Defense_Effect
        /// <tag=Defense>{DEFENSE}ごとに次の効果を獲得
        /// Item_All_Ignore_Defense_Effect2
        /// 全てのプレイヤーの防御貫通<sprite=\"Keyword\" name=IgnoreDefense> {IGNORE}
        /// </summary>
        public static ModCharm AllIgnoreDefense { get; } = ModCharmStatus.Create<Charm_AllIgnoreDefense>("All_Ignore_Defense", 4, CreateStatusGroup("DEFENSE", -4, -8, -12, -16, -20), CreateStatusGroup("FINAL_DAMAGE", 0, 2, 4, 8, 12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_Companion_Sacrifice_Name
        /// 裏サイン
        /// Item_Companion_Sacrifice_FlavorText
        /// 明かしてはならない相手との契約。
        /// Item_Companion_Sacrifice_Effect
        /// <tag=Defense>-50以下の時、\n<tag=FollowerDamage> {FOLLOWER1}
        /// Item_Companion_Sacrifice_Effect2
        /// <tag=Defense>-100以下の時、\n<tag=FollowerDamage> {FOLLOWER2}
        /// Item_Companion_Sacrifice_Effect3
        /// 死亡するダメージを受けた時、左の枠にある<tag=Follower>アーティファクトが破壊されることで、HPを{HEAL}回復し、一時的に無敵になる。
        /// </summary>
        public static ModCharm CompanionSacrifice { get; } = ModCharmStatus.Create<Charm_CompanionSacrifice>("Companion_Sacrifice", 4, CreateStatusGroup("DEFENSE", -4, -8, -12, -16, -20))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Companion).SetIsUniqueEffect().SetSimpleEffects(3).SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Drunk_Shadow_Name
        /// 黒い香車
        /// Item_Drunk_Shadow_FlavorText
        /// 黒く固まった一つの駒。
        /// Item_Drunk_Shadow_Effect
        /// 死亡するダメージを受ける時、必ず<tag=Evasion>する（クールタイム{COOLDOWN}秒、<tag=Evasion>が適用されます）
        /// </summary>
        public static ModCharm DrunkShadow { get; } = ModCharmStatus.Create<Charm_DrunkShadow>("Drunk_Shadow", 3, CreateStatusGroup("DEFENSE", -4, -8, -12, -20), CreateStatusGroup("PHYSICAL_DAMAGE", 2, 3, 5, 8), CreateStatusGroup("EVASION", 300, 500, 900, 1300))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Shadow).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Drunk_Ember_Name
        /// 汽車の玩具
        /// Item_Drunk_Ember_FlavorText
        /// 衝突するまで止まらないブレーキの壊れた汽車。
        /// Item_Drunk_Ember_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Defense>-1につき{PERCENT}の確率で<tag=Burn>を付与する（クールタイム0.5秒）
        /// Item_Drunk_Ember_Effect2
        /// 最大スタックまで<tag=Burn>デバフを付与した時、全ての<tag=Burn>デバフを解除して、火属性ダメージを与える（ダメージ：解除した<tag=Burn>デバフの数×負の<tag=Defense>{DAMAGE}）
        /// </summary>
        public static ModCharm DrunkEmber { get; } = ModCharmStatus.Create<Charm_DrunkEmber>("Drunk_Ember", 6, CreateStatusGroup("DEFENSE", -2, -4, -6, -8, -12, -16, -20), CreateStatusGroup("BURN_STACK", 1, 1, 1, 2, 2, 3, 3))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Ember).SetIsUniqueEffect().SetSimpleEffects(2).SetIsDual().SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_Drunk_Glacier_Name
        /// 雪の待ち針
        /// Item_Drunk_Glacier_FlavorText
        /// その雪だけは溶けることはない。
        /// Item_Drunk_Glacier_Effect
        /// <tag=Freeze>のスタン効果が無くなる
        /// Item_Drunk_Glacier_Effect2
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Frostbite>を付与する（クールタイム：{COOLDOWN}秒、<tag=Defense>-1につき1%早くなります）
        /// </summary>
        public static ModCharm DrunkGlacier { get; } = ModCharmStatus.Create<Charm_DrunkGlacier>("Drunk_Glacier", 5, CreateStatusGroup("DEFENSE", -3, -6, -9, -12, -16, -20), CreateStatusGroup("FREEZE_THRESHOLD", 0, 0, 1, 1, 1, 2))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Glacier).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Drunk_Dark_Cloud_Name
        /// 割れた電球
        /// Item_Drunk_Dark_Cloud_FlavorText
        /// 自ら壊れ自由になった光。
        /// Item_Drunk_Dark_Cloud_Effect2
        /// <tag=DarkCloud>の消費速度増幅 {SPEED}
        /// Item_Drunk_Dark_Cloud_Effect
        /// <tag=DarkCloud>発動時に<tag=Defense>-1ごとに追加消費 {SERIES}
        /// </summary>
        public static ModCharm DrunkDarkCloud { get; } = ModCharmStatus.Create<Charm_DrunkDarkCloud>("Drunk_Dark_Cloud", 2, CreateStatusGroup("DEFENSE", -10, -15, -20), CreateStatusGroup("DARK_CLOUD_SPEED", -500))
            .SetCategory(ItemCategories.Drunk, ItemCategories.DarkCloud).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_Drunk_Vitality_Name
        /// 経口輸血液
        /// Item_Drunk_Vitality_FlavorText
        /// 美味しい。
        /// Item_Drunk_Vitality_Effect
        /// {ITEM}の<tag=HP>回復量 {HEAL}
        /// Item_Drunk_Vitality_Effect2
        /// ダメージを受けた時、攻撃者のダメージの{PERCENT}を反射する
        /// </summary>
        public static ModCharm DrunkVitality { get; } = ModCharmStatus.Create<Charm_DrunkVitality>("Drunk_Vitality", 2, CreateStatusGroup("DEFENSE", -2, -5, -10), CreateStatusGroup("MAX_HP", 2, 5, 10))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Vitality).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_Drunk_Guardian_Name
        /// 存在の天秤
        /// Item_Drunk_Guardian_FlavorText
        /// 天使と悪魔が囁く。
        /// Item_Drunk_Guardian_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、追加の<tag=PhysicalDamage>を与える\n[ダメージ：{DAMAGE}（守護アーティファクトの数×酩酊アーティファクトの数{PERCENT} - <tag=Defense>の絶対値）]
        /// </summary>
        public static ModCharm DrunkGuardian { get; } = ModCharmStatus.Create<Charm_DrunkGuardian>("Drunk_Guardian", 3, CreateStatusGroup("FINAL_DAMAGE", 5, 10, 15, 20))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Guardian).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_Evasion_Curse_Name
        /// 暗黙の毒針
        /// Item_Evasion_Curse_FlavorText
        /// 声は出ない。
        /// Item_Evasion_Curse_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{POISON}の確率で<tag=Debuff_Poison>を付与する。（クールタイム{COOLDOWN}秒）
        /// Item_Evasion_Curse_Effect2
        /// <tag=Evasion>の発生率が<tag=Assasination>の発生率に変換される。（<tag=Assasination>発生率：{PERCENT}）
        /// </summary>
        public static ModCharm EvasionCurse { get; } = ModCharmStatus.Create<Charm_EvasionCurse>("Evasion_Curse", 2, CreateStatusGroup("EVASION", 100, 200, 400))
            .SetCategory(ItemCategories.Curse, ItemCategories.Shadow).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Random_Debuff_Name
        /// 赤黒いルーレット
        /// Item_Random_Debuff_FlavorText
        /// 塔の下から流れ着いた様々な思念が詰まった板。
        /// Item_Random_Debuff_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{PERCENT}の確率でランダムなデバフを付与する。（<tag=Luck>で確率が増加）
        /// </summary>
        public static ModCharm RandomDebuff { get; } = ModCharmStatus.Create<Charm_RandomDebuff>("Random_Debuff", 6, CreateStatusGroup("DEBUFF_DAMAGE", 10, 20, 30, 45, 70, 100, 150))
            .SetCategory(ItemCategories.Curse, ItemCategories.Fortune).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Dash_Flame_Sword_Name
        /// 過熱したエンジン
        /// Item_Dash_Flame_Sword_FlavorText
        /// 高い空に憧れた。
        /// Item_Dash_Flame_Sword_Effect
        /// ダッシュすると<tag=FlameSword>が発動する
        /// </summary>
        public static ModCharm DashFlameSword { get; } = ModCharmStatus.Create<Charm_DashFlameSword>("Dash_Flame_Sword", 4, CreateStatusGroup("FIRE_DAMAGE", 3, 6, 9, 12, 15), CreateStatusGroup("FLAME_SWORD_MAX", 0, 1, 2, 3, 5))
            .SetCategory(ItemCategories.SkySong, ItemCategories.FlameSword).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Planet_Stargaze_Name
        /// リリィの星図
        /// Item_Planet_Stargaze_FlavorText
        /// 光に込められた破れかけの形。
        /// Item_Planet_Stargaze_Effect
        /// 夜空のコンボ効果が<tag=WeaponAction_SpecialAttack>ではなく惑星の攻撃で発動する
        /// Item_Planet_Stargaze_Effect2
        /// 周囲8枠にある惑星の数だけこのアーティファクトの最大レベル {LEVEL}
        /// </summary>
        public static ModCharm PlanetStargaze { get; } = ModCharmStatus.Create<Charm_PlanetStargaze>("Planet_Stargaze", 8, CreateStatusGroup("PLANET_DAMAGE", 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 100, 120, 140, 160, 180, 200, 225, 250, 275, 300))
            .SetCategory(ItemCategories.Planet, ItemCategories.Stargaze).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Planet_Mystic_Name
        /// 夕焼けの星座
        /// Item_Planet_Mystic_FlavorText
        /// 赤くぼんやりと見える。
        /// Item_Planet_Mystic_Effect
        /// 神秘のコンボ効果の枠に置いた惑星を巨大化
        /// Item_Planet_Mystic_Effect2
        /// 神秘のコンボ効果の枠に置いた惑星のレベル合計1につき惑星攻撃速度 {SPEED}
        /// </summary>
        public static ModCharm PlanetMystic { get; } = ModCharmStatus.Create<Charm_PlanetMystic>("Planet_Mystic", 4, CreateStatusGroup("PLANET_DAMAGE", 2, 4, 6, 8, 10))
            .SetCategory(ItemCategories.Planet, ItemCategories.Mystic).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Guardian_Dark_Cloud_Name
        /// オーロラのヴェール
        /// Item_Guardian_Dark_Cloud_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Guardian_Dark_Cloud_Effect
        /// 攻撃を受けた時の無敵時間の<tag=DarkCloud>の消費速度 {PERCENT}
        /// </summary>
        public static ModCharm GuardianDarkCloud { get; } = ModCharmStatus.Create<Charm_GuardianDarkCloud>("Guardian_Dark_Cloud", 4, CreateStatusGroup("DARK_CLOUD_DAMAGE", 10, 25, 40, 60, 90), CreateStatusGroup("DEFENSE", 2, 3, 4, 6, 9))
            .SetCategory(ItemCategories.Guardian, ItemCategories.DarkCloud).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Reddew_Magic_Execution_Name
        /// 赤い山頂
        /// Item_Reddew_Magic_Execution_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Reddew_Magic_Execution_Effect
        /// <tag=MagicExecution>によって{ITEM}が発動した時、最も高い属性値の{PERCENT}の追加ダメージを{COUNT}回与える
        /// </summary>
        public static ModCharm ReddewMagicExecution { get; } = ModCharmStatus.Create<Charm_ReddewMagicExecution>("Reddew_Magic_Execution", 2, CreateStatusGroup("CRITICAL", 300, 600, 1000), CreateStatusGroup("HIGHEST_ELEMENTAL_DAMAGE", 1, 3, 5))
            .SetCategory(ItemCategories.Precision, ItemCategories.Elemental).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetDamageId();
        /// <summary>
        /// Item_Magitech_Flame_Sword_Name
        /// 燃える日時計
        /// Item_Magitech_Flame_Sword_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Magitech_Flame_Sword_Effect
        /// <tag=WeaponAction_DirectAttack>や<tag=Magic>によって<tag=FlameSword>が発動しなくなる。かわりに、<tag=Electric>ダメージを与えると<tag=FlameSword>を最大<tag=Electric>スタックの分だけ投げる
        /// </summary>
        public static ModCharm MagitechFlameSword { get; } = ModCharmStatus.Create<Charm_MagitechFlameSword>("Magitech_Flame_Sword", 5, CreateStatusGroup("ELECTRIC_DAMAGE", 25, 30, 40, 55, 75, 100), CreateStatusGroup("FIRE_DAMAGE", 2, 3, 5, 7, 10, 13))
            .SetCategory(ItemCategories.Magitech, ItemCategories.FlameSword).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Magitech_Frost_Relic_Name
        /// 凍える氷磁石
        /// Item_Magitech_Frost_Relic_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Magitech_Frost_Relic_Effect
        /// <tag=FrostRelic>が命中した時、{PERCENT}の確率で<tag=Electric>のショックを即座に発動させる。<tag=Electric>のショックが発動した時、「雹の手」バフを獲得
        /// Item_Magitech_Frost_Relic_Effect2
        /// 雹の手：{BUFF}秒の間、<tag=IceDamage>が2増加（最大20スタック）
        /// </summary>
        public static ModCharm MagitechFrostRelic { get; } = ModCharmStatus.Create<Charm_MagitechFrostRelic>("Magitech_Frost_Relic", 5, CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 5, 7, 10, 13))
            .SetCategory(ItemCategories.Magitech, ItemCategories.Frost).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_Auto_Magic_Dark_Cloud_Name
        /// 巨大実験台
        /// Item_Auto_Magic_Dark_Cloud_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Auto_Magic_Dark_Cloud_Effect
        /// <tag=MP>を消費するかわりに<tag=DarkCloud>を{CLOUD}消費して、上の枠にある<tag=Magic>を{COOLDOWN}秒遅れて自動発動する
        /// </summary>
        public static ModCharm AutoMagicDarkCloud { get; } = ModCharmStatus.Create<Charm_AutoMagicDarkCloud>("Auto_Magic_Dark_Cloud", 5, CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 10, 20, 30, 40, 60, 80))
            .SetCategory(ItemCategories.Academy, ItemCategories.DarkCloud).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_More_Shop_Name
        /// 行商人の手形
        /// Item_More_Shop_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyUncommon { get; } = ModCharmStatus.Create("More_Shop", 2, CreateStatusGroup("AdditionalShop".ToSephiriaId(), 1, 1, 2), CreateStatusGroup("AdditionalMoney".ToSephiriaId(), 500, 1000, 2000))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Uncommon).SetIsUniqueEffect();
        /// <summary>
        /// Item_More_Replenishment_Name
        /// 勇者優待券
        /// Item_More_Replenishment_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyRare { get; } = ModCharmStatus.Create("More_Replenishment", 3, CreateStatusGroup("NEGOTIATION", 2, 5, 12, 20), CreateStatusGroup("ReplenishmentCharm".ToSephiriaId(), 0, 1, 1, 2), CreateStatusGroup("ReplenishmentTablet".ToSephiriaId(), 0, 0, 1, 1))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_More_Shop_Legendary_Name
        /// 名だたる鑑定書
        /// Item_More_Shop_Legendary_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyLegendary { get; } = ModCharmStatus.Create("More_Shop_Legendary", 5, CreateStatusGroup("AdditionalShopLegendary".ToSephiriaId(), 1), CreateStatusGroup("AdditionalShopInventory".ToSephiriaId(), 20, 30, 40, 60, 80, 100))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Legend).SetIsUniqueEffect();
        /// <summary>
        /// Item_Add_Inventory_Name
        /// バッグの拡張キット
        /// Item_Add_Inventory_FlavorText
        /// 希少な魔法の布を使用した高級品。
        /// Item_Add_Inventory_Effect
        /// このアーティファクトを消費して、バッグの枠を{COUNT}拡張します
        /// </summary>
        public static ModCharm AddInventory { get; } = ModCharm.Create<Charm_AddInventory>("Add_Inventory", 0, true).SetActiveType(EItemActiveType.Hidden)
            .SetCategory().SetSimpleEffect().SetRarity(EItemRarity.Legend);

        /// <summary>
        /// Item_Sacrifice_Fire_Name
        /// 炎の儀式
        /// Item_Sacrifice_Fire_FlavorText
        /// 神に捧げる儀式。
        /// Item_Sacrifice_Fire_Effect
        /// 儀式：<tag=FireDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeFire { get; } = ModCharmSacrificeDamage.Create("Sacrifice_Fire", () => SacrificeFireReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("DEFENSE", -20))
            .SetCategory(ItemCategories.Ember, ItemCategories.FlameSword).SetSimpleEffect().SetElementalType(EDamageElementalType.Fire);
        /// <summary>
        /// Item_Sacrifice_Fire_Reward_Name
        /// 熾天使の聖剣
        /// Item_Sacrifice_Fire_Reward_FlavorText
        /// 燃え尽きた剣。
        /// Item_Sacrifice_Fire_Reward_Effect
        /// <tag=FlameSword>が<tag=Burn>スタックの数だけ追加発動する
        /// </summary>
        public static ModCharm SacrificeFireReward { get; } = ModCharmStatus.Create<Charm_EmberFlameSword>("Sacrifice_Fire_Reward", 5, CreateStatusGroup("FIRE_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("FLAME_SWORD_MAX", 1, 2, 3, 4, 5, 6))
            .SetCategory(ItemCategories.Ember, ItemCategories.FlameSword).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();
        /// <summary>
        /// Item_Sacrifice_Ice_Name
        /// 氷の儀式
        /// Item_Sacrifice_Ice_FlavorText
        /// 神に捧げる儀式。
        /// Item_Sacrifice_Ice_Effect
        /// 儀式：<tag=IceDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeIce { get; } = ModCharmSacrificeDamage.Create("Sacrifice_Ice", () => SacrificeIceReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("ATTACK_SPEED", -20))
            .SetCategory(ItemCategories.Glacier, ItemCategories.Frost).SetSimpleEffect().SetElementalType(EDamageElementalType.Ice);
        /// <summary>
        /// Item_Sacrifice_Ice_Reward_Name
        /// 極氷の曲剣
        /// Item_Sacrifice_Ice_Reward_FlavorText
        /// 凍てついた剣。
        /// Item_Sacrifice_Ice_Reward_Effect
        /// <tag=Freeze>発動時に<tag=FrostRelic>のチャージを加速させる
        /// </summary>
        public static ModCharm SacrificeIceReward { get; } = ModCharmStatus.Create<Charm_GlacierFrost>("Sacrifice_Ice_Reward", 5, CreateStatusGroup("ICE_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("CHARGING_CHARM_BONUS", 15, 30, 45, 60, 75, 90))
            .SetCategory(ItemCategories.Glacier, ItemCategories.Frost).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();
        /// <summary>
        /// Item_Sacrifice_Lightning_Name
        /// 雷の儀式
        /// Item_Sacrifice_Lightning_FlavorText
        /// 神に捧げる儀式。
        /// Item_Sacrifice_Lightning_Effect
        /// 儀式：<tag=LightningDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeLightning { get; } = ModCharmSacrificeDamage.Create("Sacrifice_Lightning", () => SacrificeLightningReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("CRITICAL", -2000))
            .SetCategory(ItemCategories.Magitech, ItemCategories.DarkCloud).SetSimpleEffect().SetElementalType(EDamageElementalType.Lightning);
        /// <summary>
        /// Item_Sacrifice_Lightning_Reward_Name
        /// 迅雷の直剣
        /// Item_Sacrifice_Lightning_Reward_FlavorText
        /// いなずまの剣。
        /// Item_Sacrifice_Lightning_Reward_Effect
        /// <tag=DarkCloud>の稲妻が命中した敵に<tag=Electric>デバフを付与する
        /// </summary>
        public static ModCharm SacrificeLightningReward { get; } = ModCharmStatus.Create<Charm_MagitechDarkCloud>("Sacrifice_Lightning_Reward", 5, CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("ELECTRIC_STACK", 1, 1, 2, 2, 3, 3))
            .SetCategory(ItemCategories.Magitech, ItemCategories.DarkCloud).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();

        /// <summary>
        /// ItemCategory_Vitality
        /// 生命
        /// </summary>
        public static ModComboEffect Vitality { get; } = ModComboEffect.Create("Vitality").SetStats(CreateComboStat(3, "MAX_HP/10"), CreateComboStat(6, "MAX_HP/10"), CreateComboStat(9, "MAX_HP/10", "FINAL_HP/10"));

        /// <summary>
        /// ItemCategory_Stargaze
        /// 夜空
        /// </summary>
        public static ModComboEffect Stargaze { get; } = ModComboEffect.Create<ComboEffect_Stargaze>("Stargaze").SetDamageIdAbility().SetDefaultEffect().SetStats(
            CreateComboStat(4, "STARGAZE_LEVEL/1"), CreateComboStat(6, "STARGAZE_LEVEL/1"), 
            CreateComboStat(8, "STARGAZE_LEVEL/1"), CreateComboStat(10, "STARGAZE_LEVEL/1"));
        /// <summary>
        /// ItemCategory_SkySong
        /// 空の歌
        /// </summary>
        public static ModComboEffect SkySong { get; } = ModComboEffect.Create("SkySong").SetStats(CreateComboStat(2, "DASH_COUNT/1", "DASH_RECOVERY_SPEED/20"), CreateComboStat(4, "DASH_ATTACK_DAMAGE/20"),
            CreateComboStat(6, "DASH_ATTACK_DAMAGE/30"), CreateComboStat(8, "DASH_ATTACK_DAMAGE/40"), CreateComboStat(10, "DASH_INVINCIBLE_TIME_BONUS/100", "DASH_RECOVERY_SPEED/20"));
        /// <summary>
        /// ItemCategory_Drunk
        /// 酩酊
        /// </summary>
        public static ModComboEffect Drunk { get; } = ModComboEffect.Create<ComboEffect_Drunk>("Drunk").SetStats(CreateComboStat(4, "DEFENSE/-10"), CreateComboStat(6, "DEFENSE/-10"), 
            CreateComboStat(8, "DEFENSE/-20"), CreateComboStat(10, "FINAL_DAMAGE/30")).SetDefaultEffect();
        /// <summary>
        /// ItemCategory_Fortune
        /// 運命
        /// </summary>
        public static ModComboEffect Fortune { get; } = ModComboEffect.Create("Fortune").SetStats(CreateComboStat(2, "LUCK/4"), CreateComboStat(4, "LUCK/8"));
        /// <summary>
        /// ItemCategory_Grimoire
        /// 魔導書
        /// ComboEffectDefault_Grimoire_Effect2
        /// +3 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect4
        /// +6 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect6
        /// +9 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect8
        /// +12 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect10
        /// +15 火・氷・雷属性ダメージ
        /// </summary>
        public static ModComboEffect Grimoire { get; } = ModComboEffect.Create("Grimoire").SetStats(CreateComboStatThreeDamage(2, "Grimoire", 3), CreateComboStatThreeDamage(4, "Grimoire", 6),
            CreateComboStatThreeDamage(6, "Grimoire", 9), CreateComboStatThreeDamage(8, "Grimoire", 12), CreateComboStatThreeDamage(10, "Grimoire", 15));

        /// <summary>
        /// EffectHUD_Physical_Damage_Buff_Name
        /// 暗影の凶刃
        /// EffectHUD_Physical_Damage_Buff_FlavorText
        /// 物理ダメージ増加（最大4スタック）
        /// </summary>
        public static ModEffectHUD EffectPhysicalDamageBuff { get; } = ModEffectHUD.CreateStackEffectHUD("Physical_Damage_Buff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance PhysicalDamageBuff { get; } = CreateBuff("PhysicalDamageBuff", "PhysicalDamageBuff", 4, CreateBuffStatus("PHYSICAL_DAMAGE", 5))
            .SetDefaultDuration(8f);
        /// <summary>
        /// EffectHUD_Magitech_Frost_Relic_Buff_Name
        /// 雹の手
        /// EffectHUD_Magitech_Frost_Relic_Buff_FlavorText
        /// 氷属性ダメージ増加（最大20スタック）
        /// </summary>
        public static ModEffectHUD EffectMagitechFrostRelicBuff { get; } = ModEffectHUD.CreateStackEffectHUD("Magitech_Frost_Relic_Buff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance MagitechFrostRelicBuff { get; } = CreateBuff("MagitechFrostRelicBuff", "MagitechFrostRelicBuff", 20, CreateBuffStatus("ICE_DAMAGE", 2))
            .SetDefaultDuration(Charm_MagitechFrostRelic.BuffDuration);

        /// <summary>
        /// EffectHUD_Stargaze_Tablet_Name
        /// 星見の石版
        /// EffectHUD_Stargaze_Tablet_FlavorText
        /// 破壊された石版の欠片。
        /// </summary>
        public static ModEffectHUD EffectStargazeTablet { get; } = ModEffectHUD.CreateStackEffectHUD("Stargaze_Tablet", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_Create_Stone_Tablet_Name
        /// 流れ星の結晶
        /// EffectHUD_Create_Stone_Tablet_FlavorText
        /// 倒した敵の数。
        /// </summary>
        public static ModEffectHUD EffectCreateStoneTablet { get; } = ModEffectHUD.CreateStackEffectHUD("Create_Stone_Tablet", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_Copy_Academy_Name
        /// 原典
        /// EffectHUD_Copy_Academy_FlavorText
        /// <tag=Grimoire>を一定回数使用するとアカデミーアーティファクトを複製する
        /// </summary>
        public static ModEffectHUD EffectCopyAcademy { get; } = ModEffectHUD.CreateStackEffectHUD("Copy_Academy", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_Electric_Stun_Name
        /// ビリビリクリームクロワッサン
        /// EffectHUD_Electric_Stun_FlavorText
        /// <tag=Electric>が付与されていない敵に<tag=LightningDamage>を与えた時の気絶確率
        /// </summary>
        public static ModEffectHUD EffectElectricStun { get; } = ModEffectHUD.CreateStackEffectHUD("Electric_Stun", UI_EffectHUD_Basic.EEffectType.Boon);

        /// <summary>
        /// Status_StargazeLevel_Name
        /// 夜空アーティファクト最大レベル
        /// Status_StargazeLevel_Description
        /// インベントリにある夜空アーティファクトの最大レベルが増加します
        /// </summary>
        public static ModCustomStatus StargazeLevel { get; } = ModCustomStatus.CreateStatus("StargazeLevel");
        /// <summary>
        /// Status_InvLevel_Name
        /// カジノチップの最大レベル
        /// Status_InvLevel_Description
        /// インベントリにあるカジノチップの最大レベルが増加します
        /// </summary>
        public static ModCustomStatus InvLevel { get; } = ModCustomStatus.CreateStatus("InvLevel");
        /// <summary>
        /// Status_AddGrimoire_Name
        /// 
        /// Status_AddGrimoire_Description
        /// 
        /// </summary>
        public static ModCustomStatus AddGrimoire { get; } = ModCustomStatus.CreateStatus("AddGrimoire");
        /// <summary>
        /// Status_AdditionalShop_Name
        /// ステージを移動した時、そのステージにいる商人の品数が{VALUE}増加
        /// Status_AdditionalShop_Description
        /// 商人が持つアイテムの数が増加します。
        /// </summary>
        public static ModCustomStatus AdditionalShop { get; } = ModCustomStatus.CreateStatus("AdditionalShop").SetIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_AdditionalShopLegendary_Name
        /// ステージを移動した時、そのステージにいる商人に伝説アーティファクトを{VALUE}個追加
        /// Status_AdditionalShopLegendary_Description
        /// 商人が持つ伝説アーティファクトの数が増加します
        /// </summary>
        public static ModCustomStatus AdditionalShopLegendary { get; } = ModCustomStatus.CreateStatus("AdditionalShopLegendary").SetIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_AdditionalShopInventory_Name
        /// ステージを移動した時、そのステージにいる商人がバッグの枠拡張キットを売る確率
        /// Status_AdditionalShopInventory_Description
        /// 確率で商人がバッグの枠拡張キットを売ります
        /// </summary>
        public static ModCustomStatus AdditionalShopInventory { get; } = ModCustomStatus.CreateStatus("AdditionalShopInventory").SetSymbol("%");
        /// <summary>
        /// Status_AdditionalMoney_Name
        /// ステージを移動した時、そのステージにいる<tag=MerchantLeaf>が{VALUE}増加
        /// Status_AdditionalMoney_Description
        /// 商人が持つ<tag=Leaf>が増加します。
        /// </summary>
        public static ModCustomStatus AdditionalMoney { get; } = ModCustomStatus.CreateStatus("AdditionalMoney").SetIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_ReplenishmentCharm_Name
        /// サファイアを使った時に商人が入荷するアーティファクトの数
        /// Status_ReplenishmentCharm_Description
        /// サファイアを使って商人に入荷させた時のアーティファクトの数が増加します
        /// </summary>
        public static ModCustomStatus ReplenishmentCharm { get; } = ModCustomStatus.CreateStatus("ReplenishmentCharm");
        /// <summary>
        /// Status_ReplenishmentTablet_Name
        /// サファイアを使った時に商人が入荷する石版の数
        /// Status_ReplenishmentTablet_Description
        /// サファイアを使って商人に入荷させた時の石版の数が増加します
        /// </summary>
        public static ModCustomStatus ReplenishmentTablet { get; } = ModCustomStatus.CreateStatus("ReplenishmentTablet");
        /// <summary>
        /// Status_MagicExecution_Name
        /// 天罰
        /// Status_MagicExecution_Description
        /// 最も高い属性ダメージまたは物理ダメージを、最も低い属性ダメージまたは物理ダメージで割った値を掛けた<tag=Elemental_Chaos>ダメージを与えます（最大8倍、クリティカル扱いされます）
        /// </summary>
        public static ModKeyword MagicExecution { get; } = ModKeyword.CreateKeyword("MagicExecution").SetOriginal("MagicDamageBonus").SetConnectedDetailEntities("Elemental_Chaos");
        /// <summary>
        /// Status_BinaryPlanet_Name
        /// 連星
        /// Status_BinaryPlanet_Description
        /// 惑星が3回攻撃します
        /// </summary>
        public static ModKeyword BinaryPlanet { get; } = ModKeyword.CreateKeyword("BinaryPlanet").SetTextColor(new Color(0.7f, 0.4f, 0.1f)).SetKeywordImage(() => CustomSpriteAsset.BinaryPlanet);
        /// <summary>
        /// Status_Assasination_Name
        /// 暗閃
        /// Status_Assasination_Description
        /// <tag=WeaponAction_DirectAttack>が命中した時、確率で対象に付与されたデバフ1つにつき、<tag=MaxHP>の4%の追加ダメージを与えます
        /// </summary>
        public static ModKeyword Assasination { get; } = ModKeyword.CreateKeyword("Assasination").SetTextColor(new Color(0.9f, 0.1f, 0.1f)).SetKeywordImage(() => CustomSpriteAsset.Assasination);
        /// <summary>
        /// Status_MerchantLeaf_Name
        /// 商人のリーフ
        /// Status_MerchantLeaf_Description
        /// 商人が持つ<tag=Leaf>です
        /// </summary>
        public static ModKeyword MerchantLeaf { get; } = ModKeyword.CreateKeyword("MerchantLeaf").SetDisplayDetails().SetKeywordImage(() => CustomSpriteAsset.MerchantLeaf);


        /// <summary>
        /// Miracle_FlameSword_Name
        /// 炎の鍛冶屋
        /// </summary>
        public static ModMiracle FlameSword { get; } = ModMiracleStatus.Create("FlameSword", CreatePositiveStat("FLAME_SWORD_MAX/3"), CreatePositiveStat("FLAME_SWORD_DAMAGE/10"))
            .SetCategories(ItemCategories.FlameSword);
        /// <summary>
        /// Miracle_Magitech_Name
        /// エンジニア
        /// </summary>
        public static ModMiracle Magitech { get; } = ModMiracleStatus.Create("Magitech", CreatePositiveStat("LIGHTNING_DAMAGE/6"), CreatePositiveStat("ELECTRIC_STACK/1"), CreateNegativeStat("CRITICAL/-1600"))
            .SetCategories(ItemCategories.Magitech);
        /// <summary>
        /// Miracle_IgnoreDefence_Name
        /// 解体屋
        /// </summary>
        public static ModMiracle IgnoreDefence { get; } = ModMiracleStatus.Create("IgnoreDefence", CreatePositiveStat("IGNORE_DEFENSE/20"), CreatePositiveStat("BASIC_ATTACK_DAMAGE/10"))
            .SetCategories(ItemCategories.Sturdy).SetNotGiveItem().SetNotAutoGenerateEffectString(2, EEffectType.Positive, true);
        /// <summary>
        /// Miracle_Buffer_Name
        /// VTuber
        /// </summary>
        public static ModMiracle Buffer { get; } = ModMiracleStatus.Create("Buffer", CreatePositiveStat("BUFF_DURATION/100"), CreatePositiveStat("MAX_MP/20"))
            .SetCategories(ItemCategories.Academy);
        /// <summary>
        /// Miracle_ExtraLife_Name
        /// アンデッド
        /// </summary>
        public static ModMiracle ExtraLife { get; } = ModMiracleStatus.Create("ExtraLife", CreatePositiveStat("EXTRA_LIFE/1"), CreatePositiveStat("TOUGHNESS/5"), CreateNegativeStat("MAX_HP/-20"))
            .SetCategories();
        /// <summary>
        /// Miracle_HpSteal_Name
        /// 看護師
        /// </summary>
        public static ModMiracle HpSteal { get; } = ModMiracleStatus.Create("HpSteal", CreatePositiveStat("HP_STEAL/3"), CreateNegativeStat("CRITICAL_DAMAGE_RATE/-40"))
            .SetCategories();
        /// <summary>
        /// Miracle_True_Name
        /// 占い師
        /// </summary>
        public static ModMiracle True { get; } = ModMiracleStatus.Create("True", CreatePositiveStat("TRUE_DAMAGE/12"), CreateNegativeStat("FINAL_WEAPONDAMAGE/-30"))
            .SetCategories();

        /// <summary>
        /// Weapon_SwordAndShield_Fire_T2_Name
        /// 熱い鼓動
        /// WeaponAddon_SwordAndShield_Fire_T2_Effect
        /// <tag=FireDamage>が{VAL0}増加します。
        /// </summary>
        public static ModWeapon SwordShieldFire { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T2", 1016, 0).SetStandardEnhancements(GetFirstWeaponId() + 1, GetFirstWeaponId() + 2).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = [CreateWeaponStat(ECustomStat.FireDamage, 5)];
            }
        });
        /// <summary>
        /// Weapon_SwordAndShield_Fire_T3_Burn_Name
        /// カゲロウ
        /// WeaponAddon_SwordAndShield_Fire_T3_Burn_Effect
        /// <tag=BurnStack>が{VAL0}増加し、<tag=BurnSpeed>が{VAL1}%増加します。
        /// </summary>
        public static ModWeapon SwordShieldFireBurn { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T3_Burn", 1016).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = [CreateWeaponStat(ECustomStat.FireDamage, 5)];

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_StatusUnsafe>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T3_Burn_Effect");
                @unsafe.status = [CreateWeaponStat("BURNSTACK", 2), CreateWeaponStat("BURNSPEED", 100)];
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_SwordAndShield_Fire_T3_Critical_Name
        /// 気炎鳳凰
        /// WeaponAddon_SwordAndShield_Fire_T3_Critical_Effect
        /// <tag=CriticalChance>が{VAL0}%増加します。クリティカルヒット時、アーティファクト鳳凰の羽のクールダウンが{COOLDOWN}秒減少します。
        /// </summary>
        public static ModWeapon SwordShieldFireCritical { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T3_Critical", 1016).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = [CreateWeaponStat(ECustomStat.FireDamage, 5)];

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_CriticalFire>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T3_Critical_Effect");
                @unsafe.status = [CreateWeaponStat("CRITICAL", 3000)];
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_Dagger_Evasion_T3_Another_Name
        /// 暗影の凶刃
        /// WeaponAddon_Dagger_Evasion_T3_Another_Effect
        /// <tag=WeaponAction_Fury>による無敵効果は<tag=Evasion>扱いされます。<tag=Evasion>すると、8秒間<tag=PhysicalDamage>が5増加します。（最大4スタック）
        /// </summary>
        public static ModWeapon DaggerEvasionAnother { get; } = ModWeapon.CreateWeapon("Dagger_Evasion_T3_Another", 1208, 1208).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_CounterEvasion>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Evasion_T3_Another_Effect");
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_Dagger_Fury_T3_MP_Name
        /// 荒れ狂う知識
        /// WeaponAddon_Dagger_Fury_T3_MP_Effect
        /// <tag=MPSkillDamage>が20%増加します。<tag=WeaponAction_Fury>を使用すると、<tag=MP>が20%回復します。
        /// </summary>
        public static ModWeapon DaggerFuryMP { get; } = ModWeapon.CreateWeapon("Dagger_Fury_T3_MP", 28, 23).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonDagger_PassFury>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_FuryMP>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Fury_T3_MP_Effect");
                @unsafe.status = [CreateWeaponStat("MP_SKILL_DAMAGE", 20)];
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
            if (main.gameObject.TryGetComponent<WeaponAddonDagger_DoubleFury>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        });

        /// <summary>
        /// Weapon_Dagger_Flame_T3_Fury_Name
        /// 打ち焦がす爪
        /// WeaponAddon_Dagger_Flame_T3_Fury_Effect
        /// <tag=FireDamage>を10回与えると、<tag=WeaponAction_Trance>を獲得します。（<tag=WeaponAction_Trance>は最大2回まで充電可能）
        public static ModWeapon DaggerFlameFury { get; } = ModWeapon.CreateWeapon("Dagger_Flame_T3_Fury", 1202, 1200).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_FireFury>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Flame_T3_Fury_Effect");
                @unsafe.parent = status.parent;

                if(main is WeaponSimple_Dagger dagger)
                {
                    dagger.maxFury = 2;
                }

                main.addons = [status, @unsafe];
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_SpecialAttackDebuff>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        }).SetBladeSprite(Vector3.zero).SetHeadSprite();
        /// <summary>
        /// Weapon_Katana_Ice_T3_Flame_Name
        /// ソリス・フロスト
        /// WeaponAddon_Katana_Ice_T3_Flame_Effect
        /// <tag=WeaponAction_DirectAttack>時、対象に<tag=FireDamage>の40%分の追加ダメージを与えます。\n<tag=FrostRelic>でダメージを与えると、<tag=FlameSword>を投げます。
        public static ModWeapon KatanaIceFlame { get; } = ModWeaponKatana.CreateKatana("Katana_Ice_T3_Flame", 421, 419).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonKatana_Deflecting>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonKatana_FrostFlameSword>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Katana_Ice_T3_Flame_Effect");
                @unsafe.additionalDamagePercent = 40;
                @unsafe.elementalType = EDamageElementalType.Fire;
                @unsafe.statId = ECustomStat.FireDamage;
                @unsafe.damageId = "Weapon_AdditionalElementalDamage_Fire";
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
            if (main.gameObject.TryGetComponent<WeaponAddonKatana_Nastrond>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        });

        /// <summary>
        /// Weapon_Crossbow_Planet_T2_Name
        /// 巨大ボーガン：望遠レンズ
        /// WeaponAddon_Crossbow_Planet_T2_Effect
        /// <tag=PlanetDamage>が12%増加します。
        public static ModWeapon CrossbowPlanet { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T2", 106, 100).SetStandardEnhancements(14008, 14009, 14010).SetMainPrefabModifier(main =>
        {
            if(main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 11;
                crossbow.defaultMagazineCount = 2;
                crossbow.reloadTime = 3;
                crossbow.fireIntervalTimer.time = 1f / 4;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                //var @unsafe = main.gameObject.AddComponent<WeaponAddonKatana_FrostFlameSword>();


                //@unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");
                //@unsafe.additionalDamagePercent = 40;
                //@unsafe.elementalType = EDamageElementalType.Fire;
                //@unsafe.statId = ECustomStat.FireDamage;
                //@unsafe.damageId = "Weapon_AdditionalElementalDamage_Fire";
                //@unsafe.parent = status.parent;

                //main.addons = [status, @unsafe];
                status.status = [CreateWeaponStat("PLANETDAMAGE", 12)];
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");
                main.addons = [status];
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Weapon_Name
        /// 巨大ボーガン：電波収束
        /// WeaponAddon_Crossbow_Planet_T3_Weapon_Effect
        /// 惑星がダメージを与えるごとに<tag=FinalWeaponDamage>が積み重なり、最大で50%増加します。
        public static ModWeapon CrossbowPlanetWeapon { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Weapon", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 9;
                crossbow.defaultMagazineCount = 2;
                crossbow.reloadTime = 2;
                crossbow.fireIntervalTimer.time = 1f / 4;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = [CreateWeaponStat("PLANETDAMAGE", 12)];
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_PlanetWeaponDamage>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Weapon_Effect");
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Attack_Name
        /// 巨大ボーガン：超新星
        /// WeaponAddon_Crossbow_Planet_T3_Attack_Effect
        /// <tag=WeaponAction_BaiscAttack>時に5%の確率で惑星が即時攻撃します。
        public static ModWeapon CrossbowPlanetAttack { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Attack", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 16;
                crossbow.defaultMagazineCount = 1;
                crossbow.reloadTime = 2.4f;
                crossbow.fireIntervalTimer.time = 1f / 5.5f;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = [CreateWeaponStat("PLANETDAMAGE", 12)];
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_DirectFirePlanet>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Attack_Effect");
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Binary_Name
        /// 巨大ボーガン：引力形成
        /// WeaponAddon_Crossbow_Planet_T3_Binary_Effect
        /// 惑星の<tag=CriticalChance>が<tag=BinaryPlanet>の発生率に変換されます。
        public static ModWeapon CrossbowPlanetBinary { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Binary", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 6;
                crossbow.defaultMagazineCount = 3;
                crossbow.reloadTime = 2f;
                crossbow.fireIntervalTimer.time = 1f / 3f;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = [CreateWeaponStat("PLANETDAMAGE", 12)];
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_BinaryPlanet>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Binary_Effect");
                @unsafe.parent = status.parent;

                main.addons = [status, @unsafe];
            }
        });
        /// <summary>
        /// Weapon_GreatSword_Fire_T3_FlameSwordRange_Name
        /// ソリス・レアノ
        /// WeaponAddon_GreatSword_Fire_T3_FlameSwordRange_Effect
        /// <tag=WeaponAction_Sweep>をした時、すべての<tag=FlameSword>を失う代わりに、12個の<tag=FlameSword>を生成して周囲に投げます。
        /*
        public static ModWeapon GreatSwordFlameSwordRange { get; } = ModWeapon.CreateWeapon("GreatSword_Fire_T3_FlameSwordRange", 1124, 1122).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var additional))
            {

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_SpecialFlameSword>();
                
                //@unsafe.status = [CreateWeaponStat("FLAME_SWORD_MAX", -4)];
                @unsafe.effectText = new LocalizedString("WeaponAddon_GreatSword_Fire_T3_FlameSwordRange_Effect");
                @unsafe.parent = additional.parent;

                main.addons = [additional, @unsafe];
            }
        }).SetBladeSprite().SetBladeUnlitSprite();*/

        /// <summary>
        /// Passive_Grimoire_Name
        /// 追憶
        /// Passive_Grimoire_Description
        /// 魔法に使う様々な記憶を思い出します。
        /// Passive_Grimoire_Effect_LV5
        /// <tag=CooldownRecovery>が+30%増加します。
        /// Passive_Grimoire_Effect_LV10
        /// 全ての<tag=Magic>に魔導書コンボが追加されます。MP再生が+4増加します。
        /// Passive_Grimoire_Effect_LV20
        /// <tag=Magic>ダメージが+20%増加し、<tag=Magic>クリティカル確率が+50%増加します。
        /// </summary>
        public static ModPassive GrimoirePassive { get; } = ModPassive.CreatePassive("Grimoire", new Color32(66, 152, 245, byte.MaxValue), "MAX_MP/2")
            .CreatePerk(EPassivePerkLv.lv5, "CooldownRecovery").SetPerkSupplierStatus("COOLDOWN_RECOVERY_SPEED/30").Parent
            .CreatePerk(EPassivePerkLv.lv10, "AddGrimoire").SetPerkSupplierStatus("ADD_GRIMOIRE/1", "MP_REGEN/4").Parent
            .CreatePerk(EPassivePerkLv.lv20, "AddMagicDamage").SetPerkSupplierStatus("MAGIC_DAMAGE_BONUS/20", "MAGIC_CRITICAL/5000").Parent;

        public static Sprite IconInWorldPotion { get; internal set; }
        public static Sprite IconInWorldCharm { get; internal set; }
        public static Sprite IconInWorldTablet { get; internal set; }

        public static EventReference DefaultEnableSound { get; internal set; }
        public static Charm_StatusInstance.StatusGroup CreateStatusGroup(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values };
        }
        public static Charm_StatusInstance.StatusGroup CreateStatusGroupHide(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values, hideIfStatValueIsZero = true };
        }
        public static ComboEffectBase.ComboStat CreateComboStat(int count, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status };
        }
        public static ComboEffectBase.ComboStat CreateComboStatReplaceText(int count, string id, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status, replaceEffectText = true, replaceEffectTextString = new LocalizedString($"ComboEffectDefault_{id}_Effect{count}") };
        }
        public static ComboEffectBase.ComboStat CreateComboStatThreeDamage(int count, string id, int status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = ["FIRE_DAMAGE/" + status, "ICE_DAMAGE/" + status, "LIGHTNING_DAMAGE/" + status],
                replaceEffectText = true, replaceEffectTextString = new LocalizedString($"ComboEffectDefault_{id}_Effect{count}") };
        }
        public static Miracle_StatusInstance.StatInfo CreatePositiveStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Positive, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNegativeStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Negative, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNeutralStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Neutral, status = status };
        }
        public static WeaponAddonCommon_Status.Stat CreateWeaponStat(ECustomStat statId, int value)
        {
            return new WeaponAddonCommon_Status.Stat() { statId = statId, value = value };
        }
        public static WeaponAddonCommon_StatusUnsafe.Stat CreateWeaponStat(string statId, int value)
        {
            return new WeaponAddonCommon_StatusUnsafe.Stat() { statId = statId, value = value };
        }
        public static void Init()
        {
            IconInWorldCharm = SpriteLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_Charm");
            IconInWorldTablet = SpriteLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_TabletStone");
            IconInWorldPotion = SpriteLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_All");

            var type = typeof(Data);
            var pros = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType.IsSubclassOf(typeof(ModItem)));
            int id = GetFirstModId();
            uint assetId = GetFirstAssetId();
            foreach (var pro in pros)
            {
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Item: " + pro.Name);
                var moditem = pro.GetValue(type) as ModItem;
                moditem.Init(id++, assetId++);
                All.Add(moditem);
            }
            AllResourcePrefabNames = new();
            foreach (var item in All)
            {
                //Melon<Core>.Logger.Msg("LateInit Item: " + item.Name);
                item.LateInit();
                AllResourcePrefabNames.Add(item.ResourcePrefab.name);
            }
            //AllResourcePrefabNames = All.Select(x => x.ResourcePrefab.name).ToList();


            var pros2 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModComboEffect) || p.PropertyType.IsSubclassOf(typeof(ModComboEffect)));
            foreach (var pro in pros2)
            {
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Category: " + pro.Name);
                var moditem = pro.GetValue(type) as ModComboEffect;
                moditem.Init(assetId++);
                Combos.Add(moditem);
            }

            var pros3 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModEffectHUD) || p.PropertyType.IsSubclassOf(typeof(ModEffectHUD)));
            foreach (var pro in pros3)
            {
                var moditem = pro.GetValue(type) as ModEffectHUD;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New EffectHUD: " + pro.Name);
                EffectHUDs.Add(moditem);
            }

            var pros4 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModMiracle) || p.PropertyType.IsSubclassOf(typeof(ModMiracle)));
            foreach (var pro in pros4)
            {
                var moditem = pro.GetValue(type) as ModMiracle;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Miracle: " + pro.Name);
                moditem.Init(assetId++);
                Miracles.Add(moditem);
            }
            var pros5 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModCustomStatus) || p.PropertyType.IsSubclassOf(typeof(ModCustomStatus)));
            foreach (var pro in pros5)
            {
                var moditem = pro.GetValue(type) as ModCustomStatus;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Status: " + pro.Name);
                moditem.Init();
                Statuses.Add(moditem);
            }
            id = GetFirstWeaponId();
            var pros6 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModWeapon) || p.PropertyType.IsSubclassOf(typeof(ModWeapon)));
            foreach (var pro in pros6)
            {
                var moditem = pro.GetValue(type) as ModWeapon;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Weapon: " + pro.Name);
                moditem.Init(id++, assetId++);
                Weapons.Add(moditem);
            }
            var pros7 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(CharacterBuffMod) || p.PropertyType.IsSubclassOf(typeof(CharacterBuffMod)));
            foreach (var pro in pros7)
            {
                var moditem = pro.GetValue(type) as CharacterBuffMod;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Buff: " + pro.Name);
                moditem.AssetId = assetId++;
                Buffs.Add(moditem);
            }
            var pros8 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModKeyword) || p.PropertyType.IsSubclassOf(typeof(ModKeyword)));
            foreach (var pro in pros8)
            {
                var moditem = pro.GetValue(type) as ModKeyword;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Keyword: " + pro.Name);
                moditem.Init();
                Keywords.Add(moditem);
            }
            var passiveId = GetFirstPassiveId();
            var pros9 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModPassive) || p.PropertyType.IsSubclassOf(typeof(ModPassive)));
            foreach (var pro in pros9)
            {
                var moditem = pro.GetValue(type) as ModPassive;
                if (Core.LogFew)
                    Melon<Core>.Logger.Msg("New Passive: " + pro.Name);
                moditem.Init(passiveId++, assetId++, assetId++, assetId++);
                Passives.Add(moditem);
            }
            //CustomCostumeDatabase.Initialize();
        }
        public static void Register(List<UnityEngine.Object> list)
        {
            foreach (var moditem in All)
            {
                list.Add(moditem.ItemEntity);
            }
        }
        public static void RegisterDamageIds(List<UnityEngine.Object> list)
        {
            foreach (var moditem in All)
            {
                if(moditem is IModDamageId charm && charm.HasDamageId)
                {
                    if (Core.LogFew)
                        Melon<Core>.Logger.Msg("New DamageId: " + charm.DamageIdEntity.name);
                    list.Add(charm.DamageIdEntity);
                }
            }
            foreach (var moditem in Combos)
            {
                if (moditem is IModDamageId charm && charm.HasDamageId)
                {
                    if (Core.LogFew)
                        Melon<Core>.Logger.Msg("New DamageId: " + charm.DamageIdEntity.name);
                    list.Add(charm.DamageIdEntity);
                }
            }
        }
        public static void RegisterCombos(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Combos)
            {
                list.Add(moditem.ItemCategoryEntity);
            }
        }

        public static void RegisterEffectHUDs(List<UnityEngine.Object> list)
        {
            GameObject stack = null;
            foreach(var o in list)
            {
                if(o is EffectHUDEntity entity)
                {
                    if(entity.id == "ABANDONEDGOLDRING")
                    {
                        stack = entity.hudPrefab;
                    }
                }
            }
            foreach(var moditem in EffectHUDs)
            {
                if(moditem.Type == ModEffectHUD.EffectHUDType.Stack && stack != null)
                {
                    var prefab = UnityEngine.Object.Instantiate(stack);
                    moditem.SetResourcePrefab(prefab);
                    list.Add(moditem.CreateEntity());
                }
            }
        }
        public static void RegisterMiracles(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Miracles)
            {
                list.Add(moditem.Prefab);
            }
        }
        public static void RegisterStatuses(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Statuses)
            {
                list.Add(moditem.StatusEntity);
            }
        }
        public static void RegisterWeapons(List<UnityEngine.Object> list)
        {
            var weapons = list.Select(x => x as WeaponEntity).ToList();


            foreach (var moditem in Weapons)
            {
                WeaponEntity copy = null;
                foreach(var w in weapons)
                {
                    if(moditem.Copy == w.id)
                    {
                        copy = w;
                    }
                }
                if (copy == null)
                    continue;
                if (moditem.WeaponEntity == null)
                    moditem.Init(copy);
                else if (moditem.MainWeaponPrefab == null)
                {
                    moditem.InitPrefab(copy);
                    moditem.WeaponEntity.mainWeaponPrefab = moditem.MainWeaponPrefab;
                }
                list.Add(moditem.WeaponEntity);
            }


            foreach (var w in weapons)
            {
                foreach (var moditem in Weapons)
                {
                    if (moditem.Dependency == -1)
                        continue;
                    if (w.id == moditem.Dependency && moditem.WeaponEntity != null && !w.standardEnhancements.Select(x => x.enhanced.id).Contains(moditem.Id))
                    {
                        w.standardEnhancements.Add(new EnhancementMetadata() { enabled  = true, enhanced = moditem.WeaponEntity });
                    }
                }
            }

            var newweapons = list.Select(x => x as WeaponEntity).ToList();
            foreach (var moditem in Weapons)
            {
                var enhances = new List<WeaponEntity>();
                foreach(var enhancement in moditem.StandardEnhancements)
                {
                    WeaponEntity copy = null;
                    foreach (var w in newweapons)
                    {
                        if (enhancement == w.id)
                        {
                            copy = w;
                        }
                    }
                    if (copy == null)
                        continue;
                    enhances.Add(copy);
                }
                if (moditem.WeaponEntity != null)
                    moditem.WeaponEntity.standardEnhancements = enhances.Select(x => new EnhancementMetadata() { enabled = true, enhanced = x }).ToList();
            }
        }
        public static void RegisterCostume(List<UnityEngine.Object> list)
        {
            if (list[0] is CostumeEntity entity && entity.costumePrefab.TryGetComponent<PlayerAvatarCostume>(out var costume))
            {
                CustomCostumeDatabase.InitializeGameObject(costume);
                list.AddRange(CustomCostumeDatabase.CreateAll());
            }
        }
        public static void RegisterCostumeSkin(List<UnityEngine.Object> list)
        {
            list.AddRange(CustomCostumeDatabase.CreateAllSkin());
        }
        public static void RegisterKeywords(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Keywords)
            {
                list.Add(moditem.KeywordEntity);
            }
            foreach (var moditem in Statuses)
            {
                if(moditem.HasKeyword)
                    list.Add(moditem.KeywordEntity);
            }
            foreach (var item in list)
            {
                if (item is KeywordEntity entity)
                {
                    foreach (var moditem in Keywords)
                    {
                        moditem.Init(entity);
                    }
                }
            }
            foreach (var item in list)
            {
                if (item is KeywordEntity entity)
                {
                    foreach (var moditem in Statuses)
                    {
                        if (moditem.HasKeyword)
                            moditem.Keyword.Init(entity);
                    }
                }
            }
        }
        public static void RegisterPassives(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Passives)
            {
                list.Add(moditem.PassiveEntity);
            }
        }
        public static int GetFirstModId()
        {
            return 14000;
        }
        public static int GetFirstWeaponId()
        {
            return 14000;
        }
        public static uint GetFirstAssetId()
        {
            return 5;
        }
        public static ulong GetFirstPassiveId()
        {
            return 140;
        }

        public static T CreateBuff<T>(string name, string hud) where T : CharacterBuffMod
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var buff = ob.AddComponent<T>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.enabled = false;
            return buff;
        }
        public static T CreateBuff<T>(string name, string hud, int stack, params BuffStatus[] status) where T : CharacterBuffMod_StatusInstance
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var buff = ob.AddComponent<T>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.maxStackCount = stack;
            buff.add = status;
            buff.enabled = false;
            return buff;
        }
        public static CharacterBuffMod_StatusInstance CreateBuff(string name, string hud, int stack, params BuffStatus[] status)
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var log = ob.AddComponent<LogComponent>();
            var buff = ob.AddComponent<CharacterBuffMod_StatusInstance>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.maxStackCount = stack;
            buff.add = status;
            buff.enabled = false;
            return buff;
        }
        public static BuffStatus CreateBuffStatus(string id, int value)
        {
            return new BuffStatus() { id = id, value = value };
        }
    }
}

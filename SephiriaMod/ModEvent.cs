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

namespace SephiriaMod
{
    public static class ModEvent
    {
        public static event Action<WeaponSimple_Crossbow> OnSubAttackCrossbow;
        public static event Action<WeaponSimple_Katana> OnSubAttackKatana;
        public static event Action<WeaponSimple_GreatSword> OnSubAttackGreatSword;
        [Obsolete]
        public static event Action<Charm_SummonUnit, DamageInstance> OnSummonUnitDied;
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
        static ModEvent()
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

            foreach (var sound in typeof(ModEvent).GetProperties().Where(p => p.PropertyType == typeof(EventReference)))
            {
                if (id != sound.Name)
                    continue;
                EventInstance eventInstance = RuntimeManager.CreateInstance((EventReference)sound.GetValue(typeof(ModEvent)));
                eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
                return;
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
        [Obsolete]
        [HarmonyPatch(typeof(Charm_SummonUnit), "HandleUnitDie", new Type[] { typeof(DamageInstance) })]
        public static class Charm_SummonUnitHandleUnitDiePatch
        {
            static void Postfix(DamageInstance obj, Charm_SummonUnit __instance)
            {
                OnSummonUnitDied?.Invoke(__instance, obj);
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
                /*
                if (name == "KKA" || name == "kka")
                {
                    if ((message.Contains("しか") || message.Contains("シカ") || message.Contains("deers") || message.Contains("Deers")))
                    {
                        message = message.Replace("しか", "Shika").Replace("シカ", "Shika").Replace("deers", "shikas").Replace("Deers", "Shikas");
                    }
                    else
                    {
                        message = "せめてストーカーするなら女の子がいい";
                    }
                    string text = name + " : " + message;
                    GameLogWriter.Instance.WriteLog(text, Color.cyan);
                    if ((bool)avatar)
                    {
                        avatar.CreateChatBubble(message);
                    }
                    return false;
                }*/
                return true;
            }
        }
        /*
        public static class Charm_TuningForksPatch
        {

            public static string damageId = "Charm_ThrowGrimoire";

            public static float bulletDamage = 100f;
            public static int staggeringLevel = 1;

            public static Dictionary<uint, int> remain = new();

            public static float externalForcePower = 1f;
            public static int[] damageByLevel = [80, 160, 240, 320, 400, 480];
            public static uint AssetId
            {
                get
                {
                    _assetId ??= SephiriaPrefabs.PallasBigBullet.GetComponent<NetworkIdentity>().assetId + 1;
                    return _assetId.Value;
                }
            }
            private static uint? _assetId = null;
            public static Sprite bulletSprite;
            public static void Init()
            {
                //bulletSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "GrimoireBullet");
                //Core.InstantiateNetworkClientPatch.OnGetPrefab += OnGetPrefab;
                //Core.InstantiateNetworkClientPatch.OnInstantiate += OnInstantiate;
            }

            private static GameObject OnInstantiate(GameObject original, Vector3 position, Quaternion rotation)
            {
                if (original.name != SephiriaPrefabs.PallasBigBullet.name)
                    return null;
                var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                ob.AddComponent<NetworkIdentity>().SetAssetId(AssetId);
                ModifyPrefab(ob.GetComponent<Bullet>());
                return ob;
            }

            private static GameObject OnGetPrefab(uint assetId)
            {
                if (assetId == AssetId)
                {
                    return SephiriaPrefabs.PallasBigBullet;
                }
                return null;
            }
            public static void ModifyPrefab(Bullet bullet)
            {
                //Melon<Core>.Logger.Msg("0: " + (bullet != null));
                var wrapper = bullet.transform.GetChild(0);
                //Melon<Core>.Logger.Msg("1: " + (wrapper != null));
                var body = wrapper.GetChild(0);
                //Melon<Core>.Logger.Msg("2: " + (body != null));
                var animator = body.GetComponent<Animator2D_SpriteRenderer>();
                //Melon<Core>.Logger.Msg("3: " + (animator != null));
                //Melon<Core>.Logger.Msg("4: " + (animator.currentSet != null));
                var stateinfo = animator.currentSet.sprites;
                //Melon<Core>.Logger.Msg("5: " + (stateinfo != null));
                if (stateinfo.Count > 0)
                {
                    var sprites = stateinfo[0].timeline;
                    //Melon<Core>.Logger.Msg("6: " + (sprites.Count));
                    for (int q = 0; q < sprites.Count; q++)
                    {
                        sprites[q].sprite = bulletSprite;
                    }
                }
                var key = animator.GetCurrentBakedKeyFrame();
                foreach (var time in key.timeline.Values)
                {
                    for (int q = 0; q < time.Count; q++)
                    {
                        time[q] = bulletSprite;
                    }
                }
                var destroy = bullet.gameObject.GetComponent<BulletDestroyModule_DestroyImmediate>();
                //animator.ChangeSet(animator.currentSet);
            }
            public static void OnAttack(CombatBehaviour combat, DamageInstance damageInstance, ProjectileBase projectile, Charm_TuningForks __instance)
            {
                Melon<Core>.Logger.Msg("OnAttack");
                if (!remain.ContainsKey(__instance.netId))
                    return;
                if (remain[__instance.netId] > 0 && !__instance.NetworkAvatar.IsDead && __instance.IsEffectEnabled && damageInstance.fromType == EDamageFromType.DirectAttack)
                {
                    remain[__instance.netId]--;
                    int count = 4 + __instance.NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
                    bulletDamage = __instance.NetworkAvatar.GetCustomStat(ECustomStat.PhysicalDamage) * (damageByLevel.SafeRandomAccess(__instance.CurrentLevelToIdx()) / 100f);
                    bulletDamage += bulletDamage * (__instance.NetworkAvatar.GetCustomStat(ECustomStat.MagicDamageBonus) / 100f);
                    float anglePer = 6f;
                    float startAngle = anglePer * (float)(count - 1) * 0.5f;
                    float angle = (__instance.WeaponController.aimedPositionClientside - __instance.WeaponController.transform.position).GetAngle();
                    for (int q = 0; q < count; q++)
                    {
                        Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                        Vector3 motionDataBegin = __instance.NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                        Vector3 motionDataEnd = __instance.NetworkAvatar.transform.position + vector3FromAngle * 8f;
                        bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                        //Melon<Core>.Logger.Msg("Prefab: " + (BulletPrefab != null));
                        Bullet bullet = Bullet.Pool.Spawn(AssetId, SephiriaPrefabs.PallasBigBullet, __instance.NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage, staggeringLevel, externalForcePower, __instance.NetworkAvatar, __instance.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), __instance.NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);
                        bullet.pierceCreatureCount = 3;
                        ModifyPrefab(bullet);
                        Vector3 pos = __instance.NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                        bullet.SetSpeedScale(2);

                        if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                        {
                            //bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                        }
                    }
                }
            }

            //[HarmonyPatch(typeof(Charm_TuningForks), "OnEnabledEffect")]
            public static class EnablePatch
            {
                static void Postfix(Charm_TuningForks __instance)
                {
                    Melon<Core>.Logger.Msg("OnEnabledEffect");
                    __instance.WeaponController.OnBasicAttack += (combat, damage, projectile) => OnAttack(combat, damage, projectile, __instance);
                }
            }


            //[HarmonyPatch(typeof(Charm_TuningForks), "OnDisabledEffect")]
            public static class DisablePatch
            {
                static void Postfix(Charm_TuningForks __instance)
                {
                    Melon<Core>.Logger.Msg("OnDisabledEffect");
                    __instance.WeaponController.OnBasicAttack -= (combat, damage, projectile) => OnAttack(combat, damage, projectile, __instance);
                }
            }
            //[HarmonyPatch(typeof(Charm_TuningForks), "OnBeginCastMagicServerside")]
            public static class AttackPatch
            {
                static void Prefix(Charm_TuningForks __instance)
                {
                    if (!__instance.GetEnhancedWeaponDamage() && __instance.cooldownTimer.Check())
                    {
                        remain[__instance.netId] = 3;
                    }
                    if (remain.ContainsKey(__instance.netId))
                        Melon<Core>.Logger.Msg("OnBeginCastMagicServerside: " + remain[__instance.netId]);
                }
            }

            //[HarmonyPatch(typeof(Charm_TuningForks), nameof(Charm_TuningForks.BuildKeywords), new Type[] { typeof(UnitAvatar), typeof(int), typeof(int), typeof(bool), typeof(bool) })]
            public static class KeywordsPatch
            {
                static void Postfix(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus, Charm_TuningForks __instance, ref Loc.KeywordValue[] __result)
                {
                    string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(__instance.maxLevel)) : damageByLevel.SafeRandomAccess(__instance.LevelToIdx(level)).ToString());
                    var list = __result.ToList();
                    list.Add(new Loc.KeywordValue("DAMAGE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)));
                    __result = list.ToArray();
                }
            }
        }*/
        /*
        [HarmonyPatch(typeof(BulletDestroyModule), nameof(BulletDestroyModule.CreateDestroyVisualOnClient))]
        public static class BulletDestroyModulePatch<T> where T : ObjectPoolable
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Melon<Core>.Logger.Msg($"BulletDestroyModulePatch Transpiler");

                var replacement = AccessTools.Method(typeof(BulletDestroyModulePatch<SpriteFx>), nameof(SpriteFx.Pool.Spawn));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(SpriteFx.Pool.Spawn))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        Melon<Core>.Logger.Msg($"Transpiler!");
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }
            private static T CustomSpawn(GameObject key, Vector3 pos, GameObject owner = null)
            {
                var spawned = SpriteFx.Pool.Spawn(key, pos, owner);
                OnBulletDestroyModuleSpriteFxSpawned?.Invoke(spawned, pos, owner);
                return spawned as T;
            }
        }*/
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
                if(color.a == 0)
                {
                    //Melon<Core>.Logger.Msg("RpcShowDamageParticle blue!");
                    msg = msg.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
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
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing_ManualDirection), [typeof(int), typeof(Vector2)])]
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

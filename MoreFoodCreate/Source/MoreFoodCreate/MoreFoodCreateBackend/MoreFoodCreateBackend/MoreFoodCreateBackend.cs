using Config;
using GameData.ArchiveData;
using GameData.Common;
using GameData.Domains;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Utilities;
using HarmonyLib;
using Redzen.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuModdingLib;
using TaiwuModdingLib.Core.Plugin;

namespace SXDZD
{
    [PluginConfig(pluginName: "CraftToolDurabilityInc", creatorId: "熟悉的总督", pluginVersion: "0.5")]
    public class MoreFoodCreateBackend : TaiwuRemakeHarmonyPlugin
    {
        public static int CraftToolDurability = 200;
        public static bool NoCraftToolDurabilityReduce = false;
        public static bool MakeImmediatelyComplete = false;
        private Harmony harmony;
        public override void OnModSettingUpdate()
        {
            DomainManager.Mod.GetSetting(ModIdStr, "CraftToolDurability", ref CraftToolDurability);
            DomainManager.Mod.GetSetting(ModIdStr, "NoCraftToolDurabilityReduce", ref NoCraftToolDurabilityReduce);
            DomainManager.Mod.GetSetting(ModIdStr, "MakeImmediatelyComplete", ref MakeImmediatelyComplete);
        }
        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(MoreFoodCreateBackend));
        }

        public override void Dispose()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ItemBase), "GetCurrDurability")]
        public static void CraftToolGetCurrDurability_Patch(ItemBase __instance, ref short __result)
        {
            if (false == NoCraftToolDurabilityReduce) return;

            if (__instance is GameData.Domains.Item.CraftTool)
            {
                __result = __result > (short)CraftToolDurability ? __result : (short)CraftToolDurability;
                AdaptableLog.Info($"返回工具当前耐久修改：{__result}");
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(ItemBase), "GetMaxDurability")]
        public static void CraftToolGetMaxDurability_Patch(ItemBase __instance, ref short __result)
        {
            if (CraftToolDurability == 0) return;

            if (__instance is GameData.Domains.Item.CraftTool)
            {
                __result = (short)CraftToolDurability;
                AdaptableLog.Info($"返回工具耐久上限修改：{__result}");
            }
        }


        //IRandomSource random, short templateId, int itemId
        [HarmonyPostfix, HarmonyPatch(typeof(GameData.Domains.Item.ItemBase), "GenerateMaxDurability")]
        public static void CraftToolGenerateMaxDurability_Patch(ItemBase __instance, ref short __result)
        {
            if (CraftToolDurability == 0) return;

            if (__instance is GameData.Domains.Item.CraftTool)
            {
                __result = (short)CraftToolDurability;
                AdaptableLog.Info($"生成工具随机耐久修改：{__result}");
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GameData.Domains.Item.CraftTool), "SetCurrDurability")]
        public static bool CraftToolSetCurrDurability_Patch(GameData.Domains.Item.CraftTool __instance, ref short currDurability, DataContext context)
        {
            if (true == NoCraftToolDurabilityReduce)
            {
                short preDurability = __instance.GetCurrDurability();
                if (preDurability > currDurability)
                {
                    currDurability = preDurability;
                }
            }

            if (CraftToolDurability > 0)
            {
                short maxDur = (short)CraftToolDurability;
                AdaptableLog.Info($"设置当前耐久同时设置耐久上限：{maxDur}");
                __instance.SetMaxDurability(maxDur, context);
            }

            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GameData.Domains.Item.CraftTool), "SetMaxDurability")]
        public static bool CraftToolMaxDurability_Patch(GameData.Domains.Item.CraftTool __instance, ref short maxDurability, DataContext context)
        {
            if (CraftToolDurability == 0) return true;

            maxDurability = (short)CraftToolDurability;
            Console.WriteLine($"设置最大耐久：{maxDurability}");
            return true;
        }

        /// <summary>
        /// 制造瞬间完成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HarmonyPrefix, HarmonyPatch(typeof(BuildingDomain), "AddElement_MakeItemDict")]
        public static bool AddElement_MakeItemDict_Patch(ref MakeItemData value)
        {
            if (!MakeImmediatelyComplete) return true;

            value.LeftTime = 0;
            return true;
        }
    }
}

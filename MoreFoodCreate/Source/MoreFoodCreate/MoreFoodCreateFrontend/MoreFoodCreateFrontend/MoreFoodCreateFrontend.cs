using GameData.Domains.Item.Display;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;
using SXDZD.Tools;

namespace MoreFoodCreateFrontend
{
    [PluginConfig(pluginName: "CraftToolDurabilityInc", creatorId: "熟悉的总督", pluginVersion: "0.4")]
    public class MoreFoodCreateFrontend : TaiwuRemakePlugin
    {
        private Harmony harmony;
        private static int CraftToolDurability;
        public static bool NoCraftToolDurabilityReduce = false;
        public static bool MakeImmediatelyComplete = false;

        public override void OnModSettingUpdate()
        {
            ModManager.GetSetting(this.ModIdStr, "CraftToolDurability", ref CraftToolDurability);
            ModManager.GetSetting(this.ModIdStr, "NoCraftToolDurabilityReduce", ref NoCraftToolDurabilityReduce);
            ModManager.GetSetting(this.ModIdStr, "MakeImmediatelyComplete", ref MakeImmediatelyComplete);
        }
        public override void Dispose()
        {
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(MoreFoodCreateFrontend));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "OnInit")]
        public static void UI_MakeOnInit_Patch(UI_Make __instance, Refers ____makeRequireResourceList, CButton ____makeButtonNext)
        {
            var ChangeMakeResourceCount = typeof(UI_Make).GetMethod("ChangeMakeResourceCount");
            //Camera_UIRoot/Canvas/LayerPopUp/UI_Make/Root/CenterPanel/Common/RequireResourceList/Food/ButtonMax

            Refers refers = ____makeRequireResourceList.transform.GetChild(0).GetComponent<Refers>();
            CButton cButtonMax = refers.CGet<CButton>("ButtonMax");
            GameObject newMaxBtn = GameObject.Instantiate(cButtonMax.gameObject, ____makeButtonNext.transform.parent);
            newMaxBtn.name = "ButtonMax";
            MouseTipDisplayer mouseTip = newMaxBtn.AddComponent<MouseTipDisplayer>();
            mouseTip.PresetParam = new string[] {"一键添加最大个数"}; 
            (newMaxBtn.transform as RectTransform).anchoredPosition = (____makeButtonNext.transform as RectTransform).anchoredPosition + new Vector2(10, 0);
            newMaxBtn.GetComponent<CButton>().ClearAndAddListener(() =>
            {
                Type insType = __instance.GetType();
                ItemDisplayData _currentTarget = __instance.GetFieldValue<ItemDisplayData>("_currentTarget");
                ItemDisplayData _currentTool = __instance.GetFieldValue<ItemDisplayData>("_currentTool");
                if (_currentTarget == null || _currentTool == null) return;
                short maxNum = (short)Mathf.Min(_currentTarget.Amount, _currentTool.Durability);
                __instance.SetFieldValue("_makeCount", maxNum);
                __instance.ExcuteMethod("CheckCondition", new object[] { });
                Debug.Log($"设置制造次数为：{maxNum}");
            });

        }

        public static void UpdateButtonMaxVisible(UI_Make __instance, CButton ____makeButtonNext)
        {
            Transform cButtonMaxTrans = ____makeButtonNext.transform.parent.Find("ButtonMax");
            if (cButtonMaxTrans == null)
            {
                Debug.Log($"UpdateButtonMaxVisible 未找到 ButtonMax！");
                return;
            }
            cButtonMaxTrans.gameObject.SetActive(____makeButtonNext.isActiveAndEnabled);
            cButtonMaxTrans.GetComponent<CButton>().interactable = ____makeButtonNext.interactable;
            //Debug.Log($"UpdateButtonMaxVisible：{cButtonMaxTrans.gameObject.activeSelf}");
        }

        [HarmonyPostfix,HarmonyPatch(typeof(UI_Make), "CheckMakeCondition")]
        public static void UI_Make_CheckMakeCondition_Patch(UI_Make __instance, CButton ____makeButtonNext)
        {
            //Debug.Log($"UI_Make_CheckMakeCondition_Patch设置MaxButton按钮显示");
            UpdateButtonMaxVisible(__instance, ____makeButtonNext);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "UpdateMakeState")]
        public static void UI_Make_UpdateMakeState_Patch(UI_Make __instance, CButton ____makeButtonNext)
        {
            //Debug.Log($"UI_Make_UpdateMakeState_Patch设置MaxButton按钮显示");
            UpdateButtonMaxVisible(__instance, ____makeButtonNext);
        }


        [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "ConfirmMake")]
        public static void UI_MakeConfirmMake_Patch(UI_Make __instance, List<ItemDisplayData> ____allItems, CButton ____makeButtonNext, ItemDisplayData ____currentTool)
        {
            if (false == NoCraftToolDurabilityReduce)
            {
                //Debug.Log($"UI_MakeConfirmMake_Patch设置MaxButton按钮显示");
                UpdateButtonMaxVisible(__instance, ____makeButtonNext);
                return;
            }else
            {
                ItemDisplayData itemDisplayData2 = ____allItems.Find((ItemDisplayData d) => d.Key.Equals(____currentTool.Key));

                if (itemDisplayData2 != null && CraftToolDurability > 0)
                {
                    itemDisplayData2.Durability = itemDisplayData2.Durability > (short)CraftToolDurability ? itemDisplayData2.Durability : (short)CraftToolDurability;
                    Debug.Log($"刷新工具耐久度！");
                }

                //Debug.Log($"UI_MakeConfirmMake_Patch设置MaxButton按钮显示");
                UpdateButtonMaxVisible(__instance, ____makeButtonNext);
            }
           
        }


    }
}

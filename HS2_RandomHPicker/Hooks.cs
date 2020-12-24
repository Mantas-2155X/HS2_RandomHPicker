using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using HS2;
using Manager;
using SceneAssist;
using Illusion.Game;

using UniRx;
using UniRx.Triggers;

namespace HS2_RandomHPicker
{
    public static class Hooks
    {
        [HarmonyPostfix, HarmonyPatch(typeof(LobbyMainUI), "LoadMapImage")]
        private static void LobbyMainUI_LoadMapImage_CreateRandomButtons(LobbySelectUI ___selectUI, LobbyMapSelectUI ___mapSelectUI)
        {
            if (___selectUI.transform.Find("Select/SelectView/btnRandom") != null || ___mapSelectUI.transform.Find("Parent/btnRandom") != null)
                return;

            for (var i = 0; i < 2; i++)
            {
                var i1 = i;

                var orig = i == 0 ? ___selectUI.transform.Find("Select/SelectView/btnChange") : ___mapSelectUI.transform.Find("Parent/btnStart");
                if (orig == null)
                    return;
                
                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.localPosition = i == 0 ? new Vector3(135, -605) : new Vector3(0, -383);
                copy.name = "btnRandom";

                var text = copy.GetComponentInChildren<Text>();
                text.text = "■ Random";

                var button = copy.GetComponent<Button>();
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate 
                {
                    if (i1 == 0)
                        HS2_RandomHPicker.RandomCharacter(___selectUI);
                    else
                        HS2_RandomHPicker.RandomMap(___mapSelectUI);
                });

                if (i == 0)
                {
                    var action = copy.GetComponent<PointerEnterExitAction>();
                
                    action.listActionEnter.Clear();
                    action.listActionEnter.Add(delegate
                    {
                        if (!button.IsInteractable()) 
                            return;
                    
                        Utils.Sound.Play(SystemSE.sel);
                        text.color = Game.selectFontColor;
                    });
                
                    action.listActionExit.Clear();
                    action.listActionExit.Add(delegate
                    {
                        if (button.IsInteractable())
                            text.color = Game.defaultFontColor;
                    });
                }
                else
                {
                    button.OnPointerEnterAsObservable().Subscribe(delegate
                    {
                        if (!button.IsInteractable()) 
                            return;
                    
                        Utils.Sound.Play(SystemSE.sel);
                        text.color = Game.selectFontColor;
                    });
                
                    button.OnPointerExitAsObservable().Subscribe(delegate
                    {
                        if (button.IsInteractable())
                            text.color = Game.defaultFontColor;
                    });
                }
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(STRConfirmation), "SetPlayerDontSelect")]
        private static void STRConfirmation_SetPlayerDontSelect_CreateRandomButtons(STRConfirmation __instance)
        {
            if (__instance.transform.Find("Select/SelectChara/Panel/btnRandom") != null || __instance.transform.Find("Select/PlayerSelect/Panel/btnRandom") != null)
                return;

            for (var i = 0; i < 2; i++)
            {
                var i1 = i;

                var orig = __instance.transform.Find("imgBack/btnNo");
                if (orig == null)
                    return;

                var scrollView = __instance.transform.Find(i == 0 ? "Select/SelectChara/Panel/View/Scroll View" : "Select/PlayerSelect/Panel/View/Scroll View");

                var scrollRect = scrollView.GetComponent<RectTransform>();
                scrollRect.offsetMin = new Vector2(scrollRect.offsetMin.x, -746);
                
                var copy = Object.Instantiate(orig, __instance.transform.Find(i == 0 ? "Select/SelectChara/Panel" : "Select/PlayerSelect/Panel"));

                var rect = copy.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(-235, rect.offsetMin.y);
                rect.offsetMax = new Vector2(-87, rect.offsetMax.y);
                
                copy.localPosition = new Vector3(8, -435);
                copy.name = "btnRandom";

                var text = copy.GetComponentInChildren<Text>();
                text.text = "■ Random";

                var button = copy.GetComponent<Button>();
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate 
                {
                    if (i1 == 0)
                        HS2_RandomHPicker.RandomCharacterDX(__instance.PartnerSelectUI);
                    else
                        HS2_RandomHPicker.RandomCharacterDX(__instance.PlayerSelectUI);
                });
                
                button.OnPointerEnterAsObservable().Subscribe(delegate
                {
                    if (!button.IsInteractable()) 
                        return;
                    
                    Utils.Sound.Play(SystemSE.sel);
                    text.color = Game.selectFontColor;
                });
                
                button.OnPointerExitAsObservable().Subscribe(delegate
                {
                    text.color = Game.defaultFontColor;
                });
            }
        }
    }
}
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
        private static void type_LoadMapImage_CreateRandomButtons(LobbySelectUI ___selectUI, LobbyMapSelectUI ___mapSelectUI)
        {
            {
                var orig = ___selectUI.transform.Find("Select/SelectView/btnChange");
                if (orig == null)
                    return;

                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.localPosition = new Vector3(135, -605);
                copy.name = "btnRandom";

                var text = copy.GetComponentInChildren<Text>();
                text.text = "■ Random";

                var button = copy.GetComponent<Button>();
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate 
                {
                    HS2_RandomHPicker.RandomCharacter(___selectUI);
                });

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

            {
                var orig = ___mapSelectUI.transform.Find("Parent/btnStart");
                if (orig == null)
                    return;

                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.localPosition = new Vector3(0, -383);
                copy.name = "btnRandom";

                var text = copy.GetComponentInChildren<Text>();
                text.text = "■ Random Map";

                var button = copy.GetComponent<Button>();
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate 
                {
                    HS2_RandomHPicker.RandomMap(___mapSelectUI);
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
                    if (button.IsInteractable())
                        text.color = Game.defaultFontColor;
                });
            }
        }
    }
}
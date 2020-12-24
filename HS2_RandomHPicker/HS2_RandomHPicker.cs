using System;
using System.Collections.Generic;

using BepInEx;
using HarmonyLib;

using HS2;
using AIChara;
using Illusion.Game;
using GameLoadCharaFileSystem;

namespace HS2_RandomHPicker
{
    [BepInPlugin(nameof(HS2_RandomHPicker), nameof(HS2_RandomHPicker), VERSION)]
    [BepInProcess("HoneySelect2")]
    public class HS2_RandomHPicker : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";

        private static readonly Random rand = new Random();
        
        private static readonly List<LobbyCharaSelectInfoScrollController.ScrollData> validCharacters = new List<LobbyCharaSelectInfoScrollController.ScrollData>();
        private static readonly List<LobbyMapSelectInfoScrollController.ScrollData> validMaps = new List<LobbyMapSelectInfoScrollController.ScrollData>();

        private void Awake() => Harmony.CreateAndPatchAll(typeof(Hooks), "HS2_RandomHPicker");

        public static void RandomCharacter(LobbySelectUI ___selectUI)
        {                
            var trav = Traverse.Create(___selectUI);
            
            Utils.Sound.Play(SystemSE.ok_s);
            validCharacters.Clear();

            var datas = trav.Field("scrollCtrl").Field("scrollerDatas").GetValue<LobbyCharaSelectInfoScrollController.ScrollData[]>();
            var entryNo = trav.Field("entryCharaNo").GetValue<int>();

            foreach (var data in datas)
                if (entryNo == 0 || entryNo == 1 && data.info.hcount != 0 && data.info.state != ChaFileDefine.State.Broken)
                    validCharacters.Add(data);

            trav.Field("scrollCtrl").Method("OnValueChange", validCharacters[rand.Next(validCharacters.Count)], true).GetValue();
            trav.Field("scrollCtrl").Method("SetNowLine").GetValue();
        }
        
        public static void RandomCharacterDX(object ___selectUI)
        {
            var trav = Traverse.Create(___selectUI);
            
            Utils.Sound.Play(SystemSE.ok_s);

            if (___selectUI.GetType() == typeof(STRCharaSelectUI))
            {
                var datas = trav.Field("listCtrl").Field("scrollerDatas").GetValue<STRCharaFileScrollController.ScrollData[]>();
                trav.Field("listCtrl").Method("OnValueChange", datas[rand.Next(datas.Length)], true).GetValue();
            }
            else
            {
                var datas = trav.Field("listCtrl").Field("scrollerDatas").GetValue<GameCharaFileScrollController.ScrollData[]>();
                trav.Field("listCtrl").Method("OnValueChange", datas[rand.Next(datas.Length)], true).GetValue();
            }
            
            trav.Field("listCtrl").Method("SetNowLine").GetValue();
        }

        public static void RandomMap(LobbyMapSelectUI ___mapSelectUI)
        {
            var trav = Traverse.Create(___mapSelectUI);
            
            Utils.Sound.Play(SystemSE.ok_s);
            validMaps.Clear();

            var datas = trav.Field("scrollCtrl").Field("scrollerDatas").GetValue<LobbyMapSelectInfoScrollController.ScrollData[]>();

            foreach (var data in datas)
                if (data.isEnable)
                    validMaps.Add(data);

            trav.Field("scrollCtrl").Method("OnClick", validMaps[rand.Next(validMaps.Count)]).GetValue();
            trav.Field("scrollCtrl").Method("SetNowLine").GetValue();
        }
    }
}
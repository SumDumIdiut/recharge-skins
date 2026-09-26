using System;
using Recharge.ModApi;
using TMPro;
using UnityEngine;

namespace RechargeCustomSkins
{
    public class RechargeCustomSkinsMod : IRechargeMod
    {
        public const string ModId = "recharge.customskins";

        public string Id => ModId;
        public string DisplayName => "Skinmod";
        public Version Version => new Version(1, 0, 0);

        private SkinController _controller;
        private IRechargeHost _host;

        public void OnLoad(IRechargeHost host)
        {
            _host = host;
            var go = new GameObject("SkinController");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _controller = go.AddComponent<SkinController>();
            _controller.Init(host);

            PauseMenuHelper.OnMenuReady(host, InstallMenuRow);
        }

        private void InstallMenuRow(pauseMenuScript menu)
        {
            if (PauseMenuHelper.MainBit(menu) == null) return;
            var panel = PauseMenuHelper.AddPanelRow(menu, "CustomSkins", "Skinmod");
            if (panel == null) return;
            if (panel.GetComponent<SkinPanelUI>() != null) return;

            var title = panel.transform.Find("Settings") ?? (panel.transform.childCount > 0 ? panel.transform.GetChild(0) : null);
            var font = title != null ? title.GetComponent<TMP_Text>()?.font : null;

            var ui = panel.AddComponent<SkinPanelUI>();
            try
            {
                ui.Build(panel, font, _controller);
            }
            catch (Exception e)
            {
                // Typically a Recharge.ModApi older than this mod was built against.
                _host.LogError("[CustomSkins] couldn't build the skin panel: " + e);
                ShowBuildFailure(panel, font);
            }
        }

        private static void ShowBuildFailure(GameObject panel, TMP_FontAsset font)
        {
            var go = new GameObject("BuildFailure", typeof(RectTransform));
            go.transform.SetParent(panel.transform, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(520f, 200f);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = 22f;
            text.alignment = TextAlignmentOptions.Center;
            text.text = "Skinmod couldn't build its menu.\nYour Recharge loader is out of date - open the Recharge app and let it finish updating, then restart the game.";
        }

        public void OnUnload() { }
    }
}

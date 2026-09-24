using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Recharge.ModApi;
using UnityEngine;

namespace RechargeCustomSkins
{
    // Skins the dash / double-jump indicators floating by the player with an
    // optional dash.png / doublejump.png from the skin folder. The vanilla
    // indicators are Animator-driven, so - like the player sprite - the
    // replacement is re-applied every LateUpdate.
    internal static class IndicatorSkinApplier
    {
        private static readonly FieldInfo DashField =
            typeof(PlayerAbilityIndicatorController).GetField("dashIndicators", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo JumpField =
            typeof(PlayerAbilityIndicatorController).GetField("jumpIndicators", BindingFlags.NonPublic | BindingFlags.Instance);

        private class Slot
        {
            public SpriteRenderer[] All;
            public bool[] WasEnabled;
            public SpriteRenderer Primary;
            public float RefWorldWidth;
            public Sprite LastVanilla;
            public bool Skinned;
        }

        private class CustomArt
        {
            public Texture2D Texture;
            public readonly Dictionary<int, Sprite> BySlot = new Dictionary<int, Sprite>();
        }

        private static PlayerAbilityIndicatorController _controller;
        private static List<Slot> _dash, _jump;
        private static int _nextFindFrame;

        private static string _loadedFolder;
        private static readonly Dictionary<IndicatorKind, CustomArt> Art = new Dictionary<IndicatorKind, CustomArt>();
        private static readonly HashSet<Sprite> OurSprites = new HashSet<Sprite>();

        public static void Apply(IRechargeHost host, string skinFolder)
        {
            if (!EnsureController(host)) return;
            if (_loadedFolder != skinFolder) LoadArt(host, skinFolder);

            ApplyKind(_dash, IndicatorKind.Dash);
            ApplyKind(_jump, IndicatorKind.DoubleJump);
        }

        public static void RestoreVanilla()
        {
            if (_loadedFolder != null) LoadArt(null, null);
            RestoreSlots(_dash);
            RestoreSlots(_jump);
        }

        private static bool EnsureController(IRechargeHost host)
        {
            if (_controller != null) return true;
            if (Time.frameCount < _nextFindFrame) return false;
            _nextFindFrame = Time.frameCount + 60;

            _controller = Object.FindFirstObjectByType<PlayerAbilityIndicatorController>();
            if (_controller == null || DashField == null || JumpField == null) { _controller = null; return false; }

            _dash = BuildSlots(DashField.GetValue(_controller) as Transform[]);
            _jump = BuildSlots(JumpField.GetValue(_controller) as Transform[]);
            host?.Log($"[CustomSkins] indicators found: {_dash.Count} dash, {_jump.Count} double-jump");
            LogHierarchy(host, "dash", _dash);
            LogHierarchy(host, "doublejump", _jump);
            return true;
        }

        private static List<Slot> BuildSlots(Transform[] roots)
        {
            var slots = new List<Slot>();
            if (roots == null) return slots;
            foreach (var root in roots)
            {
                if (root == null) continue;
                var all = root.GetComponentsInChildren<SpriteRenderer>(true);
                slots.Add(new Slot { All = all, WasEnabled = all.Select(r => r.enabled).ToArray() });
            }
            return slots;
        }

        private static void LogHierarchy(IRechargeHost host, string label, List<Slot> slots)
        {
            if (host == null) return;
            for (int i = 0; i < slots.Count; i++)
            {
                var parts = slots[i].All.Select(r => $"{r.name}({(r.sprite != null ? r.sprite.name + " " + r.sprite.rect.width + "x" + r.sprite.rect.height : "no sprite")})");
                host.Log($"[CustomSkins] {label}[{i}] renderers: {string.Join(", ", parts)}");
            }
        }

        private static void LoadArt(IRechargeHost host, string folder)
        {
            foreach (var art in Art.Values)
            {
                foreach (var sprite in art.BySlot.Values) { OurSprites.Remove(sprite); Object.Destroy(sprite); }
                if (art.Texture != null) Object.Destroy(art.Texture);
            }
            Art.Clear();
            _loadedFolder = folder;
            if (folder == null) return;

            foreach (var kind in new[] { IndicatorKind.Dash, IndicatorKind.DoubleJump })
            {
                var path = SkinFiles.FindIndicatorImage(folder, kind);
                if (path == null) continue;
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!ImageConversion.LoadImage(tex, File.ReadAllBytes(path)))
                {
                    host?.LogWarning($"[CustomSkins] couldn't decode indicator image '{path}'");
                    Object.Destroy(tex);
                    continue;
                }
                Art[kind] = new CustomArt { Texture = tex };
            }
        }

        private static void ApplyKind(List<Slot> slots, IndicatorKind kind)
        {
            if (slots == null) return;
            if (!Art.TryGetValue(kind, out var art)) { RestoreSlots(slots); return; }

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                Capture(slot);
                if (slot.Primary == null) continue;

                if (!art.BySlot.TryGetValue(i, out var sprite))
                {
                    float ppu = art.Texture.width / slot.RefWorldWidth;
                    sprite = Sprite.Create(art.Texture, new Rect(0, 0, art.Texture.width, art.Texture.height), new Vector2(0.5f, 0.5f), ppu);
                    art.BySlot[i] = sprite;
                    OurSprites.Add(sprite);
                }
                slot.Primary.sprite = sprite;
                foreach (var r in slot.All) if (r != slot.Primary) r.enabled = false;
                slot.Skinned = true;
            }
        }

        // Remembers the animator's own sprite and picks the biggest renderer
        // as the one that carries the indicator image (others, e.g. glows,
        // are hidden while a custom image replaces it).
        private static void Capture(Slot slot)
        {
            if (slot.Primary == null)
            {
                SpriteRenderer best = null;
                float bestArea = 0f;
                foreach (var r in slot.All)
                {
                    if (r == null || r.sprite == null || OurSprites.Contains(r.sprite)) continue;
                    float area = r.sprite.rect.width * r.sprite.rect.height / (r.sprite.pixelsPerUnit * r.sprite.pixelsPerUnit);
                    if (area > bestArea) { bestArea = area; best = r; }
                }
                if (best == null) return;
                slot.Primary = best;
                slot.WasEnabled = slot.All.Select(r => r != null && r.enabled).ToArray();
                slot.RefWorldWidth = best.sprite.rect.width / best.sprite.pixelsPerUnit;
            }
            var current = slot.Primary.sprite;
            if (current != null && !OurSprites.Contains(current)) slot.LastVanilla = current;
        }

        private static void RestoreSlots(List<Slot> slots)
        {
            if (slots == null) return;
            foreach (var slot in slots)
            {
                if (!slot.Skinned) continue;
                if (slot.Primary != null && slot.LastVanilla != null) slot.Primary.sprite = slot.LastVanilla;
                for (int i = 0; i < slot.All.Length; i++) if (slot.All[i] != null) slot.All[i].enabled = slot.WasEnabled[i];
                slot.Skinned = false;
            }
        }
    }
}

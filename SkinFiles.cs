using System.IO;
using System.Linq;

namespace RechargeCustomSkins
{
    internal enum IndicatorKind { Dash, DoubleJump }

    // A skin folder holds the player image plus optional dash/double-jump
    // indicator images; those must never be mistaken for the player image.
    internal static class SkinFiles
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };
        private static readonly string[] DashNames = { "dash" };
        private static readonly string[] DoubleJumpNames = { "doublejump", "double-jump", "double_jump", "jump" };

        private static bool IsImage(string path) =>
            ImageExtensions.Contains(Path.GetExtension(path), System.StringComparer.OrdinalIgnoreCase);

        private static string[] NamesFor(IndicatorKind kind) => kind == IndicatorKind.Dash ? DashNames : DoubleJumpNames;

        private static bool IsIndicatorImage(string path)
        {
            var stem = Path.GetFileNameWithoutExtension(path);
            return DashNames.Concat(DoubleJumpNames).Contains(stem, System.StringComparer.OrdinalIgnoreCase);
        }

        public static string FindPlayerImage(string folder) =>
            Directory.EnumerateFiles(folder).OrderBy(f => f, System.StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(f => IsImage(f) && !IsIndicatorImage(f));

        // Skins downloaded by the app live in a slugged folder ("Colon (:)" ->
        // "colon", ":3" -> "3"); the name they were published under is kept
        // in this file next to them.
        private const string HubMetaFile = ".recharge-hub-meta.json";

        public static string ReadDisplayName(string folder)
        {
            try
            {
                var path = Path.Combine(folder, HubMetaFile);
                if (!File.Exists(path)) return null;
                var name = (string)Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path))["name"];
                return string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            }
            catch (System.Exception)
            {
                return null; // unreadable meta - fall back to the folder name
            }
        }

        public static string FindIndicatorImage(string folder, IndicatorKind kind)
        {
            var names = NamesFor(kind);
            return Directory.EnumerateFiles(folder).OrderBy(f => f, System.StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(f => IsImage(f) && names.Contains(Path.GetFileNameWithoutExtension(f), System.StringComparer.OrdinalIgnoreCase));
        }
    }
}

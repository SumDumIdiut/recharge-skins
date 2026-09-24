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

        public static string FindIndicatorImage(string folder, IndicatorKind kind)
        {
            var names = NamesFor(kind);
            return Directory.EnumerateFiles(folder).OrderBy(f => f, System.StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(f => IsImage(f) && names.Contains(Path.GetFileNameWithoutExtension(f), System.StringComparer.OrdinalIgnoreCase));
        }
    }
}

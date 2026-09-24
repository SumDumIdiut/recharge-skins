# recharge-skins (Skinmod)

Player reskin mod for *IGTAP*: pick skins from the in-game Skinmod panel, with per-skin custom sound effects. Ghosts and clones (replays, multiplayer players) are reskinned too, without touching save data.

Part of [Recharge](https://github.com/SumDumIdiut/recharge): the app pulls this repo into its mods folder and `build-loader.ps1 -ModsDir` compiles it against the game's `Managed` folder.

## Indicator art

A skin folder can also hold optional art for the dash and double-jump indicators floating by the player. The game's own indicator code keeps running (position, bobbing, animation); the skin is just drawn over it.

- `dash.png` - each dash indicator is a dot orbiting the player's head, drawn as 12 sprite frames: the dot passing in front (frames 1-7, arcing left to right) then behind (frames 8-12, right to left). The sheet is a grid of **12 columns x 1 row** on the same magenta guide grid as the player sheet, frames in that order. Start from [`templates/skin-template/dash.png`](templates/skin-template/dash.png), which holds the vanilla frames at their true positions (a 64px game frame at 128px per cell; any cell size works). A plain image that isn't on the grid is drawn as a static dot, and the behind-the-head dot is hidden.
- `doublejump.png` (or `jump.png`) - a single image, drawn over the double-jump indicator. Start from [`templates/skin-template/doublejump.png`](templates/skin-template/doublejump.png).

Both are scaled to cover the same area as the vanilla sprite, and are never taken as the skin's own player image (which is the first other image in the folder).

`templates/skin-template/` is a ready-made skin folder with the unedited vanilla `dash.png` and `doublejump.png`: copy it, add your player image, and edit the two sheets in place.

# recharge-skins (Skinmod)

Player reskin mod for *IGTAP*: pick skins from the in-game Skinmod panel, with per-skin custom sound effects. Ghosts and clones (replays, multiplayer players) are reskinned too, without touching save data.

Part of [Recharge](https://github.com/SumDumIdiut/recharge): the app pulls this repo into its mods folder and `build-loader.ps1 -ModsDir` compiles it against the game's `Managed` folder.

## Indicator art

A skin folder can also hold optional art for the dash and double-jump indicators floating by the player. The game's own indicator code keeps running (position, bobbing, animation); the skin is just drawn over it.

- `dash.png` - a grid sheet like the player's: **7 columns x 2 rows**, row 1 = the Front glow's 7 animation frames, row 2 = the Back glow's 5 frames (the last two cells are unused). Start from [`templates/dash-template.png`](templates/dash-template.png), which holds the vanilla frames on the magenta guide grid. Any cell size works (the template uses 256px cells for a 64px game sprite). A plain image that isn't on the grid is drawn as a static Front glow, and the Back glow is hidden.
- `doublejump.png` (or `jump.png`) - a single image, drawn over the double-jump indicator. Start from [`templates/doublejump-template.png`](templates/doublejump-template.png).

Both are scaled to cover the same area as the vanilla sprite, and are never taken as the skin's own player image (which is the first other image in the folder).

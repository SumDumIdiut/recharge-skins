# recharge-skins (Skinmod)

Player reskin mod for *IGTAP*: pick skins from the in-game Skinmod panel, with per-skin custom sound effects. Ghosts and clones (replays, multiplayer players) are reskinned too, without touching save data.

Part of [Recharge](https://github.com/SumDumIdiut/recharge): the app pulls this repo into its mods folder and `build-loader.ps1 -ModsDir` compiles it against the game's `Managed` folder.

## Indicator art

A skin folder can also hold optional images for the dash and double-jump indicators floating by the player:

- `dash.png` - replaces the dash indicators
- `doublejump.png` (or `jump.png`) - replaces the double-jump indicators

They're scaled to the vanilla indicator's size, and never treated as the skin's own player image (which is the first other image in the folder). `Player.log` lists what each indicator is made of (`[CustomSkins] dash[...] renderers`), which helps when tuning the art.

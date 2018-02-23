# ArcaeaSim

An [Arcaea](https://arcaea.lowiro.com/) simulator for viewing; no playing support is planned in a near future.

ArcaeaSim is built on [MilliSim](https://github.com/hozuki/MilliSim). Thus this is also a demonstration of the possibility of MilliSim.

ArcaeaSim is in alpha phase so please don't expect a splendid visual effect.
However, neither is it expected to go beyond the line to be a clone of Arcaea.

[Demo video](https://www.bilibili.com/video/av19881005/) (even before the initial commit; a very early technical preview so it may not look very appealing)

## Usage

*This section is for end users, using packaged binaries and resources.*

There are four test songs included in the repo:

| [Grievous Lady](http://lowiro.wikia.com/wiki/Grievous_Lady) | [Metallic Punisher](http://lowiro.wikia.com/wiki/Metallic_Punisher_%28Song%29) | [Red and Blue](http://lowiro.wikia.com/wiki/Red_and_Blue) | [Snow White](http://lowiro.wikia.com/wiki/Snow_White) |
|---|---|---|---|
| <img src="ArcaeaSim/Contents/game/grievouslady/base_256.jpg" width="128" /> | <img src="ArcaeaSim/Contents/game/metallicpunisher/base_256.jpg" width="128" /> | <img src="ArcaeaSim/Contents/game/redandblue/base_256.jpg" width="128" /> | <img src="ArcaeaSim/Contents/game/snowwhite/base.jpg" width="128" /> |

1. You can set the window properties (e.g. window size) in `Contents/app.config.yml`.
2. You can set the background image properties in `Contents/config/background_image.yml`.
3. You can configure which beatmap to load in `Contents/config/beatmap_loader.yml`.
4. You can set the background music properties in `Contents/config/audio_controller.yml`.
5. You can filter out the plugins that you don't want (e.g. the debug info overlay) by commenting them out (adding a "#" at the start of the line) in `Contents/plugins.yml`. But remember to keep the essential plugins.

## Development

*This section is for developers.*

Before you start, please read [Starting.md](docs/Starting.md).

For building the solution, please read [Building.md](docs/Building.md).

## Contributing

Contributions (especially PRs) are welcome because I probably do not have much time digging too deep into this, or simply doing maintenance work.
Therefore it is not possible to make it good-looking (like that in Arcaea) only by myself.

Remember to fork this repo, create your own feature branch, and make a PR. Thank you.

Use English wherever you can. This makes it easier for contributors from various places to collaborate. :)

## License

The code is open-sourced under BSD 3-Clause Clear license.

However, the test music, beatmap, and image files are copyrighted materials of lowiro Games,
all rights reserved by their composers and beatmap makers.

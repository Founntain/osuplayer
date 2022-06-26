<p align="center">
    <img src="https://7.founntain.dev/IY26BPcE.png" />
</P>

# osu!player
[![CodeFactor](https://www.codefactor.io/repository/github/osu-player/osuplayer/badge)](https://www.codefactor.io/repository/github/osu-player/osuplayer)
[![GitHub release](https://img.shields.io/github/release-pre/osu-player/osuplayer.svg)](https://github.com/founntain/osuplayer/releases/latest)
![](https://img.shields.io/github/languages/code-size/osu-player/osuplayer)
![](https://img.shields.io/github/repo-size/osu-player/osuplayer)
![](https://img.shields.io/tokei/lines/github/osu-player/osuplayer)
![](https://img.shields.io/github/issues/osu-player/osuplayer?color=red)
![](https://img.shields.io/github/contributors/osu-player/osuplayer?color=blueviolet)  
[![CI](https://github.com/osu-player/osuplayer/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/osu-player/osuplayer/actions/workflows/dotnet.yml)
[![.NET Publish](https://github.com/osu-player/osuplayer/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/osu-player/osuplayer/actions/workflows/dotnet-publish.yml)


This is the *official* **osu!player** (Avalonia) repository and is mainly developed by [@Founntain](https://github.com/Founntain), with help of [@Cesan](https://github.com/Cesan).  
This repository is a complete rewrite of the osu!player plus, so keep in mind that some stuff is still *work in progress*, with the *ultimate goal to go cross-platform*.  
So if we speak osu!player in this repository we mean the this (Avalonia) version. If you want to refer to other versions of the osu!player mention them by their name, either *osu!player **plus*** or *osu!player **legacy*** (osu!player versions before the plus).  
 
osu!player is a music player for *osu!* with the focus of playing your osu! songs **without having to start osu!**.

If you want to contribute, feel free to fork this repository, read the [contributing information](https://github.com/osu-player/osuplayer.git) and head to the [official osu!player discord](https://discord.gg/RJQSc5B) (not mandetory, but helpful). There you can add all valuable ideas and discuss other stuff regarding development.

## ☝️ Requirements

#### osu!player requirements
✔️ A working computer  
✔️ .NET 6 or later installed  
✔️ osu! installed with an **osu!.db file** or **osu!lazer client.realm** *(Beatmaps imported in osu!)*  
✔️ An internet connection if you want to use your osu!player plus profile

#### Download osu!player
To download the osu!player head to our [release](https://github.com/osu-player/osuplayer/releases) section to download the latest release.  
You can also build the project for yourself; see the section below!

## ⚒️Building the project
 - Clone / Download the source
 - Open it with Visual Studio, Visual Studio Code, Rider or an IDE of your choice that supports C# and .NET
 - Run `dotnet restore` (or IDE tools) to restore all packages and dependencies
 - Build/Run the project

## 👋 Contributing to the project
#### ☝️Requirements
 - .NET 6 SDK
 - [Avalonia .NET Templates](https://github.com/AvaloniaUI/avalonia-dotnet-templates)
 - [Check out the Avalonia getting started](https://github.com/AvaloniaUI/Avalonia#-getting-started)
 - *Have a **decent understanding of the internal osu!** stucture and have knownledge of osu! (the game) aswell.*

#### 🚀 How to contribute
 - Make a fork of this repository
 - Implement your ideas and features
 - Make a pull request (PR) on this repository
 - Pray that I accept your PR 😂 (I'm obviously joking)
 - Profit 📈

#### ⚠️ Please keep in mind
You should implement features that are asked for and not once you like or think will be good additions.
Best practice is, that we discuss (new) features and if they are needed. So don't be afraid to ask.  
**We appreciate your ideas and feedback!**

## 📦 Dependencies
| Dependency                                                   | Description                                       |
|--------------------------------------------------------------|---------------------------------------------------|
| [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)         | The UI-Framework                                  |
| [ManagedBass](https://github.com/ManagedBass/ManagedBass)    | The Audio-Engine                                  |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)           | Used for reading and editing various config files |

## ✨ Special thanks
- ***SourRaindrop***: for creating a lot of custom images and assets like our logo

## 🪛 Features that are missing to have the full osu!player plus feature set

#### 🔧 Features with lower priority
- [x] Audio-Equalizer 
- [ ] Localization 
- [ ] Language Editor 
- [ ] Language Manager to upload and download translations   
- [ ] Hotkey support  
- [ ] Export songs to directory  
- [ ] Custom themes 
- [ ] Miniplayer to save some space  
- [ ] Synced play via osu!player API  

#### 🎱 Stop asking for it
❌ Steering wheel support

## 📫 Contact
- ✉️ 7@founntain.dev
- 📣 [Discord](https://discord.gg/RJQSc5B)

## ❤️ Supported by
<img width=150 height=150 src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains Logo (Main) logo.">

## 🖼️ Screenshots

![](https://7.founntain.dev/xR5yZCvY.png)
![](https://7.founntain.dev/JZvjRNY4.png)

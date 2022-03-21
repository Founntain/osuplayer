<p align="center">
    <img src="https://7.founntain.dev/IY26BPcE.png" />
</P>

This is the *official* **osu!player** (Avalonia) repository and is mainly developed by [@Founntain](https://github.com/Founntain), with help of [@Cesan](https://github.com/Cesan).  
This repository will be a complete rewrite of osu!player plus, so keep in mind this is all *work in progress*, with the *ultimate goal to go cross-platform*.  
So if we speak osu!player in this repository we mean the Avalonia (this) version. If you want to refer to other versions of the osu!player mention them by their name, either *osu!player **plus*** or *osu!player **legacy*** (osu!player versions before the plus).  

osu!player is a music player for *osu!* with the focus of playing your osu! songs **without having to start osu!**.

If you want to contribute, feel free to fork this repository, read the [contributing information](https://github.com/Founntain/osuplayer.git) and head to the [official osu!player discord](https://discord.gg/RJQSc5B) (not mandetory, but helpful). There you can add all valuable ideas and discuss other stuff regarding development.

## ☝️ Requirements

#### osu!player requirements
✔️ A working computer  
✔️ .NET 6 or later installed  
✔️ osu! installed with an **osu!.db file** *(Beatmaps imported in osu!)*  
✔️ An internet connection for the first start, to get language files etc.

#### Download osu!player
If you want to try out our current pre-releases head to our [release](https://github.com/Founntain/osuplayer/releases) section to download the latest pre-release
Or build the project for yourself, see the section below

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

## osu!player todo list

#### 🛠️ Features with higher priority
- [x] Import songs from osu! via osu!.db  
- [x] Play songs
- [x] Favorise songs and create custom playlists  
- [ ] Create playlists from collection.db   
- [x] Nightcore and daycore function  
- [ ] User profiles, for stats, uploading themes, languages and other stuff  
- [ ] XP and Levelsystem  
- [ ] Patchnotes inside the client  

#### 🔧 Features with lower priority
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

## Contact
- ✉️ 7@founntain.dev
- 📣 [Discord](https://discord.gg/RJQSc5B)

## Screenshots

None yet

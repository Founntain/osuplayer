<p align="center">
    <img src="https://7.founntain.dev/IY26BPcE.png" />
</P>

This is the *official* **osu!player** (Avalonia) repository and is mainly developed by [@Founntain](https://github.com/Founntain), with help of [@Cesan](https://github.com/Cesan).  
This repository will be a complete rewrite of the osu!player plus so keep in mind that this all here is WIP, with the *ultimate goal to go cross-platform*.  
So if we speak osu!player in this repository we mean the Avalonia (this) version. If you want to refer to other versions of the osu!player mention them by their name, either osu!player **plus** or osu!player **legacy** (osu!player versions before the plus)  

osu!player is a music player for *osu!* with the focus of playing your osu! songs **without having to start osu!**.

If you want to contribute, feel free to fork the repository, read the [contributing information](https://github.com/Founntain/osuplayer.git) and head to the [official osu!player discord](https://discord.gg/RJQSc5B). There you can add all valuable ideas and discuss other stuff regarding development.

## Requirements

#### osu!player requirements
✔ A working computer  
✔ .NET 6 or later installed  
✔ osu! installed with an **osu!.db file** *(Beatmaps imported in osu!)*  
✔ An internet connection for the first start, to get language files etc.

#### Download osu!player
No official downloads yet, if you want to try it out.  
You have to build the project for yourself, see the section below

The osu!player plus has a build in updater, to keep the player up to date. You will get notified when an update is downloaded and ready to be installed.

## ⚒️Building the Project
 - Clone / Download the source
 - Open it with Visual Studio, Visual Studio Code, Rider or C# IDE of your choice
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
 - Make a pull request on this repository
 - Pray that I accept it PR 😂 (I'm obviously joking)
 - Profit 📈

#### ⚠️ Please keep in mind
That you should implement features that are asked and not because you think you like it it will be a good addition.
Best practice is, that we discuss (new) features and if they are needed. So fear not to ask us.   
**We appreciate your ideas and feedback!**

## 📦 Dependencies
| Dependency                                                   | Description                                       |
|--------------------------------------------------------------|---------------------------------------------------|
| [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)         | The UI-Framework                                  |
| [ManagedBass](https://github.com/ManagedBass/ManagedBass)    | The Audio-Engine to play all the songs            |
| [EntityFramework](https://docs.microsoft.com/de-de/ef/core/) | Used for out local database                       |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)           | Used for reading and editing various config files |

## osu!player todo list

#### 🛠️ Features with higher priority
- [ ] Import songs from osu! via osu!.db  
- [ ] Favorise songs and create custom playlists  
- [ ] Create playlists from collection.db   
- [ ] Nightcore and daycore function  
- [ ] User profiles, for stats, uploading themes, languages and other stuff  
- [ ] XP and Levelsystem  
- [ ] Patchnotes inside the client  

#### 🔧 Features with lower priority
- [ ] Language Manager to upload and download translations  
- [ ] Language Editor  
- [ ] Hotkey support  
- [ ] Localization 
- [ ] Export songs to directory  
- [ ] Custom themes 
- [ ] Miniplayer to save some space  
- [ ] Synced play via osu!player API  

#### 🎱 Stop asking for it
❌ Steering wheel support

## Source Code

The source code of the *legacy osu!player* was never released and never will.  
However the osu!player (Avalonia) will be open source from the beginning!

## Screenshots

None yet

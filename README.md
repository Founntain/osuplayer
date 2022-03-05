<p align="center">
    <img src="https://7.founntain.dev/IY26BPcE.png" />
</P>

This is the *official* **osu!player (Avalonia)** repository and is mainly developed by [@Founntain](https://github.com/Founntain), with help of [@Cesan](https://github.com/Cesan).  
This repository will be a complete rewrite of the osu!player plus so keep in mind that this all here is WIP.  
osu!player is a music player for *osu!* with the focus of playing your osu! songs **without having to start osu!**.

If you want to contribute, feel free to fork the repository and get in head to the [official osu!player discord](https://discord.gg/RJQSc5B). There you can add all valuable ideas and discuss other stuff regarding development.

## Requirements

#### osu!player (Avalonia) requirements
✔ A working computer 
✔ .NET 6 or later installed  
✔ osu! installed with an **osu!.db file** *(Beatmaps imported in osu!)*  
✔ An internet connection for the first start, to get language files etc.

#### Download osu!player (Avalonia)
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
 - Pray that I accept it 😂 (I'm joking)
 - Profit 📈

Please keep in mind that you should implement features that are asked and not because you think you like it it will be a good addition.
Best practice is, that we discuss (new) features and if they are needed. So fear not to ask us.   
**We appreciate your ideas!**

## 📦 Dependencies
| Dependency                                                   | Description                                       |
|--------------------------------------------------------------|---------------------------------------------------|
| [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)         | The UI-Framework we are using                     |
| [ManagedBass](https://github.com/ManagedBass/ManagedBass)    | The Audio-Engine to play all the songs            |
| [EntityFramework](https://docs.microsoft.com/de-de/ef/core/) | Used for out local database                       |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)           | Used for reading and editing various config files |

## osu!player todo list

- [ ] Import songs from osu! via osu!.db  
- [ ] Favorise songs and create custom playlists  
- [ ] Miniplayer to save some space  
- [ ] Discord RPC  
- [ ] Synced play via Discord or osu!player API  
- [ ] Nightcore and daycore function  
- [ ] Export songs to directory  
- [ ] Custom themes  
- [ ] User profiles, for stats, uploading themes, languages and other stuff  
- [ ] Localization  
- [ ] Create playlists from collection.db  
- [ ] Language Manager to upload and download translations  
- [ ] Language Editor  
- [ ] Hotkey support  
- [ ] XP and Levelsystem  
- [ ] Patchnotes inside the client  
❌ Controller and steering wheel support

## Source Code

The source code of the *legacy osu!player* was never released and never will.  
However the osu!player (Avalonia) will be open source from the beginning!

## Screenshots

None yet

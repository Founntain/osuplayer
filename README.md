<p align="center">
    <img src="https://7.founntain.dev/IY26BPcE.png" />
</P>

# osu!player
[![CodeFactor](https://www.codefactor.io/repository/github/Founntain/osuplayer/badge)](https://www.codefactor.io/repository/github/Founntain/osuplayer)
[![GitHub release](https://img.shields.io/github/release-pre/Founntain/osuplayer.svg)](https://github.com/founntain/osuplayer/releases/latest)
![](https://img.shields.io/github/languages/code-size/Founntain/osuplayer)
![](https://img.shields.io/github/repo-size/Founntain/osuplayer)
![](https://img.shields.io/tokei/lines/github/Founntain/osuplayer)
![](https://img.shields.io/github/issues/Founntain/osuplayer?color=red)
![](https://img.shields.io/github/contributors/Founntain/osuplayer?color=blueviolet)  
[![CI](https://github.com/Founntain/osuplayer/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Founntain/osuplayer/actions/workflows/dotnet.yml)
[![.NET Publish](https://github.com/Founntain/osuplayer/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/Founntain/osuplayer/actions/workflows/dotnet-publish.yml)
 
osu!player is a music player for *osu!* with the focus of playing your osu! songs **without having to start osu!**.
This is the *official* **osu!player** repository and is mainly developed by [@Founntain](https://github.com/Founntain), with help of [@Cesan](https://github.com/Cesan).  

If you want to contribute, feel free to fork this repository, read the [contributing information](https://github.com/osu-player/osuplayer#-contributing-to-the-project) and head to the [official osu!player discord](https://discord.gg/RJQSc5B) (not mandetory, but helpful). There you can add all valuable ideas and discuss other stuff regarding development.

## ☝️ Requirements

#### osu!player requirements
✔️ A working computer  
✔️ .NET 6 or later installed  
✔️ osu! installed with an **osu!.db file** or **osu!lazer client.realm** *(Beatmaps imported in osu!)*  
✔️ An internet connection if you want to use your osu!player plus profile

#### Download osu!player
To download the osu!player head to our [release](https://github.com/Founntain/osuplayer/releases) section to download the latest release.  
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
| Dependency                                                        | Description                                       |
|-------------------------------------------------------------------|---------------------------------------------------|
| [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)              | The UI-Framework                                  |
| [ManagedBass](https://github.com/ManagedBass/ManagedBass)         | The Audio-Engine                                  |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)                | Used for reading and editing various config files |
| [discord-rpc-sharp](https://github.com/Lachee/discord-rpc-csharp) | Used to display Discord RPC                       |

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

## 🎵 We are the creators of the osu!player (about us)

### 🦊 Founntain

<a href="https://github.com/Founntain">
  <img style="border-radius: 50%;" align="right" width=200 height=200 src="https://osuplayer.founntain.dev/api/users/getProfilePicture?username=Founntain" />
</a>
Hey there my name is Founntain!
A bit about myself. I'm currently `currentYear - 1999` years old and from Germany. 
Currently I'm working for a medium sized software developing company as Software Consultant.

**Languages I use:**
+ C# *(For most of my projects)*
+ HTML, CSS and Typescript *(for web stuff)*
+ Java *(mostly for Minecraft plugin development)*

In 2016 I had my first programming contact in my IT-School. There we mostly developed in Java, but all stuff that we programmed in Java I tried to implement in C#
while learning it on my own.  

In 2017 I started development on the first versions of the osu!player in WPF and .NET-Framework 4.6. It looked bad, it feeled bad and had bad performance.
But let's be honest what do expect from someone who never used WPF at all and had not much C# experience. 
On 1st of November 2017, the first version of the osu!player was released on the osu! forum.  

After a while Cesan joined me and we started working on it together until now and I'm really greatful for that. *Thanks buddy*

### 🌸 Cesan

<a href="https://github.com/Cesan">
  <img style="border-radius: 50%;" align="right" width=200 height=200 src="https://osuplayer.founntain.dev/api/users/getProfilePicture?username=Cesan" />
</a>
Hi, I'm Cesan. You can also call me Caro if you want ^^

I'm a self taught C# dev and currently studying Applied Computer Sciences at University.
I also work as an embedded C dev in the mean time.

I mostly use C# for everything I do because I think it's the most versatile and practical language for desktop development.
In university I learned C and Java altough I would never use Java personally.

As I joined development of the osu!player I mostly did design stuff in WPF as I understood it best, but now we both do more or less the same stuff because we have quite some experience with the osu!player by now, to make the player look and feel like how it is today.

Thanks for reading and have fun with the player, cheers.

## 📫 Contact
- ✉️ 7@founntain.dev
- 📣 [Discord](https://discord.gg/RJQSc5B)

## ❤️ Supported by
<img width=150 height=150 src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains Logo (Main) logo.">

## 🖼️ Screenshots

![](https://7.founntain.dev/xR5yZCvY.png)
![](https://7.founntain.dev/JZvjRNY4.png)  
### Miniplayer  
![](https://7.founntain.dev/ThrcgojY.png)

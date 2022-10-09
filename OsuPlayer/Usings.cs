global using System;
global using System.IO;
global using System.Linq;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using OsuPlayer.Extensions.Bindables;
global using OsuPlayer.IO.DbReader.DataModels;
global using OsuPlayer.IO.Storage.Config;
global using OsuPlayer.Modules.Audio;
global using OsuPlayer.Network.API.ApiEndpoints;
global using OsuPlayer.Network.Online;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OsuPlayer.Tests")]
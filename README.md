[![HitCount](http://hits.dwyl.io/33kk/roblox-stream-sniper.svg)](http://hits.dwyl.io/33kk/roblox-stream-sniper) [![Build status](https://ci.appveyor.com/api/projects/status/6xw088wua7shg8fv?svg=true)](https://ci.appveyor.com/project/33kk/roblox-stream-sniper)
# Roblox Stream Sniper
Join players in game.

# How does it work?
Roblox has api which shows all running servers of a place. It returns players avatar headshot urls and server join script. So we can fetch targets headshot url using another api and compare them. If we find a match we know which server to join using script api gives us. To join game we can open browser console on roblox website by pressing `CTRL+SHIFT+I` and selecting Console tab and paste script there.

# Requierements
 - .NET Core 3 runtime

# Usage

  -c, --cookie      Set your .ROBLOSECURITY cookie.

  -u, --user        Set target user ID.

  -n, --username    Set target user name.

  -p, --place       Set place ID target is in.

  -s, --search      Search for game by name and use start place of first result.
  
# Building
 - Install .NET Core 3 SDK
 - `dotnet build --configuration Release`

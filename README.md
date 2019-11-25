# Roblox Stream Sniper
Join players in game.

# How does it work?
Roblox has api which shows all running servers of a place. It returns players avatar headshot urls and server join script. So we can fetch targets headshot url using another api and compare them. If we find a match we know which server to join using script api gives us. To join game we can open browser console on roblox website by pressing `CTRL+SHIFT+I` and selecting Console tab and paste script there.

# Requierements
 - .NET Core 3 runtime

# Usage

  -c, --cookie      Specify your .ROBLOSECURITY cookie.

  -i, --userid      Target user ID.

  -n, --username    Target user name.

  -p, --placeid     Target game ID.

  -s, --game        Search for game by name and use first result.
  
# Building
 - Install .NET Core 3 SDK
 - `dotnet build --configuration Release`

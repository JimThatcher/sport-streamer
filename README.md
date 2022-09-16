# Live Sport Streaming Manager
This is a very early release of a system for providing HTML/JavaScript-based graphic overlays for streaming live sporting events. The system is composed of five primarry components:
1. A web server to access game-related data.
2. A SQLite database, managed through the web server.
3. A React-based front-end web app for managing the database.
4. A set of data-driven web pages that provide graphic overlays for:
  (a). Scoreboard (with team-specific graphics from the database, and live data feed from the Daktronics scoreboard controller), 
  (b). Player hilights - show the player picture, name, jersey number, and position after a key play, 
  (c). Sponsor ads - Show your sponsors some love by putting their ad on-screen during down-time in the game.
5. A plugin for Elgoto Stream Deck devices to simplify game-time live changes to the graphic overlays

The web server is built on ASP.NET Core, using Entity Framework to access the database. It provides a set of REST APIs for reading and updating the database.
The front-end web app is built from the base web app in rjwats esp8266-react project (https://github.com/rjwats/esp8266-react).
The graphic overlay pages ar basic HTML/JavaScript using server-sent-events to keep the display up-to-date with changes on the web server.
The Stream Deck plugin is based on the Stream Deck Plugin Template project (https://github.com/elgatosf/streamdeck-plugintemplate) provided by Elgato.

# How-to/Getting Started
## Build it yourself:
This project is built with .NET Core and ASP.NET Core. If you want to build it yourself, download or clone the repo, install the .NET Core SDK version 6, and from the root folder run "dotnet publish -c Release". After the build completes, you can switch to the ".\bin\Release\net6.0\publish" folder and run "sport-streamer.exe".
## Use the pre-built binary:
If you just want to download the precompiled system, start by downloading the "webAppBinaries.zip" from the Binaries folder. After the download completes, you will need to unblock the file in Windows before you will be able to use the included binaries. From File Explorer, right-click on the zip file and select "Properties". At the bottom of the General properties page you will see a checkbox option to Unblock the content downloaded from the Internet. This is part of Windows' protections against malware, so if you aren't comfortable trusting the prebuilt binaries, go back up to the Build it yourself section. Once you have unblocked the zip file, right-click on the zip file again and select the "Extract all ..." option, choose the folder you want to extract the app into, and click the Extract button. The extracted files will be inside a "publish" folder in your target directory. Open a command prompt to that "pubilsh" folder and run "sport-streamer.exe".
## Accessing the app:
Whichever approach you use, after you run the app it will start up the web server listening on http://localhost:5000 and https://localhost:5001. The default configuration does not include listening on external IP addresses. You can then open a browser to http://localhost:5000/index.html. When the web app starts, it will create an empty database as "games.db". 

![image](https://user-images.githubusercontent.com/6655043/190552713-56621e1b-6fbd-4585-98e9-40cbd50afa88.png)

The first time you sign in to the web app the app will accept the username and password you submit as the administrative user credentials, and stores those in the database. You'll need to remember that password for future sign-in. NOTE: The usernames and passwords are stored in clear text in the games.db database, and can be viewed by anyone with access to that file using simple SQLite tools. You should manage access to the app's root directory accordingly. 
Once in the app, you will be able to begin adding schools, games, players, and sponsors to your database by selecting the relevant item from the left menu.

![image](https://user-images.githubusercontent.com/6655043/190553203-70f7f687-7713-4f3a-afd5-1cd10bd9fcf7.png)

The information for each school (name, mascot, color, logo, and win/loss record) is used for the scoreboard overlay. (There are some problems currently in the code for adding the logo files. After a logo is uploaded it will not appear in the list of schools until you select the school from the list and delete the "C:\fakepath\" prefix from the file name. We'll work on fixing that.) When you upload a logo file from the Add school page the file is saved to the "publish\wwwroot\Images" folder.
Once you have at least two schools added to your database you can create a game between those schools. Adding at least one game is a minimum requirement for the scoreboard overlay to function.

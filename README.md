# Live Sport Streaming Manager
This is a very early release of a system for providing HTML/JavaScript-based graphic overlays for streaming live sporting events. First, a quick disclaimer: I'm a hobbyist with a passion for streaming our high school football games on YouTube. Although I've been a professional developer, that was MANY years ago, working in C, C++, and Java. This is all built in C#, HTML, JavaScript, and TypeScript using frameworks I'm not really familiar with. As you look through the code, if you have any current software development experience, you're going find code that is inefficient, hacky, and mayby just plain wrong. When you do, please submit an issue with recommendations to improve the code. I'm kind of proud of what this system can do, but I'm confident the code itself can be improved ... a lot. If you find this helpful, and you have software experience that could help improve it, I would really appreciate your help to really make this a great system.

### The system is composed of five main components:
1. A web server to access game-related data that provides REST APIs to access and manipulate the data.
2. A SQLite database, managed through the web server.
3. A React-based front-end web app for managing the database.
4. A set of data-driven web pages that provide graphic overlays for:
  (a). Scoreboard (with team-specific graphics from the database, and live data feed from the Daktronics scoreboard controller), 
  (b). Player hilights - show the player picture, name, jersey number, and position after a key play, 
  (c). Sponsor ads - Show your sponsors some love by putting their ad on-screen during down-time in the game.
5. A plugin for Elgoto Stream Deck devices to simplify game-time live changes to the graphic overlays.

The web server is built on ASP.NET Core, using Entity Framework to access the database. It provides a set of REST APIs for reading and updating the database and managing an in-memory database that holds game-time data for the clock and scoreboard (that data doesn't need to be saved after the game ends).
The front-end web app is built from the base web app in rjwats' esp8266-react project (https://github.com/rjwats/esp8266-react).
The graphic overlay pages ar basic HTML/JavaScript using server-sent-events to keep the display up-to-date with changes on the web server.
The Stream Deck plugin is based on the Stream Deck Plugin Template project (https://github.com/elgatosf/streamdeck-plugintemplate) provided by Elgato.
There is a very good chance that I've grabbed ideas and code for other parts of the system from some projects I haven't mentioned. Thank you to each of the developers of those projects. 

In developing this project I have used the GitHub Copilot AI-based "pair programmer" (https://github.com/features/copilot/). It's very impressive in what it can do as it recognizes patterns in the code I'm writing, and suggests what I'm likely planning to code next. Especially when I've been working in a new framework, like React, it has really helped me write the changes I wanted to make much more quickly than if I had to keep searching the web for how to do something I hadn't done before in React. It's not flawless - it'll happily help you reproduce bad coding patterns if that's what you've been doing recently. But this project is ready for others to start working with now because I've had a lot of assistance from Copilot.

# How-to/Getting Started
To get started with the streaming manager you can either download the compiled binaries, or start with the code and build it yourself.
## Build it yourself:
This project is built with .NET Core and ASP.NET Core. If you want to build it yourself, download or clone the repo, install the .NET Core SDK version 6 (available from https://dotnet.microsoft.com/en-us/download), and from the root folder of the source run "dotnet publish -c Release". After the build completes, you can switch to the ".\bin\Release\net6.0\publish" folder and run "sport-streamer.exe".
## Use the pre-built binary:
If you just want to download the precompiled system, start by installing the ASP.NET Core Hosting Bundle from https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime. That bundle includes both the .NET Core Runtime and the ASP.NET Core Runtime. Both are needed to run the web server. 

Once you have the .NET Core runtime installed, download the "webAppBinaries.zip" from the Binaries folder. After the download completes, you will need to unblock the file in Windows before you will be able to use the included binaries. From File Explorer, right-click on the zip file and select "Properties". At the bottom of the General properties page you will see a checkbox option to Unblock the content downloaded from the Internet. This is part of Windows' protections against malware, so if you aren't comfortable trusting the prebuilt binaries, go back up to the Build it yourself section. Once you have unblocked the zip file, right-click on the zip file again and select the "Extract all ..." option, choose the folder you want to extract the app into, and click the Extract button. The extracted files will be inside a "publish" folder in your target directory. Open a command prompt to that "pubilsh" folder and run "sport-streamer.exe".
## Accessing the app:
Whichever approach you use, after you run the app it will start up the web server listening on http://localhost:5000 and https://localhost:5001. The default configuration does not include listening on external IP addresses. You can then open a browser to http://localhost:5000/index.html. When the web app starts, it will create an empty database as "games.db". 

![image](https://user-images.githubusercontent.com/6655043/190552713-56621e1b-6fbd-4585-98e9-40cbd50afa88.png)

The first time you sign in to the web app the app will accept the username and password you submit as the administrative user credentials, and stores those in the database. You'll need to remember that password for future sign-in. NOTE: The usernames and passwords are stored in clear text in the games.db database, and can be viewed by anyone with access to that file using simple SQLite tools. You should manage access to the app's root directory accordingly. 
Once in the app, you will be able to begin adding schools, games, players, and sponsors to your database by selecting the relevant item from the left menu.

![image](https://user-images.githubusercontent.com/6655043/190553203-70f7f687-7713-4f3a-afd5-1cd10bd9fcf7.png)

The information for each school (name, mascot, color, logo, and win/loss record) is used for the scoreboard overlay. (There are some problems currently in the code for adding the logo files. After a logo is uploaded it will not appear in the list of schools until you select the school from the list and delete the "C:\fakepath\" prefix from the file name. We'll work on fixing that.) When you upload a logo file from the Add school page the file is saved to the "publish\wwwroot\Images" folder.
Once you have at least two schools added to your database you can create a game between those schools. Adding at least one game is a minimum requirement for the scoreboard overlay to function.

![image](https://user-images.githubusercontent.com/6655043/190566426-aaea94c6-d720-43dd-9ffc-103fb13d526f.png)

If you have physical access to the in-stadium scoreboard controller (and if it's a Daktronics controller) the web app can parse the real-time data (RTD) output from the 25-pin serial port on the controller. Connect the serial output to an RS232-to-USB converter into the laptop running the web app and your streaming software and the web app should auto-connect to the COM port and begin parsing the RTD stream to feed your scoreboard overlay. The overlay currently pulls clock, score, period, down, distance, possession, time out, time outs left, and flag from the Daktronics RTD. So far every stadium I've streamed from uses a Daktronics controller. If others have access to other controllers being used in your stadiums, and those controllers provide an RTD output through RS232 or USB, and the manufacturer provides documentation of the RTD protocol, I'll work on adding support for other controllers.

The information in the Players page (jersey number, name, position, year, and school/team) is used for the player hilight overlay. We don't currently use the height and weight, but those are included to support using the database for other roster-related uses. The primary use of the "school/team" field is currently for sorting players in the menu when adding player hilight actions in the Stream Deck plugin. 

![image](https://user-images.githubusercontent.com/6655043/190565457-069e7e55-9f53-4f4b-92a5-56e3f210c408.png)

For the sponsor advertisement overlay, we really only need the image to place in the ad space. The image should be roughly 2:1 aspect ratio.

![Contoso](https://user-images.githubusercontent.com/6655043/190562815-dc745ff4-0999-44fd-b032-990be5eb8795.png)

That image will be transposed into the background to create the overlay for that sponsor.

![image](https://user-images.githubusercontent.com/6655043/190562364-ea72e132-948d-4d61-a20e-51b6624077f8.png)

## Adding overlays to your streaming software:
The overlays are each provided as HTML/JavaScript web pages that can be added as Browser sources into OBS Studio and similar streaming software. The pages to add are:
* Scoreboard: http://localhost:5000/overlay/ScoreBug.html
* Player Hilight: http://localhost:5000/overlay/PlayerHL.html
* Ads: http://localhost:5000/overlay/SponsorAd.html 
The pages each respond to server-sent envents to show or hide the overlay. Hiding the overlay makes the entire page transparent. This allows the sources to always be set as visible in OBS Studio, with their actual visibility being controlled through REST API requests that are sent to the server in response to button actions on your Stream Deck.
The web pages are all in the "publish\wwwroot\overlays" directory. If they don't meet your needs as they are, feel free to edit the JavaScript to your preferences. Currently the scoreboard is optimized for American Football. If you customize one for a different sport, please submit an issue and a PR to add that into the project. I'd love to get developers with HTML/CSS/JavaScript experience to help take these from functional to beautiful.
## Adding the Stream Deck plugin:
You can download the source for the Stream Deck plugin from the https://github.com/JimThatcher/SDPlugins-Sportscaster repository. Or you can download the installable plugin, com.evanscreekdev.sportscaster.streamDeckPlugin, from the Binaries folder. If you download the installable plugin you will need to unblock it from the file properties in File Explorer before you double-click on it to install it to your Stream Deck plugins folder. Oh, you'll need to have the latest Stream Deck software installed before you try to install the plugin. Once installed, you will see a "Sports Stream Manager" section added to your Stream Deck actions. See the README in the plugin repository for more information on using the plugin.

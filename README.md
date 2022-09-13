# Live Sport Streaming Manager
This is a very early release of a system for providing HTML/JavaScript-based graphic overlays for streaming live sporting events. The system is composed of five primarry components:
1. A web server to access game-related data.
2. A SQLite database, managed through the web server.
3. A React-based front-end web app for managing the database.
4. A set of data-driven web pages that provide graphic overlays for:
  a. Scoreboard (with team-specific graphics from the database, and live data feed from the Daktronics scoreboard controller)
  b. Player hilights - show the player picture, name, jersey number, and position after a key play
  c. Sponsor ads - Show your sponsors some love by putting their ad on-screen during down-time in the game
5. A plugin for Elgoto Stream Deck devices to simplify game-time live changes to the graphic overlays

The web server is built on ASP.NET Core, using Entity Framework to access the database. It provides a set of REST APIs for reading and updating the database.
The front-end web app is built from the base web app in rjwats esp8266-react project (https://github.com/rjwats/esp8266-react).
The graphic overlay pages ar basic HTML/JavaScript using server-sent-events to keep the display up-to-date with changes on the web server.
The Stream Deck plugin is based on the Stream Deck Plugin Template project (https://github.com/elgatosf/streamdeck-plugintemplate) provided by Elgato.

#How-to/Getting Started
(coming soon)

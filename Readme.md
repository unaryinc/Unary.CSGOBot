![CSGOBotBanner](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/Banner.png)

External chat bot for CSGO with a lot of cool commands to pass time while you or your friends are dead during the match.

- [Navigation](#navigation)
    - [Installation](#installation)
    - [Will I get banned](#will-i-get-banned)
    - [Bot is not typing anything](#bot-is-not-typing-anything)
    - [Logo](#logo)
    - [Thirdparty software](#thirdparty-software)

## Installation

* Download one of the latest releases [from here](https://github.com/unaryinc/Unary.CSGOBot/releases/latest)
* Unpack `.zip` somewhere
* Launch `Unary.CSGOBot.exe`
* Follow the installation steps provided by the app

After the installation, boot up the game, join the server, and then type `/start` in the chat and `WAIT` for bot to respond.
In order to stop the bot type `/stop` in the chat, `stop_bot` in console, or close the app.

## Will I get banned

* Short answer - `It is impossible to get banned`

Long, more technical answer:

We are not using any internal hacking/modifications of the game in order to make this bot functional. Running this bot in parallel to the game is as safe as any other program that does not know that CSGO even exists on your PC.

When bot needs to focus on CSGO - it asks OS to make CSGO window active. When bot needs to read some input from the game - it reads console logs that the game itself outputs in the `console.log` file. When bot needs to write something to the chat, it fill `.cfg` file in your `cfg` folder with necessary commands, then emulates press of the `F7` key, which then executes that `.cfg` config that bot filled for the game. 

You might think that emulation of making CS window active or keypresses might be a reason that `VAC` will detect those actions, but that would mean that every other keypress emulators (things like auto clickers, AutoHotKey, etc) would be also banned, but they are not.

## Bot is not typing anything
Bot cant emulate binds while you are in pause menu/chat. In order to resume bot output, you need to close them, and be in game without anything additional opened on your screen (scoreboard/buy menu does not count).

## Logo

### 2691x2691
![2691x2691](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/2691.png)
### 1024x1024
![1024x1024](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/1024.png)
### 184x184
![184x184](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/184.png)

## Thirdparty software:
https://www.vb-audio.com/Cable/

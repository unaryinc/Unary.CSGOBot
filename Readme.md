![CSGOBotBanner](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/Banner.png)

External chat bot for CSGO with a lot of cool commands to pass time while you or your friends are dead during the match.

- [Navigation](#navigation)
    - [Installation](#installation)
    - [Will I get banned](#will-i-get-banned)
    - [Bot is not typing anything](#bot-is-not-typing-anything)
    - [Thirdparty software](#thirdparty-software)

## Installation

* Close the game if you still have it running
* Add `-condebug` to your launch options of CSGO
* Type `bind f7 "exec botexec"` into the console
* Download one of the latest releases [from here](https://github.com/unaryinc/Unary.CSGOBot/releases/latest)
* Unpack `.zip` somewhere
* Follow the installation steps provided by the app

### There is only one thing that you need to keep in mind while using this bot - If you need to open a pause menu (Accessed by pressing `ESC`) then you should also close it by pressing `ESC` again. 
### !!! DO NOT PRESS `RESUME GAME` ON THE TOP LEFT PART OF THE SCREEN WHILE IN PAUSE MENU, OR IT WILL BREAK ALL THE STATE HANDLING LOGIC !!!
![Forbidden button](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/ForbiddenButton.png)

After the installation, boot up the game, join the server, and then type `/start` in the chat and `WAIT` for bot to respond.

## Will I get banned

* Short answer - `NO`

Long, more technical answer:

We are not using any internal hacking/modifications of the game in order to make this bot functional. Running this bot in parallel to the game is as safe as any other program that does not know that CSGO even exists on your PC.

When bot needs to focus on CSGO - it asks OS to make CSGO window active. When bot needs to read some input from the game - it reads console logs that the game itself outputs in the `console.log` file. When bot needs to write something to the chat, it fill `.cfg` file in your `cfg` folder with necessary commands, then emulates press of the `F7` key, which then executes that `.cfg` config that bot filled for the game. 

You might think that emulation of making CS window active or keypresses might be a reason that `VAC` will detect those actions, but that would mean that every other keypress emulators (things like auto clickers, AutoHotKey, etc) would be also banned, but they are not.

## Bot is not typing anything
Bot cant emulate binds while you are in pause menu/chat. In order to resume bot output, you need to close them, and be in game without anything additional opened on your screen (scoreboard does not count).

## Thirdparty software:
https://www.vb-audio.com/Cable/

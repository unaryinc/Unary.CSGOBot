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
* Launch the game
* Type `bind f7 "exec botexec"` into the console
* Download one of the latest releases [from here](https://github.com/unaryinc/Unary.CSGOBot/releases/latest)
* Unpack `.zip` somewhere
* Join in the match
* Follow the installation steps provided by the app

### ! THERE ARE TWO MAJOR THINGS THAT YOU NEED TO KEEP IN MIND WHILE RUNNING THIS BOT !
### In order to close the pause menu (Accessed by `ESC`) you need to press `ESC` again. Do not use the `RESUME GAME` button as it would break state handling logic!
![Resume Button](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/ResumeButton.png)
### In order to close the chat window you need to press `ESC` or send your message with the `ENTER` key. Do not use the `SEND` button or `Right mouse button` to close the chat window as it would break state handling logic!
![Send Button](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/SendButton.png)

We are sorry for the inconvenience, but since this is a fully external bot we cant reliably check for specific things like in-game button presses.

After the installation, boot up the game, join the server, and then type `/start` in the chat and `WAIT` for bot to respond.
In order to stop the bot type `/stop` in the chat, `bot_stop` in console, or close the app.

## Will I get banned

* Short answer - `It is impossible for you to get banned from this bot`

Long, more technical answer:

We are not using any internal hacking/modifications of the game in order to make this bot functional. Running this bot in parallel to the game is as safe as any other program that does not know that CSGO even exists on your PC.

When bot needs to focus on CSGO - it asks OS to make CSGO window active. When bot needs to read some input from the game - it reads console logs that the game itself outputs in the `console.log` file. When bot needs to write something to the chat, it fill `.cfg` file in your `cfg` folder with necessary commands, then emulates press of the `F7` key, which then executes that `.cfg` config that bot filled for the game. 

You might think that emulation of making CS window active or keypresses might be a reason that `VAC` will detect those actions, but that would mean that every other keypress emulators (things like auto clickers, AutoHotKey, etc) would be also banned, but they are not.

## Bot is not typing anything
Bot cant emulate binds while you are in pause menu/chat. In order to resume bot output, you need to close them, and be in game without anything additional opened on your screen (scoreboard does not count).

## Thirdparty software:
https://www.vb-audio.com/Cable/

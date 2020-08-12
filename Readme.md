![CSGOBotBanner](https://raw.githubusercontent.com/unaryinc/Unary.CSGOBot/master/Images/Banner.png)

External chat bot for CSGO with a lot of cool commands to pass time while you or your friends are dead during the match.

- [Navigation](#navigation)
    - [Installation](#installation)
    - [Will I get banned](#will-i-get-banned)
    - [Thirdparty software](#thirdparty-software)

## Installation

* Close the game if you still have it running
* Add `-condebug` to your launch options of CSGO
* Open the game, and remove/rebind these keys:
    - `Walk`/`Shift` - `Alt`
    - `Radio message`/`Ctrl` bind - Should not be bound
    - `Home` - Should not be bound
    - `Healthshot`/`X` - `O`
    - `Enter` - Should not be bound
    - `V` - Should not be bound
* Type `bind f7 "exec botexec"` into the console
* Download one of the latest releases [from here](https://github.com/unaryinc/Unary.CSGOBot/releases/latest)
* Unpack `.zip` somewhere
* Follow the installation steps provided by the app

After the installation, boot up the game, join the server, and then type `/start` in the chat.

## Will I get banned

* Short answer - `NO`

Long, more technical answer:

We are not using any internal hacking/modifications of the game in order to make this bot functional. Running this bot in parallel to the game is as safe as any other program that does not know that CSGO even exists on your PC.

When bot needs to focus on CSGO - it asks OS to make CSGO window active. When bot needs to read some input from the game - it reads console logs that the game itself outputs in the `console.log` file. When bot needs to write something to the chat, it fill `.cfg` file in your `cfg` folder with necessary commands, then emulates press of the `F7` key, which then executes that `.cfg` config that bot filled for the game. 

You might think that emulation of making CS window active or keypresses might be a reason that `VAC` will detect those actions, but that would mean that every other keypress emulators (things like auto clickers, AutoHotKey, etc) would be also banned, but they are not.

## Thirdparty software:
https://www.vb-audio.com/Cable/

ModpackUpdateManager - Automatic Modpack Updater for Minecraft

ModpackUpdateManager is a program designed to simplify the process of updating Minecraft modpacks. 
If you have a personal Minecraft modpack that you play on, you know how time-consuming it can be to update it from one version to another. 
ModpackUpdateManager is here to help.


How it works:

The program scans a provided mod directory and goes through every jar file to look for the mods.toml file inside it.
The algorithm parses the displayName inside it and uses the CefSharp library to search Curseforge for it.

In automatic mode, the program finds the best fit in the search results for the given API and Minecraft version and downloads it.

In manual mode, the program will search for you but ask for a manual choice in the list. Then, it will automatically download the mod upon confirmation.


Features:

Confirmation system: If there is a discrepancy between the downloaded displayName in the toml file and the source, the mod will be moved to an invalid folder. The user can then choose the course of action.

Configurable content: Various URLs used by the system, gameFlavorIds.json, gameVersionIds.json, searchTermBlacklist.json, and GetModSearchResults.js can be modified to suit your needs.

Sustainable traffic: The program introduces a delay of 2 seconds between each call to the website to keep the traffic sustainable and be respectful to the website owners.


Limitations:

The tool is not 100% perfect in automatic mode, but it seems to find about 95% of available mods based on tests.
If dependencies change between Minecraft versions, the system will not be able to deal with that. You will have to manually add them.
You may need to remove downloaded dependencies if they become useless between versions.
Overall, ModpackUpdateManager is a useful tool that can save you a lot of time and effort when updating your Minecraft modpack. Give it a try and see how it works for you!

This text has been reformatted to be more concise by ChatGPT!


Here is a quick video demonstration:

https://youtu.be/-xtx3QRALtg
# AdmiraltySimulator
Tool to help choose the best ship combination for the admiralty mini-game in Star Trek Online

v1.2:
 - easier loading and creating profiles/characters
 - search and auto-complete for assignments and events

v1.1:
 - parsing ships should work regardless of language/region
 - added 32bit version

Features:
 - Analyse all ship combinations and their result for the assignment
 - Choose to go with which ever you think is best
 - Keep track of used ships, their maintenance, and (1x) use cards if any
 - Use with multiple profiles/characters

How to use:
1. Download the latest release
2. Extract files and run "AdmiraltySimulatorGUI.exe", you may need to install the appropriate Microsoft .NET Framework
3. Enter the name of your character at the "new profile name" input, and click "New Profile" to create it as a profile
4. Check your character's ship roster against the ship table on the left side of the application
5. If there are any ship you have that's not in the provided ship database file, please add them, there should be enough examples for you to know what format to use, please commit them in github if you can
6. If you own a ship, tick "Is Owned" in the table, if you have those (1x) cards, put how many of them you have in "One Time Uses"
7. Click on "Save Changes" to save for this character
8. Repeat from step 3 if you want to create more profiles for alts
9. Type the name for the assignment and event to start search, click on correct match to auto-complete details
10. Click on "Simulate", sort/filter the table of all results on the right how ever you want, hold "Shift" and click on column header to sort consecutively, e.g. you can sort results by reward factor, then by total maintenance
11. Choose a desired result and see its outcome populated
12. If you want to start the assignment with those ships, do so in the game, and click "Execute" in the application
13. Repeat from step 9 for the next assignment and so on
14. Once you're done, remember to click "Save Changes" before closing the application or loading a different character


Notes:
 - Reward multiplier is how much reward is given when a critical occurs, it looks like for dilithium and EC to be 1.5x, this is used to calculate the "Reward Factor" column in the result. e.g. result has 90% success rate, and 40% critical rate, the game rewards 0 unit when fail, 1 unit when success but not critical, and 1.5 units when critical, this average to be (0.1 x 0) + (0.9 x 0.6 x 1) + (0.9 x 0.4 x 1.5) = 1.08
 - For assignments where the "main" reward comes from the event bonus, it is useful to set reward multiplier to 1, to avoid wasting more powerful ships
 - I've customised the Assignments.csv data file to denote which assignment I think is worth maximising critical chance for, these assignments will also use (1x) ships by default, you may edit this data file and adjust to your liking
 - Right click on the column header to see a "Filter Editor" that provides more powerful filtering
 - In some situations where you don't want to use your one time use ship cards, set filter on the "Ships" column in the results table to be does not contain "(1x)"
 - At the very end of the results table, there's a "Custom" column, right click on the header to see a "Expression Editor", you can use to build your own metric for sorting the results
 - My character's (fdashx) owned ship and one time use ship files are included for reference

# AdmiraltySimulator
Tool to help choose the best ship combination for the admiralty mini-game in Star Trek Online

Features:
 - Analyse all ship combinations and their result for the assignment
 - Choose to go with which ever you think is best
 - Keep track of used ships, their maintenance, and (1x) use cards if any
 - Use with multiple profiles/characters

How to use (first time):
1. Download the entire "Release" folder
2. Run "AdmiraltySimulatorGUI.exe"
3. Click on "Load Ship Database..." and select the "Ships.txt" file that's in the same folder
4. Check your character's ship roster against the ship table on the left side of the application
5. If there are any ship you have that's not in the provided ship database file, please add them, there should be enough examples for you to know what format to use, please commit them in github if you can
4. Make empty text files "yourcharactername_owned.txt" and "yourcharactername_onetime.txt"
5. Click on "Load Owned Ships..." and "Load One Time Ships..." and select the files you just made respectively
5. If you own a ship, tick "Is Owned" in the table, if you have those (1x) cards, put how many of them you have in "One Time Uses"
6. Click on "Save Changes" to save for this character

How to use:
1. Start up and load all 3 required ship files
2. Fill in the detail of the assignment in the middle part of the application, for maint off from event, type in how much maintenance is reduced by the event, e.g. 50 for -50% off, for assignment duration the format is _h_m, e.g. 2h30m, 45m, 3h, etc
3. Click on "Simulate", sort/filter the table of possible results on the right how ever you want, hold "Shift" and click on column header to sort consecutively
4. Choose a desired result and see its detail populated
5. If you want to start assignment with those combinations, do so in the game, and click "Execute" in the application
6. Fill in new assignment detail and so on
7. Once you're done, remember to click "Save Changes" before closing the application or loading a different character


Notes:
 - Reward multiplier is how much reward is given when a critical occurs, it looks like for dilithium and EC to be 1.5x, this is used to calculate the "Reward Factor" column in the result. e.g. result has 90% success rate, and 40% critical rate, the game rewards 0 unit when fail, 1 unit when success but not critical, and 1.5 units when critical, this average to be (0.1 x 0) + (0.9 x 0.6 x 1) + (0.9 x 0.4 x 1.5) = 1.08
 - For assignments where the "main" reward comes from the event bonus, it is useful to set reward multiplier to 1, to avoid wasting more powerful ships
 - Right click on the column header to see a "Filter Editor" that provides more flexible filtering
 - In some situations where you don't want to use your one time use ship cards, set filter on the "Ships" column in the results table to be does not contain "(1x)"
 - At the very end of the results table, there's a "Custom" column, right click on the header to see a "Expression Editor", you can use to build your own metric for sorting the results
 - My character's (fdashx) owned ship and one time use ship files are included for reference

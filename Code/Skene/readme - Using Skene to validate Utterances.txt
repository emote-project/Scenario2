1- Install AccessDatabaseEngine.exe
https://www.microsoft.com/en-us/download/confirmation.aspx?id=23734

2- Run Skene.exe
if you have any trouble with any of these steps, try running the programs as Administrator
(right click on it and select "Run as administrator".)


3- Navigate to the Utterances tab
You will see the loaded utterance libraries on the top left, and the loaded validation sets on the top right.

4- Select a library.
You can navigate the Categories, Subcategories and Utterances that are contained within this library.
The bottom part of the window allows to test the selected utterance on the robot, but we will cover that later, as that requires running the rest of the system (Thalamus, NAOThalamus and NAO's python script)

5 - Select the appropriate validation set.
(e.g. for Utterances Enercities EN/PT use NAOEnercities)

6- The report will open up in your default web browser (e.g. Chrome).
The report is also saved to Skene's folder in case you want or need to share it.

7- Open up your excel file and check the report to make the corrections.
If you are using Google Spreadsheets, just make the corrections there and download/save the file as Excel (xlsx).
Download it to Skene's "Utterances" folder, replacing the one that is already there.

8- Back in Skene's Utterances tab, click Reload Libraries.

9- Repeat from step 4 until no errors are found.
If at any point realoading stuff doesn't seem to work, try closing Skene and opening again. It should work most of the time, but you never know :)



10- If you have errors regarding invalid Targets, Tags or GameActions that you know are correct, then probably the Validation Set does not know those are correct.
In that case, go to Skene/Utterances/ValidationSets

You will see the validation sets that are made available to Skene.
Each validation set is actually a collection of Validators for different aspects of the utterances (you can open one of these in Notepad/Notepad++ or similar if you want to take a look inside).
The idea is that some aspects of the validation can be shared by different Validation Sets (e.g. if validating Utterances for NAO for different scenarios, the Validator for Animations or TTS instructions will be the same in both scenarios, while for Targets, Tags and Game instructions will be different).

If you need to add any targets, tags or game instructions, go into the Validators folder and open the corresponding validator file in Notepad/Notepad++ or similar.
(e.g. if you need to add valid targets to the Treasure Hunt Map Application, open the THMapApplicationTargets.svf - TH stands for TreasureHunt).
Follow the formatting you already see in the file in order to add your new targets correctly.
WARNING: Look at the "CaseSensitive": field inside the file to check if the targets/tags/game instructions are supposed to be case-sensitive or nor.

Save the file, and use the Reload button next to the Validation Sets list to reload the new rules.
Repeat the validation to see of your errors are gone.

If the Reload button makes your Validation Set disappear, then you made some mistake while adding the new rule to the .svf file. Maybe you missed a comma or something. Go back and check.

If you reload the Validation sets and re-Verify but still get the same errors, first try restarting Skene, then double-check your Validators to make sure you wrote them correctly, and then double-check the report to see if you aren't misunderstanding the error.

# **Skill Tree Mod**

**(Il2cpp) Main Version**


## **Leveling**

To open the Skill Tree Window, just press **"C"**

When a player levels up (increases Tier or Rank), they earn Skill Points:

* 1 Point for every Tier increase.
* 2 Points for every Rank increase.

* 1 Special Point for every Rank increase.

*Example: If you are Tier V and advance to Tier I, you will receive 2 points.*

Skill Points are divided into four categories: **Stats, Operations, Social, Special.**

* **Stats**: Modify the player's base attributes, such as Max Health, XP gain, and Movement Speed.
* **Operations**: Relates to plant cultivation and Chemistry Stations.
* **Social**: Affects NPC interactions and player status.
* **Special**: These skills cover unique player abilities and employee automation

---

## **Default Keybinds**

* Open Skill Tree Menu: C
* Activate Skill One "Streetsweeper: F1
* Activate Skill Two "Fit as a Fiddle": F2
* Activate Skill Three "Siphon Funds": F3

---

## **Skill Trees (Spoilers)**

### **Category: Stats**

* **Hardy**: Increase max health by 20
  * **Fleet Feet**: Increase movespeed by 10%
  * **Prison Wallet**: Double item stack size
  * **Speed Dial**: Reduces delivery time. Minimum: 60 minutes -> 30 minutes | Maximum: 6 hours -> 2 hours
  * **Crystal Ball**: See the chance of a customer accepting a counteroffer
  * **Master Sleeper**: Allow sleeping while Athletic or Energizing effects are active
    * **Napping on the Job**: Can use a bed to skip to the next time period. Plants only grow at 33% of their normal speed when time is skipped
  * **Fast Learner**: Increase XP gain by 5%
    * **Turbo Nerdo**: Increase XP gain by an additional 5%
    * **Kingpin**: Gain 5% of a drug sale's value as bonus XP

### **Category: Operations**

* **Pitchin' a Tent**: Increase quality of plants in grow tents by 16%
  * **Advanced Pot Techniques**: Increase plant and mushroom quality by 15%. Bonus for plants in grow tents, plastic pots, and moisture pots capped at 15%. Mushrooms only affected at rank 2.
    * **Harder and Stronger**: Meth and cocaine quality increased by 1
  * **Absorbent Soil**: Soil additives last until the soil is depleted
  * **Green Thumb**: Increase plant and mushroom growth speed 2.5%
    * **Plant Whisperer**: Increase plant and mushroom growth speed by an additional 2.5%
    * **Quick Crafter**: Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations
  * **Bountiful Harvest**: Increase base yield of plants by 1
    * **Crankin' One Out**: Double the slot capacity for mixing stations and drying racks
    * **Witch's Brew**: Double the cauldron's output

### **Category: Social**

* **Silver Tongued Devil**: Increase chance a potential customer will accept a free sample by 5%
  * **Spread the Wealth**: Increase citizens' weekly spending limits by 10%
  * **Hoard the Wealth**: Increase ATM deposit limit by $2000
  * **Squeaky Clean**: Increase money laundering capacity by 20%
  * **Well-Connected**: Increase dead drop order limit by 67.5% and item limit by 50%
  * **Well-Oiled Machine**: Increase dealer's customer limit by 2
    * **Cheapskate**: Decrease dealer's cut by 5%
    * **Hustler**: Double the movespeed of dealers

### **Special Skills**

* **Streetsweeper**: Once per day: destroy all trash on the map
  * **Fit as a Fiddle**: Once per day: heal to max health
  * **Siphon Funds**: Once per day: instantly collect your cash from all dealers
  * **Fast Farmers**: Botanists perform all actions twice as fast
    * **Sweatshop**: Employees don't stop at 4 AM
    * **RUN BITCH RUN!**: Employees move 3 times faster
    * **Over Worked and Underpaid**: Increase station assignment limit for botanists and chemists by 2

---

## **Base Feature Changes**

### **Plant Quality & Pots**

Plant quality is now determined by the quality of the pot used.

* **Base Grow Tent**: Trash Quality -> (The first skill in "Operations" increases this to Low).
* **Plastic Pot**: Low Quality.
* **Moisture-Preserving Pot**: Low Quality -> (Skill "More Quality" increases this to Moderate).
* **Air Pot**: Moderate Quality.

### **Hardware Store Changes**

Items added to hardware stores with a **250% base price increase** (Convenience Tax).

* Drying Rack (250 -> 400)
* Moisture Preserving Pot, LED Grow Light, Plastic Pot, Halogen Grow Light, Suspension Rack, Air Pot.

### **Rank Requirements**

Certain items now require higher ranks. Example: *Drying Rack requires Street Rat V.*

### **Business Payments**

Payments are now sent every **4 hours**. (e.g., Laundering 2,000 pays out 334 six times a day).

---

## **Installation**

1. **Verify Branch**: Steam -> Right-click "Schedule I" -> Properties -> Betas -> Select **"Alternate"**. Wait for the update.
2. **MelonLoader**: Install from [https://melonwiki.xyz/](https://melonwiki.xyz/).
3. **Plugin**: Drop *SkillTree.dll* into the *Schedule I/Mods* folder.

---

## **Inspirations**

* **S1API (Fork)**: [https://github.com/ifBars/S1API](https://github.com/ifBars/S1API)
* **QualityPlantsMod**: [https://github.com/Soul-Da-Sythe/QualityPlantsMod](https://github.com/Soul-Da-Sythe/QualityPlantsMod)
* **ScheduleOneNewGamePlus**: [https://github.com/regularberry/ScheduleOneNewGamePlus](https://github.com/regularberry/ScheduleOneNewGamePlus)
* **ProduceMore**: [https://github.com/lasersquid/Sched1ProduceMoreMod/tree/master](https://github.com/lasersquid/Sched1ProduceMoreMod/tree/master)
* **Absorbent Soil**: [https://www.nexusmods.com/schedule1/mods/843](https://www.nexusmods.com/schedule1/mods/843)
* **Wolf's Business Improvements**: [https://www.nexusmods.com/schedule1/mods/526](https://www.nexusmods.com/schedule1/mods/526)
* **BetterCounterOffer**: [https://github.com/OvrwghtUnicorn/BetterCounterOffer/tree/main](https://github.com/OvrwghtUnicorn/BetterCounterOffer/tree/main)

---

## **Notes**


**Multiplayer**: I haven't been able to test this thoroughly in multiplayer yet. There may be bugs, and I will do my best to fix them as they are reported.

**Feedback**: If you encounter bugs, have balance suggestions, or new ideas, please notify me on the mod page.

**Open Source**: SkillTree (Forked) is open source and distributed with the same licenses as the original: non-commercial use only, accepting donations is fine, and you must credit the original authors. Feel free to fork, modify, and learn from it. The original mod used AI code for the Harmony patches, and I've gutted most of it.

---

## **Known Issues**

* None at the moment

---

## **F.A.Q.**

Q: **My points disappeared or my skills reset. Why?**

A: The mod reads a JSON file in UserData which syncs with your save. If you level up and spend points but close the game without saving, the JSON and your save file will be out of sync. In this case, the mod resets your skills and returns the points to you for relocation.

Q: **Will skills get unique images?**

A: I don't think so. I initially wanted to add them, but I prioritized the mod's functionality instead.

Q: **If I have an idea for the progression system, will you add it?**

A: I'm open to it, but it depends on the complexity. I have already set aside ideas like "More Employees" or "More Routes" because the game's internal systems for those are very complex. I might consider them for a separate "Upgradable System" mod later.
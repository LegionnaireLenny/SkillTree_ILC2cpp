# SkillTree (Forked)

**(Il2cpp) Main Version**

This a significant rewrite of reizor153's mod SkillTree to fix several issues, and eventually implement some of my own ideas. This mod introduces a skill system to provide a deeper feeling of progression and implement fundamental mechanical changes.

Original Author: reizor153 aka CrazyReizor

Original Mod Page: SkillTree

Original IL2CPP Source Code: https://github.com/AugustoCesarAC/SkillTree_ILC2cpp

Forked Source Code: https://github.com/LegionnaireLenny/SkillTree_ILC2cpp

## Currently only the default version (IL2CPP) of the game is supported.
## Mono is not supported and I do not have plans to implement support at the moment.

## What's New in This Version

Changes and Additions

- Significant parts of the code have been changed to be more compatible with future changes to the base game
- The mod now supports reloading a save without restarting the game
- Active skill keybinds are now user configurable
- Switched user configuration to use MelonPreferences, meaning Mod Manager - Phone App﻿ is supported
- Removed some restrictions on skills that were not mentioned in the tooltip
- Gave all skills unique names related to their function and updated the tooltips for increased clarity/accuracy
- Rearranged the skill tree a bit for a better sense of progression

Bug Fixes

- Drying rack capacity now updates correctly (updates when rack is placed and when opening the rack, just in case)
- The player no longer loses their move speed skill bonus when Energizing and Athletic effects wear off 
- The skill menu now blocks player input while open. No more punching people by accident while picking skills
- The skill menu can't be opened and active skills can't be used while other menus are open
- Fixed crafting speed patches
- Fixed (hopefully) cauldron patches
- Fixed meth/cocaine quality patch
- Employees no longer incorrectly state their shift is over when the player has the Employees24h skill
- Probably some things I forgot. I made a lot of changes.
- (Fixed? May have been fixed in the original) If you start the mod during the Tutorial, the Skill Tree UI will bug out.

## Plans

These are only ideas I'm considering, no guarantees they'll be added

### New Skills

Stats
- Taser resistance
- More melee damage
- Increased pickpocketing ability
- Reduced visibility

Operations
- Move mushrooms from Advanced Pot Techniques and put them into a their own skill. 

Social
- Increase customers' minimum and maximum orders per week
- Increased pawn prices
- Increased trash prices

Special
- Add police to the map
- Add Benzies to the map
- Bullet-time
- Rename 'Streetsweeper' to 'Good Samaritan' and have it add 25% value of all trash removed to online balance

### Other Changes

- Make skill tree menu into a phone app 
- Tweak/remove shop injection patches
- Tweak gradual laundering patch for compatibility
- Tweak ATM patches for compatibility

## Compatibility

- No More ATM Limits﻿ (and other mods that disable ATM deposit limits) - Incompatible
- Mods that instantly launder money - Incompatible
- If you run into a compatibility issue, provide the name of the mod and a link to it, and I'll look into it

## Known Issues

- None at the moment

## Default Keybinds

- Open Skill Tree Menu: C
- Activate Skill One "Streetsweeper: F1
- Activate Skill Two "Fit as a Fiddle": F2
- Activate Skill Three "Siphon Funds": F3

## Leveling

When a player levels up (increases Tier or Rank), they earn Skill Points:
- 1 Point for every Tier increase.
- 2 Points for every Rank increase.
- 1 Special Point for every Rank increase.

*Example*: If you are Tier V and advance to Tier I, you will receive 2 points.

Skill Points are divided into four categories: Stats, Operations, Social, and Special.

Stats: Modify the player's base attributes, such as Max Health, XP gain, and Movement Speed.
Operations: Relates to plant cultivation and Chemistry Stations.
Social: Affects NPC interactions and player status.
Special: Grants active skills and bonuses to employees

## Skill Trees (Spoilers)

Category: Stats
- Hardy: Increase max health by 20
    - Battle-scarred: Increase health regen by 100% and decrease health regen delay by 50%
      - Slippery: Reduces police arrest radius by 25% and increases time until arrested by 100%
  - Fleet Feet: Increase movespeed by 10%
    - Spring-Heeled: Increase max stamina by 30% and jump height by 35%
  - Prison Wallet: Double item stack size
  - Crystal Ball: See the chance of a customer accepting a counteroffer
  - Master Sleeper: Allow sleeping while Athletic or Energizing effects are active
    - Napping on the Job: Can use a bed to skip to the next time period. Plants only grow at 33% of their normal speed when time is skipped
  - Fast Learner: Increase XP gain by 5%
    - Turbo Nerdo: Increase XP gain by an additional 5%
    - Kingpin: Gain 5% of a drug sale's value as bonus XP

Category: Operations
- Pitchin' a Tent: Increase quality of plants in grow tents by 16%
    - Advanced Pot Techniques: Increase plant and mushroom quality by 15%. Bonus for plants in grow tents, plastic pots, and moisture pots capped at 15%. Mushrooms only affected at rank 2.
       - Harder and Stronger: Meth and cocaine quality increased by 1
  - Absorbent Soil: Soil additives last until the soil is depleted
  - Green Thumb: Increase plant and mushroom growth speed 2.5%
    - Plant Whisperer: Increase plant and mushroom growth speed by an additional 2.5%
    - Quick Crafter: Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations
  - Bountiful Harvest: Increase base yield of plants by 1
    - Crankin' One Out: Double the production capacity of mixing stations and drying racks
    - Witch's Brew: Double the cauldron's output

Category: Social
- Silver Tongued Devil: Increase chance a potential customer will accept a free sample by 5%
  - Spread the Wealth: Increase citizens' weekly spending limits by 10%
  - Hoard the Wealth: Increase ATM deposit limit by $2000
    - Squeaky Clean: Increase money laundering capacity by 20%
  - Reliable Business Partner: Increase dead drop order limit by 67.5% and item limit by 50%
    - Speed Dial: Reduces delivery time. Minimum: 60 minutes -> 30 minutes | Maximum: 6 hours -> 2 hours
  - Expansive Empire: Increase dealer's customer limit by 2
    - Wage Garnishment: Decrease dealer's cut by 5%
    - Motivational Leader: Double the movespeed of dealers

Category: Special
- Streetsweeper: Once per day: destroy all trash on the map
  - Fit as a Fiddle: Once per day: heal to max health
  - Siphon Funds: Once per day: instantly collect your cash from all dealers
  - Fast Farmers: Botanists perform all actions twice as fast
  - Sweatshop: Employees don't stop at 4 AM
    - RUN BITCH RUN!: Employees move 3 times faster
    - Over Worked and Underpaid: Increase station assignment limit for botanists and chemists by 2

## Base Feature Changes

Plant Quality & Pots:
- The Operations tree gives air pots +5% quality so max level Advanced Pot Techniques gives Premium quality product when all additives are used

Hardware Store Additions (with increased price):
- Drying Rack ($400)
- Suspension Rack ($100)
- Plastic Pot ($50)
- Moisture Preserving Pot ($125)
- Air Pot ($300)
- Halogen Grow Light ($100)
- LED Grow Light ($200)

New Rank Requirements
- Suspension Rack: Street Rat V
- Plastic Pot: Street Rat V
- Moisture Preserving Pot: Hoodlum V
- Air Pot: Peddler II
- Halogen Grow Light: Street Rat V
- LED Grow Light: Hoodlum V
- Drying Rack: Street Rat 5
- Cauldron: Bagman 3
- Brick Press: 5

Laundering Changes:
- Payments are now sent every 4 hours. (e.g., Laundering 2,000 pays out 334 six times a day).

## Installation

- Verify Branch and Download the especific file.
- MelonLoader: Install from MelonDownload﻿.
- Plugin: Drop SkillTree.dll into the Schedule I/Mods folder.

## Notes

Multiplayer: I haven't tested multiplayer at all. There will be bugs and probably very few fixes for them.

Feedback: If you encounter bugs, have balance suggestions, or new ideas, please notify me on the mod page.

Open Source: SkillTree (Forked) is open source and distributed with the same licenses as the original: non-commercial use only, accepting donations is fine, and you must credit the original authors. Feel free to fork, modify, and learn from it. The original mod used AI code for the Harmony patches, and I've gutted most of it.

## Changelog

- Version 2.1.1
  - Fixed plants not having harvestables
  - Gave air pots +5% bonus quality
- Version 2.1.0
  - Added Empire 2.0 compatibility patch
  - Improved reliability by loading skill data earlier
  - Added SkillTree version checking for resetting skill tree after major changes
  - Added new skill: Battle-scarred - Increase health regen by 100% and decrease health regen delay by 50% - Parent: Hardy
  - Added new skill: Slippery - Reduces police arrest radius by 25% and increases time until arrested by 100% - Parent: Battle-scarred
  - Added new skill: Spring-Heeled - Increase max stamina by 30% and jump height by 35% - Parent: Fleet Feet
  - Moved skill Speed Dial from Stats to Social
  - Reduced max rank of Fleet Feet to 2, and increased the bonus per level to 15%
  - Renamed Cheapskate to Wage Garnishment
  - Renamed Well-Connected to Reliable Business Partner
  - Renamed Hustler to Motivational Leader
  - Renamed Well-Oiled Machine to Expansive Empire
  - Fixed Crankin' One Out unintentionally increasing output
- Version 2.0.2
  - Fixed skills stacking when reloading the game
  - Fixed health not recovering past the base game maximum
- Version 2.0.1
  - Fixed Absorbent Soil not re-applying Speed Grow additive
  - Grow Tents no longer benefit from MoreQuality skill (Advanded Pot Techniques)
- Version 2.0.0
 - Initial release

## Inspirations

- S1API (Fork): Github S1API Fork﻿
- QualityPlantsMod: Github QualityPlants﻿
- ScheduleOneNewGamePlus: Github New Game Plus﻿
- ProduceMore: Github Product More﻿
- Absorbent Soil: Github Absorbent Soil﻿
- Wolf's Business Improvements: NexusPage Wolf's Business﻿﻿
- BetterCounterOffer: Github BetterCounterOffer﻿

## Additional Credits
Icons - https://iconify.design/
- Benzies Dealer Icon - mdi:weed.png - License: MIT
- Benzies Goon Icon - mdi:dagger.png - License: Apache 2
- Police Officer Icon - mdi:police-badge-outline.png - License: Apache 2
- Heal Icon - material-symbols:heart-plus - License: Apache 2
- Trashcan Icon - ix:trashcan - License: MIT
- Cash Icon - bi:cash-coin - License: MIT
- App Icon Tree - si:flow-tree-duotone - License: MIT
- App Icon Fork - hugeicons:fork - License: MIT

## F.A.Q.

**My points disappeared or my skills reset. Why?**

The mod reads a JSON file in UserData which syncs with your save. If you level up and spend points but close the game without saving, the JSON and your save file will be out of sync. In this case, the mod resets your skills and returns the points to you for relocation.
   
Version 2.1.0 introduced a version checking system for validating compatibility between updates. Changes to the major or minor version (e.g. 2.1.0.0 = major version 2, minor version 1, patch version 0) indicate that changes were made that require resetting the skill tree to avoid issues. Such changes include changing a skill's parent node or a skill's max level.

**Will skills get unique images?**

Eventually, once I get around to making the skill tree into an app. They'll probably be some terrible MS Paint doodles until I can be bothered finding some good free-use images.

**If I have an idea for the progression system, will you add it?**

I'm open to ideas, make a suggestion in the comments and I'll consider it. I'm trying to avoid anything blatantly overpowered, even though the current balance of skills might be a bit questionable.
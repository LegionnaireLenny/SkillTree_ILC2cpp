namespace SkillTree.Core.FileManagement
{
    [Serializable]
    public class SkillTreeData
    {
        public int StatsPoints = 0;
        public int OperationsPoints = 0;
        public int SocialPoints = 0;
        public int SpecialPoints = 0;
        public int UsedSkillPoints = 0;

        /* STATS START HERE */

        [Skill("Hardy", "Increase max health by 20", SkillCategory.Stats, null, 1)]
        public int Stats = 0;

        [Skill("Fleet Feet", "Increase movespeed by 10%", SkillCategory.Stats, "Stats", 3)]
        public int MoreMovespeed = 0;

        [Skill("Prison Wallet", "Double item stack size", SkillCategory.Stats, "Stats", 1)]
        public int MoreStackItem = 0;

        [Skill("Speed Dial", "Reduces delivery time. Minimum: 60 minutes -> 30 minutes | Maximum: 6 hours -> 2 hours", SkillCategory.Stats, "Stats", 1)]
        public int BetterDelivery = 0;

        [Skill("Crystal Ball", "See the chance of a customer accepting a counteroffer", SkillCategory.Stats, "Stats", 1)]
        public int AllowSeeCounteroffChance = 0;

        [Skill("Master Sleeper", "Allow sleeping while Athletic or Energizing effects are active", SkillCategory.Stats, "Stats", 1)]
        public int AllowSleepAthEne = 0;

        [Skill("Napping on the Job", "Can use a bed to skip to the next time period. Plants only grow at 33% of their normal speed when time is skipped.", SkillCategory.Stats, "AllowSleepAthEne", 1)]
        public int SkipSchedule = 0;

        [Skill("Fast Learner", "Increase XP gain by 5%", SkillCategory.Stats, "Stats", 2)]
        public int MoreXP = 0;

        [Skill("Turbo Nerdo", "Increase XP gain by an additional 5%", SkillCategory.Stats, "MoreXP", 4)]
        public int MoreXP2 = 0;

        [Skill("Kingpin", "Gain 5% of a drug sale's value as bonus XP", SkillCategory.Stats, "MoreXP", 1)]
        public int MoreXPWhenEarnMoney = 0;

        /* STATS END HERE */

        /* OPERATIONS START HERE */

        [Skill("Pitchin' a Tent", "Increase quality of plants in grow tents by 16%", SkillCategory.Operations, null, 1)]
        public int Operations = 0;

        [Skill("Advanced Pot Techniques", "Increase plant and mushroom quality by 15%. Bonus for plants in grow tents, plastic pots, and moisture pots capped at 15%. Mushrooms only affected at rank 2.", SkillCategory.Operations, "Operations", 2)]
        public int MoreQuality = 0;

        [Skill("Harder and Stronger", "Meth and cocaine quality increased by 1", SkillCategory.Operations, "MoreQuality", 1)]
        public int MoreQualityMethCoca = 0;

        //[Skill("More Quality Mushroom", "Upgrade Mushroom Quality Tier", SkillCategory.Operations, "MoreQuality", 1)]
        //public int MoreQualityMushroom = 0;

        [Skill("Absorbent Soil", "Soil additives last until the soil is depleted", SkillCategory.Operations, "Operations", 1)]
        public int AbsorbentSoil = 0;

        [Skill("Green Thumb", "Increase plant and mushroom growth speed 2.5%", SkillCategory.Operations, "Operations", 2)]
        public int GrowthSpeed = 0;

        [Skill("Plant Whisperer", "Increase plant and mushroom growth speed by an additional 2.5%", SkillCategory.Operations, "GrowthSpeed", 2)]
        public int GrowthSpeed2 = 0;

        [Skill("Quick Crafter", "Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations", SkillCategory.Operations, "GrowthSpeed", 1)]
        public int ChemistStationQuick = 0;

        [Skill("Bountiful Harvest", "Increase base yield of plants by 1", SkillCategory.Operations, "Operations", 1)]
        public int MoreYield = 0;

        [Skill("Crankin' One Out", "Double the slot capacity for mixing stations and drying racks", SkillCategory.Operations, "MoreYield", 1)]
        public int MoreMixAndDryingRackOutput = 0;

        [Skill("Witch's Brew", "Double the cauldron's output", SkillCategory.Operations, "MoreYield", 1)]
        public int MoreCauldronOutput = 0;

        /* OPERATIONS END HERE */

        /* SOCIAL START HERE */

        [Skill("Silver Tongued Devil", "Increase chance a potential customer will accept a free sample by 5%", SkillCategory.Social, null, 2)]
        public int Social = 0;

        [Skill("Spread the Wealth", "Increase citizens' weekly spending limits by 10%", SkillCategory.Social, "Social", 2)]
        public int CityEvolving = 0;

        [Skill("Hoard the Wealth", "Increase ATM deposit limit by $2000", SkillCategory.Social, "Social", 2)]
        public int MoreATMLimit = 0;

        [Skill("Squeaky Clean", "Increase money laundering capacity by 20%", SkillCategory.Social, "Social", 3)]
        public int BusinessEvolving = 0;

        [Skill("Well-Oiled Machine", "Increase dealer's customer limit by 2", SkillCategory.Social, "Social", 1)]
        public int DealerMoreCustomer = 0;

        [Skill("Cheapskate", "Decrease dealer's cut by 5%", SkillCategory.Social, "DealerMoreCustomer", 2)]
        public int DealerCutLess = 0;

        [Skill("Well-Connected", "Increase dead drop order and item limits by 50%", SkillCategory.Social, "Social", 2)]
        public int BetterSupplier = 0;

        [Skill("Hustler", "Double the movespeed of dealers", SkillCategory.Social, "DealerMoreCustomer", 1)]
        public int DealerSpeedUp = 0;

        /* SOCIAL ENDS HERE */

        /* SPECIAL STARTS HERE */

        [Skill("Streetsweeper", "Once per day, destroy all trash on the map", SkillCategory.Special, null, 1)]
        public int Special = 0;

        [Skill("Fit as a Fiddle", "Once per day, heal to max health", SkillCategory.Special, "Special", 1)]
        public int Heal = 0;

        [Skill("Siphon Funds", "Once per day, instantly collect your cash from all dealers", SkillCategory.Special, "Special", 1)]
        public int GetCashDealer = 0;

        [Skill("Fast Farmers", "Botanists perform all actions twice as fast", SkillCategory.Special, "Special", 1)]
        public int BetterBotanists = 0;

        [Skill("Sweatshop", "Employees don't stop at 4 AM", SkillCategory.Special, "Special", 1)]
        public int Employees24h = 0;

        [Skill("RUN BITCH RUN!", "Employees move 3 times faster", SkillCategory.Special, "BetterBotanists", 1)]
        public int EmployeeMovespeed = 0;

        [Skill("Over Worked and Underpaid", "Increase station assignment limit for botanists and chemists by 2", SkillCategory.Special, "BetterBotanists", 2)]
        public int EmployeeMaxStation = 0;

        /* SPECIAL ENDS HERE */

    }
}

Issues
opening skill tree doesn't preserve previous cursor lock state

Todo
make skill tree an app
add support for mod app
add support for user configuration
SkillTreeUI
Core
SkillBasePatch
SkillSystem
tweak/remove shop injection patches
tweak gradual laundering patch for compatibility
tweak atm patches for compatibility

Ideas
Stats - Taser resistance
Stats - more melee damage
Stats - Increased pickpocketing chance
Social - increase customer min and max orders per week
Social - increased pawn prices
Social - Increased trash prices
Operations - Add police to the map
Operations - Add benzies to the map

notes - phone app
extend contacts app?

contacts app
	container
		scroll view - draggable background with contacts
			recttransform - canvas renderer - image - pinchable scrollrect
			contacts are relationcircles
		regionselection - top row of buttons with regions listed
			it's a recttransform with a row of buttons and a line element to show selected region
			regions - horizontallayoutgroup with buttons
		detail - side panel listing contact details
# RiskyCohesion
Extends the RiskyMod suite's rules to items and elites from other mods, wherever those mods can't do it from their own configs. This is...mostly about proc chains and elite scaling for now, but may expand or shrink over time.

Works with RiskyMod, RiskyTweaks, RiskyFixes and EliteReworks, and applies their rules to Aetherium, Sandswept, Starstorm 2, Spikestrip 2.0, ArtificerExtended, Ancient Scepter, StormboundFalseSon, Chef Overcooked and Eggs Skills. None of them are required. Each change follows the mod and setting it extends and does nothing if that mod isn't installed or the setting is off, so it's fine to run this with only RiskyTweaks, or without EliteReworks, etc.

Every change has its own config option and can also be configured in-game with Risk of Options. In-game options only show up for the mods and settings you actually have.

The mod is mostly Server-side. Only the host _needs_ to have it. The LookingGlass stats, the tooltip fixes, the drone display fix for your own skills, ArtificerExtended's Ion Surge speed and graze shock, Ancient Scepter's cooldown, sprint and description changes, Chef Overcooked's Cook health bar marker, and the Bandit autofire and Conduit sprint changes only work for players who have it though.

NOTE: I am in no way affiliated with Moffein or the RiskyMod team.

# Changes
- **LookingGlass:**
    - Fixed stats for vanilla items changed by RiskyMod, RiskyTweaks and/or RiskyFixes.
    - Added stats for Aetherium, Starstorm 2, Sandswept, and Spikestrip 2.0 items. Also added proc coefficients to Sandswept's own stats, which will adhere to how each mod is configured.
    - Damaging items show the proc coefficient actually in effect.


- **RiskyMod:**
	- *Disable Proc Chains*:
		- Proc coefficient set to 0:
			- `Aetherium`: Nail Bomb, Inspiring Drone, Blaster Sword
			- `Starstorm 2`: Blast Knuckles, DebuffMissiles, Erratic Gadget's on-hit lightning, Needles, Sticky Overloader
			- `Spikestrip 2.0`: Perma-Spike Factory
		- `Aetherium`: Sanguine elites' bleed, Witches Ring, and Zenith Accelerator buff hits now only trigger from hits that have a proc coefficient above 0.
		- `Spikestrip 2.0`: Blood Orange reworked:\
			*Blood Orange's design was in direct conflict with RiskyMod's proc chain goals. I tried to keep the spirit of the item alive, but if you don't like this rework I suggest you just disable the item altogether in Spikestrip's config.*
			- DoTs no longer trigger On-Hit items.
			- DoTs deal +30% (+15% per stack) damage for each other type of DoT you have on the enemy.
			- 600% DoT on big hits is unchanged.

		- Recommended config changes (**These have to be done by you MANUALLY**. I'm not gonna hack into their mod just so you don't have to open a few configs):
			- `Sandswept`:
				- Amber Knife - Set Proc Coefficient: 1 -> 0
				- Ceremonial Jar - Set Proc Coefficient: 1 -> 0
				- Millenium - Set Explosion Proc Coefficient: 0.33 -> 0
				- Nuclear Salvo - Set Missile Proc Coefficient: 0.33 -> 0
				- Sun Fragment - Set Explosion Proc Coefficient: 0.33 -> 0
			- `Starstorm 2`:
				- Cryptic Source - Set Proc Coefficient: 1 -> 0.5 (_Matches other conditional changes that RiskyMod makes_)
				- Man-o'-War (Lightning On Kill) - Set Proc Co: 1 -> 0
				- Malice - Set Proc Co: 0.1 -> 0 (_or don't. Item's kinda meh without it_)
			- `VanillaVoid`:
				- Executioner's Burden - Set Proc Coefficient: 0.5 -> 0
				- Extraterrestrial Exhaust - Set Proc Coefficient: 0.2 -> 0

	- *Combat Director Credit Multiplier*: `Starstorm 2`'s elite director now ignores this multiplier.
	- *Ion Surge - Shock*: `ArtificerExtended`'s reworked Ion Surge shocks instead of stunning (graze, chain zaps and impact), and shows ArtificerExtended's description instead of RiskyMod's.
	- *Snapfreeze Rework*: The reworked Snapfreeze now triggers `ArtificerExtended`'s Energetic Resonance.
	- *Freeze Executes Bosses*: `Chef Overcooked`'s Cook executes enemies below its *Execute Threshold* again.
	- *Contagion Passive Skill / Blight Returns*: The Cloying Breath nerf now also applies to `Spikestrip 2.0`'s Deeprot.

- **RiskyTweaks:**
	- *Shuriken - No Band Proc*: `Sandswept`'s Glacial Plasma and `Starstorm 2`'s Haunted Lamp can no longer trigger elemental bands. RiskyTweaks' *Only Affect Players* setting applies to them too.
	- *Bandit Primary Autofire*: `Starstorm 2`'s Tranquilizer Gun and `Eggs Skills`' Magic Bullet keep their own fire rates.
	- *Loader Sprint QoL*: `Spikestrip 2.0`'s Conduit no longer cancels sprinting.
	- *Ion Surge - Disable Movement Speed Scaling*: `ArtificerExtended`'s reworked Ion Surge flies at the same speed regardless of movement speed.

- **RiskyFixes:**
	- *Lunar Ruin Exponential Damage Removal*: The Lunar Ruin damage bonus follows `StormboundFalseSon`'s *Lunar Ruin Damage Cap* again.

- **Ancient Scepter:**
	- Scepter upgrades keep the Risky changes to the skill they upgrade:
		- Death Blossom: *Suppressive Fire Changes* cooldown, hold-to-fire and description.
		- Rabauld: *Ballista Changes* cooldown.
		- Megaton Punch and Thundercrash: RiskyTweaks' Loader *Sprint QoL*.
		- Thundercrash: *Thunder Gauntlet Changes* lightning range.
		- Dragon's Breath: *Flamethrower - Sprint Cancel*.
		- Forfeited Suppress: RiskyTweaks' *Corrupted Crush - Buff Skill* instant refill.
		- Lilith Strike: Diablo Strike's *Remove Falloff*, *Proc Coeficient* and *Disable Friendly Fire*.
		- COMMAND: REAP: *DIRECTIVE Harvest Changes* fruit pickup range and description.
	- Renegade's ricochet no longer triggers on Bandit's specials under RiskyMod's *Special Rework*, which have their own scepter upgrades.

- **EliteReworks:**
	- T1 multipliers (3x HP, 1.5x damage):
		- `Aetherium`: Sanguine
		- `Starstorm 2`: Toxic
		- `Spikestrip 2.0`: Warped, Plated, Veiled
	- T2 multipliers (16x HP, 4x damage):
		- `Spikestrip 2.0`: Aragonite
	- Stun/Shock/Freeze disables passive elite effects:
		- `Sandswept`: Motivating, Osmium
		- `Starstorm 2`: Ethereal, Ultra, Toxic, Tectonic, Infernal, Boreal, Supercharged
		- `Spikestrip 2.0`: Warped, Plated, Aragonite
	- Passive damage:
		- Ethereal's circles and Aragonite's strikes deal Malachite's spike damage.
		- Toxic's bombs deal Overloading's passive bomb damage.
		- Ethereal, Tectonic and Infernal damage zones deal one hit's worth of damage per second.
		- Toxic's bombs, plus Ethereal, Tectonic, Infernal and Boreal's zones, now properly have proc coefficient 0 (matching Overloading's passive bombs and Blazing's fire trails).

	- Recommended config changes (**These have to be done by you MANUALLY**. I'm not gonna hack into their mod just so you don't have to open a few configs):
		- `Sandswept`:
			- Motivating
				- Health Multiplier: 4 -> 3
				- Damage Multiplier: 2 -> 1.5
			- Osmium
				- Health Multiplier: 18 -> 16
				- Damage Multiplier: 6 -> 4


- **Balance (optional):**\
	*These are my own personal balance liberties that are tangentially related, but are not technically a cohesion issue between the mods. These are my own liberties, so they are **off** by default and do nothing until you turn them on.*
	- `Starstorm 2`: Ethereal, Ultra, Empyrean and elite event bosses' health and damage bonuses can be scaled down, or up to 10x for the psychos.
	- `Starstorm 2`: Toxic's poison debuff stacks expire individually, and blocked hits no longer splash poison. These elites are jank as hell and these seemed like sensible changes to rein them in.
	- `Starstorm 2`: A Lysate Cell lets Chirr keep two friends at once, like Engineer's extra turret. Befriending past the limit releases the oldest friend.\

    I'd also recommend manually nerfing the everliving-shit out of most Aetherium items in its config but I don't really wanna go into the specifics of that here. If you use Aetherium you already know how busted most of it is.
# Fixes
- **LookingGlass:**
    - Fixed LookingGlass' own stats for Egocentrism, Genesis Loop and H3AD-ST recharge, Ukulele range, Fireworks and Runald's Band proc coefficients, Tentabauble and Stun Grenade chances, Sentient Meat Hook damage and targets, Halcyon Seed and Lepton Daisy team-wide stacks, Infusion and Bison Steak health percentages, and Gasoline and Frost Relic units.
- **RiskyMod + Starstorm 2:**
	- Fixed a conflict between Chirr's befriend mechanisms and Riskymod's *Void Infestor - No Ally Infestation* setting. When playing Chirr with this setting on (which is the default), befriended void infestors would create permanent extra allies that weren't properly tagged as Chirr's friend. They ignored the one-friend limit and would bug out Simulacrum, preventing waves from ending. It also prevented the associated skin challenge from being completable.
- **RiskyFixes:**
    - Fixed RiskyFixes' Lunar Ruin bonus multiplying LinearDamage's item bonuses instead of adding to them (RiskyFixes looks for LinearDamage under the wrong package ID right now).
    - Fixed Warped Echo's delayed damage getting RiskyFixes' Lunar Ruin bonus a second time.
- **Starstorm 2 + EnemyAbilities:**
    - Fixed Artifact of Cognation ghosts ignoring EnemyAbilities' summon limit. Every reinforcement you killed came back as a ghost that could call its own reinforcements, which could loop infinitely (you die from this, if that wasn't obvious).
- **Starstorm 2:**
	- Fixed elite events giving their health bonus to damage instead of health. (Genuinely don't know if that was intentional by them but the variables in their code and the insanity of the numbers imply it's a bug. Disable this if they ever fix it or if you love pain.)
	- Fixed monsters summoned by Chirr's friend (like EnemyAbilities' Beetle reinforcements and Void Reaver eyes) getting copies of all the items Chirr shares with it.
- **Spikestrip 2.0:**
	- Fixed Aragonite's death strikes hitting every team, including the elite's own allies.
	- Fixed Infinity Codex only working for the host in multiplayer.
- **Sandswept:**
	- Fixed Inferno and Voltaic drones on drone vendors having a broken hurtbox. Explosions that reached it dealt no damage, and delayed ones like the Tanker's repeated endlessly.
- **Chef Overcooked:**
	- Fixed CHEF's health bars not showing Cook's execute threshold when R2API.Execute (a RiskyMod dependency) is installed.
- **Simulacrum Additions:**
	- Fixed family waves being picked on stages where part of the family can't spawn (like the Alloy Vulture family on Simulacrum's Aphelian Sanctuary), which left the wave empty until the game's failsafe ended it.
- **Voidcrid:**
	- Fixed Voidcrid registering its damage types on the first hit instead of at startup, which left players who aren't playing Voidcrid with a different set than the host, so the host dropped their hits and logged malformed packets. Every player needs RiskyCohesion for this fix.
- **Vanilla:**
	- Fixed Ukulele and Safer Spaces descriptions being one stack short. The Ukulele fix turns off with RiskyMod's Ukulele changes, and the Safer Spaces fix with RiskyFixes' *Safer Spaces Cooldown*.
	- Fixed stunning, freezing or shocking the Solus Wing's pods throwing an error and repeating the melee hit that caused it.
	- Fixed bodies without a wings model (like monsters in Simulacrum's Flight waves) throwing an error when dying with Milky Chrysalis, which cut the rest of their death code short.
	- Fixed stat tracking throwing an error on deaths of some unregistered (usually modded) bodies, which skipped the rest of the game's update for that frame.
	- Fixed each Gasoline burst set off by another burst's kills running inside that burst, which could overflow the stack and crash the host's game when a chain killed its way through a big pack. Only really applicable for heavily modded setups.

## Disclaimer

Look, this is kind of just my own personal dumping ground for balance issues or bugs I encountered in my own modlist. I don't really want to add support for every single item or elite mod because everybody stores their damn proc coefficients & numbers differently, I don't use any item mods other than these, and it's a massive pain in the ass to cover every single scenario. You get what you get. I also did not want to build in hacks for stuff that each mod supported natively so you still have to do some manual config changes even if you download this (noted above in each section where relevant).

I know that the ProcSolver/Proc Limiter mod is supposed to address a lot of the proc stuff but I'm gonna be real it did not feel like it worked properly for me, even when setting its limits to 0.
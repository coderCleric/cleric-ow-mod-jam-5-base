# Mod Jam 5 Base
The base solar system for the 5th Outer Wilds mod jam. Install jam entries separately to play!

### For entrants:

Entries must take place in a shared solar system. Each entry has two components: A starting platform in the Central Station, and a 2500m radius (5000m diameter) mini solar system. This space requirement includes hidden locations like Dark Bramble dimensions and Dreamworlds.

To make your starting platform, you must make a planet config that looks like this:

```cs
{
  "name": "Your Jam Entry Platform",
  "$schema": "https://raw.githubusercontent.com/Outer-Wilds-New-Horizons/new-horizons/main/NewHorizons/Schemas/body_schema.json",
  "starSystem": "Jam5",
  "extras": {
    "isPlatform": true
  },
  ...
}
```

Make sure to give it a unique name! If you name it "Platform" and somebody else names their platform that your mods will be incompatible and you will LOSE (not really we'll just have to ask you to fix it and that's annoying).

To define your mini solar system, you must make a central planet with `"centerOfSolarSystem": true` set. The base Jam 5 mod will know how to handle this and will position your mini solar system relative to the other entries.

You are strongly encouraged to have at least one ship log entry which is granted to the player when they complete the mod. Also avoid using credits that quit to the main menu after beating your mod: Players and judges will likely be playing multiple mods at once and having to load back into the system after beating your mod will be a waste of time.

Your mod **must** be contained within a 2500m radius sphere centered around your `centerOfSolarSystem` object. To check that you are abiding by this restriction, there is a debug option "Show Allowed Volume" which will render a sphere around your entry showing you the allowed area.

**TL;DR**:
1. Mod starts at a platform in the central station (see config above)
2. Mod takes place within 2500m radius sphere with central body marked `"centerOfSolarSystem": true`
3. Includes dimensions and dreamworlds.

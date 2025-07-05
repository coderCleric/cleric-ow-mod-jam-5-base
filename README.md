# Outer Wilds Mod Jam 5

![Mod banner](https://github.com/user-attachments/assets/fe9f852a-607d-4bd0-ba7f-a31ff2cda64b)

For the fifth mod jam (theme MINIATURE) all mods take place in the same star system. This star system! This time, each entry must fit within a 2500m radius sphere.

**This mod does not include the jam entries on its own, you must install those separately!**

## Compatibility fixes covered during the jam:

- It will arrange each mini-star-system to ensure there is no overlap.
- It will arrange the starting platforms.
- It will rearrange ship log entries to not overlap, however THE SHIP LOGS MUST BE MANUALLY POSITIONED IN YOUR SYSTEM CONFIG! This applies even if you have only one ship log entry: you must manually position it at 0, 0 in that case.

## For entrants:

Entries **must** take place in a shared solar system (named `"Jam5"`). Each entry has two components: A starting platform in the Central Station, and a 2500m radius (5000m diameter) mini solar system. This space requirement includes hidden locations like Dark Bramble dimensions and Dreamworlds. **Each entry is required to have a starting platform and a centerOfSolarSystem body.**

There is an example project repo [here](https://github.com/xen-42/ow-mod-jam-5-example).

### Starting platform

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

Put details on your platform as you would a normal planet. Use the NH debug raycasting tool on your platform to see how things must be positioned. Make sure you are positioning everything locally, as the global position of the platform will move depending on what other entries are installed!

### Mini-star-system

To define your mini star system, you must make a central planet with `"centerOfSolarSystem": true` set. The base Jam 5 mod will know how to handle this and will position your mini solar system relative to the other entries.

Your mod **must** be contained within a 2500m radius sphere centered around your `centerOfSolarSystem` object. To check that you are abiding by this restriction, there is a debug option "Show Allowed Volume" which will render a sphere around your entry showing you the allowed area.

**This requirement includes hidden places such as Bramble dimensions and dream worlds!** You can be as creative as you want to make optimal use of your space, but it must all stay within the sphere!

### Other considerations

You are strongly encouraged to have at least one ship log entry which is granted to the player when they complete the mod. More ship logs is of course better (within reason)!

Avoid using credits that quit to the main menu after beating your mod: Players and judges will likely be playing multiple mods at once and having to load back into the system after beating your mod will be a waste of time.

## TL;DR:
1. Mod starts at a platform in the central station (see config above)
2. Mod takes place within 2500m radius sphere with central body marked `"centerOfSolarSystem": true` (includes hidden dimensions).
3. Please have ship logs.

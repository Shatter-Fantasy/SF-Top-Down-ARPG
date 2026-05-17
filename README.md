# SF-Top-Down-ARPG-Package
There is a work in progress dedicated documentation website being made.

# Important Notes
This package requires Unity 6.5 or newer. The goal is for a stable release ready for Unity 6.7 LTS when it comes out.
The version requirements is because I don't want to use any of the legacy API being removed and want to not create technical debt for a package that is just in alpha state.
This packages uses a custom Physics System for burst compiled and high performance scenarios.

## Summary 
This is the Shatter Fantasy Topdown ARPG Unity package that can be used to create any game needing top down ARPGs like retro Zelda. 

## Current Alpha: Alpha One
- Added a custom SF Physics module that doesn't use any collider/rigidbodies. All physics components are custom and written by me.
- Added a set of burst compiled math formulas and utilities for performance.
- Added a Dialogue System using Unity's Graph Toolkit Foundation.


## Known Issues
These issues are known and are currently being worked on if it is not a Unity Engine side bug. 
For the most stable Unity version use Unity 6.5 beta 8 at minimum. I worked with the Unity team directly for Unity 
6.4, 6.5 and 6.6 alpha to help resolve engine bugs. 

The remaining bugs involve Tilemap render mode being set to SRP. Avoid SRP batch mode for Tilemaps till Unity patches the crash during the 6.5 beta cycle.

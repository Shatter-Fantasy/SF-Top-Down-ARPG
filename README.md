# SF-Top-Down-ARPG-Package
There is a work in progress dedicated documentation website being made.

## Summary 
This is the Shatter Fantasy Topdown ARPG Unity package that can be used to create any game needing top down ARPGs like retro Zelda. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.

## Current Alpha: Alpha One
- Implementing SF Low Level Physics v 0.0.1 package

## Known Issues
These issues are known and are currently being worked on if it is not a Unity Engine side bug.

### Unity Side Bug: Unity 6.4 beta to Unity 6.5 Rule Tile Crash
There are Tile Related crashes that are caused by EntityID related API implemented from Unity 6.4 and newer versions.
This is related to the OnTileRefreshPreview that is called by Unity.

### The Cinemachine Physics2D Shape is early WIP.
As of December 1st it is being worked on so not a smooth confiner yet for Cinemachine cameras.
Most likely be in alpha seven.

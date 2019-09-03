# Resolink

This package aims to make integrating a Unity app with a Resolume setup easier.

## Table of Contents

[Time Sync](#time-sync)

### Time Sync

There are two built-in behaviors for keeping time in sync.  Both can be disabled.

##### Pause

When you press `Pause` in Resolume, Unity's time scale is set to 0, which pauses anything not using unscaled time.

When you un-pause, Unity's time scale is set back to its previous value.

This relies on having an OSC output setup for the `Pause` button in Resolume.


##### Resolume BPM -> Unity Time Scale

When you start a session, the BPM you are at in Resolume is equivalent to a time scale of 1.

As you change BPM in Resolume, Unity's `Time.timeScale` will follow along.

This relies on having an OSC output setup for the BPM in Resolume.


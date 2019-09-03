# Resolink

This package aims to make integrating a [Unity](https://unity.com/) app with a [Resolume](https://resolume.com/) setup easier, by doing three things.

1) Generate event handlers in a Unity scene based on the events you setup in Resolume

2) Handle texture sharing from Unity -> Resolume

3) Keeping time / tempo in sync

## Table of Contents

[Aknowledgements](#aknowledgements)

[Dependencies](#dependencies)

[Time Sync](#time-sync)

### Aknowledgements

This wouldn't be possible without the work of [Keijiro Takahashi](https://github.com/keijiro). All 3 packages this depends on were developed by him.

### Dependencies

[OscJack](https://github.com/keijiro/OscJack) for handling OSC messages

[KlakSpout](https://github.com/keijiro/KlakSpout) (on Windows) or [KlakSyphon](https://github.com/keijiro/KlakSyphon) (on Mac) for sharing the rendered texture to Resolume

For now, dependencies are bundled in the package.

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


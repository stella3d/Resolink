# Resolink

This package aims to make integrating a [Unity](https://unity.com/) app with a [Resolume](https://resolume.com/) setup easier, by doing three things.

1) Generate event handlers in a Unity scene based on the events you setup in Resolume

2) Handle texture sharing from Unity -> Resolume

3) Keeping time / tempo in sync

## Table of Contents
[Aknowledgements](#aknowledgements)

[Installation](#installation)

[Supported Data Types](#supported-data-types)

[Dependencies](#dependencies)

[Time Sync](#time-sync)

### Aknowledgements

This wouldn't be possible without the work of [Keijiro Takahashi](https://github.com/keijiro). All 3 packages this depends on were developed by him.

### Installation

To use Resolink, please download the .unitypackage for your platform from the [Releases](https://github.com/stella3d/Resolink/releases) page.

You can also clone this repo directly into a Unity project, but you will get tests & dependencies your platform doesn't need.


### Supported Data Types

Resolume natively sends the overwhelming majority of data over OSC as an `int`, `float`, or `string`.

Any Resolume control that has an OSC Type Tag of `Int 0 or 1` is interpreted as a `bool`, where 0 is false.

All of these 4 types get components like this in Unity, which show the path to receive OSC at and the event to fire when a message is received.

![unity float control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_FloatControl.PNG)

Resolume sends most floating point information as a number from 0 to 1, so it may be necessary to scale or interpolate these values to achieve your specific goal.

#### Compound Controls

Resolume has controls that send vectors and colors, but it sends the value of each member of these controls to a different OSC address.  To support these controls as a single unit, we group them on the Unity side.  Resolink should automatically detect these groups for you if grouping is enabled, as it is by default.

You can toggle grouping of controls off in the Preferences menu to get `float` handlers for these controls instead.

Compound control types include `Vector2`, `Vector3`, `Quaternion`, & `Color`.

###### Vectors

In Resolume, vector controls look like this.

![resolume vector control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_Editing_VectorControl.PNG)

In Unity, you get a component like this associated with the Resolume control.

![unity vector control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_VectorControl.PNG)

###### Rotations

In Resolume, rotation controls are represented as Euler angles.

![resolume rotation control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_Editing_RotationControl.PNG)

In Unity, Euler angles are converted to a `Quaternion`, and you get a component like this associated with the Resolume control.

![unity rotation control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_RotationControl.PNG)

A note about rotations - Depending on where you position the Unity camera, the rotation can appear to be different than it is in Resolume.  I spent a while trying to work out a solution where it always looks the same, but i'm not sure if that exists.  For now it just provides a direct transmission of the angles that you see in the resolume UI.

###### Colors

Color controls are currently supported by enabling the OSC output for all 4 RGBA values, as seen below.
![resolume color control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_Editing_ColorControl.PNG)

In Unity, you will get a component like this associated with that group of controls from Resolume.
![unity color control](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_ColorControl.PNG)

Resolume _can_ send color information over OSC as a single message in a special format, but this isn't supported yet.  

### Dependencies

For now, dependencies are bundled in the package.

[OscJack](https://github.com/keijiro/OscJack) for handling OSC messages

For sharing the render to Resolume,

(on Windows) [KlakSpout](https://github.com/keijiro/KlakSpout), which is an interface to [Spout](http://spout.zeal.co/) 

(on Mac) [KlakSyphon](https://github.com/keijiro/KlakSyphon), which is an interface to [Syphon](http://syphon.v002.info/) 

### Time Sync

There are two built-in behaviors for keeping time in sync.  To disable either, either don't output the required OSC from resolume, or disable the associated component in Unity.  You'll find everything related to time sync on the `Tempo Controller` object in the Resolink prefab.

##### Pause

When you press `Pause` in Resolume, Unity's time scale is set to 0, which pauses anything not using unscaled time.

When you un-pause, Unity's time scale is set back to its previous value.

This relies on having an OSC output setup for the `Pause` button in Resolume, shown below.

![Resolume pause output](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_Pause_Output.PNG)


##### Resolume BPM -> Unity Time Scale

When you start a session & until you receive at least 2 tempo messages, the BPM you are at in Resolume is equivalent to a time scale of 1.

As you change BPM in Resolume, Unity's `Time.timeScale` will follow along.

This relies on having an OSC output setup for the BPM in Resolume, as shown below.

![Resolume BPM output](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_BPM_Output.PNG)


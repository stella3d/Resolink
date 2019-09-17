# Resolink

This package aims to make integrating a [Unity](https://unity.com/) app with a [Resolume](https://resolume.com/) setup easier, by doing three things.

1) Generate event handlers in a Unity scene based on the events you setup in Resolume

2) Handle texture sharing from Unity -> Resolume

3) Keeping time / tempo in sync


This is _not an official product_ of either Unity or Resolume.

## Table of Contents
[Aknowledgements](#aknowledgements)

[Installation](#installation)

[Setup](#setup)

[Supported Data Types](#supported-data-types)

[Time Sync](#time-sync)

[Options](#options)

[Current Limitations](#current-limitations)

[Dependencies](#dependencies)

[Contact](#contact)

### Aknowledgements

This wouldn't be possible without the work of [Keijiro Takahashi](https://github.com/keijiro). All 3 packages this depends on were developed by him.

### Installation

To use Resolink, please download the .unitypackage for your platform from the [Releases](https://github.com/stella3d/Resolink/releases) page.

You can also clone this repo directly into a Unity project, but you will get tests & dependencies your platform doesn't need.

Please note that Resolink has been tested only on **Unity 2019.x**, and may not work on earlier versions.

### Setup

The process of generating event handlers in a Unity scene for a given Resolume setup has 3 main parts.

1) **Resolume OSC Setup**

   Setup OSC output from Resolume as you would for any use.  
If you're unfamiliar with how this works, [check out the documentation](https://resolume.com/support/en/osc).  
For every control you want to send a message with, right-click and `Create Shortcut`.  Make sure that OSC output is enabled for these controls as shown.
![Resolume OSC output indicator](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_OscOutputEnabled.PNG)

When you're done, save these OSC settings somewhere - go to the Shortcuts panel in Resolume & select `Save As` from the dropdown.  

Note the port number you've setup to send OSC to.

2) **Creating a Unity asset**

In Unity, open the Resolink window: `Window > Resolink > Map Parser`.  You should see a window like this.

![Resolink unity window](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_Resolink_Window.PNG)

Click the first button, `Select Resolume OSC Map File`, and select the file you saved out of Resolume earlier.

Once you've done that, click `Create OSC Map Asset` to create an asset in your project that has all the settings you exported from Resolume.  It will be selected for you - look around in the inspector for the asset to learn more ! 

3) **Scene Setup**

Now that you have a map asset, drag the "Resolink" prefab (located at `Resolink/Runtime/Prefabs/`) into your scene.  Select it, and you should see this.

![unity component generator](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Unity_Component_PreGeneration.PNG)

Assign your map asset to the `Osc Map` field, and click `Generate Event Components`.  
For every address / control in your OSC map, a component will be added to the appropriate object within the Resolink prefab.  

These components are where you hook up [Unity Events](https://docs.unity3d.com/Manual/UnityEvents.html) to call when a message is received at each address.

Putting the Resolink prefab in your scene also sets up video sharing back to Resolume.  If you look in the `Sources` panel in Resolume, you should see a Spout (on Windows) or Syphon (on Mac) server with the name of your Main Camera object.  Assign this to one of your clip slots in resolume to use it as a video source.

![resolume spout server](https://raw.githubusercontent.com/stella3d/resolink-doc-img/master/Resolume_Spout_Server.PNG)

You should make sure the port number shown in the `Osc Router` component is the same as the OSC port you setup in Resolume.

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

### Wildcard Routing

Resolume lets you setup shortcuts that fire for any layer or clip via [wildcards](https://resolume.com/support/en/osc#wildcards) `/*/` in the OSC address.

Resolink supports these wildcards via pattern matching.

### Options

Resolink options can be found under `Edit > Preferences > Project > Resolink`.

You can disable the help boxes shown on most components, as well as enabling / disabling grouping of complex controls.

### Current Limitations

There are some intentional limitations on this initial release, and planned future features.

1) Sending OSC from Unity to Resolume is _not yet implemented_, but _will be_, sooner rather than later.  This is because i haven't yet figured out some details of how that should work, and my personal use case is focused on driving Unity from Resolume.

2) Syncing Unity to the initial state of a Resolume control will be implemented at the same time as general Unity -> Resolume communication.  Until then, Unity isn't aware of the value for a control until Resolume sends a message for it.

3) Arena-specific features aren't yet supported by the parser.  
This is just because I use Avenue - they will be implemented in future releases.

4) No included runtime UI for configuration or debugging.  It's intended to use the Editor to set everything up for now.   

5) Only tested on recent versions of the softwares - Unity 2019.x & Resolume 7.

### Dependencies

For now, dependencies are bundled in the package.

[OscJack](https://github.com/keijiro/OscJack) for handling OSC messages

For sharing the render to Resolume,

(on Windows) [KlakSpout](https://github.com/keijiro/KlakSpout), which is an interface to [Spout](http://spout.zeal.co/) 

(on Mac) [KlakSyphon](https://github.com/keijiro/KlakSyphon), which is an interface to [Syphon](http://syphon.v002.info/) 

### Contact

If something is broken, feel free to submit an issue.  Feature requests, not yet please.

To get in touch in general, [get at me on Twitter](https://twitter.com/computerpupper).



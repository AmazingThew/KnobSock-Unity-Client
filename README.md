# Unity client for Knob Sock
This is a Unity client for the [Knob Sock MIDI server]( https://github.com/AmazingThew/KnobSock).
It works in edit mode!

# Usage
* This is a Unity Package, so you can add it to a Unity project by adding the github URL in the package manager
* This package does not include the Knob Sock server itself; you'll need to download and run that for the client to connect
* Once the client package is installed, you can write scripts that call Knobs.Get() to read the knob values
* Scripts using ExecuteInEditMode/ExecuteAlways can also call Knobs.Get() in edit mode without any problems
* If you want to access knobs from shaders, the knob values need to be bound to uniforms. You can do this however you want, but the package includes a prefab named Knob Uniforms that you can just drag into your scene. It will bind all knob values to global uniform floats named Knob[number]
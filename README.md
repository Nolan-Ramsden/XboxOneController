# XboxOneController
[![Build status](https://ci.appveyor.com/api/projects/status/pcm374wditsa292t?svg=true)](https://ci.appveyor.com/project/Nolan-Ramsden/xboxonecontroller)

A C# library to send On &amp; Off commands to your XboxOne

[Available On Nuget](https://www.nuget.org/packages/XboxOneController/)

### Usage

The controller is pretty straight forward, call On, Off, or Ping to check if the Xbox
is on.

```
IXboxController xbox = new XboxController("IPAddress", "LiveId");

await xbox.TurnOn();
await xbox.Ping();
await xbox.TurnOff();
```

### PreReqs

To create an instance of the controller, you need two pieces of information:
  - The XBone's IP address (Settings->Network->Advanced Settings)
  - The XBone's Live ID (Settings->System->Console Info)

You'll also have to have your Xbox's power settings to "Instant-On" for this to work, so that
the Xbox is actively listening for commands.

### Help Needed

There's still a bunch of things I'd like to be able to do.

  - Implement the off command (apparently more involved than the on command)
  - Interpret the responses from the Xbox (I have no idea what they are)
  - Add more commands like GetStatus etc.

### Thanks

This source is pretty much a pure C# port of these two libraries, thanks for doing 
the hard work.

  - [Python](https://github.com/Schamper/xbox-remote-power/)
  - [Node.JS](https://github.com/arcreative/xbox-on/)
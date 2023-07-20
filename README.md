# What this Mod does

This is a small mod that adds customizable titles to Mount & Blade II: Bannerlord. Additionally, the names of owned villagers and caravans, and the names of armies belonging to lords can also be customized.
These options can be set in a general manner for kingdoms and cultures depending on clan tier or relation to the ruler or more specific, for individual characters or clans.
This mod comes with a configuration UI to create, edit and delete your own title configurations.
There can be multiple active configurations at the same time.
All active configurations are searched in order of their priority, defined in the configuration UI, until a configuration with title properties for the current lord is found.

When there are multiple sets of properties for the same lord they are merged together. In general, the more specific property takes priority over less specific properties when merging. 
Note, that this is only within one configuration, properties coming from different configurations are never merged.

For example:

A lord belongs to the clan "fen Seanel" and to a kingdom with the culture "Empire". There are rules defined for both the culture "Empire" and the clan "fen Seanel", then all properties defined for the clan "fen Seanel" overwrite all such properties in "Empire" for this lord, but all properties in "Empire" which are undefined in "fen Seanel" are still used. If we extend this example to also include the lords specific kingdom "Western Empire", then all properties defined for "Western Empire" overwrite those from the culture "Empire", but are still overwritten by those from the clan "fen Seanel".

Hopefully the hints in the configuration UI are enough to figure out the priority order when editing a configuration.

# Installation

Head over to the [NexusMods](https://www.nexusmods.com/mountandblade2bannerlord/mods/5748) page of this mod to install 

# For Modders

Modders of (total) conversion mods can also use the configuration UI of this mod to create and export a title configuration to include in their mod. That configuration is then automatically loaded by this mod if this mod is also installed by the user. When exporting a configuration, it is possible to set a UID, which can in the future be used to update your configuration by exporting a new configuration with the same UID.

# Additional Notes

The source code contains outdated syntax, because this mod was originally developed in .NET Framework 4.7.x, because I followed an outdated ((?) or perhaps that broke some things I'm not aware of) tutorial when starting to write this mod. I only copied the source code over to a new project in .netstandard 2.0 after I finished writing it.

The project configuration assumes that steam is installed in its default location (i.e. C:\Program Files (x86)\Steam) for building.

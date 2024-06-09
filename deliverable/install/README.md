# AutoCADCommands DLL

## Installation Instructions

1. **Copy the DLL**:
   - Copy `AutoCADCommands.dll` from the `install` folder to a location accessible to your AutoCAD installation.

2. **Open AutoCAD**.

3. **Load the DLL**:
   - Type `NETLOAD` in the command line.
   - Browse to the location of `AutoCADCommands.dll` and select it.

4. **Run the Command**:
   - Type `mtxrenumber <color> <prefix>` in the command line.
   - For example, to renumber green MText elements with the prefix `xyz`, type: `mtxrenumber green xyz`.

## Example
If you have green MText elements with the text `green xyz` and you run the command `mtxrenumber green`, the texts will be updated to `green xyz 1`, `green xyz 2`, etc.

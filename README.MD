# DDM-HDRPeak1000
## Only tested on an Alienware AW3423DWF, use at your own risk!

Dell Display Manager has a CLI that allows you to switch to any color preset your display has, EXCEPT for the HDR Peak 1000 preset. So here's the work around.

DDM doesn't seem to like when another program other than itself sends commands to monitors, so it might crash or reboot on it's own.

Usage:


You will need Dell Display Manager installed and running. Run `setup.bat` on the first time, this will copy required DLLs from DDM (that I prefer to not distribute).
```
DDM-HDRPeak1000.exe <off|on> [-i <i>]

  off        Turn off HDR Peak 1000. Toggling off turns SmartHDR off.
  on         Switch to HDR Peak 1000.
  help       Display more information on a specific command.
  version    Display version information.

  -i         Index of the monitor to change (should be the same as the DDM CLI), defaults to 0.
```
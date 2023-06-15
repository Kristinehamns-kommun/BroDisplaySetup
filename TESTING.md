# Testing

## Preparation

Read [Known issues](README.md) before testing.

Erase all known display configurations saved on the computer. There is a reg file for this in the `extras` folder.

Restart the computer after erasing the display configurations.

## Testing `BroDisplaySetup`

* Try to run the program and see if it works when there are no known display configurations.

* Switch between different display configurations and see if the program works. 
   * Do this while the computer is shut-down and while it is running.

* Go into Windows 10/11 display settings and make some 'misconfigurations' such as incorrect resolution, changing the arrangement of the screens, or placing them in the wrong order.

* Set the primary display to different displays and see if the program works and selects the left-most external screen as the primary display.

* Try to run the program with only one screen connected - this should only adjust the resolution of the screen to the optimal resolution and exit.

* Try to start the computer connected to multiple displays with the lid closed and run the program.

* Try to "fuzz" the inputs to the program by entering invalid values and see if the program crashes.




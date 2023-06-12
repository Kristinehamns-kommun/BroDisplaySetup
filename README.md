# BroDisplaySetup

BroDisplaySetup is an open source program that facilitates the configuration of the screen settings on the user's workstation. The ambition is to create a simplified form of the screen settings interface in Windows that is easy to use for all users.

## Purpose

The purpose of this program is to try to reduce the number of support cases related to screen settings that arise from the fact that Windows has a relatively complicated interface for screen settings that can be difficult to understand for users who are not used to working with computers. This problem is exacerbated in activity-based office environments where users often change workplaces.

## Usage and features

To use BroDisplaySetup, follow these steps:

1. Start the BroDisplaySetup program.
    - The program automatically detects all connected screens and an optimal resolution for each screen is retrieved from Windows.
    - The desktop is automatically extended to all connected screens

1. Each connected screen is covered by a box with a large and clear number that identifies the screen
1. The user is presented with a text field for each screen (on the current primary screen) and prompted to enter the numbers as they appear on the screens, following the order they are read in.
    - Example: If there are three connected screens (incl. the laptop folded up) and it says 2-3-1 on the screens (from left to right) you are expected to type 2 3 1 to fill in the text fields.

1. The screens are arranged from left to right in the order that the user specified for the extended desktop.
    - All screens are positioned so that the bottom edge of the screens are aligned with each other.
    - The external monitor furthest to the left will automatically become the primary monitor (The laptop's internal monitor will never become primary if there is one or more external monitors).
1. Recommended (optimal) resolution for all connected screens is automatically applied.

## Compatibility

BroDisplaySetup is compatible with Windows 10 and Windows 11.

The program has been tested with laptops from HP and with screens from Dell and HP.

More testing with other types of hardware is needed but works well for the devices used at Kristinehamn municipality.

## Known issues

Screens that are connected while the program is running will not be detected automatically. To detect new screens, restart the program.

The program has no handling for screens that are disconnected while it is running. To detect disconnected screens, restart the program. 

## Screenshots

![alt text](./screenshots/screens_start.png "Start screen")
![alt text](./screenshots/screens_typed.png "Typed screens")

## Contributing

Contributions are welcome! If you want to contribute to BroDisplaySetup, follow these guidelines:

1. Fork the project and clone it to your local machine.
2. Make your changes and test them thoroughly.
3. Create a new branch for your feature or bug fix.
4. Commit your changes with descriptive commit messages.
5. Push your branch to your forked repository.
6. Open a pull request and describe your changes in detail.

## License

BroDisplaySetup is released under the [GNU General Public License](LICENSE.md).





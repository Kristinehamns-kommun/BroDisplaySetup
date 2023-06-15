# Extras

* `BroDisplaySetup.reg`
	* This example adds a context menu item "Arrangera bildskärmar" (Arrange Displays) to the context menu that is available when right-clicking on the desktop
	* In the example, the path for the program in the registry is set to `C:\Data\Tools\BroDisplaySetup.exe` (and `BroDisplaySetup.exe` is deployed with SCCM).
	* In the example, the path for the icon in the registry is set to `C:\Data\Tools\Settings3.ico`.

* `ClassicFullContextMenuWindows11.reg`
    * This regfile only applies to Windows 11 where "Arrangera bildskärmar" (Arrange Displays) is not immediately available in the context menu.
    * In Windows 11, the context menu does not show all the entries by default as it does in Windows 10. To access the menu entries that are not shown in the default listing, you need to click the "Show more options" entry every time.
	* This regfile restores the behaviour from Windows 10 such that the full/classic context menu is displayed instead of the default condensed menu. The "Show more options" entry is removed.
	* This will show the context menu item "Arrangera bildskärmar" (Arrange Displays)immediately upon opening the context menu after right-clicking.

* `EraseScreenConfig.reg`
	* This reg file can be used when testing `BroDisplaySetup` to erase all settings for the screen configurations that exist on the computer.



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

* Connect a single large external screen (~50" diagonal or larger, eg. a conference room display) and verify:
   * The "scale displays" checkbox is hidden and the screen is automatically scaled to 250%.
   * A "Konferensrum: gör den bärbara skärmen till huvudskärm" checkbox appears in its place (in the same slot the "scale displays" checkbox would otherwise occupy), checked by default, and the help text explicitly mentions conference room mode.
   * Unchecking it before finishing the screen numbering flips the help text to the "automatic scaling only" wording, and the internal screen is *not* forced primary after arranging - the usual left-most-external rule applies instead.
   * Leaving it checked sets the internal screen as primary after arranging - even if it wasn't primary before.
   * Re-running the program with the same screen defaults the checkbox to the remembered answer from last time (persisted in `%AppData%\BroDisplaySetup\conference-rooms.json`), not always checked.
   * **Avancerat > Konferensrumsläge** starts checked/unchecked to match the checkbox, and toggling either one live-updates the other (and the help text) immediately - test both directions.
   * Toggling either one before finishing the screen numbering persists the new answer (verify by relaunching without touching it again).
   * **Avancerat > Glöm konferensrumsval...** clears the remembered answer; the checkbox defaults back to checked on the next run.
   * Check `Avancerat > Visa skärminformation...` and confirm the screen's reported `PhysicalSize`/diagonal looks correct, and that it shows a "Konferensrumsläge: Ja/Nej" line matching the checkbox/menu state.
   * If the screen also lists a wider "signal" resolution than its native panel resolution in Windows' own display settings (eg. 4096x2160 DCI 4K alongside a 3840x2160 UHD panel, where Windows marks the narrower one "Recommended") - confirm the applied resolution matches Windows' "Recommended" mode and the image fills the screen edge-to-edge, without any part of the desktop cropped off the sides.

* With no large external display connected (or more than one - eg. two normal external monitors), verify:
   * No conference-room checkbox appears (the "scale displays" checkbox, if applicable, is unaffected).
   * **Avancerat > Konferensrumsläge** is still enabled (as a manual fallback) and unchecked by default; checking it before finishing the screen numbering still sets the internal screen as primary after arranging, updates the help text accordingly, but does *not* get remembered for next time (there's no single candidate screen to key the answer against).




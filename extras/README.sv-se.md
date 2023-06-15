# Extras

* `BroDisplaySetup.reg`
	* Detta exempel skapar alternativet "Arrangera bildskärmar" i snabbmenyn (context menu) som öppnas när man högerklickar någonstans på skrivbordet 
	* I exemplet är sökvägen för programmet satt till  `C:\Data\Tools\BroDisplaySetup.exe` (där `BroDisplaySetup.exe` är utskickad med SCCM)
	* I exemplet är sökvägen för ikonen i registret satt till `C:\Data\Tools\Settings3.ico` (och `Settings3.ico` är utskickad med SCCM)

* `ClassicFullContextMenuWindows11.reg`
    * Denna regfil gäller endast för Windows 11, där "Arrangera bildskärmar" inte är omedelbart tillgängligt i snabbmenyn.
    * I Windows 11 visas inte alla menyalternativ som standard i snabbmenyn, som det gör i Windows 10. För att få tillgång till menyalternativen som inte visas i standardlistningen måste du klicka på posten "Visa fler alternativ" varje gång.
	* Denna regfil återställer beteendet från Windows 10 så att den fullständiga/klassiska snabbmenyn visas istället för den förkortade standardmenyn. Posten "Visa fler alternativ" tas bort.
	* Efter att denna regfil har körst kommer snabbmenyalternativet "Arrangera bildskärmar" visas omedelbart när du öppnar snabbmenyn genom att högerklicka på skrivbordet. Detta gäller inte för Windows 10 där man ser "Arrangera bildskärmar" direkt.

* `EraseScreenConfig.reg`
	* Denna regfil kan används vid test av `BroDisplaySetup` för att radera alla inställningar för de skärmkonfigurationer som finns på datorn.
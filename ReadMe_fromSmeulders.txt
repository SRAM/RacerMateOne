The build instructions by Prolific still apply.  Some changes:

1) Obfuscated RM1.dll and build states.  When the build is in Debug mode, the
un-obfuscated RM1.dll is selected for the output directory, and the php-based
variable-substitution script by Larry MCourry is set to run. When you change
from Release to debug mode, you need to do TWO builds, since the first run puts
the dll and files out of sync. Subsequent builds in Debug require only one
build. Likewise, when When the build is in Release mode, the obfuscated RM1.dll
is selected for the output directory, and the php-based variable-substitution
script by Larry MCourry is set to run. When you change from Debug to Release
mode, you need to do TWO builds, since the first run puts the dll and files out
of sync. Subsequent builds in Release mode require only one build.

2) Release builds use the Dotfuscator Obfuscator from Pre-emptive solutions.
The configure files are in the relevant directories. The license for this is
ErgVideo's, (RacerMate has never been charged for this) so RacerMate will need
to get their own if they want to obfuscate .net code. the configuration file
last used is 

\RacerMateOneBuild4.0.2\RacerMateOne_Source\Dotfuscator8RacerMate1NOTSignedNoFlow.xml

Meaning the dotfuscator is not used to sign the output. I did it manually after
obfuscation using signtool.exe, part of the visual studio tools. As you add UI
elements, depending how they ar referenced, you may have to adjust settings so
that obfuscation does not break references to XAML elements, which must stay
unobfuscated. As Dotfuscator evolves, these become less and less necessary.


3) Code is signed with Microsoft Authenticode signatures from digicert
registered to RacerMate, but "owned" and resident on Smeulders' build machine.
RacerMate has never been charged for this. I sign the built .net .exe from
Visual studio as well as the completed installer .exe. If desired, I can look
into transferring this certificate to RacerMate, or RacerMate can apply to
Digicert for a new one. There is a new level of code signing security that MS
has contracted with Symantec to distribute for win 7 and win 8 going forward.
They have a reasonable price set for this, since Symantec alone is outrageously
priced. It appears this is the level of security MS wants going forward into
the new age.

Changes for Build 4.0.4 and beyond.

1) THe code has been marked with compiler options to use either the obfuscated
dll (rm1.dll) included with distribution for release build configuration. For
debug build, you still require a RM1CLEAN.dll which is not obfuscated. This
does aid in debugging. Compiler options and #if DEBGU/#else/#endifs control the
compiled output. No more need for the php scripts to do name substitution.

2) Visual Studio 2012 no longer creates an MS installer. THerefore the build
using VS2008 still applies.  Actually, this does mostly nothing except the pre
and post build events in the setup project, and that follows the NSIS
generation. One change in teh patch build: Change the "load prerequisites from
same location" to "vendors website". Since this is a patch, there is no need to
re-package the pre-req's, they were installed in the base version...that's why
the base version is so damn big.



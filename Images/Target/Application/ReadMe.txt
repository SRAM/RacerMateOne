RacerMate One Release 1 Build 4.0.6

Build date Nov 28, 2013

____ Minimum System Requirements ____

• Processor: Intel Pentium 4 (or equivalent) and higher.  Dual-core or higher preferred.
• System RAM: minimum as required by the operating system used.  2 GB or more preferred.
• Operating System: Windows™ XP/Vista/Win7
• Data Communication: One Serial Port or USB Port per trainer.
• Video: DirectX 9 compatible video card with 256MB of dedicated Video RAM or higher - minimum. Note: Running 3D Cycling with more than one pacer may require more video RAM. Ideally 512MB or higher is preferred. 
• CD-ROM drive (needed for application software installation purposes only)
• DVD-ROM drive (needed for Interactive Real Course Video installation)

Note: THe majority of failures to run RacerMate One software were due to very old computers - or video cards that were too small, old, or their drivers were not up to date (when available).  PLEASE update your drivers, and if you don't know how, seek the advice of a computer specialist.  Laptop users should be especially aware that drivers and hardware may not be available to improve their computer because the manufacturers do not apply the same level of support to laptops as they do desktop computers - especially where gaming graphics are used.


____What's new with version 4.0.5?____

• Course designer by Agha Khan with bugs cleared
• Cosmetic changes reverted to design originally released.
• Velotron left/right issue resolved

____What's new with version 4.0.4?____

• Course designer by Agha Khan

____What's new with version 4.0.2?____

• This is the first general release.

Known issues:

• Can't remove USB-serial adapter while running
Correct operation assumes the virtual com ports discovered at RM1 start and discovery remain valid. Do not remove the USB-serial adapters while RM1 is active. The program is tolerant to removing the stereo cables, but removing the USB adapters causes the operating systemr (OS) to lose registration of the com port. 
Resolution: Users may have to reboot their system to remedy the resulting lock-up.
• Using a saved data preset profile does not work.
The File save options to selectively include or exclude performance data are actually ignored in the *.rmp files, which store all collected data regardless of these settings. 
Exported files do as well right now. 
The PWX format is fine and only includes a limited set of data according to that file format's standard, which does not include many of the parameters collected by RM1 and available in .rmp. 
• VeloTron Gear selection is not passing through to real-time application.
The VeloTron is not completely supported in the initial release.
• Average speed and power updates in 0.2 increments.
The data isn't inaccurate.  This is some rounding issue.
• VeloTron SpinScan has Left-Right leg data reversed.
The VeloTron is not completely supported in the initial release.
• The 3d window shows blank.
This issue is caused by the user's system changing to a full-screen display by another application, while the RM1 is active. For example, on Windows 7, Control-Alt-Delete will show a full screen view to switch user or show task manager etc. Upon return, the 3d window is black. If the user types Shift~ or gets a crash, the error will read "SetBackBuffer device not valid." 
Resolution: disable other applications that may start and take over the full screen from all windows. If a user needs that task manager, Control-Shift-Escape will call it up on all OS's supported (XP, Vista, & 7), or they can right click the task bar and select "Start Task manager". It is worthwhile to get the user's error log.
• A file xp32y17 is created in {Users}\{AccountName}\ root directory.
It's a small temp file. We will remove on later updates. It is safe for the user to remove.
• An RCV performance will not playback correctly in a 3d ride if the start point was not at the start of the RCV course.
We expect nobody to be affected, as RCV users will likely use RCV again to ride against their performance, rather than 3D. This may be fixed on subsequent updates, or we will prevent RCV perfs to playback on 3D rides.
• Erg rides can show in performance pacers selection.
It is unlikely a user will choose this option. If it causes a crash or a problem, please document it and try to track how often it is seen. It will eventually be fixed.
• Reports Bugs: Gear of 0 shows in reports even if the trainer is not a VeloTron
Inconsequential. Low priority to fix.
• Reports bugs: Left Right power (as total watts) and cadence timing and Raw SpinScan is not recorded.
These measures are considered secondary, as the LR split in % is shown, and is more informative in % than it is in watts. In fact, it's meaningless in watts. Cadence timing and Raw SpinScan recording feature have been deprecated.
• 3D ride with 3 or more riders can have incorrect grade applied to incorrect trainer targets. Known workaround: This happens when you change rider positions in the staging screen after starting the program. If you have >2 riders, establish their position in the 3d Cycling ride setup window. Configure riders and pacer positions. Start a ride. Stop the ride, and then exit the program and restart. The riders and pacers will be in the same positions. Now start the ride.
• Multiple Rider saved reports/exports can reuse rider names incorrectly. Use same workaround as described above.

____ RacerMate One Release schedule ____

RacerMate One is a replacement software package for all previous versions of RacerMate software - for both CompuTrainer and Velotron.  Due to the size and scope of this project, the release schedule was planned to be broken into pieces.  This also helps manage the software testing.  There is no specific release dates for these after the initial release, but the tentative plan after the first release in July, 2012 will be Release 2 progress beginning again sometime in the Fall of 2012 and each other to follow as time allows.

The releases where broken up as follows:

Release 1 - CompuTrainer home users.  Contains 3D Cycling with 8 rider capability (riders and pacers), Real Course Video (up to 2 riders); Power Training (replaces previous Coaching Software); SpinScan Testing (full-featured version with added features from previous editions).

Release 2 - Add Course Creator tool.  Note: You can still use all the previous course building tools and techniques and import those courses into RacerMate One by browsing for them from within any application's setup screen.

Release 3 - Velotron users (note: many Velotron functions exist.  Velotron users will be brought in for beta testing during release 2 work).

Release 4 - Classic MultiRider functions added and MultiRider centers brought in for testing.  (note: 3D Cycling allows up to 8 live riders and can be used as a MultiRider application).

____ What you will see (and maybe not see) in release #1 ____

The main goal of RacerMate One was the consolidation of applications into one cohesive experience.  We have accomplished this and then some.  Each application will run from a single set of settings and a single rider database.  All courses are also contained in one location for all applications.

When first launched, RacerMate One will attempt to help you setup your rider profile by asking you simple questions.  By following these you can fast-track directly to using the software.

RacerMate One requires registration.  The software disc sleeve has this key, which can be used by the same user on as many as two distinct computers.  You can uninstall and reinstall on these two computers as many times as you want.

There is no printed manual for RacerMate One.  The entire software contains a manual built in - just click Help and look for your answer on the page you are on.

Course creation and course are different in RacerMate One.  The software is fully capable of importing any course you may have in your course library, or that may be found on-line.  Use the Browse on any application setup screen to look for courses and RacerMate One will ask to save it as a RacerMate One version.  There will be a new Course Creator application completed shortly to allow you to use a variety of files as courses and to create other courses in a graphic user interface.  Look for that soon.

You will find a series of tabs in the Options menu to address all the various settings driving RacerMate One.  These are:
• Riders tab - this is where you can setup your riders and the settings used by the riders including the 3D cyclist image.
• Hardware tab - setup the hardware connected to RacerMate One.  Though an attempt was made when you run the software to connect to the trainer(s), you can manually manipulate the trainer hardware in this tab.
• Display tab - allows you to setup the data shown on each application and also save custom displays as well.  Setup the global metric setting here as well.
• File Saving tab - setup save options for saving performances, exports, and reports.
• Advanced tab - setup Power Training graph scale and set path for Real Course Video, if you move them.

Each application has a rider and course setup screen.  
• Because each application varies in capability, some courses may show up in one application and not the other.  This means you usually will not need to browse for courses, unless they were not supplied with the software.  You can import any course you may have had in previous versions of RacerMate software, and RacerMate One will offer to save the new RacerMate One style course.  
• Each application has a limitation on the rider and pacer settings.  3D Cycling has the most improvements in this regard.

Each application is fully scalable
• You can either resize the screen by grabbing and dragging the edges or maximize the screen to any size you want.  It will shrink or grow as needed.

Exporting and files saving.
• You can choose to be in control or let the software be in control with saving and exports.  By default we save every ride, but if you want to be prompted to save your rides, just enable prompting in the File Saving Options tab.

-3D Cycling Improvements-

There was a desire to improve the 3D experience, but the core foundation required far more work than anticipated, so the initial release saw most of the work in the adding of 8 riders, multiple improved pacers, and not on the actual scenery.  Scenery for RacerMate 3D software has also always required a generic scene system rather than a prebuilt 3D world, as is the case with most video gaming systems, or cycling systems where the world is one course, or a limited number of courses within that world.  That is fine when entertainment is the main goal, but any user of RacerMate hardware knows that priority #1 has always been making cyclist's faster.  With the foundation of the software improved, future improvements can be added.

The key improvements you will see in 3D Cycling are in the area of pacers.  By far the best feature is the addition of the SmartPacer™ which can operate from a multitude of parameters; like, heart rate, wattage, FTP and Aet, and speed.  Plus the ability to load performance files and adjust them to increase or decrease their playback.

-Real Course Video-

The key improvement is the addition of a second rider option by adding a upper/lower split screen.  If you load a performance file, it will also split the screen.

What you won't see in Real Course Video in release 1 is the registration tool.  RacerMate One can read the registrations from previous installations, but new Real Course Video's will need to be registered using a one-time use of the original Real Course Video application, which is supplied on the video disc.

-Power Training-

Power Training adds many features over the old Coaching Software application.  You can load any course type as well as read and play-back previous performance files.  It will also let you create courses using the manual control mode.  Sure to make this 2 dimensional tool a favorite.

-SpinScan Testing-

SpinScan has seen a great facelift in RacerMate One.  In addition to having both types of SpinScan available at all time, you can also now run SpinScan from courses.  Like Power Training, it also allows you to create courses using the manual mode.

____Contacting Service____

You can send RacerMate an email or call as you normally would regarding issues with the software.  Because this version is a test version, be sure to let us know you are running RacerMate One.

email: service@racermateinc.com
phone: 206-524-7392
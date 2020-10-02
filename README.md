# RacerMateOne
RacerMateOne is the software originally developed by RacerMate Inc for the CompuTrainer and Velotron cycling products. SRAM, LLC aquired RacerMate Inc, and has determined to release the RacerMateOne source code for the benefit of the community.  The RacerMateOne software has been released under the MIT License.

SRAM is seeking interested parties to help steer the project and contribute to its continued sucess.  Contact Aaron Sachau <development@sram.com> if you'd like to contribute, or if you have any other questions.


Getting the Source
------------------
1. Install Git from here: http://git-scm.com/download/win
  * Follow all default install options.
2. Make sure you can log into GitHub and have access to this repository: https://github.com/SRAM/RacerMateOne
3. Using the 'git bash' console window that was installed with git, enter the following commands:
  * Note that by default, git-bash should start in your user directory (ie: C:\Users\<username>)

```
$ mkdir GitHub
$ cd GitHub
$ mkdir RacerMateInc
$ cd RacerMateInc
$ git clone https://github.com/RacerMateInc/RacerMateOne.git
```

You should now have all the source needed to build RacerMateOne!
* Visual Studio 2015: GitHub/RacerMateInc/RacerMateOne/RacerMateOne_2015.sln
* Visual Studio 2012: GitHub/RacerMateInc/RacerMateOne/RacerMateOne_2012.sln

Brief Introduction to Git
-------------------------
Git and GitHub allow us to easily work independently on the source code and then be able to merge our changes back together whenever we have finished our changes. The repository on GitHub is often referred to as the "`origin`", and it's best to think of it as a location. At the `origin` location there is a `master` branch and from this `master` we may create additional branches to implement a new feature or bug-fix.

If you've followed the steps above to get the source, then you also have the code locally. Throughout Git documentation, this is called your "working tree". Any changes you make to your local working tree are local to you, and nobody else has access to those changes until you use `git push` to push them up to the `origin` location on GitHub. Once changes are on GitHub, other people can `git pull` and `git checkout` to get your changes onto their computer. With different branches, we can easily switch between eachother's versions of the code for code reviews, testing, bug fixing, etc. 


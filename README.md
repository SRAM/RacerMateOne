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

If you've followed the steps above to get the source, then you also have the code locally. Throughout Git documentation, this is called your "working tree". Any changes you make to your local working tree are local to you, and nobody else has access to those changes until you use `git push` to push them up to the `origin` location on GitHub. Once changes are on GitHub, other people can `git pull` and `git checkout` to get your changes onto their computer. With different branches, we can easily switch between eachother's versions of the code for code reviews, testing, bug fixing, etc. More on this below.

Creating a New Branch to Add a New Feature
------------------------------------------
If you want to start adding a new feature, or fixing a bug, then it's recommended to create a local branch on which you'll do all your development for that issue. It's good to push this branch back to the `origin` occasionally (even if it isn't completely working) so that you'll always have a backup of your code and progress online.

In order to identify a branch as being a temporary or personal branch, it should begin with your username. This also helps us know who created it. The name should also indicate the purpose of the branch. Since it's the current topic of development, I'll use the new addition of wifi support for the example, assuming the user 'larry' is doing the work.

First, git-bash should tell you which branch you are currently on, if you are in a repository directory. Notice `(master)` below:
* If you're on a different branch, run `git checkout master`
```
MACHINE-NAME+user@machine-name MINGW64 ~/GitHub/SRAM/RacerMateOne (master)
$
```

1. Make sure you have the latest version of the master branch.

   ```
   $ git pull origin master
   ```
   or more simply (although this may prompt you to set some config options)
   ```
   $ git pull
   ```

2. Create a new branch for your development, and checkout the new branch.

   ```
   $ git branch my-descriptive-branch-name
   $ git checkout my-descriptive-branch-name
   ```
3. See the status of the new branch:
   ```
   $ git status
   On branch my-descriptive-branch-name
   nothing to commit, working directory clean
   ```
   * This confirms that there are no changes in our working tree (we didn't expect any since this is a new branch).
3. Start making your changes.
   * This may be as simple as copying in newly updated files, or actually making code changes or adding / removing files.

4. Check the status of your changes. For demonstration purposes, I've add a new file, removed a file, and edited a file.

   ```
   $ git status
   On branch my-descriptive-branch-name
   Changes not staged for commit:
   (use "git add/rm <file>..." to update what will be committed)
   (use "git checkout -- <file>..." to discard changes in working directory)
   
       modified:   README.txt
       deleted:    ReadMe_fromSmeulders.txt
   
   Untracked files:
     (use "git add <file>..." to include in what will be committed)
   
       new-wifi-support.txt
   
   no changes added to commit (use "git add" and/or "git commit -a")
   ```
   * This shows the modified file, deleted file, and the newly created file (which is not yet tracked by Git).

5. Let's pretend that I didn't mean to actually delete 'ReadMe_fromSmeulders.txt'; we need to get that file back! No problem! We can checkout that specific from the history that git stores:

   ```
   $ git checkout -- ReadMe_fromSmeulders.txt
   $ git status
   On branch my-descriptive-branch-name
   Changes not staged for commit:
     (use "git add <file>..." to update what will be committed)
     (use "git checkout -- <file>..." to discard changes in working directory)
   
        modified:   README.txt
   
   Untracked files:
     (use "git add <file>..." to include in what will be committed)
   
        new-wifi-support.txt
   
   no changes added to commit (use "git add" and/or "git commit -a")
   ```

6. Let's look at the changes that were actually made to confirm that we actually want to commit them to our history:

   ```
   $ git diff
   diff --git a/README.txt b/README.txt
   index 6ba3726..8b9eafc 100644
   --- a/README.txt
   +++ b/README.txt
   @@ -4,7 +4,7 @@ Source from the 3.2.153 Patch.
    - Added DirectX includs and lib into compile path so that the sdk does not need to be installed.
   
    Steps to compile
   - - Compile using Visual Studio 8.
   + - Compile using Visual Studio 2015.
      - Any path to the directory should work.
      - Open Solution RacerMateOne_Source/RacerMateOne.sln.  This has all the projects needed to build the current version.
   ```
   * this may have been a bad example since there are already hyphens in the document, but the changed lines are indicated by a leading '-' (for removed lines) and '+' for added lines; with git-bash, the removed lines are also colored red, while added lines are colored green. We can see that "Compile using Visual Studio 8." was changed to "Compile using Visual Studio 2015."

7. What about the newly added new-wifi-support.txt file? How do we add that to be tracked by Git?

   ```
   $ git add new-wifi-support.txt
   warning: LF will be replaced by CRLF in new-wifi-support.txt.
   The file will have its original line endings in your working directory.
   ```

8. Now, confirm that the new file has been added:

   ```
$ git status
On branch my-descriptive-branch-name
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)
   
        new file:   new-wifi-support.txt
   
   Changes not staged for commit:
     (use "git add <file>..." to update what will be committed)
     (use "git checkout -- <file>..." to discard changes in working directory)
   
        modified:   README.txt
   ```

9. Let's also stage the changes to README.txt for commit:

   ```
   $ git add README.txt
   ```
  
   and confirm that it's been added:
   
   ```
   $ git status
   On branch my-descriptive-branch-name
   Changes to be committed:
     (use "git reset HEAD <file>..." to unstage)
   
        modified:   README.txt
        new file:   new-wifi-support.txt
   
   ```

10. Now let's review all the changes before we make the commit:

   ```
   $ git diff --cached
   diff --git a/README.txt b/README.txt
   index 6ba3726..8b9eafc 100644
   --- a/README.txt
   +++ b/README.txt
   @@ -4,7 +4,7 @@ Source from the 3.2.153 Patch.
    - Added DirectX includs and lib into compile path so that the sdk does not need to be installed.
   
    Steps to compile
   - - Compile using Visual Studio 8.
   + - Compile using Visual Studio 2015.
      - Any path to the directory should work.
      - Open Solution RacerMateOne_Source/RacerMateOne.sln.  This has all the projects needed to build the current version.
   
   diff --git a/new-wifi-support.txt b/new-wifi-support.txt
   new file mode 100644
   index 0000000..ff096c0
   --- /dev/null
   +++ b/new-wifi-support.txt
   @@ -0,0 +1 @@
   +Yay! This is wifi support!
   warning: LF will be replaced by CRLF in new-wifi-support.txt.
   The file will have its original line endings in your working directory.
   
   ```

11. We're happy with those changes, so let's commit them!
   There is a shorthand way to do this, and a more interactive method that allows you to type a longer commit message. If you are using git-bash, then vim is probably setup as the default editor for git. If you're not accustomed to using vim, then I highly recommend the shorthand method:

   ```
   $ git commit -m "Updated documentation to show that we're adding wifi support."
   
   [my-descriptive-branch-name 8f91233] Updated documentation to show that we're adding wifi support.
   warning: LF will be replaced by CRLF in new-wifi-support.txt.
   The file will have its original line endings in your working directory.
    2 files changed, 2 insertions(+), 1 deletion(-)
    create mode 100644 new-wifi-support.txt
   ```

   The more interactive method is to simply run `$ git commit`, which will open an instance of vim so that you can enter the commit message, which can be multiple lines. Save the file in order to initiate the commit. If you leave the commit message empty, then it will abort the commit.

Switching Between Branches
--------------------------
Now that you have another branch with changes on it. Let's pretend you need to get back to the original `master` version of the code in order to do some testing - perhaps you need to confirm the previous behavior of the code. You can switch back to the original source using a simple command (assuming that all your current changes are committed):

```
$ git checkout master
```

Now you can go ahead and do whatever testing you need to (or even make a new branch and additional code changes!). In order to switch back to your wifi branch:

```
$ git checkout my-descriptive-branch-name
```

Viola! Back to editing the wifi code!

Pushing Your Branch up to GitHub
--------------------------------
Perhaps you want to have someone else test your changes, move them to another computer, or just want to make sure they are saved - then you'll need to push your new local branch to a new branch on the server. When you do this for a brand new branch, you have to specify some additional options so that Git will know what to name the remote branch on `origin` and so that it can setup tracking information for future `git pull` and `git push` operations.
* Since this accesses the remote repository, you will be prompted for your GitHub username and password.

```
$ git push -u origin 
Counting objects: 4, done.
Delta compression using up to 6 threads.
Compressing objects: 100% (3/3), done.
Writing objects: 100% (4/4), 429 bytes | 0 bytes/s, done.
Total 4 (delta 2), reused 0 (delta 0)
To https://github.com/RacerMateInc/RacerMateOne.git
 * [new branch]      my-descriptive-branch-name -> my-descriptive-branch-name
Branch my-descriptive-branch-name set up to track remote branch my-descriptive-branch-name from origin.
```

Congrats! You've now gotten access to the RacerMateOne source code from GitHub, created a new branch, made changes, and pushed those changes back up to the server so that other people can see the changes, and help with testing!

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace NGenCustomAction
{
	[RunInstaller(true)]
	public partial class NGenCustomAction :  System.Configuration.Install.Installer
	{
		public NGenCustomAction()
		{
			InitializeComponent();
		}
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Install(System.Collections.IDictionary savedState)
		{
			base.Install(savedState);
			Context.LogMessage(">>>> ngenCA: install");
			ngenCA(savedState, "install");
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);
			Context.LogMessage(">>>> ngenCA: commit");
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);
			Context.LogMessage(">>>> ngenCA: uninstall");
			ngenCA(savedState, "uninstall");
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public override void Rollback(System.Collections.IDictionary savedState)
		{
			base.Rollback(savedState);
			Context.LogMessage(">>>> ngenCA: rollback");
			ngenCA(savedState, "uninstall");
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        private void ngenCA(System.Collections.IDictionary savedState, string ngenCommand)
        {
            String[] argsArray;

            if (string.Compare(ngenCommand, "install", StringComparison.OrdinalIgnoreCase) == 0)
            {
                String args = Context.Parameters["Args"];
                if (String.IsNullOrEmpty(args))
                {
                    throw new InstallException("No arguments specified");
                }

                char[] separators = { ';' };
                argsArray = args.Split(separators);
                savedState.Add("NgenCAArgs", argsArray); //It is Ok to 'ngen uninstall' assemblies which were not installed
            }
            else
            {
                argsArray = (String[])savedState["NgenCAArgs"];
            }

            // Gets the path to the Framework directory.
            string fxPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

            for (int i = 0; i < argsArray.Length; ++i)
            {
                string arg = argsArray[i];
                // Quotes the argument, in case it has a space in it.
                arg = "\"" + arg + "\"";

                string command = ngenCommand + " " + arg;

                ProcessStartInfo si = new ProcessStartInfo(Path.Combine(fxPath, "ngen.exe"), command);
                si.WindowStyle = ProcessWindowStyle.Hidden;

                Process p;

                try
                {
                    Context.LogMessage(">>>>" + Path.Combine(fxPath, "ngen.exe ") + command);
                    p = Process.Start(si);
                    p.WaitForExit();
                }
                catch (Exception ex)
                {
                    throw new InstallException("Failed to ngen " + arg, ex);
                }
            }
        }
	}
}

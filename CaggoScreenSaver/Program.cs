using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace CaggoScreenSaver
{
    internal static class Program
    {
        private const string MutexName = "Global\\CaggoScreenSaver_SingleInstance";

        /// <summary>
        /// The main entry point for the screensaver.
        /// Handles standard Windows screensaver command-line switches:
        ///   /s           : Fullscreen mode on all displays
        ///   /p &lt;HWND&gt;    : Preview mode embedded in the Windows Screensaver Control Panel
        ///   /c [:HWND]   : Configuration / Settings dialog
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Length > 0)
            {
                string firstArg = args[0].ToLowerInvariant().Trim();
                string switchFlag = firstArg.Length >= 2 ? firstArg.Substring(0, 2) : firstArg;

                // Handle Preview Mode: /p <HWND> or /p:<HWND>
                if (switchFlag == "/p" || switchFlag == "-p")
                {
                    IntPtr previewHwnd = IntPtr.Zero;

                    if (firstArg.Length > 3 && firstArg.Contains(':'))
                    {
                        string hwndStr = firstArg.Split(':')[1];
                        if (long.TryParse(hwndStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out long hwndVal))
                        {
                            previewHwnd = new IntPtr(hwndVal);
                        }
                    }
                    else if (args.Length > 1)
                    {
                        if (long.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long hwndVal))
                        {
                            previewHwnd = new IntPtr(hwndVal);
                        }
                    }

                    if (previewHwnd != IntPtr.Zero)
                    {
                        Application.Run(new MainForm(previewHwnd));
                        return;
                    }
                }
                // Handle Configuration / Settings Mode: /c or /c:<HWND>
                else if (switchFlag == "/c" || switchFlag == "-c")
                {
                    MessageBox.Show(
                        "Caggo Screen Saver v1.0\n\n" +
                        "A procedural box-eye digital robot pet screensaver with organic expressions, " +
                        "neon bloom lighting, multi-monitor support, and OLED burn-in protection.\n\n" +
                        "Created with .NET 8 Windows Forms.",
                        "Caggo Screen Saver Settings",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            // Standard Fullscreen Mode (/s or default execution)
            using (Mutex mutex = new Mutex(true, MutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    // An instance of the screensaver is already active
                    return;
                }

                // Multi-monitor deployment: Spawn a screensaver form across every connected display
                Screen[] screens = Screen.AllScreens;
                if (screens.Length > 1)
                {
                    MainForm primaryForm = null!;
                    foreach (Screen screen in screens)
                    {
                        MainForm form = new MainForm(screen);
                        if (screen.Primary)
                        {
                            primaryForm = form;
                        }
                        else
                        {
                            form.Show();
                        }
                    }

                    Application.Run(primaryForm ?? new MainForm(screens[0]));
                }
                else
                {
                    Application.Run(new MainForm());
                }
            }
        }
    }
}


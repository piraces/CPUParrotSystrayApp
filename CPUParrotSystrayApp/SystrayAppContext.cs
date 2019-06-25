using System;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using System.Diagnostics;


namespace SystrayApp
{
    /// <summary>
    /// Custom Application Context for the sample Systray Application.
    /// </summary>
    public class SystrayAppContext : ApplicationContext
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SystrayAppContext));

        /// <summary>
        /// The NotifyIcon property is the entry point for the Systray application.
        /// </summary>
        private static NotifyIcon NotifyIcon { get; set; }

        /// <summary>
        /// Class Objects
        /// </summary>
        private readonly Timer Timer = new Timer();
        private readonly PerformanceCounter CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        /// <summary>
        /// Variables
        /// </summary>
        private static int currentIcon;

        /// <summary>
        /// Constants
        /// </summary>
        private readonly static int MAX_NUMBER_ICON = 9;
        private readonly static int MAX_CPU = 100;
        private readonly static int MAX_TIMER_LIMIT = 102;

        /// <summary>
        /// Default constructor, will instantiate and configure the NotifyIcon.
        /// </summary>
        public SystrayAppContext()
        {
            NotifyIcon = new NotifyIcon
            {
                Text = Properties.Resources.IconText,
                Icon = Properties.Resources.frame_00,
                ContextMenu = new ContextMenu(new []
                {
                    new MenuItem("Exit", ExitHandler)
                }),
                Visible = true
            };
            ConfigureTimer();
        }

        /// <summary>
        /// ExitHandler delegate.  Will handle exiting the Application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExitHandler(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            NotifyIcon.Visible = false;
            Dispose();
            Application.Exit();
        }

        /// <summary>
        /// Method to initialize the main Timer.
        /// </summary>
        private void ConfigureTimer()
        {
            Timer.Enabled = true;
            Timer.Interval = 50;
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        /// <summary>
        /// Handler for each Timer Tick. 
        /// Updates the icon, the interval frequency of the Timer and the text with the current CPU usage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentCPU = (double)CPUCounter.NextValue();
            currentCPU = Math.Round(currentCPU, 2);
            var interval = MAX_TIMER_LIMIT - currentCPU;


            currentIcon++;
            if (currentIcon > MAX_NUMBER_ICON)
                currentIcon = 0;
            NotifyIcon.Icon = (Icon)Properties.Resources.ResourceManager.GetObject(@"frame_0" + currentIcon);
            NotifyIcon.Text = "CPU Usage: " + currentCPU + "%";
            Timer.Interval = (int)interval;
        }

        /// <summary>
        /// Dispose used objects before exiting the app.
        /// </summary>
        public new void Dispose()
        {
            Timer.Dispose();
            CPUCounter.Dispose();
            base.Dispose();
        }
    }
}
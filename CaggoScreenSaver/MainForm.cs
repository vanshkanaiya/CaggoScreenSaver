using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CaggoScreenSaver.Animation;
using CaggoScreenSaver.Pet;

namespace CaggoScreenSaver
{
    /// <summary>
    /// The main screensaver window hosting our digital pet.
    /// Supports fullscreen multi-monitor deployment, Windows screensaver mini-preview embedding (/p),
    /// startup input grace period, and anti-burn-in roaming anchor rendering.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Win32 API Interop for Windows Preview Mode (/p)
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;
        #endregion

        // Core pet instance and procedural animator
        private readonly BoxPet _pet = new BoxPet();
        private readonly PetAnimator _animator;

        // High-frequency 60 FPS update loop timer and frame stopwatch
        private readonly System.Windows.Forms.Timer _animationTimer = new System.Windows.Forms.Timer();
        private readonly Stopwatch _frameStopwatch = new Stopwatch();

        // State flags
        private readonly bool _isPreviewMode = false;
        private readonly IntPtr _previewParentHwnd = IntPtr.Zero;
        private readonly DateTime _startTime = DateTime.UtcNow;

        // Tracks initial mouse coordinates to differentiate intentional movement from launch jitter
        private Point _initialMousePosition;
        private bool _hasInitialMousePosition = false;

        /// <summary>
        /// Default constructor for primary screen fullscreen mode.
        /// </summary>
        public MainForm() : this(Screen.PrimaryScreen ?? Screen.AllScreens[0])
        {
        }

        /// <summary>
        /// Constructor for targeted multi-monitor display.
        /// </summary>
        public MainForm(Screen targetScreen)
        {
            InitializeComponent();

            _animator = new PetAnimator(_pet);
            BackColor = Color.Black;
            DoubleBuffered = true;

            // Position borderless on the specific monitor
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = targetScreen.Bounds;
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Constructor for Windows Screensaver preview mode (/p <HWND>).
        /// Embeds this form inside the mini monitor preview control in Windows Settings.
        /// </summary>
        public MainForm(IntPtr previewParentHwnd)
        {
            InitializeComponent();

            _isPreviewMode = true;
            _previewParentHwnd = previewParentHwnd;
            _animator = new PetAnimator(_pet);

            BackColor = Color.Black;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;

            // Set as child window of the Windows Settings preview control
            SetParent(Handle, previewParentHwnd);
            SetWindowLong(Handle, GWL_STYLE, GetWindowLong(Handle, GWL_STYLE) | WS_CHILD);

            // Match bounds of the preview pane
            if (GetClientRect(previewParentHwnd, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                Bounds = new Rectangle(0, 0, width, height);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_isPreviewMode)
            {
                // Hide cursor only during full screensaver
                Cursor.Hide();
            }

            // Calculate eye sizes to fill appropriate screen/preview area
            _pet.UpdateDimensionsForScreen(ClientSize.Width, ClientSize.Height);

            // Setup and start 60 FPS animation loop (~16ms)
            _animationTimer.Interval = 16;
            _animationTimer.Tick += OnAnimationTick;
            _frameStopwatch.Start();
            _animationTimer.Start();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            float deltaTime = (float)_frameStopwatch.Elapsed.TotalSeconds;
            _frameStopwatch.Restart();
            deltaTime = Math.Min(deltaTime, 0.1f);

            // Advance procedural animations and drifting
            _animator.Update(deltaTime);

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_pet != null)
            {
                _pet.UpdateDimensionsForScreen(ClientSize.Width, ClientSize.Height);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate center of screen including anti-burn-in roaming drift
            Point screenCenter = new Point(
                ClientSize.Width / 2 + (int)_animator.AnchorOffset.X,
                ClientSize.Height / 2 + (int)_animator.AnchorOffset.Y
            );

            // Draw the box-shaped eyes and glowing bloom
            _pet.Draw(e.Graphics, screenCenter);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!_isPreviewMode)
            {
                ExitScreensaver();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_isPreviewMode)
            {
                ExitScreensaver();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isPreviewMode) return;

            // Startup grace period: ignore mouse movement during first 1.5 seconds
            if ((DateTime.UtcNow - _startTime).TotalSeconds < 1.5)
            {
                _initialMousePosition = Cursor.Position;
                _hasInitialMousePosition = true;
                return;
            }

            if (!_hasInitialMousePosition)
            {
                _initialMousePosition = Cursor.Position;
                _hasInitialMousePosition = true;
                return;
            }

            int deltaX = Math.Abs(Cursor.Position.X - _initialMousePosition.X);
            int deltaY = Math.Abs(Cursor.Position.Y - _initialMousePosition.Y);

            // If mouse moved noticeably beyond threshold (15px), dismiss screensaver
            if (deltaX > 15 || deltaY > 15)
            {
                ExitScreensaver();
            }
        }

        /// <summary>
        /// Safely restores cursor, stops timers, and exits all screensaver windows.
        /// </summary>
        private void ExitScreensaver()
        {
            _animationTimer.Stop();
            Cursor.Show();
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _animationTimer.Stop();
            _animationTimer.Dispose();
            if (!_isPreviewMode)
            {
                Cursor.Show();
            }
            base.OnFormClosing(e);
        }
    }
}


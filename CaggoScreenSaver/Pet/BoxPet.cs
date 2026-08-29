using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CaggoScreenSaver.Pet
{
    /// <summary>
    /// Personality expressions for the BoxPet.
    /// </summary>
    public enum PetExpression
    {
        Normal,     // Standard square/squircle box eyes
        Happy,      // Joyful ^ ^ arched box eyes with a subtle bounce
        Surprised,  // Wide enlarged square eyes
        Sleepy,     // Half-closed droopy eyes with relaxed blinks
        Mean        // Inward-slanted fierce / grumpy glare
    }

    /// <summary>
    /// Represents the Box-Eye Digital Pet.
    /// Manages visual styling including squircle rounded corners, neon bloom glow, 
    /// squash & stretch deformation, and OLED anti-burn-in dimming.
    /// </summary>
    public class BoxPet
    {
        // Visual dimensions of each eye (in pixels) when fully open.
        public int EyeWidth { get; set; } = 400;
        public int EyeHeight { get; set; } = 400;

        // Gap / spacing between the two square eyes (in pixels)
        public int EyeSpacing { get; set; } = 150;

        // Vivid neon electric blue core color (#00F0FF)
        public Color EyeColor { get; set; } = Color.FromArgb(0x00, 0xF0, 0xFF);

        // Corner radius for modern rounded box ("squircle") eyes
        public int CornerRadius { get; set; } = 40;

        // Anti-burn-in OLED brightness factor (1.0 = full vibrant glow, 0.6 = deep sleep dim)
        public float Brightness { get; set; } = 1.0f;

        // Neon outer bloom / glow toggle and intensity
        public bool EnableGlow { get; set; } = true;

        /// <summary>
        /// Controls how open the eyes are:
        /// 1.0f = fully open square
        /// 0.0f = fully closed (represented as a sleek horizontal slit)
        /// > 1.0f = wide open (surprised)
        /// </summary>
        public float OpenAmount { get; set; } = 1.0f;

        /// <summary>
        /// Uniform scale multiplier (e.g. 1.15 for surprised enlargement).
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Organic Squash and Stretch deformation factors (1.0 = normal).
        /// </summary>
        public float SquashX { get; set; } = 1.0f;
        public float StretchY { get; set; } = 1.0f;

        /// <summary>
        /// Current emotional expression.
        /// </summary>
        public PetExpression Expression { get; set; } = PetExpression.Normal;

        /// <summary>
        /// Dynamic positional offset in pixels when looking around.
        /// </summary>
        public PointF LookOffset { get; set; } = PointF.Empty;

        /// <summary>
        /// Additional vertical bounce offset in pixels (for happy / surprised emotions).
        /// </summary>
        public float BounceOffsetY { get; set; } = 0f;

        // Dynamic Eyebrow properties
        public bool ShowEyebrows { get; set; } = true;
        public float LeftBrowAngle { get; set; } = 0f;
        public float RightBrowAngle { get; set; } = 0f;
        public float LeftBrowOffsetY { get; set; } = 0f;
        public float RightBrowOffsetY { get; set; } = 0f;

        /// <summary>
        /// Recalculates eye dimensions proportionally based on the screen or preview size.
        /// </summary>
        public void UpdateDimensionsForScreen(int screenWidth, int screenHeight)
        {
            if (screenWidth <= 0 || screenHeight <= 0) return;

            // Height takes ~40% of screen height
            int squareSize = Math.Max(24, (int)(screenHeight * 0.40));

            EyeWidth = squareSize;
            EyeHeight = squareSize;

            // Spacing between the two squares is ~35% of eye size
            EyeSpacing = Math.Max(8, (int)(squareSize * 0.35));

            // Proportional corner radius (~18% of eye size for a sleek squircle)
            CornerRadius = Math.Max(4, (int)(squareSize * 0.18));
        }

        /// <summary>
        /// Calculates the current rendered height taking blink, squash/stretch, and scale into account.
        /// </summary>
        private int GetCurrentRenderHeight()
        {
            int minSlitHeight = Math.Max(4, (int)(EyeHeight * 0.025f));
            int scaledHeight = (int)(EyeHeight * Scale * StretchY);
            int dynamicHeight = (int)(scaledHeight * OpenAmount);
            return Math.Max(minSlitHeight, dynamicHeight);
        }

        private int GetCurrentRenderWidth()
        {
            return Math.Max(6, (int)(EyeWidth * Scale * SquashX));
        }

        /// <summary>
        /// Calculates the bounding rectangle for the left eye based on center anchor and offsets.
        /// </summary>
        public Rectangle GetLeftEyeBounds(Point anchor)
        {
            int currentWidth = GetCurrentRenderWidth();
            int currentHeight = GetCurrentRenderHeight();

            int x = anchor.X - (EyeSpacing / 2) - currentWidth + (int)LookOffset.X;
            int y = anchor.Y - (currentHeight / 2) + (int)LookOffset.Y + (int)BounceOffsetY;
            return new Rectangle(x, y, currentWidth, currentHeight);
        }

        /// <summary>
        /// Calculates the bounding rectangle for the right eye based on center anchor and offsets.
        /// </summary>
        public Rectangle GetRightEyeBounds(Point anchor)
        {
            int currentWidth = GetCurrentRenderWidth();
            int currentHeight = GetCurrentRenderHeight();

            int x = anchor.X + (EyeSpacing / 2) + (int)LookOffset.X;
            int y = anchor.Y - (currentHeight / 2) + (int)LookOffset.Y + (int)BounceOffsetY;
            return new Rectangle(x, y, currentWidth, currentHeight);
        }

        /// <summary>
        /// Gets the effective eye color taking the current brightness / dimming level into account.
        /// </summary>
        public Color GetEffectiveEyeColor()
        {
            int r = Math.Clamp((int)(EyeColor.R * Brightness), 0, 255);
            int g = Math.Clamp((int)(EyeColor.G * Brightness), 0, 255);
            int b = Math.Clamp((int)(EyeColor.B * Brightness), 0, 255);
            return Color.FromArgb(EyeColor.A, r, g, b);
        }

        /// <summary>
        /// Renders the box-shaped eyes, multi-pass neon bloom glow, and expressive eyebrows.
        /// </summary>
        public void Draw(Graphics g, Point anchor)
        {
            Color effectiveColor = GetEffectiveEyeColor();
            Rectangle leftEye = GetLeftEyeBounds(anchor);
            Rectangle rightEye = GetRightEyeBounds(anchor);

            // 1. Render multi-layer Neon Bloom Glow passes
            if (EnableGlow && Brightness > 0.1f)
            {
                DrawBloomPasses(g, effectiveColor, leftEye, rightEye);
            }

            // 2. Render Core Solid Eyes
            using (SolidBrush coreBrush = new SolidBrush(effectiveColor))
            {
                if (Expression == PetExpression.Happy && OpenAmount > 0.3f)
                {
                    DrawHappyEye(g, coreBrush, leftEye);
                    DrawHappyEye(g, coreBrush, rightEye);
                }
                else if (Expression == PetExpression.Mean && OpenAmount > 0.3f)
                {
                    DrawMeanEye(g, coreBrush, leftEye, isLeftEye: true);
                    DrawMeanEye(g, coreBrush, rightEye, isLeftEye: false);
                }
                else
                {
                    DrawRoundedEye(g, coreBrush, leftEye);
                    DrawRoundedEye(g, coreBrush, rightEye);
                }

                // 3. Render Eyebrows
                if (ShowEyebrows)
                {
                    DrawEyebrows(g, coreBrush, leftEye, rightEye, effectiveColor);
                }
            }
        }

        /// <summary>
        /// Draws layered semi-transparent neon glow strokes around the eyes for an authentic OLED screen aura.
        /// </summary>
        private void DrawBloomPasses(Graphics g, Color baseColor, Rectangle leftEye, Rectangle rightEye)
        {
            // Glow layers from outermost soft haze to tighter inner halo
            int[] glowPaddings = { 24, 16, 8, 3 };
            int[] glowAlphas = { 18, 35, 65, 110 };

            for (int i = 0; i < glowPaddings.Length; i++)
            {
                int pad = (int)(glowPaddings[i] * Scale * (EyeWidth / 400.0f));
                if (pad < 1) pad = 1;

                int alpha = (int)(glowAlphas[i] * Brightness);
                if (alpha <= 0) continue;

                Color glowColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

                using (Pen glowPen = new Pen(glowColor, pad * 1.5f))
                {
                    glowPen.LineJoin = LineJoin.Round;

                    if (Expression == PetExpression.Happy && OpenAmount > 0.3f)
                    {
                        using (GraphicsPath pathL = CreateHappyEyePath(leftEye))
                        using (GraphicsPath pathR = CreateHappyEyePath(rightEye))
                        {
                            g.DrawPath(glowPen, pathL);
                            g.DrawPath(glowPen, pathR);
                        }
                    }
                    else if (Expression == PetExpression.Mean && OpenAmount > 0.3f)
                    {
                        using (GraphicsPath pathL = CreateMeanEyePath(leftEye, isLeftEye: true))
                        using (GraphicsPath pathR = CreateMeanEyePath(rightEye, isLeftEye: false))
                        {
                            g.DrawPath(glowPen, pathL);
                            g.DrawPath(glowPen, pathR);
                        }
                    }
                    else
                    {
                        using (GraphicsPath pathL = CreateRoundedRectanglePath(leftEye, CornerRadius))
                        using (GraphicsPath pathR = CreateRoundedRectanglePath(rightEye, CornerRadius))
                        {
                            g.DrawPath(glowPen, pathL);
                            g.DrawPath(glowPen, pathR);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a single rounded rectangle ("squircle") eye.
        /// </summary>
        private void DrawRoundedEye(Graphics g, Brush brush, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int dynamicRadius = Math.Min(CornerRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            if (dynamicRadius <= 2)
            {
                g.FillRectangle(brush, bounds);
                return;
            }

            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, dynamicRadius))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws expressive, sleek rounded neon rectangular eyebrows above each eye.
        /// </summary>
        private void DrawEyebrows(Graphics g, Brush coreBrush, Rectangle leftEye, Rectangle rightEye, Color baseColor)
        {
            int browWidth = (int)(EyeWidth * Scale * SquashX * 0.95f);
            int browHeight = Math.Max(6, (int)(EyeHeight * Scale * 0.085f));
            int browGap = (int)(EyeHeight * Scale * 0.12f);
            int browRadius = Math.Max(2, browHeight / 2);

            // Left eyebrow center
            Point leftBrowCenter = new Point(
                leftEye.Left + leftEye.Width / 2,
                leftEye.Top - browGap - browHeight / 2 + (int)LeftBrowOffsetY
            );

            // Right eyebrow center
            Point rightBrowCenter = new Point(
                rightEye.Left + rightEye.Width / 2,
                rightEye.Top - browGap - browHeight / 2 + (int)RightBrowOffsetY
            );

            Rectangle browRect = new Rectangle(-browWidth / 2, -browHeight / 2, browWidth, browHeight);

            // Render Left Brow
            GraphicsState leftState = g.Save();
            g.TranslateTransform(leftBrowCenter.X, leftBrowCenter.Y);
            g.RotateTransform(LeftBrowAngle);

            if (EnableGlow && Brightness > 0.1f)
            {
                using (Pen glowPen = new Pen(Color.FromArgb((int)(40 * Brightness), baseColor.R, baseColor.G, baseColor.B), 6))
                using (GraphicsPath browPath = CreateRoundedRectanglePath(browRect, browRadius))
                {
                    glowPen.LineJoin = LineJoin.Round;
                    g.DrawPath(glowPen, browPath);
                }
            }

            using (GraphicsPath browPath = CreateRoundedRectanglePath(browRect, browRadius))
            {
                g.FillPath(coreBrush, browPath);
            }
            g.Restore(leftState);

            // Render Right Brow
            GraphicsState rightState = g.Save();
            g.TranslateTransform(rightBrowCenter.X, rightBrowCenter.Y);
            g.RotateTransform(RightBrowAngle);

            if (EnableGlow && Brightness > 0.1f)
            {
                using (Pen glowPen = new Pen(Color.FromArgb((int)(40 * Brightness), baseColor.R, baseColor.G, baseColor.B), 6))
                using (GraphicsPath browPath = CreateRoundedRectanglePath(browRect, browRadius))
                {
                    glowPen.LineJoin = LineJoin.Round;
                    g.DrawPath(glowPen, browPath);
                }
            }

            using (GraphicsPath browPath = CreateRoundedRectanglePath(browRect, browRadius))
            {
                g.FillPath(coreBrush, browPath);
            }
            g.Restore(rightState);
        }

        /// <summary>
        /// Draws a joyful arched squircle eye with an inverted bottom wedge cutout (^).
        /// </summary>
        private void DrawHappyEye(Graphics g, Brush brush, Rectangle bounds)
        {
            using (GraphicsPath path = CreateHappyEyePath(bounds))
            {
                g.FillPath(brush, path);
            }
        }

        private GraphicsPath CreateHappyEyePath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int bottomCutY = bounds.Bottom - (int)(bounds.Height * 0.55f);

            Point[] points = new Point[]
            {
                new Point(bounds.Left, bounds.Top),
                new Point(bounds.Right, bounds.Top),
                new Point(bounds.Right, bounds.Bottom),
                new Point(bounds.Left + bounds.Width / 2, bottomCutY),
                new Point(bounds.Left, bounds.Bottom)
            };

            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Draws a sharp, mean / grumpy eye with an inward-slanted upper eyelid.
        /// </summary>
        private void DrawMeanEye(Graphics g, Brush brush, Rectangle bounds, bool isLeftEye)
        {
            using (GraphicsPath path = CreateMeanEyePath(bounds, isLeftEye))
            {
                g.FillPath(brush, path);
            }
        }

        private GraphicsPath CreateMeanEyePath(Rectangle bounds, bool isLeftEye)
        {
            GraphicsPath path = new GraphicsPath();
            int slantDrop = (int)(bounds.Height * 0.28f);

            Point[] points;
            if (isLeftEye)
            {
                // Left eye: inner edge (right side) slopes downward
                points = new Point[]
                {
                    new Point(bounds.Left, bounds.Top),
                    new Point(bounds.Right, bounds.Top + slantDrop),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bounds.Left, bounds.Bottom)
                };
            }
            else
            {
                // Right eye: inner edge (left side) slopes downward
                points = new Point[]
                {
                    new Point(bounds.Left, bounds.Top + slantDrop),
                    new Point(bounds.Right, bounds.Top),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bounds.Left, bounds.Bottom)
                };
            }

            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Helper to generate smooth rounded rectangle ("squircle") paths.
        /// </summary>
        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Top-left arc
            path.AddArc(arc, 180, 90);

            // Top-right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}


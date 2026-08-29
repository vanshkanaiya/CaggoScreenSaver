using System;
using System.Drawing;
using CaggoScreenSaver.Pet;

namespace CaggoScreenSaver.Animation
{
    public enum BlinkState
    {
        Idle,
        Closing,
        Opening
    }

    public enum GazeState
    {
        Centered,
        Looking
    }

    public enum MoodState
    {
        Normal,
        Happy,
        Surprised,
        Sleepy,
        Mean
    }

    /// <summary>
    /// Manages procedural animation, personality expressions, eyebrows, blinking, gaze,
    /// anti-burn-in roaming drift, OLED sleep dimming, and squash/stretch dynamics for BoxPet.
    /// </summary>
    public class PetAnimator
    {
        private readonly BoxPet _pet;
        private readonly Random _random = new Random();

        // Total running time for long-term power saving / anti-burn-in dimming
        private float _totalRuntime = 0f;

        // Anti-burn-in screen roaming anchor
        private PointF _currentAnchorOffset = PointF.Empty;
        private PointF _targetAnchorOffset = PointF.Empty;
        private float _roamTimer = 0f;
        private float _nextRoamInterval = 25.0f;

        // OLED Dimming state
        private float _targetBrightness = 1.0f;
        private float _currentBrightness = 1.0f;

        // Squash & Stretch dynamic physics
        private float _currentSquashX = 1.0f;
        private float _currentStretchY = 1.0f;
        private float _targetSquashX = 1.0f;
        private float _targetStretchY = 1.0f;

        // Blinking system
        private BlinkState _blinkState = BlinkState.Idle;
        private float _blinkTimer = 0f;
        private float _currentBlinkInterval = 3.0f;
        private float _closingDuration = 0.07f;
        private float _openingDuration = 0.09f;
        private bool _isSlowBlink = false;

        // Gaze / Looking system
        private GazeState _gazeState = GazeState.Centered;
        private float _gazeTimer = 0f;
        private float _currentGazeDuration = 3.5f;
        private float _currentLookX = 0f;
        private float _currentLookY = 0f;
        private float _targetLookX = 0f;
        private float _targetLookY = 0f;

        // Personality / Mood system
        private MoodState _currentMood = MoodState.Normal;
        private float _moodTimer = 0f;
        private float _currentMoodDuration = 10.0f;

        // Dynamic scale and bounce animation
        private float _currentScale = 1.0f;
        private float _targetScale = 1.0f;
        private float _bouncePhase = 0f;
        private bool _isBouncing = false;

        // Eyebrow animation targets & currents
        private float _currentLeftBrowAngle = 0f;
        private float _currentRightBrowAngle = 0f;
        private float _targetLeftBrowAngle = 0f;
        private float _targetRightBrowAngle = 0f;

        private float _currentLeftBrowOffsetY = 0f;
        private float _currentRightBrowOffsetY = 0f;
        private float _targetLeftBrowOffsetY = 0f;
        private float _targetRightBrowOffsetY = 0f;

        /// <summary>
        /// Gets the current roaming anchor offset to shift the pet's center around the screen.
        /// </summary>
        public PointF AnchorOffset => _currentAnchorOffset;

        public PetAnimator(BoxPet pet)
        {
            _pet = pet ?? throw new ArgumentNullException(nameof(pet));
            _currentBlinkInterval = GetNextRandomBlinkInterval();
            _currentGazeDuration = GetNextRandomCenterDuration();
            _currentMoodDuration = (float)(18.0 + _random.NextDouble() * 17.0);
            _nextRoamInterval = (float)(20.0 + _random.NextDouble() * 20.0);
        }

        /// <summary>
        /// Updates the pet's procedural animation, roaming drift, and personality every frame.
        /// </summary>
        public void Update(float deltaTime)
        {
            _totalRuntime += deltaTime;

            UpdateMood(deltaTime);
            UpdateBlinking(deltaTime);
            UpdateLooking(deltaTime);
            UpdateRoaming(deltaTime);
            UpdateDimming(deltaTime);

            InterpolateGaze(deltaTime);
            InterpolateEyebrows(deltaTime);
            InterpolateScaleAndBounce(deltaTime);
            InterpolateSquashStretch(deltaTime);
        }

        /// <summary>
        /// Periodically shifts the anchor position slightly across screen zones to prevent OLED burn-in.
        /// </summary>
        private void UpdateRoaming(float deltaTime)
        {
            _roamTimer += deltaTime;

            if (_roamTimer >= _nextRoamInterval)
            {
                _roamTimer = 0f;
                _nextRoamInterval = (float)(25.0 + _random.NextDouble() * 35.0); // Roam every 25-60s

                // Shift anchor within safe margins (up to 12% width and 8% height)
                float maxShiftX = _pet.EyeWidth * 0.45f;
                float maxShiftY = _pet.EyeHeight * 0.25f;

                float offsetX = (float)((_random.NextDouble() * 2.0 - 1.0) * maxShiftX);
                float offsetY = (float)((_random.NextDouble() * 2.0 - 1.0) * maxShiftY);

                _targetAnchorOffset = new PointF(offsetX, offsetY);
            }

            // Smooth gentle drifting interpolation
            float roamSpeed = 0.8f;
            float t = Math.Clamp(deltaTime * roamSpeed, 0f, 1f);
            float newX = _currentAnchorOffset.X + (_targetAnchorOffset.X - _currentAnchorOffset.X) * t;
            float newY = _currentAnchorOffset.Y + (_targetAnchorOffset.Y - _currentAnchorOffset.Y) * t;
            _currentAnchorOffset = new PointF(newX, newY);
        }

        /// <summary>
        /// Smoothly dims brightness over extended idle runtime or deep sleep to protect displays.
        /// </summary>
        private void UpdateDimming(float deltaTime)
        {
            if (_totalRuntime > 300.0f || _currentMood == MoodState.Sleepy) // After 5 mins or in sleepy mode
            {
                _targetBrightness = 0.72f;
            }
            else
            {
                _targetBrightness = 1.0f;
            }

            float dimSpeed = 1.5f;
            float t = Math.Clamp(deltaTime * dimSpeed, 0f, 1f);
            _currentBrightness += (_targetBrightness - _currentBrightness) * t;
            _pet.Brightness = _currentBrightness;
        }

        /// <summary>
        /// Manages random personality transitions (Happy, Mean, Surprised, Sleepy, Normal).
        /// </summary>
        private void UpdateMood(float deltaTime)
        {
            _moodTimer += deltaTime;

            if (_moodTimer >= _currentMoodDuration)
            {
                _moodTimer = 0f;

                if (_currentMood != MoodState.Normal)
                {
                    // Return back to Normal mood
                    _currentMood = MoodState.Normal;
                    _pet.Expression = PetExpression.Normal;
                    _targetScale = 1.0f;
                    _closingDuration = 0.07f;
                    _openingDuration = 0.09f;
                    _isBouncing = false;
                    _currentMoodDuration = (float)(18.0 + _random.NextDouble() * 17.0);

                    // Reset eyebrow targets to calm horizontal
                    _targetLeftBrowAngle = 0f;
                    _targetRightBrowAngle = 0f;
                    _targetLeftBrowOffsetY = 0f;
                    _targetRightBrowOffsetY = 0f;
                }
                else
                {
                    // Choose a new random personality event
                    int moodRoll = _random.Next(0, 100);

                    if (moodRoll < 25) // 25% chance Happy
                    {
                        _currentMood = MoodState.Happy;
                        _pet.Expression = PetExpression.Happy;
                        _currentMoodDuration = (float)(3.0 + _random.NextDouble() * 1.5);
                        _isBouncing = true;
                        _bouncePhase = 0f;

                        _targetLeftBrowAngle = -10f;
                        _targetRightBrowAngle = 10f;
                        _targetLeftBrowOffsetY = -14f;
                        _targetRightBrowOffsetY = -14f;
                    }
                    else if (moodRoll < 45) // 20% chance Mean / Grumpy
                    {
                        _currentMood = MoodState.Mean;
                        _pet.Expression = PetExpression.Mean;
                        _currentMoodDuration = (float)(3.2 + _random.NextDouble() * 2.0);

                        _targetLeftBrowAngle = 14f;
                        _targetRightBrowAngle = -14f;
                        _targetLeftBrowOffsetY = 16f;
                        _targetRightBrowOffsetY = 16f;
                    }
                    else if (moodRoll < 65) // 20% chance Surprised
                    {
                        _currentMood = MoodState.Surprised;
                        _pet.Expression = PetExpression.Surprised;
                        _targetScale = 1.14f;
                        _currentMoodDuration = (float)(2.0 + _random.NextDouble() * 1.0);
                        _pet.BounceOffsetY = -18f;

                        // Squash & stretch pop in shock
                        _targetSquashX = 0.92f;
                        _targetStretchY = 1.12f;

                        _targetLeftBrowAngle = 0f;
                        _targetRightBrowAngle = 0f;
                        _targetLeftBrowOffsetY = -32f;
                        _targetRightBrowOffsetY = -32f;
                    }
                    else if (moodRoll < 85) // 20% chance Sleepy
                    {
                        _currentMood = MoodState.Sleepy;
                        _pet.Expression = PetExpression.Sleepy;
                        _currentMoodDuration = (float)(8.0 + _random.NextDouble() * 6.0);
                        _closingDuration = 0.22f;
                        _openingDuration = 0.25f;

                        _targetLeftBrowAngle = 8f;
                        _targetRightBrowAngle = -8f;
                        _targetLeftBrowOffsetY = 10f;
                        _targetRightBrowOffsetY = 10f;
                    }
                    else // 15% chance deliberate Slow Blink
                    {
                        _isSlowBlink = true;
                        _closingDuration = 0.20f;
                        _openingDuration = 0.25f;
                        _blinkTimer = _currentBlinkInterval;
                        _currentMoodDuration = (float)(12.0 + _random.NextDouble() * 10.0);
                    }
                }
            }
        }

        /// <summary>
        /// Blinking cycle handling open amounts and squash deformation.
        /// </summary>
        private void UpdateBlinking(float deltaTime)
        {
            _blinkTimer += deltaTime;

            float restingOpen = _currentMood switch
            {
                MoodState.Sleepy => 0.38f,
                MoodState.Mean => 0.70f,
                MoodState.Happy => 0.75f,
                MoodState.Surprised => 1.15f,
                _ => 1.0f
            };

            switch (_blinkState)
            {
                case BlinkState.Idle:
                    _pet.OpenAmount = restingOpen;
                    _targetSquashX = 1.0f;
                    _targetStretchY = 1.0f;

                    if (_blinkTimer >= _currentBlinkInterval)
                    {
                        _blinkState = BlinkState.Closing;
                        _blinkTimer = 0f;
                    }
                    break;

                case BlinkState.Closing:
                    float closeProgress = Math.Clamp(_blinkTimer / _closingDuration, 0f, 1f);
                    _pet.OpenAmount = restingOpen * (1.0f - closeProgress);

                    // Subtle horizontal squash when closing eyes
                    _targetSquashX = 1.05f;
                    _targetStretchY = 0.95f;

                    if (_blinkTimer >= _closingDuration)
                    {
                        _pet.OpenAmount = 0.0f;
                        _blinkState = BlinkState.Opening;
                        _blinkTimer = 0f;
                    }
                    break;

                case BlinkState.Opening:
                    float openProgress = Math.Clamp(_blinkTimer / _openingDuration, 0f, 1f);
                    _pet.OpenAmount = restingOpen * openProgress;

                    if (_blinkTimer >= _openingDuration)
                    {
                        _pet.OpenAmount = restingOpen;
                        _blinkState = BlinkState.Idle;
                        _blinkTimer = 0f;

                        if (_isSlowBlink)
                        {
                            _isSlowBlink = false;
                            _closingDuration = 0.07f;
                            _openingDuration = 0.09f;
                        }

                        _currentBlinkInterval = GetNextRandomBlinkInterval();
                    }
                    break;
            }
        }

        /// <summary>
        /// Gaze direction scheduler for looking around.
        /// </summary>
        private void UpdateLooking(float deltaTime)
        {
            _gazeTimer += deltaTime;

            if (_gazeState == GazeState.Centered)
            {
                if (_gazeTimer >= _currentGazeDuration)
                {
                    ChooseRandomLookDirection();
                    _gazeState = GazeState.Looking;
                    _gazeTimer = 0f;
                    _currentGazeDuration = (float)(2.0 + _random.NextDouble() * 2.5);
                }
            }
            else if (_gazeState == GazeState.Looking)
            {
                if (_gazeTimer >= _currentGazeDuration)
                {
                    _targetLookX = 0f;
                    _targetLookY = 0f;
                    _targetSquashX = 1.0f;
                    _targetStretchY = 1.0f;
                    _gazeState = GazeState.Centered;
                    _gazeTimer = 0f;
                    _currentGazeDuration = GetNextRandomCenterDuration();

                    if (_currentMood == MoodState.Normal)
                    {
                        _targetLeftBrowAngle = 0f;
                        _targetRightBrowAngle = 0f;
                        _targetLeftBrowOffsetY = 0f;
                        _targetRightBrowOffsetY = 0f;
                    }
                }
            }
        }

        private void ChooseRandomLookDirection()
        {
            float maxLookX = _pet.EyeWidth * 0.24f;
            float maxLookY = _pet.EyeHeight * 0.14f;

            int choice = _random.Next(0, 6);
            switch (choice)
            {
                case 0: // Look Left
                    _targetLookX = -maxLookX;
                    _targetLookY = 0f;
                    _targetSquashX = 0.96f;
                    _targetStretchY = 1.03f;
                    if (_currentMood == MoodState.Normal)
                    {
                        _targetLeftBrowAngle = -5f;
                        _targetRightBrowAngle = 4f;
                    }
                    break;

                case 1: // Look Right
                    _targetLookX = maxLookX;
                    _targetLookY = 0f;
                    _targetSquashX = 0.96f;
                    _targetStretchY = 1.03f;
                    if (_currentMood == MoodState.Normal)
                    {
                        _targetLeftBrowAngle = -4f;
                        _targetRightBrowAngle = 5f;
                    }
                    break;

                case 2: // Look Up
                    _targetLookX = 0f;
                    _targetLookY = -maxLookY;
                    _targetSquashX = 1.04f;
                    _targetStretchY = 0.97f;
                    if (_currentMood == MoodState.Normal)
                    {
                        _targetLeftBrowOffsetY = -10f;
                        _targetRightBrowOffsetY = -10f;
                    }
                    break;

                case 3: // Look Down
                    _targetLookX = 0f;
                    _targetLookY = maxLookY;
                    _targetSquashX = 1.04f;
                    _targetStretchY = 0.97f;
                    if (_currentMood == MoodState.Normal)
                    {
                        _targetLeftBrowOffsetY = 6f;
                        _targetRightBrowOffsetY = 6f;
                    }
                    break;

                case 4: // Look Up-Left
                    _targetLookX = -maxLookX * 0.75f;
                    _targetLookY = -maxLookY * 0.75f;
                    break;

                case 5: // Look Up-Right
                    _targetLookX = maxLookX * 0.75f;
                    _targetLookY = -maxLookY * 0.75f;
                    break;
            }
        }

        private void InterpolateGaze(float deltaTime)
        {
            float speed = (_currentMood == MoodState.Sleepy) ? 5.0f : 11.0f;
            float t = Math.Clamp(deltaTime * speed, 0f, 1f);

            _currentLookX += (_targetLookX - _currentLookX) * t;
            _currentLookY += (_targetLookY - _currentLookY) * t;

            _pet.LookOffset = new PointF(_currentLookX, _currentLookY);
        }

        private void InterpolateSquashStretch(float deltaTime)
        {
            float speed = 12.0f;
            float t = Math.Clamp(deltaTime * speed, 0f, 1f);

            _currentSquashX += (_targetSquashX - _currentSquashX) * t;
            _currentStretchY += (_targetStretchY - _currentStretchY) * t;

            _pet.SquashX = _currentSquashX;
            _pet.StretchY = _currentStretchY;
        }

        /// <summary>
        /// Smoothly interpolates eyebrow angles and offsets towards target expressions.
        /// </summary>
        private void InterpolateEyebrows(float deltaTime)
        {
            const float BrowSpeed = 10.0f;
            float t = Math.Clamp(deltaTime * BrowSpeed, 0f, 1f);

            _currentLeftBrowAngle += (_targetLeftBrowAngle - _currentLeftBrowAngle) * t;
            _currentRightBrowAngle += (_targetRightBrowAngle - _currentRightBrowAngle) * t;

            _currentLeftBrowOffsetY += (_targetLeftBrowOffsetY - _currentLeftBrowOffsetY) * t;
            _currentRightBrowOffsetY += (_targetRightBrowOffsetY - _currentRightBrowOffsetY) * t;

            _pet.LeftBrowAngle = _currentLeftBrowAngle;
            _pet.RightBrowAngle = _currentRightBrowAngle;
            _pet.LeftBrowOffsetY = _currentLeftBrowOffsetY;
            _pet.RightBrowOffsetY = _currentRightBrowOffsetY;
        }

        private void InterpolateScaleAndBounce(float deltaTime)
        {
            float scaleT = Math.Clamp(deltaTime * 8.0f, 0f, 1f);
            _currentScale += (_targetScale - _currentScale) * scaleT;
            _pet.Scale = _currentScale;

            if (_isBouncing)
            {
                _bouncePhase += deltaTime * 8.0f;
                float sinVal = (float)Math.Sin(_bouncePhase);
                _pet.BounceOffsetY = -Math.Abs(sinVal) * 22.0f;

                // Organic squash on landing, stretch in air
                _targetSquashX = 1.0f + (sinVal > 0 ? 0.06f : -0.04f);
                _targetStretchY = 1.0f - (sinVal > 0 ? 0.06f : -0.04f);
            }
            else if (_currentMood == MoodState.Surprised)
            {
                float bounceT = Math.Clamp(deltaTime * 6.0f, 0f, 1f);
                _pet.BounceOffsetY += (0f - _pet.BounceOffsetY) * bounceT;
            }
            else
            {
                _pet.BounceOffsetY = 0f;
            }
        }

        private float GetNextRandomBlinkInterval()
        {
            if (_currentMood == MoodState.Sleepy)
            {
                return (float)(2.5 + _random.NextDouble() * 3.5);
            }

            if (_random.NextDouble() < 0.15)
            {
                return (float)(0.2 + _random.NextDouble() * 0.3);
            }

            return (float)(3.5 + _random.NextDouble() * 4.0);
        }

        private float GetNextRandomCenterDuration()
        {
            return (float)(4.0 + _random.NextDouble() * 5.0);
        }
    }
}

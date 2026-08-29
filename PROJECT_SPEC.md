# Build a Minimalist Box-Eye Digital Pet for Windows 11

I want you to help me build a personal side project: a minimalist animated digital pet that eventually works as a Windows 11 screensaver.

I will use Visual Studio to open, run, debug, and inspect the project.

I am familiar with the general concepts of Unity but I am not an experienced C#/.NET developer. Therefore, I want the implementation to be simple and understandable.

## 1. Technology

Use:

- C#
- Modern .NET
- Windows Forms initially
- Visual Studio
- Standard .NET/Windows APIs wherever practical
- Minimal external dependencies

Do not use Unity.

Do not add libraries/frameworks unless they provide a genuine advantage.

The final application should eventually be capable of being used as a Windows `.scr` screensaver.

---

## 2. Main Concept

The application is a tiny futuristic digital creature consisting primarily of two large box-shaped blue eyes.

The screen should initially look approximately like:

```text
┌────────────────────────────────────────────┐
│                                            │
│                                            │
│             ██████      ██████             │
│             ██████      ██████             │
│             ██████      ██████             │
│                                            │
│                                            │
│                                            │
│                    03:52                   │
│                                            │
└────────────────────────────────────────────┘
```

The actual application should be fullscreen, so the border shown above is only a conceptual representation.

The creature should feel alive through subtle animation while maintaining a very minimalist appearance.

---

## 3. Visual Design

### Background

Use a pure black background:

`#000000`

Do not initially use:

- gradients
- textures
- images
- unnecessary UI
- menus
- borders
- decorations

Keep the screen extremely clean.

### Eyes

There should initially be exactly **two large box-shaped eyes**.

Eye color:

`#00A8FF`

or another vivid electric-blue/cyan-blue close to it.

The eyes should:

- be geometric rectangles rather than emoji/text characters
- be symmetrical when looking forward
- have adjustable size
- have adjustable spacing
- be centered appropriately
- remain the primary visual focus

A subtle blue glow may be added later, but do **not** complicate the first version just for a glow effect.

The eyes should NOT have a cartoon body yet.

The creature is literally just the eyes.

---

## 4. Animation

The eyes should eventually feel alive.

Implement animation gradually.

Possible behaviors:

### Normal

```text
██████        ██████
██████        ██████
██████        ██████
```

### Blink

```text
────────      ────────
```

### Looking left

The pupils/eye shapes should shift appropriately toward the left.

### Looking right

The eyes should shift appropriately toward the right.

### Looking up/down

Support subtle vertical movement if practical.

The animation should NOT constantly move.

It should feel like an idle digital creature.

For example:

```text
Idle
 ↓
wait random amount of time
 ↓
look left/right
 ↓
return to normal
 ↓
wait
 ↓
blink
 ↓
wait
 ↓
repeat
```

Use random timing so the behavior doesn't look robotic.

Do not implement every animation/state at once.

Start simple.

---

## 5. Animation Architecture

Use a simple state-based approach if possible.

Potential states:

- Idle
- LookingLeft
- LookingRight
- LookingUp
- LookingDown
- Blinking
- Sleeping

However, **do not over-engineer this**.

If a simpler approach works, use the simpler approach.

Keep animation logic separate from drawing/rendering logic where reasonably practical.

Do not create a complicated game engine.

---

## 6. Clock

Add a digital clock near the bottom of the screen.

The clock must display the actual current Windows system time.

Requirements:

- update automatically
- support 12-hour format
- support 24-hour format
- clean minimalist font
- positioned near the bottom
- visually separate from the eyes
- blue or another subtle matching color

The clock should eventually be configurable.

For example:

```text
        ██████        ██████


                 03:52
```

Do not hard-code the time.

---

## 7. User Interaction

This is intended to eventually behave like a real Windows screensaver.

When the screensaver is active:

### Mouse

Any meaningful mouse movement should exit the screensaver.

Mouse clicks should exit the screensaver.

### Keyboard

Any keyboard input should exit the screensaver.

The application should NOT capture or interfere with normal keyboard/mouse usage after the screensaver exits.

During normal development/debugging in Visual Studio, keep a convenient way to run the application without requiring Windows to launch it through the Screen Saver settings.

A normal `.exe` development/debug mode is required.

---

## 8. Screensaver Support

Do NOT make `.scr` support the first development priority.

First make a normal working `.exe`.

Once the pet is working properly, implement Windows screensaver behavior.

Eventually support:

- `.scr` execution
- fullscreen operation
- mouse movement exit
- mouse click exit
- keyboard exit
- appropriate Windows screensaver launch behavior
- preview/configuration behavior where practical

Keep the development `.exe` available so I can easily test changes from Visual Studio.

Do not rename files to `.scr` and consider the project finished. Implement the necessary screensaver behavior properly.

---

## 9. Code Simplicity

This is a learning side project, so this requirement is extremely important:

**Keep the code as simple as possible.**

Prefer:

- straightforward C#
- small methods
- understandable classes
- clear variable names
- simple control flow
- built-in .NET functionality
- minimal dependencies

Avoid unnecessary:

- design patterns
- abstractions
- frameworks
- dependency injection
- complicated architecture
- excessive interfaces
- premature optimization

Do NOT turn a tiny digital pet into an enterprise application. 😂

If a simple solution works, use it.

---

## 10. Comments

Add useful comments throughout the code.

However, do not comment every line.

Comments should explain:

- non-obvious code
- Windows-specific behavior
- animation logic
- rendering logic
- screensaver-specific behavior
- important timing decisions

Whenever a new or unfamiliar C#/.NET concept is introduced, explain it briefly.

When useful, explain concepts using Unity comparisons.

For example:

> **Unity equivalent:**  
> This Timer is roughly performing the job that `Update()` would normally perform.

The goal is that I can inspect the project and gradually understand how it works.

---

## 11. Unity Comparisons

I have experience with Unity concepts, so when explaining architecture, use comparisons when appropriate.

Useful comparisons:

| Unity | This project |
|---|---|
| `Update()` | Timer/update loop |
| `Time.deltaTime` | elapsed time |
| `GameObject` | pet/eye object |
| `Transform` | X/Y position |
| `SpriteRenderer` | drawing/rendering |
| Canvas | application window |
| Input | Windows keyboard/mouse events |
| Game fullscreen | screensaver fullscreen |

Do not force these comparisons when they don't make sense.

---

## 12. Project Structure

Do NOT put everything inside `Form1.cs`.

Keep the project reasonably organized.

A possible structure:

```text
BoxEyePet/
│
├── Program.cs
├── MainForm.cs
│
├── Pet/
│   ├── Pet.cs
│   ├── Eye.cs
│   └── PetState.cs
│
├── Animation/
│   └── PetAnimator.cs
│
├── Rendering/
│   └── PetRenderer.cs
│
├── Configuration/
│   └── PetSettings.cs
│
└── README.md
```

However, this is only a guideline.

Do not create classes just for the sake of having many classes.

If something can reasonably remain together, keep it together.

---

## 13. Development Phases

Build this project incrementally.

### Phase 1 — Basic window

Create:

- C# Windows Forms application
- fullscreen-capable window
- black background
- clean rendering surface

Then add:

- two blue rectangular eyes

At the end of Phase 1, I should be able to run the application and see the eyes.

---

### Phase 2 — Basic interaction

Add:

- mouse movement detection
- mouse click exit
- keyboard exit

Keep this simple.

Make sure the development version can still be tested comfortably from Visual Studio.

---

### Phase 3 — Blinking

Add:

- random blink intervals
- simple closing animation
- reopening animation
- natural timing

Do not add complex expressions yet.

---

### Phase 4 — Looking around

Add:

- looking left
- looking right
- optional looking up/down
- random idle behavior

Keep movements subtle.

---

### Phase 5 — Clock

Add:

- current time
- 12/24-hour option
- automatic updating
- bottom positioning

---

### Phase 6 — Personality

Add subtle behaviors such as:

- occasional longer blink
- add suitable eyebrows
- sleepy state
- surprised expression
- happy expression
- random idle behavior


Do NOT add sound yet.

Do NOT add a body yet.

---

### Phase 7 — Configuration

Eventually allow:

- eye size
- eye spacing
- eye position
- eye color
- animation speed
- blink frequency
- clock visibility
- clock position
- clock format
- background color
- personality level

Do not build the settings UI until the core application is stable.

---

### Phase 8 — Windows Screensaver

Finally implement proper `.scr` screensaver support.

Keep the normal `.exe` development mode.

Test the screensaver independently from the development mode.

---

## 14. Future Ideas — Do NOT Implement Yet

Keep these in mind for possible future versions but do not implement them unless I specifically request them:

- mouse tracking
- eyes following the cursor
- different eye designs
- glow effects
- particles
- sound
- music reactions
- weather
- notifications
- virtual pet needs
- hunger
- sleep schedule
- leveling system
- achievements
- desktop interaction
- tiny body
- accessories
- hats 😂
- multiple pets
- watch-face version for my Amazfit Pop 3S

The first goal is simply:

**black screen + two blue boxy eyes + subtle animation + clock + proper screensaver behavior.**

---

## 15. Git Safety

Treat this as a Git project.

Before making major changes:

- keep changes small
- don't delete working functionality unnecessarily
- avoid rewriting large portions of the project without a reason
- make it easy to revert changes

If Git is available, recommend sensible commit points after major working phases.

---

## 16. Important Agent Behavior

Do NOT blindly generate the entire project in one huge step.

Work incrementally.

At the beginning:

1. Inspect the existing project.
2. Determine whether it is already a valid Visual Studio/.NET project.
3. Do not overwrite existing files unnecessarily.
4. Explain what you intend to change.
5. Implement **Phase 1 only**.

After Phase 1:

- explain what was created
- explain how to run it in Visual Studio
- explain the important code
- identify what I should see when it works
- wait for my next instruction before moving to the next major phase

If you encounter an error:

1. Diagnose it.
2. Explain the cause simply.
3. Fix the smallest reasonable part.
4. Don't randomly rewrite the project.

---

# Final Goal

I want to end up with a tiny personal digital creature that feels like it is **living on my Windows desktop**:

```text
                    ██████        ██████
                    ██████        ██████
                    ██████        ██████


                           👀


                            03:52
```

Minimal.

Black background.

Electric-blue eyes.

Subtle personality.

Always alive when the screensaver is running.

And when I move my mouse or press a key:

**the little guy disappears and Windows returns to normal.**

Start with **Phase 1 only**.

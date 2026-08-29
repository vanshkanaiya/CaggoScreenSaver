<div align="center">

# 🤖 CaggoScreenSaver

**A procedural, cybernetic Box-Eye Digital Robot Pet screensaver for Windows.**

[![Release](https://img.shields.io/github/v/release/vanshkanaiya/CaggoScreenSaver?color=00F0FF&style=for-the-badge&logo=github)](https://github.com/vanshkanaiya/CaggoScreenSaver/releases/latest)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-0078D6?style=for-the-badge&logo=windows)](https://github.com/vanshkanaiya/CaggoScreenSaver)
[![.NET](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23%2012-239120?style=for-the-badge&logo=csharp)](https://github.com/vanshkanaiya/CaggoScreenSaver)
[![License](https://img.shields.io/badge/License-MIT-brightgreen?style=for-the-badge)](LICENSE)

<br />

*Inspired by desktop companion robots (like Vector & Cozmo), Caggo brings an expressive, lively digital pet to your screen whenever your computer goes idle.*

[📥 Download Latest Release](https://github.com/vanshkanaiya/CaggoScreenSaver/releases/latest) • [✨ Features](#-key-features) • [💻 Installation](#-installation-guide) • [🛠️ Build from Source](#-building-from-source)

</div>

---

## 📖 Overview

**CaggoScreenSaver** is a modern Windows screensaver (`.scr`) written in C# and .NET 8. Instead of static loops or pre-rendered video files, Caggo is powered by a **real-time procedural state engine** featuring dynamic eye expressions, glowing neon bloom graphics, organic squash-and-stretch deformation, multi-monitor spanning, and proactive OLED anti-burn-in safeguards.

---

## ✨ Key Features

### 🎭 1. Procedural Personality & Expressive Moods
Caggo transitions naturally between distinct emotional states using procedural interpolation and independent gaze scheduling:

| Mood | Expression | Behavior & Visuals |
| :--- | :---: | :--- |
| **Normal** | `■ ■` | Calm, alert electric-cyan squircle eyes looking around curiously with subtle blinks. |
| **Happy** | `^ ^` | Arched joyful wedge eyes with dynamic rhythm bouncing and angled eyebrows. |
| **Surprised** | `■! ■!` | Instant eye enlargement (`+14%`), quick upward pop, and raised eyebrows in shock. |
| **Sleepy** | `— —` | Droopy half-closed eyelids, slow heavy blinks, and relaxed downward brows. |
| **Mean / Grumpy** | `\ /` | Fierce inward-slanted eyelids and sharply furrowed inward eyebrows. |

---

### 🌟 2. Neon Bloom & Squircle Aesthetics
- **Multi-Pass Emissive Bloom**: Layered semi-transparent alpha strokes create a soft, glowing neon aura around the eyes and eyebrows on pure pitch-black (`#000000`) backgrounds.
- **Squircle Rounded Geometry**: Modern proportional corner smoothing for a sleek digital robot interface.
- **Organic Squash & Stretch**: Volume-preserving physics deform eye width and height during fast glances, blinks, and happy bounces.

---

### 🛡️ 3. OLED & CRT Anti-Burn-In Protection
Screensavers exist to protect your display. Caggo includes dual anti-burn-in mechanisms:
1. **Screen Roaming Anchor Drift**: Every 25–60 seconds, Caggo gently glides its anchor coordinates to a different area of the screen so pixels never remain static.
2. **Deep Sleep Idle Dimming**: After prolonged runtime (>5 minutes) or in deep sleepy states, brightness and glow opacity attenuate gracefully to reduce pixel fatigue.

---

### 🖥️ 4. Full Multi-Monitor & Windows OS Integration
- **Multi-Monitor Spanning**: Automatically detects all connected displays (`Screen.AllScreens`) and runs borderless canvas instances across every monitor with synchronized exit.
- **Live Settings Mini-Preview (`/p`)**: Embeds seamlessly into the Windows Screen Saver Settings mini-monitor preview pane via Win32 `SetParent` interop.
- **Input Grace Period**: 1.5-second startup grace period and 15px movement threshold prevent accidental exits from desk bumps or launch jitter.

---

## 💻 Installation Guide

### Quick Install (No Coding Required)

1. Go to the **[Latest Release Page](https://github.com/vanshkanaiya/CaggoScreenSaver/releases/latest)**.
2. Download **`CaggoScreenSaver.scr`** (or download and extract the `.zip`).
3. Right-click **`CaggoScreenSaver.scr`** and select **"Install"**.
4. The Windows **Screen Saver Settings** window will appear with Caggo active in the preview monitor.
5. Set your preferred idle timeout (e.g., `5` minutes) and click **Apply** / **OK**.

> **Note for Windows SmartScreen**:
> Because `.scr` files are executable screensavers, Windows may display a blue *"Windows protected your PC"* popup on first run. Click **"More info"** → **"Run anyway"**.

---

## 🛠️ Building from Source

### Prerequisites
- [Windows 10 / 11](https://www.microsoft.com/windows)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or [Visual Studio 2022](https://visualstudio.microsoft.com/) (with *.NET desktop development* workload)

### Build Commands

1. **Clone the repository**:
   ```bash
   git clone https://github.com/vanshkanaiya/CaggoScreenSaver.git
   cd CaggoScreenSaver
   ```

2. **Build Release Binary**:
   ```bash
   dotnet build -c Release
   ```

3. **Output Location**:
   The custom MSBuild target automatically generates both the `.exe` and `.scr` in:
   ```text
   CaggoScreenSaver/bin/Release/net8.0-windows/
   ├── CaggoScreenSaver.exe
   ├── CaggoScreenSaver.scr   <-- Ready to Install!
   ├── CaggoScreenSaver.dll
   └── ...
   ```

---

## 🕹️ Command-Line Switches

CaggoScreenSaver adheres to the standard Windows Screen Saver command-line protocol:

| Switch | Argument | Description |
| :--- | :--- | :--- |
| `/s` | *None* | Runs the screensaver fullscreen on all connected monitors. |
| `/p` | `<HWND>` | Runs in preview mode embedded inside the specified parent window handle. |
| `/c` | `[:HWND]` | Displays the Configuration / About dialog. |

---

## 📂 Project Architecture

```text
CaggoScreenSaver/
├── Animation/
│   └── PetAnimator.cs          # Procedural state engine, gaze scheduler & roam physics
├── Pet/
│   └── BoxPet.cs               # Eye rendering, bloom passes, squircles & eyebrows
├── MainForm.cs                 # Window host, Win32 preview interop & input coordination
├── MainForm.Designer.cs        # Form layout component declarations
├── Program.cs                  # Entry point, command-line argument parser & mutex check
├── CaggoScreenSaver.csproj     # Project config & automatic .scr generation target
├── .gitignore                  # Clean repository ignore filters
└── README.md                   # Project documentation
```

---

## 🤝 Contributing

Contributions, feature suggestions, and pull requests are welcome!
1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/CoolFeature`)
3. Commit your Changes (`git commit -m 'Add CoolFeature'`)
4. Push to the Branch (`git push origin feature/CoolFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the **MIT License**. See `LICENSE` for more information.

---

<div align="center">
  <sub>Created with ❤️ by <a href="https://github.com/vanshkanaiya">Vansh Kanaiya</a></sub>
</div>

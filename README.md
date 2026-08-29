# CaggoScreenSaver 🤖

A modern, procedural Box-Eye Digital Robot Pet screensaver for Windows built with C# and .NET 8 Windows Forms.

Inspired by desktop companion robot aesthetics (such as Vector / Cozmo), CaggoScreenSaver brings a lively, expressive cyber pet to your screen whenever your PC is idle.

---

## ✨ Features

- **Procedural Eye & Eyebrow Expressions**:
  - `Normal`: Calm, alert cyan squircle eyes.
  - `Happy`: Joyful arched eyes with rhythmic vertical bounce.
  - `Surprised`: Shocked enlargement with raised eyebrows.
  - `Sleepy`: Half-closed droopy eyes with relaxed blinks.
  - `Mean`: Grumpy inward-slanted fierce glare.
- **Neon Glow & Bloom**: Multi-pass alpha glow creating a vivid, emissive OLED screen aura.
- **Organic Animation Physics**: Squash & stretch volume conservation during quick blinks, glances, and bounces.
- **Anti-Burn-In & OLED Protection**:
  - Roaming anchor drift periodically shifts screen positions to avoid pixel fatigue.
  - Automatic sleep dimming after prolonged idle running.
- **Multi-Monitor Support**: Spans all connected displays with synchronized exit.
- **Windows Screensaver Integration**:
  - Native `.scr` support.
  - Real-time mini preview in Windows Screen Saver Settings (`/p <HWND>`).
  - Configuration switch support (`/c`).

---

## 🚀 Getting Started

### Prerequisites
- Windows 10 / 11
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or Visual Studio 2022

### Build from Source
Clone the repository:
```bash
git clone https://github.com/your-username/CaggoScreenSaver.git
cd CaggoScreenSaver
```

Build the project in Release mode:
```bash
dotnet build -c Release
```

The MSBuild pipeline automatically creates both `CaggoScreenSaver.exe` and `CaggoScreenSaver.scr` inside `CaggoScreenSaver/bin/Release/net8.0-windows/`.

---

## 💻 Installation on Windows

1. Navigate to `CaggoScreenSaver/bin/Release/net8.0-windows/`.
2. Right-click **`CaggoScreenSaver.scr`** and select **"Install"**.
3. Windows Screen Saver Settings will open. Adjust your wait timer and click **OK**.

---

## 🛠️ Tech Stack
- **Language**: C# 12 / .NET 8.0
- **Framework**: Windows Forms (GDI+ Anti-Aliased Graphics)
- **APIs**: Win32 Interop (`user32.dll`)

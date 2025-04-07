# unity-photon-assignment
This project is a multiplayer third-person movement demo built using **Unity 6000.0.37f1** and **Photon Fusion 2 (Shared Mode)**. It features smooth third-person character control, basic networking for movement synchronization, and a simple lobby-to-game flow.

## Features

-  Unity 6000.0.37f1 (LTS preview)
-  Photon Fusion 2 (Shared Mode)
-  Character model & animations from Unity's **Third Person Starter Assets Pack**
-  Smooth synced movement (walk, run, jump)
-  Basic camera with Cinemachine 3.1.3
-  Display player name above character (billboard)
-  Basic network UI: host/join, disconnect, feedback
-  Cursor lock/unlock control for menu/gameplay

## Requirements

- Unity 6000.0.37f1
- Photon Fusion 2 (installed via Unity Package Manager or Git)
- Unity's Starter Assets - Third Person Character Controller (for model & animation)
- Cinemachine 3.1.3

## How to Test

### 1. Launch Test in Editor
- Open `Menu.unity` scene.
- Press **Play**.
- Enter your desired name in the **InputField** (this name will show above your character).
- Use **Host** button to start the game.
- A **loading screen** will appear briefly before entering the game.
- A player will spawn in the game world.

### 2. Add a Second Player (Multiplayer Test)
- Build the project (File → Build Settings → Add scenes → Build).
- Run **built .exe** alongside the Unity Editor.
- On one instance: click **Host**
- On the second instance: click **Join**
- You will see two synced characters.
- Try moving both → check animation & camera sync.

### 3. Disconnect Test
- Close host window, wait for a while, client should return to menu with message.
- Test network resilience with cable disconnect or forcing shutdown.

## Notes

- Cursor is locked during gameplay and restored on returning to menu.
- Animations are handled via parameters: `Speed`, `Jump`, `Grounded`, `MotionSpeed`.
- If any issues occur with animation on clients, fallback logic is applied based on movement sync state.

## Known Issues

- Minor position jitter when players first connect (Fusion sync delay).
- UI may require manual resize adjustment on low resolutions.
- If animations do not play for remote players, ensure `[Networked]` variables are updating correctly.

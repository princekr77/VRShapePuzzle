# VR Shape Puzzle

A virtual reality application developed with **Unity** and the **Meta All-in-One SDK**. This application demonstrates core VR interaction mechanics, hand tracking, socket placement puzzle mechanics, world-space UI, haptic/audio feedback, animations, and VR screen transitions for Meta Quest headsets.

---

## 🛠️ Technical Requirements & Specifications

- **Engine Version:** Unity 6
- **Render Pipeline:** Universal Render Pipeline (URP `17.3.0`)
- **SDK & Packages:**
  - Meta All-in-One SDK (`com.meta.xr.sdk.all` `v207.0.0`)
  - Unity OpenXR Plugin (`com.unity.xr.openxr` `v1.18.0`)
  - Unity XR Plugin Management (`com.unity.xr.management` `v4.7.0`)
  - TextMeshPro (`com.unity.ugui` `v2.0.0`)
- **Target Hardware:** Meta Quest 3 / Quest 3S / Quest Pro / Quest 2
- **Language:** C#

---

## 🎮 Core Features Implemented

### 1. Hand Tracking & Controller Auto-Switching
- Full support for both **Hand Tracking** and **Meta Quest Touch Controllers** with seamless auto-switching.
- Configured hand grab for all interactables using Meta Interaction SDK (`Oculus.Interaction`).

### 2. Shape Objects & Auto-Return Mechanics
- **3 Interactable Shapes:** Cube, Sphere, and Cylinder.
- **Auto-Return System (`DropZoneDetector` & `PuzzleObject`):** If an object is dropped or thrown outside the designated table/drop zone, it automatically returns to its original position after a 2-second grace period.

### 3. Socket Placement Puzzle Mechanics
- **3 Target Sockets:** Distinct sockets matching each shape type (`Cube`, `Sphere`, `Cylinder`).
- **Object Validation:** Strict validation prevents incorrect shape placement.
- **Incorrect Placement:**
  - Displays a red error banner: `"Wrong socket! Try the matching shape."`
  - Triggers error audio chime and controller warning haptic pulses (`HapticManager`).
  - Auto-returns object to table origin.
- **Correct Placement:**
  - Snaps object precisely to the socket `snapPoint`.
  - Locks object in place (`isKinematic = true`) and disables further grabbing.
  - Plays placement success audio & haptic pulse.
  - Changes socket visual color to green.
- **Puzzle Completion (3/3 Shapes Placed):**
  - Displays `"Task Completed!"` banner text immediately on the Canvas.
  - Plays completion fanfare audio and haptic feedback.
  - Triggers the door opening animation (`"Open"` parameter).

### 4. World-Space Dashboard UI
- **Floating Dashboard:** Displays live **Timer**, **Objects Remaining count**, **Reset Button**, and **Restart Button**.
- **Self-Healing UI Manager (`UIManager`):** Automatically discovers scene text elements and updates UI state dynamically.

### 5. Wrist-Mounted Hand Menu (`WristMenuController`)
- Menu canvas anchored directly to the user's left wrist with customizable transform offsets (`Local Position: (0.05, 0.02, 0.08)`, `Rotation: (16°, -72°, -42°)`).
- **Interactive Options:**
  - **Reset Scene:** Resets all shape objects, sockets, timer, and errors.
  - **Restart Task:** Restarts the puzzle task from scratch.
  - **Toggle Hand Visualization:** Toggles VR hand mesh visibility on and off (`HandVisualToggle`).

### 6. Interaction Feedback, Audio & Haptics
- **Visual Highlight:** Highlights object mesh on hover with bright cyan color using `MaterialPropertyBlock` targeting URP `_BaseColor` and `_Color`.
- **Audio Feedback (`AudioManager`):** Synthesizes & plays audio SFX for Hover, Grab, Place, Wrong Placement, and Completion.
- **Haptic Feedback (`HapticManager`):** Delivers controller vibration pulses for Hover, Grab, Place, and Wrong Placement via `OVRInput` and `UnityEngine.XR.InputDevice`.

### 7. Door Animations
- **Door Opening Animation:** Triggers `doorAnimator` with parameter `"Open"` upon solving the puzzle.
---

## ⚙️ Setup & Installation Instructions

### Prerequisites
1. **Unity Hub** with **Unity 6** installed.
2. **Android Build Support** module installed in Unity (Android SDK & NDK Tools, OpenJDK).
3. **Meta Quest Link App** (for PC Editor testing) or **Meta Quest Mobile App** (to enable Developer Mode on headset).

### Project Setup
1. Clone / download this repository into your local Unity projects folder.
2. Launch Unity Hub and click **Open** ➔ select `VRShapePuzzle` folder.
3. Open `Assets/Scenes/SampleScene.unity`.
4. Go to **Edit ➔ Project Settings ➔ XR Plug-in Management**:
   - Under **Android Settings tab**, ensure **OpenXR** is checked.
   - Expand **OpenXR** feature group and ensure **Meta Quest Support**, **Hand Tracking**, and **Hand Interaction Poses** are enabled.

---

## 🏗️ Build & Deployment Instructions (Meta Quest APK)

Follow these steps to build the APK and deploy directly to your Meta Quest 3 / Quest 3S / Quest Pro / Quest 2:

### 1. Connect Headset
- Enable **Developer Mode** on your Meta Quest headset via the Meta Quest mobile app (`Devices ➔ Headset Settings ➔ Developer Mode`).
- Connect your Meta Quest to your PC using a USB-C cable. Put on the headset and select **Allow USB Debugging**.

### 2. Switch Platform to Android
- In Unity, navigate to **File ➔ Build Settings**.
- Select **Android** under the platform list and click **Switch Platform**.
- Ensure `Assets/Scenes/SampleScene.unity` is checked under **Scenes In Build**.

### 3. Configure Player Settings
Click **Player Settings...** (bottom left of Build Settings window) and verify:
- **Company Name:** Custom or `DefaultCompany`
- **Product Name:** `VRShapePuzzle`
- **Minimum API Level:** `Android 10.0 (API Level 29)` or higher
- **Target API Level:** `Automatic (highest installed)` / `Level 32+`
- **Scripting Backend:** `IL2CPP`
- **Target Architectures:** `ARM64` checked (`ARMv7` unchecked)
- **Active Input Handling:** `Input System Package (New)` or `Both`

### 4. Build and Run
- In **Build Settings**, set **Run Device** to your connected Meta Quest device.
- Click **Build and Run**.
- Choose a save location (e.g. `Builds/VRShapePuzzle.apk`).
- Unity will compile the IL2CPP binaries, package the APK, and install it onto your Quest.
- Once installed, the application will automatically launch in your Quest headset. You can also re-launch it anytime from **App Library ➔ Unknown Sources ➔ VRShapePuzzle**.

---

## 💻 PC Editor Testing (Meta Quest Link / AirLink)

To test and debug directly inside the Unity Editor without building an APK:

1. Connect your Meta Quest to your PC using a Meta Quest Link cable or AirLink.
2. Launch the **Meta Quest Link** desktop application on your PC.
3. In Unity, go to **Edit ➔ Project Settings ➔ XR Plug-in Management**:
   - Under **PC Standalone Settings tab**, ensure **OpenXR** is checked.
4. Press the **Play** button in Unity Editor.
5. You can now test hand tracking, controller interactions, UI buttons, and shape puzzle mechanics live inside the Editor.

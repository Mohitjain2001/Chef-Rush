# Yes Chef! - 3D Kitchen Management Game

**"Yes Chef!"** is a 3D top-down kitchen management game built in Unity (C#) for the Tentworks Interactive Dev Test.

---

## 🎮 Game Overview

You control a solo chef serving various plates of food in a single 3D kitchen area. Fulfill as many customer orders as possible within the **3-minute (180-second)** time limit while managing preparation timers and ingredient constraints.

### Key Gameplay Mechanics

- **Customer Windows (4 Windows)**: Active orders are generated automatically (50% chance of 2 ingredients, 50% chance of 3 ingredients; duplicates allowed). Each window features a timer tracking order duration.
- **Scoring Formula**: 
  $$\text{Order Score} = \sum (\text{Ingredient Values}) - \lfloor \text{Time Active (seconds)} \rfloor$$
  *Order score can become negative if delayed! Completed orders display an animated score floating popup (`+17`, `-6`). Empty windows respawn a new order after a 5-second delay.*
- **Ingredient Preparation & Score Values**:
  - **Vegetable**: Score = `20`. Must be chopped at the **Cutting Table** (2.0s duration). Includes overhead progress bar & visual raw/chopped distinction.
  - **Cheese**: Score = `10`. Requires no prep, can be delivered directly.
  - **Meat**: Score = `30`. Must be cooked at the **Stove** (6.0s duration per slot). Stove features **2 independent cooking slots** with overhead progress bars & visual raw/cooked distinction.
- **Trash Can**: Discards any held ingredient (raw or prepared).
- **Single-Item Carrying & Smart Swapping**: The player can carry only 1 item at a time. Smart item swapping allows immediate pick-up of prepared items while placing a new raw ingredient.

---

## ⌨️ Controls

| Action | Input Key | Description |
|---|---|---|
| **Move Chef** | `W` `A` `S` `D` / `Arrow Keys` | Top-down 3D character movement |
| **Interact / Action** | `E` / `Space` / `Left Mouse Click` | Pick up raw items, place on stations, deliver to window, or throw in trash |
| **Pause Game** | `Escape` / `P` | Pause/Resume menu toggle |

---

## 🛠️ Architecture & Design Decisions

The project follows a clean, modular, and scalable C# architecture without third-party plugins:

1. **Data-Driven ScriptableObjects (`IngredientSO`)**: Ingredient properties (scores, prep times, colors, primitive shapes, scales) are decoupled into ScriptableObjects for easy balancing and extension.
2. **State Management (`GameManager`)**: Handles state transitions (`StartMenu`, `Playing`, `Paused`, `GameOver`), score aggregation, 3-minute timer countdown, and `PlayerPrefs` high score persistence.
3. **Order Manager (`OrderManager`)**: Handles random order composition generation and customer window assignment.
4. **Component-Based Stations (`IInteractable`, `BaseStation`)**: Polymoic station design (`FridgeStation`, `CuttingTableStation`, `StoveStation`, `CustomerWindowStation`, `TrashStation`) for easy extension of new kitchen devices.
5. **Event-Driven UI (`UIManager`)**: Listens to `GameManager` events to update HUD, interaction prompts, score, high score, and game over overlays.
6. **Automated Scene Builder (`KitchenSceneSetup` & `SceneAutoBuilder`)**: Features a 1-click Editor menu item (`YesChef -> Build & Setup Kitchen Scene`) that populates the complete 3D environment, cameras, lights, walls, stations, and UI out-of-the-box.

---

## 🚀 How to Run in Unity (6000+)

1. Open the project directory `/Users/mohitjain/Chef Rush` in **Unity Editor (Version 6000+)**.
2. Go to the top menu bar and select:
   ```text
   YesChef -> Build & Setup Kitchen Scene
   ```
3. Click the **Play (▶️)** button in Unity Editor.
4. On the Start Screen, click **START GAME** and enjoy playing!

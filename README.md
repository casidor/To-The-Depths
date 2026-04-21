# 🏚️ To the Depths

A console-based dungeon crawler written in C# (.NET 10). Explore procedurally generated dungeons, collect keys and gold, avoid enemies, and find your way out.

---

## 🎮 Gameplay

You wake up deep underground. The exit is sealed — you'll need to collect **3 keys** scattered across the dungeon before you can escape. Watch out for thieves lurking in the dark, spend gold at altars to restore health, and descend as deep as possible to find freedom.

### Win Condition
Reach **Floor 5**, collect all **3 keys**, then step onto the **Exit tile (`▥`)**.

### Death Condition
Your HP reaches **0**. Progress is lost — you return to the main menu.

---

## 🗺️ Legend

| Symbol | Meaning                        |
|--------|--------------------------------|
| `☺`    | Player (you)                   |
| `☻`    | Enemy (Thief)                  |
| `⚷`    | Key                            |
| `♦`    | Gold (+10 per pickup)          |
| `+`    | Altar (healing for gold)       |
| `▥`    | Exit                           |
| `█`    | Wall                           |
| `·`    | Floor                          |

---

## 🕹️ Controls

| Key              | Action                  |
|------------------|-------------------------|
| `W` / `↑`        | Move Up                 |
| `S` / `↓`        | Move Down               |
| `A` / `←`        | Move Left               |
| `D` / `→`        | Move Right              |
| `ESC`            | Return to Menu / Cancel |
| `Enter`          | Confirm (in menus)      |

---

## ⚔️ Game Mechanics

### 🔑 Keys & Exit
Collect all **3 keys** scattered across the dungeon to unlock the exit. The sidebar tracks your progress. Once all keys are collected, the exit becomes active — find it and step in.

### 👿 Enemies (Thieves)
Enemies patrol the dungeon and turn aggressive when you get within **7 tiles**. When an enemy reaches you:
- You lose **15 HP**
- You lose **10 Gold**
- The enemy disappears

Enemies wander randomly when out of range, and chase you using pathfinding when close.

### ⛪ Altars
Ancient altars are scattered across each floor (up to 3 per level). When you step onto an altar, a menu appears:
- **Heal**: Spend **20 Gold** to restore **20 HP** (up to your maximum)
- Each altar has **2 charges** — after that, it fades away

### ♥ Health
You start with **100 HP**. Damage is taken from enemy attacks. Health can be restored only at altars. If HP drops to 0, it's game over.

### 🪙 Gold
Gold is scattered across floor tiles (~15% chance per tile). It's used to pay for healing at altars. Enemies can steal gold when they attack you, but your gold can never go below 0.

### 🪜 Descending
Step onto an active exit tile to descend to the next floor. Your **HP, Gold, and current floor** are carried over, but your **keys reset** — you'll need to collect them again on each floor. Progress is **autosaved** on descent.

---

## 💾 Save System

- The game **autosaves** when you descend to the next floor
- From the main menu, choose **Load Save** to resume where you left off
- If you exit mid-floor, that floor's progress is lost (you restart the floor from the same seed)
- A confirmation prompt appears if you try to exit with unsaved progress
- Save data includes a checksum — corrupted or tampered saves are deleted automatically

---

## ⚙️ Level Generation

Each floor is procedurally generated using a seeded random:

1. Up to **10 rooms** are placed randomly (no overlaps)
2. Rooms are connected with **L-shaped corridors**
3. **Walls** are added around all floor tiles
4. **Gold** is scattered by chance (~10% per floor tile)
5. **3 Keys** are placed in random rooms (never the starting room)
6. **1 Exit** is placed in the room farthest from the start
7. **7 Enemies** are placed in random rooms (never the starting room)
8. **2 Altars** are placed at room centers (never the starting room)
9. The player starts at the center of the first room

The same seed produces the same dungeon layout, which means loading a save restores the same map.

---

## 🚀 Getting Started

### ⬇️ Just play
Download `ToTheDepths.exe` from the [Releases](https://github.com/casidor/To-The-Depths/releases/latest) page and run it. No installation needed.

> Windows only.

### 🔧 Build from source
```bash
git clone https://github.com/casidor/To-The-Depths.git
cd To-The-Depths
dotnet run --project ConsoleUI
```

---

## 🔧 Configuration

All game parameters live in `Core/Config.cs`:

| Constant         | Default | Description                              |
|------------------|---------|------------------------------------------|
| `ConsoleWidth`   | 110     | Console window width                     |
| `ConsoleHeight`  | 30      | Console window height                    |
| `FieldWidth`     | 80      | Dungeon grid width                       |
| `FieldHeight`    | 25      | Dungeon grid height                      |
| `PlayerMaxHP`    | 100     | Player starting and maximum HP           |
| `MaxRooms`       | 10      | Maximum number of rooms per floor        |
| `KeysAmount`     | 3       | Keys required to open the exit           |
| `EnemiesAmount`  | 7       | Number of enemies per floor              |
| `AltarsAmount`   | 2       | Number of altars per floor               |
| `ExitAmount`     | 1       | Number of exits per floor                |
| `GoldAmount`     | 5       | Gold value per pickup                    |
| `GoldChance`     | 15      | % chance of gold on each floor tile      |
| `EnemyDamage`    | 15      | HP lost when attacked by an enemy        |
| `GoldStolen`     | 10      | Gold lost when attacked by an enemy      |
| `AggroRange`     | 7       | Tile distance at which enemies chase     |
| `AltarHeal`      | 20      | HP restored per altar use                |
| `HealCost`       | 20      | Gold cost per altar heal                 |
| `AltarCharges`   | 2       | Uses per altar before it fades           |
| `MaxFloor`       | 5       | Total number of floors                   |
| `MinRoomSize`    | 3       | Minimum room dimension                   |
| `MaxRoomSize`    | 10       | Maximum room dimension                   |
| `GenAttempts`    | 300     | Room placement attempts per generation   |

---

## 📸 Screenshots

### Main Menu
![Main Menu](docs/images/menu.png)

### Gameplay & Sidebar
![Gameplay](docs/images/gameplay.png)

### Escape
![Win Screen](docs/images/win.png)

---

## 📄 License

MIT — do whatever you want with it.

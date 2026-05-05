# 🏚️ To the Depths

> *The exit is sealed. The keys are scattered. The dark is watching.*

A dungeon crawler built with **AvaloniaUI** and **C# (.NET 10)**. Descend through five procedurally generated floors, collect keys, loot gold, fight increasingly dangerous enemies, and upgrade your weapons in the shop between floors. Every run uses a unique seed — but your saves always restore the exact same map.

---

## 📸 Screenshots

| Main Menu | Gameplay |
|-----------|----------|
| ![Menu](docs/images/menu.png) | ![Gameplay](docs/images/gameplay.png) |

---

## 🚀 Getting Started

**Just play** — download the latest release from the [Releases page](https://github.com/casidor/To-The-Depths/releases/latest) and run the executable. No installation required.

> Windows only.

**Build from source:**
```bash
git clone https://github.com/casidor/To-The-Depths.git
cd To-The-Depths
dotnet run --project AvaloniaUI/AvaloniaUI.Desktop
```

---

## 🎮 How to Play

Each floor has a set number of **keys** hidden across the dungeon. Find them all, then reach the **exit** — which is always tucked away in the room farthest from where you start. The exit opens the **shop**, where you can spend your hard-earned gold before dropping to the next floor.

Make it through all **5 floors** and you're out. Die anywhere and it's back to the menu.

---

## 🕹️ Controls

| Input | Action |
|---|---|
| `W A S D` / Arrow Keys | Move |
| `1` `2` `3` `4` | Switch hotbar slot |
| `E` | Toggle aim mode (ranged weapons) |
| Left Click *(aim mode)* | Shoot at tile |
| `Space` | Snap camera to player |
| Right Click + Drag | Pan camera freely |
| Scroll Wheel | Zoom in / out |
| `Esc` | Cancel aim / Exit menu |

---

## ⚔️ Combat

### Melee
Walk into an enemy to hit them with your equipped melee weapon. If you find a better melee weapon on the ground, it's **equipped automatically** (as long as it has a higher damage ceiling than what you have).

| Weapon | Damage | Floor |
|--------|--------|-------|
| Dagger | 15     | 2     |
| Sword  | 25     | 3     |

No melee equipped? You still punch for **10 damage**.

### Ranged
Press `E` to enter aim mode — visible tiles within range light up yellow. Click one to shoot. Press `E` again or `Esc` to cancel.

| Weapon    | Damage | Range | Base Ammo | Floor |
|-----------|--------|-------|-----------|-------|
| Bow       | 20     | 6     | 8         | 1     |
| Crossbow  | 35     | 8     | 5         | 4     |

Ammo doesn't refill between floors unless you reload in the shop.

---

## 👹 Enemies

Enemy HP scales with floor depth, reaching up to **×2.5** on the fifth floor.

### Soldier
The standard enemy. Chases you on sight (within 7 tiles) or if it hears you within 3 tiles — even through walls. Wanders randomly otherwise. Drops gold on death.

`30 HP · 5 DMG · Floor 1+`

### Tank
Slow and massive. Takes a beating before going down, so ranged weapons are your friend here. Same AI as the Soldier.

`75 HP · 3 DMG · Floor 3+`

### Ranged Enemy
The most annoying one. Shoots you from a distance and actively retreats when you close in. Corner it with melee if you can.

`24 HP · 8 DMG · 4 range · Floor 3+`

---

## 🛒 Shop

After every floor you get access to the shop before descending. Upgrades are permanent and carry through the rest of the run.

| Upgrade | Effect | Cost |
|---------|--------|------|
| **DMG ▲** | +10 damage to a weapon (up to 2× per weapon) | 450 / 1180 ♦ |
| **AMMO ▲** | +3 max ammo + instant reload (up to 2× per weapon) | 200 / 420 ♦ |
| **RELOAD** | Refill ammo to current max | 100 ♦ |
| **Melee DMG ▲** | Upgrade your equipped melee weapon | 450 / 1180 ♦ |

---

## ⛪ Altars

Two altars are placed on each floor (never in the starting room). Walk into one and you'll be offered a heal:

**Restore 30 HP** for **50 gold** — each altar has 2 charges before it goes dark.

---

## 🗺️ Floor Progression

| Floor | Enemies | Keys | Enemy HP | Notable |
|-------|---------|------|----------|---------|
| 1     | 7       | 3    | ×1.0     | Bow appears |
| 2     | 9       | 3    | ×1.3     | Dagger appears |
| 3     | 12      | 4    | ×1.6     | Tanks & ranged enemies spawn · Sword appears |
| 4     | 15      | 4    | ×2.0     | Crossbow appears |
| 5     | 18      | 5    | ×2.5     | Final floor — reach the exit to win |

---

## 💾 Saving & Loading

Progress is **autosaved the moment you descend** to a new floor. If you quit mid-floor, that floor's progress is lost — you'll restart it from scratch (same map, same seed). The game warns you before you exit with unsaved progress.

Save files include a **checksum** — modified or corrupted saves are detected and deleted automatically.

---

## 🗺️ World Generation

Each floor is built fresh from a seeded random — same seed, same dungeon, every time.

1. Up to **15 non-overlapping rooms** are scattered across the map
2. Rooms are connected by a **Minimum Spanning Tree**, with a 20% chance of extra random connections for variety
3. Corridors are carved with **BFS pathfinding** — strict routing first, relaxed fallback if needed
4. **Doors** are placed at room entrances — they close each turn and open when walked into
5. **Gold**, **keys**, **weapons**, **altars**, **enemies**, and the **exit** are placed by the rules above
6. The player always starts at the **center of the first room**; the exit is always in the **farthest room**

---

## 🔧 Configuration

Everything is tunable in `Core/Config.cs`. Key values:

| Constant | Default | What it controls |
|----------|---------|-----------------|
| `FieldWidth` / `FieldHeight` | 90 / 70 | Map size |
| `PlayerMaxHP` | 100 | Starting HP |
| `PlayerDamage` | 10 | Unarmed damage |
| `MaxRooms` | 15 | Rooms per floor |
| `AggroRange` | 7 | Enemy sight range |
| `HearRange` | 3 | Enemy hearing range (through walls) |
| `AltarHeal` / `HealCost` | 30 / 50 | Altar heal amount and gold cost |
| `AltarCharges` | 2 | Uses before altar fades |
| `MaxFloor` | 5 | Number of floors |
| `WeaponDamageUpgradeAmount` | 10 | DMG gained per shop upgrade |
| `RangedWeaponReloadCost` | 100 | Gold to reload |

---

## 📄 License

MIT — do whatever you want with it.

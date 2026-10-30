# LSA

Precision 3D fighting prototype (Unity, URP, mobile-first). Early scope: local 1v1 feel on a compact flat arena — mannequin + sword, fireball channeled from the weapon tip, MOBA touch HUD stubs.

## Requirements

- **Unity 2022.3 LTS** (project set to `2022.3.50f1`; nearby 2022.3.x should upgrade/import fine)
- Universal RP package (listed in `Packages/manifest.json`)

## Open & play

1. Clone this repo and open the project folder in **Unity Hub** → **Open** → select the repo root (the folder that contains `Assets/` and `ProjectSettings/`).
2. Wait for package import. On first open, menu **LSA → Configure URP (Mobile)** runs automatically when possible; if materials look pink, run that menu once.
3. Open `Assets/Scenes/Arena.unity` (or **LSA → Open Arena Scene**).
4. Press **Play**.

`GameBootstrap` builds the prototype at runtime: compact arena + bounds, player mannequin with sword, training dummy, fight camera, and MOBA HUD.

### Controls

| Action | Touch HUD | Keyboard / mouse |
|--------|-----------|------------------|
| Move | Left virtual stick | WASD / arrows |
| Jump | **JMP** | Space |
| Basic attack (sword melee) | **ATK** | LMB (not over UI) |
| Fireball (from sword tip) | **FB** | Q or RMB |
| Ability stubs | **A2** / **A3** | E / R |

### Precision note

Fireballs are straight-line projectiles with an **~8 cm** hit radius and **no magnetic aim / homing**. Sidestepping a few centimeters should clear a shot aimed at your previous center — that is an intentional acceptance criterion for this prototype.

## Layout

```
Assets/
  Scenes/Arena.unity          # Bootstrap entry scene
  Scripts/
    Arena/                    # Flat plane + wall bounds
    Bootstrap/                # Runtime prototype factory
    Combat/                   # Hurtbox, sword melee, fireball, weapon socket
    Projectiles/              # Precise (non-seeking) projectile
    Player/                   # Motor, fighter controller, camera
    Input/ + UI/              # Virtual stick + MOBA HUD
    Editor/                   # LSA menu helpers (URP + scene)
```

## Next (not in this PR)

- Netcode 1v1 authority
- Android device builds + touch feel pass
- Hit/miss/block feedback polish

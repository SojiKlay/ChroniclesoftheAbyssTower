# Game Flow System

## Main Flow

```text
Login/Register
-> Main Menu
-> New Game or Continue
-> Intro Story
-> StoryPage
-> Floor Progression
-> EndingPage
```

## App Start

- User opens app
- App checks session through auth/session logic
- User goes to Login/Register or Main Menu depending on session

## New Game Flow

1. User selects New Game
2. App creates or resets active player data
3. Player starts with base stats
4. App navigates to Intro Story
5. Story starts at floor 1

## Story Flow

Story data comes from `Resources/Raw/floors.json`.

Current data:
- 30 floors
- Floor 1 ถึง Floor 30
- Each floor has title, narrative, event type, choices and optional image file

Each choice can affect:
- HP
- Gold
- EXP
- Item reward
- Required item checks
- Blocked item checks
- Story journal unlock
- Floor advancement
- Ending type

## Side Pages During Gameplay

From the main game flow, player can open:
- Character page for stats
- Inventory page for item CRUD/use/delete
- Journal page for story/player journals
- SaveLoad page for save/load slots
- Backup page for export/import
- Settings page for preferences/audio

## Ending Flow

Ending depends on story state and final choice/outcome.

Current ending model supports:
- `Good`
- `Bad`
- other custom string values if story data uses them

## Demo Flow For Teacher

Recommended demo:

1. Register user
2. Login
3. Start New Game
4. Read intro
5. Choose a floor 1 option
6. Show HP/Gold/item result
7. Open Inventory and demonstrate CRUD
8. Open Journal and create/edit/delete a note
9. Save to slot
10. Load save
11. Show Backup page

# Q&A For Presentation

## 1. What is this project?

Chronicles of the Abyss Tower is a text-based dark fantasy RPG made with .NET MAUI, SQLite and MVVM. The player controls Arin and makes choices while climbing a 30-floor tower.

## 2. What does it demonstrate for class?

- CRUD
- Multi-page application
- SQLite database
- MVVM architecture
- Shell navigation
- Save/load system
- Local data seed from JSON

## 3. What CRUD systems are included?

Inventory CRUD:
- Create item when story gives reward
- Read inventory list
- Update quantity/use item
- Delete item

Journal CRUD:
- Create player journal
- Read story/player journals
- Update player journal
- Delete player journal

SaveData CRUD:
- Create save slot
- Read save slots
- Update/overwrite save slot
- Delete save slot

Player data also demonstrates create/read/update during new game and story progress.

## 4. What database is used?

SQLite local database. Main service is `DatabaseService.cs`. Database file name is `abyss_tower.db3`.

## 5. What tables exist now?

- `Users`
- `Players`
- `Items`
- `InventoryItems`
- `Journals`
- `SaveData`
- `StoryProgress`

## 6. How many screens are implemented?

Current main pages:
- Login
- Register
- Main Menu
- Intro Story
- Story
- Character
- Inventory
- Journal
- Journal Editor
- Save/Load
- Backup
- Settings
- Ending

## 7. How does the story system work?

`SeedDataService` loads `floors.json`. `StoryService` gets the current floor event and applies choice outcomes. Choices can change stats, give items, require items, unlock journals, advance floors or trigger endings.

## 8. How many floors/items/story journals are currently seeded?

- 30 floors
- 12 items
- 11 story journals

## 9. How is MVVM used?

XAML files in `Views/` are UI. ViewModels in `ViewModels/` hold commands and bindable state. Services in `Services/` handle database and game logic. Models in `Models/` define data.

## 10. What should be tested before submission?

- Register and login
- New game
- Story choice result
- Inventory add/use/delete
- Journal create/edit/delete
- Save/load slot
- Backup export/import
- Build on Android emulator

## Extra: Why not use an online server?

The project goal is beginner-friendly local CRUD and mobile development. SQLite is enough for offline gameplay and easier to present for class.

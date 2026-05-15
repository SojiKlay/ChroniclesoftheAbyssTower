# PROJECT_STATUS.md

Last Updated: 2026-05-15
Project: Chronicles of the Abyss Tower

## Purpose

ไฟล์นี้ใช้เป็น checkpoint ให้ AI หรือผู้พัฒนาที่เปิดโปรเจกต์บนเครื่องอื่นเข้าใจทันทีว่าโปรเจกต์ทำถึงไหนแล้ว ควรอ่านไฟล์นี้ร่วมกับ `AGENTS.md` ก่อนเริ่มแก้ไขงานต่อ

## Current Status

โปรเจกต์เป็นแอป .NET MAUI text-based dark fantasy RPG ใช้ SQLite และ MVVM ระบบหลักสำหรับส่งอาจารย์ทำไว้ครบหลายส่วนแล้ว ได้แก่ login/register, story choice, inventory CRUD, journal CRUD, save/load, backup, settings และ audio service

## Confirmed From Current Files

- Target frameworks: `net10.0-android` และ `net10.0-windows10.0.19041.0`
- Root namespace: `ChroniclesoftheAbyssTower`
- Database file: `abyss_tower.db3`
- Total floors constant: 30
- Max save slots: 3
- Max inventory size: 30
- Raw story data: `ChroniclesoftheAbyssTower/Resources/Raw/floors.json`
- Raw item seed: `ChroniclesoftheAbyssTower/Resources/Raw/items.json`
- Raw story journal seed: `ChroniclesoftheAbyssTower/Resources/Raw/story_journals.json`
- `floors.json`: 30 floors, floor 1 ถึง floor 30
- `items.json`: 12 items
- `story_journals.json`: 11 story journals

## Implemented Screens

- `LoginPage`
- `RegisterPage`
- `MainMenuPage`
- `IntroStoryPage`
- `StoryPage`
- `CharacterPage`
- `InventoryPage`
- `JournalPage`
- `JournalEditorPage`
- `SaveLoadPage`
- `BackupPage`
- `SettingsPage`
- `EndingPage`
- `MainPage` placeholder/default page

## Implemented Models

- `User`
- `Player`
- `Item`
- `InventoryItem`
- `Journal`
- `SaveData`
- `StoryProgress`
- `StoryEvent`
- `StoryChoice`
- `StoryJournalSeed`
- `BackupData`

## Implemented Services

- `DatabaseService`: SQLite connection, table creation, generic CRUD
- `SeedDataService`: loads raw JSON and seeds item master data
- `AuthService`: login/register/session flow
- `PlayerService`: player creation and stat/progress updates
- `StoryService`: floor event loading and choice outcome logic
- `InventoryService`: inventory CRUD/search/filter/item use
- `JournalService`: story journal unlock and player journal CRUD
- `SaveLoadService`: 3-slot save/load using JSON snapshots
- `BackupService`: export/import backup data as JSON
- `SettingsService`: stores preferences
- `AudioService`: audio-related app service
- `ValidationService`: validates username/password/player name inputs

## Current Database Tables

`DatabaseService.InitializeAsync()` creates these SQLite tables:
- `Users`
- `Players`
- `Items`
- `InventoryItems`
- `Journals`
- `SaveData`
- `StoryProgress`

## Main Game Flow

1. User opens app
2. Login or Register
3. Main Menu
4. New Game creates/loads active player
5. Intro Story
6. StoryPage loads current floor event from `floors.json`
7. Player selects choices that affect HP, Gold, EXP, items, journal unlocks and floor progression
8. Player can open Character, Inventory, Journal, Save/Load, Backup and Settings pages
9. Final choices lead to EndingPage

## Recently Important Notes

- Visual Studio lock icons in Solution Explorer are source control/read-only status indicators and normally do not affect running the app
- Visual Studio may show a warning when a file has unsaved editor changes and was also changed externally; choose carefully to avoid losing unsaved edits
- Opening image files may show a binary/unsupported format warning; this is normal for images if Visual Studio tries to open them as text
- `floors.json` is actively important; validate JSON after editing story data

## Known Risks / Things To Check Next

- Run a full build on Android emulator after documentation or resource changes
- Test login/register on a clean app install
- Test New Game -> Intro -> Story floor 1
- Test at least one item reward and one required-item choice
- Test Inventory CRUD: add/use/delete item
- Test Journal CRUD: create/edit/delete player journal and unlock story journal
- Test Save/Load all 3 slots
- Test Backup export/import if the target device permits file sharing
- Check that all `ImageFile` names in `floors.json` exist in `Resources/Images`

## Recommended Next Development Tasks

1. Validate every floor choice from floor 1 ถึง floor 30
2. Check ending conditions and ending text
3. Polish image assets and ensure each image is included as `MauiImage`
4. Run `dotnet build` or Visual Studio build before submission
5. Prepare demo script for teacher: Login -> New Game -> Story choice -> Inventory CRUD -> Journal CRUD -> Save/Load

## Rules For Future AI Work

- Read `AGENTS.md` first
- Keep namespaces unchanged
- Keep MVVM pattern
- Do not replace SQLite with an online backend
- Do not add complex RPG systems unless explicitly requested
- Prefer small beginner-friendly changes
- If editing raw JSON, preserve valid JSON and verify count/structure afterwards

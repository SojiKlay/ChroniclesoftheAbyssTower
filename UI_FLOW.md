# UI FLOW
Chronicles of the Abyss Tower

==================================================
Main UI Flow
==================================================

Main Menu
→ Intro Story
→ Story Screen
→ Character Screen
→ Inventory Screen
→ Journal Screen
→ Ending Screen

==================================================
1. Main Menu
==================================================

Purpose:
หน้าหลักของเกม

Features:
- New Game
- Continue
- Delete Save
- Exit

Navigation:
Main Menu
→ Intro Story
→ Load Save

==================================================
2. Intro Story Screen
==================================================

Purpose:
เล่าเนื้อเรื่องเริ่มต้น

Features:
- Story Text
- Continue Button

Navigation:
Intro Story
→ Floor 1

==================================================
3. Story Screen
==================================================

Purpose:
หน้าหลักของการเล่นเกม

Features:
- Story Text
- Player HP
- Gold
- Current Floor
- Choice Buttons

Bottom Navigation:
- Character
- Inventory
- Journal

Navigation:
Story Screen
→ Next Event
→ Boss Event
→ Ending

==================================================
4. Character Screen
==================================================

Purpose:
แสดงค่าสถานะตัวละคร

Features:
- HP
- MaxHP
- Attack
- Defense
- Gold
- Level

Navigation:
Character Screen
→ Back to Story

==================================================
5. Inventory Screen
==================================================

Purpose:
แสดง Item ทั้งหมด

Features:
- Use Item
- Delete Item
- Item Details

CRUD:
- Create Item
- Read Item
- Update Quantity
- Delete Item

Navigation:
Inventory
→ Back to Story

==================================================
6. Journal Screen
==================================================

Purpose:
แสดง Journal ทั้งหมด

Features:
- Read Journal
- Create Note
- Edit Note
- Delete Note

CRUD:
- Create Journal
- Read Journal
- Update Journal
- Delete Journal

Navigation:
Journal
→ Back to Story

==================================================
7. Ending Screen
==================================================

Purpose:
แสดง Ending ของเกม

Possible Endings:
- Good Ending
- Bad Ending

Features:
- Ending Text
- Restart Button
- Return to Menu

==================================================
Recommended Navigation Structure
==================================================

AppShell
│
├── MainMenuPage
├── IntroStoryPage
├── StoryPage
├── CharacterPage
├── InventoryPage
├── JournalPage
└── EndingPage

==================================================
Recommended ViewModels
==================================================

ViewModels/
- MainMenuViewModel
- StoryViewModel
- CharacterViewModel
- InventoryViewModel
- JournalViewModel

==================================================
Recommended Services
==================================================

Services/
- NavigationService
- DatabaseService
- SaveService
- InventoryService
- JournalService

==================================================
Recommended .NET MAUI Features
==================================================

- NavigationPage
- Shell Navigation
- CollectionView
- SQLite
- MVVM
- Data Binding

==================================================
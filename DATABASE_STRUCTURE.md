# DATABASE STRUCTURE
Chronicles of the Abyss Tower

==================================================
Database Overview
==================================================

Database:
SQLite

Architecture:
MVVM

Purpose:
- Save Player Data
- Save Inventory
- Save Journal
- Save Progression

==================================================
TABLE: Players
==================================================

Description:
เก็บข้อมูลตัวละคร

Fields:
- PlayerId
- PlayerName
- HP
- MaxHP
- Attack
- Defense
- Gold
- CurrentFloor
- Level

CRUD Usage:

Create
- สร้างตัวละครใหม่

Read
- โหลดข้อมูลตัวละคร

Update
- อัปเดต HP
- อัปเดต Gold
- อัปเดต Level

Delete
- ลบ Save Game

==================================================
TABLE: SaveData
==================================================

Description:
เก็บข้อมูลเซฟเกม

Fields:
- SaveId
- PlayerId
- SaveSlot
- SaveDate

CRUD Usage:

Create
- สร้าง Save ใหม่

Read
- โหลด Save

Update
- Auto Save

Delete
- ลบ Save Slot

==================================================
TABLE: InventoryItems
==================================================

Description:
เก็บ Item ของผู้เล่น

Fields:
- InventoryId
- PlayerId
- ItemName
- ItemType
- Quantity

CRUD Usage:

Create
- ได้รับ Item ใหม่

Read
- เปิด Inventory

Update
- ใช้ Item
- เปลี่ยนจำนวน Item

Delete
- ทิ้ง Item

==================================================
TABLE: PlayerJournals
==================================================

Description:
เก็บ Journal ของผู้เล่น

Fields:
- JournalId
- PlayerId
- FloorNumber
- Title
- Content
- CreatedAt

CRUD Usage:

Create
- เขียน Journal ใหม่

Read
- อ่าน Journal

Update
- แก้ไข Journal

Delete
- ลบ Journal

==================================================
Relationships
==================================================

Players
→ SaveData
→ InventoryItems
→ PlayerJournals

==================================================
Recommended Folder Structure
==================================================

Models/
- Player.cs
- InventoryItem.cs
- Journal.cs
- SaveData.cs

Services/
- DatabaseService.cs
- SaveService.cs
- InventoryService.cs
- JournalService.cs

ViewModels/
- PlayerViewModel.cs
- InventoryViewModel.cs
- JournalViewModel.cs

Views/
- MainMenuPage.xaml
- StoryPage.xaml
- InventoryPage.xaml
- JournalPage.xaml

==================================================
Recommended NuGet Packages
==================================================

- CommunityToolkit.Mvvm
- sqlite-net-pcl
- SQLitePCLRaw.bundle_green

==================================================
Database Flow
==================================================

New Game
→ Create Player
→ Create SaveData
→ Enter Floor

Get Item
→ Create Inventory Item

Use Item
→ Update Quantity

Write Journal
→ Create Journal

Delete Save
→ Delete SaveData

==================================================
# Chronicles of the Abyss Tower

==================================================
Overview
==================================================

Chronicles of the Abyss Tower เป็นเกมแนว
Text-based RPG และ Decision-based Adventure
ที่พัฒนาด้วย .NET MAUI

ผู้เล่นจะรับบทเป็น “อาริน”
อดีตอัศวินที่ถูกกล่าวหาว่าเป็นต้นเหตุของ “หมอกอเวจี”

เป้าหมายของผู้เล่นคือ:
- ปีนหอคอยอเวจี
- ค้นหาความจริง
- ตามหาเอลีน่า
- เอาชีวิตรอด

==================================================
Genre
==================================================

- Text-based RPG
- Decision-based Adventure
- Dark Fantasy

==================================================
Technology
==================================================

Framework:
- .NET MAUI

Language:
- C#

Architecture:
- MVVM

Database:
- SQLite

==================================================
Core Systems
==================================================

1. Story Choice System
- ผู้เล่นเลือกตัวเลือกในแต่ละ Event
- ทางเลือกส่งผลต่อ HP และ Item

--------------------------------------------------

2. HP / Gold System
- HP ใช้สำหรับเอาชีวิตรอด
- Gold ใช้ซื้อ Item

--------------------------------------------------

3. Inventory System
ผู้เล่นสามารถ:
- เก็บ Item
- ใช้ Item
- ทิ้ง Item

รองรับ CRUD:
- Create
- Read
- Update
- Delete

--------------------------------------------------

4. Journal System
ใช้สำหรับ:
- บันทึกเนื้อเรื่อง
- จด Hint
- จดข้อมูลสำคัญ

รองรับ CRUD:
- Create
- Read
- Update
- Delete

--------------------------------------------------

5. Save / Load System
- Save Progress
- Continue Game
- Delete Save

==================================================
Main Screens
==================================================

- Main Menu
- Story Screen
- Character Screen
- Inventory Screen
- Journal Screen
- Ending Screen

==================================================
Project Structure
==================================================

Models/
- Player.cs
- InventoryItem.cs
- Journal.cs
- SaveData.cs

ViewModels/
- StoryViewModel.cs
- InventoryViewModel.cs
- JournalViewModel.cs

Views/
- MainMenuPage.xaml
- StoryPage.xaml
- InventoryPage.xaml
- JournalPage.xaml

Services/
- DatabaseService.cs
- SaveService.cs
- InventoryService.cs
- JournalService.cs

==================================================
Recommended NuGet Packages
==================================================

- CommunityToolkit.Mvvm
- sqlite-net-pcl
- SQLitePCLRaw.bundle_green

==================================================
Project Goal
==================================================

โปรเจคนี้ออกแบบเพื่อ:
- ใช้ส่งอาจารย์
- รองรับ CRUD จริง
- ใช้ Multi-page Application
- ใช้ SQLite จริง
- พัฒนาได้จริงในมือถือ
- ไม่ซับซ้อนเกินไป

==================================================
Game Flow
==================================================

Main Menu
→ New Game
→ Intro Story
→ Enter Tower
→ Story Events
→ Boss Event
→ Ending

==================================================
Possible Endings
==================================================

Good Ending
- ช่วยโลกสำเร็จ

Bad Ending
- ถูกหมอกกลืนกิน

==================================================
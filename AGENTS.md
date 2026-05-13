# AGENTS.md

==================================================
Project Overview
==================================================

Project Name:
Chronicles of the Abyss Tower

Genre:
- Text-based RPG
- Decision-based Adventure
- Dark Fantasy

Technology:
- .NET MAUI
- SQLite
- MVVM
- C#

==================================================
Project Goal
==================================================

โปรเจคนี้ถูกออกแบบเพื่อ:
- ใช้ส่งอาจารย์
- แสดงการใช้งาน CRUD
- ใช้งาน Multi-page Application
- ใช้งาน SQLite Database
- พัฒนาได้จริงบนมือถือ

==================================================
Coding Rules
==================================================

- ใช้โค้ดที่อ่านง่าย
- ใช้ MVVM Architecture
- ใช้ SQLite สำหรับเก็บข้อมูล
- แยกโฟลเดอร์ให้ชัดเจน
- หลีกเลี่ยงระบบที่ซับซ้อนเกินไป
- ใช้ async / await เมื่อจำเป็น
- เน้น Beginner Friendly

==================================================
Project Structure
==================================================

Views/
- XAML Pages

ViewModels/
- MVVM ViewModels

Models/
- Database Models

Services/
- Database Services
- Game Logic Services

==================================================
Main Systems
==================================================

1. Story Choice System
- ระบบเลือกตัวเลือก
- ส่งผลต่อ HP และ Item

--------------------------------------------------

2. Inventory CRUD
รองรับ:
- Create
- Read
- Update
- Delete

--------------------------------------------------

3. Journal CRUD
รองรับ:
- Create
- Read
- Update
- Delete

--------------------------------------------------

4. Save / Load System
ใช้สำหรับ:
- Save Progress
- Continue Game
- Delete Save

==================================================
Main Screens
==================================================

- MainMenuPage
- IntroStoryPage
- StoryPage
- CharacterPage
- InventoryPage
- JournalPage
- EndingPage

==================================================
Recommended .NET MAUI Features
==================================================

- Shell Navigation
- NavigationPage
- Data Binding
- CollectionView
- SQLite
- MVVM Toolkit

==================================================
Recommended NuGet Packages
==================================================

- CommunityToolkit.Mvvm
- sqlite-net-pcl
- SQLitePCLRaw.bundle_green

==================================================
Important Notes
==================================================

- Keep namespaces unchanged
- Avoid advanced RPG systems
- Avoid multiplayer systems
- Avoid online server systems
- Focus on CRUD and Navigation

==================================================
Development Priority
==================================================

1. Navigation System
2. Database System
3. Story Events
4. Inventory CRUD
5. Journal CRUD
6. Save / Load

==================================================
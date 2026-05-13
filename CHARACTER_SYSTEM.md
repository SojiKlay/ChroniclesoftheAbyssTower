# CHARACTER SYSTEM
Chronicles of the Abyss Tower

==================================================
Character Overview
==================================================

ระบบ Character ใช้สำหรับ:
- เก็บข้อมูลตัวละคร
- จัดการค่าสถานะ
- เชื่อมกับ Story
- ใช้กับ SQLite Database

==================================================
Main Character
==================================================

Name:
Arin

Title:
The Cursed Knight

Background:
อดีตอัศวินแห่งราชวงศ์
ผู้ถูกกล่าวหาว่าเป็นต้นเหตุของหมอกอเวจี

Goal:
- ค้นหาความจริง
- ตามหาเอลีน่า
- เอาชีวิตรอดจากหอคอย

==================================================
Character Stats
==================================================

Base Stats:
- HP
- MaxHP
- Attack
- Defense
- Gold
- Level

==================================================
Starting Stats
==================================================

HP:
100

MaxHP:
100

Attack:
10

Defense:
5

Gold:
50

Level:
1

==================================================
Level System
==================================================

Level Up Rewards:
- HP +10
- Attack +2
- Defense +1

==================================================
Gameplay Usage
==================================================

HP
- ใช้สำหรับเอาชีวิตรอด

Attack
- ใช้คำนวณการโจมตี

Defense
- ลดความเสียหาย

Gold
- ใช้ซื้อ Item

Level
- ใช้แสดงความก้าวหน้าของผู้เล่น

==================================================
Character Database Fields
==================================================

Table:
Players

Fields:
- PlayerId
- PlayerName
- HP
- MaxHP
- Attack
- Defense
- Gold
- Level
- CurrentFloor

==================================================
CRUD Integration
==================================================

Create
- Create Character

Read
- Read Character Status

Update
- Update HP
- Update Gold
- Update Level

Delete
- Delete Save Data

==================================================
Character Progression
==================================================

Early Game:
- เรียนรู้ระบบเกม
- เก็บ Item
- เอาชีวิตรอด

Mid Game:
- พบ Boss
- ใช้ Journal
- จัดการ Resource

Late Game:
- Final Boss
- Ending Choice

==================================================
Related Systems
==================================================

- Inventory System
- Story System
- Database Structure
- UI Flow

==================================================
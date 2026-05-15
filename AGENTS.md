# AGENTS.md

## Project Overview

Project Name: Chronicles of the Abyss Tower

Genre:
- Text-based RPG
- Decision-based Adventure
- Dark Fantasy

Technology:
- .NET MAUI 10
- C#
- SQLite
- MVVM
- Shell Navigation

## Project Goal

โปรเจกต์นี้ทำเพื่อส่งอาจารย์และแสดงการใช้งานจริงของ:
- CRUD
- Multi-page Application
- SQLite Database
- MVVM Architecture
- Save / Load
- การพัฒนาแอปมือถือด้วย .NET MAUI

## Current Implementation Snapshot

ข้อมูลสถานะล่าสุดแบบละเอียดอยู่ใน `PROJECT_STATUS.md` ให้ AI อ่านไฟล์นั้นก่อนเริ่มแก้ไขงานต่อ

สถานะปัจจุบันโดยย่อ:
- มีระบบ Login / Register / Session
- มีระบบ Story Choice จาก `Resources/Raw/floors.json`
- มี 30 floors
- มี item seed 12 รายการจาก `Resources/Raw/items.json`
- มี story journal seed 11 รายการจาก `Resources/Raw/story_journals.json`
- มี Inventory CRUD
- มี Journal CRUD แยก Story Journal และ Player Journal
- มี Save / Load 3 slots
- มี Backup export/import JSON
- มี Settings และ AudioService

## Coding Rules

- ใช้โค้ดที่อ่านง่าย
- ใช้ MVVM Architecture
- ใช้ SQLite สำหรับเก็บข้อมูล
- แยกโฟลเดอร์ให้ชัดเจน
- หลีกเลี่ยงระบบที่ซับซ้อนเกินไป
- ใช้ async / await เมื่อทำงานกับ database, file, navigation หรือ service
- เน้น Beginner Friendly
- Keep namespaces unchanged
- หลีกเลี่ยง multiplayer, online server, cloud backend ที่ซับซ้อน
- ถ้าเพิ่ม feature ใหม่ ให้ลงทะเบียน service/page/viewmodel ใน `MauiProgram.Registrations.cs` ตาม pattern เดิม

## Project Structure

`Views/`
- XAML Pages

`ViewModels/`
- MVVM ViewModels

`Models/`
- Database Models และ data models

`Services/`
- Database Services
- Game Logic Services
- Auth, Save/Load, Backup, Settings, Audio

`Resources/Raw/`
- `floors.json`
- `items.json`
- `story_journals.json`

`Resources/Images/`
- รูปประกอบหน้า story, ending, item, background

## Main Systems

1. Auth System
- Login
- Register
- Session storage

2. Story Choice System
- โหลด event จาก JSON
- ตัวเลือกส่งผลต่อ HP, Gold, EXP, Item, Journal, Ending
- รองรับ RequiredItem, RequiredItems, RequiredItemNoConsume, RequiresItem, RequiresItems, BlockedByItem, BlockedByItems

3. Inventory CRUD
- Create: เพิ่ม item
- Read: แสดง inventory/search/filter
- Update: ใช้ item หรือปรับ quantity
- Delete: ลบ item

4. Journal CRUD
- Story Journal ปลดล็อกจากเนื้อเรื่อง
- Player Journal สร้าง/แก้ไข/ลบได้

5. Save / Load System
- 3 save slots ต่อ user
- เก็บ snapshot เป็น JSON
- Continue Game
- Delete Save

6. Backup System
- Export/import ข้อมูล user เป็นไฟล์ JSON

7. Settings / Audio
- BGM/SFX preference
- AudioService ใช้กับเสียงในแอป

## Main Screens

- LoginPage
- RegisterPage
- MainMenuPage
- IntroStoryPage
- StoryPage
- CharacterPage
- InventoryPage
- JournalPage
- JournalEditorPage
- SaveLoadPage
- BackupPage
- SettingsPage
- EndingPage
- MainPage placeholder

## Development Priority

1. Keep app buildable on Android emulator
2. Keep database schema and seed data consistent
3. Preserve CRUD and navigation demonstration for grading
4. Improve story/resources only after testing JSON validity
5. Avoid large architecture rewrites unless required

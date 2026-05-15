# PROJECT OVERVIEW

## Overview

Chronicles of the Abyss Tower เป็นแอปเกม Text-based Dark Fantasy RPG ที่เน้นการตัดสินใจของผู้เล่น พัฒนาด้วย .NET MAUI และ SQLite เพื่อแสดง CRUD, multi-page navigation และ MVVM สำหรับงานส่งอาจารย์

## Player Role

ผู้เล่นรับบทเป็น Arin อดีตอัศวินผู้ถูกกล่าวหาว่าเกี่ยวข้องกับหมอกอเวจี เขาต้องปีน Abyss Tower เพื่อค้นหาความจริงและตามหา Eleanor

## Core Gameplay

- อ่านเนื้อเรื่องแต่ละชั้น
- เลือกตัวเลือกที่มีผลต่อ HP, Gold, EXP, Item และ Ending
- เก็บและใช้ item
- อ่าน story journal และเขียน player journal
- save/load ความคืบหน้า

## Implemented Scope

- Auth: Login/Register/Session
- Database: SQLite local database
- Story: 30 floors จาก `floors.json`
- Items: 12 seed items จาก `items.json`
- Journals: 11 story journal seeds จาก `story_journals.json`
- Inventory CRUD
- Journal CRUD
- Save/Load 3 slots
- Backup export/import
- Settings and AudioService

## Design Goals

- Beginner friendly
- โครงสร้างชัดเจนตาม MVVM
- ใช้งานจริงบนมือถือได้
- ไม่พึ่ง online server
- โชว์ CRUD และ navigation ได้ชัดเจน

## Main Flow

```text
Login/Register
-> Main Menu
-> New Game / Continue
-> Intro Story
-> Story Floor Events
-> Inventory / Journal / Character / Save / Settings
-> Ending
```

## Important Files

- `AGENTS.md`: กติกาสำหรับ AI และผู้พัฒนา
- `PROJECT_STATUS.md`: สถานะล่าสุดของโปรเจกต์
- `ChroniclesoftheAbyssTower/ChroniclesoftheAbyssTower.csproj`: target framework และ NuGet packages
- `ChroniclesoftheAbyssTower/MauiProgram.Registrations.cs`: DI registration
- `ChroniclesoftheAbyssTower/AppShell.xaml.cs`: route registration
- `ChroniclesoftheAbyssTower/Resources/Raw/floors.json`: story floors
- `ChroniclesoftheAbyssTower/Resources/Raw/items.json`: item master data
- `ChroniclesoftheAbyssTower/Resources/Raw/story_journals.json`: story journal seed

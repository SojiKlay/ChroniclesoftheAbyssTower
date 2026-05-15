# DATABASE STRUCTURE

## Database Overview

Database: SQLite

Database file name: `abyss_tower.db3`

Main service: `ChroniclesoftheAbyssTower/Services/DatabaseService.cs`

`DatabaseService.InitializeAsync()` creates the current tables and exposes generic CRUD helpers.

## Current Tables

- `Users`
- `Players`
- `Items`
- `InventoryItems`
- `Journals`
- `SaveData`
- `StoryProgress`

## Users

Model: `User.cs`

Purpose:
- เก็บบัญชีผู้ใช้
- ใช้กับ Login/Register
- เก็บ password hash และ salt

Important fields:
- `UserId`
- `Username`
- `PasswordHash`
- `Salt`
- `CreatedAt`
- `LastLoginAt`

## Players

Model: `Player.cs`

Purpose:
- เก็บตัวละครของ user
- ใช้กับ Character, Story, Save/Load

Important fields:
- `PlayerId`
- `UserId`
- `PlayerName`
- `Hp`, `MaxHp`
- `Attack`, `Defense`
- `Gold`, `Level`, `Experience`
- `CurrentFloor`, `HighestFloor`
- `TotalChoicesMade`
- `IsGameCompleted`, `EndingType`
- `CreatedAt`, `UpdatedAt`

## Items

Model: `Item.cs`

Purpose:
- Master data ของ item ทั้งหมด
- Seed จาก `Resources/Raw/items.json`

Important fields:
- `ItemId`
- `ItemName`
- `ThaiName`
- `ItemType`
- `Description`
- `EffectValue`
- `IconKey`
- `ShopPrice`
- `IsConsumable`

## InventoryItems

Model: `InventoryItem.cs`

Purpose:
- เก็บ item ที่ player ถืออยู่

Important fields:
- `InventoryId`
- `PlayerId`
- `ItemId`
- `Quantity`
- `AcquiredAt`

Runtime helper:
- `ItemData` เป็น ignored/navigation property สำหรับข้อมูล item master

## Journals

Model: `Journal.cs`

Purpose:
- เก็บ Story Journal และ Player Journal

Important fields:
- `JournalId`
- `PlayerId`
- `JournalType`
- `FloorNumber`
- `Title`
- `Content`
- `StoryKey`
- `CreatedAt`
- `UpdatedAt`

## SaveData

Model: `SaveData.cs`

Purpose:
- เก็บ save slot 3 ช่องต่อ user
- ใช้ snapshot JSON สำหรับ load state กลับมา

Important fields:
- `SaveId`
- `UserId`
- `SaveSlot`
- `SaveName`
- `SaveDate`
- `PlayerSnapshot`
- `InventorySnapshot`
- `JournalSnapshot`
- `ProgressSnapshot`
- `PlayerName`
- `CurrentFloor`
- `Hp`
- `Level`
- `PlayTimeSeconds`

## StoryProgress

Model: `StoryProgress.cs`

Purpose:
- บันทึก floor ที่ผ่านแล้วและ choice ที่เลือก

Important fields:
- `ProgressId`
- `PlayerId`
- `FloorNumber`
- `IsCompleted`
- `ChoiceMade`
- `ChoiceText`
- `CompletedAt`

## Seed Data

Service: `SeedDataService.cs`

Raw files:
- `floors.json`: 30 floors
- `items.json`: 12 items
- `story_journals.json`: 11 story journal seeds

`SeedAllAsync()` initializes database and seeds item master data.

## Relationships

```text
User 1 -> many Players
User 1 -> up to 3 SaveData slots
Player 1 -> many InventoryItems
InventoryItem many -> 1 Item
Player 1 -> many Journals
Player 1 -> many StoryProgress rows
```

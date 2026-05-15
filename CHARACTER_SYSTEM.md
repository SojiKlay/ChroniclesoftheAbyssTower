# Character System

## Overview

Character system stores the player character state in SQLite and exposes it to Story, Character, Save/Load and Ending systems.

Main character:
- Name: Arin
- Role: อดีตอัศวินผู้ขึ้น Abyss Tower เพื่อค้นหาความจริงและตามหา Eleanor

## Starting Stats

Defined in `AppConstants.cs` and `Player.cs` defaults:

- HP: 100
- Max HP: 100
- Attack: 10
- Defense: 5
- Gold: 50
- Level: 1
- Current Floor: 1

## Current Player Fields

Model: `Player.cs`

Important fields:
- `PlayerId`
- `UserId`
- `PlayerName`
- `Hp`
- `MaxHp`
- `Attack`
- `Defense`
- `Gold`
- `Level`
- `Experience`
- `CurrentFloor`
- `HighestFloor`
- `TotalChoicesMade`
- `IsGameCompleted`
- `EndingType`
- `CreatedAt`
- `UpdatedAt`

## Service

`PlayerService.cs` handles:
- Creating a new player
- Loading active/latest player
- Updating HP, Gold, EXP and progress
- Advancing floor
- Completing game/ending state

## UI

Page:
- `CharacterPage.xaml`

ViewModel:
- `CharacterViewModel.cs`

## CRUD Role

Create:
- New game creates player

Read:
- Character page reads player status

Update:
- Story choices update HP, Gold, EXP and floor

Delete:
- Save/new game cleanup can remove player-related data when needed

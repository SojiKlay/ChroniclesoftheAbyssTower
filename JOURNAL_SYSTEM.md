# Journal System

## Overview

Journal system supports both story journals and player-created notes. It demonstrates CRUD clearly while also connecting to story progression.

## Current Data

Story journal seed file:
- `ChroniclesoftheAbyssTower/Resources/Raw/story_journals.json`

Current seed count:
- 11 story journals

## Journal Types

1. Story Journal
- Unlocked by story events
- Usually read-only for the player
- Uses `StoryKey` to prevent duplicate unlocks

2. Player Journal
- Created by player
- Editable
- Deletable
- Used to demonstrate CRUD

## Model

Model file:
- `Journal.cs`

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

## CRUD

Service:
- `JournalService.cs`

Create:
- `CreatePlayerJournalAsync`
- `UnlockStoryJournalAsync`

Read:
- Get journals by player
- Search journals
- Separate story/player journal views

Update:
- Update player journal
- Story journals should remain protected/read-only

Delete:
- Delete player journal

## UI

Pages:
- `JournalPage.xaml`
- `JournalEditorPage.xaml`

ViewModels:
- `JournalViewModel.cs`
- `JournalEditorViewModel.cs`

## Story Integration

Story events and choices can unlock journal entries through story journal keys from `story_journals.json`.

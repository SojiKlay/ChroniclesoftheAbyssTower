# Story

## Premise

โลกถูกคุกคามโดยหมอกอเวจี ผู้คนค่อย ๆ สูญหายและความทรงจำถูกกลืนกิน Arin อดีตอัศวินถูกกล่าวหาว่าเป็นต้นเหตุของหายนะ เขาจึงต้องปีน Abyss Tower เพื่อค้นหาความจริงและตามหา Eleanor

## Current Story Implementation

Story content used by the app is mainly stored in:

- `ChroniclesoftheAbyssTower/Resources/Raw/floors.json`

Current state:
- 30 floors
- Floor 1 ถึง Floor 30
- Each floor has narrative text and choices
- Choices can change HP, Gold, EXP, item inventory, story journal unlocks and ending state

## Core Themes

- ความผิดและการไถ่บาป
- ความทรงจำที่ถูกหมอกกลืนกิน
- การเอาชีวิตรอดในหอคอย
- การตามหา Eleanor
- ความจริงที่ซ่อนอยู่บนชั้นสูงสุด

## Story Data Structure

Each `StoryEvent` can contain:
- `FloorNumber`
- `Title`
- `Narrative`
- `EventType`
- `Icon`
- `ImageFile`
- `StoryJournalKey`
- `BossHp`
- `BossAttack`
- `BossName`
- `IsFinalFloor`
- `Choices`

Each `StoryChoice` can contain:
- `Text`
- `ResultText`
- `RepeatResultText`
- `HpDelta`
- `GoldDelta`
- `ExpDelta`
- `ItemReward`
- `ItemRewardQuantity`
- item requirement/block fields
- `AdvanceFloor`
- `EndingType`

## Current Endings

The story system supports ending selection through `EndingType`, commonly:
- `Good`
- `Bad`

If story JSON uses another ending string, update `EndingViewModel` and documentation together.

## Editing Rule

When editing story content:
- Keep JSON valid
- Keep floor numbers sequential unless intentionally changing progression
- Ensure `ImageFile` exists in `Resources/Images`
- Ensure item names match item master data in `items.json`
- Ensure story journal keys match `story_journals.json`

# NPC SYSTEM

## Overview

NPCs are currently represented inside story events rather than a separate NPC database table. There is no `NPC` model/table in the current codebase.

This file documents NPC concepts used by `floors.json` story content.

## Current Implementation

NPC-related content is stored in:
- `Resources/Raw/floors.json`

Examples can appear as:
- floor title
- narrative text
- choice text
- result text
- boss fields such as `BossName`, `BossHp`, `BossAttack`

## NPC Categories In Story

- Merchant: shop-like choices and item exchange
- Survivor: lore, hints and moral choices
- Boss/Gatekeeper: combat or boss event content
- Eleanor-related memory/vision: story and ending context

## Important Note

Do not document or build an `NPCs` SQLite table unless the project explicitly adds an `NPC.cs` model and table creation in `DatabaseService.cs`.

For the current beginner-friendly scope, keeping NPCs as story data is simpler and consistent with the project goal.

## Related Systems

- `StoryService.cs`
- `StoryEvent.cs`
- `StoryChoice.cs`
- `floors.json`
- `JournalService.cs`

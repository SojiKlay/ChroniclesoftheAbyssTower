# Item System

## Overview

Item system supports story rewards, healing, key items, inventory CRUD and shop-like story events.

Current raw seed file:
- `ChroniclesoftheAbyssTower/Resources/Raw/items.json`

Current seed count:
- 12 items

## Main Models

`Item.cs`
- Master data of each item
- Seeded into SQLite table `Items`

`InventoryItem.cs`
- Player-owned item instance
- Stores `PlayerId`, `ItemId`, `Quantity`, `AcquiredAt`

## Item Fields

- `ItemId`
- `ItemName`
- `ThaiName`
- `ItemType`
- `Description`
- `EffectValue`
- `IconKey`
- `ShopPrice`
- `IsConsumable`

## Inventory CRUD

Service: `InventoryService.cs`

Create:
- Add item reward to player inventory
- Stack quantity if same item already exists

Read:
- Get inventory by player
- Search item
- Filter by item type

Update:
- Update quantity
- Consume item

Delete:
- Delete inventory item
- Remove item when quantity reaches 0

## Story Integration

Story choices in `floors.json` can use:
- `ItemReward`
- `ItemRewardQuantity`
- `RequiredItem`
- `RequiredItems`
- `RequiredItemNoConsume`
- `RequiresItem`
- `RequiresItems`
- `BlockedByItem`
- `BlockedByItems`

## UI

Main page:
- `InventoryPage.xaml`

ViewModel:
- `InventoryViewModel.cs`

Expected demo:
- Gain item from story
- View item in inventory
- Use item if consumable
- Delete item

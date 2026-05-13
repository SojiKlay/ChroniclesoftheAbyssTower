# NPC SYSTEM
Chronicles of the Abyss Tower

==================================================
NPC Overview
==================================================

NPC ใช้สำหรับ:
- ดำเนินเนื้อเรื่อง
- ให้ข้อมูลสำคัญ
- ขาย Item
- ให้ Hint เกี่ยวกับหอคอย

==================================================
NPC Categories
==================================================

1. Merchant
2. Survivor
3. Boss

==================================================
Merchant
==================================================

Name:
Masked Merchant

Floor:
6

Role:
ขาย Item ให้ผู้เล่น

Items:
- Health Potion
- Rune Key

Purpose:
- แนะนำระบบ Shop
- แนะนำการใช้ Gold

==================================================
Survivor
==================================================

Name:
Lost Survivor

Floor:
15

Role:
ให้ Hint เกี่ยวกับหอคอย

Choices:
- ช่วยเหลือ
- เดินผ่าน

Rewards:
- Journal Hint
- Gold

Purpose:
- เพิ่มเนื้อเรื่อง
- เพิ่มการตัดสินใจ

==================================================
Boss
==================================================

Name:
Gatekeeper

Floor:
10

Role:
Mini Boss ของช่วง Tutorial

Choices:
- สู้
- ใช้ Potion
- หนี

Rewards:
- Gold
- Unlock Next Floor

Purpose:
- แนะนำระบบ Boss Event

==================================================
NPC Database Fields
==================================================

Table:
NPCs

Fields:
- NPCId
- NPCName
- NPCType
- FloorNumber
- Dialogue

==================================================
Recommended Usage in Game
==================================================

Merchant
→ ใช้ใน Story Event
→ ซื้อ Item

Survivor
→ ใช้เพิ่ม Lore
→ ให้ Hint

Boss
→ ใช้เป็น Event สำคัญ

==================================================
Related Systems
==================================================

- Story System
- Inventory System
- Journal System
- Database Structure

==================================================
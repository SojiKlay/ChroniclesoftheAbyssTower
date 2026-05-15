# Chronicles of the Abyss Tower

Chronicles of the Abyss Tower เป็นเกม Text-based RPG / Decision-based Adventure แนว Dark Fantasy พัฒนาด้วย .NET MAUI, C#, SQLite และ MVVM

ผู้เล่นรับบทเป็น Arin อดีตอัศวินที่ต้องขึ้นหอคอย Abyss Tower เพื่อค้นหาความจริง ตามหา Eleanor และเอาชีวิตรอดจากหมอกอเวจี

## Current Project Status

อ่านสถานะล่าสุดที่ `PROJECT_STATUS.md`

สถานะโดยย่อ:
- โปรเจกต์ .NET MAUI 10
- รองรับ Android และ Windows target
- ใช้ SQLite local database
- ใช้ MVVM + Shell Navigation
- มีระบบ Login/Register
- มี story data 30 floors
- มี item seed 12 รายการ
- มี story journal seed 11 รายการ
- มี Inventory CRUD
- มี Journal CRUD
- มี Save/Load 3 slots
- มี Backup export/import JSON
- มี Settings และ AudioService

## Main Systems

1. Auth System
- Register
- Login
- Session

2. Story Choice System
- โหลด floor event จาก `Resources/Raw/floors.json`
- ตัวเลือกมีผลต่อ HP, Gold, EXP, Item, Journal และ Ending

3. Inventory CRUD
- เพิ่ม item
- แสดง inventory
- ใช้ item / ปรับจำนวน
- ลบ item

4. Journal CRUD
- Story Journal จากเนื้อเรื่อง
- Player Journal ที่ผู้เล่นสร้างเอง

5. Save / Load
- 3 save slots ต่อ user
- Save snapshot เป็น JSON

6. Backup
- Export/import ข้อมูลเป็น JSON file

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

## Project Structure

```text
ChroniclesoftheAbyssTower/
  Helpers/
  Models/
  Services/
  ViewModels/
  Views/
  Resources/
    Images/
    Raw/
      floors.json
      items.json
      story_journals.json
```

## Technology

- .NET MAUI 10
- C#
- XAML
- SQLite
- CommunityToolkit.Mvvm
- sqlite-net-pcl
- SQLitePCLRaw.bundle_green
- Newtonsoft.Json
- Plugin.Maui.Audio

## Run Notes

เปิด solution ด้วย Visual Studio แล้วเลือก Android emulator เช่น Pixel API 36 จากนั้นกด Run

ถ้า Visual Studio แสดงไอคอน lock ใน Solution Explorer โดยทั่วไปเป็นสถานะ Git/source control ไม่ใช่ error และไม่กระทบการรันแอป

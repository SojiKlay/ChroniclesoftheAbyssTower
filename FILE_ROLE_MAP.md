# File Role Map

เอกสารนี้อธิบายว่าแต่ละไฟล์ในโปรเจกต์มีหน้าที่อะไร และเชื่อมกับไฟล์ไหนบ้าง เหมาะสำหรับเตรียมตอบอาจารย์เวลาเปิด Solution Explorer แล้วไล่ถามทีละไฟล์

## ภาพรวมการเชื่อมกัน

```text
Views/*.xaml
    -> แสดง UI
Views/*.xaml.cs
    -> ตั้ง BindingContext และเรียก OnAppearing
ViewModels/*.cs
    -> เก็บ state ของหน้า และรับคำสั่งจากปุ่ม
Services/*.cs
    -> ทำ business logic และคุยกับ database/file/json/preferences
Models/*.cs
    -> โครงสร้างข้อมูลและ SQLite table
Resources/Raw/*.json
    -> ข้อมูลเนื้อเรื่อง, item, journal seed
Resources/Images/*
    -> รูปที่ UI แสดง
```

## Root App Files

### `ChroniclesoftheAbyssTower.csproj`

หน้าที่:

- ไฟล์ project ของ .NET MAUI
- กำหนด target framework เช่น Android และ Windows
- กำหนด resource เช่น image, raw file, font, package reference

เชื่อมกับ:

- `Resources/Images/*`
- `Resources/Raw/*`
- `Platforms/*`
- package ที่ใช้ใน service/viewmodel เช่น SQLite, CommunityToolkit

ถ้าอาจารย์ถาม:

> ไฟล์นี้สำคัญยังไง?

ตอบว่า:

> เป็นไฟล์ config หลักของโปรเจกต์ครับ ใช้บอกว่าแอป build ไป platform ไหน ใช้ resource อะไร และอ้าง package อะไรบ้าง

### `MauiProgram.cs`

หน้าที่:

- จุดเริ่มต้นการตั้งค่า MAUI app
- สร้าง `MauiApp`
- เรียก extension method เพื่อลงทะเบียน service/page/viewmodel

เชื่อมกับ:

- `MauiProgram.Registrations.cs`
- `App.xaml.cs`
- `Services/*`
- `Views/*`
- `ViewModels/*`

### `MauiProgram.Registrations.cs`

หน้าที่:

- ลงทะเบียน Dependency Injection
- แยก `RegisterServices`, `RegisterViewModels`, `RegisterPages`

เชื่อมกับ:

- Services ทุกไฟล์
- ViewModels ทุกไฟล์
- Views ทุกไฟล์

เหตุผลที่แยกไฟล์:

- ทำให้ `MauiProgram.cs` ไม่ยาวเกินไป
- ถ้าเพิ่มหน้าใหม่หรือ service ใหม่ ให้เพิ่มที่ไฟล์นี้ตาม pattern เดิม

### `App.xaml`

หน้าที่:

- ประกาศ resource ระดับแอป
- โหลด style, color, converter ที่ใช้ทั้งแอป

เชื่อมกับ:

- `Resources/Styles/Colors.xaml`
- `Resources/Styles/Styles.xaml`
- `Helpers/Converters/*`

### `App.xaml.cs`

หน้าที่:

- Entry point ฝั่ง runtime ของแอป
- เก็บ `IServiceProvider` ไว้ใน `App.Services`
- สร้าง window หลักด้วย `AppShell`
- จับ global exception

เชื่อมกับ:

- `AppShell.xaml`
- `AppShell.xaml.cs`
- `MauiProgram.cs`

### `AppShell.xaml`

หน้าที่:

- กำหนด Shell หลักของแอป
- กำหนด route หลัก: login, register, main
- ปิด flyout และ nav bar

เชื่อมกับ:

- `Views/LoginPage.xaml`
- `Views/RegisterPage.xaml`
- `Views/MainMenuPage.xaml`
- `AppShell.xaml.cs`

### `AppShell.xaml.cs`

หน้าที่:

- Register route ของหน้ารอง เช่น Story, Inventory, Journal, SaveLoad
- seed ข้อมูลตอนเปิดแอป
- ตรวจ session ถ้า login อยู่ให้ไป Main Menu
- เพิ่มเสียงปุ่ม default ให้ button ทั้งแอป

เชื่อมกับ:

- `Helpers/AppConstants.cs`
- `Helpers/SessionManager.cs`
- `Services/SeedDataService.cs`
- `Services/AudioService.cs`
- `Views/*`

### `MainPage.xaml` และ `MainPage.xaml.cs`

หน้าที่:

- หน้า placeholder/default จาก template
- ใน flow จริงของแอปไม่ได้เป็นหน้าหลัก เพราะใช้ `AppShell` กับ Login/MainMenu

เชื่อมกับ:

- ไม่ได้เป็น core flow หลัก

## Helpers

### `Helpers/AppConstants.cs`

หน้าที่:

- เก็บค่าคงที่กลางของแอป
- เช่น database file name, route name, starting stat, max save slot, setting key

เชื่อมกับ:

- `DatabaseService`
- `PlayerService`
- `MainMenuViewModel`
- `StoryViewModel`
- `SaveLoadService`
- `BackupService`
- `SettingsService`
- ทุกไฟล์ที่ต้องใช้ route หรือค่าคงที่

เหตุผล:

- ลด magic number/string
- แก้ค่ากลางได้ที่เดียว

### `Helpers/SessionManager.cs`

หน้าที่:

- เก็บ session ของผู้ใช้ที่ login
- เก็บ active player id
- ใช้ Preferences/Secure storage pattern สำหรับจำสถานะ

เชื่อมกับ:

- `AuthService`
- `PlayerService`
- `MainMenuViewModel`
- `SaveLoadService`
- `BackupService`
- `AppShell.xaml.cs`

### `Helpers/PasswordHasher.cs`

หน้าที่:

- สร้าง salt
- hash password
- verify password ตอน login

เชื่อมกับ:

- `AuthService`
- `Models/User.cs`

### `Helpers/AnimationHelper.cs`

หน้าที่:

- helper สำหรับ animation ใน UI
- ใช้ลดการเขียน animation ซ้ำ ถ้าหน้าไหนต้องทำเอฟเฟกต์

เชื่อมกับ:

- หน้า UI ที่เรียกใช้ animation

## Converters

### `Helpers/Converters/StringNotEmptyConverter.cs`

หน้าที่:

- แปลง string เป็น bool
- ใช้กับ `IsVisible` เช่น ถ้ามี error message ค่อยแสดง label

เชื่อมกับ:

- `App.xaml`
- `LoginPage.xaml`
- หน้าอื่นที่ต้องเช็ก string ว่าง/ไม่ว่าง

### `Helpers/Converters/InverseBoolConverter.cs`

หน้าที่:

- แปลง bool เป็นค่าตรงข้าม
- เช่น true -> false, false -> true

เชื่อมกับ:

- `App.xaml`
- XAML ที่ต้องกลับค่าการแสดงผลหรือ enabled

### `Helpers/Converters/HpToColorConverter.cs`

หน้าที่:

- แปลงค่า HP เป็นสี
- เช่น HP ต่ำอาจเป็นแดง HP สูงเป็นเขียว/ทอง

เชื่อมกับ:

- XAML ที่แสดง HP หรือ status สี

## Models

### `Models/User.cs`

หน้าที่:

- ตาราง `Users`
- เก็บ username, password hash, salt, created date, last login

เชื่อมกับ:

- `AuthService`
- `DatabaseService`
- `BackupService`
- `BackupData`

### `Models/Player.cs`

หน้าที่:

- ตาราง `Players`
- เก็บตัวละครของผู้ใช้ เช่น HP, MaxHP, Attack, Defense, Gold, Level, Experience, CurrentFloor, EndingType

เชื่อมกับ:

- `PlayerService`
- `StoryService`
- `StoryViewModel`
- `CharacterViewModel`
- `InventoryViewModel`
- `SaveLoadService`
- `BackupService`

### `Models/Item.cs`

หน้าที่:

- ตาราง `Items`
- เป็น master data ของ item เช่น ชื่อ, ชื่อไทย, ประเภท, description, effect, icon, price

เชื่อมกับ:

- `SeedDataService` อ่านจาก `items.json` แล้ว seed ลงตารางนี้
- `InventoryService`
- `InventoryItem`

### `Models/InventoryItem.cs`

หน้าที่:

- ตาราง `InventoryItems`
- เก็บ item ที่ player ถือจริง
- มี `PlayerId`, `ItemId`, `Quantity`
- มี property `[Ignore]` เช่น `DisplayName`, `DisplayImage`, `ItemData` สำหรับ UI

เชื่อมกับ:

- `InventoryService`
- `InventoryViewModel`
- `InventoryPage.xaml`
- `StoryService`
- `SaveLoadService`
- `BackupService`
- `Item.cs`

### `Models/Journal.cs`

หน้าที่:

- ตาราง `Journals`
- เก็บ Story Journal และ Player Journal
- ใช้ enum `JournalType` แยก `Story` กับ `Player`

เชื่อมกับ:

- `JournalService`
- `JournalViewModel`
- `JournalEditorViewModel`
- `StoryService`
- `SaveLoadService`
- `BackupService`

### `Models/SaveData.cs`

หน้าที่:

- ตาราง `SaveData`
- เก็บ save slot 1-3
- เก็บ snapshot เป็น JSON เช่น player, inventory, journal, progress

เชื่อมกับ:

- `SaveLoadService`
- `SaveLoadViewModel`
- `MainMenuViewModel`
- `BackupService`

### `Models/StoryEvent.cs`

หน้าที่:

- model ของ floor event จาก `floors.json`
- เก็บ floor number, title, narrative, event type, choices, image, journal key

เชื่อมกับ:

- `SeedDataService`
- `StoryService`
- `StoryViewModel`
- `StoryChoice.cs`
- `Resources/Raw/floors.json`

### `Models/StoryChoice.cs`

หน้าที่:

- model ของตัวเลือกในแต่ละ floor
- เก็บ text, result, HP/Gold/EXP delta, item reward, required item, ending type

เชื่อมกับ:

- `StoryEvent`
- `StoryService`
- `StoryViewModel`
- `Resources/Raw/floors.json`

### `Models/StoryProgress.cs`

หน้าที่:

- ตาราง `StoryProgress`
- บันทึกว่า player ผ่าน floor ไหน เลือก choice ไหน เมื่อไร

เชื่อมกับ:

- `StoryService`
- `SaveLoadService`
- `BackupService`
- `CharacterViewModel` ถ้าต้องแสดง progress

### `Models/StoryJournalSeed.cs`

หน้าที่:

- model สำหรับข้อมูล seed จาก `story_journals.json`
- ยังไม่ใช่ journal ของ player โดยตรง

เชื่อมกับ:

- `SeedDataService`
- `JournalService`
- `Resources/Raw/story_journals.json`

### `Models/BackupData.cs`

หน้าที่:

- model รวมข้อมูล backup ทั้ง user
- ใช้ export/import JSON

เชื่อมกับ:

- `BackupService`
- `BackupViewModel`
- `User`
- `Player`
- `InventoryItem`
- `Journal`
- `SaveData`
- `StoryProgress`

## Services

### `Services/DatabaseService.cs`

หน้าที่:

- เปิด SQLite connection
- สร้าง table ทั้งหมด
- มี generic CRUD เช่น insert, update, delete, get

เชื่อมกับ:

- Models ทุกตัวที่เป็น table
- Services เกือบทุกไฟล์

เป็นฐานของ back-end ทั้งระบบ

### `Services/SeedDataService.cs`

หน้าที่:

- อ่าน raw JSON จาก `Resources/Raw`
- seed item master ลง SQLite
- โหลด floor story และ story journal seed

เชื่อมกับ:

- `DatabaseService`
- `Models/Item`
- `Models/StoryEvent`
- `Models/StoryJournalSeed`
- `Resources/Raw/items.json`
- `Resources/Raw/floors.json`
- `Resources/Raw/story_journals.json`
- `StoryService`
- `JournalService`

### `Services/AuthService.cs`

หน้าที่:

- Register
- Login
- Logout
- Get current user
- hash/verify password
- set/clear session

เชื่อมกับ:

- `DatabaseService`
- `ValidationService`
- `PasswordHasher`
- `SessionManager`
- `User`
- `LoginViewModel`
- `RegisterViewModel`
- `MainMenuViewModel`

### `Services/ValidationService.cs`

หน้าที่:

- ตรวจ username/password/player name
- คืนผลว่าถูกต้องหรือ error message

เชื่อมกับ:

- `AuthService`
- หน้าอื่นที่ต้อง validate input

### `Services/PlayerService.cs`

หน้าที่:

- สร้าง player ใหม่
- โหลด active player
- update stat เช่น HP, Gold, EXP, Level
- advance floor
- complete game
- delete player พร้อมข้อมูลที่เกี่ยวข้อง

เชื่อมกับ:

- `DatabaseService`
- `SessionManager`
- `Player`
- `MainMenuViewModel`
- `StoryViewModel`
- `StoryService`
- `InventoryViewModel`
- `CharacterViewModel`
- `SaveLoadService`

### `Services/StoryService.cs`

หน้าที่:

- engine หลักของ story choice
- โหลด current floor
- ตรวจ choice ว่าเลือกได้ไหม
- apply ผล choice เช่น HP/Gold/EXP/item/journal
- save progress
- ตรวจ ending

เชื่อมกับ:

- `SeedDataService`
- `DatabaseService`
- `PlayerService`
- `InventoryService`
- `JournalService`
- `StoryEvent`
- `StoryChoice`
- `StoryProgress`
- `StoryViewModel`

### `Services/InventoryService.cs`

หน้าที่:

- CRUD ของ inventory
- เพิ่ม item
- โหลด item ของ player
- ใช้ item/ลด quantity
- ลบ item
- search/filter item

เชื่อมกับ:

- `DatabaseService`
- `InventoryItem`
- `Item`
- `InventoryViewModel`
- `StoryService`
- `SaveLoadService`

### `Services/JournalService.cs`

หน้าที่:

- CRUD ของ Player Journal
- unlock Story Journal จาก seed
- search journal
- ป้องกันไม่ให้แก้/ลบ Story Journal

เชื่อมกับ:

- `DatabaseService`
- `SeedDataService`
- `Journal`
- `StoryJournalSeed`
- `JournalViewModel`
- `JournalEditorViewModel`
- `StoryService`
- `SaveLoadService`

### `Services/SaveLoadService.cs`

หน้าที่:

- จัดการ save slot 1-3
- save player/inventory/journal/progress เป็น JSON snapshot
- load snapshot กลับเข้า database
- delete save slot

เชื่อมกับ:

- `DatabaseService`
- `PlayerService`
- `InventoryService`
- `JournalService`
- `SaveData`
- `StoryProgress`
- `SaveLoadViewModel`
- `MainMenuViewModel`

### `Services/BackupService.cs`

หน้าที่:

- export ข้อมูล user เป็นไฟล์ JSON
- preview backup file
- restore backup กลับ database
- delete backup file

เชื่อมกับ:

- `DatabaseService`
- `BackupData`
- `User`
- `Player`
- `InventoryItem`
- `Journal`
- `SaveData`
- `StoryProgress`
- `BackupViewModel`

### `Services/SettingsService.cs`

หน้าที่:

- อ่าน/บันทึก setting เช่น BGM/SFX enabled และ volume
- ใช้ Preferences เหมาะกับข้อมูลเล็ก ๆ

เชื่อมกับ:

- `SettingsViewModel`
- `AudioService`
- `AppConstants`

### `Services/AudioService.cs`

หน้าที่:

- เล่น BGM และ SFX
- ใช้ setting เพื่อเปิด/ปิดเสียงหรือปรับ volume
- เล่นเสียงปุ่ม เสียง item เสียง ending

เชื่อมกับ:

- `SettingsService`
- `AppShell.xaml.cs`
- `MainMenuPage.xaml.cs`
- `StoryPage.xaml.cs`
- `InventoryViewModel`
- `EndingViewModel`
- `Resources/Raw/Audio/*`

## ViewModels

### `ViewModels/Base/BaseViewModel.cs`

หน้าที่:

- class แม่ของ ViewModel ทุกหน้า
- มี `IsBusy`, `IsNotBusy`, `Title`, `ErrorMessage`, `HasError`

เชื่อมกับ:

- ViewModels ทุกไฟล์
- XAML ทุกหน้าที่ bind loading/error/title

### `ViewModels/LoginViewModel.cs`

หน้าที่:

- รับ username/password จาก Login UI
- เรียก login
- ไป Main Menu เมื่อสำเร็จ
- ไป Register เมื่อกดสมัคร

เชื่อมกับ:

- `Views/LoginPage.xaml`
- `AuthService`
- `AppConstants`

### `ViewModels/RegisterViewModel.cs`

หน้าที่:

- รับ username/password/confirm password
- เรียก register
- ไป Main Menu เมื่อสมัครสำเร็จ
- กลับ Login ได้

เชื่อมกับ:

- `Views/RegisterPage.xaml`
- `AuthService`
- `AppConstants`

### `ViewModels/MainMenuViewModel.cs`

หน้าที่:

- แสดง username/greeting
- เช็ก continue game
- เช็ก save slot
- เริ่มเกมใหม่
- เล่นต่อ
- ไป character/save-load/backup/settings
- logout/exit

เชื่อมกับ:

- `Views/MainMenuPage.xaml`
- `AuthService`
- `PlayerService`
- `SaveLoadService`
- `SessionManager`
- `AppConstants`

### `ViewModels/IntroStoryViewModel.cs`

หน้าที่:

- ควบคุมหน้า intro story
- แสดง intro เป็นลำดับก่อนเข้า story หลัก
- ไปหน้า Story เมื่อ intro จบ

เชื่อมกับ:

- `Views/IntroStoryPage.xaml`
- `AudioService`
- `AppConstants`
- รูป `Resources/Images/intro_*.jpg`

### `ViewModels/StoryViewModel.cs`

หน้าที่:

- state หลักของหน้าเล่นเกม
- โหลด player และ floor
- แสดง HP/Gold/Floor/Narrative/Choices
- รับคำสั่งเลือก choice
- แสดง outcome
- ไป inventory/journal/menu/ending

เชื่อมกับ:

- `Views/StoryPage.xaml`
- `PlayerService`
- `StoryService`
- `AudioService`
- `StoryEvent`
- `StoryChoice`
- `ChoiceOutcome`
- `AppConstants`

### `ViewModels/CharacterViewModel.cs`

หน้าที่:

- โหลดและแสดงข้อมูลตัวละคร
- เช่น level, HP, attack, defense, floor, choices made

เชื่อมกับ:

- `Views/CharacterPage.xaml`
- `PlayerService`
- `StoryService`

### `ViewModels/InventoryViewModel.cs`

หน้าที่:

- โหลด inventory
- search/filter item
- แสดงรายละเอียด item
- ใช้ healing item
- ทิ้ง item
- กลับหน้าก่อนหน้า

เชื่อมกับ:

- `Views/InventoryPage.xaml`
- `InventoryService`
- `PlayerService`
- `AudioService`
- `InventoryItem`
- `Item`

### `ViewModels/JournalViewModel.cs`

หน้าที่:

- แสดง Story Journal และ Player Journal
- switch tab
- search journal
- เปิด journal
- ไปหน้าเขียน journal ใหม่

เชื่อมกับ:

- `Views/JournalPage.xaml`
- `JournalService`
- `PlayerService`
- `JournalEditorPage`
- `Journal`

### `ViewModels/JournalEditorViewModel.cs`

หน้าที่:

- สร้าง/แก้ไข/ลบ Player Journal
- แสดงรายละเอียด Story Journal แบบ read-only

เชื่อมกับ:

- `Views/JournalEditorPage.xaml`
- `JournalService`
- `PlayerService`
- `Journal`

### `ViewModels/SaveLoadViewModel.cs`

หน้าที่:

- โหลด save slot ทั้ง 3 ช่อง
- save เกมลง slot
- load เกมจาก slot
- delete save

เชื่อมกับ:

- `Views/SaveLoadPage.xaml`
- `SaveLoadService`
- `PlayerService`
- `SessionManager`
- `SaveData`

### `ViewModels/BackupViewModel.cs`

หน้าที่:

- export backup
- list backup files
- preview backup
- import/restore backup
- delete backup file

เชื่อมกับ:

- `Views/BackupPage.xaml`
- `BackupService`
- `SessionManager`
- `BackupData`

### `ViewModels/SettingsViewModel.cs`

หน้าที่:

- แสดงและแก้ไข setting
- เปิด/ปิด BGM/SFX
- ปรับ volume
- reset setting

เชื่อมกับ:

- `Views/SettingsPage.xaml`
- `SettingsService`
- `AudioService`

### `ViewModels/EndingViewModel.cs`

หน้าที่:

- รับ ending type
- แสดง ending text/image/music ตาม Good/Bad/TrueGood
- กลับ main menu หรือเริ่มใหม่

เชื่อมกับ:

- `Views/EndingPage.xaml`
- `PlayerService`
- `AudioService`
- `AppConstants`
- รูป `ending_*.jpg`

## Views

หลักการของ Views:

- `.xaml` คือหน้าตา UI
- `.xaml.cs` คือ code-behind ใช้ตั้ง `BindingContext` และเรียก lifecycle เช่น `OnAppearing`
- logic หลักอยู่ใน ViewModel ไม่ใช่ code-behind

### `Views/LoginPage.xaml` / `LoginPage.xaml.cs`

หน้าที่:

- หน้า login
- UI ช่อง username/password
- ปุ่ม login และลิงก์สมัครสมาชิก

เชื่อมกับ:

- `LoginViewModel`
- `AuthService` ผ่าน ViewModel
- `AppConstants.RouteMainMenu`
- `RegisterPage`

### `Views/RegisterPage.xaml` / `RegisterPage.xaml.cs`

หน้าที่:

- หน้าสมัครสมาชิก
- UI ช่อง username/password/confirm password

เชื่อมกับ:

- `RegisterViewModel`
- `AuthService` ผ่าน ViewModel
- `LoginPage`
- `MainMenuPage`

### `Views/MainMenuPage.xaml` / `MainMenuPage.xaml.cs`

หน้าที่:

- หน้าเมนูหลัก
- แสดงชื่อผู้ใช้
- ปุ่ม New Game, Continue, Character, Save/Load, Backup, Settings, Logout, Exit
- เล่นเพลงเมนูใน `OnAppearing`

เชื่อมกับ:

- `MainMenuViewModel`
- `AudioService`
- `IntroStoryPage`
- `StoryPage`
- `CharacterPage`
- `SaveLoadPage`
- `BackupPage`
- `SettingsPage`
- `LoginPage`

### `Views/IntroStoryPage.xaml` / `IntroStoryPage.xaml.cs`

หน้าที่:

- หน้า intro ก่อนเริ่มเล่นจริง
- แสดงภาพ/ข้อความ intro เป็นลำดับ

เชื่อมกับ:

- `IntroStoryViewModel`
- `StoryPage`
- `Resources/Images/intro_*.jpg`
- `AudioService`

### `Views/StoryPage.xaml` / `StoryPage.xaml.cs`

หน้าที่:

- หน้าเล่นเกมหลัก
- แสดง player status, floor title, narrative, image, choices, outcome
- ปุ่มไป inventory/journal/menu

เชื่อมกับ:

- `StoryViewModel`
- `StoryService`
- `PlayerService`
- `InventoryService`
- `JournalService`
- `AudioService`
- `InventoryPage`
- `JournalPage`
- `EndingPage`
- `Resources/Raw/floors.json`
- `Resources/Images/floor_*.jpg`

### `Views/CharacterPage.xaml` / `CharacterPage.xaml.cs`

หน้าที่:

- หน้าแสดงข้อมูลตัวละคร
- stat, progress, floor, level

เชื่อมกับ:

- `CharacterViewModel`
- `PlayerService`
- `StoryService`

### `Views/InventoryPage.xaml` / `InventoryPage.xaml.cs`

หน้าที่:

- หน้า inventory
- แสดง item เป็น grid/list
- search/filter item
- แตะ item เพื่อดูรายละเอียดหรือใช้ item

เชื่อมกับ:

- `InventoryViewModel`
- `InventoryService`
- `PlayerService`
- `InventoryItem`
- `Item`
- `Resources/Images/*item*.png`

### `Views/JournalPage.xaml` / `JournalPage.xaml.cs`

หน้าที่:

- หน้า list journal
- แยก Story Journal กับ Player Journal
- search journal
- ปุ่มสร้าง journal ใหม่

เชื่อมกับ:

- `JournalViewModel`
- `JournalService`
- `JournalEditorPage`
- `Journal`

### `Views/JournalEditorPage.xaml` / `JournalEditorPage.xaml.cs`

หน้าที่:

- หน้าเขียน/แก้ไข/อ่าน journal
- Player Journal แก้/ลบได้
- Story Journal อ่านอย่างเดียว

เชื่อมกับ:

- `JournalEditorViewModel`
- `JournalService`
- `JournalPage`
- `Journal`

### `Views/SaveLoadPage.xaml` / `SaveLoadPage.xaml.cs`

หน้าที่:

- หน้า save/load slot
- แสดง slot 1-3
- save, load, delete save

เชื่อมกับ:

- `SaveLoadViewModel`
- `SaveLoadService`
- `SaveData`
- `StoryPage`

### `Views/BackupPage.xaml` / `BackupPage.xaml.cs`

หน้าที่:

- หน้า backup
- export/import/delete backup file
- preview ข้อมูล backup

เชื่อมกับ:

- `BackupViewModel`
- `BackupService`
- `BackupData`

### `Views/SettingsPage.xaml` / `SettingsPage.xaml.cs`

หน้าที่:

- หน้า setting
- ปรับ BGM/SFX enabled และ volume
- reset setting

เชื่อมกับ:

- `SettingsViewModel`
- `SettingsService`
- `AudioService`

### `Views/EndingPage.xaml` / `EndingPage.xaml.cs`

หน้าที่:

- หน้า ending
- แสดงภาพ/ข้อความ/เพลงตาม ending type

เชื่อมกับ:

- `EndingViewModel`
- `PlayerService`
- `AudioService`
- `MainMenuPage`
- `Resources/Images/ending_*.jpg`

## Resources

### `Resources/Raw/floors.json`

หน้าที่:

- เก็บเนื้อเรื่อง 30 floor
- แต่ละ floor มี title, narrative, image, choices, reward, required item, ending

เชื่อมกับ:

- `SeedDataService`
- `StoryService`
- `StoryViewModel`
- `StoryEvent`
- `StoryChoice`
- รูป `Resources/Images/floor_*.jpg`

### `Resources/Raw/items.json`

หน้าที่:

- master item seed 12 รายการ
- ใช้ seed ลงตาราง `Items`

เชื่อมกับ:

- `SeedDataService`
- `Item`
- `InventoryService`
- `StoryService`

### `Resources/Raw/story_journals.json`

หน้าที่:

- seed ของ Story Journal
- ยังไม่ใช่ journal ของผู้เล่นจนกว่าจะ unlock

เชื่อมกับ:

- `SeedDataService`
- `JournalService`
- `StoryService`
- `StoryJournalSeed`
- `Journal`

### `Resources/Images/*`

หน้าที่:

- รูปพื้นหลัง
- รูป floor
- รูป item
- รูป intro
- รูป ending

เชื่อมกับ:

- XAML หลายหน้า
- `InventoryItem.DisplayImage`
- `StoryEvent.ImageFile`
- `IntroStoryViewModel`
- `EndingViewModel`

### `Resources/Raw/Audio/*`

หน้าที่:

- เพลง BGM และเสียง SFX

เชื่อมกับ:

- `AudioService`
- `MainMenuPage.xaml.cs`
- `StoryPage.xaml.cs`
- `EndingViewModel`
- `InventoryViewModel`
- `AppShell.xaml.cs`

### `Resources/Styles/Colors.xaml`

หน้าที่:

- สีหลักของแอป เช่น background, gold, text, danger

เชื่อมกับ:

- `App.xaml`
- XAML ทุกหน้า

### `Resources/Styles/Styles.xaml`

หน้าที่:

- style กลาง เช่น button, label, border, entry

เชื่อมกับ:

- `App.xaml`
- XAML ทุกหน้า

## Platforms

### `Platforms/Android/MainActivity.cs`

หน้าที่:

- entry point ฝั่ง Android
- เชื่อม MAUI app กับ Android activity

เชื่อมกับ:

- MAUI runtime
- `MauiProgram.cs`

### `Platforms/Android/MainApplication.cs`

หน้าที่:

- Android application class
- เรียกสร้าง MAUI app

เชื่อมกับ:

- `MauiProgram.cs`

### `Platforms/Android/AndroidManifest.xml`

หน้าที่:

- config Android เช่น permission, app metadata

เชื่อมกับ:

- Android build/deployment

### `Platforms/Windows/*`

หน้าที่:

- config และ entry point สำหรับ Windows build

เชื่อมกับ:

- MAUI runtime บน Windows
- `MauiProgram.cs`

### `Platforms/iOS/*` และ `Platforms/MacCatalyst/*`

หน้าที่:

- config สำหรับ iOS/MacCatalyst template
- โปรเจกต์นี้ focus หลักคือ Android/Windows ตามสถานะปัจจุบัน

เชื่อมกับ:

- MAUI runtime ของ platform นั้น ๆ

## Flow ตามหน้าแบบสั้น

### Login

```text
LoginPage.xaml
    -> LoginViewModel
        -> AuthService
            -> DatabaseService
                -> User table
        -> SessionManager
        -> MainMenuPage
```

### Register

```text
RegisterPage.xaml
    -> RegisterViewModel
        -> AuthService
            -> ValidationService
            -> PasswordHasher
            -> DatabaseService
                -> User table
        -> SessionManager
        -> MainMenuPage
```

### Main Menu

```text
MainMenuPage.xaml
    -> MainMenuViewModel
        -> SessionManager
        -> PlayerService
        -> SaveLoadService
    -> IntroStoryPage / StoryPage / SaveLoadPage / BackupPage / SettingsPage
```

### Story

```text
StoryPage.xaml
    -> StoryViewModel
        -> PlayerService
        -> StoryService
            -> SeedDataService -> floors.json
            -> InventoryService
            -> JournalService
            -> DatabaseService -> StoryProgress
    -> EndingPage / InventoryPage / JournalPage
```

### Inventory

```text
InventoryPage.xaml
    -> InventoryViewModel
        -> InventoryService
            -> InventoryItems table
            -> Items table
        -> PlayerService
```

### Journal

```text
JournalPage.xaml
    -> JournalViewModel
        -> JournalService
            -> Journals table
    -> JournalEditorPage
        -> JournalEditorViewModel
            -> JournalService
```

### Save / Load

```text
SaveLoadPage.xaml
    -> SaveLoadViewModel
        -> SaveLoadService
            -> PlayerService
            -> InventoryService
            -> JournalService
            -> SaveData table
```

### Backup

```text
BackupPage.xaml
    -> BackupViewModel
        -> BackupService
            -> Users / Players / InventoryItems / Journals / SaveData / StoryProgress
            -> JSON backup file
```

### Settings

```text
SettingsPage.xaml
    -> SettingsViewModel
        -> SettingsService
            -> Preferences
        -> AudioService
```

### Ending

```text
EndingPage.xaml
    -> EndingViewModel
        -> PlayerService
        -> AudioService
    -> MainMenuPage
```


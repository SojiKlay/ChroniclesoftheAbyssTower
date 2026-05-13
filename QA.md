# Chronicles of the Abyss Tower — Q&A 10 ข้อ (สำหรับนำเสนออาจารย์)

## 1. โปรเจคนี้คืออะไร?

**Chronicles of the Abyss Tower** เป็นเกม **Text-based Dark Fantasy RPG** แนวการตัดสินใจ (Choice-Based) พัฒนาด้วย **.NET MAUI** + **SQLite** โดยใช้ MVVM Architecture

ผู้เล่นรับบทเป็น *Arin* อัศวินผู้ถูกเนรเทศ ที่ต้องเดินทางขึ้นหอคอยอเวจี 30 ชั้น เพื่อหา Eleanor น้องสาวที่หายไป โดยทุกชั้นมี 3 ตัวเลือกที่ส่งผลต่อ HP, Gold, Item และ Ending สุดท้าย

---

## 2. ใช้เทคโนโลยีอะไรบ้าง?

| หมวด | เทคโนโลยี |
|---|---|
| Framework | .NET 10 + .NET MAUI 10.0.60 |
| ภาษา | C# 12 |
| UI | XAML + Shell Navigation |
| Architecture | MVVM (CommunityToolkit.Mvvm) |
| Database | SQLite (sqlite-net-pcl) |
| JSON | Newtonsoft.Json (Backup) |
| Auth | PBKDF2-SHA256 (100k iterations) |
| Platform | Android เป็นหลัก |

---

## 3. มีระบบ CRUD อะไรบ้าง?

มี CRUD ครบทั้ง 4 ระบบหลัก:

### 1. Player (Character)
- **C** — `PlayerService.CreateNewPlayerAsync` (สร้างตัวละครใหม่)
- **R** — `GetActivePlayerAsync`, `GetByUserAsync`, `GetLatestByUserAsync`
- **U** — `UpdateAsync`, `ApplyHpDeltaAsync`, `AddExperienceAsync`, `AdvanceFloorAsync`
- **D** — `DeleteAsync` (เริ่มเกมใหม่)

### 2. Inventory
- **C** — `AddItemAsync` (stack อัตโนมัติถ้ามีอยู่แล้ว)
- **R** — `GetByPlayerAsync`, `SearchAsync`, `FilterByTypeAsync`
- **U** — `UpdateQuantityAsync`, `ConsumeItemAsync` (-1 quantity)
- **D** — `DeleteAsync`

### 3. Journal (บันทึก)
- **C** — `CreatePlayerJournalAsync`, `UnlockStoryJournalAsync`
- **R** — `GetByPlayerAsync`, `SearchAsync`, แบ่ง Story/Player
- **U** — `UpdatePlayerJournalAsync` (Story journal แก้ไม่ได้)
- **D** — `DeletePlayerJournalAsync`

### 4. SaveData (3 Slots)
- **C** — `SaveToSlotAsync` (เก็บ snapshot JSON)
- **R** — `GetAllSlotsAsync`, `GetSlotAsync`
- **U** — `SaveToSlotAsync` (overwrite slot เดิม)
- **D** — `DeleteSlotAsync`

---

## 4. ใช้ MVVM Pattern อย่างไร?

โครงสร้างแยกชัดเจน:

```
Views/        ← XAML pages (View)
ViewModels/   ← Logic + state (ViewModel)
Models/       ← Plain data classes (Model)
Services/    ← Business logic + DB access
```

ตัวอย่าง flow: `MainMenuPage.xaml` ผูก `BindingContext = MainMenuViewModel` ที่ inject ผ่าน DI → ViewModel ใช้ `AuthService` + `PlayerService` → Service เข้าถึง `DatabaseService` → SQLite

ใช้ `[ObservableProperty]` และ `[RelayCommand]` ของ `CommunityToolkit.Mvvm` เพื่อลด boilerplate

---

## 5. ระบบฐานข้อมูลออกแบบอย่างไร?

ใช้ SQLite 7 tables:

| Table | บทบาท |
|---|---|
| `Users` | บัญชีผู้ใช้ + PBKDF2 hash |
| `Players` | ตัวละครในเกม (1 user หลายตัว) |
| `Items` | Master data ของ item ทุกชนิด (seed จาก JSON) |
| `InventoryItems` | Instance ของ item ที่ player ถือ |
| `Journals` | บันทึก (Story + Player) |
| `SaveData` | 3 save slots (snapshot JSON) |
| `StoryProgress` | บันทึก floor ที่ผ่านแล้ว |

ความสัมพันธ์:
- `User 1—N Player`
- `Player 1—N InventoryItem N—1 Item`
- `Player 1—N Journal`
- `User 1—3 SaveData`

---

## 6. มีกี่หน้า (Pages)?

รวม **14 หน้า** แบ่งตาม Phase:

| หน้า | ไฟล์ |
|---|---|
| Login | `LoginPage` |
| Register | `RegisterPage` |
| Main Menu | `MainMenuPage` |
| Intro Story | `IntroStoryPage` |
| Story (เกมหลัก) | `StoryPage` |
| Ending | `EndingPage` |
| Character | `CharacterPage` |
| Inventory | `InventoryPage` |
| Journal | `JournalPage` |
| Journal Editor | `JournalEditorPage` |
| Save/Load | `SaveLoadPage` |
| Backup | `BackupPage` |
| Settings | `SettingsPage` |
| MainPage (default placeholder) | `MainPage` |

ใช้ **Shell Navigation** ผสมระหว่าง absolute route (`//login`, `//main`) และ relative route (`character`, `story`, ฯลฯ)

---

## 7. ระบบเลือกตัดสินใจ (Choice System) ทำงานอย่างไร?

แต่ละ Floor มี `StoryEvent` ที่เก็บ 3 `StoryChoice` ใน JSON file `floors.json`

แต่ละ choice กำหนดได้:
- **HpDelta / GoldDelta / ExpDelta** — เปลี่ยน stats
- **ItemReward** — ได้ item เพิ่ม
- **RequiredItem** — ต้องมีและถูก consume
- **RequiresItem** — ต้องมีแต่ไม่ consume (item key)
- **AdvanceFloor** — เลื่อนชั้นต่อไป
- **EndingType** — `"Good"` / `"Bad"` / `""`

Flow:
1. `StoryService.GetCurrentFloorEventAsync` โหลด floor ปัจจุบัน
2. `CanSelectChoiceAsync` ตรวจว่ามี item ที่ require หรือไม่ (disable ถ้าไม่มี)
3. `ApplyChoiceAsync` apply effect ทั้งหมด → คืน `ChoiceOutcome`
4. ถ้า HP = 0 → Bad Ending, ถ้า Floor 30 → Good/TrueGood Ending

---

## 8. ระบบ Save/Load ทำงานอย่างไร?

**3 Save Slots ต่อ User** (`SaveLoadService`)

### Save Flow
1. ดึง active player + inventory + journals + progress
2. Serialize ทั้งหมดเป็น JSON ผ่าน `Newtonsoft.Json`
3. เก็บใน `SaveData` table ที่ slot นั้นๆ พร้อม denormalized fields (PlayerName, Floor, HP, Level)

### Load Flow
1. อ่าน snapshot JSON ของ slot
2. Deserialize เป็น Player → reset PlayerId → insert ใหม่
3. map PlayerId เก่า → ใหม่ → insert inventory/journal/progress
4. Set active player ผ่าน `SessionManager.SetActivePlayerIdAsync`

นอกจากนี้มี **Backup System** ที่ export ทั้ง user เป็น JSON file เก็บใน `FileSystem.AppDataDirectory/AbyssTowerBackups/` (ใช้ `Share` หรือ File Manager เพื่อย้ายไป cloud)

---

## 9. มีระบบรักษาความปลอดภัยอย่างไร?

### 1. Password Hashing
- **PBKDF2-SHA256** จาก `System.Security.Cryptography`
- **100,000 iterations**
- **Salt 16 bytes** (random ต่อ user)
- **Hash 32 bytes** (Base64 เก็บใน DB)
- ไฟล์: `Helpers/PasswordHasher.cs`

### 2. Session Storage
- ใช้ `SecureStorage` ของ MAUI (เข้ารหัสโดยระบบ OS)
- เก็บ 3 keys: `abyss_user_id`, `abyss_username`, `abyss_active_player_id`

### 3. Validation
- `ValidationService` ตรวจ username/password/playerName ก่อนบันทึก
- Username ต้อง unique (constraint ระดับ DB)

---

## 10. แต่ละ Phase ทำอะไรบ้าง?

| Phase | สิ่งที่ทำ | สถานะ |
|---|---|---|
| **0** | Foundation: project, packages, theme, DI | ✅ |
| **1** | Database + 11 Models + Seed (12 items, 11 journals, 30 floors) | ✅ |
| **2** | Auth (Login/Register/Session) | ✅ |
| **3** | Main Menu + Character + Settings | ✅ |
| **4** | Story System + Choice Engine + 30 Floors + 3 Endings | ✅ |
| **5** | Inventory + Journal CRUD + Search + Filter | ✅ |
| **6** | Save/Load (3 slots) + Backup (JSON export/import/share) | ✅ |
| **7** | Animations Helper + UI Polish | ✅ |
| **8** | Q&A 10 + เอกสาร | ✅ |
| **9** | Build AAB + Play Store Submission | ⏳ ผู้ใช้ทำเอง |

---

# Bonus: คำถามที่อาจารย์อาจถาม

### Q: ทำไมเลือก SQLite ไม่ใช่ cloud database?
**A:** เกมนี้เป็น offline-first — ผู้เล่นต้องเล่นได้แม้ไม่มี internet โดย SQLite ทำงาน in-process รวดเร็ว ไม่ต้องการ server มี **Backup System** เป็น JSON file ให้ user copy ไป cloud เองได้

### Q: ใช้ MVVM แล้วได้ประโยชน์อะไร?
**A:**
- แยก UI (XAML) จาก Logic (C#) → maintainable
- ทดสอบ ViewModel ง่ายโดยไม่ต้องเปิด UI
- Reusable: 1 ViewModel ใช้กับหลาย Pages ได้
- Data Binding ลดโค้ดที่ต้องเขียนด้วยมือ

### Q: ทำไมเก็บ snapshot เป็น JSON?
**A:** Snapshot pattern ทำให้:
1. Save/Load เป็น atomic — ไม่มี partial state
2. ง่ายต่อการ migration (เพิ่ม field ใหม่ไม่เสีย save เก่า)
3. ใช้ schema เดียวกับ Backup System (reuse code)

### Q: ถ้ามีเวลาเพิ่ม จะทำอะไรต่อ?
**A:**
1. **Audio** — BGM ที่เปลี่ยนตาม floor + SFX choice
2. **Cloud Backup** — Google Drive integration
3. **Achievement** — ติดตามสถิติพิเศษ
4. **Multi-language** — ไทย/อังกฤษ
5. **Visual effects** — Lottie animations ที่ ending

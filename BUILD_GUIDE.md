# Chronicles of the Abyss Tower — Build & Submission Guide

คู่มือสร้าง AAB และนำเสนอ Play Store

---

## 📋 Prerequisites

- **Visual Studio 2026** + .NET MAUI workload
- **.NET 10 SDK** (10.0.203+)
- **Android SDK** + Java JDK 17
- **Keystore สำหรับ release** (สร้างเอง ครั้งเดียว)

---

## 🔑 Step 1: สร้าง Keystore (ครั้งเดียว)

```powershell
# เปิด PowerShell แล้วรัน:
keytool -genkeypair -v -keystore abyss-tower.keystore `
    -alias abysstower `
    -keyalg RSA `
    -keysize 2048 `
    -validity 10000
```

> **⚠️ สำคัญ:** เก็บไฟล์ `abyss-tower.keystore` + รหัสผ่านไว้ในที่ปลอดภัย — ถ้าหาย จะอัพเดทแอปเก่าไม่ได้

---

## 📝 Step 2: ปรับ csproj สำหรับ Release

เปิด `@c:\Users\User\source\repos\ChroniclesoftheAbyssTowerSln\ChroniclesoftheAbyssTower\ChroniclesoftheAbyssTower.csproj` และตรวจ:

```xml
<PropertyGroup>
    <ApplicationTitle>Abyss Tower</ApplicationTitle>
    <ApplicationId>com.yourname.abysstower</ApplicationId>
    <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
</PropertyGroup>
```

**ทุกครั้งที่อัพเดท** ต้องเพิ่ม `ApplicationVersion` (1 → 2 → 3) และเปลี่ยน `ApplicationDisplayVersion` (1.0.0 → 1.0.1)

---

## 🏗️ Step 3: Build AAB (Android App Bundle)

### วิธีที่ 1: ใน Visual Studio
1. คลิกขวาที่ project → **Properties**
2. **Android > Options** → ตั้ง **Configuration = Release**, **Bundle Format = aab**
3. **Android > Signing** → ใส่ keystore path + password
4. คลิกขวาที่ project → **Publish**
5. AAB จะอยู่ที่ `bin/Release/net10.0-android/publish/`

### วิธีที่ 2: Command Line (แนะนำ)

```powershell
# จากโฟลเดอร์ ChroniclesoftheAbyssTower/
dotnet publish -f net10.0-android `
    -c Release `
    -p:AndroidSigningKeyStore="$PSScriptRoot\abyss-tower.keystore" `
    -p:AndroidSigningKeyAlias=abysstower `
    -p:AndroidSigningKeyPass=YourKeystorePassword `
    -p:AndroidSigningStorePass=YourKeystorePassword `
    -p:AndroidPackageFormat=aab
```

ไฟล์ output: `bin/Release/net10.0-android/publish/com.yourname.abysstower-Signed.aab`

---

## 📱 Step 4: ทดสอบ AAB ก่อน Submit

ใช้ **bundletool** เพื่อแปลง AAB เป็น APK เพื่อทดสอบ:

```powershell
java -jar bundletool.jar build-apks `
    --bundle=com.yourname.abysstower-Signed.aab `
    --output=test.apks `
    --mode=universal

# Extract APK
java -jar bundletool.jar install-apks --apks=test.apks
```

---

## 🎨 Step 5: เตรียม Play Store Assets

### Required Assets

| Asset | Size | จำนวน |
|---|---|---|
| App Icon | 512x512 PNG | 1 |
| Feature Graphic | 1024x500 PNG | 1 |
| Phone Screenshots | 1080x1920+ JPEG/PNG | 5-8 |
| Short Description | < 80 ตัวอักษร | 1 |
| Full Description | < 4000 ตัวอักษร | 1 |

### Suggested Screenshots
1. **Main Menu** — แสดงหน้าจอเริ่มต้น
2. **Story Page** — Floor 1 พร้อม 3 ตัวเลือก
3. **Character Page** — สถานะของ Arin
4. **Inventory** — กระเป๋าพร้อม items
5. **Ending Page** — ฉาก ending สวยๆ
6. **Save/Load** — 3 slots
7. **Settings** — preferences

### Sample Descriptions

**Short (Thai):**
> Text RPG แนว Dark Fantasy ที่ทุกการตัดสินใจส่งผลต่อชะตา ขึ้นหอคอย 30 ชั้น 3 ending

**Full (Thai):**
```
🌌 Chronicles of the Abyss Tower 🌌

เกม Text-based Dark Fantasy RPG ที่จะให้คุณรับบทเป็น Arin อัศวินผู้ถูกเนรเทศ
ออกผจญภัยในหอคอยอเวจี 30 ชั้น เพื่อหา Eleanor น้องสาวที่หายไป

✨ Feature Highlights:
• 30 ชั้นพร้อม 90+ ตัวเลือก
• 3 endings (Good / Bad / True Good)
• ระบบ Inventory + Journal CRUD
• 3 save slots ต่อ user
• Export/Import backup เป็นไฟล์ JSON
• Dark Fantasy aesthetic บน UI ภาษาไทย

📖 เนื้อเรื่อง:
หมอกอเวจีกลืนโลกทีละน้อย Arin ถูกกล่าวหาว่าเป็นต้นเหตุของหายนะ
ทางเดียวที่จะพิสูจน์ความบริสุทธิ์และพบเอลีน่าคือพิชิตหอคอย

#TextRPG #DarkFantasy #ChoiceBased #Offline
```

---

## 🚀 Step 6: Submit to Play Console

1. เข้า [Google Play Console](https://play.google.com/console)
2. **Create App** → ตั้งชื่อ + เลือกประเภท (Game)
3. ใน **App Content** กรอก:
   - Privacy Policy URL
   - App Access (login required: no)
   - Ads (no ads)
   - Content rating (Questionnaire)
   - Target audience (13+)
   - News app (no)
   - Data safety (ข้อมูล local เท่านั้น)
4. **Internal Testing**:
   - สร้าง track
   - อัพโหลด AAB
   - เพิ่ม email tester
5. ทดสอบ 1-2 วัน
6. **Production**:
   - Promote build จาก Internal → Production
   - กรอก Release notes
   - Submit for review (รอ 1-7 วัน)

---

## 🐛 Troubleshooting

### Build error: "Could not find package"
```powershell
dotnet restore --force
dotnet nuget locals all --clear
dotnet restore
```

### "Keystore was tampered with"
- ตรวจ password ถูกต้องหรือไม่
- ตรวจ path ของ keystore (ใช้ absolute path ถ้า relative ไม่ทำงาน)

### AAB file ใหญ่เกิน 150MB
- เปิด **R8 (code shrinking)** ใน csproj:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <AndroidLinkMode>SdkOnly</AndroidLinkMode>
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
</PropertyGroup>
```

### "Different signature" error เวลาอัพเดท
- ต้องใช้ keystore เดียวกันกับเวอร์ชันที่เคย publish แล้ว
- ห้ามสร้าง keystore ใหม่ทับ

---

## 📦 Optional: NuGet Packages ที่ตัดออกชั่วคราว

หลังจาก MAUI 10 stable แล้ว ให้ใส่กลับเข้าไป:

```xml
<!-- Audio -->
<PackageReference Include="Plugin.Maui.Audio" Version="X.X.X" />

<!-- Lottie animations -->
<PackageReference Include="SkiaSharp.Extended.UI.Maui" Version="X.X.X" />

<!-- Community Toolkit (Popups, Behaviors) -->
<PackageReference Include="CommunityToolkit.Maui" Version="X.X.X" />

<!-- Google Drive backup -->
<PackageReference Include="Google.Apis.Drive.v3" Version="1.69.0" />
<PackageReference Include="Google.Apis.Auth" Version="1.69.0" />
```

แล้วเปิด services/code ที่ comment ไว้ใน Phase 6/7

---

## ✅ Checklist ก่อน Submit

- [ ] Build AAB สำเร็จ (no warnings)
- [ ] รันทดสอบบนเครื่องจริง 2-3 เครื่อง
- [ ] ทดสอบ flow: Login → New Game → Floor 30 → Ending
- [ ] ทดสอบ Save / Load / Backup / Restore
- [ ] ทดสอบ Logout + Re-login (session persist)
- [ ] App icon + Splash screen แสดงถูก
- [ ] Permission ใน AndroidManifest ถูกต้อง (no INTERNET ถ้าไม่ใช้)
- [ ] ApplicationVersion อัพเดท
- [ ] Privacy Policy พร้อม (Google Sites หรือ GitHub Pages ฟรีก็ได้)
- [ ] Screenshots พร้อม
- [ ] Description ภาษาไทย + อังกฤษ พร้อม

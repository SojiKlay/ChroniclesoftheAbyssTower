# Code Walkthrough: Front-end + Back-end

เอกสารนี้ใช้เตรียมตอบคำถามอาจารย์เวลาเปิดโค้ดดู โดยอธิบายว่าแต่ละหน้า UI เชื่อมกับ ViewModel, Service, Model, SQLite และ JSON อย่างไร

## ภาพรวมสถาปัตยกรรม

โปรเจกต์นี้ใช้รูปแบบ MVVM ของ .NET MAUI

```text
Page.xaml (Front-end UI)
    -> Page.xaml.cs (ผูก BindingContext / lifecycle)
        -> ViewModel (state + command)
            -> Service (business logic)
                -> Model / SQLite / JSON
```

เหตุผลที่ใช้แบบนี้:

- แยกหน้าจอออกจาก logic ทำให้โค้ดอ่านง่าย
- หน้า XAML ไม่ต้องติดต่อ database โดยตรง
- Service เป็นจุดรวม logic เช่น login, story, inventory, journal
- ViewModel เป็นตัวกลางให้ UI bind ข้อมูลและคำสั่งได้
- เหมาะกับ requirement ของโปรเจกต์: MVVM, SQLite, CRUD, multi-page, save/load

## 1. App Startup และ Dependency Injection

ไฟล์หลัก:

- `ChroniclesoftheAbyssTower/MauiProgram.cs`
- `ChroniclesoftheAbyssTower/MauiProgram.Registrations.cs`
- `ChroniclesoftheAbyssTower/App.xaml.cs`
- `ChroniclesoftheAbyssTower/AppShell.xaml`
- `ChroniclesoftheAbyssTower/AppShell.xaml.cs`

### โค้ดลงทะเบียน Service

```csharp
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<SeedDataService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<JournalService>();
builder.Services.AddSingleton<StoryService>();
builder.Services.AddSingleton<SaveLoadService>();
builder.Services.AddSingleton<BackupService>();
```

โค้ดนี้อยู่ใน `MauiProgram.Registrations.cs`

ทำงานยังไง:

- เอา service เข้า DI Container ของ MAUI
- เวลา Page หรือ ViewModel ต้องใช้ service ระบบจะ inject ให้เอง
- ไม่ต้องเขียน `new AuthService(...)` เองทุกหน้า

ทำไมใช้ `Singleton`:

- Service เป็น logic กลางของแอป
- `DatabaseService` ควรมีตัวจัดการ connection หลักตัวเดียว
- ลดการสร้าง object ซ้ำโดยไม่จำเป็น

ถ้าอาจารย์ถาม:

> ทำไม Service ใช้ Singleton?

ตอบว่า:

> เพราะ service เหล่านี้เป็นตัวกลางของระบบ เช่น database, auth, story, inventory ใช้ร่วมกันทั้งแอป ไม่จำเป็นต้องสร้างใหม่ทุกครั้ง และช่วยให้จัดการ connection กับข้อมูลกลางได้ง่ายขึ้น

### โค้ดลงทะเบียน ViewModel และ Page

```csharp
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<StoryViewModel>();
builder.Services.AddTransient<InventoryViewModel>();

builder.Services.AddTransient<LoginPage>();
builder.Services.AddTransient<StoryPage>();
builder.Services.AddTransient<InventoryPage>();
```

ทำไมใช้ `Transient`:

- Page และ ViewModel ควรสร้างใหม่เมื่อเปิดหน้า
- ป้องกันข้อมูลหน้าก่อนค้างอยู่
- เหมาะกับ UI state เช่น search text, error message, loading state

## 2. Shell Navigation

ไฟล์ `AppShell.xaml`

```xml
<ShellContent Title="Login"
              Route="login"
              ContentTemplate="{DataTemplate views:LoginPage}"/>

<ShellContent Title="Register"
              Route="register"
              ContentTemplate="{DataTemplate views:RegisterPage}"/>

<ShellContent Title="Main"
              Route="main"
              ContentTemplate="{DataTemplate views:MainMenuPage}"/>
```

โค้ดนี้คือ route หลักของแอป

ทำงานยังไง:

- `Route="login"` ใช้กับ `Shell.Current.GoToAsync("//login")`
- `ContentTemplate` บอกว่า route นี้ต้องเปิด Page ไหน

ไฟล์ `AppShell.xaml.cs`

```csharp
Routing.RegisterRoute(AppConstants.RouteStory, typeof(StoryPage));
Routing.RegisterRoute(AppConstants.RouteInventory, typeof(InventoryPage));
Routing.RegisterRoute(AppConstants.RouteJournal, typeof(JournalPage));
Routing.RegisterRoute(AppConstants.RouteSaveLoad, typeof(SaveLoadPage));
Routing.RegisterRoute(AppConstants.RouteBackup, typeof(BackupPage));
Routing.RegisterRoute(AppConstants.RouteSettings, typeof(SettingsPage));
Routing.RegisterRoute(AppConstants.RouteEnding, typeof(EndingPage));
```

ทำไมต้อง register route:

- หน้า login/main เป็น ShellContent หลัก
- หน้าอื่นเป็น relative route ต้อง register เอง
- ทำให้ ViewModel สามารถสั่งนำทางด้วย route name ได้

ตัวอย่าง:

```csharp
await Shell.Current.GoToAsync(AppConstants.RouteInventory);
```

คือเปิดหน้า Inventory

## 3. BaseViewModel: โค้ดกลางของทุกหน้า

ไฟล์ `ViewModels/Base/BaseViewModel.cs`

```csharp
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
}
```

ทำงานยังไง:

- ทุก ViewModel สืบทอดจาก `BaseViewModel`
- ทุกหน้าจึงมี `IsBusy`, `IsNotBusy`, `Title`, `ErrorMessage`
- UI สามารถ bind ไปยังค่าเหล่านี้ได้

ทำไมใช้ `[ObservableProperty]`:

- มาจาก CommunityToolkit.Mvvm
- ช่วย generate property พร้อม `PropertyChanged`
- เมื่อค่าเปลี่ยน UI จะ refresh อัตโนมัติ

ตัวอย่างใน XAML:

```xml
<ActivityIndicator IsRunning="{Binding IsBusy}"
                   IsVisible="{Binding IsBusy}" />
```

ถ้า ViewModel ตั้ง `IsBusy = true` ตัว loading จะขึ้นเอง

## 4. Login Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/LoginPage.xaml`
- `Views/LoginPage.xaml.cs`
- `ViewModels/LoginViewModel.cs`
- `Services/AuthService.cs`
- `Services/DatabaseService.cs`
- `Models/User.cs`

### Front-end: LoginPage.xaml

```xml
<Entry Placeholder="กรอกชื่อผู้ใช้"
       Text="{Binding Username}"
       MaxLength="20" />

<Entry Placeholder="กรอกรหัสผ่าน"
       Text="{Binding Password}"
       IsPassword="True"
       ReturnCommand="{Binding LoginCommand}" />

<Label Text="{Binding ErrorMessage}"
       IsVisible="{Binding ErrorMessage, Converter={StaticResource StringNotEmptyConverter}}" />

<Button Text="เข้าสู่ระบบ"
        Command="{Binding LoginCommand}"
        IsEnabled="{Binding IsNotBusy}" />
```

ทำงานยังไง:

- `Entry Text="{Binding Username}"` ส่งค่าที่ผู้ใช้กรอกไปที่ `LoginViewModel.Username`
- `Password` bind ไปที่ `LoginViewModel.Password`
- ปุ่ม Login เรียก `LoginCommand`
- ถ้ามี error จะ bind `ErrorMessage` มาแสดง
- `IsEnabled="{Binding IsNotBusy}"` กันผู้ใช้กดซ้ำตอนกำลัง login

ทำไมใช้ Binding:

- UI ไม่ต้องรู้ว่า login ยังไง
- UI แค่ส่งข้อมูลให้ ViewModel
- ทำตาม MVVM

### Code-behind: LoginPage.xaml.cs

```csharp
public LoginPage(LoginViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;
}
```

ทำงานยังไง:

- รับ `LoginViewModel` จาก DI
- ตั้งเป็น `BindingContext`
- XAML จึง bind ไปหา property/command ใน ViewModel ได้

### ViewModel: LoginViewModel.cs

```csharp
[ObservableProperty]
private string username = string.Empty;

[ObservableProperty]
private string password = string.Empty;
```

โค้ดนี้คือ property ที่ XAML bind อยู่

```csharp
[RelayCommand]
private async Task LoginAsync()
{
    if (IsBusy) return;

    ClearError();
    IsBusy = true;

    try
    {
        var result = await _authService.LoginAsync(Username.Trim(), Password);

        if (!result.Success)
        {
            SetError(result.ErrorMessage ?? "เข้าสู่ระบบไม่สำเร็จ");
            return;
        }

        Password = string.Empty;
        await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
    }
    finally
    {
        IsBusy = false;
    }
}
```

ทำงานยังไง:

1. กันกดซ้ำด้วย `IsBusy`
2. เรียก `_authService.LoginAsync`
3. ถ้า login fail แสดง error
4. ถ้า success ล้าง password
5. นำทางไปหน้า Main Menu

ทำไมใช้ `[RelayCommand]`:

- ทำให้ method กลายเป็น `LoginCommand`
- XAML bind ปุ่มกับ command ได้
- ไม่ต้องเขียน `ICommand` เอง

### Back-end: AuthService.cs

```csharp
var user = await conn.Table<User>()
    .Where(u => u.Username == username)
    .FirstOrDefaultAsync();
```

ทำงาน:

- ค้นหา user จากตาราง `Users`

```csharp
if (!PasswordHasher.VerifyPassword(password, user.Salt, user.PasswordHash))
{
    return new AuthResult(false, "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
}
```

ทำงาน:

- ตรวจ password ด้วย salt และ hash

ทำไมไม่เก็บ password ตรง ๆ:

- ปลอดภัยกว่า
- ถ้า database หลุดจะไม่เห็นรหัสจริง

```csharp
await SessionManager.SetSessionAsync(user.UserId, user.Username);
```

ทำงาน:

- บันทึก session ว่าผู้ใช้ login แล้ว
- รอบหน้าเปิดแอปสามารถตรวจ session ได้

สรุปเส้นทาง Login:

```text
LoginPage.xaml
    -> LoginCommand
        -> LoginViewModel.LoginAsync()
            -> AuthService.LoginAsync()
                -> DatabaseService.GetConnectionAsync()
                    -> SQLite table Users
            -> SessionManager.SetSessionAsync()
            -> Shell.GoToAsync("//main")
```

## 5. Register Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/RegisterPage.xaml`
- `ViewModels/RegisterViewModel.cs`
- `Services/AuthService.cs`
- `Services/ValidationService.cs`
- `Models/User.cs`

### Front-end

หน้า Register ใช้ pattern เดียวกับ Login

```xml
<Entry Text="{Binding Username}" />
<Entry Text="{Binding Password}" IsPassword="True" />
<Entry Text="{Binding ConfirmPassword}" IsPassword="True" />

<Button Text="สมัครสมาชิก"
        Command="{Binding RegisterCommand}"
        IsEnabled="{Binding IsNotBusy}" />
```

ทำงาน:

- ช่องกรอกผูกกับ property ใน `RegisterViewModel`
- ปุ่มเรียก `RegisterCommand`

### ViewModel

```csharp
var result = await _authService.RegisterAsync(
    Username.Trim(),
    Password,
    ConfirmPassword);
```

ทำงาน:

- ส่งข้อมูลสมัครไปให้ `AuthService`

### Back-end

```csharp
var usernameCheck = _validationService.ValidateUsername(username);
var passwordCheck = _validationService.ValidatePassword(password);
var confirmCheck = _validationService.ValidateConfirmPassword(password, confirmPassword);
```

ทำไมใช้ ValidationService:

- แยก logic ตรวจ input ออกจาก ViewModel
- ใช้ซ้ำได้ทั้ง register หรือหน้าอื่นในอนาคต

```csharp
var salt = PasswordHasher.GenerateSalt();
var hash = PasswordHasher.HashPassword(password, salt);
```

ทำงาน:

- สร้าง salt
- hash password ก่อนบันทึก

```csharp
await conn.InsertAsync(user);
await SessionManager.SetSessionAsync(user.UserId, user.Username);
```

ทำงาน:

- เพิ่ม user ลง SQLite
- login ให้อัตโนมัติหลังสมัคร

## 6. Main Menu Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/MainMenuPage.xaml`
- `Views/MainMenuPage.xaml.cs`
- `ViewModels/MainMenuViewModel.cs`
- `Services/PlayerService.cs`
- `Services/SaveLoadService.cs`
- `Services/AuthService.cs`

### Front-end: MainMenuPage.xaml

```xml
<Label Text="{Binding Username}" />
<Label Text="{Binding GreetingText}" />

<Button Text="เล่นต่อ"
        Command="{Binding ContinueGameCommand}"
        IsVisible="{Binding HasContinueGame}" />

<Button Text="เริ่มเกมใหม่"
        Command="{Binding NewGameCommand}"
        IsEnabled="{Binding IsNotBusy}" />

<Button Text="ดูตัวละคร"
        Command="{Binding GoToCharacterCommand}"
        IsVisible="{Binding HasContinueGame}" />

<Button Text="เซฟ / โหลดเกม"
        Command="{Binding GoToSaveLoadCommand}"
        IsVisible="{Binding HasSaveLoadMenu}" />
```

ทำงาน:

- `Username`, `GreetingText` แสดงข้อมูลผู้ใช้
- ปุ่มเล่นต่อแสดงเฉพาะถ้ามีเกมค้าง
- ปุ่ม Save/Load แสดงเมื่อมี continue หรือ save slot
- ปุ่มแต่ละปุ่ม bind กับ command ใน `MainMenuViewModel`

ทำไมใช้ `IsVisible`:

- UI เปลี่ยนตาม state ได้เอง
- ถ้าไม่มีตัวละครก็ซ่อนปุ่มที่ยังใช้ไม่ได้

### Code-behind

```csharp
public MainMenuPage(MainMenuViewModel viewModel, AudioService audioService)
{
    InitializeComponent();
    BindingContext = viewModel;
    _audioService = audioService;
}

protected override async void OnAppearing()
{
    base.OnAppearing();
    await _audioService.PlayBgmAsync(...);

    if (BindingContext is MainMenuViewModel vm)
    {
        await vm.OnAppearingAsync();
    }
}
```

ทำงาน:

- ผูก ViewModel
- เล่นเพลงเมนู
- ทุกครั้งที่กลับมาหน้านี้จะ reload state เช่น มี save หรือ continue ไหม

### ViewModel: โหลดสถานะเมนู

```csharp
Username = await SessionManager.GetUsernameAsync() ?? string.Empty;

var userId = await SessionManager.GetUserIdAsync();
var slots = await _saveLoadService.GetAllSlotsAsync(userId.Value);
HasLoadSlots = slots.Any(slot => slot != null);

var active = await _playerService.GetActivePlayerAsync();
```

ทำงาน:

- อ่าน username จาก session
- โหลด save slot ของ user
- โหลด active player
- ใช้ตั้งค่า `HasContinueGame`, `HasSaveLoadMenu`, `ContinueInfo`

### ViewModel: New Game

```csharp
await _playerService.CreateNewPlayerAsync(userId.Value, name);
await Shell.Current.GoToAsync(AppConstants.RouteIntroStory);
```

ทำงาน:

- สร้างตัวละครใหม่
- ไปหน้า Intro Story

### Back-end: PlayerService.CreateNewPlayerAsync

```csharp
var player = new Player
{
    UserId = userId,
    PlayerName = string.IsNullOrWhiteSpace(playerName) ? "ผู้เล่น" : playerName.Trim(),
    Hp = AppConstants.StartingHp,
    MaxHp = AppConstants.StartingMaxHp,
    Attack = AppConstants.StartingAttack,
    Defense = AppConstants.StartingDefense,
    Gold = AppConstants.StartingGold,
    Level = AppConstants.StartingLevel,
    CurrentFloor = AppConstants.StartingFloor,
};

await conn.InsertAsync(player);
await SessionManager.SetActivePlayerIdAsync(player.PlayerId);
```

ทำไมใช้ `AppConstants`:

- ค่าเริ่มต้นรวมไว้ที่เดียว
- แก้ง่าย เช่น HP เริ่มต้น, จำนวน floor, save slot
- ลดเลข magic number ในโค้ด

สรุปเส้นทาง New Game:

```text
MainMenuPage.xaml
    -> NewGameCommand
        -> MainMenuViewModel.NewGameAsync()
            -> PlayerService.CreateNewPlayerAsync()
                -> SQLite table Players
                -> SessionManager.SetActivePlayerIdAsync()
            -> Shell.GoToAsync("introStory")
```

## 7. Story Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/StoryPage.xaml`
- `Views/StoryPage.xaml.cs`
- `ViewModels/StoryViewModel.cs`
- `Services/StoryService.cs`
- `Services/SeedDataService.cs`
- `Services/PlayerService.cs`
- `Services/InventoryService.cs`
- `Services/JournalService.cs`
- `Resources/Raw/floors.json`

### Front-end: แสดงข้อมูลผู้เล่น

```xml
<Label Text="{Binding PlayerName}" />
<Label Text="{Binding FloorDisplay}" />
<Label Text="{Binding Gold}" />

<ProgressBar Progress="{Binding HpRatio}" />
<Label Text="{Binding HpDisplay}" />
```

ทำงาน:

- แสดงชื่อผู้เล่น
- แสดงชั้นปัจจุบัน
- แสดงเงิน
- แสดง HP ด้วย progress bar

ค่าพวกนี้มาจาก `StoryViewModel`

```csharp
[ObservableProperty] private string playerName;
[ObservableProperty] private int gold;
[ObservableProperty] private double hpRatio;
[ObservableProperty] private string hpDisplay;
```

### Front-end: แสดงเนื้อเรื่อง

```xml
<Label Text="{Binding FloorIcon}"
       IsVisible="{Binding HasFloorIcon}" />

<Label Text="{Binding FloorTitle}" />
<Label Text="{Binding EventType}" />

<Image Source="{Binding FloorImage}" />
<Label Text="{Binding Narrative}" />
```

ทำงาน:

- ดึงข้อมูล floor จาก ViewModel
- ถ้า floor มีรูป `FloorImage` จะโชว์รูป
- เนื้อเรื่องมาจาก `floors.json`

### Front-end: ปุ่มเลือก Choice

```xml
<VerticalStackLayout BindableLayout.ItemsSource="{Binding Choices}"
                     IsEnabled="{Binding IsNotBusy}">
    <BindableLayout.ItemTemplate>
        <DataTemplate>
            <VerticalStackLayout>
                <Button Text="{Binding Text}"
                        IsEnabled="{Binding IsEnabled}"
                        Command="{Binding BindingContext.SelectChoiceCommand, Source={x:Reference StoryRoot}}"
                        CommandParameter="{Binding .}" />

                <Label Text="{Binding LockReason}"
                       IsVisible="{Binding ShowLockReason}" />
            </VerticalStackLayout>
        </DataTemplate>
    </BindableLayout.ItemTemplate>
</VerticalStackLayout>
```

ทำงาน:

- `Choices` เป็น list จาก `StoryViewModel`
- แต่ละ choice กลายเป็นปุ่ม
- ถ้า choice ใช้ไม่ได้ `IsEnabled = false`
- ถ้า lock จะโชว์เหตุผล เช่น ต้องมี item ก่อน
- กดปุ่มแล้วส่ง choice item ทั้งก้อนไปที่ `SelectChoiceCommand`

ทำไมใช้ `BindableLayout`:

- จำนวน choice มาจาก JSON
- UI สร้างปุ่มตามข้อมูลอัตโนมัติ
- ไม่ต้อง hardcode ปุ่ม 1, 2, 3 ใน XAML

### Code-behind

```csharp
public StoryPage(StoryViewModel viewModel, AudioService audioService)
{
    InitializeComponent();
    BindingContext = viewModel;
    _audioService = audioService;
}

protected override async void OnAppearing()
{
    base.OnAppearing();
    await _audioService.PlayBgmAsync(...);

    if (BindingContext is StoryViewModel vm)
    {
        await vm.OnAppearingAsync();
    }
}
```

ทำงาน:

- ผูก `StoryViewModel`
- เล่น BGM ของหอคอย
- โหลด player และ floor ทุกครั้งที่เข้าหน้า

### ViewModel: โหลด player และ floor

```csharp
Player = await _playerService.GetActivePlayerAsync();

if (Player.IsGameCompleted)
{
    await GoToEndingAsync(Player.EndingType);
    return;
}

await LoadCurrentFloorAsync();
```

ทำงาน:

- โหลด active player จาก session/database
- ถ้าเกมจบแล้วไปหน้า Ending
- ถ้ายังไม่จบ โหลด floor ปัจจุบัน

```csharp
CurrentEvent = await _storyService.GetCurrentFloorEventAsync(Player);

FloorTitle = CurrentEvent.Title;
Narrative = CurrentEvent.Narrative;
FloorIcon = CurrentEvent.Icon;
FloorImage = CurrentEvent.ImageFile ?? "";
EventType = CurrentEvent.EventType;
```

ทำงาน:

- โหลดข้อมูล floor จาก `StoryService`
- เอาค่ามาใส่ property ที่ XAML bind อยู่
- UI จึง refresh อัตโนมัติ

### Back-end: โหลด story จาก JSON

ใน `StoryService`

```csharp
public Task<StoryEvent?> GetCurrentFloorEventAsync(Player player)
{
    return _seedDataService.GetFloorAsync(player.CurrentFloor);
}
```

ใน `SeedDataService`

```csharp
var json = await LoadRawFileAsync("floors.json");
_floors = JsonConvert.DeserializeObject<List<StoryEvent>>(json) ?? new();
```

ทำไมใช้ JSON:

- เนื้อเรื่อง 30 ชั้นเป็น data ไม่ใช่ logic
- แก้เนื้อเรื่องโดยไม่ต้องแก้ C#
- เพิ่ม choice, item reward, image, ending ได้จาก JSON

### ViewModel: เมื่อผู้เล่นเลือก Choice

```csharp
outcome = await _storyService.ApplyChoiceAsync(Player, CurrentEvent, choice.Index);

if (outcome.Success)
{
    Player = await _playerService.GetActivePlayerAsync();
    UpdateTopBar();
}
```

ทำงาน:

- ส่ง choice index ไปให้ `StoryService`
- Service คำนวณผล
- โหลด player ใหม่จาก database เพื่อ update HP/Gold/Floor

### Back-end: ApplyChoiceAsync

```csharp
var lockReason = await GetChoiceLockReasonAsync(player, choice);
if (lockReason != null)
{
    outcome.Success = false;
    outcome.RejectMessage = lockReason;
    return outcome;
}
```

ทำงาน:

- ตรวจว่า choice เลือกได้ไหม
- เช่น ต้องมี item, ต้องไม่มี item บางชิ้น, gold พอไหม

```csharp
if (outcome.HpDelta != 0)
    await _playerService.ApplyHpDeltaAsync(player, outcome.HpDelta);

if (outcome.GoldDelta != 0)
    await _playerService.ApplyGoldDeltaAsync(player, outcome.GoldDelta);
```

ทำงาน:

- ปรับ HP และ Gold ตามผลลัพธ์ choice

```csharp
if (!string.IsNullOrWhiteSpace(choice.ItemReward) && choice.ItemRewardQuantity > 0)
{
    await _inventoryService.AddItemAsync(player.PlayerId, choice.ItemReward, choice.ItemRewardQuantity);
}
```

ทำงาน:

- ถ้า choice ให้ item จะเพิ่มเข้า inventory

```csharp
if (!string.IsNullOrWhiteSpace(storyEvent.StoryJournalKey))
{
    var journal = await _journalService.UnlockStoryJournalAsync(player.PlayerId, storyEvent.StoryJournalKey);
}
```

ทำงาน:

- ถ้า floor มี story journal key จะ unlock journal

```csharp
await SaveProgressAsync(player.PlayerId, storyEvent.FloorNumber, choiceIndex, choice.Text);
```

ทำงาน:

- บันทึกว่าชั้นนี้เลือก choice ไหน

```csharp
if (outcome.ShouldAdvanceFloor)
{
    await _playerService.AdvanceFloorAsync(player);
}
```

ทำงาน:

- เลื่อนไป floor ถัดไป

สรุปเส้นทาง Story:

```text
StoryPage.xaml
    -> bind FloorTitle / Narrative / Choices
StoryPage.xaml.cs
    -> StoryViewModel.OnAppearingAsync()
StoryViewModel
    -> PlayerService.GetActivePlayerAsync()
    -> StoryService.GetCurrentFloorEventAsync()
StoryService
    -> SeedDataService.GetFloorAsync()
        -> Resources/Raw/floors.json

เมื่อกด choice:
StoryPage.xaml Button
    -> SelectChoiceCommand
        -> StoryViewModel.SelectChoiceAsync()
            -> StoryService.ApplyChoiceAsync()
                -> PlayerService update HP/Gold/EXP/Floor
                -> InventoryService add/consume item
                -> JournalService unlock story journal
                -> StoryProgress save to SQLite
```

## 8. Inventory Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/InventoryPage.xaml`
- `Views/InventoryPage.xaml.cs`
- `ViewModels/InventoryViewModel.cs`
- `Services/InventoryService.cs`
- `Services/PlayerService.cs`
- `Models/InventoryItem.cs`
- `Models/Item.cs`

### Front-end: ค้นหาและ filter

```xml
<SearchBar Text="{Binding SearchText, Mode=TwoWay}"
           Placeholder="ค้นหา item..." />

<Button Text="ทั้งหมด"
        Command="{Binding SetFilterCommand}"
        CommandParameter="All" />

<Button Text="ฟื้นพลัง"
        Command="{Binding SetFilterCommand}"
        CommandParameter="Healing" />
```

ทำงาน:

- SearchBar bind สองทางกับ `SearchText`
- พิมพ์ค้นหาแล้ว ViewModel filter list
- ปุ่ม filter ส่ง parameter เช่น `Healing`, `Key`, `Story`

ทำไมใช้ `Mode=TwoWay`:

- UI ส่งค่าที่พิมพ์กลับไป ViewModel
- ViewModel ใช้ค่านั้นค้นหา item

### Front-end: แสดง item list

```xml
<CollectionView ItemsSource="{Binding Items}">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:InventoryItem">
            <Border>
                <Border.GestureRecognizers>
                    <TapGestureRecognizer
                        Command="{Binding BindingContext.ShowItemDetailCommand, Source={x:Reference InventoryRoot}}"
                        CommandParameter="{Binding .}" />
                </Border.GestureRecognizers>

                <Image Source="{Binding DisplayImage}"
                       IsVisible="{Binding HasDisplayImage}" />

                <Label Text="{Binding DisplayName}" />
                <Label Text="{Binding DisplayDescription}" />
                <Label Text="{Binding Quantity, StringFormat='x{0}'}" />
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

ทำงาน:

- `Items` คือ ObservableCollection จาก ViewModel
- แต่ละ item แสดงรูป, ชื่อ, คำอธิบาย, จำนวน
- แตะ item แล้วเรียก `ShowItemDetailCommand`

ทำไมใช้ `CollectionView`:

- เหมาะกับ list/grid ที่มีหลายรายการ
- render item ตาม collection อัตโนมัติ
- ถ้า `Items` เปลี่ยน UI เปลี่ยนตาม

### Code-behind

```csharp
public InventoryPage(InventoryViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;
}

protected override async void OnAppearing()
{
    base.OnAppearing();
    if (BindingContext is InventoryViewModel vm)
    {
        await vm.OnAppearingAsync();
    }
}
```

ทำงาน:

- ผูก ViewModel
- โหลด inventory ทุกครั้งที่เปิดหน้า
- จำเป็นเพราะ story อาจให้ item ใหม่ก่อนเข้าหน้านี้

### ViewModel: โหลด Inventory

```csharp
Player = await _playerService.GetActivePlayerAsync();

PlayerName = Player.PlayerName;
Gold = Player.Gold;

_allItems = await _inventoryService.GetByPlayerAsync(Player.PlayerId);
ApplyFilterAndSearch();
```

ทำงาน:

- โหลด player ปัจจุบัน
- โหลด item ทั้งหมดของ player
- เก็บไว้ใน `_allItems`
- filter/search แล้วใส่ใน `Items` ที่ UI bind อยู่

ทำไมมี `_allItems`:

- เก็บข้อมูลเต็มไว้ใน memory
- เวลาค้นหาหรือ filter ไม่ต้อง query database ทุกครั้ง

### ViewModel: filter และ search

```csharp
partial void OnSearchTextChanged(string value) => ApplyFilterAndSearch();
partial void OnActiveFilterChanged(string value) => ApplyFilterAndSearch();
```

ทำงาน:

- เมื่อ `SearchText` หรือ `ActiveFilter` เปลี่ยน จะ filter ทันที

```csharp
if (!string.IsNullOrWhiteSpace(SearchText))
{
    var keyword = SearchText.Trim().ToLowerInvariant();
    result = result.Where(i => i.DisplayName.ToLowerInvariant().Contains(keyword)
                            || i.DisplayDescription.ToLowerInvariant().Contains(keyword));
}
```

ทำงาน:

- ค้นหาจากชื่อและคำอธิบาย item

### Back-end: InventoryService

Read:

```csharp
var inventory = await conn.Table<InventoryItem>()
    .Where(i => i.PlayerId == playerId && i.Quantity > 0)
    .OrderByDescending(i => i.AcquiredAt)
    .ToListAsync();
```

ทำงาน:

- อ่าน item ของ player ที่ quantity มากกว่า 0

```csharp
var items = await conn.Table<Item>()
    .Where(i => itemIds.Contains(i.ItemId))
    .ToListAsync();

foreach (var inv in inventory)
{
    if (itemMap.TryGetValue(inv.ItemId, out var data))
        inv.ItemData = data;
}
```

ทำงาน:

- โหลดข้อมูล master item
- map เข้า `InventoryItem.ItemData`

ทำไมต้อง map เอง:

- sqlite-net-pcl ไม่ได้ทำ relationship join อัตโนมัติ
- จึงโหลด Item master แล้วผูกกับ InventoryItem เอง

Create:

```csharp
await _inventoryService.AddItemAsync(player.PlayerId, itemName, quantity);
```

ใช้ตอน story reward เพิ่ม item

Update:

```csharp
item.Quantity = newQuantity;
await _databaseService.UpdateAsync(item);
```

ใช้ตอนใช้ item หรือปรับ quantity

Delete:

```csharp
await _databaseService.DeleteAsync(item);
```

ใช้ตอนทิ้ง item หรือ quantity เหลือ 0

สรุป Inventory CRUD:

```text
InventoryPage.xaml
    -> SearchText / SetFilterCommand / ShowItemDetailCommand
InventoryViewModel
    -> InventoryService.GetByPlayerAsync()
        -> SQLite InventoryItems
        -> SQLite Items
    -> InventoryService.UpdateQuantityAsync()
    -> InventoryService.DeleteAsync()
```

## 9. Journal Page และ Journal Editor

ไฟล์ที่เกี่ยวข้อง:

- `Views/JournalPage.xaml`
- `Views/JournalEditorPage.xaml`
- `ViewModels/JournalViewModel.cs`
- `ViewModels/JournalEditorViewModel.cs`
- `Services/JournalService.cs`
- `Models/Journal.cs`
- `Resources/Raw/story_journals.json`

### Front-end: Journal tabs

```xml
<Button Text="{Binding StoryCount, StringFormat='เนื้อเรื่อง ({0})'}"
        Command="{Binding SwitchTabCommand}"
        CommandParameter="Story" />

<Button Text="{Binding PlayerCount, StringFormat='ของฉัน ({0})'}"
        Command="{Binding SwitchTabCommand}"
        CommandParameter="Player" />
```

ทำงาน:

- แยก tab ระหว่าง Story Journal และ Player Journal
- กด tab แล้วส่ง parameter ไป ViewModel

### Front-end: Search และ list

```xml
<Entry Text="{Binding SearchText, Mode=TwoWay}"
       Placeholder="ค้นหาบันทึก..." />

<CollectionView ItemsSource="{Binding Journals}">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:Journal">
            <Border>
                <Border.GestureRecognizers>
                    <TapGestureRecognizer
                        Command="{Binding BindingContext.OpenJournalCommand, Source={x:Reference JournalRoot}}"
                        CommandParameter="{Binding .}" />
                </Border.GestureRecognizers>

                <Label Text="{Binding Title}" />
                <Label Text="{Binding FloorNumber, StringFormat='ชั้น {0}'}" />
                <Label Text="{Binding Content}" />
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

ทำงาน:

- แสดง journal ทั้งหมดใน collection
- แตะ journal แล้วเปิดรายละเอียดหรือหน้าแก้ไข

### Front-end: Add button

```xml
<Button Text="เขียนบันทึกใหม่"
        Command="{Binding AddJournalCommand}"
        IsVisible="{Binding CanAddJournal}" />
```

ทำงาน:

- ปุ่มเพิ่ม journal แสดงเฉพาะ tab Player
- Story Journal เป็น read-only จึงไม่ควรมีปุ่มเพิ่มเอง

### Back-end: JournalService

Create Player Journal:

```csharp
var journal = new Journal
{
    PlayerId = playerId,
    JournalType = JournalType.Player,
    FloorNumber = floorNumber,
    Title = title.Trim(),
    Content = content.Trim(),
};

await conn.InsertAsync(journal);
```

Read:

```csharp
return await conn.Table<Journal>()
    .Where(j => j.PlayerId == playerId)
    .OrderBy(j => j.FloorNumber)
    .ToListAsync();
```

Update:

```csharp
if (journal.JournalType == JournalType.Story)
    throw new InvalidOperationException("Story Journal ไม่สามารถแก้ไขได้");

journal.Title = newTitle.Trim();
journal.Content = newContent.Trim();
await _databaseService.UpdateAsync(journal);
```

Delete:

```csharp
if (journal.JournalType == JournalType.Story)
    throw new InvalidOperationException("Story Journal ไม่สามารถลบได้");

return _databaseService.DeleteAsync(journal);
```

ทำไม Story Journal ห้ามแก้/ลบ:

- Story Journal เป็นบันทึกจากเนื้อเรื่อง
- ใช้แสดงความคืบหน้าที่ระบบปลดล็อกให้
- ส่วน Player Journal เป็นของผู้เล่น จึง CRUD ได้ครบ

Unlock Story Journal:

```csharp
var seed = await _seedDataService.GetStoryJournalByKeyAsync(storyKey);

var journal = new Journal
{
    PlayerId = playerId,
    JournalType = JournalType.Story,
    FloorNumber = seed.FloorNumber,
    Title = seed.Title,
    Content = seed.Content,
    StoryKey = seed.Key,
};

await conn.InsertAsync(journal);
```

ทำงาน:

- StoryService ส่ง `StoryJournalKey`
- JournalService อ่าน seed จาก `story_journals.json`
- บันทึกลงตาราง Journals

สรุป Journal:

```text
JournalPage.xaml
    -> SwitchTabCommand / OpenJournalCommand / AddJournalCommand
JournalViewModel
    -> JournalService.GetByPlayerAndTypeAsync()
    -> Shell.GoToAsync("journalEditor")
JournalEditorViewModel
    -> JournalService.CreatePlayerJournalAsync()
    -> JournalService.UpdatePlayerJournalAsync()
    -> JournalService.DeletePlayerJournalAsync()
```

## 10. Character Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/CharacterPage.xaml`
- `ViewModels/CharacterViewModel.cs`
- `Services/PlayerService.cs`
- `Services/StoryService.cs`

หลักการทำงาน:

- UI แสดง stat ของ active player เช่น HP, Attack, Defense, Gold, Level, CurrentFloor
- ViewModel โหลด active player จาก `PlayerService`
- ถ้าต้องแสดง progress จะใช้ `StoryService.GetProgressAsync`

โค้ดสำคัญในแนวเดียวกัน:

```csharp
Player = await _playerService.GetActivePlayerAsync();
```

ทำไมหน้า Character ไม่แก้ข้อมูลเอง:

- เป็นหน้าแสดงสถานะ
- stat ถูกเปลี่ยนจาก story choice หรือ item use ผ่าน service
- ช่วยให้ source of truth อยู่ที่ database และ service เดียวกัน

เส้นทาง:

```text
CharacterPage.xaml
    -> CharacterViewModel.OnAppearingAsync()
        -> PlayerService.GetActivePlayerAsync()
            -> SQLite Players
```

## 11. Save / Load Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/SaveLoadPage.xaml`
- `ViewModels/SaveLoadViewModel.cs`
- `Services/SaveLoadService.cs`
- `Models/SaveData.cs`

### Back-end: Save

```csharp
var player = await _playerService.GetActivePlayerAsync();
var inventory = await _inventoryService.GetByPlayerAsync(player.PlayerId);
var journals = await _journalService.GetByPlayerAsync(player.PlayerId);
var progress = await conn.Table<StoryProgress>()
    .Where(p => p.PlayerId == player.PlayerId)
    .ToListAsync();
```

ทำงาน:

- รวมสถานะเกมทั้งหมดของ player

```csharp
var playerJson = JsonConvert.SerializeObject(player);
var inventoryJson = JsonConvert.SerializeObject(inventory);
var journalJson = JsonConvert.SerializeObject(journals);
var progressJson = JsonConvert.SerializeObject(progress);
```

ทำงาน:

- แปลงสถานะเป็น JSON snapshot

```csharp
saveData.PlayerSnapshot = playerJson;
saveData.InventorySnapshot = inventoryJson;
saveData.JournalSnapshot = journalJson;
saveData.ProgressSnapshot = progressJson;
```

ทำไมใช้ JSON snapshot:

- เก็บสถานะทั้งเกมใน save slot ได้ง่าย
- restore กลับมาได้ครบทั้ง player, inventory, journal, progress
- เหมาะกับ save slot ของเกม

### Back-end: Load

```csharp
var loadedPlayer = JsonConvert.DeserializeObject<Player>(save.PlayerSnapshot);
loadedPlayer.PlayerId = 0;
await conn.InsertAsync(loadedPlayer);
```

ทำงาน:

- อ่าน player จาก JSON
- reset `PlayerId = 0` เพื่อให้ SQLite สร้าง id ใหม่

ทำไม reset id:

- ป้องกัน primary key ชนกับข้อมูลเดิม
- save เดิมอาจมี id ที่มีอยู่แล้วใน database ปัจจุบัน

```csharp
await SessionManager.SetActivePlayerIdAsync(newPlayerId);
```

ทำงาน:

- ตั้ง player ที่ load มาเป็นตัวที่กำลังเล่น

เส้นทาง Save/Load:

```text
SaveLoadPage.xaml
    -> SaveLoadViewModel
        -> SaveLoadService.SaveToSlotAsync()
            -> PlayerService / InventoryService / JournalService
            -> Serialize JSON
            -> SQLite SaveData

Load:
SaveLoadViewModel
    -> SaveLoadService.LoadFromSlotAsync()
        -> Deserialize JSON
        -> Insert Players / InventoryItems / Journals / StoryProgress
        -> Set active player
```

## 12. Backup Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/BackupPage.xaml`
- `ViewModels/BackupViewModel.cs`
- `Services/BackupService.cs`
- `Models/BackupData.cs`

### Export

```csharp
var data = await BuildBackupDataAsync(userId);
var json = JsonConvert.SerializeObject(data, _jsonSettings);
await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);
```

ทำงาน:

- รวมข้อมูล user ทั้งหมด
- แปลงเป็น JSON
- เขียนเป็นไฟล์ backup

### BuildBackupDataAsync

```csharp
var user = await conn.Table<User>().Where(u => u.UserId == userId).FirstOrDefaultAsync();
var players = await conn.Table<Player>().Where(p => p.UserId == userId).ToListAsync();
var inventory = await conn.Table<InventoryItem>().Where(i => playerIds.Contains(i.PlayerId)).ToListAsync();
var journals = await conn.Table<Journal>().Where(j => playerIds.Contains(j.PlayerId)).ToListAsync();
var saves = await conn.Table<SaveData>().Where(s => s.UserId == userId).ToListAsync();
```

ทำงาน:

- ดึงข้อมูลทั้งหมดที่เกี่ยวกับ user
- รวมเป็น `BackupData`

ต่างจาก Save/Load:

- Save/Load คือ save slot ของเกม
- Backup คือ export/import ข้อมูลผู้ใช้ออกเป็นไฟล์

### Import / Restore

```csharp
await conn.ExecuteAsync("DELETE FROM Players WHERE UserId = ?", userId);
await conn.ExecuteAsync("DELETE FROM SaveData WHERE UserId = ?", userId);
```

ทำงาน:

- ลบข้อมูลเดิมของ user ก่อน restore

```csharp
p.PlayerId = 0;
p.UserId = userId;
await conn.InsertAsync(p);
idMap[oldId] = p.PlayerId;
```

ทำงาน:

- insert player ใหม่
- map id เก่าไป id ใหม่

ทำไมต้อง map id:

- ข้อมูล inventory/journal/progress อ้างถึง PlayerId เดิม
- เมื่อ restore แล้ว PlayerId ใหม่เปลี่ยน จึงต้องแปลง id ให้ถูก

## 13. Settings Page และ Audio

ไฟล์ที่เกี่ยวข้อง:

- `Views/SettingsPage.xaml`
- `ViewModels/SettingsViewModel.cs`
- `Services/SettingsService.cs`
- `Services/AudioService.cs`

หลักการ:

- Settings ใช้ `Preferences` แทน SQLite เพราะเป็นค่าขนาดเล็ก
- เช่น เปิด/ปิด BGM, เปิด/ปิด SFX, volume

ตัวอย่างค่าคงที่:

```csharp
public const string PrefBgmVolume = "pref_bgm_volume";
public const string PrefSfxVolume = "pref_sfx_volume";
public const string PrefBgmEnabled = "pref_bgm_enabled";
public const string PrefSfxEnabled = "pref_sfx_enabled";
```

ทำไมใช้ Preferences:

- เป็น key-value storage ของ MAUI
- เหมาะกับ setting เล็ก ๆ
- ไม่จำเป็นต้องสร้าง table เพิ่มใน SQLite

Audio เชื่อมกับ UI:

```csharp
await _audioService.PlayBgmAsync("Audio/Bgm/main_menu_login_register.mp3");
```

ใช้ใน Page `OnAppearing` เพื่อเล่นเพลงตามหน้า

ใน `AppShell.xaml.cs` มีการ hook ปุ่ม:

```csharp
button.Clicked += async (_, _) =>
    await audioService.PlaySfxAsync(AudioService.DefaultButtonClickSfx);
```

ทำงาน:

- ทุกปุ่มที่เจอใน visual tree จะมีเสียงคลิก
- ลดการเขียนเสียงปุ่มซ้ำในทุกหน้า

## 14. Ending Page

ไฟล์ที่เกี่ยวข้อง:

- `Views/EndingPage.xaml`
- `ViewModels/EndingViewModel.cs`
- `Services/PlayerService.cs`

Story ส่งไป Ending แบบ query string:

```csharp
await Shell.Current.GoToAsync($"{AppConstants.RouteEnding}?type={safeType}");
```

ทำงาน:

- ส่ง ending type เช่น Good หรือ Bad ไปหน้า Ending

ใน StoryService:

```csharp
await _playerService.CompleteGameAsync(player, choice.EndingType);
```

ทำงาน:

- บันทึกว่าเกมจบแล้ว
- เก็บ ending type ใน player

ทำไมต้องเก็บ ending ใน database:

- กลับมาเปิดเกมใหม่ยังรู้ว่า player นี้จบแบบไหน
- หน้า Ending สามารถแสดงผลตามสถานะจริง

## 15. DatabaseService: จุดกลางของ SQLite

ไฟล์ `Services/DatabaseService.cs`

```csharp
_connection = new SQLiteAsyncConnection(
    DatabasePath,
    SQLiteOpenFlags.ReadWrite |
    SQLiteOpenFlags.Create |
    SQLiteOpenFlags.SharedCache);
```

ทำงาน:

- เปิด connection ไปยังไฟล์ `abyss_tower.db3`
- ถ้ายังไม่มี database จะสร้างใหม่

```csharp
await _connection.CreateTableAsync<User>();
await _connection.CreateTableAsync<Player>();
await _connection.CreateTableAsync<Item>();
await _connection.CreateTableAsync<InventoryItem>();
await _connection.CreateTableAsync<Journal>();
await _connection.CreateTableAsync<SaveData>();
await _connection.CreateTableAsync<StoryProgress>();
```

ทำงาน:

- สร้าง table จาก model class
- ถ้ามีอยู่แล้วจะไม่ลบข้อมูลเดิม

ทำไมใช้ sqlite-net-pcl:

- map C# class เป็น SQLite table ได้ง่าย
- beginner friendly
- เหมาะกับ MAUI app แบบ local database

Generic CRUD:

```csharp
public async Task<int> InsertAsync<T>(T entity) where T : new()
{
    var conn = await GetConnectionAsync();
    return await conn.InsertAsync(entity);
}
```

ทำงาน:

- service อื่นสามารถเรียก insert/update/delete ผ่าน DatabaseService ได้
- ลดโค้ดซ้ำ

## 16. Model และความสัมพันธ์ข้อมูล

### User -> Player

```csharp
[Table("Players")]
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int PlayerId { get; set; }

    [Indexed, NotNull]
    public int UserId { get; set; }
}
```

ความสัมพันธ์:

```text
User 1 คน -> มี Player ได้หลายตัว
Player.UserId -> ชี้กลับไป User.UserId
```

### Player -> InventoryItem -> Item

```csharp
public class InventoryItem
{
    public int PlayerId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    [Ignore]
    public Item? ItemData { get; set; }
}
```

ความสัมพันธ์:

```text
Player 1 คน -> มี InventoryItem หลายรายการ
InventoryItem 1 รายการ -> อ้าง Item master 1 ชิ้น
```

ทำไมแยก `Item` กับ `InventoryItem`:

- `Item` คือ master data เช่น ชื่อ, ประเภท, คำอธิบาย
- `InventoryItem` คือ item ที่ player ถือจริง เช่น quantity
- ลดข้อมูลซ้ำ

### Player -> Journal

```csharp
public class Journal
{
    public int PlayerId { get; set; }
    public JournalType JournalType { get; set; }
    public int FloorNumber { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
```

ความสัมพันธ์:

```text
Player 1 คน -> มี Journal หลายรายการ
Journal มี 2 ประเภท: Story และ Player
```

## 17. คำตอบสรุปเวลาอาจารย์ถาม

### ถาม: Front-end กับ Back-end เชื่อมกันตรงไหน?

ตอบ:

> เชื่อมกันผ่าน BindingContext ครับ เช่น `StoryPage.xaml.cs` ตั้ง `BindingContext = viewModel` แล้ว XAML bind property และ command จาก ViewModel เช่น `FloorTitle`, `Narrative`, `SelectChoiceCommand` จากนั้น ViewModel เรียก Service เช่น `StoryService` เพื่อทำ logic และ Service ไปอ่าน SQLite หรือ JSON อีกที

### ถาม: ทำไม UI ไม่เรียก database ตรง ๆ?

ตอบ:

> เพราะใช้ MVVM ครับ UI มีหน้าที่แสดงผลกับรับ input เท่านั้น ถ้า UI ติดต่อ database เอง โค้ดจะปนกันและแก้ยาก เลยให้ ViewModel เป็นตัวกลาง และให้ Service ดูแล database/logic

### ถาม: ทำไมใช้ JSON กับ story?

ตอบ:

> เพราะ story เป็นข้อมูลจำนวนมาก เช่น floor, narrative, choice, reward ถ้าเก็บใน C# จะเปลี่ยนยาก การแยกเป็น JSON ทำให้แก้เนื้อเรื่องได้โดยไม่ต้องแก้ logic

### ถาม: CRUD อยู่ตรงไหน?

ตอบ:

> Inventory CRUD อยู่ใน `InventoryService`: เพิ่ม item, อ่าน item, update quantity, delete item ส่วน Journal CRUD อยู่ใน `JournalService`: สร้าง อ่าน แก้ ลบ Player Journal โดย Story Journal เป็น read-only เพราะมาจากเนื้อเรื่อง

### ถาม: Save/Load ทำงานยังไง?

ตอบ:

> Save จะเอา player, inventory, journal, progress มา serialize เป็น JSON แล้วเก็บใน `SaveData` แต่ Load จะ deserialize JSON กลับมา สร้าง player ใหม่ แล้ว restore inventory/journal/progress และตั้ง active player ใหม่

### ถาม: ทำไมใช้ async/await?

ตอบ:

> เพราะงาน database, file, navigation เป็นงานที่อาจใช้เวลา ถ้าไม่ใช้ async UI อาจค้าง จึงใช้ async/await เพื่อให้แอปยังตอบสนองระหว่างทำงาน

### ถาม: ทำไมต้องมี Service หลายตัว?

ตอบ:

> เพื่อแยกหน้าที่ครับ `AuthService` ดู login/register, `StoryService` ดู choice logic, `InventoryService` ดู item, `JournalService` ดู journal, `SaveLoadService` ดู save slot ทำให้แต่ละไฟล์รับผิดชอบเรื่องเดียว อ่านง่ายและแก้ง่าย

## 18. Flow รวมทั้งโปรแกรม

```text
เปิดแอป
    -> AppShell
        -> SeedDataService.SeedAllAsync()
            -> DatabaseService.InitializeAsync()
            -> Seed items.json ลง SQLite
        -> ตรวจ Session

Login/Register
    -> AuthService
        -> Users table
        -> SessionManager

Main Menu
    -> MainMenuViewModel
        -> PlayerService / SaveLoadService

New Game
    -> PlayerService.CreateNewPlayerAsync()
        -> Players table
        -> Set active player

Story
    -> StoryViewModel
        -> StoryService
            -> SeedDataService อ่าน floors.json
            -> PlayerService update stat
            -> InventoryService add/consume item
            -> JournalService unlock story journal
            -> StoryProgress table

Inventory
    -> InventoryViewModel
        -> InventoryService
            -> InventoryItems table
            -> Items table

Journal
    -> JournalViewModel / JournalEditorViewModel
        -> JournalService
            -> Journals table
            -> story_journals.json เฉพาะตอน unlock

Save/Load
    -> SaveLoadViewModel
        -> SaveLoadService
            -> JSON snapshot
            -> SaveData table

Backup
    -> BackupViewModel
        -> BackupService
            -> Export/Import JSON file

Settings
    -> SettingsViewModel
        -> SettingsService
            -> Preferences

Ending
    -> EndingViewModel
        -> Player.EndingType
```


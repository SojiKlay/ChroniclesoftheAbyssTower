# UI FLOW

## Current Pages

The project currently includes these XAML pages:

- `LoginPage.xaml`
- `RegisterPage.xaml`
- `MainMenuPage.xaml`
- `IntroStoryPage.xaml`
- `StoryPage.xaml`
- `CharacterPage.xaml`
- `InventoryPage.xaml`
- `JournalPage.xaml`
- `JournalEditorPage.xaml`
- `SaveLoadPage.xaml`
- `BackupPage.xaml`
- `SettingsPage.xaml`
- `EndingPage.xaml`
- `MainPage.xaml`

## Main Navigation Flow

```text
LoginPage / RegisterPage
-> MainMenuPage
-> IntroStoryPage
-> StoryPage
-> EndingPage
```

## Gameplay Navigation

From Main Menu:
- New Game
- Continue
- Save/Load
- Backup
- Settings

From StoryPage:
- Character
- Inventory
- Journal
- Save/Load
- Settings
- Ending when story completes

From JournalPage:
- JournalEditorPage for create/edit player journal

## Routes

Routes are defined in `AppConstants.cs` and registered in `AppShell.xaml.cs`.

Absolute routes:
- `//login`
- `//register`
- `//main`

Relative routes:
- `introStory`
- `story`
- `character`
- `inventory`
- `journal`
- `journalEditor`
- `saveLoad`
- `backup`
- `settings`
- `ending`

## ViewModels

- `LoginViewModel`
- `RegisterViewModel`
- `MainMenuViewModel`
- `IntroStoryViewModel`
- `StoryViewModel`
- `CharacterViewModel`
- `InventoryViewModel`
- `JournalViewModel`
- `JournalEditorViewModel`
- `SaveLoadViewModel`
- `BackupViewModel`
- `SettingsViewModel`
- `EndingViewModel`

## UI Notes

- Keep UI beginner-friendly
- Keep XAML pages connected to ViewModels through existing DI pattern
- Avoid adding new pages without registering route, ViewModel and page in `MauiProgram.Registrations.cs`

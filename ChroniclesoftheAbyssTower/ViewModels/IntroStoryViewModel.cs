using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับ Intro Story (เนื้อเรื่องเริ่มเกม)
    /// แสดงเนื้อเรื่องเป็นหน้าๆ ก่อนเข้าหอคอย
    /// </summary>
    public partial class IntroStoryViewModel : BaseViewModel
    {
        // เนื้อเรื่องแบ่งเป็น page (จาก STORY.md บทที่ 1-6)
        private readonly List<string> _pages = new()
        {
            "ไม่มีใครจำวันที่ \"หมอกอเวจี\" ปรากฏขึ้นได้แน่ชัด\n\nทุกที่ที่หมอกผ่านไป\nชีวิตจะหายไปทีละน้อย\n\nไม่ใช่แค่ร่างกาย\nแต่รวมถึง \"ความทรงจำ\"\n\nเมืองหลายแห่งล่มสลาย\nผู้คนเริ่มหวาดกลัว",

            "อาริน อดีตอัศวินแห่งราชวงศ์\n\nในคืนที่เมืองหลวงถูกหมอกกลืนกิน\nเขากลับถูกกล่าวหาว่าเป็นต้นเหตุของหายนะ\n\nผู้คนหวาดกลัวพลังที่อยู่รอบตัวเขา\n\nสุดท้าย อารินถูกเนรเทศออกจากเมือง",

            "ก่อนออกจากเมือง\nอารินพยายามตามหาน้องสาวของเขา\n\n\"เอลีน่า\"\n\nแต่ไม่พบแม้แต่ร่องรอย\n\nสิ่งเดียวที่เหลืออยู่คือ\n... จี้ที่แตกหัก",

            "ผู้คนเล่าขานกันว่า\n\n\"ผู้ที่ขึ้นไปถึงจุดสูงสุด\nจะได้พบความจริงของโลก\"\n\nหลายคนเคยพยายามปีนหอคอย\n\nแต่ไม่มีใครกลับมา",

            "ในคืนที่ไร้แสงดาว\n\nเสียงหนึ่งดังขึ้นจากความมืด\n\n\"หากอยากรู้ความจริง\nจงปีนหอคอย\"\n\nหอคอยสีดำขนาดมหึมาปรากฏขึ้นจากหมอก\n\nและอารินก็ตัดสินใจก้าวเข้าไป",

            "ประตูเหล็กปิดลงด้านหลัง\n\n\"ไม่มีทางย้อนกลับอีกแล้ว\"\n\nชั้นแรกของหอคอยเต็มไปด้วย:\n• กับดัก\n• เงาปริศนา\n• ห้องลับ\n• บันทึกของผู้ที่จากไป\n\nทุกการตัดสินใจมีผลต่อการเอาชีวิตรอด..."
        };

        [ObservableProperty]
        private int currentPageIndex;

        [ObservableProperty]
        private string currentPageText = string.Empty;

        [ObservableProperty]
        private string pageIndicator = string.Empty;

        [ObservableProperty]
        private string currentPageImage = string.Empty;

        [ObservableProperty]
        private bool isLastPage;

        [ObservableProperty]
        private string nextButtonText = "ต่อไป →";

        public IntroStoryViewModel()
        {
            Title = "บทนำ";
            ShowPage(0);
        }

        private void ShowPage(int index)
        {
            CurrentPageIndex = Math.Clamp(index, 0, _pages.Count - 1);
            CurrentPageText = _pages[CurrentPageIndex];
            CurrentPageImage = $"intro_{CurrentPageIndex + 1:00}.jpg";
            PageIndicator = $"บทที่ {CurrentPageIndex + 1} / {_pages.Count}";
            IsLastPage = CurrentPageIndex == _pages.Count - 1;
            NextButtonText = IsLastPage ? "เข้าสู่หอคอย ⚔️" : "ต่อไป →";
        }

        [RelayCommand]
        private async Task NextAsync()
        {
            if (IsLastPage)
            {
                // จบ intro → ไปหน้า Story (Phase 4.5)
                await Shell.Current.GoToAsync(AppConstants.RouteStory);
            }
            else
            {
                ShowPage(CurrentPageIndex + 1);
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (CurrentPageIndex > 0)
                ShowPage(CurrentPageIndex - 1);
        }

        [RelayCommand]
        private async Task SkipAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "ข้ามบทนำ?",
                "ต้องการข้ามเนื้อเรื่องเริ่มต้นและเข้าหอคอยทันทีหรือไม่?",
                "ข้าม", "อ่านต่อ");
            if (!confirm) return;

            await Shell.Current.GoToAsync(AppConstants.RouteStory);
        }
    }
}

//using SmartFall.Platforms.Android;

namespace SmartFall
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}

        private void StartService_Clicked(object sender, EventArgs e)
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var intent = new Android.Content.Intent(context, typeof(SmartFall.Platforms.Android.SmartFallService));
            context.StartForegroundService(intent);
#endif
        }

        private void StopService_Clicked(object sender, EventArgs e)
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var intent = new Android.Content.Intent(context, typeof(SmartFall.Platforms.Android.SmartFallService));
            context.StopService(intent);
#endif
        }
    }

}

using M_Hiking.ViewModel;
//using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
//using Windows.Graphics.Display;

namespace M_Hiking
{
    public partial class MainPage : ContentPage
    {
        private readonly HikeViewModel _viewModel;

        public MainPage(HikeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadHikesAsync();
        }
    }
}
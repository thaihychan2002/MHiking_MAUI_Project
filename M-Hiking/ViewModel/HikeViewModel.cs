using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M_Hiking.Data;
using M_Hiking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Hiking.ViewModel
{
    public partial class HikeViewModel : ObservableObject
    {   private readonly DatabaseContext _context;
        public HikeViewModel(DatabaseContext context) 
        {
            _context = context;
        }
        [ObservableProperty]
        private ObservableCollection<Hikes> _hike = new();
        [ObservableProperty]
        private Hikes _operatingHike = new();
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private string _busyText;

        public async Task LoadHikesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var hike = await _context.GetAllAsync<Hikes>();
                if (hike is not null && hike.Any())
                {
                    Hike ??= new ObservableCollection<Hikes>();
                    foreach (var item in hike)
                    {
                        Hike.Add(item);
                    }
                }
            }, "Fetching Hike");
        }
        [RelayCommand]
        private void SetOperatingHike(Hikes ? hikes) => OperatingHike = hikes ?? new();
        [RelayCommand]
        private async Task SaveHikeAsync()
        {
            OperatingHike.Date = DateTime.Now;
            if (OperatingHike is null)
            {
                return;
            }
            var (isValid, errorMessage) = OperatingHike.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation error", errorMessage, "Ok");
                return;
            }
            var busyText = OperatingHike.Id == 0 ? "Creating Hike..." : "Updating Hike...";
            await ExecuteAsync(async () =>
            {

                if (OperatingHike.Id == 0)
                {
                    await _context.AddItemAsync<Hikes>(OperatingHike);
                    Hike.Add(OperatingHike);
                }
                else
                {
                    if(await _context.UpdateItemAsync<Hikes>(OperatingHike))
                    {
                        var HikeCopy = OperatingHike.Clone();
                        int index = Hike.IndexOf(OperatingHike);
                        Hike.RemoveAt(index);
                        Hike.Insert(index, HikeCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert(" error","Hike update failed", "Ok");
                        return;
                    }
                   
                }
                OperatingHike = new();
                SetOperatingHikeCommand.Execute(new());
            }, busyText);
        }
        [RelayCommand]
        private async Task DeleteHikeAsync(int id)
        {
            await ExecuteAsync(async () => {
                if (await _context.DeleteItemByKeyAsync<Hikes>(id))
                {
                    var hike = Hike.FirstOrDefault(predicate => predicate.Id == id);
                    Hike.Remove(hike);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Hike was not deleted", "Ok");
                }
            }, "Deleting Hike");
        }
        private async Task ExecuteAsync(Func<Task>operation, string ? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText?? "Processing...";
            try
            {
                await operation?.Invoke();
            }
            catch(Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing...";
            }
        }

    }
}

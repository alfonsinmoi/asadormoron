using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
// using Xamarin.Forms.OpenWhatsApp; // TODO: Reimplementar WhatsApp
namespace AsadorMoron.ViewModels.Clientes
{
    public class QuienViewModel : ViewModelBase
    {
        private ConfiguracionAdmin configuracionAdmin = new ConfiguracionAdmin();
        public QuienViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            App.DAUtil.EnTimer = false;
            configuracionAdmin = ResponseServiceWS.getConfiguracionAdmin();
            NumTelefono = configuracionAdmin.telefono;
            NumWhatsApp = configuracionAdmin.whatsapp;
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));

        }
        private string cuerpo = "";
        public string Cuerpo
        {
            get
            {
                return cuerpo;
            }
            set
            {
                if (cuerpo != value)
                {
                    cuerpo = value;
                    OnPropertyChanged(nameof(Cuerpo));
                }
            }
        }
        public ICommand cmdEmail { get { return new DelegateCommandAsync(EnviarMail); } }
        private async Task EnviarMail()
        {
            try
            {
                if (Cuerpo.Length > 80)
                {
                    try
                    {
                        List<string> lista = new List<string>();
                        lista.Add(configuracionAdmin.email);
                        var message = new EmailMessage
                        {
                            Subject = "Información App",
                            Body = Cuerpo,
                            To = lista
                        };
                        await Email.ComposeAsync(message);
                        Cuerpo = "";
                    }
                    catch (Exception)
                    {
                        await App.customDialog.ShowDialogAsync(AppResources.ErrorEnvioEmail, AppResources.App, AppResources.Cerrar);
                    }
                }
                else
                {
                    await App.customDialog.ShowDialogAsync(AppResources.ErrorMensajeCorto, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private string numTelefono;
        public string NumTelefono
        {
            get
            {
                return numTelefono;
            }
            set
            {
                if (numTelefono != value)
                {
                    numTelefono = value;
                    OnPropertyChanged(nameof(NumTelefono));
                }
            }
        }
        private string nuWhatsApp;
        public string NumWhatsApp
        {
            get
            {
                return nuWhatsApp;
            }
            set
            {
                if (nuWhatsApp != value)
                {
                    nuWhatsApp = value;
                    OnPropertyChanged(nameof(NumWhatsApp));
                }
            }
        }
        public ICommand Telefono { get { return new Command(Llamar); } }
        private void Llamar()
        {
            try
            {
                PhoneDialer.Open(configuracionAdmin.telefono.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public ICommand WhatsApp { get { return new Command(AbreWhatsApp); } }
        private async void AbreWhatsApp()
        {
            try
            {
                // WhatsApp via URL scheme
                await Launcher.Default.OpenAsync(new Uri($"https://wa.me/34{configuracionAdmin.whatsapp.Replace(" ", "")}"));
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}

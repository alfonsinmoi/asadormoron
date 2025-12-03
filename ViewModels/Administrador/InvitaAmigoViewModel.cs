using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class InvitaAmigoViewModel:ViewModelBase
    {
        private bool haEntrado = false;
        public InvitaAmigoViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                try
                {
                    if (!haEntrado)
                    {

                        Promo = ResponseServiceWS.getPromocionAmigo();

                        if (Promo != null)
                        {
                            Activo = Promo.activo;
                            Descripcion = Promo.descripcion;
                            FechaFin = Promo.fechaFin;
                            FechaInicio = Promo.fechaInicio;
                            PedidoMinimo = Promo.pedidoMinimo.ToString();
                            PersonasAlcanzadas = Promo.personasAlcanzadas;
                            SaldoAmigo =Promo.saldoAmigo.ToString();
                            SaldoRepartido = Promo.saldoRepartido.ToString();
                            SaldoUsuario = Promo.saldoUsuario.ToString();
                        }
                        else
                        {
                            Promo = new PromocionAmigoModel();
                            Promo.idPueblo = App.DAUtil.Usuario.idPueblo;
                            Promo.nombre = "INVITA A TU AMIGO";
                            Promo.personasAlcanzadas = 0;
                        }
                    }
                    haEntrado = true;
                }
                catch (Exception)
                {
                }
                App.DAUtil.EnTimer = false;
            }
            catch (Exception ex)
            {

                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }
        #region Propiedades
        private PromocionAmigoModel promo;
        public PromocionAmigoModel Promo
        {
            get
            {
                return promo;
            }
            set
            {
                if (promo != value)
                {
                    promo = value;
                    OnPropertyChanged(nameof(Promo));
                }
            }
        }
        private bool activo;
        public bool Activo
        {
            get
            {
                return activo;
            }
            set
            {
                if (activo != value)
                {
                    activo = value;
                    OnPropertyChanged(nameof(Activo));
                }
            }
        }
        private string descripcion;
        public string Descripcion
        {
            get
            {
                return descripcion;
            }
            set
            {
                if (descripcion != value)
                {
                    descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }
        private DateTime fechaInicio;
        public DateTime FechaInicio
        {
            get
            {
                return fechaInicio;
            }
            set
            {
                if (fechaInicio != value)
                {
                    fechaInicio = value;
                    OnPropertyChanged(nameof(FechaInicio));
                }
            }
        }
        private DateTime fechaFin;
        public DateTime FechaFin
        {
            get
            {
                return fechaFin;
            }
            set
            {
                if (fechaFin != value)
                {
                    fechaFin = value;
                    OnPropertyChanged(nameof(FechaFin));
                }
            }
        }
        private string pedidoMinimo;
        public string PedidoMinimo
        {
            get
            {
                return pedidoMinimo;
            }
            set
            {
                if (pedidoMinimo != value)
                {
                    pedidoMinimo = value;
                    OnPropertyChanged(nameof(PedidoMinimo));
                }
            }
        }
        private string saldoAmigo;
        public string SaldoAmigo
        {
            get
            {
                return saldoAmigo;
            }
            set
            {
                if (saldoAmigo != value)
                {
                    saldoAmigo = value;
                    OnPropertyChanged(nameof(SaldoAmigo));
                }
            }
        }
        private string saldoUsuario;
        public string SaldoUsuario
        {
            get
            {
                return saldoUsuario;
            }
            set
            {
                if (saldoUsuario != value)
                {
                    saldoUsuario = value;
                    OnPropertyChanged(nameof(SaldoUsuario));
                }
            }
        }
        private string saldoRepartido;
        public string SaldoRepartido
        {
            get
            {
                return saldoRepartido;
            }
            set
            {
                if (saldoRepartido != value)
                {
                    saldoRepartido = value;
                    OnPropertyChanged(nameof(SaldoRepartido));
                }
            }
        }
        private int personasAlcanzadas;
        public int PersonasAlcanzadas
        {
            get
            {
                return personasAlcanzadas;
            }
            set
            {
                if (personasAlcanzadas != value)
                {
                    personasAlcanzadas = value;
                    OnPropertyChanged(nameof(PersonasAlcanzadas));
                }
            }
        }
        #endregion
        #region Comandos
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        #endregion
        #region Métodos
        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Guardando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await initGuardar();
                App.userdialog.HideLoading();
                await App.DAUtil.NavigationService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task initGuardar()
        {
            try
            {
                Promo.activo = Activo;
                Promo.descripcion = Descripcion;
                Promo.fechaInicio = FechaInicio;
                Promo.fechaFin = FechaFin;
                Promo.pedidoMinimo = double.Parse(PedidoMinimo.Replace(".", ","));
                Promo.saldoAmigo = double.Parse(SaldoAmigo.Replace(".", ","));
                Promo.saldoUsuario = double.Parse(SaldoUsuario.Replace(".", ","));
                if (App.ResponseWS.editaCromoInvitaAmigo(Promo))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync("Proceso terminado correctamente", AppResources.App, AppResources.Aceptar);
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                }
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
        }
        #endregion
    }
}

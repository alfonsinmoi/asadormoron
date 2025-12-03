using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
// 
using AsadorMoron.Models;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Syncfusion.Maui.Charts;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Contabilidad
{
    public class HomeContabilidadViewModel:ViewModelBase
    {

        public HomeContabilidadViewModel()
        {
			Facturas = new ObservableCollection<FacturaEstInformeModel>(ResponseServiceWS.getFacturas());
            FacturaEstInformeModel f = new FacturaEstInformeModel();
            f.idPueblo = 1;
            f.poblacion = "Morón";
            f.total = 2400;
            f.year = 2022;
            Facturas.Add(f);
            f = new FacturaEstInformeModel();
            f.idPueblo = 2;
            f.poblacion = "Arahal";
            f.total = 400;
            f.year = 2022;
            Facturas.Add(f);
            f = new FacturaEstInformeModel();
            f.idPueblo = 3;
            f.poblacion = "Paradas";
            f.total = 200;
            f.year = 2022;
            Facturas.Add(f);
            f = new FacturaEstInformeModel();
            f.idPueblo = 5;
            f.poblacion = "La Roda";
            f.total = 24;
            f.year = 2022;
            Facturas.Add(f);
            f = new FacturaEstInformeModel();
            f.idPueblo = 6;
            f.poblacion = "Pedrera";
            f.total = 100;
            f.year = 2022;
            Facturas.Add(f);

        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!cargado)
                {
                    cargado = true;
                    App.DAUtil.EnTimer = false;
                    
                }
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }
        #region Propiedades
        private static bool cargado = false;
        private ObservableCollection<FacturaEstInformeModel> facturas;
        public ObservableCollection<FacturaEstInformeModel> Facturas
        {
            get
            {
                return facturas;
            }
            set
            {
                if (facturas != value)
                {
                    facturas = value;
                    OnPropertyChanged(nameof(Facturas));
                }
            }
        }
        #endregion
    }
}

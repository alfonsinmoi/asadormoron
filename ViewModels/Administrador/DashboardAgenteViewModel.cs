using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Views.Administrador;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    /// <summary>
    /// KPIs agregados del agente de voz para el gestor.
    /// </summary>
    public class DashboardAgenteViewModel : ViewModelBase
    {
        public DashboardAgenteViewModel()
        {
            Desde = DateTime.Today.AddDays(-30);
            Hasta = DateTime.Today;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            App.DAUtil.EnTimer = false;
            await CargarAsync();
            await base.InitializeAsync(navigationData);
        }

        public ICommand cmdRefrescar => new Command(async () => await CargarAsync());
        public ICommand cmdVerLlamadas => new Command(async () =>
            await App.DAUtil.NavigationService.NavigateToAsync<LlamadasViewModelAdmin>());

        private async Task CargarAsync()
        {
            try
            {
                try { App.userdialog.ShowLoading(Recursos.AppResources.Cargando); } catch { }

                var m = await App.ResponseWS.GetMetricasAgenteAsync(Desde, Hasta);
                if (m != null)
                {
                    Metricas = m;
                    TieneDatos = m.llamadas_totales > 0;
                    CeldasHeatmap = new System.Collections.ObjectModel.ObservableCollection<Models.HeatmapCeldaModel>(m.CalcularCeldasHeatmap());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error DashboardAgente: " + ex.Message);
            }
            finally
            {
                try { App.userdialog.HideLoading(); } catch { }
            }
        }

        private DateTime desde;
        public DateTime Desde { get => desde; set { desde = value; OnPropertyChanged(nameof(Desde)); } }

        private DateTime hasta;
        public DateTime Hasta { get => hasta; set { hasta = value; OnPropertyChanged(nameof(Hasta)); } }

        private MetricasDashboardModel metricas = new();
        public MetricasDashboardModel Metricas { get => metricas; set { metricas = value; OnPropertyChanged(nameof(Metricas)); } }

        private bool tieneDatos;
        public bool TieneDatos { get => tieneDatos; set { tieneDatos = value; OnPropertyChanged(nameof(TieneDatos)); } }

        private System.Collections.ObjectModel.ObservableCollection<Models.HeatmapCeldaModel> celdasHeatmap = new();
        public System.Collections.ObjectModel.ObservableCollection<Models.HeatmapCeldaModel> CeldasHeatmap
        {
            get => celdasHeatmap;
            set { celdasHeatmap = value; OnPropertyChanged(nameof(CeldasHeatmap)); }
        }
    }
}

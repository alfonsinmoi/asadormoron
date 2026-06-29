using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Models;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Views.Administrador;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    /// <summary>
    /// Listado paginado de llamadas del agente de voz con filtros básicos.
    /// </summary>
    public class LlamadasViewModelAdmin : ViewModelBase
    {
        public LlamadasViewModelAdmin()
        {
            Estados = new ObservableCollection<string> {
                "Todos", "completada", "transferida", "no_pedido", "fallida", "en_curso"
            };
            EstadoFiltro = "Todos";
            Desde = DateTime.Today.AddDays(-30);
            Hasta = DateTime.Today;
            Llamadas = new ObservableCollection<LlamadaModel>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            App.DAUtil.EnTimer = false;
            await CargarAsync();
            await base.InitializeAsync(navigationData)
                .ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                }));
        }

        #region Comandos
        public ICommand cmdFiltrar       => new Command(async () => await CargarAsync());
        public ICommand cmdRefrescar     => new Command(async () => await CargarAsync());
        public ICommand cmdSiguiente     => new Command(async () => { if (Page < Pages) { Page++; await CargarAsync(); } });
        public ICommand cmdAnterior      => new Command(async () => { if (Page > 1)     { Page--; await CargarAsync(); } });
        public ICommand cmdVerDetalle    => new Command<LlamadaModel>(async (m) => await IrDetalle(m));
        #endregion

        #region Métodos
        private async Task CargarAsync()
        {
            try
            {
                try { App.userdialog.ShowLoading(Recursos.AppResources.Cargando); } catch { }

                var estado = EstadoFiltro == "Todos" ? null : EstadoFiltro;
                var data = await App.ResponseWS.GetLlamadasAgenteAsync(
                    desde: Desde,
                    hasta: Hasta,
                    estado: estado,
                    telefono: string.IsNullOrWhiteSpace(TelefonoFiltro) ? null : TelefonoFiltro,
                    page: Page,
                    perPage: 20);

                Llamadas.Clear();
                if (data?.items != null)
                {
                    foreach (var item in data.items)
                        Llamadas.Add(item);
                }
                Total = data?.total ?? 0;
                Pages = data?.pages ?? 0;
                Vacio = Llamadas.Count == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error CargarAsync llamadas: " + ex.Message);
            }
            finally
            {
                try { App.userdialog.HideLoading(); } catch { }
            }
        }

        private async Task IrDetalle(LlamadaModel l)
        {
            if (l == null) return;
            await App.DAUtil.NavigationService.NavigateToAsync<DetalleLlamadaViewModelAdmin>(l.id);
        }
        #endregion

        #region Propiedades
        private DateTime desde;
        public DateTime Desde { get => desde; set { desde = value; OnPropertyChanged(nameof(Desde)); } }

        private DateTime hasta;
        public DateTime Hasta { get => hasta; set { hasta = value; OnPropertyChanged(nameof(Hasta)); } }

        private string estadoFiltro;
        public string EstadoFiltro { get => estadoFiltro; set { estadoFiltro = value; OnPropertyChanged(nameof(EstadoFiltro)); } }

        private string telefonoFiltro;
        public string TelefonoFiltro { get => telefonoFiltro; set { telefonoFiltro = value; OnPropertyChanged(nameof(TelefonoFiltro)); } }

        public ObservableCollection<string> Estados { get; }

        private ObservableCollection<LlamadaModel> llamadas;
        public ObservableCollection<LlamadaModel> Llamadas { get => llamadas; set { llamadas = value; OnPropertyChanged(nameof(Llamadas)); } }

        private int page = 1;
        public int Page { get => page; set { page = value; OnPropertyChanged(nameof(Page)); } }

        private int pages;
        public int Pages { get => pages; set { pages = value; OnPropertyChanged(nameof(Pages)); } }

        private int total;
        public int Total { get => total; set { total = value; OnPropertyChanged(nameof(Total)); } }

        private bool vacio = true;
        public bool Vacio { get => vacio; set { vacio = value; OnPropertyChanged(nameof(Vacio)); } }
        #endregion
    }
}

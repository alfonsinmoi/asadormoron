using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class FacturasAdministradorViewModel:ViewModelBase
    {
        public FacturasAdministradorViewModel()
        {

        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }


                
                CargarPueblos();
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }
        #region Propiedades
        private List<FacturaAdministradorModel> ListadoFacturasTemp;
        private bool esMultiAdmin;
        public bool EsMultiAdmin
        {
            get
            {
                return esMultiAdmin;
            }
            set
            {
                if (esMultiAdmin != value)
                {
                    esMultiAdmin = value;
                    OnPropertyChanged(nameof(EsMultiAdmin));
                }
            }
        }
        private ObservableCollection<PueblosModel> pueblos;
        public ObservableCollection<PueblosModel> Pueblos
        {
            get
            {
                return pueblos;
            }
            set
            {
                if (pueblos != value)
                {
                    pueblos = value;
                    OnPropertyChanged(nameof(Pueblos));

                }
            }
        }
        private PueblosModel puebloSeleccionado;
        public PueblosModel PuebloSeleccionado
        {
            get
            {
                return puebloSeleccionado;
            }
            set
            {
                if (puebloSeleccionado != value)
                {
                    puebloSeleccionado = value;
                    OnPropertyChanged(nameof(PuebloSeleccionado));
                    if (PuebloSeleccionado != null)
                        CargaFacturas();
                }
            }
        }
        private void CargarPueblos()
        {
            try
            {

                Pueblos = new ObservableCollection<PueblosModel>(ResponseServiceWS.getPueblosAdministrador());
                EsMultiAdmin = Pueblos.Count > 1;
                PuebloSeleccionado = Pueblos[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private DateTime desde;
        public DateTime Desde
        {
            get
            {
                return desde;
            }
            set
            {
                if (desde != value)
                {

                    desde = value;
                    OnPropertyChanged(nameof(Desde));
                    if (value > hasta)
                    {
                        Hasta = value;
                    }
                    else
                    {
                        Filtro();
                    }

                }
            }
        }
        private DateTime hasta;
        public DateTime Hasta
        {
            get
            {
                return hasta;
            }
            set
            {
                if (hasta != value)
                {
                    hasta = value;
                    OnPropertyChanged(nameof(Hasta));
                    if (value < desde)
                    {
                        Desde = value;
                    }
                    else
                    {
                        Filtro();
                    }

                }
            }
        }
        private List<FacturaAdministradorModel> listado;
        public List<FacturaAdministradorModel> Listado
        {
            get { return listado; }
            set
            {
                if (listado != value)
                {
                    listado = value;
                    OnPropertyChanged(nameof(Listado));
                }
            }
        }
        #endregion
        #region Métodos
        private void CargaFacturas()
        {
            try
            {
                Desde = DateTime.Now.AddYears(-1);
                Hasta = DateTime.Now;
                App.DAUtil.pedidoNuevo = false;
                ListadoFacturasTemp = new List<FacturaAdministradorModel>(ResponseServiceWS.getFacturasAdministrador(PuebloSeleccionado.id));
                Filtro();
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void Filtro()
        {
            try
            {
                if (ListadoFacturasTemp != null)
                {
                    Listado = new List<FacturaAdministradorModel>(ListadoFacturasTemp.FindAll(p => ((DateTime)p.hasta).Date >= (Desde.Date) && ((DateTime)p.hasta).Date <= (Hasta.Date)));
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void VerPDF(object obj)
        {
            try
            {
                try
                {
                    FacturaModel f = (FacturaModel)obj;
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.ViendoDocumento = true;
                        DependencyService.Get<ISave>().SaveAndView(f.nombre + ".pdf", "application/pdf", GetStreamFromUrl(f.ruta));
                    });
                }
                catch (Exception ex)
                {
                    // 
                }
                
                //App.DAUtil.NavigationService.NavigateToAsyncMenu<PDFViewModel>(f.ruta);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private MemoryStream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;
            MemoryStream ms;

            ms = null;

            try
            {
                using (var wc = new System.Net.WebClient())
                {
                    imageData = wc.DownloadData(url);
                }
                ms = new MemoryStream(imageData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //forbidden, proxy issues, file not found (404) etc
            }
            return ms;
        }
        #endregion
        #region Comandos
        public ICommand cmdVerPDF { get { return new Command(VerPDF); } }
        
        #endregion
    }
}

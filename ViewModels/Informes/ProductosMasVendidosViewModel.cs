using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Syncfusion.XlsIO;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Informes
{
    public class ProductosMasVendidosViewModel : ViewModelBase
    {
        public ProductosMasVendidosViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                cargarZonas();
                cargarEstablecimientos();
                cargaDatos();

            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Propiedades
        private ObservableCollection<ZonaModel> zonas;
        public ObservableCollection<ZonaModel> Zonas
        {
            get
            {
                return zonas;
            }
            set
            {
                if (zonas != value)
                {
                    zonas = value;
                    OnPropertyChanged(nameof(Zonas));
                }
            }
        }
        private ZonaModel zonaSeleccionada;
        public ZonaModel ZonaSeleccionada
        {
            get
            {
                return zonaSeleccionada;
            }
            set
            {
                if (zonaSeleccionada != value)
                {
                    zonaSeleccionada = value;
                    OnPropertyChanged(nameof(ZonaSeleccionada));
                    Filtro();
                }
            }
        }
        private ObservableCollection<Establecimiento> establecimientos;
        public ObservableCollection<Establecimiento> Establecimientos
        {
            get
            {
                return establecimientos;
            }
            set
            {
                if (establecimientos != value)
                {
                    establecimientos = value;
                    OnPropertyChanged(nameof(Establecimientos));
                }
            }
        }
        private Establecimiento establecimientoSeleccionado;
        public Establecimiento EstablecimientoSeleccionado
        {
            get
            {
                return establecimientoSeleccionado;
            }
            set
            {
                if (establecimientoSeleccionado != value)
                {
                    establecimientoSeleccionado = value;
                    OnPropertyChanged(nameof(EstablecimientoSeleccionado));
                    Filtro();
                }
            }
        }
        private ObservableCollection<MasVendidosModel> listado;
        public ObservableCollection<MasVendidosModel> Listado
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
                }
            }
        }
        #endregion

        #region Command
        public ICommand cmdFilter { get { return new Command(Filtro); } }
        public ICommand cmdExport => new Command(Exportar);
        #endregion

        #region Métodos
        private void Filtro()
        {
            try
            {
                Listado = new ObservableCollection<MasVendidosModel>(ResponseServiceWS.getProductosMasVendidos(EstablecimientoSeleccionado == null ? 0 : EstablecimientoSeleccionado.idEstablecimiento, zonaSeleccionada.idZona));
            }
            catch (Exception ex)
            {
                // 
            }

        }
        private void cargaDatos()
        {
            try
            {
                Listado = new ObservableCollection<MasVendidosModel>(ResponseServiceWS.getProductosMasVendidos(0, 0));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void cargarZonas()
        {
            try
            {
                List<ZonaModel> listZonas = App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo", 0));
                List<ZonaModel> zs = new List<ZonaModel>();
                ZonaModel zz = new ZonaModel();
                zz.idZona = 0;
                zz.nombre = AppResources.Todas;
                zs.Add(zz);
                foreach (ZonaModel e in listZonas)
                {
                    zs.Add(e);
                }
                Zonas = new ObservableCollection<ZonaModel>(zs);
                ZonaSeleccionada = Zonas[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void cargarEstablecimientos()
        {
            try
            {
                List<Establecimiento> l = ResponseServiceWS.getListadoEstablecimientos();
                List<Establecimiento> ests = new List<Establecimiento>();
                Establecimiento es = new Establecimiento();
                es.id = 0;
                es.nombre = AppResources.Todos;
                ests.Add(es);
                foreach (Establecimiento e in l)
                {
                    ests.Add(e);
                }
                Establecimientos = new ObservableCollection<Establecimiento>(ests);
                EstablecimientoSeleccionado = Establecimientos[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        void Exportar()
        {
            try { App.userdialog.ShowLoading(AppResources.GenerandoInforme); } catch (Exception) { }
            Task.Run(async () =>
            {
                try
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                    }

                    status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageRead>();

                    }
                }
                catch (Exception)
                {
                    //...
                }
                try
                {
                    App.DAUtil.ExportandoExcel = true;
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        Syncfusion.XlsIO.IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        //Create a workbook with a worksheet
                        IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                        IWorksheet worksheet = workbook.Worksheets[0];
                        worksheet.Range["A1"].Text = AppResources.Cantidad;
                        worksheet.Range["B1"].Text = AppResources.Establecimiento;
                        worksheet.Range["C1"].Text = AppResources.Producto;

                        worksheet.Range["A1:C1"].CellStyle.Font.Bold = true;

                        int i = 2;
                        foreach (MasVendidosModel f in Listado)
                        {
                            if (i <= 102)
                            {
                                worksheet.Range["A" + i].Text = f.cantidad.ToString();
                                worksheet.Range["B" + i].Text = f.establecimiento;
                                worksheet.Range["C" + i].Text = f.producto;
                                worksheet.AutofitColumn(1);
                                worksheet.AutofitColumn(2);
                                worksheet.AutofitColumn(3);
                                i++;
                            }
                        }
                        MemoryStream stream = new MemoryStream();
                        workbook.SaveAs(stream);

                        workbook.Close();

                        MainThread.BeginInvokeOnMainThread(
                                () => DependencyService.Get<ISave>().SaveAndView("MasVendidos.xlsx", "application/msexcel", stream)
                            );
                        //Save the stream as a file in the device and invoke it for viewing
                    }
                }
                catch (Exception ex)
                {
                    // 
                }
                App.userdialog.HideLoading();
            });

        }
        #endregion
    }
}

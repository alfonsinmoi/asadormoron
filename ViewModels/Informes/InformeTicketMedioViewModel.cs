using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public class InformeTicketMedioViewModel : ViewModelBase
    {
        public InformeTicketMedioViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                Desde = DateTime.Today.AddYears(-1);
                Hasta = DateTime.Today;
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
        private ObservableCollection<TicketMedioModel> listado;
        public ObservableCollection<TicketMedioModel> Listado
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
                    else
                    {
                        cargaDatos();
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
                        cargaDatos();
                    }

                }
            }
        }
        #endregion

        #region Command
        public ICommand cmdExport => new DelegateCommandAsync(Exportar);
        #endregion

        #region Métodos
        private void cargaDatos()
        {
            try
            {
                App.DAUtil.pedidoNuevo = false;
                Listado = new ObservableCollection<TicketMedioModel>(ResponseServiceWS.getInformeTicketMedio(Desde, Hasta));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        async Task Exportar()
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
                    worksheet.Range["A1"].Text = AppResources.Establecimiento;
                    worksheet.Range["B1"].Text = AppResources.TicketMedio;

                    worksheet.Range["A1:B1"].CellStyle.Font.Bold = true;

                    int i = 2;
                    foreach (TicketMedioModel f in Listado.OrderBy(p => p.nombre))
                    {
                        worksheet.Range["A" + i].Text = f.nombre;
                        worksheet.Range["B" + i].Text = f.ticketMedio.ToString("#0.00") + "€";
                        worksheet.AutofitColumn(1);
                        worksheet.AutofitColumn(2);
                        i++;
                    }
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    workbook.Close();

                    //Save the stream as a file in the device and invoke it for viewing
                    Microsoft.Maui.Controls.DependencyService.Get<ISave>().SaveAndView("TicketMedio.xlsx", "application/msexcel", stream);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        #endregion
    }
}

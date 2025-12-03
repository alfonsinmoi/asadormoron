using System;
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
    public class ClientesInactivosViewModel : ViewModelBase
    {
        public ClientesInactivosViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
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
        private ObservableCollection<UsuarioModel> listado;
        public ObservableCollection<UsuarioModel> Listado
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

        #region Command
        public ICommand cmdExport => new Command(Exportar);
        #endregion

        #region Métodos
        private void cargaDatos()
        {
            try
            {
                Listado = new ObservableCollection<UsuarioModel>(ResponseServiceWS.getClientesInactivos());
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
                        worksheet.Range["A1"].Text = AppResources.Cliente;
                        worksheet.Range["B1"].Text = AppResources.Telefono;
                        worksheet.Range["C1"].Text = AppResources.Email;

                        worksheet.Range["A1:C1"].CellStyle.Font.Bold = true;

                        int i = 2;
                        foreach (UsuarioModel f in Listado)
                        {
                            worksheet.Range["A" + i].Text = f.nombre + " " + f.apellidos;
                            worksheet.Range["B" + i].Text = f.telefono;
                            worksheet.Range["C" + i].Text = f.email;
                            worksheet.AutofitColumn(1);
                            worksheet.AutofitColumn(2);
                            worksheet.AutofitColumn(3);
                            i++;
                        }
                        MemoryStream stream = new MemoryStream();
                        workbook.SaveAs(stream);

                        workbook.Close();

                        MainThread.BeginInvokeOnMainThread(
                                () => DependencyService.Get<ISave>().SaveAndView("ClientesInactivos.xlsx", "application/msexcel", stream)
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

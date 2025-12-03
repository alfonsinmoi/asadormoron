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
    public class InformeBeneficiosViewModel : ViewModelBase
    {
        public InformeBeneficiosViewModel()
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
        public List<BeneficiosModel> ListBeneficiosTemp;
        public List<BeneficiosModel> ListBeneficiosSemanalTemp;
        public List<BeneficiosModel> ListBeneficiosMensualTemp;

        private List<BeneficiosModel> listado;
        public List<BeneficiosModel> Listado
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
        private List<BeneficiosModel> listadoSemanal;
        public List<BeneficiosModel> ListadoSemanal
        {
            get { return listadoSemanal; }
            set
            {
                if (listadoSemanal != value)
                {
                    listadoSemanal = value;
                    OnPropertyChanged(nameof(ListadoSemanal));
                }
            }
        }
        private List<BeneficiosModel> listadoMensual;
        public List<BeneficiosModel> ListadoMensual
        {
            get { return listadoMensual; }
            set
            {
                if (listadoMensual != value)
                {
                    listadoMensual = value;
                    OnPropertyChanged(nameof(ListadoMensual));
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
                if (ListBeneficiosTemp != null)
                {
                    List<BeneficiosModel> l = ListBeneficiosTemp.FindAll(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date));
                    if (l != null)
                    {
                        Listado = new List<BeneficiosModel>(l);
                    }
                }
                if (ListBeneficiosSemanalTemp != null)
                {
                    List<BeneficiosModel> l = ListBeneficiosSemanalTemp.FindAll(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date));
                    if (l != null)
                    {
                        ListadoSemanal = new List<BeneficiosModel>(l);
                    }
                }
                if (ListBeneficiosMensualTemp != null)
                {
                    List<BeneficiosModel> l = ListBeneficiosMensualTemp.FindAll(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date));
                    if (l != null)
                    {
                        ListadoMensual = new List<BeneficiosModel>(l);
                    }
                }
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
                Desde = DateTime.Now.AddDays(-1);
                Hasta = DateTime.Now;
                App.DAUtil.pedidoNuevo = false;
                ListBeneficiosTemp = new List<BeneficiosModel>(ResponseServiceWS.getInformeBeneficiosDiario());
                ListBeneficiosSemanalTemp = new List<BeneficiosModel>(ResponseServiceWS.getInformeBeneficiosSemanal());
                ListBeneficiosMensualTemp = new List<BeneficiosModel>(ResponseServiceWS.getInformeBeneficiosMensual());

                if (ListBeneficiosTemp != null && ListBeneficiosTemp.Count > 0)
                {
                    List<BeneficiosModel> Listado2 = new List<BeneficiosModel>(ListBeneficiosTemp.Where(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date)));

                    Listado = new List<BeneficiosModel>(Listado2);
                }
                if (ListBeneficiosSemanalTemp != null && ListBeneficiosSemanalTemp.Count > 0)
                {
                    List<BeneficiosModel> Listado2 = new List<BeneficiosModel>(ListBeneficiosSemanalTemp.Where(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date)));

                    ListadoSemanal = new List<BeneficiosModel>(Listado2);
                }
                if (ListBeneficiosMensualTemp != null && ListBeneficiosMensualTemp.Count > 0)
                {
                    List<BeneficiosModel> Listado2 = new List<BeneficiosModel>(ListBeneficiosMensualTemp.Where(p => (p.fecha).Date >= (Desde.Date) && (p.fecha).Date <= (Hasta.Date)));

                    ListadoMensual = new List<BeneficiosModel>(Listado2);
                }
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
                        worksheet.Name = AppResources.BeneficioDiario;
                        worksheet.Range["A1"].Text = AppResources.Fecha;
                        worksheet.Range["B1"].Text = AppResources.Pedidos;
                        worksheet.Range["C1"].Text = AppResources.GastosEnvio;
                        worksheet.Range["D1"].Text =AppResources.Total;

                        worksheet.Range["A1:D1"].CellStyle.Font.Bold = true;

                        int i = 2;
                        foreach (BeneficiosModel f in Listado.OrderBy(p => p.fecha))
                        {
                            worksheet.Range["A" + i].Text = f.fecha.ToString("dd/MM/yyyy");
                            worksheet.Range["B" + i].Text = f.pedidos.ToString("#0.00") + "€";
                            worksheet.Range["C" + i].Text = f.gastos.ToString("#0.00") + "€";
                            worksheet.Range["D" + i].Text = f.total.ToString("#0.00") + "€";
                            worksheet.AutofitColumn(1);
                            worksheet.AutofitColumn(2);
                            worksheet.AutofitColumn(3);
                            worksheet.AutofitColumn(4);
                            i++;
                        }

                        workbook.Worksheets.Create();
                        worksheet = workbook.Worksheets[1];
                        worksheet.Name = AppResources.BeneficioSemanal;
                        worksheet.Range["A1"].Text = AppResources.Fecha;
                        worksheet.Range["B1"].Text = AppResources.Pedidos;
                        worksheet.Range["C1"].Text = AppResources.GastosEnvio;
                        worksheet.Range["D1"].Text = AppResources.Total;

                        worksheet.Range["A1:D1"].CellStyle.Font.Bold = true;

                        i = 2;
                        foreach (BeneficiosModel f in ListadoSemanal.OrderBy(p => p.fecha))
                        {
                            worksheet.Range["A" + i].Text = f.fecha.ToString("dd/MM/yyyy");
                            worksheet.Range["B" + i].Text = f.pedidos.ToString("#0.00") + "€";
                            worksheet.Range["C" + i].Text = f.gastos.ToString("#0.00") + "€";
                            worksheet.Range["D" + i].Text = f.total.ToString("#0.00") + "€";
                            worksheet.AutofitColumn(1);
                            worksheet.AutofitColumn(2);
                            worksheet.AutofitColumn(3);
                            worksheet.AutofitColumn(4);
                            i++;
                        }

                        workbook.Worksheets.Create();
                        worksheet = workbook.Worksheets[2];
                        worksheet.Name = AppResources.BeneficioMensual;
                        worksheet.Range["A1"].Text = AppResources.Fecha;
                        worksheet.Range["B1"].Text = AppResources.Pedidos;
                        worksheet.Range["C1"].Text = AppResources.GastosEnvio;
                        worksheet.Range["D1"].Text = AppResources.Total;

                        worksheet.Range["A1:D1"].CellStyle.Font.Bold = true;

                        i = 2;
                        foreach (BeneficiosModel f in ListadoMensual.OrderBy(p => p.fecha))
                        {
                            worksheet.Range["A" + i].Text = f.fecha.ToString("dd/MM/yyyy");
                            worksheet.Range["B" + i].Text = f.pedidos.ToString("#0.00") + "€";
                            worksheet.Range["C" + i].Text = f.gastos.ToString("#0.00") + "€";
                            worksheet.Range["D" + i].Text = f.total.ToString("#0.00") + "€";
                            worksheet.AutofitColumn(1);
                            worksheet.AutofitColumn(2);
                            worksheet.AutofitColumn(3);
                            worksheet.AutofitColumn(4);
                            i++;
                        }


                        MemoryStream stream = new MemoryStream();
                        workbook.SaveAs(stream);

                        workbook.Close();
                        MainThread.BeginInvokeOnMainThread(
                                () => DependencyService.Get<ISave>().SaveAndView("Beneficios.xlsx", "application/msexcel", stream)
                            );
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

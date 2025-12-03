using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using PointF = Syncfusion.Drawing.PointF;
using SizeF = Syncfusion.Drawing.SizeF;

namespace AsadorMoron.ViewModels.Contabilidad
{
    public class CuentasAdministradorViewModel:ViewModelBase
    {
        public CuentasAdministradorViewModel()
        {
            cargado = false;
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!cargado)
                {
                    cargado = true;
                    App.DAUtil.EnTimer = false;
                    Facturas = new ObservableCollection<FacturaAdministradorModel>();
                    Anyos = new ObservableCollection<int>();
                    int i = 2021;
                    for (int x = i; x <= DateTime.Today.Year; x++)
                    {
                        Anyos.Add(x);
                    }
                    VisibleBoton = false;
                    AnyoSeleccionado = DateTime.Today.Year;
                    NumeroFactura = App.global.numeroFactura;
                    FDesde = DateTime.Today.AddDays(-10);
                    FHasta = DateTime.Today.AddDays(-1);
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
        private List<FacturaAdministradorModel> facts;
        private ObservableCollection<FacturaAdministradorModel> facturas;
        public ObservableCollection<FacturaAdministradorModel> Facturas
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
        private bool visibleBoton = false;
        public bool VisibleBoton
        {
            get
            {
                return visibleBoton;
            }
            set
            {
                if (visibleBoton != value)
                {
                    visibleBoton = value;
                    OnPropertyChanged(nameof(VisibleBoton));
                }
            }
        }
        private ObservableCollection<int> anyos;
        public ObservableCollection<int> Anyos
        {
            get
            {
                return anyos;
            }
            set
            {
                if (anyos != value)
                {
                    anyos = value;
                    OnPropertyChanged(nameof(Anyos));
                }
            }
        }
        private int anyoSeleccionado;
        public int AnyoSeleccionado
        {
            get
            {
                return anyoSeleccionado;
            }
            set
            {
                if (anyoSeleccionado != value)
                {
                    anyoSeleccionado = value;
                    OnPropertyChanged(nameof(AnyoSeleccionado));
                }
            }
        }
        private DateTime fDesde;
        public DateTime FDesde
        {
            get
            {
                return fDesde;
            }
            set
            {
                if (fDesde != value)
                {
                    fDesde = value;
                    OnPropertyChanged(nameof(FDesde));
                }
            }
        }
        private DateTime fHasta;
        public DateTime FHasta
        {
            get
            {
                return fHasta;
            }
            set
            {
                if (fHasta != value)
                {
                    fHasta = value;
                    OnPropertyChanged(nameof(FHasta));
                }
            }
        }
        private int numeroFactura;
        public int NumeroFactura
        {
            get
            {
                return numeroFactura;
            }
            set
            {
                if (numeroFactura != value)
                {
                    numeroFactura = value;
                    OnPropertyChanged(nameof(NumeroFactura));
                }
            }
        }
        #endregion

        #region Comandos
        public ICommand cmdGenerar { get { return new DelegateCommandAsync(Generar); } }
        public ICommand cmdVer { get { return new Command((parametro) => Ver(parametro)); } }
        public ICommand cmdAceptar { get { return new DelegateCommandAsync(Aceptar); } }
        public ICommand cmdRechazar { get { return new Command((parametro) => Rechazar(parametro)); } }
        #endregion

        #region Funciones
        private async Task CrearPDF()
        {
            facts = new List<FacturaAdministradorModel>();
            //Creates a new PDF document
            int factura = NumeroFactura;
            MemoryStream stream = new MemoryStream();
            List<CuentasAdministradorModel> invoiceDetails = ResponseServiceWS.getCuentasAdministrador(FDesde, FHasta);
            Stream f = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PolloAndaluz.Recursos.NunitoSans-Regular.ttf");
            PdfDocument document = new PdfDocument();
            foreach (CuentasAdministradorModel c in invoiceDetails)
            {
                document = new PdfDocument();
                //Adds page settings
                document.PageSettings.Orientation = PdfPageOrientation.Portrait;
                document.PageSettings.Margins.All = 50;
                
                AdministradorFiscalModel fiscal = ResponseServiceWS.getDatosFiscalesGrupoPueblos(c.idPueblo);
                if (!string.IsNullOrEmpty(fiscal.iban))
                {
                    try
                    {
                        ConfiguracionAdmin conf = ResponseServiceWS.getConfiguracionAdmin(c.idPueblo);
                        factura++;
                        //Adds a page to the document
                        PdfPage page = document.Pages.Add();
                        PdfGraphics graphics = page.Graphics;

                        //Loads the image as stream
                        Stream imageStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PolloAndaluz.Recursos.logo_morado.png");
                        RectangleF bounds = new RectangleF(page.Size.Width - 270, 0, 151, 49);
                        PdfImage image = PdfImage.FromStream(imageStream);
                        //Draws the image to the PDF page
                        page.Graphics.DrawImage(image, bounds);



                        PdfFont subHeadingFont = new PdfTrueTypeFont(f, 10, PdfFontStyle.Regular);
                        PdfTextElement element = new PdfTextElement("PolloAndaluz Development SL", subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        PdfLayoutResult result = element.Draw(page, new PointF(0, 0));
                        element = new PdfTextElement("B06971998", subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 2));
                        element = new PdfTextElement("Bda. de la Paz, Bloque 2 -2ª D", subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 2));
                        element = new PdfTextElement("41600, Arahal (Sevilla)", subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 2));

                        PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                        bounds = new RectangleF(0, result.Bounds.Bottom + 5, graphics.ClientSize.Width, 30);
                        //Draws a rectangle to place the heading in that region.
                        graphics.DrawRectangle(solidBrush, bounds);
                        //Creates a font for adding the heading in the page
                        //Creates a text element to add the invoice number
                        //PdfTextElement element = new PdfTextElement("INVOICE " + id.ToString(), subHeadingFont);
                        element = new PdfTextElement("Factura " + AnyoSeleccionado + "/" + factura, subHeadingFont);
                        element.Brush = PdfBrushes.White;

                        //Draws the heading on the page
                        result = element.Draw(page, new PointF(10, bounds.Top + 8));
                        string currentDate = "Fecha " + FHasta.ToString("dd/MM/yyyy");
                        //Measures the width of the text to place it in the correct location
                        SizeF textSize = subHeadingFont.MeasureString(currentDate);
                        PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                        //Draws the date by using DrawString method
                        graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);

                        //Creates text elements to add the address and draw it to the page.
                        PdfFont timesRoman = new PdfTrueTypeFont(f, 10);
                        element = new PdfTextElement("CLIENTE ", timesRoman);
                        element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                        result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));

                        element = new PdfTextElement(fiscal.razonSocial.ToUpper(), subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 2));
                        element = new PdfTextElement(fiscal.cif.ToUpper(), subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 2));
                        element = new PdfTextElement(fiscal.direccion.ToUpper(), subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 2));
                        element = new PdfTextElement(fiscal.cp + ", " + fiscal.poblacion + " (" + fiscal.provincia + ")", subHeadingFont);
                        element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                        result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 2));

                        PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
                        PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
                        PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3);
                        //Draws a line at the bottom of the address
                        graphics.DrawLine(linePen, startPoint, endPoint);

                        //Creates the datasource for the table

                        //Creates a PDF grid
                        PdfGrid grid = new PdfGrid();
                        //Adds the data source
                        //Add columns to the DataTable
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Cantidad");
                        dataTable.Columns.Add("Concepto");
                        dataTable.Columns.Add("Base Imponible");
                        dataTable.Columns.Add("IVA");
                        dataTable.Columns.Add("Total");


                        double totalVentas = c.totalVentas;
                        double baseImponibleTotalVentas = totalVentas / 1.21;
                        double pagosAEstablecimientos = c.pagosAEstablecimientos;
                        double baseImponiblePagosAEstablecimientos = c.pagosAEstablecimientos/1.21;
                        double totalVentasTarjeta = c.totalVentasTarjeta;
                        double baseImponibleTotalVentasTarjeta = totalVentasTarjeta / 1.21;
                        double totalVentasDatafono = c.totalVentasDatafono;
                        double baseImponibleTotalVentasDatafono = totalVentasDatafono / 1.21;
                        double fijoTarjeta = c.fijoTarjeta;
                        double baseImponibleFijoTarjeta = c.fijoTarjeta/1.21;
                        double variableTarjeta = c.variableTarjeta;
                        double baseImponibleVariableTarjeta = c.variableTarjeta/1.21;
                        double comisionDatafono = c.comisionDatafono;
                        double baseImponibleComisionDatafono = c.comisionDatafono/1.21;
                        double comision= c.comision * 1.21;
                        double baseImponibleComision = c.comision;

                        double baseImponible = baseImponibleFijoTarjeta+baseImponibleVariableTarjeta+baseImponibleComisionDatafono+baseImponibleComision;
                        double total = fijoTarjeta+variableTarjeta+comisionDatafono+comision;
                        double moi = 0;
                        double basemoi = moi / 1.21;
                        if (comision > 0)
                            dataTable.Rows.Add(new object[] { 1, "Comisión PolloAndaluz (" + conf.comision.ToString("N1") + "%)" , baseImponibleComision.ToString("C2"), (comision - baseImponibleComision).ToString("C2"), comision.ToString("C2") });
                        if (variableTarjeta > 0)
                            dataTable.Rows.Add(new object[] { 1, "Comisión Tarjetas (" + conf.variableTarjeta.ToString("N1") + "%)", baseImponibleVariableTarjeta.ToString("C2"), (variableTarjeta - baseImponibleVariableTarjeta).ToString("C2"), variableTarjeta.ToString("C2") });
                        if (fijoTarjeta > 0)
                            dataTable.Rows.Add(new object[] { 1, "Comisión Fija Tarjetas (" + conf.fijoTarjeta.ToString("C2") + ")", baseImponibleFijoTarjeta.ToString("C2"), (fijoTarjeta - baseImponibleFijoTarjeta).ToString("C2"), fijoTarjeta.ToString("C2") });
                        if (comisionDatafono > 0)
                            dataTable.Rows.Add(new object[] { 1, "Comisión Datafonos (" + conf.variableDatafono.ToString("N1") + "%)", baseImponibleComisionDatafono.ToString("C2"), (comisionDatafono - baseImponibleComisionDatafono).ToString("C2"), comisionDatafono.ToString("C2") });
                        

                        /*double bIMoto = 189;
                        double moto = 189 * 1.21;
                        dataTable.Rows.Add(new object[] { 1, "Alquier moto", bIMoto.ToString("C2"), (moto - bIMoto).ToString("C2"), moto.ToString("C2") });
                        baseImponible += bIMoto;
                        total += moto;*/
                        if (total != 0)
                        {
                            //Assign data source.
                            grid.DataSource = dataTable;
                            //Creates the grid cell styles
                            PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                            cellStyle.Borders.All = PdfPens.White;
                            PdfGridRow header = grid.Headers[0];
                            //Creates the header style
                            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
                            headerStyle.Borders.All = new PdfPen(new PdfColor(93, 56, 188));
                            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            headerStyle.TextBrush = PdfBrushes.White;
                            headerStyle.Font = new PdfTrueTypeFont(f, 10f);
                            headerStyle.CellPadding = new PdfPaddings(5, 5, 0, 0);
                            header.ApplyStyle(headerStyle);
                            cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
                            cellStyle.Font = new PdfTrueTypeFont(f, 12f);
                            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            cellStyle.Borders.All = new PdfPen(PdfBrushes.Transparent);
                            cellStyle.CellPadding = new PdfPaddings(5, 5, 0, 0);
                            grid.Columns[0].Width = 60;
                            grid.Columns[2].Width = 80;
                            grid.Columns[3].Width = 80;
                            grid.Columns[4].Width = 80;
                            //Adds cell customizations
                            header.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                            header.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                            header.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            header.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            header.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            for (int i = 0; i < grid.Rows.Count; i++)
                            {
                                grid.Rows[i].ApplyStyle(cellStyle);
                                grid.Rows[i].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                grid.Rows[i].Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                grid.Rows[i].Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                                grid.Rows[i].Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                                grid.Rows[i].Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            }


                            //Applies the header style




                            //Creates the layout format for grid
                            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
                            // Creates layout format settings to allow the table pagination
                            layoutFormat.Layout = PdfLayoutType.Paginate;
                            //Draws the grid to the PDF page.
                            PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);

                            PdfFont timesRoman3 = new PdfTrueTypeFont(f, 12f, PdfFontStyle.Regular);
                            element = new PdfTextElement("BASE IMPONIBLE:   " + baseImponible.ToString("C2"), timesRoman3);
                            element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            result = element.Draw(page, new PointF(0, gridResult.Bounds.Bottom + 25));
                            element = new PdfTextElement("IVA:   " + (total - baseImponible).ToString("C2"), timesRoman3);
                            element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 2));
                            PdfFont timesRoman2 = new PdfTrueTypeFont(f, 12f, PdfFontStyle.Bold);
                            element = new PdfTextElement("TOTAL FACTURA:   " + total.ToString("C2"), timesRoman2);
                            element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 2));


                            PdfGrid grid2 = new PdfGrid();
                            DataTable dataTable2 = new DataTable();
                            dataTable2.Columns.Add("DETALLE FACTURA");
                            dataTable2.Columns.Add("  ");
                            dataTable2.Rows.Add(new object[] { "TOTAL VENTAS", totalVentas.ToString("C2") });
                            if (pagosAEstablecimientos > 0)
                                dataTable2.Rows.Add(new object[] { "BENEFICIOS", pagosAEstablecimientos.ToString("C2") });
                            if (totalVentasTarjeta > 0)
                                dataTable2.Rows.Add(new object[] { "TOTAL VENTAS POR TARJETA", totalVentasTarjeta.ToString("C2")});
                            if (totalVentasDatafono > 0)
                                dataTable2.Rows.Add(new object[] { "TOTAL VENTAS POR DATAFONO", totalVentasDatafono.ToString("C2") });

                            if (moi > 0)
                                dataTable2.Rows.Add(new object[] { "FACT ANTERIOR", moi.ToString("C2") });

                            dataTable2.Rows.Add(new object[] { "", "" });

                            if (total != 0)
                                if (moi!=0)
                                    dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", $"{pagosAEstablecimientos.ToString("N2")} - {total.ToString("N2")} - {moi.ToString("N2")} = {(pagosAEstablecimientos - total - moi).ToString("N2")}" });
                                else
                                    dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", $"{pagosAEstablecimientos.ToString("N2")} - {total.ToString("N2")} = {(pagosAEstablecimientos - total).ToString("N2")}" });


                            grid2.DataSource = dataTable2;
                            //Creates the grid cell styles
                            cellStyle = new PdfGridCellStyle();
                            cellStyle.Borders.All = PdfPens.White;
                            header = grid2.Headers[0];
                            //Creates the header style
                            headerStyle = new PdfGridCellStyle();
                            headerStyle.Borders.All = new PdfPen(new PdfColor(93, 56, 188));
                            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            headerStyle.TextBrush = PdfBrushes.White;
                            headerStyle.Font = new PdfTrueTypeFont(f, 10f);
                            headerStyle.CellPadding = new PdfPaddings(5, 5, 0, 0);
                            header.ApplyStyle(headerStyle);
                            cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
                            cellStyle.Font = new PdfTrueTypeFont(f, 12f);
                            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            cellStyle.Borders.All = new PdfPen(PdfBrushes.Transparent);
                            cellStyle.CellPadding = new PdfPaddings(5, 5, 0, 0);
                            grid2.Columns[1].Width = 220;
                            //Adds cell customizations
                            header.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                            header.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            for (int i = 0; i < grid2.Rows.Count; i++)
                            {
                                grid2.Rows[i].ApplyStyle(cellStyle);
                                grid2.Rows[i].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                grid2.Rows[i].Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                            }
                            result = grid2.Draw(page, new PointF(0, result.Bounds.Bottom + 25));

                            element = new PdfTextElement("REALIZAR TRANSFERENCIA BANCARIA A LA CUENTA DEL CLIENTE:", timesRoman2);
                            element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 10));
                            element = new PdfTextElement(fiscal.iban.ToUpper(), timesRoman2);
                            element.Brush = new PdfSolidBrush(new PdfColor(93, 56, 188));
                            result = element.Draw(page, new PointF(0, result.Bounds.Bottom + 10));

                            //gridResult = grid2.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 10), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);
                            //Save the PDF document to stream.
                            stream = new MemoryStream();
                            document.Save(stream);
                            //Close the document.

                            //Save the stream as a file in the device and invoke it for viewing
                            Microsoft.Maui.Controls.DependencyService.Get<ISave>().SaveAndUpAdmin(fiscal.razonSocial.Replace(" ", "_") + "_" + factura + ".pdf", fiscal.razonSocial.Replace(" ", "_") + "_" + factura, stream);
                            stream.Close();
                            stream.Dispose();
                            stream = null;
                            FacturaAdministradorModel fact = new FacturaAdministradorModel();
                            fact.desde = FDesde;
                            fact.hasta = FHasta;
                            fact.nombre = fiscal.razonSocial.Replace(" ", "_") + "_" + factura;
                            fact.numero = AnyoSeleccionado + "/" + factura;
                            fact.ruta = ResponseServiceWS.urlPro + "documentos/facturas/administradores/" + fiscal.razonSocial.Replace(" ", "_") + "_" + factura + ".pdf";
                            fact.idPueblo = c.idPueblo;
                            fact.nombreAdministrador = fiscal.razonSocial;
                            fact.total = total;
                            facts.Add(fact);
                        }
                        else
                            factura--;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            App.global.numeroFactura = factura;
            document.Close(true);
        }
        private async Task Generar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Run(async () => { await CrearPDF(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                   {
                       Facturas = new ObservableCollection<FacturaAdministradorModel>(facts);
                       VisibleBoton = Facturas.Count > 0;
                       App.userdialog.HideLoading();
                   });
                }));
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
        }
        private void Ver(object parametro)
        {
            try
            {
                FacturaAdministradorModel f = (FacturaAdministradorModel)parametro;
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
        private async Task Aceptar()
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaGuardarFacturas, AppResources.No, AppResources.Si);
            Aceptar2(result);
        }
        private void Rechazar(object parametro)
        {

        }
        private void Aceptar2(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }

                    Task.Run(() =>
                    {
                        foreach (FacturaAdministradorModel c in Facturas)
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await App.ResponseWS.NuevaFacturaAdministrador(c);
                            });
                        }
                    });

                    App.ResponseWS.saveConfiguracionGeneral();
                    NumeroFactura = App.global.numeroFactura;
                    App.customDialog.ShowDialogAsync(AppResources.FacturasGeneradasOK, AppResources.App, AppResources.Cerrar);
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }
        #endregion
    }
}

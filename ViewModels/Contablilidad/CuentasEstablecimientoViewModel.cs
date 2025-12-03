using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using PointF = Syncfusion.Drawing.PointF;
using SizeF = Syncfusion.Drawing.SizeF;

namespace AsadorMoron.ViewModels.Contabilidad
{
    public class CuentasEstablecimientoViewModel:ViewModelBase
    {
        
        public CuentasEstablecimientoViewModel()
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
                    Facturas = new ObservableCollection<FacturaModel>();
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
                    Pueblos = new ObservableCollection<PueblosModel>(App.ResponseWS.getPueblos());
                    PuebloSeleccionado = Pueblos[0];
                }
                TienePermiso = await GetPermisoEscritura();
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

        private async Task<bool> GetPermisoEscritura()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status == PermissionStatus.Granted)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }

        }
        #region Funciones
        private async Task<List<FacturaModel>> CrearPDF()
        {
            var facts = new List<FacturaModel>();
            //Creates a new PDF document
            int factura = NumeroFactura;
            MemoryStream stream = new MemoryStream();
            List<Establecimiento> establecimientos = ResponseServiceWS.getListadoTodosEstablecimientos(PuebloSeleccionado.id);
            Stream f = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PolloAndaluz.Recursos.NunitoSans-Regular.ttf");
            PdfDocument document = new PdfDocument();
            foreach (Establecimiento e in establecimientos)
            {
                document = new PdfDocument();
                //Adds page settings
                document.PageSettings.Orientation = PdfPageOrientation.Portrait;
                document.PageSettings.Margins.All = 50;
                List<PreFacturaModel> invoiceDetails = ResponseServiceWS.getHistoricoPedidosEstablecimiento(e,FDesde, FHasta);
                if (invoiceDetails.Count > 0)
                {
                    EstablecimientoFiscalModel fiscal = ResponseServiceWS.getDatosFiscalesEstablecimiento(e.idEstablecimiento);

                    if (fiscal != null)
                    {
                        try
                        {
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
                            string currentDate = "Fecha " + DateTime.Now.ToString("dd/MM/yyyy");
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
                            if (e.configuracion == null)
                                e.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(e.idEstablecimiento);
                            double totalVentas = invoiceDetails.Sum(p=>p.precio);
                            double totalEnvios = invoiceDetails.Where(p=>p.tipoVenta.Equals("Envío")).Sum(p => p.precio);
                            double totalEnLocal = invoiceDetails.Where(p => p.tipoVenta.Equals("Recogida")).Sum(p => p.precio);
                            double totalSinGastos = invoiceDetails.Where(p => p.tipo == 0 || p.tipoVenta.Equals("Reparto Propio")).Sum(p => p.precio);
                            double gastosEnvio= invoiceDetails.Where(p => p.tipo == 1 &&  !p.tipoVenta.Equals("Reparto Propio")).Sum(p => p.precio);
                            double descuentos = invoiceDetails.Where(p => p.tipo == 3).Sum(p => p.precio);
                            gastosEnvio += descuentos;
                            double baseImponibleGastosEnvio = gastosEnvio / 1.21;
                            double baseImponibleComisionEnvios = invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Envío") && p.pagadoConPuntos == 0).Sum(p => p.precio) * (e.configuracion.comision/100);
                            double comisionEnvios = baseImponibleComisionEnvios * 1.21;
                            double baseImponibleRepartoPropio = invoiceDetails.Where(p => p.tipoVenta.Equals("Reparto Propio")).Sum(p => p.precio) * (e.configuracion.comisionReparto / 100);
                            double comisionRepartoPropio = baseImponibleRepartoPropio * 1.21;
                            double totalRepartoPropio = invoiceDetails.Where(p => p.tipoVenta.Equals("Reparto Propio")).Sum(p => p.precio);
                            double totalRepartoPropioEfectivo = invoiceDetails.Where(p => p.tipoVenta.Equals("Reparto Propio") && p.tipoPago.Equals("Efectivo")).Sum(p => p.precio);
                            double baseImponibleComisionRecogida = invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Recogida") && p.pagadoConPuntos == 0).Sum(p => p.precio) * (e.configuracion.comisionRecogida / 100);
                            double comisionRecogida = baseImponibleComisionRecogida * 1.21;
                            /*double baseImponibleComisionRecogidaEfectivo = invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Recogida") && p.pagadoConPuntos == 0 && !p.tipoPago.Equals("Tarjeta")).Sum(p => p.precio) * (e.configuracion.comisionRecogida / 100);
                            double comisionRecogidaEfectivo = baseImponibleComisionRecogidaEfectivo * 1.21;*/
                            double comisionAutoPedidos = 0;
                            double comisionPuntos = invoiceDetails.Where(p => p.pagadoConPuntos==1).Sum(p => p.precio)*-1;
                            double baseImponibleComisionPuntos = comisionPuntos / 1.21;
                            double baseImponibleCuotaFija = e.configuracion.cuotaFija;
                            double cuotaFija = baseImponibleCuotaFija * 1.21;
                            double baseImponibleOtrasCuotas = e.configuracion.otrasCuotas;
                            double otrasCuotas = baseImponibleOtrasCuotas * 1.21;
                            double baseImponible = baseImponibleOtrasCuotas + baseImponibleComisionPuntos + baseImponibleCuotaFija + baseImponibleGastosEnvio + baseImponibleComisionEnvios + baseImponibleComisionRecogida + baseImponibleRepartoPropio;
                            double total = gastosEnvio + comisionEnvios + comisionAutoPedidos + comisionPuntos + comisionRecogida + cuotaFija + otrasCuotas + comisionRepartoPropio;
                            if (gastosEnvio > 0)
                                dataTable.Rows.Add(new object[] { invoiceDetails.Where(p => p.tipo == 1 && p.precio>0).Count(), "Repartos", baseImponibleGastosEnvio.ToString("C2"), (gastosEnvio- baseImponibleGastosEnvio).ToString("C2"), gastosEnvio.ToString("C2") });
                            if (comisionEnvios > 0)
                                dataTable.Rows.Add(new object[] { invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Envío") && p.pagadoConPuntos==0).GroupBy(p=>p.codigo).Count(), "Comisión Ventas " + e.configuracion.comision + "%", baseImponibleComisionEnvios.ToString("C2"), (comisionEnvios - baseImponibleComisionEnvios).ToString("C2"), comisionEnvios.ToString("C2") });
                            if (comisionRecogida > 0)
                                dataTable.Rows.Add(new object[] { invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Recogida") && p.pagadoConPuntos == 0).GroupBy(p => p.codigo).Count(), "Comisión Recogida en local " + e.configuracion.comisionRecogida + "%", baseImponibleComisionRecogida.ToString("C2"), (comisionRecogida- baseImponibleComisionRecogida).ToString("C2"), comisionRecogida.ToString("C2") });
                            if (comisionRepartoPropio > 0)
                                dataTable.Rows.Add(new object[] { invoiceDetails.Where(p => p.tipo == 0 && p.tipoVenta.Equals("Reparto Propio") && p.pagadoConPuntos == 0).GroupBy(p => p.codigo).Count(), "Comisión Reparto Propio " + e.configuracion.comisionReparto + "%", baseImponibleRepartoPropio.ToString("C2"), (comisionRepartoPropio - baseImponibleRepartoPropio).ToString("C2"), comisionRepartoPropio.ToString("C2") });
                            if (comisionPuntos > 0)
                                dataTable.Rows.Add(new object[] { invoiceDetails.Where(p => p.pagadoConPuntos==1).GroupBy(p => p.codigo).Count(), "Pago con puntos", baseImponibleComisionPuntos.ToString("C2"), (comisionPuntos - baseImponibleComisionPuntos).ToString("C2"), comisionPuntos.ToString("C2") });
                            if (cuotaFija > 0)
                                dataTable.Rows.Add(new object[] { 1, "Cuota Fija", baseImponibleCuotaFija.ToString("C2"), (cuotaFija- baseImponibleCuotaFija).ToString("C2"), cuotaFija.ToString("C2") });
                            if (otrasCuotas > 0)
                                dataTable.Rows.Add(new object[] { 1, "Otras cuotas", baseImponibleOtrasCuotas.ToString("C2"), (otrasCuotas- baseImponibleOtrasCuotas).ToString("C2"), otrasCuotas.ToString("C2") });
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
                                element = new PdfTextElement("IVA:   " + (total-baseImponible).ToString("C2"), timesRoman3);
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
                                dataTable2.Columns.Add("DESDE " + FDesde.ToString("dd/MM/yyyy") + " A " + FHasta.ToString("dd/MM/yyyy"));
                                dataTable2.Rows.Add(new object[] { "TOTAL VENTAS", totalVentas.ToString("C2"), "" });
                                if (gastosEnvio-descuentos > 0)
                                    dataTable2.Rows.Add(new object[] { "REPARTOS", (gastosEnvio-descuentos).ToString("C2"), "" });
                                if (descuentos != 0)
                                    dataTable2.Rows.Add(new object[] { "DESCUENTOS", descuentos.ToString("C2"), "" });
                                if (totalRepartoPropioEfectivo != 0)
                                    dataTable2.Rows.Add(new object[] { "COBRADO EN EFECTIVO", totalRepartoPropioEfectivo.ToString("C2"), "" });
                                if (comisionEnvios > 0)
                                    dataTable2.Rows.Add(new object[] { "COMISIÓN ENVÍOS", comisionEnvios.ToString("C2"),$"({e.configuracion.comision}% de {totalSinGastos.ToString("C2")} = {baseImponibleComisionEnvios.ToString("C2")}*21%)" });
                                if (comisionRecogida > 0)
                                    dataTable2.Rows.Add(new object[] { "COMISIÓN RECOGIDA EN LOCAL", comisionRecogida.ToString("C2"), $"({e.configuracion.comisionRecogida}% de {totalEnLocal.ToString("C2")} = {(baseImponibleComisionRecogida).ToString("C2")}*21%)" });

                                if (comisionEnvios > 0)
                                    dataTable2.Rows.Add(new object[] { "COMISIÓN REPARTO PROPIO", comisionRepartoPropio.ToString("C2"), $"({e.configuracion.comisionReparto}% de {totalRepartoPropio.ToString("C2")} = {baseImponibleRepartoPropio.ToString("C2")}*21%)" });
                                /*if (comisionRecogidaEfectivo > 0)
                                    dataTable2.Rows.Add(new object[] { "EFECTIVOS RECOGIDA EN LOCAL", comisionRecogidaEfectivo.ToString("C2"), "" });*/
                                if (comisionPuntos > 0)
                                    dataTable2.Rows.Add(new object[] { "PAGADO CON PUNTOS", comisionPuntos.ToString("C2"),"" });
                                if (cuotaFija > 0)
                                    dataTable2.Rows.Add(new object[] { "CUOTA FIJA", cuotaFija.ToString("C2"),"" });
                                if (otrasCuotas > 0)
                                    dataTable2.Rows.Add(new object[] { "OTRAS CUOTAS", otrasCuotas.ToString("C2"), "" });

                                double regul = 0;
                                if (e.idEstablecimiento == 87)
                                    regul = -6.65;
                                
                                if (regul != 0)
                                    dataTable2.Rows.Add(new object[] { "PAGO PEDIDO hXisuYxL", regul.ToString("C2"), "" });
                                
                                dataTable2.Rows.Add(new object[] { "", "", "" });
                                /*if (comisionRecogida > 0)
                                {
                                    if (regul > 0)
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {comisionRecogidaEfectivo.ToString("N2")} + {regul.ToString("N2")} = {(totalVentas - total - comisionRecogidaEfectivo+regul).ToString("N2")}" });
                                    else if (regul < 0)
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {comisionRecogidaEfectivo.ToString("N2")} {regul.ToString("N2")} = {(totalVentas - total - comisionRecogidaEfectivo + regul).ToString("N2")}" });
                                    else
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {comisionRecogidaEfectivo.ToString("N2")} = {(totalVentas - total - comisionRecogidaEfectivo+regul).ToString("N2")}" });
                                }
                                else
                                {*/
                                    if (regul > 0)
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {totalRepartoPropioEfectivo.ToString("N2")} + {regul.ToString("N2")}= {(totalVentas - total-totalRepartoPropioEfectivo+regul).ToString("N2")}" });
                                    else if (regul < 0)
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {totalRepartoPropioEfectivo.ToString("N2")} {regul.ToString("N2")}= {(totalVentas - total - totalRepartoPropioEfectivo + regul).ToString("N2")}" });
                                    else
                                        dataTable2.Rows.Add(new object[] { "IMPORTE A TRANSFERIR", "", $"{totalVentas.ToString("N2")} - {total.ToString("N2")} - {totalRepartoPropioEfectivo.ToString("N2")} = {(totalVentas - total - totalRepartoPropioEfectivo).ToString("N2")}" });
                                //}

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
                                grid2.Columns[0].Width = 180;
                                grid2.Columns[1].Width = 80;
                                //Adds cell customizations
                                header.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                header.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                                header.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                for (int i = 0; i < grid2.Rows.Count; i++)
                                {
                                    grid2.Rows[i].ApplyStyle(cellStyle);
                                    grid2.Rows[i].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                                    grid2.Rows[i].Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                                    grid2.Rows[i].Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
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
                                Microsoft.Maui.Controls.DependencyService.Get<ISave>().SaveAndUp(e.nombre.Replace(" ", "_") + "_" + factura + ".pdf", e.nombre.Replace(" ", "_") + "_" + factura, stream);
                                stream.Close();
                                stream.Dispose();
                                stream = null;
                                FacturaModel fact = new FacturaModel();
                                fact.desde = FDesde;
                                fact.hasta = FHasta;
                                fact.nombre = e.nombre.Replace(" ", "_") + "_" + factura;
                                fact.numero = AnyoSeleccionado + "/" + factura;
                                fact.ruta = ResponseServiceWS.urlPro + "documentos/facturas/" + e.nombre.Replace(" ", "_") + "_" + factura + ".pdf";
                                fact.idEstablecimiento = e.idEstablecimiento;
                                fact.nombreEstablecimiento = e.nombre;
                                fact.total = total;
                                facts.Add(fact);
                            }
                            else
                                factura--;
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            App.global.numeroFactura = factura;
            document.Close(true);

            return facts;
        }
        private async Task Generar()
        {
            try
            {
                if (!TienePermiso)
                    TienePermiso = await GetPermisoEscritura();

                if (TienePermiso)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                    await Task.Delay(500);
                    var result = await CrearPDF();
                    Facturas = new ObservableCollection<FacturaModel>(result);
                    VisibleBoton = Facturas.Count > 0;
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }
        private void Ver(object parametro)
        {
            try
            {
                FacturaModel f = (FacturaModel)parametro;
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.ViendoDocumento = true;
                        DependencyService.Get<ISave>().SaveAndView(f.nombre+".pdf","application/pdf", GetStreamFromUrl(f.ruta));
                    });
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
                        foreach (FacturaModel c in Facturas)
                        {
                            MainThread.BeginInvokeOnMainThread(async () => await App.ResponseWS.nuevaFacturaEstablecimiento(c));
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

        #region Propiedades
        private static bool cargado = false;
        
        private ObservableCollection<FacturaModel> facturas;
        public ObservableCollection<FacturaModel> Facturas
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
        private bool cobrarCuota;
        public bool CobrarCuota
        {
            get
            {
                return cobrarCuota;
            }
            set
            {
                if (cobrarCuota != value)
                {
                    cobrarCuota = value;
                    OnPropertyChanged(nameof(CobrarCuota));
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

        private bool tienePermiso;

        public bool TienePermiso
        {
            get { return tienePermiso; }
            set { tienePermiso = value; }
        }


        #endregion

        #region Comandos
        public ICommand /*IAsyncRelayCommand*/ cmdGenerar { get { return new AsyncRelayCommand(async()=>await Generar()); } }
        public ICommand cmdVer { get { return new Command((parametro) => Ver(parametro)); } }
        public ICommand cmdAceptar { get { return new DelegateCommandAsync(Aceptar); } }
        public ICommand cmdRechazar { get { return new Command((parametro) => Rechazar(parametro)); } }
        #endregion
    }
}

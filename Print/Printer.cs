using ESC_POS_USB_NET.Enums;
using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Print.Command;
using AsadorMoron.Print.ModelCommand;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Maui.Devices;

namespace AsadorMoron.Print
{
    public class Printer : IPrinter
    {
        private byte[] _buffer;
        private readonly string _printerName;
        private readonly IPrintCommand _command;
        private readonly string _codepage;
        private string cod;

        public Printer(string printerName, string codigo, string codepage = "IBM01145")
        {
            _printerName = string.IsNullOrEmpty(printerName) ? "escpos.prn" : printerName.Trim();
            _command = new EscPos();
            _codepage = codepage;
            cod = codigo;
        }

        public int ColsNomal
        {
            get
            {
                return _command.ColsNomal;
            }
        }

        public int ColsCondensed
        {
            get
            {
                return _command.ColsCondensed;
            }
        }

        public int ColsExpanded
        {
            get
            {
                return _command.ColsExpanded;
            }
        }

        public void PrintDocument()
        {
            if (_buffer == null)
                return;
#if WINDOWS
            if (!RawPrinterHelper.SendBytesToPrinter(_printerName, _buffer))
                throw new ArgumentException("Unable to access printer : " + _printerName);
#else
            throw new PlatformNotSupportedException("Direct printing is only supported on Windows");
#endif
        }

        public void Append(string value)
        {
            AppendString(value, true);
        }

        public void Append(byte[] value)
        {
            if (value == null)
                return;
            var list = new List<byte>();
            if (_buffer != null)
                list.AddRange(_buffer);
            list.AddRange(value);
            _buffer = list.ToArray();
        }

        public void AppendWithoutLf(string value)
        {
            AppendString(value, false);
        }

        private void AppendString(string value, bool useLf)
        {
            if (string.IsNullOrEmpty(value))
                return;
            if (useLf)
                value += "\n";
            var list = new List<byte>();
            if (_buffer != null)
                list.AddRange(_buffer);
            var bytes = Encoding.GetEncoding(_codepage).GetBytes(value);
            list.AddRange(bytes);
            _buffer = list.ToArray();
        }

        public void NewLine()
        {
            Append("\r");
        }

        public void NewLines(int lines)
        {
            for (int i = 1, loopTo = lines - 1; i <= loopTo; i++)
                NewLine();
        }

        public void Clear()
        {
            _buffer = null;
        }

        public void Separator(char speratorChar = '-')
        {
            Append(_command.Separator(speratorChar));
        }

        public void AutoTest()
        {
            Append(_command.AutoTest());
        }

        public void ImprimirTicketPedido(int altura)
        {
            ConfiguracionAdmin cAdmin = ResponseServiceWS.getConfiguracionAdmin();
            CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
            if (c2.tipoVenta.Equals("Recogida"))
                c2.direccionUsuario = "";
            //NewLine();
            //SetLineHeight((byte)altura);
            AlignCenter();
            Append($"{cAdmin.nombreTicket.ToUpper().Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N')}");
            Append($"{cAdmin.cifTicket}");
            Append($"{cAdmin.direccionTicket}");
            Append($"{cAdmin.telefonoTicket}");
            Separator();
            Append(" ");
            AlignCenter();
            DoubleWidth2();
            if (c2.tipoVenta.Equals("Recogida"))
                BoldMode("RECOGIDA");
            else
                BoldMode("A DOMICILIO");

            AlignLeft();
            DoubleWidth2();
            Append($"{AppResources.Pedido.ToUpper()}: {cod}");
            Append($"FECHA: {c2.horaPedido.ToString("dd/MM/yyyy")}");
            Append($"H. PEDIDO: {c2.horaPedido.ToString("HH:mm")}");
            Append($"H. ENTREGA: {DateTime.Parse(c2.horaEntrega).AddMinutes(App.EstActual.configuracion.tiempoEntrega).ToString("HH:mm")}");
            NormalWidth();
            Append($"{AppResources.Pago.ToUpper()}: {c2.tipoPago}");
            Separator();
            Append(" ");




            double total = 0;
            double totalDescuento = 0;
            double totalBolsas = 0;
            double gastos2 = 0;
            if (!string.IsNullOrEmpty(c2.comentario.Trim()))
                Append($"{AppResources.Notas.ToUpper()}: {c2.comentario.Trim().ToUpper().Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N').ToLower()}");
            Append(" ");

            foreach (LineasPedido l in c2.lineasPedidos)
            {
                if (l.tipoComida == 3)
                    totalDescuento = l.precio;
                else if (l.tipoComida == 4)
                    totalBolsas = l.precio;
                else if (!l.nombreProducto.Equals("GASTOS DE ENVÍO"))
                {
                    string a = l.cantidad.ToString().PadRight(4, ' ') + l.nombreProducto.ToUpper().Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N');
                    string[] b = a.Split('\n');
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(b[i]))
                        {

                            if (i == 0)
                            {
                                string texto = "";
                                string resto = "";
                                if (b[i].Length > 42)
                                {
                                    texto = b[i].Substring(0, 42);
                                    resto = b[i].Substring(42);
                                }
                                else
                                    texto = b[i].PadRight(42, ' ');
                                if (l.pagadoConPuntos == 0)
                                    texto += string.Format("{0:N2}", l.cantidad * l.precio).PadLeft(6, ' ');
                                else
                                {
                                    texto += string.Format("{0:N2}", 0).PadLeft(6, ' ');
                                    totalDescuento += l.cantidad * l.precio;
                                }
                                total += l.cantidad * l.precio;
                                Append($"{texto}");
                                while (!string.IsNullOrEmpty(resto))
                                {
                                    texto = "";
                                    if (resto.Length > 38)
                                    {
                                        texto = "    " + resto.Substring(0, 38);
                                        resto = resto.Substring(38);
                                    }
                                    else
                                    {
                                        texto = "    " + resto;
                                        resto = "";
                                    }
                                    Append(texto);
                                }
                            }
                            else
                            {
                                string resto = b[i];
                                while (!string.IsNullOrEmpty(resto))
                                {
                                    string texto = "";
                                    if (resto.Length > 38)
                                    {
                                        texto = "    " + resto.Substring(0, 38);
                                        resto = resto.Substring(38);
                                    }
                                    else
                                    {
                                        texto = "    " + resto;
                                        resto = "";
                                    }
                                    Append(texto);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(l.comentario))
                    {
                        string resto = "Nota: " + l.comentario;
                        while (!string.IsNullOrEmpty(resto))
                        {
                            string texto = "";
                            if (resto.Length > 38)
                            {
                                texto = "    " + resto.Substring(0, 38);
                                resto = resto.Substring(38);
                            }
                            else
                            {
                                texto = "    " + resto;
                                resto = "";
                            }
                            BoldMode(texto);
                        }
                    }
                }
                else
                {
                    total += l.precio;
                    gastos2 = l.precio;
                }
                Append(" ");
            }

            if (gastos2 == 0 && c2.tipoVenta.StartsWith("Envío"))
            {
                ZonaModel z2 = App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo", 1)).Where(p => p.idZona == c2.idZona).FirstOrDefault();
                gastos2 = z2.gastos;
                total += z2.gastos;

            }
            double baseI = (total - totalDescuento + totalBolsas) / 1.10;
            if (c2.tipoVenta.StartsWith("Envío"))
            {

                Separator();
                BoldMode($"{AppResources.GastosEnvio.ToUpper().Replace('Í', 'I')}".PadRight(42) + string.Format("{0:N2}", gastos2).PadLeft(6, ' '));
            }
            Separator();
            if (totalBolsas > 0)
                BoldMode($"BOLSAS:".PadRight(42) + string.Format("{0:N2}", totalBolsas).PadLeft(6, ' '));
            Separator();
            if (totalDescuento > 0)
                BoldMode($"DESCUENTO:".PadRight(42) + string.Format("{0:N2}", totalDescuento).PadLeft(6, ' '));
            BoldMode($"{AppResources.Base.ToUpper()}".PadRight(42) + string.Format("{0:N2}", baseI).PadLeft(6, ' '));
            BoldMode($"{AppResources.Iva.ToUpper()} 10%".PadRight(42) + string.Format("{0:N2}", (total - totalDescuento + totalBolsas) - baseI).PadLeft(6, ' '));
            BoldMode($"{AppResources.Total.ToUpper()}".PadRight(42) + string.Format("{0:N2}", (total - totalDescuento + totalBolsas)).PadLeft(6, ' '));
            Separator();
            DoubleWidth2();
            UsuarioModel usu = ResponseServiceWS.getUsuario(c2.idUsuario);
            BoldMode($"{AppResources.Cliente}: {usu.nombre} {usu.apellidos}");
            BoldMode($"{AppResources.Direccion}: {c2.direccionUsuario}");
            ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
            BoldMode($"{AppResources.Zona}: {z.nombre}");
            BoldMode($"{AppResources.Telefono}:{usu.telefono}");
            NormalWidth();
            SetLineHeight((byte)(altura + 10));
            NewLine();
            NewLine();
            NewLine();
            FullPaperCut();

            /*Append("NORMAL - 48 COLUMNS");
            Append("1...5...10...15...20...25...30...35...40...45.48");
            Separator();
            Append("Text Normal");
            BoldMode("Bold Text");
            UnderlineMode("Underlined text");
            Separator();
            ExpandedMode(PrinterModeState.On);
            Append("Expanded - 23 COLUMNS");
            Append("1...5...10...15...20..23");
            ExpandedMode(PrinterModeState.Off);
            Separator();
            CondensedMode(PrinterModeState.On);
            Append("Condensed - 64 COLUMNS");
            Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            CondensedMode(PrinterModeState.Off);
            Separator();
            DoubleWidth2();
            Append("Font Width 2");
            DoubleWidth3();
            Append("Font Width 3");
            NormalWidth();
            Append("Normal width");
            Separator();
            AlignRight();
            Append("Right aligned text");
            AlignCenter();
            Append("Center-aligned text");
            AlignLeft();
            Append("Left aligned text");
            Separator();
            Font("Font A", Fonts.FontA);
            Font("Font B", Fonts.FontB);
            Font("Font C", Fonts.FontC);
            Font("Font D", Fonts.FontD);
            Font("Font E", Fonts.FontE);
            Font("Font Special A", Fonts.SpecialFontA);
            Font("Font Special B", Fonts.SpecialFontB);
            Separator();
            InitializePrint();
            SetLineHeight(24);
            Append("This is first line with line height of 30 dots");
            SetLineHeight(40);
            Append("This is second line with line height of 24 dots");
            Append("This is third line with line height of 40 dots");
            NewLines(3);
            Append("End of Test :)");
            Separator();
            FullPaperCut();*/
        }
        public void ImprimirTicketComanda(int numeroImpresora)
        {
            ConfiguracionAdmin cAdmin = ResponseServiceWS.getConfiguracionAdmin();
            CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
            try
            {
                if (c2.lineasPedidos.Where(p => p.numeroImpresora == numeroImpresora).ToList().Count > 0)
                {
                    NewLine();
                    AlignLeft();
                    ExpandedMode($"{AppResources.Pedido.ToUpper()}: {cod}");
                    Font($"{AppResources.Fecha.ToUpper()}: {c2.horaPedido.ToString("dd/MM/yyyy HH:mm:ss")}", Fonts.FontB);

                    if (!string.IsNullOrEmpty(c2.comentario.Trim()))
                        Font($"{AppResources.Notas.ToUpper()}: {c2.comentario.Trim().ToUpper().Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N').ToLower()}", Fonts.FontB);
                    Append(" ");
                    foreach (LineasPedido l in c2.lineasPedidos.Where(p => p.numeroImpresora == numeroImpresora))
                    {
                        if (!l.nombreProducto.Equals("GASTOS DE ENVÍO"))
                        {
                            string a = l.cantidad + " " + l.nombreProducto.ToUpper().Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N');
                            string[] b = a.Split('\n');
                            for (int i = 0; i < b.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(b[i]))
                                {

                                    if (i == 0)
                                    {
                                        if (b[i].Length > 35)
                                            b[i] = b[i].Substring(0, 35);
                                        else
                                            b[i] = b[i].PadRight(35, ' ');
                                    }
                                    else
                                        b[i] = "  " + b[i];

                                    Font($"{b[i]}", Fonts.FontB);
                                }
                            }
                        }
                    }
                    NewLine();
                    NewLine();
                    NewLine();
                    FullPaperCut();
                }
            }
            catch (Exception)
            {

            }
        }
        public void BoldMode(string value)
        {
            Append(_command.FontMode.Bold(value));
        }

        public void BoldMode(PrinterModeState state)
        {
            Append(_command.FontMode.Bold(state));
        }

        public void Font(string value, Fonts state)
        {
            Append(_command.FontMode.Font(value, state));
        }

        public void UnderlineMode(string value)
        {
            Append(_command.FontMode.Underline(value));
        }

        public void UnderlineMode(PrinterModeState state)
        {
            Append(_command.FontMode.Underline(state));
        }

        public void ExpandedMode(string value)
        {
            Append(_command.FontMode.Expanded(value));
        }

        public void ExpandedMode(PrinterModeState state)
        {
            Append(_command.FontMode.Expanded(state));
        }

        public void CondensedMode(string value)
        {
            Append(_command.FontMode.Condensed(value));
        }

        public void CondensedMode(PrinterModeState state)
        {
            Append(_command.FontMode.Condensed(state));
        }

        public void NormalWidth()
        {
            Append(_command.FontWidth.Normal());
        }

        public void DoubleWidth2()
        {
            Append(_command.FontWidth.DoubleWidth2());
        }

        public void DoubleWidth3()
        {
            Append(_command.FontWidth.DoubleWidth3());
        }

        public void AlignLeft()
        {
            Append(_command.Alignment.Left());
        }

        public void AlignRight()
        {
            Append(_command.Alignment.Right());
        }

        public void AlignCenter()
        {
            Append(_command.Alignment.Center());
        }

        public void FullPaperCut()
        {
            Append(_command.PaperCut.Full());
        }

        public void PartialPaperCut()
        {
            Append(_command.PaperCut.Partial());
        }

        public void OpenDrawer()
        {
            Append(_command.Drawer.Open());
        }

        public void QrCode(string qrData)
        {
            Append(_command.QrCode.Print(qrData));
        }

        public void QrCode(string qrData, QrCodeSize qrCodeSize)
        {
            Append(_command.QrCode.Print(qrData, qrCodeSize));
        }

        public void Code128(string code, Positions printString = Positions.NotPrint)
        {
            Append(_command.BarCode.Code128(code, printString));
        }

        public void Code39(string code, Positions printString = Positions.NotPrint)
        {
            Append(_command.BarCode.Code39(code, printString));
        }

        public void Ean13(string code, Positions printString = Positions.NotPrint)
        {
            Append(_command.BarCode.Ean13(code, printString));
        }

        public void InitializePrint()
        {
#if WINDOWS
            RawPrinterHelper.SendBytesToPrinter(_printerName, _command.InitializePrint.Initialize());
#endif
        }

        public void Image(Bitmap image)
        {
            Append(_command.Image.Print(image));
        }
        public void NormalLineHeight()
        {
            Append(_command.LineHeight.Normal());
        }

        public void SetLineHeight(byte height)
        {
            Append(_command.LineHeight.SetLineHeight(height));
        }

        private bool esMultiplo(int n1, int n2)
        {
            if (n1 % n2 == 0)
            {
                return true;
            }
            return false;
        }
    }
}

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
            NewLine();
            SetLineHeight((byte)altura);
            AlignCenter();
            Append($"{cAdmin.nombreTicket.ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N')}");
            AlignLeft();
            Font($"{cAdmin.cifTicket}", Fonts.FontB);
            Font($"{cAdmin.direccionTicket}", Fonts.FontB);
            Font($"{cAdmin.telefonoTicket}", Fonts.FontB);
            Append(" ");
            AlignCenter();
            Font($"{c2.nombreEstablecimiento.ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N')}", Fonts.FontA);
            AlignLeft();
            ExpandedMode($"{AppResources.Pedido.ToUpper()}: {cod}");
            Font($"{AppResources.Fecha.ToUpper()}: {c2.horaPedido.ToString("dd/MM/yyyy HH:mm:ss")}", Fonts.FontB);
            NewLine();
            Font($"{AppResources.Pago.ToUpper()}: {c2.tipoPago}", Fonts.FontB);
            

            double total = 0;
            double totalDescuento = 0;
            double totalBolsas = 0;
            double gastos2 = 0;
            if (!string.IsNullOrEmpty(c2.comentario.Trim()))
                Font($"{AppResources.Notas.ToUpper()}: {c2.comentario.Trim().ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N').ToLower()}", Fonts.FontB);
            Append(" ");
            foreach (LineasPedido l in c2.lineasPedidos)
            {
                if (l.tipoComida == 3)
                    totalDescuento = l.precio;
                else if (l.tipoComida == 4)
                    totalBolsas = l.precio;
                else if (!l.nombreProducto.Equals("GASTOS DE ENV�O"))
                {
                    string a = l.cantidad + " " + l.nombreProducto.ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N');
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
                                    b[i] = b[i].PadRight(35,' ');
                                if (l.pagadoConPuntos == 0)
                                    b[i] += string.Format("{0:N2}", l.cantidad * l.precio).PadLeft(6, ' ') ;
                                else
                                {
                                    b[i] += string.Format("{0:N2}", 0).PadLeft(6, ' ') ;
                                    totalDescuento += l.cantidad * l.precio;
                                }
                                total += l.cantidad * l.precio;
                            }else
                                b[i] = "  " + b[i];

                            Font($"{b[i]}", Fonts.FontB);
                            if (!string.IsNullOrEmpty(l.comentario))
                                Font("    " + l.comentario, Fonts.FontB);
                        }
                    }
                }
                else
                {
                    total += l.precio;
                    gastos2 = l.precio;
                }

            }

            if (gastos2 == 0 && c2.tipoVenta.StartsWith("Env�o"))
            {
                ZonaModel z2 = App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo",1)).Where(p => p.idZona == c2.idZona).FirstOrDefault();
                gastos2 = z2.gastos;
                total += z2.gastos;

            }
            double baseI = (total - totalDescuento+totalBolsas) / 1.10;
            if (c2.tipoVenta.StartsWith("Env�o"))
            {

                Append($"--------------------------------");
                Font($"{AppResources.GastosEnvio.ToUpper().Replace('�', 'I')}".PadRight(35) + string.Format("{0:N2}", gastos2).PadLeft(6, ' '), Fonts.FontB);
            }
            Append($"--------------------------------");
            if (totalDescuento > 0)
                Font($"BOLSAS:".PadRight(35) + string.Format("{0:N2}", totalBolsas).PadLeft(6, ' '), Fonts.FontB);
            Append($"--------------------------------");
            if (totalDescuento > 0)
                Font($"DESCUENTO:".PadRight(35) + string.Format("{0:N2}", totalDescuento).PadLeft(6, ' '), Fonts.FontB);
            Font($"{AppResources.Base.ToUpper()}".PadRight(35) + string.Format("{0:N2}", baseI).PadLeft(6, ' '), Fonts.FontB);
            Font($"{AppResources.Iva.ToUpper()} 10%".PadRight(35) + string.Format("{0:N2}", (total-totalDescuento+totalBolsas) - baseI).PadLeft(6, ' '), Fonts.FontB);
            Font($"{AppResources.Total.ToUpper()}".PadRight(35) + string.Format("{0:N2}", (total-totalDescuento+totalBolsas)).PadLeft(6, ' '), Fonts.FontB);
            Append($"--------------------------------");
            UsuarioModel usu = ResponseServiceWS.getUsuario(c2.idUsuario);
            Font($"{AppResources.Cliente}: {usu.nombre} {usu.apellidos}", Fonts.FontB);
            Font($"{AppResources.Direccion}: {c2.direccionUsuario}", Fonts.FontB);
            ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
            Font($"{AppResources.Zona}: {z.nombre}", Fonts.FontB);
            Font($"{AppResources.Telefono}:{usu.telefono}", Fonts.FontB);
            SetLineHeight((byte)(altura + 10));
            NewLine();
            NewLine();
            NewLine();
            FullPaperCut();

            //Append("NORMAL - 48 COLUMNS");
            //Append("1...5...10...15...20...25...30...35...40...45.48");
            //Separator();
            //Append("Text Normal");
            //BoldMode("Bold Text");
            //UnderlineMode("Underlined text");
            //Separator();
            //ExpandedMode(PrinterModeState.On);
            //Append("Expanded - 23 COLUMNS");
            //Append("1...5...10...15...20..23");
            //ExpandedMode(PrinterModeState.Off);
            //Separator();
            //CondensedMode(PrinterModeState.On);
            //Append("Condensed - 64 COLUMNS");
            //Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            //CondensedMode(PrinterModeState.Off);
            //Separator();
            //DoubleWidth2();
            //Append("Font Width 2");
            //DoubleWidth3();
            //Append("Font Width 3");
            //NormalWidth();
            //Append("Normal width");
            //Separator();
            //AlignRight();
            //Append("Right aligned text");
            //AlignCenter();
            //Append("Center-aligned text");
            //AlignLeft();
            //Append("Left aligned text");
            //Separator();
            //Font("Font A", Fonts.FontA);
            //Font("Font B", Fonts.FontB);
            //Font("Font C", Fonts.FontC);
            //Font("Font D", Fonts.FontD);
            //Font("Font E", Fonts.FontE);
            //Font("Font Special A", Fonts.SpecialFontA);
            //Font("Font Special B", Fonts.SpecialFontB);
            //Separator();
            //InitializePrint();
            //SetLineHeight(24);
            //Append("This is first line with line height of 30 dots");
            //SetLineHeight(40);
            //Append("This is second line with line height of 24 dots");
            //Append("This is third line with line height of 40 dots");
            //NewLines(3);
            //Append("End of Test :)");
            //Separator();
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
                        Font($"{AppResources.Notas.ToUpper()}: {c2.comentario.Trim().ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N').ToLower()}", Fonts.FontB);
                    Append(" ");
                    foreach (LineasPedido l in c2.lineasPedidos.Where(p => p.numeroImpresora == numeroImpresora))
                    {
                        if (!l.nombreProducto.Equals("GASTOS DE ENV�O"))
                        {
                            string a = l.cantidad + " " + l.nombreProducto.ToUpper().Replace('�', 'O').Replace('�', 'A').Replace('�', 'E').Replace('�', 'I').Replace('�', 'U').Replace('�', 'N');
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
            }catch (Exception)
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AsadorMoron.Utils
{
    public class NetPOSPrinter
    {
        string ipPort = "198.168.0.22";

        public NetPOSPrinter()
        {
        }

        public NetPOSPrinter(string IpPort)
        {
            this.ipPort = IpPort; // printer port 
        }

        ///   <summary> 
        /// output text to the printer 
        ///   </summary> 
        /// <param name = "str"> content to be printed </ param> 
        public void PrintLine(string str)
        {
            //establish connection 
            IPAddress ipa = IPAddress.Parse(ipPort);
            IPEndPoint ipe = new IPEndPoint(ipa, 9100); // 9100 designated port for the small ticket printer
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.Connect(ipe);

            // string str = "hello, 123456789, Hello, everyone!";

            byte[] b = System.Text.Encoding.GetEncoding("GB2312").GetBytes(str);
            soc.Send(b);
            soc.Close();
        }


        /*public void PrintPic(Bitmap bmp)
        {
            // ip and port into the instance IPEndPoint
            IPEndPoint ip_endpoint = new IPEndPoint(IPAddress.Parse(ipPort), 9100);

            // Create a Socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            // connect to the server
            socket.Connect(ip_endpoint);
            // Connect to deal with synchronization timeout long way, guess should be the first to establish a connection and then use the asynchronous mode,
            After confirming // connection is available, then an error or shut down, re-establish a synchronous connection                    

            //socket.SendTimeout = 1000;

            // Initialize the printer and print

            Byte[] byte_send = Encoding.GetEncoding("gb18030").GetBytes("\x1b\x40");

            // send a test message
            socket.Send(byte_send, byte_send.Length, 0);


            byte[] data = new byte[] { 0x1B, 0x33, 0x00 };
            socket.Send(data, data.Length, 0);
            data[0] = (byte)'\x00';
            data[1] = (byte)'\x00';
            data[2] = (byte)'\x00';    // Clear to Zero.

            Color pixelColor;

            // ESC * m nL nH bitmap
            byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x00, 0x00, 0x00 };

            escBmp[2] = (byte)'\x21';

            //nL, nH
            escBmp[3] = (byte)(bmp.Width % 256);
            escBmp[4] = (byte)(bmp.Width / 256);

            // data
            for (int i = 0; i < (bmp.Height / 24) + 1; i++)
            {
                socket.Send(escBmp, escBmp.Length, 0);

                for (int j = 0; j < bmp.Width; j++)
                {
                    for (int k = 0; k < 24; k++)
                    {
                        if (((i * 24) + k) < bmp.Height)   // if within the BMP size
                        {
                            pixelColor = bmp.GetPixel(j, (i * 24) + k);
                            if (pixelColor.R == 0)
                            {
                                data[k / 8] += (byte)(128 >> (k % 8));
                            }
                        }
                    }

                    socket.Send(data, 3, 0);
                    data[0] = (byte)'\x00';
                    data[1] = (byte)'\x00';
                    data[2] = (byte)'\x00';    // Clear to Zero.
                }

                byte_send = Encoding.GetEncoding("gb18030").GetBytes("\n");

                // send a test message
                socket.Send(byte_send, byte_send.Length, 0);
            } // data

            byte_send = Encoding.GetEncoding("gb18030").GetBytes("\n");

            // send a test message
            socket.Send(byte_send, byte_send.Length, 0);
            socket.Close();
        }*/


        ///   <summary> 
        /// open the cash box 
        ///   </summary> 
        public void OpenCashBox()
        {
            IPAddress ipa = IPAddress.Parse(ipPort);
            IPEndPoint ipe = new IPEndPoint(ipa, 9100); // 9100 designated port for the small ticket printer
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.Connect(ipe);
            char[] c = { Convert.ToChar(27), 'p', Convert.ToChar(0), Convert.ToChar(60), Convert.ToChar(255) };
            byte[] b = System.Text.Encoding.GetEncoding("GB2312").GetBytes(c);
            soc.Send(b);
            soc.Close();
        }


    }
}


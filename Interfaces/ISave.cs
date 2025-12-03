#region Copyright Syncfusion Inc. 2001-2019.
// Copyright Syncfusion Inc. 2001-2019. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;

namespace AsadorMoron.Interfaces
{
    public interface ISave
    {
        //Method to save document as a file and view the saved document
        void SaveAndView(string filename, string contentType, MemoryStream stream);
        void SaveAndUp(string filename,string nombre, MemoryStream stream);
        void SaveAndUpAdmin(string filename, string nombre, MemoryStream stream);
        void SaveImageFromByte(byte[] imageByte, string filename);
    }
}

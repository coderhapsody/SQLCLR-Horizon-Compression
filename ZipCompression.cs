using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{   
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString Zip(SqlString stringToZip)
    {
        string zipedString = "";
        byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToZip.Value);
        MemoryStream ms = new MemoryStream();

        ZipOutputStream zipOut = new ZipOutputStream(ms);
        ZipEntry ZipEntry = new ZipEntry("ZippedFile");
        zipOut.PutNextEntry(ZipEntry);
        zipOut.SetLevel(9);
        zipOut.Write(inputByteArray, 0, inputByteArray.Length);
        zipOut.Finish();
        zipOut.Close();
        //using (GZipStream zipOut = new GZipStream(ms, CompressionMode.Compress))
        //    zipOut.Write(inputByteArray, 0, inputByteArray.Length);

        byte[] outData = ms.ToArray();
        zipedString = Convert.ToBase64String(outData);
        
        return new SqlString(zipedString);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString Unzip(SqlString stringToUnzip)
    {
        string unzipedString = "";
        byte[] inputByteArray = Convert.FromBase64String(stringToUnzip.Value);
        MemoryStream ms = new MemoryStream(inputByteArray);
        MemoryStream ret = new MemoryStream();

        ZipInputStream zipIn = new ZipInputStream(ms);
        ZipEntry theEntry = zipIn.GetNextEntry();
        //using (GZipStream zipIn = new GZipStream(ms, CompressionMode.Decompress))
        //{
        Byte[] buffer = new Byte[2048];
        int size = 2048;
        while (true)
        {
            size = zipIn.Read(buffer, 0, buffer.Length);
            if (size > 0)
            {
                ret.Write(buffer, 0, size);
            }
            else
            {
                break;
            }
        }
        //}
        byte[] outData = ret.ToArray();
        unzipedString = Encoding.UTF8.GetString(outData);
        return new SqlString(unzipedString);
    }
}

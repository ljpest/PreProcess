using System;
using System.Net.FtpClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace PreProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            //download file
            //userid,pwd and ftp
            string userid = Properties.Settings.Default.UserID;
            string password = Properties.Settings.Default.PWD;
            string ftp = Properties.Settings.Default.FTP;
            string downloadPath = Properties.Settings.Default.DownLoadPath;
            string downloadFile = Properties.Settings.Default.DownLoadFileName;
            //string downloadLocal = Properties.Settings.Default.DownLoadLocal;
            string root = Environment.CurrentDirectory;
            root = root.Substring(0, root.IndexOf("PreProcess"));
            string utf8Path = root + Properties.Settings.Default.UTF8Path;
            string shiftjisPath = root + Properties.Settings.Default.ShiftJisPath;
            string archivePath = root + Properties.Settings.Default.ArchivePath;
            string logPath = root + Properties.Settings.Default.Log;
            string uploadPathPCV = Properties.Settings.Default.UploadPathPCV;
            string uploadPathIMP = Properties.Settings.Default.UploadPathIMP;
            string uploadPathREGION = Properties.Settings.Default.UploadPathREGION;


/*test*/
            //Console.WriteLine(System.Text.Encoding.Default.EncodingName);
            /*System.IO.StreamReader srtest = new System.IO.StreamReader(utf8Path + "IMP301010.dat" + "_temp.txt", System.Text.Encoding.UTF8);

            //Write
           
            string sstest = "";
            string nametest = "";

            try
            {
                while (!srtest.EndOfStream)
                {
                    sstest = srtest.ReadLine();
                    nametest = SubStr(sstest, 4, 14);
                    if (Regex.IsMatch(nametest, @"(\u0020|\u3000)+"))
                        nametest = Regex.Replace(nametest, @"(\u0020|\u3000)+", ",");
                    sstest = SubStr(sstest, 0, 4) + "," + nametest + ","
                        + SubStr(sstest, 18, 2) + "," + SubStr(sstest, 20, 2) + "," + SubStr(sstest, 22, 2) + ","
                        + SubStr(sstest, 24, 80);





                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/
            
 /* */









//WriteSuccessLog(logPath, "CovertError");
            string[] sArray = downloadFile.Split(','); ;
            
            using (FtpClient cl = new FtpClient(userid, password, ftp))
            {
               try
                {
                    cl.FtpLogStream = Console.OpenStandardOutput();
                    cl.FtpLogFlushOnWrite = true;
                    cl.DataChannelType = FtpDataChannelType.ExtendedPassive;
                    cl.SslMode = FtpSslMode.Explicit;
                    cl.SecurityNotAvailable += new SecurityNotAvailable(OnSecurityNotAvailable);
                    cl.InvalidCertificate += new FtpInvalidCertificate(OnInvalidCertficate);
                    cl.TransferProgress += new FtpTransferProgress(OnTransferProgress);

                    /*cl.ProxyType = System.Net.FtpClient.Proxy.ProxyType.Socks5;
                    cl.ProxyHost = "10.56.250.50";
                    cl.ProxyPort = 80;
                    cl.ProxyUsername = "";
                    cl.ProxyPassword = "";*/

                    FtpListItem[] fileList = cl.GetListing(downloadPath);
                    string fileName = "";
                    int fileCount = 0;
                    foreach(FtpListItem f in fileList)
                    {
                        if (f.Name.IndexOf("Keiyaku") > 0)
                        {
                            fileName = f.Name;
                            downloadFile = f.Name + "," + downloadFile;
                            //break;
                            fileCount++;
                        }
                    }
                    if (fileCount != 1) Environment.Exit(0);
                    
                    sArray = downloadFile.Split(',');
                    foreach(string s in sArray)
                        if (cl.FileExists(downloadPath + s))
                        {
                            cl.Download(downloadPath + s, shiftjisPath + s, FtpDataType.ASCII);
                            WriteSuccessLog(logPath, "Download" + " " + s);
                        }
                    if (cl.FileExists(downloadPath + fileName))
                        cl.RemoveFile(downloadPath + fileName);
                    
                }
                catch (Exception e)
                {
                    WriteErrLog(logPath, "Error occurred during pre-processing", e);
                }

            }

            //convert shift-jis to utf-8
            string utfpcv = "";
            try
            {
                foreach (string s in sArray)
                
                {
                    //if (!"IMP301005.dat".Equals(s))
                    //{
                        if (File.Exists(shiftjisPath + s))
                            Convert(shiftjisPath + s, utf8Path + s + "_temp.txt");

                        System.IO.StreamReader sr;
                        if (File.Exists(utf8Path + s + "_temp.txt"))
                        {
                            sr = new System.IO.StreamReader(utf8Path + s + "_temp.txt", System.Text.Encoding.UTF8);

                            //Write
                            StreamWriter sw = new StreamWriter(utf8Path + "WR177254_UTF_"+ s, false);
                            string ss, sstest1, sstest2, sstest3 = "";
                            //string name = "";
                            int pcvfileCount = 0;
                            while (!sr.EndOfStream)
                            {
                                ss = sr.ReadLine();
                                pcvfileCount++;
                                if ("IMP301010.dat".Equals(s))
                                {
                                    //name = SubStr(ss, 4, 14);
                                    //if (Regex.IsMatch(name, @"(\u0020|\u3000)+"))
                                    //    name = Regex.Replace(name, @"(\u0020|\u3000)+", ",");
                                    //ss = SubStr(ss, 0, 4) + "," + name + ","
                                    //    + SubStr(ss, 18, 2) + "," + SubStr(ss, 20, 2) + "," + SubStr(ss, 22, 2) + ","
                                    //    + SubStr(ss, 24, 80);
                                    //sw.WriteLine(ss);
                                    //modified by jili 20120912 start
                                    sstest1 = bSubstring(ss, 4).Trim();
                                    sstest2 = bSubstring(ss, 18).Trim().Replace(sstest1, "");
                                    sstest3 = ss.Replace(bSubstring(ss, 18), "").Trim();
                                    if (Regex.IsMatch(sstest2, @"(\u0020|\u3000)+"))
                                        sstest2 = Regex.Replace(sstest2, @"(\u0020|\u3000)+", ",");
                                    ss = sstest1 + "," + sstest2 + ","
                                        + sstest3.Substring(0, 2) + "," + sstest3.Substring(2, 2) + "," + sstest3.Substring(4, 2) + ","
                                        + sstest3.Substring(6);
                                    sw.WriteLine(ss);
                                    //modified by jili 20120912 end
                                }
                                else if ("IMP301005.dat".Equals(s))
                                {
                                    ss = SubStr(ss, 0, 3) + "," + ss.Substring(3).Trim();
                                    sw.WriteLine(ss);
                                }
                                else
                                {
                                    if (pcvfileCount > 4)
                                    {
                                        utfpcv = "WR177254_UTF_" + s;
                                        ss = ss.Replace("\u0009", ",");
                                        sw.WriteLine(ss);
                                    }
                                }

                                
                            }

                            sw.Flush();
                            sw.Close();
                            sr.Close();

                            //delete temp file and copy input file to archive folder
                            if (File.Exists(utf8Path + s + "_temp.txt"))
                                File.Delete(utf8Path + s + "_temp.txt");
                            if (File.Exists(shiftjisPath + s))
                            {
                                //string fileType=inputfileName.Substring(inputfileName.LastIndexOf("."));
                                DateTime now = DateTime.Now;
                                string strNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                                string archiveStr = s + "_" + strNow;//+ fileType.ToString();
                                File.Move(shiftjisPath + s, archivePath + archiveStr);
                            }
                            WriteSuccessLog(logPath, "Convert" + " " + s);
                        }
                    //}
                }
            }
            catch (Exception e)
            {
                WriteErrLog(logPath, "CovertError", e);
            }
            
            //upload utf file
            using (FtpClient clu = new FtpClient(userid, password, ftp))
            {
                try
                {
                    clu.FtpLogStream = Console.OpenStandardOutput();
                    clu.FtpLogFlushOnWrite = true;
                    clu.DataChannelType = FtpDataChannelType.ExtendedPassive;
                    clu.SslMode = FtpSslMode.Explicit;
                    clu.SecurityNotAvailable += new SecurityNotAvailable(OnSecurityNotAvailable);
                    clu.InvalidCertificate += new FtpInvalidCertificate(OnInvalidCertficate);
                    clu.TransferProgress += new FtpTransferProgress(OnTransferProgress);

                    //string utfpcv = "WR177254_UTF_PCV_to_PMS.txt";
                    string utfimp = "WR177254_UTF_IMP301010.dat";
                    if (File.Exists(utf8Path + utfpcv))
                    {
                        clu.Upload(utf8Path + utfpcv, uploadPathPCV + utfpcv, FtpDataType.ASCII);
                        WriteSuccessLog(logPath, "Upload" + " " + utfpcv);
                    }
                    string utfregion = "WR177254_UTF_IMP301005.dat";
                    if (File.Exists(utf8Path + utfregion))
                    {
                        clu.Upload(utf8Path + utfregion, uploadPathREGION + utfregion, FtpDataType.ASCII);
                        WriteSuccessLog(logPath, "Upload" + " " + utfregion);
                    }
                    if (File.Exists(utf8Path + utfimp))
                    {
                        clu.Upload(utf8Path + utfimp, uploadPathIMP + utfimp, FtpDataType.ASCII);
                        WriteSuccessLog(logPath, "Upload" + " " + utfimp);
                    }
                    

                }
                catch (Exception e)
                {
                    WriteErrLog(logPath, "Error occurred during pre-processing", e);
                }

            }

            //Console.ReadLine();
        }
        #region
        static void OnSecurityNotAvailable(FtpSecurityNotAvailable e)
        {
            // SSL/TLS could not be negotiated with the AUTH command.
            // If you do not want login credentials to be sent in plain
            // text set the e.Cancel property true to cancel the login.
            // Doing so will trigger a FtpCommandException to be thrown
            // for the failed AUTH command.
            e.Cancel = false;
        }
        static void OnInvalidCertficate(FtpChannel c, InvalidCertificateInfo e)
        {
            // we don't care if a certificate is invalid
            e.Ignore = true;
        }

        static void OnTransferProgress(FtpTransferInfo e)
        {
            // e.TransferType == FtpTransferType.Download
            Console.Write("\r{0}/{1} {2}% {3}/s       ",
                FormatBytes(e.Transferred), FormatBytes(e.Length), e.Percentage, FormatBytes(e.BytesPerSecond));

            if (e.Complete)
            {
                Console.WriteLine();
            }
        }

        static string FormatBytes(long size)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB" };
            double val = size;
            int count = 0;

            while (val > 1024 && count < units.Length)
            {
                val /= 1024;
                count++;
            }

            return string.Format("{0}{1}", Math.Round(val, 2), units[count]);
        }

        /// <summary>
        /// Write error log
        /// </summary>
        private static void WriteErrLog(string path, string errTitle, Exception ex)
        {
            path = path + "errLog.txt";

            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                float MyFileSize = (float)f.Length / (1024 * 1024);
                if (MyFileSize > 100)
                    File.Delete(path);
            }

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            StringBuilder strBuilderErrorMessage = new StringBuilder();

            strBuilderErrorMessage.Append("________________________________________________________________________________________________________________\r\n");
            strBuilderErrorMessage.Append("Date:" + System.DateTime.Now.ToString() + "\r\n");
            strBuilderErrorMessage.Append("Error Tile:" + errTitle + "\r\n");
            strBuilderErrorMessage.Append("Error Message:" + ex.Message + "\r\n");
            strBuilderErrorMessage.Append("Error Stack:" + ex.StackTrace + "\r\n");
            strBuilderErrorMessage.Append("________________________________________________________________________________________________________________\r\n");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.Write(strBuilderErrorMessage);
                sw.Flush();
                sw.Close();
            }
        }

        
        /// <summary>
        /// Write Success log
        /// </summary>
        private static void WriteSuccessLog(string path, string title)
        {
            path = path + "success.txt";

            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                float MyFileSize = (float)f.Length / (1024 * 1024);
                if (MyFileSize > 100)
                    File.Delete(path);
            }

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            StringBuilder strMessage = new StringBuilder();

            strMessage.Append("________________________________________________________________________________________________________________\r\n");
            strMessage.Append("Date:" + System.DateTime.Now.ToString() + "\r\n");
            strMessage.Append(title + " Success!" + "\r\n");
            strMessage.Append("________________________________________________________________________________________________________________\r\n");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.Write(strMessage);
                sw.Flush();
                sw.Close();
            }
        }
        #endregion
        
        #region
        private static string SubStr(string str, int a_StartIndex, int a_Cnt)
        {
            byte[] l_byte = System.Text.Encoding.Default.GetBytes(str);
            return System.Text.Encoding.Default.GetString(l_byte, a_StartIndex, a_Cnt).Trim();
        }

        public static String bSubstring(string s, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            int n = 0;  //  表示当前的字节数
            int i = 0;  //  要截取的字节数
            for (; i < bytes.GetLength(0) && n < length; i++)
            {
                //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    n++;      //  在UCS2第一个字节时n加1
                }
                else
                {
                    //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }

            }
            //  如果i为奇数时，处理成偶数
            if (i % 2 == 1)
            {
                //  该UCS2字符是汉字时，去掉这个截一半的汉字
                if (bytes[i] > 0)
                    i = i - 1;
                 //  该UCS2字符是字母或数字，则保留该字符
                else
                    i = i + 1;
            }
            return System.Text.Encoding.Unicode.GetString(bytes, 0, i);
        }
        
        /// <summary>
        /// Convert shift-jis to utf-8
        /// </summary>
        private static void Convert(string inputStr, string outputStr)
        {
            using (TextReader input = new StreamReader(new FileStream(inputStr, FileMode.Open), Encoding.GetEncoding("shift-jis")))
            {
                using (TextWriter output = new StreamWriter(
                  new FileStream(outputStr, FileMode.Create), Encoding.UTF8))
                {
                    var buffer = new char[512];
                    int len;

                    while ((len = input.Read(buffer, 0, 512)) > 0)
                    {
                        output.Write(buffer, 0, len);
                    }
                }
            }
        }
        #endregion
    }
}

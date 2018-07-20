using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Processor.Logic
{
    class Hashes
    {
        /// <summary>
        /// Calculates a MD5 of this given FileStream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string MD5(FileStream fileStream)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(fileStream);

                fileStream.Close();

                StringBuilder sb = new StringBuilder();
                foreach (byte a in hash)
                {
                    if (a < 16)
                        sb.Append("0" + a.ToString("x"));
                    else
                        sb.Append(a.ToString("x"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "Hashes");
                throw;
            }
        }

        /// <summary>
        /// Calculates a MD5 of this given FileStream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string MD5(Byte[] fileStream)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(fileStream);

                StringBuilder sb = new StringBuilder();
                foreach (byte a in hash)
                {
                    if (a < 16)
                        sb.Append("0" + a.ToString("x"));
                    else
                        sb.Append(a.ToString("x"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "Hashes");
                throw;
            }
        }

        /// <summary>
        /// Calculates a MD5 of this given FileStream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string MD5(String fileString)
        {
            byte[] fileStream = Encoding.UTF8.GetBytes(fileString);

            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(fileStream);

                StringBuilder sb = new StringBuilder();
                foreach (byte a in hash)
                {
                    if (a < 16)
                        sb.Append("0" + a.ToString("x"));
                    else
                        sb.Append(a.ToString("x"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "Hashes");
                throw;
            }
        }
    }
}

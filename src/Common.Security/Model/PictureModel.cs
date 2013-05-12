using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DJRM.Common.Security.Model
{
    public class PictureModel
    {
        public string Email { get; set; }
        public string PictureURL { get; set; }

        public string HashedEmail
        {
            get
            {
                if (!String.IsNullOrEmpty(_hashedEmail))
                    return _hashedEmail;

                if (String.IsNullOrEmpty(Email))
                    return String.Empty;

                using (MD5 md5Hash = MD5.Create())
                {
                    // Get hash from email as binary array
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Email));

                    StringBuilder sBuilder = new StringBuilder();

                    // Convert binary array to string displaying hexadecimal
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    _hashedEmail = sBuilder.ToString();
                }

                return _hashedEmail;
            }
        }
        private string _hashedEmail;
    }
}

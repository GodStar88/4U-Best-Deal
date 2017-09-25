using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _4U_Best_Deal
{
    class CKeyCheck
    {
        public bool KeyCheck()
        {
            WebRequest request = null;
            WebResponse response = null;
            Stream resStream = null;
            StreamReader resReader = null;
            string resstring = "";
            try
            {
                string uristring = "http://3901-22185.el-alt.com/bot/1.php";
                request = WebRequest.Create(uristring.Trim());
                response = request.GetResponse();
                resStream = response.GetResponseStream();
                resReader = new StreamReader(resStream);
                resstring = resReader.ReadToEnd();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (resReader != null) resReader.Close();
                if (response != null) response.Close();
            }

            if (resstring.Contains("4U Best Deal")) return true;

            return false;
        }
    }
}

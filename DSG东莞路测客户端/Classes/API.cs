using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSG东莞路测客户端
{
    class API
    {
        public static async Task<NormalResponse> GetWhiteRadio(string province = "", string city = "", string district = "", int getCount = 0)
        {
            Dictionary<string, object> dik = new Dictionary<string, object>();
            dik.Add("province", province);
            dik.Add("city", city);
            dik.Add("district", district);
            dik.Add("getCount", getCount);
            return await HttpHelper.Get("/api/Platform/GetWhiteRadio", dik);
        }

    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayardsSafetyApp.Entities
{
    public static class Utils //useful common methods
    {
        public static string SerializeToJson(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T DeserializeFromJson<T>(string jsonObj)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(jsonObj);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.ViewModel
{
    public class Microatmdeviceadd
    {
        public string deviceid { get; set; }
        public DateTime inserteddate { get; set; }
        public string planname { get; set; }
        public string userid { get; set; }
    }
    public class GetAllDeviceinfo
    {
        public IEnumerable<show_microatm_deviceinfo_Result> devicelist { get; set; }
        public MIcroatm_admin_device_info DevicelistByid { get; set; }
        public string msg { get; set; }

    }
}
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class SettingViewModel
    {
        public Setting Settings { get; set; }

        public SettingViewModel()
        {

        }
        public SettingViewModel(Setting setting)
        {
            Settings = setting;
        }
    }
}

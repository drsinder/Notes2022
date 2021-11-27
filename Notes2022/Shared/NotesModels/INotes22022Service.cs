using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Notes2022.Shared
{
    public interface INotes2022Service
    {
        public AboutModel GetAboutModel();


    }
}

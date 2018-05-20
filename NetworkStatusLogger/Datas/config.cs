using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Runtime.Serialization;

namespace NetworkStatusLogger.Datas
{
    [DataContract(Name = "Config")]
    class Config
    {
        [DataMember( Name ="LogFilePath")]
        public string LogFilePath { get; set; }
        [DataMember(Name = "SurveyTimeSpan")]
        public TimeSpan Span { get; set; }
        [DataMember(Name = "ConnectServerAddresses")]
        public List<string> Adresses { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChurnGuard_Client.Models
{
    public class FinalReportResponse
    {
        public string SummaryReport { get; set; } =string.Empty;
        public List<ModelMetric> AllGridResults { get; set; } = new();
    }
}

namespace ChurnGuard_Client.Models
{
    public class ModelMetric
    {
        public string ModelName { get; set; } = string.Empty;
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1 { get; set; }
    }
}

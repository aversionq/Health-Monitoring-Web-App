namespace AuthServer.ResponseModels
{
    public class BmiResponse
    {
        public double UserHeight { get; set; }
        public double UserWeight { get; set; }
        public double Bmi
        {
            get { return UserWeight / Math.Pow(UserHeight / 100, 2); }
            set { Bmi = value; }
        }
    }
}

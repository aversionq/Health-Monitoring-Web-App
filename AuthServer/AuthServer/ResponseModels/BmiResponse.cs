using AuthServer.Services;

namespace AuthServer.ResponseModels
{
    public class BmiResponse
    {
        public double? UserHeight { get; set; }
        public double? UserWeight { get; set; }
        public double? Bmi
        {
            get
            {
                if (UserHeight is not null && UserWeight is not null)
                {
                    return UserWeight / Math.Pow((double)UserHeight / 100, 2);
                }
                else
                {
                    return null;
                }
            }
            set { Bmi = value; }
        }
        public string BmiState 
        {
            get
            {
                if (Bmi is not null)
                {
                    return BmiStateHandler.GetBmiState(Bmi.Value).ToString();
                }
                else
                {
                    return BmiStateHandler.BmiStateType.None.ToString();
                }
            }
            set
            {
                BmiState = value;
            }
        }
    }
}

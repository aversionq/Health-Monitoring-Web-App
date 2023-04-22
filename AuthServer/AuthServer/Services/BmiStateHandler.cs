namespace AuthServer.Services
{
    public static class BmiStateHandler
    {
        public enum BmiStateType
        {
            None,
            Underweight,
            Normal,
            Overweight,
            Obese
        }

        public static BmiStateType GetBmiState(double? bmiValue)
        {
            switch (bmiValue)
            {
                case < 18.5:
                    return BmiStateType.Underweight;
                case >= 18.5 and < 24.9:
                    return BmiStateType.Normal;
                case >= 24.9 and < 29.9:
                    return BmiStateType.Overweight;
                case >= 29.9:
                    return BmiStateType.Obese;
                default:
                    return BmiStateType.None;
            }
        }
    }
}

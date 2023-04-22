using HealthMonitoringApp.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Services
{
    public static class MedicalStateHandler
    {
        public static MedicalState.MedicalStateType GetUserBloodSugarState(int userSugarValue)
        {
            switch (userSugarValue)
            {
                case < 80:
                    return MedicalState.MedicalStateType.Low;
                case >= 80 and <100:
                    return MedicalState.MedicalStateType.Normal;
                case >= 100 and < 125:
                    return MedicalState.MedicalStateType.Elevated;
                case >= 125:
                    return MedicalState.MedicalStateType.High;
                default:
                    return MedicalState.MedicalStateType.None;
            }
        }

        public static MedicalState.MedicalStateType GetUserHeartRateState(int userHeartRateValue)
        {
            switch (userHeartRateValue)
            {
                case < 60:
                    return MedicalState.MedicalStateType.Low;
                case >= 60 and < 90:
                    return MedicalState.MedicalStateType.Normal;
                case >= 90 and < 115:
                    return MedicalState.MedicalStateType.Elevated;
                case >= 115:
                    return MedicalState.MedicalStateType.High;
                default:
                    return MedicalState.MedicalStateType.None;
            }
        }

        public static MedicalState.MedicalStateType GetUserPressureState(int userSystolic, int userDiastolic)
        {
            switch (userSystolic, userDiastolic)
            {
                case ( < 90, < 60):
                    return MedicalState.MedicalStateType.Low;
                case ( >= 90 and < 120, >= 60 and < 80):
                    return MedicalState.MedicalStateType.Normal;
                case ( >= 120 and < 130, >= 80 and <90):
                    return MedicalState.MedicalStateType.Elevated;
                case ( >= 130, >= 90):
                    return MedicalState.MedicalStateType.High;
                default:
                    return MedicalState.MedicalStateType.None;
            }
        }
    }
}

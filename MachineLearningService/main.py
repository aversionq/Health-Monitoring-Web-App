# uvicorn main:app --host 127.0.0.1 --port 8000

from fastapi import FastAPI, APIRouter, Depends
from fastapi.middleware.cors import CORSMiddleware
import joblib
import pandas as pd

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

router = APIRouter()

def predict_diabetes(highBp, highChol, bmi, stroke, heartDiseaseAttack, genHlth, physHlth, diffWalk, age):
    age = int(age)
    classified_age = None
    
    if age <= 24:
        classified_age = 1
    elif 25 <= age <= 29:
        classified_age = 2
    elif 30 <= age <= 34:
        classified_age = 3
    elif 35 <= age <= 39:
        classified_age = 4
    elif 40 <= age <= 44:
        classified_age = 5
    elif 45 <= age <= 49:
        classified_age = 6
    elif 50 <= age <= 54:
        classified_age = 7
    elif 55 <= age <= 59:
        classified_age = 8
    elif 60 <= age <= 64:
        classified_age = 9
    elif 65 <= age <= 69:
        classified_age = 10
    elif 70 <= age <= 74:
        classified_age = 11
    elif 75 <= age <= 79:
        classified_age = 12
    else:
        classified_age = 13

    user_data = {
        'HighBP': [highBp],
        'HighChol': [highChol],
        'BMI': [bmi],
        'Stroke': [stroke],
        'HeartDiseaseorAttack': [heartDiseaseAttack],
        'GenHlth': [genHlth],
        'PhysHlth': [physHlth],
        'DiffWalk': [diffWalk],
        'Age': [classified_age]
    }
    user_df = pd.DataFrame(data=user_data)
    model = joblib.load("diabetics_ml_test.joblib")
    prediction = model.predict(user_df)

    return int(prediction[0])

@router.post("/diabeticsPrediction")
def get_ml_resp(highBp, highChol, bmi, stroke, heartDiseaseAttack, genHlth, physHlth, diffWalk, age):
    return predict_diabetes(highBp, highChol, bmi, stroke, heartDiseaseAttack, genHlth, physHlth, diffWalk, age)

app.include_router(router, prefix="")

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
    user_data = {
        'HighBP': [highBp],
        'HighChol': [highChol],
        'BMI': [bmi],
        'Stroke': [stroke],
        'HeartDiseaseorAttack': [heartDiseaseAttack],
        'GenHlth': [genHlth],
        'PhysHlth': [physHlth],
        'DiffWalk': [diffWalk],
        'Age': [age]
    }
    user_df = pd.DataFrame(data=user_data)
    model = joblib.load("diabetics_ml_test.joblib")
    prediction = model.predict(user_df)

    return int(prediction[0])

@router.post("/diabeticsPrediction")
def get_ml_resp(highBp, highChol, bmi, stroke, heartDiseaseAttack, genHlth, physHlth, diffWalk, age):
    return predict_diabetes(highBp, highChol, bmi, stroke, heartDiseaseAttack, genHlth, physHlth, diffWalk, age)

app.include_router(router, prefix="")
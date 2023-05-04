FROM python:3.11

EXPOSE 80
EXPOSE 443

RUN mkdir /fastapi_app

WORKDIR /fastapi_app

COPY requirements.txt .

RUN pip install -r requirements.txt

COPY . .

WORKDIR .

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "80"]
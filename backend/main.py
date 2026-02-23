from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.core.database import Base, engine
from app.api.routes import auth, admin, maestro, alumno, padre, inscripciones, periodos
from app.bot import start_polling

# Import all models so SQLAlchemy registers them
from app.models import user, alumno as alumno_model, maestro as maestro_model, padre as padre_model, inscripcion as inscripcion_model  # noqa

Base.metadata.create_all(bind=engine)


@asynccontextmanager
async def lifespan(app: FastAPI):
    start_polling()   # start Telegram bot polling in background thread
    yield


app = FastAPI(
    title="Portal IIM",
    version="1.0.0",
    description="Instituto Intercultural Monterrey",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173", "http://localhost:3000", "https://prepaiim.tudominio.com"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(auth.router, prefix="/api")
app.include_router(admin.router, prefix="/api")
app.include_router(maestro.router, prefix="/api")
app.include_router(alumno.router, prefix="/api")
app.include_router(padre.router, prefix="/api")
app.include_router(inscripciones.router, prefix="/api")
app.include_router(periodos.router, prefix="/api")


@app.get("/health")
def health():
    return {"status": "ok", "system": "Portal IIM — Instituto Intercultural Monterrey"}

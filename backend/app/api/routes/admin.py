from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.core.security import get_password_hash
from app.api.deps import require_role
from app.models.user import User, RoleEnum
from app.models.alumno import Alumno, Adeudo
from pydantic import BaseModel, EmailStr
from datetime import datetime

router = APIRouter(prefix="/admin", tags=["admin"])
admin_only = require_role(RoleEnum.admin)


class CreateUserRequest(BaseModel):
    email: EmailStr
    password: str
    nombre: str
    apellido: str
    role: RoleEnum


class CreateAdeudoRequest(BaseModel):
    alumno_id: int
    concepto: str
    monto: float
    fecha_vencimiento: datetime


@router.get("/alumnos")
def get_alumnos(db: Session = Depends(get_db), _=Depends(admin_only)):
    alumnos = db.query(Alumno).join(User).filter(User.activo == True).all()
    return [
        {
            "id": a.id,
            "matricula": a.matricula,
            "nombre": a.user.nombre,
            "apellido": a.user.apellido,
            "email": a.user.email,
            "tetramestre": a.tetramestre_actual,
            "grupo": a.grupo,
        }
        for a in alumnos
    ]


@router.post("/usuarios")
def create_user(data: CreateUserRequest, db: Session = Depends(get_db), _=Depends(admin_only)):
    existing = db.query(User).filter(User.email == data.email).first()
    if existing:
        raise HTTPException(status_code=400, detail="Email ya registrado")
    user = User(
        email=data.email,
        nombre=data.nombre,
        apellido=data.apellido,
        hashed_password=get_password_hash(data.password),
        role=data.role,
    )
    db.add(user)
    db.commit()
    db.refresh(user)
    return {"id": user.id, "email": user.email, "role": user.role}


@router.get("/adeudos")
def get_adeudos(db: Session = Depends(get_db), _=Depends(admin_only)):
    adeudos = db.query(Adeudo).join(Alumno).join(User).all()
    return [
        {
            "id": a.id,
            "alumno": f"{a.alumno.user.nombre} {a.alumno.user.apellido}",
            "concepto": a.concepto,
            "monto": a.monto,
            "pagado": a.pagado,
            "pendiente": a.monto - a.pagado,
            "vencimiento": a.fecha_vencimiento,
        }
        for a in adeudos
    ]


@router.post("/adeudos")
def create_adeudo(data: CreateAdeudoRequest, db: Session = Depends(get_db), _=Depends(admin_only)):
    adeudo = Adeudo(**data.dict())
    db.add(adeudo)
    db.commit()
    return {"ok": True}

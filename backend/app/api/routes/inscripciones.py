from fastapi import APIRouter, Depends
from pydantic import BaseModel
from datetime import datetime
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.models.inscripcion import Inscripcion
from app.api.deps import get_current_user
from app.models.user import User

router = APIRouter(tags=['inscripciones'])

class InscripcionIn(BaseModel):
    nombre: str
    telefono: str
    secundaria: str

class InscripcionOut(InscripcionIn):
    id: int
    created_at: datetime | None = None

    class Config:
        from_attributes = True

@router.post('/inscripciones', response_model=InscripcionOut, status_code=201)
def crear_inscripcion(data: InscripcionIn, db: Session = Depends(get_db)):
    ins = Inscripcion(**data.model_dump())
    db.add(ins)
    db.commit()
    db.refresh(ins)
    return ins

@router.get('/inscripciones', response_model=list[InscripcionOut])
def listar_inscripciones(
    db: Session = Depends(get_db),
    current_user: User = Depends(get_current_user)
):
    if current_user.role != 'admin':
        from fastapi import HTTPException
        raise HTTPException(status_code=403, detail='Solo admins')
    return db.query(Inscripcion).order_by(Inscripcion.created_at.desc()).all()

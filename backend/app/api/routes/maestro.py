from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.api.deps import require_role, get_current_user
from app.models.user import User, RoleEnum
from app.models.maestro import Maestro, Tarea, MaestroMateria
from app.models.alumno import Calificacion
from pydantic import BaseModel
from typing import Optional
from datetime import datetime

router = APIRouter(prefix="/maestro", tags=["maestro"])
maestro_only = require_role(RoleEnum.maestro, RoleEnum.admin)


class TareaCreate(BaseModel):
    materia_id: int
    titulo: str
    descripcion: Optional[str] = None
    fecha_entrega: Optional[datetime] = None


class CalificacionSet(BaseModel):
    alumno_id: int
    materia_id: int
    tetramestre: int
    calificacion: float


@router.get("/mis-materias")
def get_mis_materias(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(maestro_only),
):
    maestro = db.query(Maestro).filter(Maestro.user_id == current_user.id).first()
    if not maestro:
        return []
    materias = db.query(MaestroMateria).filter(MaestroMateria.maestro_id == maestro.id).all()
    return [
        {
            "id": mm.materia.id,
            "nombre": mm.materia.nombre,
            "tetramestre": mm.materia.tetramestre,
            "grupo": mm.grupo,
        }
        for mm in materias
    ]


@router.post("/tareas")
def create_tarea(data: TareaCreate, db: Session = Depends(get_db), _=Depends(maestro_only)):
    tarea = Tarea(**data.dict())
    db.add(tarea)
    db.commit()
    return {"ok": True}


@router.put("/calificaciones")
def set_calificacion(data: CalificacionSet, db: Session = Depends(get_db), _=Depends(maestro_only)):
    cal = (
        db.query(Calificacion)
        .filter(
            Calificacion.alumno_id == data.alumno_id,
            Calificacion.materia_id == data.materia_id,
            Calificacion.tetramestre == data.tetramestre,
        )
        .first()
    )
    if cal:
        cal.calificacion = data.calificacion
    else:
        cal = Calificacion(**data.dict())
        db.add(cal)
    db.commit()
    return {"ok": True}

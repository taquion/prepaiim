from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from pydantic import BaseModel
from typing import Optional
from datetime import datetime

from app.core.database import get_db
from app.api.deps import require_role, get_current_user
from app.models.user import User, RoleEnum
from app.models.maestro import Maestro, Tarea, MaestroMateria, Planeacion, Periodo
from app.models.alumno import Calificacion

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


class PlaneacionUpsert(BaseModel):
    titulo: Optional[str] = None
    contenido: Optional[str] = None


# ─── Mis materias del periodo activo ──────────────────────────────────────────

@router.get("/mis-materias")
def get_mis_materias(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(maestro_only),
):
    maestro = db.query(Maestro).filter(Maestro.user_id == current_user.id).first()
    if not maestro:
        return []

    periodo_activo = db.query(Periodo).filter(Periodo.activo == True).first()

    q = db.query(MaestroMateria).filter(MaestroMateria.maestro_id == maestro.id)
    if periodo_activo:
        q = q.filter(MaestroMateria.periodo_id == periodo_activo.id)

    materias = q.all()
    return [
        {
            "asignacion_id": mm.id,
            "materia_id": mm.materia.id,
            "nombre": mm.materia.nombre,
            "clave": mm.materia.clave,
            "grupo": mm.grupo or "—",
            "periodo": mm.periodo.nombre if mm.periodo else None,
            "semanas": mm.periodo.semanas if mm.periodo else 14,
            "planeaciones_completadas": len([p for p in mm.planeaciones if p.contenido]),
        }
        for mm in materias
    ]


# ─── Planeaciones de una asignación (14 semanas) ─────────────────────────────

@router.get("/planeaciones/{asignacion_id}")
def get_planeaciones(
    asignacion_id: int,
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(maestro_only),
):
    maestro = db.query(Maestro).filter(Maestro.user_id == current_user.id).first()
    if not maestro:
        raise HTTPException(status_code=403, detail="Sin perfil de maestro")

    asig = (
        db.query(MaestroMateria)
        .filter(
            MaestroMateria.id == asignacion_id,
            MaestroMateria.maestro_id == maestro.id,
        )
        .first()
    )
    if not asig:
        raise HTTPException(status_code=404, detail="Asignación no encontrada")

    semanas = (asig.periodo.semanas if asig.periodo else 14) or 14
    plan_map = {p.semana: p for p in asig.planeaciones}

    return {
        "asignacion_id": asig.id,
        "materia": asig.materia.nombre,
        "clave": asig.materia.clave,
        "grupo": asig.grupo or "—",
        "periodo": asig.periodo.nombre if asig.periodo else None,
        "semanas": semanas,
        "planeaciones": [
            {
                "semana": s,
                "titulo": plan_map[s].titulo if s in plan_map else None,
                "contenido": plan_map[s].contenido if s in plan_map else None,
                "updated_at": plan_map[s].updated_at.isoformat() if s in plan_map and plan_map[s].updated_at else None,
            }
            for s in range(1, semanas + 1)
        ],
    }


# ─── Guardar / actualizar planeación de una semana ───────────────────────────

@router.put("/planeaciones/{asignacion_id}/{semana}")
def upsert_planeacion(
    asignacion_id: int,
    semana: int,
    data: PlaneacionUpsert,
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(maestro_only),
):
    maestro = db.query(Maestro).filter(Maestro.user_id == current_user.id).first()
    if not maestro:
        raise HTTPException(status_code=403, detail="Sin perfil de maestro")

    asig = (
        db.query(MaestroMateria)
        .filter(
            MaestroMateria.id == asignacion_id,
            MaestroMateria.maestro_id == maestro.id,
        )
        .first()
    )
    if not asig:
        raise HTTPException(status_code=404, detail="Asignación no encontrada")

    plan = (
        db.query(Planeacion)
        .filter(Planeacion.asignacion_id == asignacion_id, Planeacion.semana == semana)
        .first()
    )

    if plan:
        plan.titulo = data.titulo
        plan.contenido = data.contenido
        plan.updated_at = datetime.utcnow()
    else:
        plan = Planeacion(
            asignacion_id=asignacion_id,
            semana=semana,
            titulo=data.titulo,
            contenido=data.contenido,
        )
        db.add(plan)

    db.commit()
    return {"ok": True, "semana": semana}


# ─── Rutas existentes (tareas + calificaciones) ───────────────────────────────

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

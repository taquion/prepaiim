from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import Optional

from app.core.database import get_db
from app.api.deps import require_role
from app.models.user import User, RoleEnum
from app.models.maestro import Maestro, Materia, MaestroMateria, Periodo

router = APIRouter(prefix="/admin", tags=["admin-planeaciones"])
admin_only = require_role(RoleEnum.admin)


# ─── Pydantic schemas ─────────────────────────────────────────────────────────
from pydantic import BaseModel


class PeriodoCreate(BaseModel):
    nombre: str
    semanas: int = 14


class MateriaCreate(BaseModel):
    nombre: str
    clave: Optional[str] = None
    descripcion: Optional[str] = None


class AsignacionCreate(BaseModel):
    maestro_id: int
    materia_id: int
    periodo_id: int
    grupo: str


# ─── Periodos ─────────────────────────────────────────────────────────────────

@router.get("/periodos")
def get_periodos(db: Session = Depends(get_db), _=Depends(admin_only)):
    periodos = db.query(Periodo).order_by(Periodo.id.desc()).all()
    return [
        {"id": p.id, "nombre": p.nombre, "semanas": p.semanas, "activo": p.activo}
        for p in periodos
    ]


@router.post("/periodos")
def create_periodo(data: PeriodoCreate, db: Session = Depends(get_db), _=Depends(admin_only)):
    p = Periodo(nombre=data.nombre, semanas=data.semanas, activo=False)
    db.add(p)
    db.commit()
    db.refresh(p)
    return {"id": p.id, "nombre": p.nombre, "semanas": p.semanas, "activo": p.activo}


@router.put("/periodos/{pid}/activar")
def activar_periodo(pid: int, db: Session = Depends(get_db), _=Depends(admin_only)):
    db.query(Periodo).update({"activo": False})
    p = db.query(Periodo).filter(Periodo.id == pid).first()
    if not p:
        raise HTTPException(status_code=404, detail="Periodo no encontrado")
    p.activo = True
    db.commit()
    return {"ok": True, "activo_id": pid}


@router.delete("/periodos/{pid}")
def delete_periodo(pid: int, db: Session = Depends(get_db), _=Depends(admin_only)):
    p = db.query(Periodo).filter(Periodo.id == pid).first()
    if not p:
        raise HTTPException(status_code=404, detail="Periodo no encontrado")
    db.delete(p)
    db.commit()
    return {"ok": True}


# ─── Materias ─────────────────────────────────────────────────────────────────

@router.get("/materias")
def get_materias(db: Session = Depends(get_db), _=Depends(admin_only)):
    materias = db.query(Materia).order_by(Materia.nombre).all()
    return [
        {"id": m.id, "nombre": m.nombre, "clave": m.clave, "descripcion": m.descripcion}
        for m in materias
    ]


@router.post("/materias")
def create_materia(data: MateriaCreate, db: Session = Depends(get_db), _=Depends(admin_only)):
    m = Materia(nombre=data.nombre, clave=data.clave, descripcion=data.descripcion)
    db.add(m)
    db.commit()
    db.refresh(m)
    return {"id": m.id, "nombre": m.nombre, "clave": m.clave}


@router.delete("/materias/{mid}")
def delete_materia(mid: int, db: Session = Depends(get_db), _=Depends(admin_only)):
    m = db.query(Materia).filter(Materia.id == mid).first()
    if not m:
        raise HTTPException(status_code=404, detail="Materia no encontrada")
    db.delete(m)
    db.commit()
    return {"ok": True}


# ─── Maestros list (para dropdown) ────────────────────────────────────────────

@router.get("/maestros-list")
def get_maestros_list(db: Session = Depends(get_db), _=Depends(admin_only)):
    maestros = db.query(Maestro).join(User).filter(User.activo == True).all()
    return [
        {
            "id": m.id,
            "user_id": m.user_id,
            "nombre": f"{m.user.nombre} {m.user.apellido}",
            "email": m.user.email,
        }
        for m in maestros
    ]


# ─── Asignaciones ─────────────────────────────────────────────────────────────

@router.get("/asignaciones")
def get_asignaciones(
    periodo_id: Optional[int] = None,
    db: Session = Depends(get_db),
    _=Depends(admin_only),
):
    q = db.query(MaestroMateria)
    if periodo_id:
        q = q.filter(MaestroMateria.periodo_id == periodo_id)
    asigs = q.all()
    return [
        {
            "id": a.id,
            "maestro": f"{a.maestro.user.nombre} {a.maestro.user.apellido}",
            "materia": a.materia.nombre,
            "materia_id": a.materia_id,
            "maestro_id": a.maestro_id,
            "grupo": a.grupo,
            "periodo_id": a.periodo_id,
            "periodo": a.periodo.nombre if a.periodo else None,
        }
        for a in asigs
        if a.maestro and a.materia
    ]


@router.post("/asignaciones")
def create_asignacion(data: AsignacionCreate, db: Session = Depends(get_db), _=Depends(admin_only)):
    a = MaestroMateria(
        maestro_id=data.maestro_id,
        materia_id=data.materia_id,
        periodo_id=data.periodo_id,
        grupo=data.grupo,
    )
    db.add(a)
    db.commit()
    db.refresh(a)
    return {"id": a.id, "ok": True}


@router.delete("/asignaciones/{aid}")
def delete_asignacion(aid: int, db: Session = Depends(get_db), _=Depends(admin_only)):
    a = db.query(MaestroMateria).filter(MaestroMateria.id == aid).first()
    if not a:
        raise HTTPException(status_code=404, detail="Asignación no encontrada")
    db.delete(a)
    db.commit()
    return {"ok": True}

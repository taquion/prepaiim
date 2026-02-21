from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.api.deps import require_role, get_current_user
from app.models.user import User, RoleEnum
from app.models.padre import Padre
from app.models.alumno import Alumno, Calificacion

router = APIRouter(prefix="/padre", tags=["padre"])
padre_only = require_role(RoleEnum.padre)


@router.get("/mis-hijos")
def get_hijos(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(padre_only),
):
    padre = db.query(Padre).filter(Padre.user_id == current_user.id).first()
    if not padre:
        return []
    return [
        {
            "id": pa.alumno_id,
            "nombre": pa.alumno.user.nombre,
            "apellido": pa.alumno.user.apellido,
            "matricula": pa.alumno.matricula,
            "tetramestre": pa.alumno.tetramestre_actual,
        }
        for pa in padre.hijos
    ]


def _check_padre_hijo(padre, alumno_id):
    hijo_ids = [pa.alumno_id for pa in padre.hijos]
    if alumno_id not in hijo_ids:
        raise HTTPException(status_code=403, detail="Sin permisos sobre este alumno")


@router.get("/hijo/{alumno_id}/calificaciones")
def get_calificaciones_hijo(
    alumno_id: int,
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(padre_only),
):
    padre = db.query(Padre).filter(Padre.user_id == current_user.id).first()
    _check_padre_hijo(padre, alumno_id)
    cals = db.query(Calificacion).filter(Calificacion.alumno_id == alumno_id).all()
    return [
        {"materia": c.materia.nombre, "tetramestre": c.tetramestre, "calificacion": c.calificacion}
        for c in cals
    ]


@router.get("/hijo/{alumno_id}/adeudos")
def get_adeudos_hijo(
    alumno_id: int,
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(padre_only),
):
    padre = db.query(Padre).filter(Padre.user_id == current_user.id).first()
    _check_padre_hijo(padre, alumno_id)
    alumno = db.query(Alumno).filter(Alumno.id == alumno_id).first()
    return [
        {
            "concepto": a.concepto,
            "monto": a.monto,
            "pagado": a.pagado,
            "pendiente": a.monto - a.pagado,
            "vencimiento": a.fecha_vencimiento,
        }
        for a in alumno.adeudos
    ]

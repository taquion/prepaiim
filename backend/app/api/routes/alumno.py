from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.api.deps import require_role, get_current_user
from app.models.user import User, RoleEnum
from app.models.alumno import Alumno, Calificacion, Adeudo

router = APIRouter(prefix="/alumno", tags=["alumno"])
alumno_only = require_role(RoleEnum.alumno)


@router.get("/mi-perfil")
def get_perfil(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(alumno_only),
):
    alumno = db.query(Alumno).filter(Alumno.user_id == current_user.id).first()
    return {
        "nombre": current_user.nombre,
        "apellido": current_user.apellido,
        "email": current_user.email,
        "matricula": alumno.matricula if alumno else None,
        "tetramestre": alumno.tetramestre_actual if alumno else None,
    }


@router.get("/calificaciones")
def get_calificaciones(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(alumno_only),
):
    alumno = db.query(Alumno).filter(Alumno.user_id == current_user.id).first()
    if not alumno:
        return []
    cals = db.query(Calificacion).filter(Calificacion.alumno_id == alumno.id).all()
    return [
        {"materia": c.materia.nombre, "tetramestre": c.tetramestre, "calificacion": c.calificacion}
        for c in cals
    ]


@router.get("/adeudos")
def get_adeudos(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db),
    _=Depends(alumno_only),
):
    alumno = db.query(Alumno).filter(Alumno.user_id == current_user.id).first()
    if not alumno:
        return []
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

import urllib.request
import urllib.parse
import json
import threading

from fastapi import APIRouter, Depends
from pydantic import BaseModel
from datetime import datetime
from sqlalchemy.orm import Session
from app.core.database import get_db
from app.models.inscripcion import Inscripcion
from app.api.deps import get_current_user
from app.models.user import User

router = APIRouter(tags=['inscripciones'])

# ── Telegram config ────────────────────────────────────────────────
TG_TOKEN   = '8382926934:AAF7wsOx5ACllUeW4Rlc2d2mGN4nBk-o7_s'
TG_CHAT_ID = '-5201794562'

def _send_tg(mensaje: str):
    """Envía mensaje a Telegram en background (no bloquea el endpoint)."""
    try:
        url  = f'https://api.telegram.org/bot{TG_TOKEN}/sendMessage'
        data = urllib.parse.urlencode({
            'chat_id': TG_CHAT_ID,
            'text':    mensaje,
            'parse_mode': 'HTML',
        }).encode()
        req = urllib.request.Request(url, data=data, method='POST')
        urllib.request.urlopen(req, timeout=5)
    except Exception:
        pass  # Nunca romper el endpoint por falla de notificación

def notify_tg(ins: 'Inscripcion'):
    msg = (
        '📋 <b>Nueva solicitud de inscripción</b>\n\n'
        f'👤 <b>Nombre:</b> {ins.nombre}\n'
        f'📱 <b>Teléfono:</b> {ins.telefono}\n'
        f'🏫 <b>Secundaria:</b> {ins.secundaria or "—"}\n'
        f'🕐 <b>Fecha:</b> {ins.created_at.strftime("%d/%m/%Y %H:%M") if ins.created_at else "ahora"}'
    )
    threading.Thread(target=_send_tg, args=(msg,), daemon=True).start()

# ── Schemas ────────────────────────────────────────────────────────

class InscripcionIn(BaseModel):
    nombre: str
    telefono: str
    secundaria: str

class InscripcionOut(InscripcionIn):
    id: int
    created_at: datetime | None = None

    class Config:
        from_attributes = True

# ── Endpoints ──────────────────────────────────────────────────────

@router.post('/inscripciones', response_model=InscripcionOut, status_code=201)
def crear_inscripcion(data: InscripcionIn, db: Session = Depends(get_db)):
    ins = Inscripcion(**data.model_dump())
    db.add(ins)
    db.commit()
    db.refresh(ins)
    notify_tg(ins)   # ← notificación Telegram en background
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

import urllib.request
import urllib.parse
import threading
import re

from fastapi import APIRouter, Depends
from pydantic import BaseModel
from datetime import datetime, timezone
from zoneinfo import ZoneInfo
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
    try:
        url  = f'https://api.telegram.org/bot{TG_TOKEN}/sendMessage'
        data = urllib.parse.urlencode({
            'chat_id':    TG_CHAT_ID,
            'text':       mensaje,
            'parse_mode': 'HTML',
        }).encode()
        req = urllib.request.Request(url, data=data, method='POST')
        urllib.request.urlopen(req, timeout=5)
    except Exception:
        pass

def notify_tg(ins):
    # Limpiar teléfono → URL WhatsApp
    digits = re.sub(r'\D', '', ins.telefono)
    if len(digits) == 10:
        digits = '52' + digits

    wa_msg = (
        f'Hola {ins.nombre}, muchas gracias por tu interes en el IIM, '
        f'actualmente tenemos inscripciones abiertas, '
        f'en que periodo estas pensando entrar, septiembre o enero?'
    )
    wa_link = f'https://wa.me/{digits}?text={urllib.parse.quote(wa_msg)}'

    sec = ins.secundaria if ins.secundaria else '-'
    mty = ZoneInfo('America/Monterrey')
    fecha_local = ins.created_at.replace(tzinfo=timezone.utc).astimezone(mty) if ins.created_at else None
    fecha = fecha_local.strftime('%d/%m/%Y %H:%M') + ' (MTY)' if fecha_local else 'ahora'

    msg = (
        '<b>Nueva solicitud de inscripcion</b>\n\n'
        f'Nombre: {ins.nombre}\n'
        f'Telefono: {ins.telefono}\n'
        f'Secundaria: {sec}\n'
        f'Fecha: {fecha}\n\n'
        f'<a href="{wa_link}">Responder por WhatsApp</a>'
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
    notify_tg(ins)
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

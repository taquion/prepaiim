"""
Prepa IIM Webmaster Bot — Telegram polling
Runs as a background daemon thread inside the FastAPI process.
"""
import os
import json
import time
import threading
import urllib.request
import urllib.parse
import urllib.error
from datetime import datetime
from zoneinfo import ZoneInfo

TG_TOKEN  = os.getenv("TG_TOKEN", "")
MX        = ZoneInfo("America/Monterrey")


def _api(method: str, **params) -> dict:
    if not TG_TOKEN:
        return {}
    url  = f"https://api.telegram.org/bot{TG_TOKEN}/{method}"
    data = urllib.parse.urlencode(params).encode()
    try:
        with urllib.request.urlopen(url, data=data, timeout=35) as r:
            return json.loads(r.read())
    except Exception:
        return {}


def send(chat_id: int | str, text: str) -> None:
    _api("sendMessage", chat_id=chat_id, text=text, parse_mode="HTML")


def _now() -> str:
    return datetime.now(MX).strftime("%d/%m/%Y %H:%M")


HELP = (
    "🏫 <b>Prepa IIM Webmaster Bot</b>\n\n"
    "Comandos disponibles:\n"
    "/status  — Estado del sistema\n"
    "/hora    — Hora actual en Monterrey\n"
    "/ayuda   — Este menú\n\n"
    "También recibo notificaciones automáticas de nuevas inscripciones."
)


def _handle(update: dict) -> None:
    msg     = update.get("message") or update.get("edited_message", {})
    text    = (msg.get("text") or "").strip()
    chat_id = (msg.get("chat") or {}).get("id")
    if not chat_id or not text:
        return

    lower = text.lower()

    if any(lower.startswith(cmd) for cmd in ("/start", "/hola", "/hello")):
        send(chat_id, f"👋 Hola! Soy el bot de Prepa IIM Webmaster.\n\n{HELP}")

    elif lower.startswith("/status"):
        send(chat_id,
             f"✅ <b>Sistema activo</b>\n"
             f"🕐 {_now()} (Monterrey)\n"
             f"Portal IIM funcionando correctamente.")

    elif lower.startswith("/hora"):
        send(chat_id, f"🕐 {_now()} (Monterrey)")

    elif any(lower.startswith(cmd) for cmd in ("/ayuda", "/help")):
        send(chat_id, HELP)

    else:
        send(chat_id,
             f"No reconozco ese comando.\n\n{HELP}")


def _poll_loop() -> None:
    if not TG_TOKEN:
        print("[bot] TG_TOKEN no configurado — polling desactivado")
        return

    print("[bot] Iniciando polling de Telegram...")
    offset = 0
    while True:
        try:
            url = (
                f"https://api.telegram.org/bot{TG_TOKEN}/getUpdates"
                f"?offset={offset}&timeout=30&allowed_updates=message"
            )
            with urllib.request.urlopen(url, timeout=35) as r:
                data = json.loads(r.read())
            for upd in data.get("result", []):
                offset = upd["update_id"] + 1
                try:
                    _handle(upd)
                except Exception as e:
                    print(f"[bot] Error manejando update: {e}")
        except urllib.error.URLError as e:
            print(f"[bot] Network error: {e} — reintentando en 10s")
            time.sleep(10)
        except Exception as e:
            print(f"[bot] Error inesperado: {e} — reintentando en 5s")
            time.sleep(5)


def start_polling() -> None:
    """Call once at app startup to begin background polling."""
    t = threading.Thread(target=_poll_loop, name="tg-bot-poll", daemon=True)
    t.start()

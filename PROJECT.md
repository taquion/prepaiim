# 📋 Portal IIM — Estado del Proyecto

> Instituto Intercultural Monterrey, Pte.  
> Stack: FastAPI + PostgreSQL + React + Vite + Tailwind CSS  
> VPS: `64.225.115.133` · Repo: `github.com/taquion/prepaiim`

---

## ✅ Completado

### Infraestructura
- [x] VPS Ubuntu 24.04 configurado
- [x] PostgreSQL instalado y corriendo
- [x] Base de datos `iimdb` + usuario `iim` creados
- [x] Nginx instalado y configurado como reverse proxy
- [x] Backend como servicio systemd (auto-restart)
- [x] Repo en GitHub (`taquion/prepaiim`)

### Backend (FastAPI)
- [x] Autenticación JWT con roles
- [x] Modelos: User, Alumno, Maestro, Padre, Materia, Tarea, Calificación, Adeudo
- [x] API REST por rol: admin / maestro / alumno / padre
- [x] Seed inicial con usuarios de prueba
- [x] CORS configurado

### Frontend (React)
- [x] Landing pública con identidad IIM (azul `#1B4F8A` / dorado `#C8A84B`)
- [x] Login con redirección por rol
- [x] Dashboard Admin — tabla de alumnos + adeudos
- [x] Dashboard Maestro — materias asignadas + acciones rápidas
- [x] Dashboard Alumno — calificaciones + estado de cuenta
- [x] Dashboard Padre — info de hijos + calificaciones + adeudos
- [x] Build de producción desplegado

---

## 🔲 Pendientes

### 🔴 Alta prioridad

- [ ] **Dominio + SSL** — Configurar dominio real en nginx + certbot (Let's Encrypt)
- [ ] **OTP WhatsApp para login** — Alumnos y padres entran con número de teléfono + código por WhatsApp (sin contraseña)
  - [ ] Elegir proveedor: Twilio / Meta Cloud API / WAHA
  - [ ] Agregar campo `phone` a User (alumno/padre)
  - [ ] Tabla `otp_tokens` (phone, code, expires_at, used)
  - [ ] Endpoints: `POST /api/auth/request-otp` y `POST /api/auth/verify-otp`
  - [ ] Rediseñar pantalla de login (tab alumno/padre vs tab maestro/admin)
- [ ] **Replicar proceso de Google Sheets en Admin** — Analizar hoja actual y mapear flujos al panel administrativo

### 🟡 Media prioridad

- [ ] **Módulo de becas** — Solicitudes, aprobación, seguimiento
- [ ] **Gestión de pagos** — Registrar abonos a adeudos, historial de pagos
- [ ] **Subida de contenido (Maestro)** — Archivos, material de clase, exámenes (storage S3 o local)
- [ ] **Tareas en línea (Alumno)** — Ver, entregar y recibir calificación de tarea
- [ ] **Exámenes en línea** — Crear, aplicar y calificar exámenes desde el portal
- [ ] **Notificaciones internas** — Avisos de admin → alumnos/padres dentro del portal
- [ ] **Gestión de grupos y tetramestres** — Asignar alumnos a grupos, avanzar tetramestre

### 🟢 Futuro / WhatsApp

- [ ] **Comunicación WhatsApp masiva** — Avisos de admin a padres/alumnos vía WhatsApp
- [ ] **Recordatorios automáticos de pago** — Cron que notifica adeudos próximos a vencer
- [ ] **Calificaciones por WhatsApp** — Padre recibe notificación cuando se publican notas
- [ ] **Bot conversacional** — Consultas básicas por WhatsApp (saldo, materias, calificaciones)

### 🔧 Técnico / DevOps

- [ ] **Variables de entorno seguras** — Mover secrets a vault o .env fuera del repo
- [ ] **Cambiar contraseñas default** del seed (admin, maestro, alumno, padre)
- [ ] **Backups automáticos** — Cron de pg_dump diario
- [ ] **Logs centralizados** — Configurar journald o enviar logs a servicio externo
- [ ] **CI/CD** — GitHub Actions para auto-deploy al pushear a `main`
- [ ] **Tests** — Unit tests para rutas críticas (auth, pagos, calificaciones)
- [ ] **Rate limiting en API** — Proteger endpoints de OTP contra abuso
- [ ] **Migración con Alembic** — Reemplazar `create_all` por migraciones versionadas

---

## 🔑 Credenciales de prueba (cambiar en producción)

| Rol | Email | Contraseña |
|-----|-------|------------|
| 🏛️ Admin | admin@iim.edu.mx | admin123 |
| 👨‍🏫 Maestro | maestra.garcia@iim.edu.mx | maestra123 |
| 🎓 Alumno | alumno.perez@iim.edu.mx | alumno123 |
| 👨‍👩‍👧 Padre | padre.perez@gmail.com | padre123 |

---

## 🌐 URLs

| Recurso | URL |
|---------|-----|
| Portal (prod) | http://64.225.115.133 |
| API docs | http://64.225.115.133/docs |
| GitHub | https://github.com/taquion/prepaiim |

---

## 📁 Estructura del repo

```
prepaiim/
├── backend/
│   ├── app/
│   │   ├── api/routes/     # auth, admin, maestro, alumno, padre
│   │   ├── core/           # config, security, database
│   │   ├── models/         # SQLAlchemy: user, alumno, maestro, padre
│   │   └── schemas/        # Pydantic schemas
│   ├── scripts/seed.py
│   ├── main.py
│   └── requirements.txt
├── frontend/
│   └── src/
│       ├── components/     # DashboardLayout
│       ├── contexts/       # AuthContext (JWT)
│       └── pages/          # Landing, Login, dashboards/
├── nginx/iim.conf
├── scripts/
│   ├── deploy.sh
│   └── prepaiim-backend.service
├── PROJECT.md              ← este archivo
└── README.md
```

---

_Última actualización: 2026-02-21_

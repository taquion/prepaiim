# Portal IIM — Instituto Intercultural Monterrey

Sistema escolar integral: landing pública + portal multi-rol.

## Stack
- **Backend:** FastAPI (Python) + PostgreSQL + JWT
- **Frontend:** React + Vite + Tailwind CSS
- **Deploy:** Nginx + systemd en VPS Ubuntu

## Roles
| Rol | Acceso |
|-----|--------|
| 🏛️ Admin | Alumnos, pagos, adeudos, becas |
| 👨‍🏫 Maestro | Materias, tareas, calificaciones |
| 🎓 Alumno | Mi avance, tareas, estado de cuenta |
| 👨‍👩‍👧 Padre | Calificaciones e adeudos de hijos |

## Colores institucionales
- Azul principal: `#1B4F8A`
- Dorado: `#C8A84B`
- Teal: `#2A9D8F`

---

## Setup local

### 1. Base de datos (PostgreSQL)
```bash
sudo -u postgres psql
CREATE DATABASE iimdb;
CREATE USER iim WITH PASSWORD 'iimpass';
GRANT ALL PRIVILEGES ON DATABASE iimdb TO iim;
\q
```

### 2. Backend
```bash
cd backend
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt

# Configurar variables de entorno
cp .env.example .env
# Editar .env con tus valores

# Crear tablas y seed inicial
python scripts/seed.py

# Correr servidor
uvicorn main:app --reload --port 8000
```

API docs: http://localhost:8000/docs

### 3. Frontend
```bash
cd frontend
npm install
cp .env.example .env
# VITE_API_URL=http://localhost:8000

npm run dev
```

App: http://localhost:5173

---

## Credenciales de prueba (seed)
| Rol | Email | Contraseña |
|-----|-------|------------|
| 🏛️ Admin | admin@iim.edu.mx | admin123 |
| 👨‍🏫 Maestro | maestra.garcia@iim.edu.mx | maestra123 |
| 🎓 Alumno | alumno.perez@iim.edu.mx | alumno123 |
| 👨‍👩‍👧 Padre | padre.perez@gmail.com | padre123 |

> ⚠️ **Cambiar todas las contraseñas en producción**

---

## Deploy en VPS

### Requisitos previos
```bash
apt install -y python3 python3-venv python3-pip nodejs npm nginx postgresql
```

### Ejecutar deploy
```bash
# Configurar archivos .env en /var/www/prepaiim/backend/.env
# y /var/www/prepaiim/frontend/.env

chmod +x scripts/deploy.sh
sudo bash scripts/deploy.sh
```

---

## Estructura del proyecto
```
prepaiim/
├── backend/
│   ├── app/
│   │   ├── api/routes/   # auth, admin, maestro, alumno, padre
│   │   ├── core/         # config, security, database
│   │   ├── models/       # SQLAlchemy models
│   │   └── schemas/      # Pydantic schemas
│   ├── scripts/seed.py
│   ├── main.py
│   └── requirements.txt
├── frontend/
│   ├── src/
│   │   ├── components/   # DashboardLayout
│   │   ├── contexts/     # AuthContext
│   │   └── pages/        # Landing, Login, dashboards/
│   └── package.json
├── nginx/iim.conf
├── scripts/
│   ├── deploy.sh
│   └── prepaiim-backend.service
└── README.md
```

---

## Roadmap
- [ ] WhatsApp integration (comunicación padres/alumnos)
- [ ] Módulo de becas
- [ ] Carga de documentos
- [ ] Notificaciones push
- [ ] Exámenes en línea

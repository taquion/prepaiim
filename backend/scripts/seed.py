"""
Seed inicial: crea usuarios de prueba para cada rol.
Ejecutar desde backend/: python scripts/seed.py
"""
import sys
sys.path.insert(0, ".")

from app.core.database import SessionLocal, Base, engine
from app.core.security import get_password_hash
from app.models.user import User, RoleEnum
from app.models.alumno import Alumno, Adeudo
from app.models.maestro import Maestro, Materia, MaestroMateria
from app.models.padre import Padre, PadreAlumno

# Importar todos los modelos para que SQLAlchemy los registre
from app.models import alumno as am, maestro as mm, padre as pm  # noqa

Base.metadata.create_all(bind=engine)
db = SessionLocal()


def create_user_if_not_exists(email, nombre, apellido, password, role):
    existing = db.query(User).filter(User.email == email).first()
    if existing:
        print(f"  ⚠️  Ya existe: {email}")
        return existing
    user = User(
        email=email,
        nombre=nombre,
        apellido=apellido,
        hashed_password=get_password_hash(password),
        role=role,
    )
    db.add(user)
    db.commit()
    db.refresh(user)
    print(f"  ✅ Creado: {email} ({role})")
    return user


print("\n🌱 Seed Portal IIM\n")

# Admin
admin = create_user_if_not_exists("admin@iim.edu.mx", "Admin", "IIM", "admin123", RoleEnum.admin)

# Maestro
maestro_user = create_user_if_not_exists("maestra.garcia@iim.edu.mx", "Laura", "García", "maestra123", RoleEnum.maestro)
maestro = db.query(Maestro).filter(Maestro.user_id == maestro_user.id).first()
if not maestro:
    maestro = Maestro(user_id=maestro_user.id, clave="M001", especialidad="Matemáticas")
    db.add(maestro)
    db.commit()
    db.refresh(maestro)

# Materias
materias_data = [
    ("Matemáticas I", 1), ("Español I", 1), ("Física I", 2),
    ("Historia Universal", 2), ("Química I", 3), ("Inglés I", 1),
]
materias = []
for nombre, tet in materias_data:
    m = db.query(Materia).filter(Materia.nombre == nombre).first()
    if not m:
        m = Materia(nombre=nombre, tetramestre=tet)
        db.add(m)
        db.commit()
        db.refresh(m)
    materias.append(m)

# Asignar primera materia al maestro
mm_existing = db.query(MaestroMateria).filter(
    MaestroMateria.maestro_id == maestro.id,
    MaestroMateria.materia_id == materias[0].id,
).first()
if not mm_existing:
    db.add(MaestroMateria(maestro_id=maestro.id, materia_id=materias[0].id, grupo="A"))
    db.commit()

# Alumno
alumno_user = create_user_if_not_exists("alumno.perez@iim.edu.mx", "Carlos", "Pérez", "alumno123", RoleEnum.alumno)
alumno = db.query(Alumno).filter(Alumno.user_id == alumno_user.id).first()
if not alumno:
    alumno = Alumno(user_id=alumno_user.id, matricula="IIM-2024-001", tetramestre_actual=1, grupo="A")
    db.add(alumno)
    db.commit()
    db.refresh(alumno)

# Padre
padre_user = create_user_if_not_exists("padre.perez@gmail.com", "Roberto", "Pérez", "padre123", RoleEnum.padre)
padre = db.query(Padre).filter(Padre.user_id == padre_user.id).first()
if not padre:
    padre = Padre(user_id=padre_user.id)
    db.add(padre)
    db.commit()
    db.refresh(padre)

pa_existing = db.query(PadreAlumno).filter(
    PadreAlumno.padre_id == padre.id,
    PadreAlumno.alumno_id == alumno.id,
).first()
if not pa_existing:
    db.add(PadreAlumno(padre_id=padre.id, alumno_id=alumno.id))
    db.commit()

db.close()

print("\n✅ Seed completo!\n")
print("Credenciales de acceso:")
print("  🏛️  Admin   : admin@iim.edu.mx        / admin123")
print("  👨‍🏫 Maestro : maestra.garcia@iim.edu.mx / maestra123")
print("  🎓 Alumno  : alumno.perez@iim.edu.mx  / alumno123")
print("  👨‍👩‍👧 Padre   : padre.perez@gmail.com   / padre123")
print()

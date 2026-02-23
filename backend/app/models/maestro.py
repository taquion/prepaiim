from sqlalchemy import Column, Integer, String, ForeignKey, DateTime, Text, Boolean, Float, UniqueConstraint
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Periodo(Base):
    __tablename__ = "periodos"
    id = Column(Integer, primary_key=True, index=True)
    nombre = Column(String, nullable=False)       # "Ene-Abr 2026"
    semanas = Column(Integer, default=14)
    activo = Column(Boolean, default=False)
    created_at = Column(DateTime, default=datetime.utcnow)

    asignaciones = relationship("MaestroMateria", back_populates="periodo")


class Maestro(Base):
    __tablename__ = "maestros"
    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(Integer, ForeignKey("users.id"), unique=True)
    clave = Column(String, unique=True, nullable=True)
    especialidad = Column(String, nullable=True)

    user = relationship("User", back_populates="maestro_profile")
    materias = relationship("MaestroMateria", back_populates="maestro")


class Materia(Base):
    __tablename__ = "materias"
    id = Column(Integer, primary_key=True, index=True)
    nombre = Column(String, nullable=False)
    clave = Column(String, nullable=True)
    descripcion = Column(Text, nullable=True)
    tetramestre = Column(Integer, nullable=True)

    maestros = relationship("MaestroMateria", back_populates="materia")
    tareas = relationship("Tarea", back_populates="materia")
    calificaciones = relationship("Calificacion", back_populates="materia")


class MaestroMateria(Base):
    __tablename__ = "maestro_materias"
    id = Column(Integer, primary_key=True, index=True)
    maestro_id = Column(Integer, ForeignKey("maestros.id"))
    materia_id = Column(Integer, ForeignKey("materias.id"))
    periodo_id = Column(Integer, ForeignKey("periodos.id"), nullable=True)
    grupo = Column(String, nullable=True)

    maestro = relationship("Maestro", back_populates="materias")
    materia = relationship("Materia", back_populates="maestros")
    periodo = relationship("Periodo", back_populates="asignaciones")
    planeaciones = relationship("Planeacion", back_populates="asignacion", cascade="all, delete-orphan")


class Planeacion(Base):
    __tablename__ = "planeaciones"
    id = Column(Integer, primary_key=True, index=True)
    asignacion_id = Column(Integer, ForeignKey("maestro_materias.id"), nullable=False)
    semana = Column(Integer, nullable=False)     # 1-14
    titulo = Column(String, nullable=True)
    contenido = Column(Text, nullable=True)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    asignacion = relationship("MaestroMateria", back_populates="planeaciones")

    __table_args__ = (
        UniqueConstraint("asignacion_id", "semana", name="uq_planeacion_asig_semana"),
    )


class Tarea(Base):
    __tablename__ = "tareas"
    id = Column(Integer, primary_key=True, index=True)
    materia_id = Column(Integer, ForeignKey("materias.id"))
    titulo = Column(String, nullable=False)
    descripcion = Column(Text)
    fecha_entrega = Column(DateTime)
    created_at = Column(DateTime, default=datetime.utcnow)

    materia = relationship("Materia", back_populates="tareas")
    entregas = relationship("TareaAlumno", back_populates="tarea")


class TareaAlumno(Base):
    __tablename__ = "tarea_alumnos"
    id = Column(Integer, primary_key=True, index=True)
    tarea_id = Column(Integer, ForeignKey("tareas.id"))
    alumno_id = Column(Integer, ForeignKey("alumnos.id"))
    entregado = Column(Boolean, default=False)
    calificacion = Column(Float)
    comentario = Column(Text)
    entregado_at = Column(DateTime)

    tarea = relationship("Tarea", back_populates="entregas")
    alumno = relationship("Alumno", back_populates="tareas")

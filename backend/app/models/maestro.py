from sqlalchemy import Column, Integer, String, ForeignKey, DateTime, Text, Boolean, Float
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Maestro(Base):
    __tablename__ = "maestros"
    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(Integer, ForeignKey("users.id"), unique=True)
    clave = Column(String, unique=True)
    especialidad = Column(String)

    user = relationship("User", back_populates="maestro_profile")
    materias = relationship("MaestroMateria", back_populates="maestro")


class Materia(Base):
    __tablename__ = "materias"
    id = Column(Integer, primary_key=True, index=True)
    nombre = Column(String, nullable=False)
    tetramestre = Column(Integer)
    descripcion = Column(Text)

    maestros = relationship("MaestroMateria", back_populates="materia")
    tareas = relationship("Tarea", back_populates="materia")
    calificaciones = relationship("Calificacion", back_populates="materia")


class MaestroMateria(Base):
    __tablename__ = "maestro_materias"
    id = Column(Integer, primary_key=True, index=True)
    maestro_id = Column(Integer, ForeignKey("maestros.id"))
    materia_id = Column(Integer, ForeignKey("materias.id"))
    grupo = Column(String)

    maestro = relationship("Maestro", back_populates="materias")
    materia = relationship("Materia", back_populates="maestros")


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

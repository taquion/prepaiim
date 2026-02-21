from sqlalchemy import Column, Integer, String, Float, ForeignKey, DateTime
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Alumno(Base):
    __tablename__ = "alumnos"
    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(Integer, ForeignKey("users.id"), unique=True)
    matricula = Column(String, unique=True, index=True)
    tetramestre_actual = Column(Integer, default=1)
    grupo = Column(String)

    user = relationship("User", back_populates="alumno_profile")
    calificaciones = relationship("Calificacion", back_populates="alumno")
    tareas = relationship("TareaAlumno", back_populates="alumno")
    adeudos = relationship("Adeudo", back_populates="alumno")


class Calificacion(Base):
    __tablename__ = "calificaciones"
    id = Column(Integer, primary_key=True, index=True)
    alumno_id = Column(Integer, ForeignKey("alumnos.id"))
    materia_id = Column(Integer, ForeignKey("materias.id"))
    tetramestre = Column(Integer)
    calificacion = Column(Float)

    alumno = relationship("Alumno", back_populates="calificaciones")
    materia = relationship("Materia", back_populates="calificaciones")


class Adeudo(Base):
    __tablename__ = "adeudos"
    id = Column(Integer, primary_key=True, index=True)
    alumno_id = Column(Integer, ForeignKey("alumnos.id"))
    concepto = Column(String)
    monto = Column(Float)
    pagado = Column(Float, default=0)
    fecha_vencimiento = Column(DateTime)
    created_at = Column(DateTime, default=datetime.utcnow)

    alumno = relationship("Alumno", back_populates="adeudos")

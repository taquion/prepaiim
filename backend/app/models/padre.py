from sqlalchemy import Column, Integer, ForeignKey
from sqlalchemy.orm import relationship
from app.core.database import Base


class Padre(Base):
    __tablename__ = "padres"
    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(Integer, ForeignKey("users.id"), unique=True)

    user = relationship("User", back_populates="padre_profile")
    hijos = relationship("PadreAlumno", back_populates="padre")


class PadreAlumno(Base):
    __tablename__ = "padre_alumnos"
    id = Column(Integer, primary_key=True, index=True)
    padre_id = Column(Integer, ForeignKey("padres.id"))
    alumno_id = Column(Integer, ForeignKey("alumnos.id"))

    padre = relationship("Padre", back_populates="hijos")
    alumno = relationship("Alumno")
